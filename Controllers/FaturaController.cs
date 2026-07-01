using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;

namespace KcetasWeb.Controllers
{
    public class FaturaController : Controller
    {
        public IActionResult Index()
        {
            // Veritabanı (PostgreSQL) bağlanana kadar ekranda göreceğimiz geçici faturalar
            var faturalar = new List<Fatura>
            {
                new Fatura {
                    FaturaId = 1, FaturaNo = "FAT-2026-001", AboneTipi = "Mesken", 
                    Donem = "2026-06", SonOdemeTarihi = new DateTime(2026, 7, 15), 
                    TuketimKwh = 245.50m, ToplamTutar = 450.75m, Durum = "Ödendi"
                },
                new Fatura {
                    FaturaId = 2, FaturaNo = "FAT-2026-002", AboneTipi = "Ticarethane", 
                    Donem = "2026-06", SonOdemeTarihi = new DateTime(2026, 7, 15), 
                    TuketimKwh = 1250.00m, ToplamTutar = 3250.00m, Durum = "Bekliyor"
                }
            };

            return View(faturalar);
        }
    }
}