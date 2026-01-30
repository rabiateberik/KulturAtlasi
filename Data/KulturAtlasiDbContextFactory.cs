using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace KulturAtlasi.Data
{
    // IDesignTimeDbContextFactory arayüzünü uyguluyoruz
    public class KulturAtlasiDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            //  appsettings.json dosyasını bulmak için konfigurasyon oluşturma
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            //  appsettings.json dosyasından bağlantı dizinini okuma
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // DbContextOptions'ı oluşturma
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

           
            builder.UseSqlServer(connectionString);

           
            return new ApplicationDbContext(builder.Options);
        }
    }
}