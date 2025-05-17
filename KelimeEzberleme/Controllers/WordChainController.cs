using Microsoft.AspNetCore.Mvc;
using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using KelimeEzberleme.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace KelimeEzberleme.Controllers
{
    public class WordChainController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WordChainController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Öğrenilmiş kelimeleri getir, form göster
        [HttpGet]
        public IActionResult Index()
        {
            var learnedWords = _context.Words.Where(w => w.IsLearned).ToList();
            var model = new WordChainViewModel
            {
                LearnedWords = learnedWords,
                SelectedWordIds = new List<int>()
            };
            return View(model);
        }
            [HttpPost]
            public IActionResult Index(string selectedWordIds)
            {
                var selectedIds = string.IsNullOrEmpty(selectedWordIds)
                    ? new List<int>()
                    : selectedWordIds.Split(',').Select(int.Parse).ToList();

                var learnedWords = _context.Words.Where(w => w.IsLearned).ToList();

                var sentences = new List<string>();
                foreach (var id in selectedIds)
                {
                    var wordSentences = _context.WordSentences
                        .Where(ws => ws.WordId == id)
                        .OrderBy(ws => ws.Id)
                        .Select(ws => ws.Sentence)
                        .ToList();
                    sentences.AddRange(wordSentences);
                }

                var paragraph = string.Join(" ", sentences);

                var model = new WordChainViewModel
                {
                    LearnedWords = learnedWords,
                    SelectedWordIds = selectedIds,
                    SelectedSentencesParagraph = paragraph,

                    // Buraya resim yolunu veriyoruz
                    ImagePath = "/uploads/sample-image.jpg"  // Kendi resim yolunu yaz
                };

                return View(model);
            }

        }
    }
