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
    public class DizilerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DizilerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET Diziler
        public async Task<IActionResult> Index(string durum) 
        {
            var userId = _userManager.GetUserId(User);

            // Sadece benim dizilerim gelsin
            var dizilerQuery = _context.Diziler
                .Include(d => d.Durum)
                .Where(d => d.KullaniciID == userId);

            // Filtreleme 
            if (!string.IsNullOrEmpty(durum))
            {
                // Layout'tan gelen İngilizce karakterli değerleri 
                // veritabanındaki Türkçe karşılıklarına çeviriyoruz
                if (durum == "Izledim") durum = "İzledim";
                if (durum == "Izliyorum") durum = "İzliyorum";
                if (durum == "Izleyecegim") durum = "İzleyeceğim";

                dizilerQuery = dizilerQuery.Where(d => d.Durum.Ad == durum);
            }

            return View(await dizilerQuery.ToListAsync());
        }

        // GET: Diziler/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var dizi = await _context.Diziler
                .Include(d => d.Durum)
                .Include(d => d.Kullanici)
                .FirstOrDefaultAsync(m => m.DiziID == id);

            if (dizi == null) return NotFound();

            return View(dizi);
        }

        //Diziler/Create
        public IActionResult Create()
        {
            // Sadece DİZİ durumlarını getir (Okudum/Gittim gelmesin)
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Dizi"), "DurumID", "Ad");
            return View();
        }

        // POST: Diziler/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiziID,Baslik,Yonetmen,Aciklama,BaslangicYili,SezonSayisi,BolumSayisi,PosterYolu,KullaniciPuani,DurumID,Tur")] Dizi dizi)
        {
            var userId = _userManager.GetUserId(User);
            dizi.KullaniciID = userId;
            dizi.KayitTarihi = DateTime.Now;

            ModelState.Remove("Kullanici");
            ModelState.Remove("Durum");
            ModelState.Remove("KullaniciID");

            if (ModelState.IsValid)
            {
                _context.Add(dizi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Dizi"), "DurumID", "Ad", dizi.DurumID);
            return View(dizi);
        }

        // POST: Diziler/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiziID,Baslik,Yonetmen,Aciklama,BaslangicYili,SezonSayisi,BolumSayisi,PosterYolu,KullaniciPuani,DurumID,KayitTarihi,KullaniciID,Tur")] Dizi dizi)
        {
            if (id != dizi.DiziID) return NotFound();

            ModelState.Remove("Kullanici");
            ModelState.Remove("Durum");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dizi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiziExists(dizi.DiziID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Dizi"), "DurumID", "Ad", dizi.DurumID);
            return View(dizi);
        }
        // GET: Diziler/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var userId = _userManager.GetUserId(User);
            var dizi = await _context.Diziler
                .Include(d => d.Durum)
                .FirstOrDefaultAsync(m => m.DiziID == id && m.KullaniciID == userId);
            if (dizi == null) return NotFound();
            return View(dizi);
        }

        // POST: Diziler/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dizi = await _context.Diziler.FindAsync(id);
            if (dizi != null) _context.Diziler.Remove(dizi);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiziExists(int id)
        {
            return _context.Diziler.Any(e => e.DiziID == id);
        }
    }
}
