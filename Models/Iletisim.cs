using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulturAtlasi.Models
{
    [Table("IletisimMesajlari")]
    public class Iletisim
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad boş geçilemez.")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Email adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Konu boş geçilemez.")]
        public string Konu { get; set; }

        [Required(ErrorMessage = "Mesaj boş geçilemez.")]
        public string Mesaj { get; set; }

        public DateTime Tarih { get; set; } = DateTime.Now;

        public bool OkunduMu { get; set; } = false; // Yeni mesajlar "Okunmadı" olarak gelir
    }
}