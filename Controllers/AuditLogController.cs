using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.Models;
using System.Linq;

namespace KcetasWeb.Controllers
{
    // Yalnızca BT Yöneticisi ve Denetçi rollerinin görmesini sağlıyoruz.
    [Authorize(Roles = "BTYoneticisi,Denetci")]
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;
        private readonly IKullaniciDeposu _kullaniciDeposu;

        public AuditLogController(IAuditLogService auditLogService, IKullaniciDeposu kullaniciDeposu)
        {
            _auditLogService = auditLogService;
            _kullaniciDeposu = kullaniciDeposu;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 100; // Sayfa başına 100 veri ile kasmadan çekiyoruz
            var loglar = _auditLogService.GetAll(page, pageSize);
            
            ViewBag.CurrentPage = page;

            // Tüm kullanıcıları bir kez çekip View'a sözlük olarak gönderiyoruz
            // Böylece View içinde kullanici_id'yi isme çevirebiliriz.
            var kullanicilar = _kullaniciDeposu.Listele();
            ViewBag.KullaniciDict = kullanicilar.ToDictionary(k => k.kullanici_id, k => k.ad_soyad ?? k.kullanici_adi);

            return View(loglar);
        }
    }
}
