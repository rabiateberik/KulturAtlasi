namespace KulturAtlasi.Models
{
    public class ApiAramaSonuc
    {
        public string Baslik { get; set; }
        public string AltBilgi { get; set; } // Yazar veya Yönetmen adı
        public string Resim { get; set; }
        public string Tur { get; set; } // "Kitap" veya "Film"
    }
}