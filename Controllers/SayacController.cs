using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Controllers
{
    public class SayacController : Controller
    {
        private static List<Sayac> _sayaclar = new List<Sayac>
        {
            new Sayac { SayacId = 1, SayacNo = "S-1001", MarkaId = 1, ModelId = 1, Tip = "Monofaze", ImalYili = 2020, Durum = "Depoda", CreatedAt = DateTime.Now },
            new Sayac { SayacId = 2, SayacNo = "S-1002", MarkaId = 2, ModelId = 2, Tip = "Trifaze", ImalYili = 2021, Durum = "Depoda", CreatedAt = DateTime.Now },
            new Sayac { SayacId = 3, SayacNo = "S-1003", MarkaId = 1, ModelId = 3, Tip = "Monofaze", ImalYili = 2019, Durum = "Bağlı", TuketimNoktasiId = 1, CreatedAt = DateTime.Now }
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
