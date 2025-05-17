using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using KelimeEzberleme.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var model = new WordAddViewModel
            {
                CategoryList = _context.Kategoriler.Select(c => new SelectListItem
                {
                    Value = c.KategoriID.ToString(),
                    Text = c.KategoriAd
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(WordAddViewModel model)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kategori listesini her durumda doldur (validation başarısızsa view'da lazım)
            model.CategoryList = _context.Kategoriler.Select(c => new SelectListItem
            {
                Value = c.KategoriID.ToString(),
                Text = c.KategoriAd
            }).ToList();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Debug"] = string.Join(" | ", errors);
                return View(model);
            }

            string? imagePath = null;

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
                Picture = imagePath,
                KategoriID = model.SelectedCategoryID,
                Samples = model.Samples
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => new WordSample
                    {
                        Samples = s,
                        KategoriID = model.SelectedCategoryID
                    }).ToList()
            };

            try
            {
                _context.Words.Add(word);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Debug"] = "DB Hatası: " + ex.Message;
                return View(model);
            }

            TempData["SuccessMessage"] = "Yeni kelime başarıyla eklendi.";
            return RedirectToAction("Add");
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
                WordID = word.WordID,
                EngWordName = word.EngWordName,
                TurWordName = word.TurWordName,
                Samples = word.Samples.Select(s => s.Samples).ToList(),
                CategoryList = _context.Kategoriler.Select(c => new SelectListItem
                {
                    Value = c.KategoriID.ToString(),
                    Text = c.KategoriAd
                }).ToList(),
                SelectedCategoryID = word.KategoriID ?? 0
            };

            ViewBag.WordID = word.WordID;
            ViewBag.ExistingImage = word.Picture;

            return View(model);
        }

        [HttpPost]
        public IActionResult EditPost(int id, WordAddViewModel model)
        {
            var word = _context.Words.Include(w => w.Samples).FirstOrDefault(w => w.WordID == id);
            if (word == null) return NotFound();

            word.EngWordName = model.EngWordName;
            word.TurWordName = model.TurWordName;

            if (model.Image != null)
            {
                var folder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(folder);
                var fileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
                var fullPath = Path.Combine(folder, fileName);
                using (var FileStream = new FileStream(fullPath, FileMode.Create))
                {
                    model.Image.CopyTo(FileStream);
                }
                word.Picture = "/uploads/" + fileName;
            }

            _context.WordSamples.RemoveRange(word.Samples);

            word.Samples = model.Samples
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => new WordSample
                {
                    Samples = s,
                    WordID = word.WordID,
                    KategoriID = model.SelectedCategoryID
                }).ToList();

            word.KategoriID = model.SelectedCategoryID;

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
