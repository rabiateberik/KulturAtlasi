using System.Collections.Generic;

namespace KulturAtlasi.Models
{
    public class UyeListesiViewModel
    {
        public List<ApplicationUser> Users { get; set; } // O sayfadaki kullanıcılar
        public int CurrentPage { get; set; }             // Şu anki sayfa no
        public int TotalPages { get; set; }              // Toplam sayfa sayısı
        public string SearchTerm { get; set; }           // Arama kelimesi
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}