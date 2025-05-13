using KelimeEzberleme.Data;
using KelimeEzberleme.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ➕ Servisler buraya!
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // 🔥 Bu satır Build'dan ÖNCE olacak

var app = builder.Build();

// 🌐 Middleware'ler
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // ✅ Session burada aktif edilir
app.UseAuthorization();

// Tek bir portu belirlemek için UseUrls metodunu burada kullanıyoruz
builder.WebHost.UseUrls("http://localhost:5259");  // Bu satır, portu belirler
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
