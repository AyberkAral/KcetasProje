using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using KcetasWeb.Models.enums;

namespace KcetasWeb.Controllers
{
    public class EndeksOkumaController : Controller
    {
        private static List<EndeksOkuma> _okumalar = new List<EndeksOkuma>
        {
            new EndeksOkuma { OkumaId = 1, TuketimNoktasiKodu = "TK-2026-001", SayacSeriNo = "S-123456", OkumaTarihi = DateTime.Now.AddDays(-30), OncekiEndeks = 1050.5m, GuncelEndeks = 1200.75m, TuketimMiktari = 150.25m, OkumaDurumu = OkumaDurumu.Basarili, OkumaKaynagi = OkumaKaynagi.Manuel, OkuyanKullaniciAdi = "Ali Veli", TarifeGrubu = "Mesken", CreatedAt = DateTime.Now.AddDays(-30) },
            new EndeksOkuma { OkumaId = 2, TuketimNoktasiKodu = "TK-2026-002", SayacSeriNo = "S-987654", OkumaTarihi = DateTime.Now.AddDays(-15), OncekiEndeks = 5000.0m, GuncelEndeks = 5400.0m, TuketimMiktari = 400.0m, OkumaDurumu = OkumaDurumu.Basarili, OkumaKaynagi = OkumaKaynagi.Manuel, OkuyanKullaniciAdi = "Ayşe Demir", TarifeGrubu = "Ticarethane", CreatedAt = DateTime.Now.AddDays(-15) },
            new EndeksOkuma { OkumaId = 3, TuketimNoktasiKodu = "TK-2026-003", SayacSeriNo = "S-555555", OkumaTarihi = DateTime.Now.AddDays(-2), OncekiEndeks = 200.0m, GuncelEndeks = 1200.0m, TuketimMiktari = 1000.0m, OkumaDurumu = OkumaDurumu.Anormal, OkumaKaynagi = OkumaKaynagi.Manuel, OkuyanKullaniciAdi = "Mehmet Can", AnomaliAciklamasi = "Yüksek Tüketim", TarifeGrubu = "Mesken", CreatedAt = DateTime.Now.AddDays(-2) }
        };

        public IActionResult Index()
        {
            return View(_okumalar.OrderByDescending(x => x.OkumaTarihi).ToList());
        }

        public IActionResult Detay(long id)
        {
            var okuma = _okumalar.FirstOrDefault(x => x.OkumaId == id);
            if (okuma == null)
            {
                return NotFound();
            }
            return View(okuma);
        }

        public IActionResult Yeni()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(string TuketimNoktasiKodu, string SayacSeriNo, decimal OncekiEndeks, decimal GuncelEndeks, string TarifeGrubu)
        {
            long nextId = _okumalar.Any() ? _okumalar.Max(x => x.OkumaId) + 1 : 1;
            decimal tuketim = GuncelEndeks - OncekiEndeks;
            
            var yeniOkuma = new EndeksOkuma
            {
                OkumaId = nextId,
                TuketimNoktasiKodu = TuketimNoktasiKodu,
                SayacSeriNo = SayacSeriNo,
                OncekiEndeks = OncekiEndeks,
                GuncelEndeks = GuncelEndeks,
                TuketimMiktari = tuketim,
                OkumaTarihi = DateTime.Now,
                CreatedAt = DateTime.Now,
                OkumaDurumu = tuketim > 1500 ? OkumaDurumu.Anormal : OkumaDurumu.Basarili,
                OkumaKaynagi = OkumaKaynagi.Manuel,
                OkuyanKullaniciAdi = User.Identity?.Name ?? "Admin",
                TarifeGrubu = TarifeGrubu,
                AnomaliAciklamasi = tuketim > 1500 ? "Otomatik anomali algılandı (Çok yüksek tüketim)" : null
            };

            _okumalar.Add(yeniOkuma);
            TempData["OkumaMesaji"] = TuketimNoktasiKodu + " noktası için endeks okuma işlemi başarıyla kaydedildi.";
            
            return RedirectToAction("Index");
        }
    }
}
