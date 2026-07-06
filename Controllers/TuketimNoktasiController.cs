using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;

namespace KcetasWeb.Controllers
{
    public class TuketimNoktasiController : Controller
    {
        public static List<TuketimNoktasi> _tuketimNoktalari = new List<TuketimNoktasi>
        {
            new TuketimNoktasi { TuketimNoktasiId = 1, TekilKod = "TK-2026-001", TuketiciGrubu = "Mesken", BaglantiGucuKw = 5.00m, BaglantiDurumu = "Aktif", AcikAdres = "Atatürk Mah." },
            new TuketimNoktasi { TuketimNoktasiId = 2, TekilKod = "TK-2026-002", TuketiciGrubu = "Ticarethane", BaglantiGucuKw = 15.50m, BaglantiDurumu = "Bağlantı Bekliyor", AcikAdres = "Cumhuriyet Cad." }
        };

        // 1. Liste Ekranı (Sayfa ilk açıldığında çalışır)
        public IActionResult Index()
        {
            return View(_tuketimNoktalari);
        }

        // 2. Yeni Kayıt Formu (Sayfa ilk açıldığında GET olarak çalışır)
        public IActionResult Yeni()
        {
            return View();
        }

        // 3. FORMDAN GELEN VERİYİ YAKALAYAN METOT (POST işlemi)
        [HttpPost]
        public IActionResult Yeni(string TuketiciGrubu, string BaglantiGucu, string AcikAdres)
        {
            decimal gucu = 0;
            if(!string.IsNullOrEmpty(BaglantiGucu))
            {
                decimal.TryParse(BaglantiGucu.Replace('.', ','), out gucu);
            }

            int count = _tuketimNoktalari.Count + 1;

            _tuketimNoktalari.Add(new TuketimNoktasi
            {
                TuketimNoktasiId = count,
                TekilKod = $"TK-2026-{(count).ToString().PadLeft(3, '0')}",
                TuketiciGrubu = TuketiciGrubu,
                BaglantiGucuKw = gucu,
                AcikAdres = AcikAdres,
                BaglantiDurumu = "Bağlantı Bekliyor",
                Status = "AKTIF",
                CreatedAt = DateTime.Now
            });

            // Şimdilik işlemi başarılı sayıp, ekrana JavaScript KULLANMADAN
            // mesaj basmak için TempData kullanıyoruz:
            TempData["BasariMesaji"] = "Harika! " + TuketiciGrubu + " grubundaki yeni kayıt başarıyla oluşturuldu.";

            // Kayıt bitince kullanıcıyı tekrar listeye (Index'e) geri yolla:
            return RedirectToAction("Index");
        }

        public IActionResult Detay(string id)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.TekilKod == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        public IActionResult Duzenle(string id)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.TekilKod == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Duzenle(string TekilKod, string TuketiciGrubu, string BaglantiGucu, string AcikAdres, string BaglantiDurumu)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.TekilKod == TekilKod);
            if (item != null)
            {
                decimal gucu = 0;
                if(!string.IsNullOrEmpty(BaglantiGucu))
                {
                    decimal.TryParse(BaglantiGucu.Replace('.', ','), out gucu);
                }
                item.TuketiciGrubu = TuketiciGrubu;
                item.BaglantiGucuKw = gucu;
                item.AcikAdres = AcikAdres;
                item.BaglantiDurumu = BaglantiDurumu;
            }
            TempData["BasariMesaji"] = TekilKod + " kodlu nokta başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = TekilKod });
        }
    }
}

