using Microsoft.AspNetCore.Http; // Resim yükleme için şart

namespace KulturAtlasi.Models
{
    public class ProfilDuzenleViewModel
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string? Meslek { get; set; }
        public string? Sehir { get; set; }
        public string? Hakkimda { get; set; }

        // Mevcut resim yolunu göstermek için
        public string? MevcutResim { get; set; }

        // Yeni yüklenecek dosya
        public IFormFile? ProfilResmi { get; set; }
    }
}