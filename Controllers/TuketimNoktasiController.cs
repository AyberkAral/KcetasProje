using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Controllers
{
    public class TuketimNoktasiController : Controller
    {
        public static List<TuketimNoktasi> _tuketimNoktalari = new List<TuketimNoktasi>
        {
<<<<<<< HEAD
            new TuketimNoktasi { TuketimNoktasiId = 1001, TuketimNoktasiNo = "TK-2026-001", AboneId = 1, SokakId = 10, BinaNo = "1", DaireNo = "12", TuketiciGrubuId = 1, TarifeTipiId = 1, Durum = "Aktif", CreatedAt = DateTime.Now.AddDays(-10) },
            new TuketimNoktasi { TuketimNoktasiId = 1002, TuketimNoktasiNo = "TK-2026-002", AboneId = 2, SokakId = 20, BinaNo = "2", DaireNo = "4", TuketiciGrubuId = 2, TarifeTipiId = 2, Durum = "Pasif", CreatedAt = DateTime.Now.AddDays(-5) }
=======
            new TuketimNoktasi { TuketimNoktasiId = 1, TekilKod = "TK-2026-001", TuketiciGrubu = "Mesken", BaglantiGucuKw = 5.00m, BaglantiDurumu = "Aktif", AcikAdres = "Atatürk Mah." },
            new TuketimNoktasi { TuketimNoktasiId = 2, TekilKod = "TK-2026-002", TuketiciGrubu = "Ticarethane", BaglantiGucuKw = 15.50m, BaglantiDurumu = "Bağlantı Bekliyor", AcikAdres = "Cumhuriyet Cad." }
>>>>>>> f13b31bf62746ba12e74dddcd4306d480ca64f69
        };

        public IActionResult Index()
        {
            return View(_tuketimNoktalari);
        }

        public IActionResult Yeni()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(long AboneId, int SokakId, string BinaNo, string DaireNo, int TuketiciGrubuId, int TarifeTipiId)
        {
            int count = _tuketimNoktalari.Count + 1;

            _tuketimNoktalari.Add(new TuketimNoktasi
            {
<<<<<<< HEAD
                TuketimNoktasiId = 1000 + count,
                TuketimNoktasiNo = $"TK-2026-{(count).ToString().PadLeft(3, '0')}",
                AboneId = AboneId,
                SokakId = SokakId,
                BinaNo = BinaNo,
                DaireNo = DaireNo,
                TuketiciGrubuId = TuketiciGrubuId,
                TarifeTipiId = TarifeTipiId,
                Durum = "Pasif",
=======
                TuketimNoktasiId = count,
                TekilKod = $"TK-2026-{(count).ToString().PadLeft(3, '0')}",
                TuketiciGrubu = TuketiciGrubu,
                BaglantiGucuKw = gucu,
                AcikAdres = AcikAdres,
                BaglantiDurumu = "Bağlantı Bekliyor",
                Status = "AKTIF",
>>>>>>> f13b31bf62746ba12e74dddcd4306d480ca64f69
                CreatedAt = DateTime.Now
            });

            TempData["BasariMesaji"] = "Harika! Yeni tüketim noktası başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(string id)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.TuketimNoktasiNo == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        public IActionResult Duzenle(string id)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.TuketimNoktasiNo == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Duzenle(string TuketimNoktasiNo, long AboneId, int SokakId, string BinaNo, string DaireNo, int TuketiciGrubuId, int TarifeTipiId, string Durum)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.TuketimNoktasiNo == TuketimNoktasiNo);
            if (item != null)
            {
                item.AboneId = AboneId;
                item.SokakId = SokakId;
                item.BinaNo = BinaNo;
                item.DaireNo = DaireNo;
                item.TuketiciGrubuId = TuketiciGrubuId;
                item.TarifeTipiId = TarifeTipiId;
                item.Durum = Durum;
                item.UpdatedAt = DateTime.Now;
            }
            TempData["BasariMesaji"] = TuketimNoktasiNo + " kodlu nokta başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = TuketimNoktasiNo });
        }
    }
}
