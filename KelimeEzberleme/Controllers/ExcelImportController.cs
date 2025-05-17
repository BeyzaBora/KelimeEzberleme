using ClosedXML.Excel;
using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KelimeEzberleme.Controllers
{
    public class ExcelImportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExcelImportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ImportFromExcel()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Kelime.xlsx");

            if (!System.IO.File.Exists(filePath))
            {
                return Content("Excel dosyası bulunamadı. Lütfen 'wwwroot/uploads' klasörüne 'Kelime.xlsx' dosyasını ekleyin.");
            }

            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                var eng = row.Cell(1).GetString();
                var tur = row.Cell(2).GetString();
                var imageName = row.Cell(3).GetString();
                var sample1 = row.Cell(4).GetString();
                var sample2 = row.Cell(5).GetString();
                var kategoriIdCell = row.Cell(6).GetString();

                if (!int.TryParse(kategoriIdCell, out int kategoriID))
                {
                    return Content($"Geçersiz kategori ID: '{kategoriIdCell}'");
                }

                var kategori = _context.Kategoriler.FirstOrDefault(k => k.KategoriID == kategoriID);
                if (kategori == null)
                {
                    return Content($"Kategori ID '{kategoriID}' veritabanında bulunamadı.");
                }

                // Var olan kelime kontrolü (güncellemek istersen burayı kullanabilirsin)
                var existingWord = _context.Words
                    .Include(w => w.Samples)
                    .FirstOrDefault(w => w.EngWordName.ToLower() == eng.ToLower());

                if (existingWord != null)
                {
                    // Güncelleme işlemi yapmak istersen buraya ekle
                    // Şimdilik sadece atla veya hata ver
                    continue; // Veya return Content("Aynı kelime zaten var: " + eng);
                }

                var word = new Word
                {
                    EngWordName = eng,
                    TurWordName = tur,
                    Picture = !string.IsNullOrWhiteSpace(imageName) ? imageName : null,
                    CreatedAt = DateTime.Now,
                    KategoriID = kategoriID,
                    Samples = new List<WordSample>()
                };

                if (!string.IsNullOrWhiteSpace(sample1))
                {
                    word.Samples.Add(new WordSample
                    {
                        Samples = sample1,
                        KategoriID = kategoriID
                    });
                }

                if (!string.IsNullOrWhiteSpace(sample2))
                {
                    word.Samples.Add(new WordSample
                    {
                        Samples = sample2,
                        KategoriID = kategoriID
                    });
                }

                _context.Words.Add(word);
                _context.SaveChanges();
            }

            return Content("Excel'den veri başarıyla yüklendi.");
        }
    }
}
