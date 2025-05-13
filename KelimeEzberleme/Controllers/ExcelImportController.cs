using ClosedXML.Excel;
using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using Microsoft.AspNetCore.Mvc;

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
            string filePath = "C:\\Users\\232803047\\Downloads\\KelimeListesi_OrnekCumleli (1).xlsx"; // 💡 Dosyanın tam yolu

            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // 1. satır başlık

            foreach (var row in rows)
            {
                var eng = row.Cell(1).GetString();
                var tur = row.Cell(2).GetString();
                var imageName = row.Cell(3).GetString().Trim();
                var sample1 = row.Cell(4).GetString();
                var sample2 = row.Cell(5).GetString();


                var word = new Word
                {
                    EngWordName = eng,
                    TurWordName = tur,
                    Picture = imageName.StartsWith("/uploads/")
                                       ? imageName
        :                          "/uploads/" + imageName,
                    CreatedAt = DateTime.Now
                };

                if (!string.IsNullOrWhiteSpace(sample1))
                {
                    word.Samples.Add(new WordSample { Samples = sample1 });
                }

                if (!string.IsNullOrWhiteSpace(sample2))
                {
                    word.Samples.Add(new WordSample { Samples = sample2 });
                }

                _context.Words.Add(word);
            }

            _context.SaveChanges();

            return Content("Excel'den veri başarıyla yüklendi.");
        }
    }
}
