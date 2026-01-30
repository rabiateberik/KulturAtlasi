using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Bildirimler")]
    public class Bildirim
    {
        [Key]
        public int BildirimID { get; set; }

        public string Tur { get; set; } // "Takip", "Yorum", "Beğeni"
        public string Mesaj { get; set; }
        public string? Link { get; set; } 

        public bool GorulduMu { get; set; } = false;
        public DateTime Tarih { get; set; } = DateTime.Now;

        // Bildirimi alan kişi
        public string KullaniciID { get; set; }
        [ForeignKey("KullaniciID")]
        public virtual ApplicationUser Kullanici { get; set; }
    }
}