using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Kitaplar")]
    public class Kitap
    {
        [Key]
        public int KitapID { get; set; }

        [Required]
        public string Baslik { get; set; } = string.Empty;

        public string? Yazar { get; set; }
        [StringLength(100)]
        public string? Tur { get; set; } 
        public string? ISBN { get; set; }
        public string? Aciklama { get; set; }
        public string? KapakResmiYolu { get; set; }
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