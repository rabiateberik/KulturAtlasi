using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KulturAtlasi.Models;
using System.Linq;

namespace KulturAtlasi.Data
{
    // Identity için ApplicationUser modelini kullanacağımızı belirtiyoruz.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //DbSet Tanımları
        public DbSet<Durum> Durumlar { get; set; }


        public DbSet<Kitap> Kitaplar { get; set; }
        public DbSet<Film> Filmler { get; set; }
        public DbSet<Dizi> Diziler { get; set; }
        public DbSet<Muzik> Muzikler { get; set; }
        public DbSet<Seyahat> Seyahatler { get; set; }
        public DbSet<GezilecekYer> GezilecekYerler { get; set; }

        
        public DbSet<Paylasim> Paylasimlar { get; set; }
        public DbSet<Yorum> Yorumlar { get; set; }
        public DbSet<Begeni> Begeniler { get; set; }
        public DbSet<Mesaj> Mesajlar { get; set; }
        public DbSet<Oneri> Oneriler { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }
        public DbSet<AktiviteKaydi> AktiviteKayitlari { get; set; }

       
       
        public DbSet<Etiket> Etiketler { get; set; }
        public DbSet<EtiketliIcerik> EtiketliIcerikler { get; set; }
        public DbSet<Takip> Takipler { get; set; }
        public DbSet<Iletisim> IletisimMesajlari { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // İlişkisel Silme Kuralını Ayarlama (Cascade Delete Hatalarını Engeller)
            // Identity tabloları dışındaki tüm Foreign Key'ler için silme davranışını Kısıtla (Restrict) olarak ayarlar.
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            builder.Entity<Takip>()
                .HasOne(t => t.TakipEden)
                .WithMany()
                .HasForeignKey(t => t.TakipEdenID)
                .OnDelete(DeleteBehavior.Restrict); // Hata vermemesi için şart!

            builder.Entity<Takip>()
                .HasOne(t => t.TakipEdilen)
                .WithMany()
                .HasForeignKey(t => t.TakipEdilenID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}