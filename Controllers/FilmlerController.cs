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
    [Authorize] // Sadece giriş yapanlar görebilir
    public class FilmlerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public FilmlerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        // GET: Filmler
        public async Task<IActionResult> Index(string durum) 
        {
            var userId = _userManager.GetUserId(User);

            // Temel sorgu
            var sorgu = _context.Filmler
                .Include(f => f.Durum)
                .Where(f => f.KullaniciID == userId)
                .AsQueryable();

            // Filtreleme 
            if (!string.IsNullOrEmpty(durum))
            {
                // Layout'tan "Izledim" veya "İzledim" gelse de yakalaması için
                if (durum.Equals("Izledim", StringComparison.OrdinalIgnoreCase) || durum == "İzledim")
                    durum = "İzledim";

                if (durum.Equals("Izleyecegim", StringComparison.OrdinalIgnoreCase) || durum == "İzleyeceğim")
                    durum = "İzleyeceğim";

                sorgu = sorgu.Where(f => f.Durum.Ad == durum);
            }

            return View(await sorgu.ToListAsync());
        }

        // GET: Filmler/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var film = await _context.Filmler
                .Include(f => f.Durum)
                .Include(f => f.Kullanici)
                .FirstOrDefaultAsync(m => m.FilmID == id);

            if (film == null) return NotFound();

            return View(film);
        }



        // GET: Filmler/Create
        public IActionResult Create()
        {
            // appsettings.json dosyasındaki "TMDb:ApiKey" değerini okuyup View'a gönderiyoruz
            ViewBag.TmdbApiKey = _configuration["TMDb:ApiKey"];

            // Sadece 'Film' kategorisindeki durumları getir (İzledim/İzleyeceğim)
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Film"), "DurumID", "Ad");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Baslik,Yonetmen,Yil,Tur,Aciklama,PosterYolu,KullaniciPuani,DurumID")] Film film)
        {
            
            var userId = _userManager.GetUserId(User);
            film.KullaniciID = userId;
            film.KayitTarihi = DateTime.Now;

            // Kullanici ve Durum navigasyon özelliklerini doğrulamadan muaf tutuyoruz (Hata vermemesi için)
            ModelState.Remove("Kullanici");
            ModelState.Remove("Durum");
            ModelState.Remove("KullaniciID");

            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Eğer hata varsa formu tekrar dolduruyoruz
            ViewBag.TmdbApiKey = _configuration["TMDb:ApiKey"];
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Film"), "DurumID", "Ad", film.DurumID);
            return View(film);
        }
        // GET: Filmler/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var film = await _context.Filmler.FirstOrDefaultAsync(f => f.FilmID == id && f.KullaniciID == userId);

            if (film == null) return NotFound();

            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Film"), "DurumID", "Ad", film.DurumID);
            return View(film);
        }

        // POST: Filmler/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Edit(int id, [Bind("FilmID,Baslik,Yonetmen,Aciklama,Yil,PosterYolu,KullaniciPuani,DurumID,KayitTarihi,KullaniciID,Tur")] Film film)
        {
            if (id != film.FilmID) return NotFound();

            ModelState.Remove("Kullanici");
            ModelState.Remove("Durum");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.FilmID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DurumID"] = new SelectList(_context.Durumlar.Where(d => d.Kategori == "Film"), "DurumID", "Ad", film.DurumID);
            return View(film);
        }
        // GET: Filmler/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var film = await _context.Filmler
                .Include(f => f.Durum)
                .FirstOrDefaultAsync(m => m.FilmID == id && m.KullaniciID == userId);

            if (film == null) return NotFound();

            return View(film);
        }

        // POST: Filmler/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Filmler.FindAsync(id);
            if (film != null)
            {
                _context.Filmler.Remove(film);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(int id)
        {
            return _context.Filmler.Any(e => e.FilmID == id);
        }
    }
}