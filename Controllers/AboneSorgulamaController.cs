using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;

namespace KcetasWeb.Controllers
{
    public class AboneSorgulamaController : Controller
    {
        private static List<Abone> _aboneler = new List<Abone>
        {
            new Abone { AboneId = 1, AboneNo = "ABN-10045", AdSoyadUnvan = "Ahmet Yılmaz", AboneTipi = "Bireysel", TcKimlikVergiNo = "12345678901", Telefon = "05321234567", EPosta = "ahmet@ornek.com", Status = "Aktif", CreatedAt = DateTime.Now.AddDays(-100) },
            new Abone { AboneId = 2, AboneNo = "ABN-10046", AdSoyadUnvan = "Örnek Ltd. Şti.", AboneTipi = "Kurumsal", TcKimlikVergiNo = "9876543210", Telefon = "02121234567", EPosta = "iletisim@ornek.com.tr", Status = "Aktif", CreatedAt = DateTime.Now.AddDays(-200) },
            new Abone { AboneId = 3, AboneNo = "ABN-10047", AdSoyadUnvan = "Ayşe Demir", AboneTipi = "Bireysel", TcKimlikVergiNo = "11122233344", Telefon = "05339998877", EPosta = "ayse@ornek.com", Status = "Pasif", CreatedAt = DateTime.Now.AddDays(-50) }
        };

        public IActionResult Index(string q)
        {
            ViewBag.Query = q;
            if (string.IsNullOrEmpty(q))
            {
                return View(null);
            }

            var results = _aboneler.Where(a => 
                (a.AboneNo != null && a.AboneNo.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (a.AdSoyadUnvan != null && a.AdSoyadUnvan.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (a.TcKimlikVergiNo != null && a.TcKimlikVergiNo.Contains(q, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            return View(results);
        }

        public IActionResult Detay(long id)
        {
            var abone = _aboneler.FirstOrDefault(x => x.AboneId == id);
            if (abone == null)
            {
                return NotFound();
            }
            return View(abone);
        }
    }
}
