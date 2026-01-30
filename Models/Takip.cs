using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Takipler")]
    public class Takip
    {
        [Key]
        public int Id { get; set; }

        // Takip Eden Kişi 
        public string TakipEdenID { get; set; }
        [ForeignKey("TakipEdenID")]
        public virtual ApplicationUser TakipEden { get; set; }

        // Takip Edilen Kişi 
        public string TakipEdilenID { get; set; }
        [ForeignKey("TakipEdilenID")]
        public virtual ApplicationUser TakipEdilen { get; set; }

        public DateTime TakipTarihi { get; set; } = DateTime.Now;
    }
}