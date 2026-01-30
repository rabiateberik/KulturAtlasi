using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Diziler")]
    public class Dizi
    {
        [Key]
        public int DiziID { get; set; }

        [Required]
        public string Baslik { get; set; } = string.Empty; // Dizi Adı

        public string? Yonetmen { get; set; } // Veya "Yapımcı / Kanal"
        public string? Tur { get; set; } // API'den "Bilim Kurgu, Aksiyon" diye gelecek
        public string? Aciklama { get; set; }
        public int? BaslangicYili { get; set; }

        // --- DİZİYE ÖZEL ALANLAR ---
        public int? SezonSayisi { get; set; }
        public int? BolumSayisi { get; set; }
        // ---------------------------

        public string? PosterYolu { get; set; }
        public int? KullaniciPuani { get; set; }
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // İlişkiler
        public int DurumID { get; set; }
        [ForeignKey("DurumID")]
        public virtual Durum Durum { get; set; }

        public string KullaniciID { get; set; }
        [ForeignKey("KullaniciID")]
        public virtual ApplicationUser Kullanici { get; set; }
    }
}