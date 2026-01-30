namespace KulturAtlasi.Models
{
    public class ListeOgesiViewModel
    {
        //profil listesi
        public int? Id { get; set; }        // Kitap/Film ID'si (Kullanıcı için gerekmez string id kullanırız)
        public string? UserId { get; set; } // Kullanıcı ID'si
        public string Baslik { get; set; }  // Kullanıcı Adı veya Kitap Adı
        public string AltBaslik { get; set; } // @kullaniciadi veya Yazar Adı
        public string Resim { get; set; }   // Profil Resmi veya Kapak
        public string Link { get; set; }    // Tıklayınca gideceği yer
        public bool ButtonGoster { get; set; } // Takip Et butonu vb. için (şimdilik basit tutalım)
    }
}