using Microsoft.AspNetCore.Mvc;
using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;


namespace KelimeEzberleme.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
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
                    Password = model.Password // NOT: İleride şifre hashleyeceğiz!
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: /Account/Login (Boş kalsın, sonra yazacağız)
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string kullaniciAdi, string sifre)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == kullaniciAdi && u.Password == sifre);

            if (user != null)
            {
                // Session'a kullanıcıyı yaz
                HttpContext.Session.SetInt32("UserID", user.UserID);
                HttpContext.Session.SetString("UserName", user.UserName);

                // Başarılı giriş → Dashboard'a yönlendir
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
            HttpContext.Session.Clear(); // Tüm session verilerini temizle
            return RedirectToAction("Login");
        }
       
        }
        
    }

