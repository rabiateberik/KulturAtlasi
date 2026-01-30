using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KulturAtlasi.Data;
using KulturAtlasi.Models;
using KulturAtlasi.ViewModels;

namespace KulturAtlasi.Controllers
{
    // Buraya sadece "Admin" rolü olanlar girebilir!
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        //login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) return RedirectToAction("Index");
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Böyle bir yetkili bulunamadı.");
                return View(model);
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError("", "Bu alana giriş yetkiniz yok!");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded) return RedirectToAction("Index");

            ModelState.AddModelError("", "Şifre hatalı.");
            return View(model);
        }

        //Dashboard
        public async Task<IActionResult> Index()
        {
            // 1. Son 6 ayı hesapla (Ocak, Şubat vb. isimler için)
            var sonAltiAy = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .OrderBy(d => d) // Eskiden yeniye sırala
                .ToList();

            var ayIsimleri = sonAltiAy.Select(d => d.ToString("MMMM")).ToArray();
            var kayitRakamlari = new int[6];

            // 2. Her ay için kayıtlı üye sayısını veritabanından çek
            for (int i = 0; i < 6; i++)
            {
                var hedefAy = sonAltiAy[i].Month;
                var hedefYil = sonAltiAy[i].Year;

                kayitRakamlari[i] = await _userManager.Users
                    .CountAsync(u => u.KayitTarihi.Month == hedefAy && u.KayitTarihi.Year == hedefYil);
            }

            // 3. ViewModel'i doldur
            var model = new AdminDashboardViewModel
            {
                ToplamUye = await _userManager.Users.CountAsync(),
                ToplamKitap = await _context.Kitaplar.CountAsync(),
                ToplamFilm = await _context.Filmler.CountAsync(),
                // Toplam yorumu merkezi tablodan çekiyoruz
                ToplamYorum = await _context.Yorumlar.CountAsync(),
                SonUyeler = await _userManager.Users
                    .OrderByDescending(u => u.KayitTarihi)
                    .Take(5)
                    .ToListAsync()
            };

            // 4. ViewBag verilerini gönder
            ViewBag.OkunmamisMesajSayisi = await _context.IletisimMesajlari.CountAsync(x => !x.OkunduMu);
            ViewBag.Aylar = ayIsimleri;
            ViewBag.KayitSayilari = kayitRakamlari;

            return View(model);
        }

        //Üye yönetimi
        public async Task<IActionResult> Uyeler(string search, int page = 1)
        {
            int pageSize = 10;
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Email.Contains(search) || u.UserName.Contains(search));
            }

            var totalUsers = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var users = await query.OrderByDescending(u => u.KayitTarihi)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var model = new UyeListesiViewModel
            {
                Users = users,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchTerm = search
            };
            return View(model);
        }

        //Rol yönetimi
        [HttpGet]
        public async Task<IActionResult> RolAta(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var model = new RolAtaViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roller = allRoles.Select(role => new RolSecim
                {
                    RolId = role.Id,
                    RolAdi = role.Name,
                    SeciliMi = userRoles.Contains(role.Name)
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RolAta(RolAtaViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var secilenRoller = model.Roller.Where(r => r.SeciliMi).Select(r => r.RolAdi).ToList();

            await _userManager.AddToRolesAsync(user, secilenRoller.Except(userRoles));
            await _userManager.RemoveFromRolesAsync(user, userRoles.Except(secilenRoller));

            TempData["Mesaj"] = $"{user.UserName} yetkileri güncellendi.";
            return RedirectToAction("Uyeler");
        }

        //üye silme
        [HttpPost]
        public async Task<IActionResult> UyeSil(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser.Id == user.Id) { TempData["Hata"] = "Kendini silemezsin!"; return RedirectToAction("Uyeler"); }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded) TempData["Mesaj"] = "Kullanıcı silindi.";
                else TempData["Hata"] = "Hata oluştu.";
            }
            return RedirectToAction("Uyeler");
        }

        //kitap yönetimi
        public async Task<IActionResult> KitapYonetimi(int page = 1)
        {
            int pageSize = 10;
            var query = _context.Kitaplar.Include(k => k.Kullanici).AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var kitaplar = await query.OrderByDescending(k => k.KayitTarihi)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(kitaplar);
        }

        [HttpPost]
        public async Task<IActionResult> KitapSilAdmin(int id)
        {
            var kitap = await _context.Kitaplar.FindAsync(id);
            if (kitap != null)
            {
                _context.Kitaplar.Remove(kitap);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Kitap sistemden tamamen silindi.";
            }
            else { TempData["Hata"] = "Kitap bulunamadı."; }
            return RedirectToAction("KitapYonetimi");
        }

        //film yönetimi
        public async Task<IActionResult> FilmYonetimi(int page = 1)
        {
            int pageSize = 10;
            var query = _context.Filmler.Include(f => f.Kullanici).AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var filmler = await query.OrderByDescending(f => f.KayitTarihi)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(filmler);
        }

        [HttpPost]
        public async Task<IActionResult> FilmSilAdmin(int id)
        {
            var film = await _context.Filmler.FindAsync(id);
            if (film != null)
            {
                _context.Filmler.Remove(film);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Film sistemden tamamen silindi.";
            }
            else { TempData["Hata"] = "Film bulunamadı."; }
            return RedirectToAction("FilmYonetimi");
        }
       //dizi yönetimi
        public async Task<IActionResult> DiziYonetimi(int page = 1)
        {
            int pageSize = 10;
            // DİKKAT: Senin modelinde "Kullanici" ilişkisi yoksa .Include kısmını sil.
            var query = _context.Diziler.Include(d => d.Kullanici).AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var diziler = await query.OrderByDescending(d => d.KayitTarihi)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(diziler);
        }

        [HttpPost]
        public async Task<IActionResult> DiziSilAdmin(int id)
        {
            var dizi = await _context.Diziler.FindAsync(id);
            if (dizi != null)
            {
                _context.Diziler.Remove(dizi);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Dizi sistemden tamamen silindi.";
            }
            else { TempData["Hata"] = "Dizi bulunamadı."; }
            return RedirectToAction("DiziYonetimi");
        }
        //Müzik yönetimi
        public async Task<IActionResult> MuzikYonetimi(int page = 1)
        {
            int pageSize = 10;
            var query = _context.Muzikler.Include(m => m.Kullanici).AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var muzikler = await query.OrderByDescending(m => m.KayitTarihi)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(muzikler);
        }

        [HttpPost]
        public async Task<IActionResult> MuzikSilAdmin(int id)
        {
            var muzik = await _context.Muzikler.FindAsync(id);
            if (muzik != null)
            {
                _context.Muzikler.Remove(muzik);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Müzik sistemden tamamen silindi.";
            }
            else { TempData["Hata"] = "Müzik bulunamadı."; }
            return RedirectToAction("MuzikYonetimi");
        }

        //Seyahat yönetimi
        public async Task<IActionResult> SeyahatYonetimi(int page = 1)
        {
            int pageSize = 10;
            var query = _context.Seyahatler.Include(s => s.Kullanici).AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var seyahatler = await query.OrderByDescending(s => s.ZiyaretTarihi)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(seyahatler);
        }

        [HttpPost]
        public async Task<IActionResult> SeyahatSilAdmin(int id)
        {
            var seyahat = await _context.Seyahatler.FindAsync(id);
            if (seyahat != null)
            {
                _context.Seyahatler.Remove(seyahat);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Seyahat kaydı silindi.";
            }
            else { TempData["Hata"] = "Seyahat bulunamadı."; }
            return RedirectToAction("SeyahatYonetimi");
        }

        //yorum yönetimi
        public async Task<IActionResult> YorumYonetimi()
        {
            
            // yorumlar tablosuyla paylasim tablosu join ediliyor

            var yorumlar = await _context.Yorumlar
                .Include(y => y.Kullanici) 
                .Include(y => y.Paylasim)  
                .OrderByDescending(y => y.Tarih) 
                .Select(y => new AdminYorumViewModel
                {
                    YorumID = y.YorumID,
                    YorumIcerik = y.Icerik,
                    Tarih = y.Tarih,
                    KullaniciAdi = y.Kullanici.UserName,

                   
                    PaylasimID = y.PaylasimID,
                    PaylasimIcerik = y.Paylasim.Icerik.Length > 50
                                     ? y.Paylasim.Icerik.Substring(0, 50) + "..." 
                                     : y.Paylasim.Icerik,
                    PaylasimTuru = y.Paylasim.PaylasimTuru 
                })
                .ToListAsync();

            return View(yorumlar);
        }

        [HttpPost]
        public async Task<IActionResult> YorumSil(int id)
        {
         
            var yorum = await _context.Yorumlar.FindAsync(id);
            if (yorum != null)
            {
                _context.Yorumlar.Remove(yorum);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Yorum başarıyla silindi.";
            }
            else
            {
                TempData["Hata"] = "Yorum bulunamadı.";
            }
            return RedirectToAction("YorumYonetimi");
        }
        //gelen kutusu
        public async Task<IActionResult> GelenKutusu()
        {
            ViewBag.OkunmamisMesajSayisi = _context.IletisimMesajlari.Count(x => !x.OkunduMu);
            var mesajlar = await _context.IletisimMesajlari
                .OrderByDescending(m => m.Tarih)
                .ToListAsync();

            return View(mesajlar);
        }

        [HttpPost]
        public async Task<IActionResult> MesajSil(int id)
        {
            var mesaj = await _context.IletisimMesajlari.FindAsync(id);
            if (mesaj != null)
            {
                _context.IletisimMesajlari.Remove(mesaj);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Mesaj silindi.";
            }
            return RedirectToAction("GelenKutusu");
        }

        // İsteğe bağlı: Okundu olarak işaretleme
        [HttpPost]
        public async Task<IActionResult> MesajOkunduIsaretle(int id)
        {
            var mesaj = await _context.IletisimMesajlari.FindAsync(id);
            if (mesaj != null)
            {
                mesaj.OkunduMu = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("GelenKutusu");
        }

        [HttpPost]
        public async Task<IActionResult> KullaniciBanla(string userId, string sure)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            DateTimeOffset? lockoutEnd;

            switch (sure)
            {
                case "3Ay":
                    lockoutEnd = DateTimeOffset.UtcNow.AddMonths(3);
                    break;
                case "6Ay":
                    lockoutEnd = DateTimeOffset.UtcNow.AddMonths(6);
                    break;
                case "Tamamen":
                    lockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // Sınırsız
                    break;
                default:
                    return BadRequest();
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

            if (result.Succeeded)
            {
                TempData["Mesaj"] = $"{user.UserName} başarıyla banlandı ({sure}).";
            }
            else
            {
                TempData["Hata"] = "Banlama işlemi sırasında bir hata oluştu.";
            }

            return Redirect(Request.Headers["Referer"].ToString() ?? "/Admin/Uyeler");
        }
        [HttpPost]
        public async Task<IActionResult> KullaniciBanKaldir(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

          
            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (result.Succeeded)
            {
                TempData["Mesaj"] = $"{user.UserName} kullanıcısının yasağı kaldırıldı.";
            }
            else
            {
                TempData["Hata"] = "Yasak kaldırılırken bir hata oluştu.";
            }

            return Redirect(Request.Headers["Referer"].ToString() ?? "/Admin/Uyeler");
        }
        // 1. Kullanıcı durumunu kontrol eden metot (AJAX için)
        [HttpGet]
        public async Task<IActionResult> KullaniciYetkiKontrol(string email)
        {
            // 1. İşlemi yapan kişi Süper Admin mi
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Email != "rabia@gmail.com")
            {
                return Json(new { success = false, message = "Bu sorgulama yetkisi sadece Üst Yöneticiye aittir." });
            }

            //  E-posta boş mu kontrolü
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Lütfen bir e-posta adresi girin." });
            }

            // Hedef kullanıcıyı bul
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(new { success = false, message = "Sistemde bu e-posta ile kayıtlı bir kullanıcı bulunamadı." });
            }

            // Durumu dön
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return Json(new
            {
                success = true,
                userId = user.Id,
                userName = user.UserName,
                isAdmin = isAdmin
            });
        }

        // Yetki Değiştirme Metodu (Ver/Al)
        [HttpPost]
        public async Task<IActionResult> YetkiDegistir(string userId, bool yapilsinMi)
        {
            // kişi kontrolü
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Email != "rabia@gmail.com") 
            {
                TempData["Hata"] = "Sadece Süper Admin yetki yönetimi yapabilir!";
                return RedirectToAction("Index");
            }

            var targetUser = await _userManager.FindByIdAsync(userId);
            if (targetUser == null) return NotFound();

            // Süper adminin kendi yetkisiyle oynanmasın
            if (targetUser.Email == "rabia@gmail.com" && !yapilsinMi)
            {
                TempData["Hata"] = "Kendi yöneticilik yetkini kaldıramazsın!";
                return RedirectToAction("Index");
            }

            IdentityResult result;
            if (yapilsinMi)
                result = await _userManager.AddToRoleAsync(targetUser, "Admin");
            else
                result = await _userManager.RemoveFromRoleAsync(targetUser, "Admin");

            if (result.Succeeded)
            {
                TempData["Mesaj"] = "Yetki güncelleme işlemi başarılı.";
                return RedirectToAction("Index");
            }
            return BadRequest();
        }
    }
}