using KulturAtlasi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KulturAtlasi.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Veritabanı yoksa oluştur
            context.Database.EnsureCreated();

           
            // Addmin rolü 
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            
            //durum verileri
            var tumDurumlar = new Durum[]
            {
                // Kitaplar
                new Durum{Ad="Okudum", Kategori="Kitap"},
                new Durum{Ad="Okuyacağım", Kategori="Kitap"},
                new Durum{Ad="Yarıda Bıraktım", Kategori="Kitap"},

                // Filmler
                new Durum{Ad="İzledim", Kategori="Film"},
                new Durum{Ad="İzleyeceğim", Kategori="Film"},
                new Durum{Ad="Yarıda Bıraktım", Kategori="Film"},

                // Diziler
                new Durum{Ad="İzledim", Kategori="Dizi"},
                new Durum{Ad="İzliyorum", Kategori="Dizi"},
                new Durum{Ad="İzleyeceğim", Kategori="Dizi"},
                new Durum{Ad="Yarıda Bıraktım", Kategori="Dizi"},

                // Müzikler
                new Durum{Ad="Dinledim", Kategori="Muzik"},
                new Durum{Ad="Favorilerim", Kategori="Muzik"},

                // Seyahatler
                new Durum{Ad="Gittim", Kategori="Seyahat"},
                new Durum{Ad="Gitmek İstiyorum", Kategori="Seyahat"},
                new Durum{Ad="Favorilerim", Kategori="Seyahat"}
            };

            // Veritabanında eksik olanları tamamla
            foreach (var d in tumDurumlar)
            {
                if (!context.Durumlar.Any(x => x.Ad == d.Ad && x.Kategori == d.Kategori))
                {
                    context.Durumlar.Add(d);
                }
            }

            await context.SaveChangesAsync();

           //Admin atama

          
            string adminEmail = "rabia@gmail.com";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser != null)
            {
              
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}