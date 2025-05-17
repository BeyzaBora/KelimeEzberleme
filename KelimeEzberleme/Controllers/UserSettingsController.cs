using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;

public class UserSettingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserSettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // UserSettings verisini veritabanından alıyoruz
        var userSettings = await _context.UserSettings
                                          .FirstOrDefaultAsync(u => u.UserID == userId);

        if (userSettings == null)
        {
            // Eğer veri yoksa, yeni bir UserSettings nesnesi oluşturuyoruz
            userSettings = new UserSettings
            {
                UserID = userId.Value,
                WordCount = 5, // Varsayılan değer 5
                UserName = "defaultUser" // UserName'i buraya atıyoruz
            };

            _context.UserSettings.Add(userSettings); // Yeni veri ekliyoruz
            await _context.SaveChangesAsync(); // Değişiklikleri kaydediyoruz
        }

        // UserSettings bilgilerini session'a kaydediyoruz
        HttpContext.Session.SetInt32("WordCount", userSettings.WordCount);

        return View(userSettings);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuizCount(int wordCount)
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Veritabanında UserSettings kaydını alıyoruz
        var userSettings = await _context.UserSettings
                                          .FirstOrDefaultAsync(u => u.UserID == userId);

        // Eğer userSettings bulunamazsa Index sayfasına yönlendir
        if (userSettings == null)
        {
            return RedirectToAction("Index");
        }

        // WordCount değerini güncelliyoruz
        userSettings.WordCount = wordCount;
        await _context.SaveChangesAsync(); // Değişiklikleri kaydediyoruz

        // Başarı mesajı ekliyoruz
        TempData["SuccessMessage"] = "Quiz soru sayısı başarıyla güncellendi!";

        // Güncelleme işlemi sonrası Index sayfasına yönlendiriyoruz
        return RedirectToAction("Index");
    }

}
