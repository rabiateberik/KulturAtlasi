using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Yorumlar")]
    public class Yorum
    {
        [Key]
        public int YorumID { get; set; }

        [Required]
        public string Icerik { get; set; }

        public DateTime Tarih { get; set; } = DateTime.Now;

        public string KullaniciID { get; set; }
        [ForeignKey("KullaniciID")]
        public virtual ApplicationUser Kullanici { get; set; }

        public int PaylasimID { get; set; }
        [ForeignKey("PaylasimID")]
        public virtual Paylasim Paylasim { get; set; }
    }
}