using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Oneriler")]
    public class Oneri
    {
        [Key]
        public int OneriID { get; set; }
        public string IcerikTuru { get; set; } = string.Empty;
        public string OnerilenIcerik { get; set; } = string.Empty;
        public string Neden { get; set; } = string.Empty;
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        // İlişki
        public string KullaniciID { get; set; }
        [ForeignKey("KullaniciID")]
        public virtual ApplicationUser Kullanici { get; set; }
    }
}