using KulturAtlasi.Services;
using System.Collections.Generic;
using System.Linq; // Listeler için

namespace KulturAtlasi.Models
{
    public class DashboardViewModel
    {
        // kart istatistikleri
        public int ToplamKitap { get; set; }
        public int ToplamFilm { get; set; }
        public int ToplamDizi { get; set; }
        public int ToplamMuzik { get; set; }
        public int ToplamSeyahat { get; set; }
        public int GezilenUlkeSayisi { get; set; }

        // son eklelenenler
        public Kitap? SonKitap { get; set; }
        public Film? SonFilm { get; set; }
        public Dizi? SonDizi { get; set; }
        public Seyahat? SonSeyahat { get; set; }

        // garfik verileri
        
        public Dictionary<string, int> KitapTurDagilimi { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> FilmTurDagilimi { get; set; } = new Dictionary<string, int>();
        public List<OneriItem> Oneriler { get; set; } = new List<OneriItem>();
    }
}