using Microsoft.AspNetCore.Mvc;
using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using KelimeEzberleme.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using KelimeEzberleme.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace KelimeEzberleme.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public AccountController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    FullName = model.FullName,
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = model.Password,
                    PasswordResetToken = "",       // Boş string atandı
                    TokenExpireDate = DateTime.Now.AddHours(1)     // Nullable olduğu için null bırakıldı
                };


                _context.Users.Add(newUser);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string kullaniciAdi, string sifre)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == kullaniciAdi && u.Password == sifre);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserID", user.UserID);
                HttpContext.Session.SetString("UserName", user.UserName);

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewBag.Hata = "Kullanıcı adı veya şifre yanlış.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ============================
        // Şifremi Unuttum - Maille Sıfırlama
        // ============================

        // GET: Şifremi Unuttum sayfası
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Şifremi Unuttum formu gönderimi
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email) || !new EmailAddressAttribute().IsValid(email))
            {
                ModelState.AddModelError("", "Lütfen geçerli bir e-posta adresi giriniz.");
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                Console.WriteLine("Kullanıcı bulunamadı: " + email);
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.TokenExpireDate = DateTime.Now.AddHours(1);
            await _context.SaveChangesAsync();

            Console.WriteLine("Token oluşturuldu: " + token);
            Console.WriteLine("Email: " + user.Email);

            var resetLink = Url.Action("ResetPassword", "Account", new { token = token, email = user.Email }, Request.Scheme);
            Console.WriteLine("Reset linki: " + resetLink);

            string emailBody = $"Şifrenizi sıfırlamak için lütfen <a href='{resetLink}'>buraya tıklayın</a>.";

            try
            {
                Console.WriteLine("Mail gönderiliyor...");
                await _emailSender.SendEmailAsync(user.Email, "Şifre Sıfırlama", emailBody);
                Console.WriteLine("Mail gönderildi!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mail gönderme hatası: " + ex.Message);
                ModelState.AddModelError("", "E-posta gönderilemedi: " + ex.Message);
                return View();
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        // GET: Şifre sıfırlama linki gönderildi onay sayfası
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: Şifre sıfırlama formu
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                ViewBag.Hata = "Geçersiz şifre sıfırlama isteği.";
                return View("Error");
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        // POST: Yeni şifre kaydetme
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.PasswordResetToken == model.Token);

            if (user == null || user.TokenExpireDate < DateTime.Now)
            {
                ViewBag.Hata = "Geçersiz veya süresi dolmuş token.";
                return View("Error");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Şifreler uyuşmuyor.");
                return View(model);
            }

            user.Password = model.NewPassword; // NOT: Şifreyi hashleyerek kaydetmelisin
            user.PasswordResetToken = null;
            user.TokenExpireDate = null;

            _context.SaveChanges();

            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        // GET: Şifre sıfırlama başarılı onay sayfası
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}

