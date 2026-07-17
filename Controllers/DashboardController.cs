using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.Models.entities;
using System.Linq;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi, SayacOkumaPersoneli,SahaOperasyonAmir,FaturalamaUzmani,Denetci ")]
    public class DashboardController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;

        public DashboardController(IKullaniciDeposu kullaniciDeposu, ITuketimNoktasiService tuketimNoktasiService)
        {
            _kullaniciDeposu = kullaniciDeposu;
            _tuketimNoktasiService = tuketimNoktasiService;
        }

        public IActionResult Index()
        {
            var kullaniciListe = _kullaniciDeposu.Listele();

            ViewBag.ToplamKullanici = kullaniciListe.Count;
            ViewBag.AktifKullanici = kullaniciListe.Count(k => k.durum == "AKTIF");
            
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
