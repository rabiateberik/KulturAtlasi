using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KulturAtlasi.Data;
using KulturAtlasi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace KulturAtlasi.Controllers
{
    [Authorize] // Güvenlik Kilidi
    public class MuziklerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MuziklerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Muzikler
        public async Task<IActionResult> Index(string durum)
        {
            var userId = _userManager.GetUserId(User);

            // Sadece bu kullanıcının müzikleri
            var muzikler = _context.Muzikler
                .Include(m => m.Durum)
                .Where(m => m.KullaniciID == userId);

            // Filtreleme
            if (!string.IsNullOrEmpty(durum))
            {
                if (durum == "Dinledim") durum = "Dinledim";
                if (durum == "Dinleyecegim") durum = "Dinleyeceğim"; // Türkçe karakter düzeltmesi

                muzikler = muzikler.Where(m => m.Durum.Ad == durum);
            }

            return View(await muzikler.ToListAsync());
        }

        //Muzikler/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var muzik = await _context.Muzikler
                .Include(m => m.Durum)
                .FirstOrDefaultAsync(m => m.MuzikID == id);

            if (muzik == null) return NotFound();

            return View(muzik);
        }

        //Muzikler/Create
        public IActionResult Create()
        {
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Muzik"), "DurumID", "Ad");
            return View();
        }

        //Muzikler/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MuzikID,Baslik,Sanatci,Tur,Album,CikisYili,KapakResmiYolu,DinlemeLinki,KullaniciPuani,Aciklama,DurumID")] Muzik muzik)
        {
            var userId = _userManager.GetUserId(User);
            muzik.KullaniciID = userId;
            muzik.KayitTarihi = DateTime.Now;

            // Validasyon temizliği
            ModelState.Remove("Kullanici");
            ModelState.Remove("Durum");
            ModelState.Remove("KullaniciID");

            if (ModelState.IsValid)
            {
                _context.Add(muzik);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Muzik"), "DurumID", "Ad", muzik.DurumID);
            return View(muzik);
        }

        // Muzikler/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var userId = _userManager.GetUserId(User);

            var muzik = await _context.Muzikler.FirstOrDefaultAsync(m => m.MuzikID == id && m.KullaniciID == userId);

            if (muzik == null) return NotFound();

            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Muzik"), "DurumID", "Ad", muzik.DurumID);
            return View(muzik);
        }

        //Muzikler/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MuzikID,Baslik,Sanatci,Tur,Album,CikisYili,KapakResmiYolu,DinlemeLinki,KullaniciPuani,Aciklama,DurumID,KayitTarihi,KullaniciID")] Muzik muzik)
        {
            if (id != muzik.MuzikID) return NotFound();

            ModelState.Remove("Kullanici");
            ModelState.Remove("Durum");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(muzik);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MuzikExists(muzik.MuzikID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Muzik"), "DurumID", "Ad", muzik.DurumID);
            return View(muzik);
        }

        // Muzikler/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var userId = _userManager.GetUserId(User);

            var muzik = await _context.Muzikler
                .Include(m => m.Durum)
                .FirstOrDefaultAsync(m => m.MuzikID == id && m.KullaniciID == userId);

            if (muzik == null) return NotFound();

            return View(muzik);
        }

        // Muzikler/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var muzik = await _context.Muzikler.FindAsync(id);
            if (muzik != null) _context.Muzikler.Remove(muzik);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MuzikExists(int id)
        {
            return _context.Muzikler.Any(e => e.MuzikID == id);
        }
    }
}