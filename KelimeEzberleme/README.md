# 📚 Kelime Ezberleme Uygulaması (6 Tekrar Prensibiyle)

Bu proje, kullanıcıların İngilizce kelimeleri **6 tekrar prensibine** göre ezberlemesini sağlayan bir web uygulamasıdır.  
Kullanıcılar görsellerle desteklenmiş kelimeleri öğrenebilir, quiz çözebilir ve istatistiklerini takip edebilir.

---

## 🚀 Özellikler

- ✅ Kullanıcı kayıt / giriş ve şifremi unuttum özelliği  
- ✅ Kelime ekleme ve örnek cümlelerle destekleme  
- ✅ 6 tekrar prensibine dayalı quiz sistemi  
- ✅ Günlük kelime hedefi belirleme  
- ✅ Rapor ekranı: başarı, hata ve kategori bazlı analiz  
- ✅ SonarQube ile optimize edilmiş temiz kod

---

## 🛠️ Kullanılan Teknolojiler

- ASP.NET Core MVC (.NET 6)
- MSSQL (SQL Server)
- Entity Framework Core
- HTML / CSS / Bootstrap
- Docker (SonarQube analizleri için)
- Git, GitHub

---

## 🔧 Kurulum Adımları

1. Bu repoyu klonlayın:
   ```bash
   git clone https://github.com/kullaniciadi/KelimeEzberleme.git
   ```

2. Visual Studio ile projeyi açın (`KelimeEzberleme.csproj` dosyasına çift tıklayarak).

3. `appsettings.json` dosyasındaki `DefaultConnection` alanını kendi SQL Server bağlantı bilgilerinizle güncelleyin.

4. Paketleri yüklemek için Visual Studio içindeki **NuGet Package Manager**'ı kullanın.  
   Gerekirse aşağıdaki komutları `Package Manager Console` içinde çalıştırabilirsiniz:
   ```bash
   Update-Package
   ```

5. Veritabanını oluşturmak için aşağıdaki komutu çalıştırın:
   ```bash
   Update-Database
   ```

---




## 📌 Notlar

- Bu uygulama akademik amaçlı hazırlanmıştır.
- Kod kalitesi SonarQube ile analiz edilmiştir.

---

## 📬 İletişim

Herhangi bir soru veya öneri için:  
📧 `yebeceenglish@gmail.com`

---

> © 2025 KelimeEzberleme Projesi