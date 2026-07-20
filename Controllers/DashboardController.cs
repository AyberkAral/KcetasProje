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
        private readonly IAboneService _aboneService;
        private readonly ISozlesmeService _sozlesmeService;
        private readonly IIsEmriService _isEmriService;
        private readonly IFaturaService _faturaService;

        public DashboardController(
            IKullaniciDeposu kullaniciDeposu, 
            ITuketimNoktasiService tuketimNoktasiService,
            IAboneService aboneService,
            ISozlesmeService sozlesmeService,
            IIsEmriService isEmriService,
            IFaturaService faturaService)
        {
            _kullaniciDeposu = kullaniciDeposu;
            _tuketimNoktasiService = tuketimNoktasiService;
            _aboneService = aboneService;
            _sozlesmeService = sozlesmeService;
            _isEmriService = isEmriService;
            _faturaService = faturaService;
        }

        public IActionResult Index()
        {
            var kullaniciListe = _kullaniciDeposu.Listele();

            ViewBag.ToplamKullanici = kullaniciListe.Count;
            ViewBag.AktifKullanici = kullaniciListe.Count(k => k.durum == "AKTIF");
            
            // Veritabanından TuketimNoktasi sayısı
            ViewBag.AktifTuketimNoktasi = _tuketimNoktasiService.GetAll().Count;

            var aboneCount = 0;
            var sozlesmeCount = 0;
            var aktifIsEmriCount = 0;
            var bekleyenFaturaCount = 0;

            try { aboneCount = _aboneService.GetAll().Count(); } catch { }
            try { sozlesmeCount = _sozlesmeService.GetAll().Count(s => s.durum == "Aktif"); } catch { }
            try { 
                var isEmirleri = _isEmriService.GetAll();
                aktifIsEmriCount = isEmirleri.Count(i => i.durum != KcetasWeb.Constants.IsEmriDurumlari.Tamamlandi && i.durum != "Tamamlandi" && i.durum != "İptal Edildi" && i.durum != "IptalEdildi" && i.durum != "Iptal" && i.durum != KcetasWeb.Constants.IsEmriDurumlari.TamamlandiKucuk && i.durum != KcetasWeb.Constants.IsEmriDurumlari.Iptal);
            } catch { }
            try { bekleyenFaturaCount = _faturaService.GetAll().Count(f => f.durum == "TASLAK" || f.durum == "HESAPLANDI" || f.durum == "HATALI" || f.durum == "ODENMEDI"); } catch { }

            ViewBag.AboneCount = aboneCount;
            ViewBag.SozlesmeCount = sozlesmeCount;
            ViewBag.AktifIsEmriCount = aktifIsEmriCount;
            ViewBag.BekleyenFaturaCount = bekleyenFaturaCount;
            
            // Recent users
            ViewBag.SonKullanicilar = kullaniciListe
                                        .OrderByDescending(k => k.created_at)
                                        .Take(5).ToList();

            return View();
        }
    }
}
