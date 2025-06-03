using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using KelimeEzberleme.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using KelimeEzberleme.Services;
using KelimeEzberleme.Models;  // Eğer EmailSettings bu namespace içindeyse


var builder = WebApplication.CreateBuilder(args);

// Geliştirme ortamı olarak   ayarlandı
builder.Environment.EnvironmentName = "Development";

// Veritabanı bağlantısı ayarlanıyor
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.PendingModelChangesWarning));
});

// Controller ve view desteği ekleniyor
builder.Services.AddControllersWithViews();

// Session yapılandırması
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 30 dakika boyunca oturum açık kalır
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Cookie izni gerekmez (GDPR için)
});


// HttpClient servisi ekleniyor (REST API çağrıları için)
builder.Services.AddHttpClient();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Ortama göre hata sayfası ayarlanıyor
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

// Statik dosyalar sunuluyor
app.UseStaticFiles();

app.UseRouting();

// Session middleware kullanılıyor
app.UseSession();

app.UseAuthorization();

// Route ayarları
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var seeder = new DataSeeder(context);
    seeder.Seed();
}


app.Run();

