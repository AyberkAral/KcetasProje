using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using KcetasWeb.ViewModels;
using System;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi,Denetci")]
    public class AboneController : Controller
    {
        private readonly IAboneService _aboneService;
        private readonly IAuditLogService _auditLogService;

        public AboneController(IAboneService aboneService, IAuditLogService auditLogService)
        {
            _aboneService = aboneService;
            _auditLogService = auditLogService;
        }

        public IActionResult Index()
        {
            var aboneler = _aboneService.GetAll();
            
            var viewModel = new AboneListeViewModel
            {
                Aboneler = aboneler.Select(a => new AboneSatirViewModel
                {
                    AboneId = a.abone_id,
                    AboneNo = a.abone_no,
                    AdSoyad = (a.abone_tipi == "Gerçek" || a.abone_tipi == "BIREYSEL") ? $"{a.Ad} {a.Soyad}" : a.Unvan ?? "",
                    KimlikNoMaskeli = (a.abone_tipi == "Gerçek" || a.abone_tipi == "BIREYSEL") ? Maskele(a.tckn) : Maskele(a.vkn),
                    Telefon = a.telefon,
                    Mail = a.e_posta,
                    AboneTipi = a.abone_tipi,
                    Durum = a.Durum
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Yeni()
        {
            return View(new AboneEkleViewModel());
        }

        [HttpPost]
        public IActionResult Yeni(AboneEkleViewModel model)
        {
            var tumAboneler = _aboneService.GetAll();

            if (!string.IsNullOrEmpty(model.TCKN) && tumAboneler.Any(a => a.tckn == model.TCKN))
                ModelState.AddModelError("TCKN", "HATA: Bu TC Kimlik Numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.VKN) && tumAboneler.Any(a => a.vkn == model.VKN))
                ModelState.AddModelError("VKN", "HATA: Bu Vergi Kimlik Numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.Telefon) && tumAboneler.Any(a => a.telefon == model.Telefon))
                ModelState.AddModelError("Telefon", "HATA: Bu telefon numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.Mail) && tumAboneler.Any(a => a.e_posta == model.Mail))
                ModelState.AddModelError("Mail", "HATA: Bu e-posta adresi sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            
            // Abone isminin aynısı var mı kontrolü (Küçük-büyük harf duyarsız)
            var temizAdSoyad = (model.AdSoyadUnvan ?? "").Trim().ToLower();
            if (tumAboneler.Any(a => (a.Ad + " " + a.Soyad).Trim().ToLower() == temizAdSoyad || (a.Unvan ?? "").Trim().ToLower() == temizAdSoyad))
            {
                ModelState.AddModelError("AdSoyadUnvan", "HATA: Bu Abone Adı/Ünvanı sistemde zaten kayıtlı! Lütfen farklı bir isim girin veya mevcut abone kaydını kullanın.");
            }

            if (ModelState.IsValid)
            {
                var yeniAbone = new Abone
                {
                    abone_tipi = model.IsTuzel ? "KURUMSAL" : "BIREYSEL",
                    telefon = model.Telefon,
                    e_posta = model.Mail,
                    Durum = "Aktif",
                    CreatedAt = DateTime.Now
                };

                if (model.IsTuzel)
                {
                    yeniAbone.Unvan = model.AdSoyadUnvan;
                    yeniAbone.vkn = model.VKN ?? "";
                    yeniAbone.Ad = "";
                    yeniAbone.Soyad = "";
                    yeniAbone.tckn = "";
                }
                else
                {
                    var nameParts = model.AdSoyadUnvan.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    yeniAbone.Soyad = nameParts.Length > 1 ? nameParts.Last() : "";
                    yeniAbone.Ad = nameParts.Length > 1 ? string.Join(" ", nameParts.Take(nameParts.Length - 1)) : model.AdSoyadUnvan;
                    yeniAbone.tckn = model.TCKN ?? "";
                    yeniAbone.vkn = "";
                    yeniAbone.Unvan = "";
                }

                _aboneService.Create(yeniAbone);

                // Son eklenen abonenin ID'sini tahmin etmeye çalışıyoruz (Mock serviste ID atanıyor)
                int yeniId = _aboneService.GetAll().Max(a => a.abone_id);
                
                _auditLogService.Ekle(
                    varlikTipi: "Abone",
                    varlikId: yeniId,
                    islemTipi: "INSERT",
                    eskiDeger: "Yok",
                    yeniDeger: model.AdSoyadUnvan,
                    kullaniciId: 1, // Mock User ID
                    islemGerekcesi: "Yeni abone kaydı oluşturuldu."
                );

                TempData["Mesaj"] = "Müşteri / Abone başarıyla eklendi!";
                TempData["MesajTip"] = "success";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Detay(int id)
        {
            var abone = _aboneService.GetById(id);
            if (abone == null) return NotFound();

            var viewModel = new AboneDetayViewModel
            {
                Abone = abone,
                KimlikNoMaskeli = (abone.abone_tipi == "Gerçek" || abone.abone_tipi == "BIREYSEL") ? Maskele(abone.tckn) : Maskele(abone.vkn)
                // Diğer sekmelerdeki Sözleşmeler, İş Emirleri, Tüketim Noktaları mocklanmış şekilde sayfa içinde foreach döngüsü boş kalacak şekilde bırakıldı, istendiğinde servisten çekilecek.
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Duzenle(int id)
        {
            var abone = _aboneService.GetById(id);
            if (abone == null) return NotFound();

            var model = new AboneEkleViewModel
            {
                IsTuzel = (abone.abone_tipi == "Tüzel" || abone.abone_tipi == "KURUMSAL"),
                AdSoyadUnvan = (abone.abone_tipi == "Tüzel" || abone.abone_tipi == "KURUMSAL") ? abone.Unvan : $"{abone.Ad} {abone.Soyad}",
                TCKN = abone.tckn,
                VKN = abone.vkn,
                Telefon = abone.telefon,
                Mail = abone.e_posta,
                TebligatAdresi = "Mevcut Adres (Mock)", // DB'de adres alanı olsaydı buradan dolardı
                KvkkOnayi = true
            };
            ViewBag.AboneId = id;

            return View("Duzenle", model);
        }

        [HttpPost]
        public IActionResult Duzenle(int id, AboneEkleViewModel model)
        {
            var tumAboneler = _aboneService.GetAll();

            if (!string.IsNullOrEmpty(model.TCKN) && tumAboneler.Any(a => a.tckn == model.TCKN && a.abone_id != id))
                ModelState.AddModelError("TCKN", "HATA: Bu TC Kimlik Numarası sistemde başka bir abonede kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.VKN) && tumAboneler.Any(a => a.vkn == model.VKN && a.abone_id != id))
                ModelState.AddModelError("VKN", "HATA: Bu Vergi Kimlik Numarası sistemde başka bir abonede kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.Telefon) && tumAboneler.Any(a => a.telefon == model.Telefon && a.abone_id != id))
                ModelState.AddModelError("Telefon", "HATA: Bu telefon numarası sistemde başka bir abonede kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.Mail) && tumAboneler.Any(a => a.e_posta == model.Mail && a.abone_id != id))
                ModelState.AddModelError("Mail", "HATA: Bu e-posta adresi sistemde başka bir abonede kayıtlı! Lütfen farklı bir değer girin.");

            if (ModelState.IsValid)
            {
                var abone = _aboneService.GetById(id);
                if (abone == null) return NotFound();

                abone.abone_tipi = model.IsTuzel ? "KURUMSAL" : "BIREYSEL";
                abone.telefon = model.Telefon;
                abone.e_posta = model.Mail;

                if (model.IsTuzel)
                {
                    abone.Unvan = model.AdSoyadUnvan;
                    abone.vkn = model.VKN ?? "";
                    abone.Ad = "";
                    abone.Soyad = "";
                    abone.tckn = "";
                }
                else
                {
                    var nameParts = model.AdSoyadUnvan.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    abone.Soyad = nameParts.Length > 1 ? nameParts.Last() : "";
                    abone.Ad = nameParts.Length > 1 ? string.Join(" ", nameParts.Take(nameParts.Length - 1)) : model.AdSoyadUnvan;
                    abone.tckn = model.TCKN ?? "";
                    abone.vkn = "";
                    abone.Unvan = "";
                }

                _aboneService.Update(abone);

                _auditLogService.Ekle(
                    varlikTipi: "Abone",
                    varlikId: id,
                    islemTipi: "UPDATE",
                    eskiDeger: "Bilinmiyor",
                    yeniDeger: model.AdSoyadUnvan,
                    kullaniciId: 1, // Mock User ID
                    islemGerekcesi: "Abone bilgileri güncellendi."
                );

                TempData["Mesaj"] = "Müşteri / Abone başarıyla güncellendi!";
                TempData["MesajTip"] = "success";
                return RedirectToAction("Index");
            }
            ViewBag.AboneId = id;
            return View("Duzenle", model);
        }

        [HttpPost]
        public IActionResult Sil(int id)
        {
            _aboneService.Delete(id);

            _auditLogService.Ekle(
                varlikTipi: "Abone",
                varlikId: id,
                islemTipi: "DELETE",
                eskiDeger: "Sistemde Kayıtlı Abone",
                yeniDeger: "SİLİNDİ",
                kullaniciId: 1, // Mock User ID
                islemGerekcesi: "Abone sistemden silindi."
            );

            TempData["Mesaj"] = "Abone başarıyla silindi.";
            TempData["MesajTip"] = "success";
            return RedirectToAction("Index");
        }

        private string Maskele(string deger)
        {
            if (string.IsNullOrWhiteSpace(deger) || deger.Length < 5) return "***";
            return deger.Substring(0, 3) + new string('*', deger.Length - 5) + deger.Substring(deger.Length - 2);
        }
    }
}