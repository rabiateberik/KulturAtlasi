namespace KulturAtlasi.Models
{
    public class ProfilViewModel
    {
        // Kullanıcı Bilgileri
        public string KullaniciID { get; set; }
        public string AdSoyad { get; set; }
        public string KullaniciAdi { get; set; }
        public string ProfilResmi { get; set; }
        public string Biyografi { get; set; }
        public DateTime KayitTarihi { get; set; }

        // İstatistikler
        public int TakipciSayisi { get; set; }
        public int TakipEdilenSayisi { get; set; }
        public int ToplamPaylasim { get; set; }
        public int OkunanKitap { get; set; }
        public int IzlenenFilm { get; set; }

        // Durumlar
        public bool KendiProfilimMi { get; set; }
        public bool TakipEdiyorMuyum { get; set; }

        // gönderielirin listelendiği yer
        public List<AkisViewModel> Paylasimlar { get; set; }
    }
}