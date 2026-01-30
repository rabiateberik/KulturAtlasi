using System.Collections.Generic;

namespace KulturAtlasi.Models
{
    public class AramaViewModel
    {
        public string AramaKelimesi { get; set; } // Ne aradığını başlıkta göstermek için

        public List<ApplicationUser> Kullanicilar { get; set; }
        public List<Kitap> Kitaplar { get; set; }
        public List<Film> Filmler { get; set; }
        public List<Dizi> Diziler { get; set; }
    }
}