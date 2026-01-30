using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;
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

namespace KulturAtlasi.Controllers
{
    [Authorize]
    public class KitaplarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment; // Dosya işlemleri için gerekli

        public KitaplarController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        //  Kitaplar
        public async Task<IActionResult> Index(string durum)
        {
            var userId = _userManager.GetUserId(User);
            var kitaplar = _context.Kitaplar
                .Include(k => k.Durum)
                .Where(k => k.KullaniciID == userId);

            if (!string.IsNullOrEmpty(durum))
            {
                if (durum == "Okudum") durum = "Okudum";
                if (durum == "Okuyacagim") durum = "Okuyacağım";
                if (durum == "Yarida") durum = "Yarıda Bıraktım";
                kitaplar = kitaplar.Where(k => k.Durum.Ad == durum);
            }

            return View(await kitaplar.ToListAsync());
        }

        // Kitaplar/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var kitap = await _context.Kitaplar
                .Include(k => k.Durum)
                .FirstOrDefaultAsync(m => m.KitapID == id);

            if (kitap == null) return NotFound();
            return View(kitap);
        }

        //  Kitaplar/Create
        public IActionResult Create()
        {
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Kitap"), "DurumID", "Ad");
            return View();
        }

        // Kitaplar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KitapID,Baslik,Yazar,ISBN,Aciklama,KapakResmiYolu,KullaniciPuani,DurumID,Tur")] Kitap kitap, IFormFile? resimDosyasi)
        {
            var userId = _userManager.GetUserId(User);
            kitap.KullaniciID = userId;
            kitap.KayitTarihi = DateTime.Now;

            ModelState.Remove("Kullanici");
            ModelState.Remove("Durum");
            ModelState.Remove("KullaniciID");

            if (ModelState.IsValid)
            {
                // Resim yükleme
                if (resimDosyasi != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
                    string uploadPath = Path.Combine(wwwRootPath, @"img\kitaplar");

                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await resimDosyasi.CopyToAsync(fileStream);
                    }
                    kitap.KapakResmiYolu = @"/img/kitaplar/" + fileName;
                }
                // Eğer dosya yoksa, Google'dan gelen linki kullanmaya devam eder.

                _context.Add(kitap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Kitap"), "DurumID", "Ad", kitap.DurumID);
            return View(kitap);
        }

        // Kitaplar/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var userId = _userManager.GetUserId(User);

            var kitap = await _context.Kitaplar.FirstOrDefaultAsync(k => k.KitapID == id && k.KullaniciID == userId);
            if (kitap == null) return NotFound();

            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Kitap"), "DurumID", "Ad", kitap.DurumID);
            return View(kitap);
        }

        //Kitaplar/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KitapID,Baslik,Yazar,ISBN,Aciklama,KapakResmiYolu,KullaniciPuani,DurumID,KayitTarihi,KullaniciID,Tur")] Kitap kitap, IFormFile? resimDosyasi)
        {
            if (id != kitap.KitapID) return NotFound();

            ModelState.Remove("Kullanici");
            ModelState.Remove("Durum");

            if (ModelState.IsValid)
            {
                try
                {
                    // resim güncelle
                    if (resimDosyasi != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
                        string uploadPath = Path.Combine(wwwRootPath, @"img\kitaplar");

                        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                        using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                        {
                            await resimDosyasi.CopyToAsync(fileStream);
                        }
                        kitap.KapakResmiYolu = @"/img/kitaplar/" + fileName;
                    }

                    _context.Update(kitap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KitapExists(kitap.KitapID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Kitap"), "DurumID", "Ad", kitap.DurumID);
            return View(kitap);
        }

        //  Kitaplar/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var userId = _userManager.GetUserId(User);
            var kitap = await _context.Kitaplar
                .Include(k => k.Durum)
                .FirstOrDefaultAsync(m => m.KitapID == id && m.KullaniciID == userId);

            if (kitap == null) return NotFound();
            return View(kitap);
        }

        //Kitaplar/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kitap = await _context.Kitaplar.FindAsync(id);
            if (kitap != null) _context.Kitaplar.Remove(kitap);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KitapExists(int id)
        {
            return _context.Kitaplar.Any(e => e.KitapID == id);
        }
    }
}