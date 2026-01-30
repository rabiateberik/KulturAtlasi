# 🗺️ Kültür Atlası

Kültür Atlası; kitap, film, müzik ve seyahat verilerini tek bir platformda toplayan, AI destekli bir dijital arşiv ve kültür–sanat uygulamasıdır.
Kullanıcı etkileşimlerini analiz ederek kişiye özel içerik deneyimi sunar.

## ⭐ Öne Çıkan Özellikler

- **📚 Kitap Modülü:** API destekli kitap arama, okuma durumu takibi ve puanlama
- **🎬 Film Modülü:** Otomatik veri çekme ve manuel ekleme seçenekleri
- **🛡️ Akış Modülü:** Kullanıcı ilgi alanlarına göre kişiselleştirilmiş içerik akışı
- **🤖 Akıllı Asistan (AI):** İçerik önerileri ve kullanıcıya özel analizler
- **🎵 Müzik Arşivi:** Favori müzikleri arama, ekleme ve değerlendirme
- **📍 Seyahat Rotaları:** Leaflet.js ile harita üzerinde konum işaretleme

## 🛠️ Kullanılan Teknolojiler

- **Backend:** ASP.NET Core 8.0 (MVC)
- **Veritabanı:** MSSQL & Entity Framework Core
- **Frontend:** HTML5, CSS3, Bootstrap, JavaScript
- **Harita:** Leaflet.js & OpenStreetMap
- **Görseller:** Unsplash API
- **Kimlik Doğrulama:** ASP.NET Core Identity

## ⚙️ Kurulum ve Çalıştırma

1. Projeyi Klonlayın: git clone https://github.com/rabiateberik/KulturAtlasi.git.

2. API Yapılandırması: appsettings.json dosyasını oluşturup TMDb, Groq ve GoogleAI anahtarlarınızı ekleyin.

3. Veritabanı: Package Manager Console üzerinden Update-Database komutuyla tabloları oluşturun.

4. Çalıştır: Visual Studio üzerinden projeyi başlatın.
