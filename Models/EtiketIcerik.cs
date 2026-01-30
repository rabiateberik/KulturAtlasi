using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("EtiketliIcerikler")]
    public class EtiketliIcerik
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string IcerikTuru { get; set; } = string.Empty;
        public int IcerikID { get; set; } // Hangi Paylasim/Kitap/Film ID'si

        // İlişki
        public int EtiketID { get; set; }
        [ForeignKey("EtiketID")]
        public virtual Etiket Etiket { get; set; }
    }
}