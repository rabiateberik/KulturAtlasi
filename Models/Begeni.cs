using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("Begeniler")]
    public class Begeni
    {
        [Key]
        public int BegeniID { get; set; }

        public string KullaniciID { get; set; }
        [ForeignKey("KullaniciID")]
        public virtual ApplicationUser Kullanici { get; set; }

        public int PaylasimID { get; set; }
        [ForeignKey("PaylasimID")]
        public virtual Paylasim Paylasim { get; set; }
    }
}