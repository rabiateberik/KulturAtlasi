using KulturAtlasi.Data;
using KulturAtlasi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using KulturAtlasi.Services; // Yapay Zeka Servisi 

namespace KulturAtlasi.Controllers
{
    [Authorize] // Sadece giriş yapanlar görebilsin
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

       
        private readonly OneriService _oneriService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager,OneriService oneriService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _oneriService = oneriService;
        }

        // 1. ANA SAYFA 
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);

            // Verileri Çek
            var kitaplar = await _context.Kitaplar.Where(x => x.KullaniciID == userId).ToListAsync();
            var filmler = await _context.Filmler.Where(x => x.KullaniciID == userId).ToListAsync();
            var diziler = await _context.Diziler.Where(x => x.KullaniciID == userId).ToListAsync();
            var seyahatler = await _context.Seyahatler.Where(x => x.KullaniciID == userId).ToListAsync();
            var muzikler = await _context.Muzikler.Where(x => x.KullaniciID == userId).ToListAsync();

            var gezilenUlkeSayisi = seyahatler.Select(s => s.Ulke).Distinct().Count();

            // Dashboard Modeli
            var model = new DashboardViewModel
            {
                ToplamKitap = kitaplar.Count,
                ToplamFilm = filmler.Count,
                ToplamDizi = diziler.Count,
                ToplamMuzik = muzikler.Count,
                ToplamSeyahat = seyahatler.Count,
                GezilenUlkeSayisi = gezilenUlkeSayisi,

                SonKitap = kitaplar.OrderByDescending(k => k.KayitTarihi).FirstOrDefault(),
                SonFilm = filmler.OrderByDescending(f => f.KayitTarihi).FirstOrDefault(),
                SonDizi = diziler.OrderByDescending(d => d.KayitTarihi).FirstOrDefault(),
                SonSeyahat = seyahatler.OrderByDescending(s => s.ZiyaretTarihi).FirstOrDefault(),

                KitapTurDagilimi = kitaplar.Where(k => !string.IsNullOrEmpty(k.Tur)).GroupBy(k => k.Tur).ToDictionary(g => g.Key, g => g.Count()),
                FilmTurDagilimi = filmler.Where(f => !string.IsNullOrEmpty(f.Tur)).GroupBy(f => f.Tur).ToDictionary(g => g.Key, g => g.Count()),

               
                Oneriler = new List<OneriItem>()
            };

            ViewBag.AdSoyad = $"{user.Ad} {user.Soyad}";
            ViewBag.ProfilResmi = user.ProfilFotografiYolu;

            return View(model);
        }

      
        [HttpGet]
        public async Task<IActionResult> YapayZekaOneriGetir()
        {
            var userId = _userManager.GetUserId(User);

            // veritabanı işlemleri ve prompt hazırlama servisin içinde yapılıyor.
            // Biz sadece UserID gönderiyoruz, o bize hazır listeyi (List<OneriItem>) veriyor.
            var oneriListesi = await _oneriService.GetOneriAsync(userId);

          
            return Json(new { success = true, oneriler = oneriListesi });
        }
      
        //Paylaşım yap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaylasimYap(string Icerik, string PaylasimTuru, string IlgiliIcerikTuru, int? IlgiliIcerikID)
        {
            var userId = _userManager.GetUserId(User);

            if (PaylasimTuru == "Düşünce")
            {
                IlgiliIcerikTuru = null;
                IlgiliIcerikID = null;
            }

            var yeniPaylasim = new Paylasim
            {
                KullaniciID = userId,
                Icerik = Icerik,
                PaylasimTuru = PaylasimTuru,
                Tarih = DateTime.Now,
                IlgiliIcerikTuru = IlgiliIcerikTuru,
                IlgiliIcerikID = IlgiliIcerikID
            };

            _context.Paylasimlar.Add(yeniPaylasim);
            await _context.SaveChangesAsync();

            TempData["Basarili"] = "Gönderiniz başarıyla paylaşıldı!";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<IActionResult> GetKullaniciKutuphanesi()
        {
            var userId = _userManager.GetUserId(User);
            var veriler = new
            {
                kitaplar = await _context.Kitaplar.Where(k => k.KullaniciID == userId).Select(x => new { id = x.KitapID, ad = x.Baslik }).ToListAsync(),
                filmler = await _context.Filmler.Where(f => f.KullaniciID == userId).Select(x => new { id = x.FilmID, ad = x.Baslik }).ToListAsync(),
                diziler = await _context.Diziler.Where(d => d.KullaniciID == userId).Select(x => new { id = x.DiziID, ad = x.Baslik }).ToListAsync()
            };
            return Json(veriler);
        }
        //Geri bildirim
        [HttpGet]
        public async Task<IActionResult> GetBildirimler()
        {
            var userId = _userManager.GetUserId(User);

            var bildirimler = await _context.Bildirimler
                .Where(b => b.KullaniciID == userId)
                .OrderByDescending(b => b.Tarih)
                .Take(10)
                .Select(b => new
                {
                    b.Mesaj,
                    b.Link,
                    b.Tur,
                    Tarih = b.Tarih.ToString("dd.MM HH:mm"),
                    Okundu = b.GorulduMu
                })
                .ToListAsync();

            var okunmamislar = await _context.Bildirimler.Where(b => b.KullaniciID == userId && !b.GorulduMu).ToListAsync();
            if (okunmamislar.Any())
            {
                foreach (var item in okunmamislar) item.GorulduMu = true;
                await _context.SaveChangesAsync();
            }

            return Json(bildirimler);
        }

        [HttpGet]
        public async Task<IActionResult> GetOkunmamisBildirimSayisi()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Json(0);
            var sayi = await _context.Bildirimler.CountAsync(b => b.KullaniciID == userId && b.GorulduMu == false);
            return Json(sayi);
        }
        //Arama kısmı
        [HttpGet]
        public async Task<IActionResult> Ara(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return RedirectToAction("Index");

            var model = new AramaViewModel
            {
                AramaKelimesi = q,
                Kullanicilar = await _userManager.Users.Where(u => u.UserName.Contains(q) || u.Ad.Contains(q) || u.Soyad.Contains(q)).Take(5).ToListAsync(),
                Kitaplar = await _context.Kitaplar.Where(k => k.Baslik.Contains(q) || k.Yazar.Contains(q)).Include(k => k.Kullanici).OrderByDescending(k => k.KayitTarihi).Take(5).ToListAsync(),
                Filmler = await _context.Filmler.Where(f => f.Baslik.Contains(q) || f.Yonetmen.Contains(q)).Include(f => f.Kullanici).OrderByDescending(f => f.KayitTarihi).Take(5).ToListAsync(),
                Diziler = await _context.Diziler.Where(d => d.Baslik.Contains(q)).Include(d => d.Kullanici).OrderByDescending(d => d.KayitTarihi).Take(5).ToListAsync()
            };

            return View(model);
        }

        //İletişim
        // Formu Göster 
        [HttpGet]
        public IActionResult Iletisim()
        {
            return View();
        }

        // Formu Kaydet 
        [HttpPost]
        public async Task<IActionResult> Iletisim(Iletisim model)
        {
            if (ModelState.IsValid)
            {
                _context.IletisimMesajlari.Add(model);
                await _context.SaveChangesAsync();

                TempData["Basarili"] = "Mesajınız bize ulaştı! En kısa sürede dönüş yapacağız.";
                return RedirectToAction("Iletisim");
            }
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult Privacy() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() { return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); }
    }
}