using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;

namespace KcetasWeb.Controllers
{
    public class SayacController : Controller
    {
        private static List<Sayac> _sayaclar = new List<Sayac>
        {
            new Sayac { SayacId = 1, SeriNo = "S-1001", Marka = "Makel", Model = "M-200", Faz = "Monofaze", Carpan = 1.0m, AktifEndeks = 1500, Durum = "Depoda", Status = "AKTIF" },
            new Sayac { SayacId = 2, SeriNo = "S-1002", Marka = "Luna", Model = "L-300", Faz = "Trifaze", Carpan = 10.0m, AktifEndeks = 25000, Durum = "Depoda", Status = "AKTIF" },
            new Sayac { SayacId = 3, SeriNo = "S-1003", Marka = "Viko", Model = "V-100", Faz = "Monofaze", Carpan = 1.0m, AktifEndeks = 500, Durum = "Bağlı", TuketimNoktasiId = 1, Status = "AKTIF" }
        };

        public IActionResult Index()
        {
            ViewBag.TuketimNoktalari = TuketimNoktasiController._tuketimNoktalari;
            return View(_sayaclar);
        }

        public IActionResult Bagla(long id)
        {
            var sayac = _sayaclar.FirstOrDefault(s => s.SayacId == id);
            if (sayac == null) return NotFound();

            ViewBag.TuketimNoktalari = TuketimNoktasiController._tuketimNoktalari;
            return View(sayac);
        }

        [HttpPost]
        public IActionResult Bagla(long SayacId, long TuketimNoktasiId)
        {
            var sayac = _sayaclar.FirstOrDefault(s => s.SayacId == SayacId);
            if (sayac != null)
            {
                sayac.TuketimNoktasiId = TuketimNoktasiId;
                sayac.Durum = TuketimNoktasiId > 0 ? "Bağlı" : "Depoda";
                TempData["BasariMesaji"] = $"Sayaç başarıyla {(TuketimNoktasiId > 0 ? "bağlandı" : "boşa alındı")}.";
            }
            return RedirectToAction("Index");
        }
    }
}
