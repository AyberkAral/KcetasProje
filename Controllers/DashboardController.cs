using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.Models.entities;
using System.Linq;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = AppRoles.BTYoneticisi + ",Yonetici")]
    public class DashboardController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;
        private readonly IOutboxService _outboxService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;

        public DashboardController(IKullaniciDeposu kullaniciDeposu, IOutboxService outboxService, ITuketimNoktasiService tuketimNoktasiService)
        {
            _kullaniciDeposu = kullaniciDeposu;
            _outboxService = outboxService;
            _tuketimNoktasiService = tuketimNoktasiService;
        }

        public IActionResult Index()
        {
            var outboxStats = _outboxService.GetIstatistikler();
            var kullaniciListe = _kullaniciDeposu.Listele();

            ViewBag.ToplamKullanici = kullaniciListe.Count;
            ViewBag.AktifKullanici = kullaniciListe.Count(k => k.durum == "AKTIF");
            
            ViewBag.OutboxBekleyen = outboxStats.Bekleyen;
            ViewBag.OutboxHatali = outboxStats.Basarisiz;
            ViewBag.OutboxToplam = outboxStats.Toplam;
            
            // Veritabanından TuketimNoktasi sayısı
            ViewBag.AktifTuketimNoktasi = _tuketimNoktasiService.GetAll().Count;
            
            // Recent users
            ViewBag.SonKullanicilar = kullaniciListe
                                        .OrderByDescending(k => k.created_at)
                                        .Take(5).ToList();

            return View();
        }
    }
}
