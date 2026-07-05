using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "Yonetici")]
    public class AboneSorgulamaController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;

        // Statik örnek veriler (demo amaçlı) - dilerseniz ileride tamamen kaldırılabilir
        private static List<Abone> _aboneler = new List<Abone>
        {
            new Abone { AboneId = 1, AboneNo = "ABN-10045", AdSoyadUnvan = "Ahmet Yılmaz", AboneTipi = "Bireysel", TcKimlikVergiNo = "12345678901", Telefon = "05321234567", EPosta = "ahmet@ornek.com", Status = "Aktif", CreatedAt = DateTime.Now.AddDays(-100) },
            new Abone { AboneId = 2, AboneNo = "ABN-10046", AdSoyadUnvan = "Örnek Ltd. Şti.", AboneTipi = "Kurumsal", TcKimlikVergiNo = "9876543210", Telefon = "02121234567", EPosta = "iletisim@ornek.com.tr", Status = "Aktif", CreatedAt = DateTime.Now.AddDays(-200) },
            new Abone { AboneId = 3, AboneNo = "ABN-10047", AdSoyadUnvan = "Ayşe Demir", AboneTipi = "Bireysel", TcKimlikVergiNo = "11122233344", Telefon = "05339998877", EPosta = "ayse@ornek.com", Status = "Pasif", CreatedAt = DateTime.Now.AddDays(-50) }
        };

        public AboneSorgulamaController(IKullaniciDeposu kullaniciDeposu)
        {
            _kullaniciDeposu = kullaniciDeposu;
        }

        // Statik demo verisi + sisteme kayıt olmuş "Abone" rolündeki kullanıcıları birleştirir
        private List<Abone> TumAboneleriGetir()
        {
            var liste = new List<Abone>(_aboneler);

            var kayitliAboneler = _kullaniciDeposu.Listele()
                .Where(k => k.Rol?.RolAdi == "Abone");

            foreach (var kullanici in kayitliAboneler)
            {
                liste.Add(new Abone
                {
                    // Statik listedeki ID'lerle çakışmasın diye ofsetli ID kullanıyoruz
                    AboneId = 100000 + kullanici.KullaniciId,
                    AboneNo = "ABN-" + (20000 + kullanici.KullaniciId),
                    AdSoyadUnvan = kullanici.AdSoyad,
                    AboneTipi = kullanici.AboneTuru == "Mesken" ? "Mesken" : "İş Yeri",
                    TcKimlikVergiNo = "-",
                    Telefon = "-",
                    EPosta = kullanici.EPosta,
                    Status = kullanici.Durum == "AKTIF" ? "Aktif" : "Pasif",
                    CreatedAt = kullanici.CreatedAt,
                    UpdatedAt = kullanici.UpdatedAt
                });
            }

            return liste;
        }

        public IActionResult Index(string q)
        {
            ViewBag.Query = q;
            if (string.IsNullOrEmpty(q))
            {
                return View(null);
            }

            var tumAboneler = TumAboneleriGetir();

            var results = tumAboneler.Where(a =>
                (a.AboneNo != null && a.AboneNo.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (a.AdSoyadUnvan != null && a.AdSoyadUnvan.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (a.TcKimlikVergiNo != null && a.TcKimlikVergiNo.Contains(q, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            return View(results);
        }

        public IActionResult Detay(long id)
        {
            var tumAboneler = TumAboneleriGetir();
            var abone = tumAboneler.FirstOrDefault(x => x.AboneId == id);
            if (abone == null)
            {
                return NotFound();
            }
            return View(abone);
        }
    }
}