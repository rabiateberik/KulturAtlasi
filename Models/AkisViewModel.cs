using System;

namespace KulturAtlasi.Models
{
    // tabloalrı ekranda göstermek için taşıyıcı model
    public class AkisViewModel
    {
        public string KullaniciID { get; set; }
        public string KullaniciAdi { get; set; }
        public string AdSoyad { get; set; }
        public string ProfilResmi { get; set; }

        public string Eylem { get; set; } 
        public string IcerikBaslik { get; set; }
        public string IcerikResim { get; set; }
        public string IcerikTuru { get; set; } 
        public int IcerikId { get; set; }
   
        public string YorumMetni { get; set; } 
        public int PaylasimID { get; set; } 

        
        public int BegeniSayisi { get; set; }
        public int YorumSayisi { get; set; }
        public bool BegendiMi { get; set; } 

        
        public List<YorumViewModel> Yorumlar { get; set; }
        public DateTime Tarih { get; set; }
        public string ZamanGecenSure => GecenSureHesapla(Tarih);

        
        private string GecenSureHesapla(DateTime tarih)
        {
            var fark = DateTime.Now - tarih;
            if (fark.TotalMinutes < 1) return "Az önce";
            if (fark.TotalMinutes < 60) return $"{(int)fark.TotalMinutes}dk önce";
            if (fark.TotalHours < 24) return $"{(int)fark.TotalHours}sa önce";
            return $"{(int)fark.TotalDays} gün önce";
        }
        public class YorumViewModel
        {
            public string YazanAd { get; set; }
            public string YazanResim { get; set; }
            public string Icerik { get; set; }
            public string Tarih { get; set; }
        }
    }
}