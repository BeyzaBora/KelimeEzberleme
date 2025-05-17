using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using KelimeEzberleme.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KelimeEzberleme.Controllers
{
    public class RaporController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RaporController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var progresses = _context.WordProgresses
                                     .Include(wp => wp.Word)
                                     .ThenInclude(w => w.Kategori)
                                     .Where(wp => wp.UserID == userId)
                                     .ToList();

            // Genel toplamlar
            int toplamDogru = progresses.Sum(wp => wp.CorrectCount);
            int toplamYanlis = progresses.Sum(wp => wp.IncorrectCount);
            int ogrenilen = progresses.Count(wp => wp.CorrectCount >= 6);
            double basariOrani = (toplamDogru + toplamYanlis) == 0
                ? 0
                : (double)toplamDogru / (toplamDogru + toplamYanlis) * 100;

            // Kategori bazlı istatistik
            var kategoriGruplari = progresses
                .Where(p => p.Word.Kategori != null)
                .GroupBy(p => p.Word.Kategori.KategoriAd)
                .Select(grp => new KategoriBazliIstatistik
                {
                    KategoriAdi = grp.Key,
                    DogruSayisi = grp.Sum(p => p.CorrectCount),
                    YanlisSayisi = grp.Sum(p => p.IncorrectCount),
                    BasariOrani = (grp.Sum(p => p.CorrectCount) + grp.Sum(p => p.IncorrectCount)) == 0
                        ? 0
                        : (double)grp.Sum(p => p.CorrectCount) / (grp.Sum(p => p.CorrectCount) + grp.Sum(p => p.IncorrectCount)) * 100
                })
                .ToList();

            var viewModel = new RaporViewModel
            {
                ToplamDogruCevapSayisi = toplamDogru,
                ToplamYanlisCevapSayisi = toplamYanlis,
                OgrenilenKelimeSayisi = ogrenilen,
                BasariOrani = basariOrani,
                KategoriIstatistikleri = kategoriGruplari
            };

            return View(viewModel);
        }
    }
}


