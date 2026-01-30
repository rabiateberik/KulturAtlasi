using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Etiketler")]
    public class Etiket
    {
        [Key]
        public int EtiketID { get; set; }

        [Required]
        public string EtiketAdi { get; set; } = string.Empty;

        public string? Kategori { get; set; }
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        public virtual ICollection<EtiketliIcerik>? EtiketliIcerikler { get; set; }
    }
}