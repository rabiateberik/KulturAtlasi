using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Paylasimlar")]
    public class Paylasim
    {
        [Key]
        public int PaylasimID { get; set; }

        [Required]
        public string Icerik { get; set; } = string.Empty; // Yazılan metin

        [Required]
        public string PaylasimTuru { get; set; } = "Düşünce"; // "Alıntı", "İnceleme", "Düşünce"

        public DateTime Tarih { get; set; } = DateTime.Now;

        // --- HİBRİT YAPI (BOŞ OLABİLİR) ---
        // Eğer "Düşünce" ise buralar NULL kalacak.
        public string? IlgiliIcerikTuru { get; set; } // "Kitap", "Film"
        public int? IlgiliIcerikID { get; set; }      // 5, 12 vb.
        // ----------------------------------
       
        public string KullaniciID { get; set; }
        [ForeignKey("KullaniciID")]
        public virtual ApplicationUser Kullanici { get; set; }
        public virtual ICollection<Begeni> Begeniler { get; set; }
        public virtual ICollection<Yorum> Yorumlar { get; set; }
    }
}