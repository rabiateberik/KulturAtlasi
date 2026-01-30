using KulturAtlasi.Data;
using KulturAtlasi.Models;
using KulturAtlasi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KulturAtlasi.Controllers
{
    [Authorize]
    public class KullanicilarController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly OneriService _oneriService;

        public KullanicilarController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, OneriService oneriService)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _oneriService = oneriService;
        }

        // Listeleme
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var users = await _userManager.Users
                                          .Where(u => u.Id != currentUser.Id && u.GizliHesap == false)
                                          .ToListAsync();
            return View(users);
        }

        // Profil görüntüleme
        public async Task<IActionResult> Profil(string id)
        {
            var aktifKullaniciId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(id))
            {
                id = aktifKullaniciId;
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var takipci = await _context.Takipler.CountAsync(t => t.TakipEdilenID == id);
            var takip = await _context.Takipler.CountAsync(t => t.TakipEdenID == id);
            var kitapSayisi = await _context.Kitaplar.CountAsync(k => k.KullaniciID == id);
            var filmSayisi = await _context.Filmler.CountAsync(f => f.KullaniciID == id);

            var takipEdiyorMuyum = await _context.Takipler.AnyAsync(t => t.TakipEdenID == aktifKullaniciId && t.TakipEdilenID == id);

            // Paylaşımları çek
            var paylasimlar = await _context.Paylasimlar
                .Where(p => p.KullaniciID == id)
                .Include(p => p.Kullanici)
                .OrderByDescending(p => p.Tarih)
                .Select(p => new AkisViewModel
                {
                    PaylasimID = p.PaylasimID,
                    KullaniciID = p.KullaniciID,
                    AdSoyad = p.Kullanici.Ad + " " + p.Kullanici.Soyad,
                    ProfilResmi = p.Kullanici.ProfilFotografiYolu,
                    Eylem = $"bir <strong>{p.PaylasimTuru}</strong> paylaştı:",
                    YorumMetni = p.Icerik,
                    Tarih = p.Tarih,
                    IcerikTuru = p.IlgiliIcerikTuru,
                    IcerikId = p.IlgiliIcerikID ?? 0,

                    // Görsel ve Başlık Ataması
                    IcerikBaslik = p.IlgiliIcerikTuru == "Kitap"
                        ? _context.Kitaplar.Where(k => k.KitapID == p.IlgiliIcerikID).Select(k => k.Baslik).FirstOrDefault()
                        : _context.Filmler.Where(f => f.FilmID == p.IlgiliIcerikID).Select(f => f.Baslik).FirstOrDefault(),

                    IcerikResim = p.IlgiliIcerikTuru == "Kitap"
                        ? _context.Kitaplar.Where(k => k.KitapID == p.IlgiliIcerikID).Select(k => k.KapakResmiYolu).FirstOrDefault()
                        : _context.Filmler.Where(f => f.FilmID == p.IlgiliIcerikID).Select(f => f.PosterYolu).FirstOrDefault(),

                    BegeniSayisi = _context.Begeniler.Count(b => b.PaylasimID == p.PaylasimID),
                    YorumSayisi = _context.Yorumlar.Count(y => y.PaylasimID == p.PaylasimID),
                    BegendiMi = _context.Begeniler.Any(b => b.PaylasimID == p.PaylasimID && b.KullaniciID == aktifKullaniciId)
                }).ToListAsync();

            var model = new ProfilViewModel
            {
                KullaniciID = user.Id,
                AdSoyad = $"{user.Ad} {user.Soyad}",
                KullaniciAdi = user.UserName,
                ProfilResmi = user.ProfilFotografiYolu,
                Biyografi = user.Hakkimda ?? "Kültür Atlası Kaşifi",
                KayitTarihi = DateTime.Now,
                TakipciSayisi = takipci,
                TakipEdilenSayisi = takip,
                OkunanKitap = kitapSayisi,
                IzlenenFilm = filmSayisi,
                ToplamPaylasim = paylasimlar.Count,
                KendiProfilimMi = (id == aktifKullaniciId),
                TakipEdiyorMuyum = takipEdiyorMuyum,
                Paylasimlar = paylasimlar
            };

            ViewBag.CurrentUserId = aktifKullaniciId;
            return View(model);
        }

        // begenileri getir(ajax)
        [HttpGet]
        public async Task<IActionResult> BegenenleriGetir(int paylasimId)
        {
            var data = await _context.Begeniler
                .Include(b => b.Kullanici)
                .Where(b => b.PaylasimID == paylasimId)
                .Select(b => new ListeOgesiViewModel
                {
                    UserId = b.KullaniciID,
                    Baslik = b.Kullanici.Ad + " " + b.Kullanici.Soyad,
                    AltBaslik = "@" + b.Kullanici.UserName,
                    Resim = b.Kullanici.ProfilFotografiYolu,
                    Link = "/Kullanicilar/Profil/" + b.KullaniciID
                }).ToListAsync();

            return PartialView("_ProfilListesiModal", data);
        }

        // yorumları getir(ajax)
        [HttpGet]
        public async Task<IActionResult> YorumlariGetir(int paylasimId)
        {
            var data = await _context.Yorumlar
                .Include(y => y.Kullanici)
                .Where(y => y.PaylasimID == paylasimId)
                .OrderByDescending(y => y.Tarih)
                .Select(y => new AkisViewModel.YorumViewModel
                {
                    YazanAd = y.Kullanici.Ad + " " + y.Kullanici.Soyad,
                    YazanResim = y.Kullanici.ProfilFotografiYolu,
                    Icerik = y.Icerik,
                    Tarih = y.Tarih.ToString("dd.MM.yyyy HH:mm")
                }).ToListAsync();

            return PartialView("_YorumlarListesiModal", data);
        }

        // takip et
        [HttpPost]
        public async Task<IActionResult> TakipEt(string id)
        {
            var benID = _userManager.GetUserId(User);
            var benUser = await _userManager.GetUserAsync(User);
            var hedefUser = await _userManager.FindByIdAsync(id);
            if (hedefUser.GizliHesap) return RedirectToAction("Index");

            var zatenVar = await _context.Takipler.AnyAsync(t => t.TakipEdenID == benID && t.TakipEdilenID == id);

            if (!zatenVar)
            {
                var yeniTakip = new Takip { TakipEdenID = benID, TakipEdilenID = id, TakipTarihi = DateTime.Now };
                _context.Takipler.Add(yeniTakip);

                var bildirim = new Bildirim
                {
                    KullaniciID = id,
                    Tur = "Takip",
                    Mesaj = $"{benUser.UserName} seni takip etmeye başladı.",
                    Link = $"/Kullanicilar/Profil/{benID}",
                    Tarih = DateTime.Now,
                    GorulduMu = false
                };
                _context.Bildirimler.Add(bildirim);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Profil", new { id = id });
        }

        // takibi bırak
        [HttpPost]
        public async Task<IActionResult> TakibiBirak(string id)
        {
            var ben = _userManager.GetUserId(User);
            var takip = await _context.Takipler.FirstOrDefaultAsync(t => t.TakipEdenID == ben && t.TakipEdilenID == id);

            if (takip != null)
            {
                _context.Takipler.Remove(takip);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Profil", new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> ProfilDuzenleGetir()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var model = new ProfilDuzenleViewModel
            {
                Ad = user.Ad,
                Soyad = user.Soyad,
                Meslek = user.Meslek,
                Sehir = user.Sehir,
                Hakkimda = user.Hakkimda,
                MevcutResim = user.ProfilFotografiYolu
            };
            return PartialView("_ProfilDuzenleModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> ProfilDuzenleKaydet(ProfilDuzenleViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return Json(new { success = false, message = "Kullanıcı bulunamadı." });

            user.Ad = model.Ad;
            user.Soyad = model.Soyad;
            user.Meslek = model.Meslek;
            user.Sehir = model.Sehir;
            user.Hakkimda = model.Hakkimda;

            if (model.ProfilResmi != null)
            {
                var uzanti = Path.GetExtension(model.ProfilResmi.FileName);
                var yeniIsim = Guid.NewGuid() + uzanti;
                var klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/profil");
                if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);
                var yol = Path.Combine(klasorYolu, yeniIsim);
                using (var stream = new FileStream(yol, FileMode.Create))
                {
                    await model.ProfilResmi.CopyToAsync(stream);
                }
                user.ProfilFotografiYolu = "/img/profil/" + yeniIsim;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> ProfilListesiGetir(string id, string tur)
        {
            var liste = new List<ListeOgesiViewModel>();

            if (tur == "Takipciler")
            {
                var data = await _context.Takipler.Include(t => t.TakipEden).Where(t => t.TakipEdilenID == id).Select(t => t.TakipEden).ToListAsync();
                foreach (var user in data) liste.Add(new ListeOgesiViewModel { UserId = user.Id, Baslik = user.Ad + " " + user.Soyad, AltBaslik = "@" + user.UserName, Resim = user.ProfilFotografiYolu, Link = "/Kullanicilar/Profil/" + user.Id });
            }
            else if (tur == "TakipEdilen")
            {
                var data = await _context.Takipler.Include(t => t.TakipEdilen).Where(t => t.TakipEdenID == id).Select(t => t.TakipEdilen).ToListAsync();
                foreach (var user in data) liste.Add(new ListeOgesiViewModel { UserId = user.Id, Baslik = user.Ad + " " + user.Soyad, AltBaslik = "@" + user.UserName, Resim = user.ProfilFotografiYolu, Link = "/Kullanicilar/Profil/" + user.Id });
            }
            else if (tur == "Kitap")
            {
                var data = await _context.Kitaplar.Where(x => x.KullaniciID == id).ToListAsync();
                foreach (var item in data) liste.Add(new ListeOgesiViewModel { Id = item.KitapID, Baslik = item.Baslik, AltBaslik = item.Yazar, Resim = item.KapakResmiYolu, Link = "/Kitaplar/Details/" + item.KitapID });
            }
            else if (tur == "Film")
            {
                var data = await _context.Filmler.Where(x => x.KullaniciID == id).ToListAsync();
                foreach (var item in data) liste.Add(new ListeOgesiViewModel { Id = item.FilmID, Baslik = item.Baslik, AltBaslik = item.Yonetmen, Resim = item.PosterYolu, Link = "/Filmler/Details/" + item.FilmID });
            }

            return PartialView("_ProfilListesiModal", liste);
        }

        [HttpGet]
        public async Task<IActionResult> Ayarlar()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");
            return View(new AyarlarViewModel { Eposta = user.Email, GizliHesap = user.GizliHesap });
        }

        [HttpPost]
        public async Task<IActionResult> SifreDegistir(AyarlarViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            if (!string.IsNullOrEmpty(model.MevcutSifre) && !string.IsNullOrEmpty(model.YeniSifre))
            {
                var sonuc = await _userManager.ChangePasswordAsync(user, model.MevcutSifre, model.YeniSifre);
                if (sonuc.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["Basari"] = "Şifren başarıyla güncellendi! 🔒";
                    return RedirectToAction("Ayarlar");
                }
                foreach (var error in sonuc.Errors) ModelState.AddModelError("", error.Description);
            }
            else ModelState.AddModelError("", "Lütfen şifre alanlarını eksiksiz doldur.");

            model.Eposta = user.Email;
            model.GizliHesap = user.GizliHesap;
            return View("Ayarlar", model);
        }

        [HttpPost]
        public async Task<IActionResult> GizlilikGuncelle(bool gizliHesap)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false });
            user.GizliHesap = gizliHesap;
            await _userManager.UpdateAsync(user);
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> AnalizGetir()
        {
            var user = await _userManager.GetUserAsync(User);
            var sonuc = await _oneriService.KulturAnaliziYapAsync(user.Id);
            sonuc.ProfilResmi = user.ProfilFotografiYolu ?? $"https://ui-avatars.com/api/?name={user.Ad}+{user.Soyad}&background=random&size=128";
            return PartialView("_AnalizSonucPartial", sonuc);
        }
    }
}