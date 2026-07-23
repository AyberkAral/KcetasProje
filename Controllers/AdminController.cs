using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi")]
    public class AdminController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;

        public AdminController(IKullaniciDeposu kullaniciDeposu)
        {
            _kullaniciDeposu = kullaniciDeposu;
        }

        public IActionResult Index(KcetasWeb.ViewModels.AdminListeViewModel filtre)
        {
            var liste = _kullaniciDeposu.Listele().AsQueryable();

            if (!string.IsNullOrEmpty(filtre.FiltreKullaniciAdi))
                liste = liste.Where(x => x.kullanici_adi != null && x.kullanici_adi.Contains(filtre.FiltreKullaniciAdi, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filtre.FiltreAdSoyad))
                liste = liste.Where(x => x.ad_soyad != null && x.ad_soyad.Contains(filtre.FiltreAdSoyad, StringComparison.OrdinalIgnoreCase));

            if (filtre.FiltreRol.HasValue)
                liste = liste.Where(x => x.rol_id == filtre.FiltreRol.Value);

            var orderedList = liste.OrderByDescending(k => k.created_at).ToList();

            int totalItems = orderedList.Count;
            
            filtre.CurrentPage = filtre.CurrentPage > 0 ? filtre.CurrentPage : 1;
            filtre.PageSize = filtre.PageSize > 0 ? filtre.PageSize : 50;
            
            var pagedData = orderedList.Skip((filtre.CurrentPage - 1) * filtre.PageSize).Take(filtre.PageSize).ToList();

            filtre.TotalItems = totalItems;
            filtre.Kullanicilar = pagedData;

            return View(filtre);
        }

        public IActionResult Yeni()
        {
            ViewBag.Roller = RolListesi.Roller;
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(string AdSoyad, string KullaniciAdi, string EPosta, string Sifre, short RolId)
        {
            if (_kullaniciDeposu.KullaniciAdiVarMi(KullaniciAdi))
            {
                TempData["PersonelMesaji"] = "Bu kullanıcı adı zaten kullanılıyor.";
                return RedirectToAction("Yeni");
            }

            var yeniKullanici = new Kullanici
            {
                ad_soyad = AdSoyad,
                kullanici_adi = KullaniciAdi,
                e_posta = EPosta,
                rol_id = RolId,
                durum = KcetasWeb.Models.Enums.KullaniciDurumu.Aktif,
                created_at = DateTime.Now,
                Sifre = Sifre
            };

            var hasher = new PasswordHasher<Kullanici>();
            yeniKullanici.sifre_hash = hasher.HashPassword(yeniKullanici, Sifre);

            _kullaniciDeposu.Ekle(yeniKullanici);

            TempData["PersonelMesaji"] = AdSoyad + " isimli kullanıcı sisteme başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(long id)
        {
            var kullanici = _kullaniciDeposu.BulId(id);
            if (kullanici == null) return NotFound();
            return View(kullanici);
        }

        public IActionResult Duzenle(long id)
        {
            var kullanici = _kullaniciDeposu.BulId(id);
            if (kullanici == null) return NotFound();
            ViewBag.Roller = RolListesi.Roller;
            return View(kullanici);
        }

        [HttpPost]
        public IActionResult Duzenle(Kullanici model)
        {
            var mevcut = _kullaniciDeposu.BulId(model.kullanici_id);
            if (mevcut == null) return NotFound();

            mevcut.ad_soyad = model.ad_soyad;
            mevcut.kullanici_adi = model.kullanici_adi;
            mevcut.e_posta = model.e_posta;
            mevcut.durum = model.durum;
            mevcut.rol_id = model.rol_id;

            _kullaniciDeposu.Guncelle(mevcut);

            TempData["PersonelMesaji"] = mevcut.ad_soyad + " isimli kullanıcı başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.kullanici_id });
        }

        public IActionResult Sil(long id)
        {
            var kullanici = _kullaniciDeposu.BulId(id);
            if (kullanici != null)
            {
                _kullaniciDeposu.Sil(id);
                TempData["PersonelMesaji"] = kullanici.ad_soyad + " isimli kullanıcı başarıyla silindi.";
            }
            return RedirectToAction("Index");
        }
    }
}