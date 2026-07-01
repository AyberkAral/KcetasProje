using Microsoft.AspNetCore.Mvc;

namespace KcetasWeb.Controllers
{
    public class TuketimNoktasiController : Controller
    {
        // 1. Tüketim Noktaları Listesi (Index Sayfası)
        public IActionResult Index()
        {
            return View();
        }

        // 2. Yeni Tüketim Noktası Ekleme Formu
        public IActionResult Yeni()
        {
            return View();
        }
    }
}