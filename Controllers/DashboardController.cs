using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.Models.entities;
using System.Linq;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "Yonetici")]
    public class DashboardController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;
        private readonly IOutboxService _outboxService;

        public DashboardController(IKullaniciDeposu kullaniciDeposu, IOutboxService outboxService)
        {
            _kullaniciDeposu = kullaniciDeposu;
            _outboxService = outboxService;
        }

        public IActionResult Index()
        {
            var outboxStats = _outboxService.GetIstatistikler();
            var kullaniciListe = _kullaniciDeposu.Listele();

            ViewBag.ToplamKullanici = kullaniciListe.Count;
            ViewBag.AktifKullanici = kullaniciListe.Count(k => k.Durum == "AKTIF");
            
            ViewBag.OutboxBekleyen = outboxStats.Bekleyen;
            ViewBag.OutboxHatali = outboxStats.Basarisiz;
            ViewBag.OutboxToplam = outboxStats.Toplam;
            
            // Mock data access for TuketimNoktasi
            ViewBag.AktifTuketimNoktasi = TuketimNoktasiController._tuketimNoktalari.Count;
            
            // Recent users
            ViewBag.SonKullanicilar = kullaniciListe
                                        .OrderByDescending(k => k.CreatedAt)
                                        .Take(5).ToList();

            return View();
        }
    }
}
