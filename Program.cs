using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KulturAtlasi.Data;
using KulturAtlasi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Identity Ayarları 
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddRoles<IdentityRole>() 
.AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Cookie Ayarları
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    // Eğer yetkin yoksa (Admin sayfasına girmeye çalışana) bu sayfaya at:
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 4. MVC ve JSON Ayarları
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Servisler
builder.Services.AddScoped<KulturAtlasi.Services.OneriService>();

var app = builder.Build();

// HTTP Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Kimlik Doğrulama
app.UseAuthorization();  // Yetkilendirme

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// --- VERİ TOHUMLAMA (DATA SEEDING) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // 👇 Rol yöneticisini de çağırıyoruz (DbInitializer için lazım olabilir)
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Eğer DbInitializer metodunu 3 parametreli yaptıysan bunu kullan:
        await DbInitializer.Initialize(context, userManager, roleManager);

        // Eğer DbInitializer metodun hala 2 parametreliyse (eski haliyse) alttakini kullan:
        // await DbInitializer.Initialize(context, userManager); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı başlatılırken bir hata oluştu.");
    }
}
// --- Şifre Sıfırlama Bloğu ---
//using (var scope = app.Services.CreateScope())
//{
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//    var user = await userManager.FindByEmailAsync("rabia@gmail.com"); // Buraya kendi e-postanı yaz

//    if (user != null)
//    {
//        var token = await userManager.GeneratePasswordResetTokenAsync(user);
//        var result = await userManager.ResetPasswordAsync(user, token, "YeniSifre123!"); // Yeni şifreni buraya yaz

//        if (result.Succeeded)
//        {
//            Console.WriteLine("Şifre başarıyla sıfırlandı!");
//        }
//    }
//}
// ----------------------------

app.Run();
app.Run();