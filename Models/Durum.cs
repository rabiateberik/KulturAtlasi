using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace KulturAtlasi.Models
{
    [Table("Durumlar")]
    public class Durum
    {
        [Key]
        public int DurumID { get; set; }

        [Required]
        [StringLength(50)]
        public string Ad { get; set; } = string.Empty; // Örn: "Okudum", "İzleyeceğim"

        [StringLength(50)]
        public string Kategori { get; set; } = string.Empty; // Örn: "Kitap", "Film"

        // Navigation Properties
        public virtual ICollection<Kitap>? Kitaplar { get; set; }
        public virtual ICollection<Film>? Filmler { get; set; }
        public virtual ICollection<Seyahat>? Seyahatler { get; set; }
    }
}