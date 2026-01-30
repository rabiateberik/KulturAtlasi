using KulturAtlasi.Data;
using KulturAtlasi.Models;
using KulturAtlasi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static KulturAtlasi.Models.AkisViewModel;

namespace KulturAtlasi.Controllers
{
    [Authorize]
    public class KesfetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly OneriService _oneriService;

        public KesfetController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, OneriService oneriService)
        {
            _context = context;
            _userManager = userManager;
            _oneriService = oneriService;
        }

      //Keşfet sayfası
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            
            ViewBag.CurrentUserId = userId;

            //  Takip edilenleri + Kendimi bul (Gizli olmayanları)
            var takipEttiklerim = await _context.Takipler
                .Include(t => t.TakipEdilen)
                .Where(t => t.TakipEdenID == userId && t.TakipEdilen.GizliHesap == false)
                .Select(t => t.TakipEdilenID)
                .ToListAsync();

           

            var akisListesi = new List<AkisViewModel>();

            // Paylasimlari çek
            var paylasimlar = await _context.Paylasimlar
                .Include(p => p.Kullanici)
                .Where(p => takipEttiklerim.Contains(p.KullaniciID))
                .OrderByDescending(p => p.Tarih).Take(30).ToListAsync();

            foreach (var p in paylasimlar)
            {
                string baslik = "";
                string resim = "";

                // İçerik Bilgilerini Çek
                if (p.IlgiliIcerikTuru == "Kitap" && p.IlgiliIcerikID != null)
                {
                    var k = await _context.Kitaplar.FindAsync(p.IlgiliIcerikID);
                    if (k != null) { baslik = k.Baslik; resim = k.KapakResmiYolu; }
                }
                else if (p.IlgiliIcerikTuru == "Film" && p.IlgiliIcerikID != null)
                {
                    var f = await _context.Filmler.FindAsync(p.IlgiliIcerikID);
                    if (f != null) { baslik = f.Baslik; resim = f.PosterYolu; }
                }
                else if (p.IlgiliIcerikTuru == "Dizi" && p.IlgiliIcerikID != null)
                {
                    var d = await _context.Diziler.FindAsync(p.IlgiliIcerikID);
                    if (d != null) { baslik = d.Baslik; resim = d.PosterYolu; }
                }

                // sosyal etkileşim verilerini çek
                var begeniSayisi = await _context.Begeniler.CountAsync(b => b.PaylasimID == p.PaylasimID);
                var yorumSayisi = await _context.Yorumlar.CountAsync(y => y.PaylasimID == p.PaylasimID);
                var begendiMi = await _context.Begeniler.AnyAsync(b => b.PaylasimID == p.PaylasimID && b.KullaniciID == userId);

                // Son yorumları çek 
                var sonYorumlar = await _context.Yorumlar
                    .Where(y => y.PaylasimID == p.PaylasimID)
                    .Include(y => y.Kullanici)
                    .OrderBy(y => y.Tarih)
                    .Take(3)
                    .Select(y => new YorumViewModel
                    {
                        YazanAd = y.Kullanici.Ad + " " + y.Kullanici.Soyad,
                        YazanResim = y.Kullanici.ProfilFotografiYolu,
                        Icerik = y.Icerik,
                        Tarih = y.Tarih.ToString("HH:mm")
                    }).ToListAsync();

                // Listeye Ekle
                akisListesi.Add(new AkisViewModel
                {
                    KullaniciID = p.KullaniciID,
                    AdSoyad = $"{p.Kullanici.Ad} {p.Kullanici.Soyad}",
                    ProfilResmi = p.Kullanici.ProfilFotografiYolu,
                    Eylem = $"bir <strong>{p.PaylasimTuru}</strong> paylaştı:",
                    YorumMetni = p.Icerik,
                    IcerikBaslik = baslik,
                    IcerikResim = resim,
                    IcerikTuru = p.IlgiliIcerikTuru ?? "Düşünce",
                    IcerikId = p.IlgiliIcerikID ?? 0,
                    Tarih = p.Tarih,
                    PaylasimID = p.PaylasimID,
                    BegeniSayisi = begeniSayisi,
                    YorumSayisi = yorumSayisi,
                    BegendiMi = begendiMi,
                    Yorumlar = sonYorumlar
                });
            }

            // Paylaşım kutusu dropdownları için veriler
            ViewBag.Kitaplarim = await _context.Kitaplar.Where(k => k.KullaniciID == userId).Select(x => new { Id = x.KitapID, Ad = x.Baslik }).ToListAsync();
            ViewBag.Filmlerim = await _context.Filmler.Where(f => f.KullaniciID == userId).Select(x => new { Id = x.FilmID, Ad = x.Baslik }).ToListAsync();
            ViewBag.Dizilerim = await _context.Diziler.Where(d => d.KullaniciID == userId).Select(x => new { Id = x.DiziID, Ad = x.Baslik }).ToListAsync();

            return View(akisListesi);
        }

   
        // Paylaşım , beğeni,yorum işlemleri

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

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Begen(int id)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);
            var paylasim = await _context.Paylasimlar.FindAsync(id);

            var begeni = await _context.Begeniler.FirstOrDefaultAsync(b => b.PaylasimID == id && b.KullaniciID == userId);
            bool begenildi = false;

            if (begeni != null)
            {
                _context.Begeniler.Remove(begeni);
            }
            else
            {
                _context.Begeniler.Add(new Begeni { PaylasimID = id, KullaniciID = userId });
                begenildi = true;

                if (paylasim.KullaniciID != userId)
                {
                    string link = $"/Kesfet/Index#post-{paylasim.PaylasimID}";

                    var bildirim = new Bildirim
                    {
                        KullaniciID = paylasim.KullaniciID,
                        Tur = "Beğeni",
                        Mesaj = $"{user.UserName} gönderini beğendi.",
                        Link = link,
                        Tarih = DateTime.Now,
                        GorulduMu = false
                    };
                    _context.Bildirimler.Add(bildirim);
                }
            }

            await _context.SaveChangesAsync();
            var yeniSayi = await _context.Begeniler.CountAsync(b => b.PaylasimID == id);
            return Json(new { success = true, begenildi = begenildi, sayi = yeniSayi });
        }

        [HttpPost]
        public async Task<IActionResult> YorumYap(int id, string yorum)
        {
            if (string.IsNullOrWhiteSpace(yorum)) return Json(new { success = false });

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);
            var paylasim = await _context.Paylasimlar.FindAsync(id);

            var yeniYorum = new Yorum
            {
                PaylasimID = id,
                KullaniciID = userId,
                Icerik = yorum,
                Tarih = DateTime.Now
            };

            _context.Yorumlar.Add(yeniYorum);

            if (paylasim.KullaniciID != userId)
            {
                string link = $"/Kesfet/Index#post-{paylasim.PaylasimID}";

                var bildirim = new Bildirim
                {
                    KullaniciID = paylasim.KullaniciID,
                    Tur = "Yorum",
                    Mesaj = $"{user.UserName} gönderine yorum yaptı.",
                    Link = link,
                    Tarih = DateTime.Now,
                    GorulduMu = false
                };
                _context.Bildirimler.Add(bildirim);
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                ad = user.Ad + " " + user.Soyad,
                resim = user.ProfilFotografiYolu,
                yorum = yorum,
                tarih = DateTime.Now.ToString("HH:mm")
            });
        }

        
        // Paylaşım düzenleme ve silme modal

        // düzenleme penceresini getir
        [HttpGet]
        public async Task<IActionResult> PaylasimDuzenleGetir(int id)
        {
            var userId = _userManager.GetUserId(User);

            // Paylaşımı bul ve sahibi kim kontrol et (Güvenlik Şart!)
            var paylasim = await _context.Paylasimlar.FirstOrDefaultAsync(x => x.PaylasimID == id && x.KullaniciID == userId);

            if (paylasim == null) return NotFound();

            // Partial View döndür
            return PartialView("_GonderiDuzenle", paylasim);
        }

        // güncellenen veriyi kaydet
        [HttpPost]
        public async Task<IActionResult> PaylasimDuzenleKaydet(Paylasim model)
        {
            var userId = _userManager.GetUserId(User);

            var paylasim = await _context.Paylasimlar.FirstOrDefaultAsync(x => x.PaylasimID == model.PaylasimID && x.KullaniciID == userId);

            if (paylasim == null) return Json(new { success = false, message = "Yetkisiz işlem veya paylaşım bulunamadı." });

            paylasim.Icerik = model.Icerik; // Sadece içeriği güncelle

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Paylaşımın başarıyla güncellendi!" });
        }

        // Silme işlemi
        [HttpPost]
        public async Task<IActionResult> PaylasimSil(int id)
        {
            var userId = _userManager.GetUserId(User);
            var paylasim = await _context.Paylasimlar.FirstOrDefaultAsync(x => x.PaylasimID == id && x.KullaniciID == userId);

            if (paylasim == null) return Json(new { success = false, message = "Hata oluştu." });

            // Önce ilişkili yorum ve beğenileri temizle (SQL Hatası almamak için)
            var begeniler = _context.Begeniler.Where(b => b.PaylasimID == id);
            _context.Begeniler.RemoveRange(begeniler);

            var yorumlar = _context.Yorumlar.Where(y => y.PaylasimID == id);
            _context.Yorumlar.RemoveRange(yorumlar);

            _context.Paylasimlar.Remove(paylasim);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> BanaBunuSat(int id, string tur)
        {
            var userId = _userManager.GetUserId(User);

            // Servisi çağır
            string iknaMetni = await _oneriService.IknaEtAsync(userId, id, tur);

            return Json(new { success = true, text = iknaMetni });
        }
        [HttpGet]
        public async Task<IActionResult> BunaBenzerOner(string ad, string tur)
        {
            // Servisi çağır
            string oneriMetni = await _oneriService.BenzeriniOnerAsync(ad, tur);

            return Json(new { success = true, text = oneriMetni });
        }
    }
}