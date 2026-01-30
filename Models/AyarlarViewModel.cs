using System.ComponentModel.DataAnnotations;

namespace KulturAtlasi.Models
{
    public class AyarlarViewModel
    {
        // GİZLİLİK İÇİN
        public bool GizliHesap { get; set; }
        public string? Eposta { get; set; } // Sadece bilgi amaçlı 

        // ŞİFRE DEĞİŞTİRME İÇİN
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string? MevcutSifre { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string? YeniSifre { get; set; }

        [DataType(DataType.Password)]
        [Compare("YeniSifre", ErrorMessage = "Şifreler birbiriyle uyuşmuyor.")]
        [Display(Name = "Yeni Şifre (Tekrar)")]
        public string? YeniSifreTekrar { get; set; }
    }
}