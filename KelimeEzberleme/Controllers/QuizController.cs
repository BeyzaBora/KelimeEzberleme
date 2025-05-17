using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System;
using KelimeEzberleme.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;

public class QuizController : Controller
{
    private readonly ApplicationDbContext _context;

    public QuizController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 Quiz Başlatma
    public IActionResult Quiz()
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Kullanıcının ayarlarını al
        var userSettings = _context.UserSettings
            .FirstOrDefault(u => u.UserID == userId);

        if (userSettings == null)
        {
            // Eğer kullanıcı ayarı yoksa, varsayılan ayarı kullan
            userSettings = new UserSettings
            {
                WordCount = 5 // Varsayılan 5
            };
        }

        int wordCount = userSettings.WordCount;

        DateTime now = DateTime.Now;
        int UserID = userId.Value;

        // Kullanıcının WordProgress kayıtlarını al, NextRepeat zamanı gelmiş ve öğrenilmemiş olanlar
        var quizWords = _context.WordProgresses
            .Include(wp => wp.Word) // Word navigasyonunu dahil et
            .Where(wp => wp.UserID == userId
                         && !wp.IsLearned
                         && wp.NextRepeat <= now)
            .OrderBy(x => Guid.NewGuid())
            .Take(wordCount)
            .Select(wp => wp.Word)  // sadece Word nesnesini alıyoruz
            .ToList();

        // Eğer yeterli kelime yoksa, öğrenilmemiş ve WordProgress kaydı olmayan kelimelerle tamamla
        if (quizWords.Count < wordCount)
        {
            int missingCount = wordCount - quizWords.Count;

            var additionalWords = _context.Words
                .Where(w => !_context.WordProgresses.Any(wp => wp.WordID == w.WordID && wp.UserID == userId))
                .OrderBy(x => Guid.NewGuid())
                .Take(missingCount)
                .ToList();

            quizWords.AddRange(additionalWords);
        }


        // Verileri session'a kaydet
        HttpContext.Session.SetString("QuizWords", JsonConvert.SerializeObject(quizWords));
        HttpContext.Session.SetInt32("currentIndex", 0); // Başlangıç index
        HttpContext.Session.SetInt32("correct", 0); // Doğru sayısı
        HttpContext.Session.SetInt32("incorrect", 0); // Yanlış sayısı
        HttpContext.Session.SetObjectAsJson("answers", new List<string>()); // Cevapları kaydet

        return RedirectToAction("NextQuestion"); // Soruyu göstermek için yönlendir
    }


    // 🔹 Soruyu Göster (View'e model gönderilir)
    public IActionResult NextQuestion()
    {
        var quizWordsJson = HttpContext.Session.GetString("QuizWords");
        var currentIndex = HttpContext.Session.GetInt32("currentIndex") ?? 0;

        if (string.IsNullOrEmpty(quizWordsJson))
            return RedirectToAction("QuizResultsInteractive"); // Eğer quiz verisi yoksa sonuç sayfasına yönlendir

        var quizWords = JsonConvert.DeserializeObject<List<Word>>(quizWordsJson);

        // Burada quizWords tanımlandıktan sonra kontrol yap
        if (currentIndex >= quizWords.Count)
        {
            if (quizWords.Count == 0)
            {
                ViewBag.Message = "Tebrikler! Şu anda cevaplanmamış veya tekrar edilmesi gereken kelime bulunmamaktadır. Sıradaki kelimeler belirlenen tekrar tarihine göre size sunulacaktır. Lütfen daha sonra tekrar deneyiniz.";
                return View("QuizEmpty"); // Bu özel mesaj için yeni bir view tasarla
            }
            else
            {
                return RedirectToAction("QuizResultsInteractive");
            }
        }

        var word = quizWords[currentIndex];
        var model = new InteractiveQuizViewModel
        {
            WordID = word.WordID,
            EngWordName = word.EngWordName,
            TurWordName = word.TurWordName,
            Picture = word.Picture,
            Samples = _context.WordSamples.Where(ws => ws.WordID == word.WordID).ToList(),
            CurrentQuestionIndex = currentIndex + 1,
            TotalQuestions = quizWords.Count,
            CorrectCount = HttpContext.Session.GetInt32("correct") ?? 0,
            IncorrectCount = HttpContext.Session.GetInt32("incorrect") ?? 0,
            IsCorrect = null
        };

        return View("Quiz", model); // Quiz sayfasını görüntüle
    }
    [HttpPost]
    public IActionResult SubmitAnswer(int WordID, string userAnswer, string skip)
    {
        // Eğer WordID geçersizse, sonraki soruya geç
        if (WordID == 0)
            return RedirectToAction("NextQuestion");

        // Boş bırakma seçeneği kontrolü
        if (!string.IsNullOrEmpty(skip) && skip == "true")
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null || userId == 0)
                return RedirectToAction("Login", "Account");

            var quizWordsJson = HttpContext.Session.GetString("QuizWords");
            var currentIndex = HttpContext.Session.GetInt32("currentIndex") ?? 0;

            if (string.IsNullOrEmpty(quizWordsJson))
                return RedirectToAction("QuizResultsInteractive");

            var quizWords = JsonConvert.DeserializeObject<List<Word>>(quizWordsJson);
            var word = quizWords.ElementAtOrDefault(currentIndex);

            // Cevap geçmişine boş bırakma kaydı ekle
            var answers = HttpContext.Session.GetObjectFromJson<List<string>>("answers") ?? new List<string>();
            answers.Add($"Kelime: {word?.EngWordName} - Doğru: {word?.TurWordName} - Sen boş bıraktın.");
            HttpContext.Session.SetObjectAsJson("answers", answers);

            // Soruyu boş geç, currentIndex arttır
            HttpContext.Session.SetInt32("currentIndex", currentIndex + 1);

            return RedirectToAction("NextQuestion");
        }

        // Normal cevap kontrolü (boş cevap verilmişse sonraki soruya geç)
        if (string.IsNullOrWhiteSpace(userAnswer))
            return RedirectToAction("NextQuestion");

        var quizWordsJson2 = HttpContext.Session.GetString("QuizWords");
        var currentIndex2 = HttpContext.Session.GetInt32("currentIndex") ?? 0;

        if (string.IsNullOrEmpty(quizWordsJson2))
            return RedirectToAction("QuizResultsInteractive");

        var quizWords2 = JsonConvert.DeserializeObject<List<Word>>(quizWordsJson2);
        if (currentIndex2 >= quizWords2.Count)
            return RedirectToAction("QuizResultsInteractive");

        var word2 = quizWords2[currentIndex2];
        var correctAnswer = word2.TurWordName?.Trim().ToLower();
        var userAns = userAnswer?.Trim().ToLower();
        bool isCorrect = correctAnswer == userAns;

        int? userId2 = HttpContext.Session.GetInt32("UserID");
        if (userId2 == null || userId2 == 0)
            return RedirectToAction("Login", "Account");

        var progress = _context.WordProgresses
            .FirstOrDefault(p => p.UserID == userId2 && p.WordID == WordID);

        if (isCorrect)
        {
            HttpContext.Session.SetInt32("correct", (HttpContext.Session.GetInt32("correct") ?? 0) + 1);

            if (progress == null)
            {
                progress = new WordProgress
                {
                    UserID = userId2.Value,
                    WordID = WordID,
                    CorrectCount = 1,
                    IncorrectCount = 0,
                    NextRepeat = CalculateNextRepeatTime(1),
                    LastAnswered = DateTime.Now,
                    IsLearned = false
                };

                _context.WordProgresses.Add(progress);
            }
            else
            {
                progress.CorrectCount++;
                progress.LastAnswered = DateTime.Now;
                progress.NextRepeat = CalculateNextRepeatTime(progress.CorrectCount);

                if (progress.CorrectCount >= 6)
                {
                    progress.IsLearned = true;

                    var wordToUpdate = _context.Words.FirstOrDefault(w => w.WordID == WordID);
                    if (wordToUpdate != null && !wordToUpdate.IsLearned)
                    {
                        wordToUpdate.IsLearned = true;
                    }
                }
            }

            _context.SaveChanges();

            // Kelime öğrenildiyse quiz listesini güncelle
            if (progress.IsLearned)
            {
                var userSettings = _context.UserSettings.FirstOrDefault(u => u.UserID == userId2);
                int wordCount = userSettings?.WordCount ?? 5;

                var updatedQuizWords = _context.Words
                    .Where(w => !w.IsLearned)
                    .OrderBy(x => Guid.NewGuid())
                    .Take(wordCount)
                    .ToList();

                if (updatedQuizWords.Count < wordCount)
                {
                    int missingCount = wordCount - updatedQuizWords.Count;

                    var additionalWords = _context.Words
                        .Where(w => !_context.WordProgresses.Any(wp => wp.WordID == w.WordID && wp.UserID == userId2))
                        .OrderBy(x => Guid.NewGuid())
                        .Take(missingCount)
                        .ToList();

                    updatedQuizWords.AddRange(additionalWords);
                }

                HttpContext.Session.SetString("QuizWords", JsonConvert.SerializeObject(updatedQuizWords));
                HttpContext.Session.SetInt32("currentIndex", 0);
            }
        }
        else
        {
            HttpContext.Session.SetInt32("incorrect", (HttpContext.Session.GetInt32("incorrect") ?? 0) + 1);

            if (progress == null)
            {
                progress = new WordProgress
                {
                    UserID = userId2.Value,
                    WordID = WordID,
                    CorrectCount = 0,
                    IncorrectCount = 1,
                    NextRepeat = DateTime.Now,
                    LastAnswered = DateTime.Now,
                    IsLearned = false
                };

                _context.WordProgresses.Add(progress);
            }
            else
            {
                progress.IncorrectCount++;
                progress.CorrectCount = 0;
                progress.LastAnswered = DateTime.Now;
                progress.NextRepeat = DateTime.Now;
                progress.IsLearned = false;
            }

            _context.SaveChanges();
        }

        // Cevap geçmişine kayıt
        var answers2 = HttpContext.Session.GetObjectFromJson<List<string>>("answers") ?? new List<string>();
        answers2.Add($"Kelime: {word2.EngWordName} - Doğru: {word2.TurWordName} - Senin Cevabın: {userAnswer} - {(isCorrect ? "Doğru" : "Yanlış")}");
        HttpContext.Session.SetObjectAsJson("answers", answers2);

        // Sonraki soruya geç
        HttpContext.Session.SetInt32("currentIndex", currentIndex2 + 1);

        return RedirectToAction("NextQuestion");
    }



    private DateTime CalculateNextRepeatTime(int CorrectCount)
    {
        DateTime now = DateTime.Now;
        switch (Math.Min(CorrectCount, 6))
        {
            case 1: return now.AddDays(1);     // 1 gün sonra tekrar
            case 2: return now.AddDays(7);     // 1 hafta sonra tekrar
            case 3: return now.AddMonths(1);   // 1 ay sonra tekrar
            case 4: return now.AddMonths(3);   // 3 ay sonra tekrar
            case 5: return now.AddMonths(6);   // 6 ay sonra tekrar
            case 6: return now.AddYears(1);    // 1 yıl sonra tekrar
            default: return now.AddYears(1);   // Eğer 6. deneme ise, 1 yıl sonra tekrar
        }
    }

    // 🔹 Sonuç Sayfası
    public IActionResult QuizResultsInteractive()
    {
        ViewBag.Correct = HttpContext.Session.GetInt32("correct") ?? 0;
        ViewBag.Incorrect = HttpContext.Session.GetInt32("incorrect") ?? 0;
        ViewBag.Answers = HttpContext.Session.GetObjectFromJson<List<string>>("answers");

        return View();
    }
}