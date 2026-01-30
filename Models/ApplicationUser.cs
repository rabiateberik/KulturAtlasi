using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace KulturAtlasi.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Ad { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Soyad { get; set; } = string.Empty;

        [Required] // <-- YENİ EKLENEN KULLANICI ADI
        [StringLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty;

        [StringLength(255)]
        public string? ProfilFotografiYolu { get; set; }

        public DateTime KayitTarihi { get; set; } = DateTime.Now;
        // Mevcutların altına ekle:
        public string? Meslek { get; set; } // Örn: Yazılım Mühendisi
        public string? Sehir { get; set; }  // Örn: İstanbul
        public string? Hakkimda { get; set; } // Biyografi yazısı
        // Navigation Properties (İlişkiler)
        public virtual ICollection<Kitap>? Kitaplar { get; set; }
        public virtual ICollection<Film>? Filmler { get; set; }
        public virtual ICollection<Dizi>? Diziler { get; set; }
        public virtual ICollection<Muzik>? Muzikler { get; set; }
        public virtual ICollection<Seyahat>? Seyahatler { get; set; }
        public virtual ICollection<Paylasim>? Paylasimlar { get; set; }
        public virtual ICollection<Yorum>? Yorumlar { get; set; }
        public virtual ICollection<Begeni>? Begeniler { get; set; }
        public virtual ICollection<Oneri>? Oneriler { get; set; }
        public virtual ICollection<Bildirim>? Bildirimler { get; set; }
        public virtual ICollection<AktiviteKaydi>? AktiviteKayitlari { get; set; }

        [InverseProperty("Gonderen")]
        public virtual ICollection<Mesaj>? GonderilenMesajlar { get; set; }
        [InverseProperty("Alici")]
        public virtual ICollection<Mesaj>? AlinanMesajlar { get; set; }
        // Gizlilik Ayarı (True = Gizli, False = Herkese Açık)
        public bool GizliHesap { get; set; } = false;
    }
}