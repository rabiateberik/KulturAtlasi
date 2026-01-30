using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json; // JSON işlemleri için
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KulturAtlasi.Data;
using KulturAtlasi.Models;
using System.Text.Json.Serialization;

namespace KulturAtlasi.Controllers
{
    [Authorize]
    public class SeyahatlerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public SeyahatlerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // Seyahatler
        public async Task<IActionResult> Index(string durum)
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.Seyahatler
                .Include(s => s.Durum)
                .Where(s => s.KullaniciID == userId);

            if (!string.IsNullOrEmpty(durum))
            {
                //filtreleme
                if (durum == "Gittim") durum = "Gittim";
                if (durum == "GitmekIstiyorum") durum = "Gitmek İstiyorum";
                if (durum == "Favorilerim") durum = "Favorilerim";

                query = query.Where(s => s.Durum.Ad == durum);
            }

            var seyahatListesi = await query.ToListAsync();

            var haritaVerisi = seyahatListesi.Select(x => new
            {
                baslik = x.Baslik ?? "Başlıksız",
                sehir = x.Sehir ?? "",
                ulke = x.Ulke ?? "",
                tur = x.Tur ?? "",
                enlem = x.Enlem,
                boylam = x.Boylam
            }).ToList(); 

            // döngüsel referansları tamamen yok sayması için seçenek ekliyoruz
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = false
            };

            ViewBag.HaritaJson = JsonSerializer.Serialize(haritaVerisi, jsonOptions);

            return View(seyahatListesi);
        }

        // Seyahatler/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var seyahat = await _context.Seyahatler
                .Include(s => s.Durum)
                .FirstOrDefaultAsync(m => m.SeyahatID == id);

            if (seyahat == null) return NotFound();

            return View(seyahat);
        }

        // Seyahatler/Create
        public IActionResult Create()
        {
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Seyahat"), "DurumID", "Ad");
            return View();
        }

        //Seyahatler/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SeyahatID,Baslik,Sehir,Ulke,Tur,GeziNotu,ZiyaretTarihi,Enlem,Boylam,DurumID")] Seyahat seyahat, IFormFile? resimDosyasi)
        {
            var userId = _userManager.GetUserId(User);
            seyahat.KullaniciID = userId;

            ModelState.Remove("Kullanici");
            ModelState.Remove("Durum");
            ModelState.Remove("KullaniciID");

            if (ModelState.IsValid)
            {
                // --- RESİM İŞLEMLERİ ---
                if (resimDosyasi != null)
                {
                    // A) Dosya Yükleme
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
                    string uploadPath = Path.Combine(wwwRootPath, @"img\seyahatler");

                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await resimDosyasi.CopyToAsync(fileStream);
                    }

                    seyahat.ResimYolu = @"/img/seyahatler/" + fileName;
                }
                else
                {
                    // B) Kullanıcı Dosya Seçmediyse (GERÇEK STOCK FOTOĞRAF):

                    //  Arama kelimesini belirle
                    // Eğer şehir varsa şehri, yoksa ülkeyi, o da yoksa "holiday" ara.
                    string yer = !string.IsNullOrEmpty(seyahat.Sehir) ? seyahat.Sehir : (seyahat.Ulke ?? "holiday");

                    //  Türkçe Karakter Temizliği (Linkin bozulmaması için şart)
                    string arama = yer.ToLower()
                        .Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i")
                        .Replace("ö", "o").Replace("ş", "s").Replace("ü", "u")
                        .Replace(" ", "-");

                    // 3. LOREMFLICKR KULLAN (Gerçek Fotoğraflar)
                    // "city,view,landmark" etiketlerini ekledik manzara gelmesi için.
                    // "/all" parametresi her yenilemede farklı resim gelmesini sağlar.
                    seyahat.ResimYolu = $"https://loremflickr.com/800/600/{arama},city,view/all";
                }
                _context.Add(seyahat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Seyahat"), "DurumID", "Ad", seyahat.DurumID);
            return View(seyahat);
        }

        //Seyahatler/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var userId = _userManager.GetUserId(User);
            var seyahat = await _context.Seyahatler.FirstOrDefaultAsync(s => s.SeyahatID == id && s.KullaniciID == userId);

            if (seyahat == null) return NotFound();
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Seyahat"), "DurumID", "Ad", seyahat.DurumID);
            return View(seyahat);
        }

        //Seyahatler/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SeyahatID,Baslik,Sehir,Ulke,Tur,GeziNotu,ZiyaretTarihi,Enlem,Boylam,ResimYolu,DurumID,KullaniciID")] Seyahat seyahat)
        {
            if (id != seyahat.SeyahatID) return NotFound();

            ModelState.Remove("Kullanici");
            ModelState.Remove("Durum");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seyahat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeyahatExists(seyahat.SeyahatID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Seyahat"), "DurumID", "Ad", seyahat.DurumID);
            return View(seyahat);
        }

        //Seyahatler/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var userId = _userManager.GetUserId(User);
            var seyahat = await _context.Seyahatler
                .Include(s => s.Durum)
                .FirstOrDefaultAsync(m => m.SeyahatID == id && m.KullaniciID == userId);

            if (seyahat == null) return NotFound();
            return View(seyahat);
        }

        // Seyahatler/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seyahat = await _context.Seyahatler.FindAsync(id);
            if (seyahat != null) _context.Seyahatler.Remove(seyahat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeyahatExists(int id)
        {
            return _context.Seyahatler.Any(e => e.SeyahatID == id);
        }
    }
}