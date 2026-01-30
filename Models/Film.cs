using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Filmler")]
    public class Film
    {
        [Key]
        public int FilmID { get; set; }

        [Required]
        public string Baslik { get; set; } = string.Empty;

        public string? Yonetmen { get; set; }
        public string? Tur { get; set; } // API'den "Bilim Kurgu, Aksiyon" diye gelecek
        public string? Aciklama { get; set; }
        public int? Yil { get; set; }
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