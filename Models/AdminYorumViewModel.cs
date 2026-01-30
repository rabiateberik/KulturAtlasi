using System;

namespace KulturAtlasi.ViewModels
{
    public class AdminYorumViewModel
    {
        public int YorumID { get; set; }
        public string YorumIcerik { get; set; } // Kullanıcının yazdığı yorum
        public DateTime Tarih { get; set; }

        public string KullaniciAdi { get; set; } // Yorumu yapan
        public string KullaniciResmi { get; set; }

        // Yorum neyin altına yapılmış?
        public string PaylasimIcerik { get; set; } // Paylaşımın kendisi 
        public string PaylasimTuru { get; set; } // "Alıntı", "İnceleme" vs.
        public int PaylasimID { get; set; }
    }
}