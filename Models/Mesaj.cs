using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Mesajlar")]
    public class Mesaj
    {
        [Key]
        public int MesajID { get; set; }
        public string Metin { get; set; } = string.Empty;
        public DateTime GonderimTarihi { get; set; } = DateTime.Now;

        // İlişki (Gönderen)
        public string GonderenID { get; set; }
        [ForeignKey("GonderenID")]
        public virtual ApplicationUser Gonderen { get; set; }

        // İlişki (Alıcı)
        public string AliciID { get; set; }
        [ForeignKey("AliciID")]
        public virtual ApplicationUser Alici { get; set; }
    }
}