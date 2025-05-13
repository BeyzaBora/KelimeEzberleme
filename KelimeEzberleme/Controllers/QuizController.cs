using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System;
using KelimeEzberleme.Helpers;

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
        // Çözülmemiş kelimeleri al
        var quizWords = _context.Words
            .Where(w => !w.IsLearned) // Bu kelimeler öğrenilmemiş olmalı
            .OrderBy(x => Guid.NewGuid()) // Karışık sıraya
            .Take(5) // İlk 5 kelimeyi al
            .ToList();

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

        if (currentIndex >= quizWords.Count)
            return RedirectToAction("QuizResultsInteractive"); // Bütün sorular bitince sonuç sayfasına yönlendir

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

    // 🔹 Cevap Gönderme
    [HttpPost]
    public IActionResult SubmitAnswer(int WordID, string userAnswer)
    {
        // Eğer cevap boş bırakılmışsa, işlem yapılmaz
        if (WordID == 0 || string.IsNullOrWhiteSpace(userAnswer))
            return RedirectToAction("NextQuestion");

        var quizWordsJson = HttpContext.Session.GetString("QuizWords");
        var currentIndex = HttpContext.Session.GetInt32("currentIndex") ?? 0;

        if (string.IsNullOrEmpty(quizWordsJson))
            return RedirectToAction("QuizResultsInteractive");

        var quizWords = JsonConvert.DeserializeObject<List<Word>>(quizWordsJson);
        if (currentIndex >= quizWords.Count)
            return RedirectToAction("QuizResultsInteractive");

        var word = quizWords[currentIndex];
        var correctAnswer = word.TurWordName?.Trim().ToLower();
        var userAns = userAnswer?.Trim().ToLower();
        bool isCorrect = correctAnswer == userAns;

        int? userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null || userId == 0)
        {
            // Giriş yapılmamışsa, login'e yönlendir
            return RedirectToAction("Login", "Account");
        }

        if (isCorrect)
        {
            HttpContext.Session.SetInt32("correct", (HttpContext.Session.GetInt32("correct") ?? 0) + 1);

            var progress = _context.WordProgresses
                .FirstOrDefault(p => p.UserID == userId && p.WordID == WordID);

            if (progress == null)
            {
                progress = new WordProgress
                {
                    UserID = userId.Value,
                    WordID = WordID,
                    CorrectCount = 1,
                    NextRepeat = DateTime.Now,
                    LastAnswered = DateTime.Now
                };

                _context.WordProgresses.Add(progress);
            }
            else
            {
                progress.CorrectCount++;
                progress.NextRepeat = CalculateNextRepeatTime(progress.CorrectCount);
                progress.LastAnswered = DateTime.Now;
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veritabanına kaydetme hatası: {ex.Message}");
                throw;
            }
        }
        else
        {
            var progress = _context.WordProgresses
                .FirstOrDefault(p => p.UserID == userId && p.WordID == WordID);

            if (progress != null)
            {
                progress.CorrectCount = 0;
                progress.NextRepeat = DateTime.Now;
                progress.LastAnswered = DateTime.Now;

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Veritabanına kaydetme hatası: {ex.Message}");
                    throw;
                }
            }

            HttpContext.Session.SetInt32("incorrect", (HttpContext.Session.GetInt32("incorrect") ?? 0) + 1);
        }

        var answers = HttpContext.Session.GetObjectFromJson<List<string>>("answers") ?? new List<string>();
        answers.Add($"Kelime: {word.EngWordName} - Doğru: {word.TurWordName} - Senin Cevabın: {userAnswer} - {(isCorrect ? "Doğru" : "Yanlış")}");

        HttpContext.Session.SetObjectAsJson("answers", answers);

        HttpContext.Session.SetInt32("currentIndex", currentIndex + 1);

        return RedirectToAction("NextQuestion");
    }

    private DateTime CalculateNextRepeatTime(int correctCount)
    {
        DateTime now = DateTime.Now;
        switch (Math.Min(correctCount, 6))
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
