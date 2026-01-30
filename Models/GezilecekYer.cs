using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("GezilecekYerler")]
    public class GezilecekYer
    {
        [Key]
        public int YerID { get; set; }
        public string MekanAdi { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string FotografYolu { get; set; } = string.Empty;
        public string KonumKoordinati { get; set; } = string.Empty;
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // İlişki (Hangi seyahate ait)
        public int SeyahatID { get; set; }
        [ForeignKey("SeyahatID")]
        public virtual Seyahat Seyahat { get; set; }
    }
}