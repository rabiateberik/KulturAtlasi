using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("AktiviteKayitlari")]
    public class AktiviteKaydi
    {
        [Key]
        public int LogID { get; set; }
        public string Islem { get; set; } = string.Empty;
        public int IcerikID { get; set; }
        public string Kategori { get; set; } = string.Empty;
        public DateTime Tarih { get; set; } = DateTime.Now;

        // İlişki
        public string KullaniciID { get; set; }
        [ForeignKey("KullaniciID")]
        public virtual ApplicationUser Kullanici { get; set; }
    }
}