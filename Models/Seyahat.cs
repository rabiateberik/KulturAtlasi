using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace KulturAtlasi.Models
{
    [Table("Seyahatler")]
    public class Seyahat
    {
        [Key]
        public int SeyahatID { get; set; }

        [Required]
        public string Baslik { get; set; } = string.Empty;

        public string? Sehir { get; set; }
        public string? Ulke { get; set; }
        public string? Tur { get; set; } // Şehir, Müze, Doğa vs
        public string? GeziNotu { get; set; }
        public string? ResimYolu { get; set; } // Kapak fotoğrafı linki
        public DateTime? ZiyaretTarihi { get; set; }

        public double? Enlem { get; set; }
        public double? Boylam { get; set; }

        // İlişkiler
        public int DurumID { get; set; }
        [ForeignKey("DurumID")]
        public virtual Durum Durum { get; set; }

        public string KullaniciID { get; set; }
        [ForeignKey("KullaniciID")]
        public virtual ApplicationUser Kullanici { get; set; }

        public virtual ICollection<GezilecekYer>? GezilecekYerler { get; set; }
    }
}