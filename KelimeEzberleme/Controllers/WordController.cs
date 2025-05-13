using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using KelimeEzberleme.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KelimeEzberleme.Controllers
{
    public class WordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public WordController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(WordAddViewModel model)
        {
            // Giriş kontrolü: Session'da kullanıcı yoksa login sayfasına at
            if (HttpContext.Session.GetInt32("KullaniciID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                string imagePath = null!;
                if (model.Image != null)
                {
                    var folder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(folder);
                    var fileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
                    var fullPath = Path.Combine(folder, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        model.Image.CopyTo(stream);
                    }
                    imagePath = "/uploads/" + fileName;
                }

                var word = new Word
                {
                    EngWordName = model.EngWordName,
                    TurWordName = model.TurWordName,
                    Picture = imagePath
                };

                foreach (var sample in model.Samples)
                {
                    word.Samples.Add(new WordSample
                    {
                        Samples = sample
                    });
                }

                _context.Words.Add(word);
                _context.SaveChanges();

                // Başarı mesajı
                TempData["SuccessMessage"] = "Yeni kelime başarıyla eklendi.";

                return RedirectToAction("Add");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult List()
        {
            var words = _context.Words
                .Include(w => w.Samples)
                .ToList();

            return View(words);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var word = _context.Words.Include(w => w.Samples).FirstOrDefault(w => w.WordID == id);
            if (word == null) return NotFound();

            var model = new WordAddViewModel
            {
                EngWordName = word.EngWordName,
                TurWordName = word.TurWordName,
                Samples = word.Samples.Select(s => s.Samples).ToList()
            };

            ViewBag.WordID = word.WordID;
            ViewBag.ExistingImage = word.Picture;

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, WordAddViewModel model)
        {
            var word = _context.Words.Include(w => w.Samples).FirstOrDefault(w => w.WordID == id);
            if (word == null) return NotFound();

            word.EngWordName = model.EngWordName;
            word.TurWordName = model.TurWordName;

            // Yeni resim yüklendiyse değiştir
            if (model.Image != null)
            {
                var folder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(folder);
                var fileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
                var fullPath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    model.Image.CopyTo(stream);
                }
                word.Picture = "/uploads/" + fileName;
            }

            // Önce eski örnekleri sil
            _context.WordSamples.RemoveRange(word.Samples);

            // Yeni örnek cümleleri ekle
            word.Samples = model.Samples
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => new WordSample { Samples = s, WordID = word.WordID })
                .ToList();

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Kelime güncellendi.";
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var word = _context.Words.Include(w => w.Samples).FirstOrDefault(w => w.WordID == id);
            if (word == null) return NotFound();

            _context.WordSamples.RemoveRange(word.Samples);
            _context.Words.Remove(word);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Kelime silindi.";
            return RedirectToAction("List");
        }


    }
}
