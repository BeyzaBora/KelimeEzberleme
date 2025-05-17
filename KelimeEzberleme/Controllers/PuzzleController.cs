using KelimeEzberleme.Data;
using KelimeEzberleme.Helpers;
using KelimeEzberleme.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KelimeEzberleme.Controllers
{
    public class PuzzleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int maxGuesses = 6;

        public PuzzleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Yeni oyun başlatma
        public IActionResult StartGame()
        {
            // Öğrenilmiş ve en az 6 doğruya sahip kelimelerin WordID'lerini çek
            var learnedWordIds = _context.WordProgresses
                .Where(wp => wp.IsLearned == true && wp.CorrectCount >= 6)
                .Select(wp => wp.WordID)
                .Distinct()
                .ToList();

            if (learnedWordIds.Count == 0)
            {
                ViewBag.Message = "Henüz öğrenilmiş kelime bulunmamaktadır. Lütfen daha fazla kelime öğrenin.";
                return View("NoLearnedWords"); // Bu view'ı oluşturabilirsin
            }

            // Öğrenilmiş kelimelerden rastgele birini seç
            var learnedWords = _context.Words
                .Where(w => learnedWordIds.Contains(w.WordID))
                .OrderBy(r => System.Guid.NewGuid())
                .Select(w => w.EngWordName.ToLower())
                .ToList();

            var hiddenWord = learnedWords.First();

            var game = new PuzzleGame()
            {
                HiddenWord = hiddenWord,
                MaxGuesses = maxGuesses
            };

            HttpContext.Session.SetObjectAsJson("PuzzleGame", game);

            return RedirectToAction("Play");
        }



        // Oyun ekranı
        public IActionResult Play()
        {
            var game = HttpContext.Session.GetObjectFromJson<PuzzleGame>("PuzzleGame");
            if (game == null)
                return RedirectToAction("StartGame");

            ViewBag.WordLength = game.HiddenWord.Length;
            ViewBag.FirstLetter = char.ToUpper(game.HiddenWord[0]);

            return View(game);
        }

        // Tahmin gönderme (POST)
        [HttpPost]
        public IActionResult Guess(string guess)
        {
            var game = HttpContext.Session.GetObjectFromJson<PuzzleGame>("PuzzleGame");
            if (game == null)
                return RedirectToAction("StartGame");

            if (game.IsFinished)
                return RedirectToAction("Play");

            guess = guess?.ToLowerInvariant() ?? string.Empty;


            if (guess.Length != game.HiddenWord.Length)
            {
                TempData["Error"] = $"Lütfen {game.HiddenWord.Length} harfli bir kelime tahmin edin.";
                return RedirectToAction("Play");
            }

            // Harf durumlarını hesapla
            var letterStatuses = new List<LetterStatus>();
            var hiddenWordChars = game.HiddenWord.ToCharArray();
            var guessChars = guess.ToCharArray();

            var unmatchedHidden = new List<char>();
            for (int i = 0; i < hiddenWordChars.Length; i++)
            {
                if (guessChars[i] != hiddenWordChars[i])
                    unmatchedHidden.Add(hiddenWordChars[i]);
            }

            for (int i = 0; i < guessChars.Length; i++)
            {
                if (guessChars[i] == hiddenWordChars[i])
                {
                    letterStatuses.Add(new LetterStatus { Letter = guessChars[i], Color = "green" });
                }
                else if (unmatchedHidden.Contains(guessChars[i]))
                {
                    letterStatuses.Add(new LetterStatus { Letter = guessChars[i], Color = "yellow" });
                    unmatchedHidden.Remove(guessChars[i]);
                }
                else
                {
                    letterStatuses.Add(new LetterStatus { Letter = guessChars[i], Color = "gray" });
                }
            }

            // GuessResult listesine ekle
            game.GuessResults.Add(new GuessResult { Guess = guess, Letters = letterStatuses });

            // Tahminleri listeye ekle
            game.Guesses.Add(guess);

            var hiddenWordLower = game.HiddenWord.ToLowerInvariant();

            if (guess == hiddenWordLower)
            {
                game.IsFinished = true;
                game.IsWon = true;
            }

            else if (game.Guesses.Count >= game.MaxGuesses)
            {
                game.IsFinished = true;
                game.IsWon = false;
            }

            HttpContext.Session.SetObjectAsJson("PuzzleGame", game);

            return RedirectToAction("Play");
        }


    }
}
