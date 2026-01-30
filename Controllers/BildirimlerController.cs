using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KulturAtlasi.Data;
using KulturAtlasi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace KulturAtlasi.Controllers
{
    [Authorize] // Sadece giriş yapanlar
    public class BildirimlerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BildirimlerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Bildirimler
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            // eskiden yeniye göre bildirimleri çek
            var bildirimler = await _context.Bildirimler
                .Where(b => b.KullaniciID == userId)
                .OrderByDescending(b => b.Tarih)
                .ToListAsync();
            var okunmamislar = bildirimler.Where(b => b.GorulduMu == false).ToList();
            if (okunmamislar.Any())
            {
                foreach (var bildirim in okunmamislar)
                {
                    bildirim.GorulduMu = true;
                }
                await _context.SaveChangesAsync();
            }

            return View(bildirimler);
        }

        //Tümünü Sil
        [HttpPost]
        public async Task<IActionResult> TumunuSil()
        {
            var userId = _userManager.GetUserId(User);
            var bildirimler = _context.Bildirimler.Where(b => b.KullaniciID == userId);

            _context.Bildirimler.RemoveRange(bildirimler);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Tek Sil
        [HttpPost]
        public async Task<IActionResult> Sil(int id)
        {
            var bildirim = await _context.Bildirimler.FindAsync(id);
            if (bildirim != null)
            {
                _context.Bildirimler.Remove(bildirim);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
