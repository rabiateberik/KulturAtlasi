using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Muzikler")]
    public class Muzik
    {
        [Key]
        public int MuzikID { get; set; }

        [Required]
        public string Baslik { get; set; } = string.Empty; // Şarkı veya Albüm Adı

        public string? Sanatci { get; set; } // Zorunlu değil ama olsa iyi olur

        public string? Tur { get; set; } // Pop, Rock, Caz
        public string? Album { get; set; }
        public int? Yil { get; set; } // Çıkış Yılı

        // --- EKLENEN YENİ ALANLAR ---
        public string? KapakResmiYolu { get; set; } // Apple'dan gelen resim buraya
        public string? DinlemeLinki { get; set; } // Şarkının MP3/M4A linki
        public int? KullaniciPuani { get; set; } // 1-5 Yıldız
        // ----------------------------

        public string? Aciklama { get; set; }

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