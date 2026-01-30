namespace KulturAtlasi.Models
{
    public class AdminDashboardViewModel
    {
        // İstatistikler
        public int ToplamUye { get; set; }
        public int ToplamKitap { get; set; }
        public int ToplamFilm { get; set; }
        public int ToplamYorum { get; set; } 

        // Listeler
        public List<ApplicationUser> SonUyeler { get; set; }
    }
}