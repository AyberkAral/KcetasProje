using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi,SahaOperasyonAmiri,Denetci,FaturalamaUzmani,Yonetici")]
    public class TuketimNoktasiController : Controller
    {
        public static List<TuketimNoktasi> _tuketimNoktalari = new List<TuketimNoktasi>
        {
            new TuketimNoktasi {
                TuketimNoktasiId = 1, tekil_kod = "TK-2026-001",
                musteri_ad = "Ahmet", musteri_soyad = "Yılmaz", tckn = "12345678901", telefon = "05321234567",
                il_adi = "Kayseri", ilce_id = 1, ilce_adi = "Melikgazi", mahalle = "Merkez", bina_no = "1", bagimsiz_bolum_no = "12", acik_adres = "Merkez Mah. 1. Sokak No:1 D:12",
                tuketici_grubu = "Mesken", baglanti_grubu = "AG", status = "Aktif",
                BaglantiGucuKw = 15.5m, Enlem = "38.7205", Boylam = "35.4826",
                CreatedAt = DateTime.Now.AddDays(-10)
            },
            new TuketimNoktasi {
                TuketimNoktasiId = 2, tekil_kod = "TK-2026-002",
                musteri_unvan = "Örnek Ltd. Şti.", vkn = "1234567890", telefon = "02121234567",
                il_adi = "Kayseri", ilce_id = 2, ilce_adi = "Kocasinan", mahalle = "Sanayi", bina_no = "2", bagimsiz_bolum_no = "4", acik_adres = "Sanayi Mah. 2. Cadde No:2 D:4",
                tuketici_grubu = "Ticarethane", baglanti_grubu = "OG", status = "Pasif",
                BaglantiGucuKw = 50.0m, Enlem = "38.7300", Boylam = "35.4900",
                CreatedAt = DateTime.Now.AddDays(-5)
            }
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
        public IActionResult Yeni(TuketimNoktasi model)
        {
            int maxId = _tuketimNoktalari.Any() ? (int)_tuketimNoktalari.Max(x => x.TuketimNoktasiId) : 0;
            model.TuketimNoktasiId = maxId + 1;
            model.tekil_kod = $"TK-2026-{(maxId + 1).ToString().PadLeft(3, '0')}";
            model.status = "Pasif";
            model.CreatedAt = DateTime.Now;

            switch (model.ilce_id)
            {
                case 1: model.ilce_adi = "Melikgazi"; break;
                case 2: model.ilce_adi = "Kocasinan"; break;
                case 3: model.ilce_adi = "Talas"; break;
                case 4: model.ilce_adi = "Akkışla"; break;
                case 5: model.ilce_adi = "Bünyan"; break;
                case 6: model.ilce_adi = "Develi"; break;
                case 7: model.ilce_adi = "Felahiye"; break;
                case 8: model.ilce_adi = "Hacılar"; break;
                case 9: model.ilce_adi = "İncesu"; break;
                case 10: model.ilce_adi = "Özvatan"; break;
                case 11: model.ilce_adi = "Pınarbaşı"; break;
                case 12: model.ilce_adi = "Sarıoğlan"; break;
                case 13: model.ilce_adi = "Sarız"; break;
                case 14: model.ilce_adi = "Tomarza"; break;
                case 15: model.ilce_adi = "Yahyalı"; break;
                case 16: model.ilce_adi = "Yeşilhisar"; break;
                case 99: model.ilce_adi = "Merkez İlçe"; break;
                default: model.ilce_adi = "Bilinmeyen İlçe"; break;
            }

            _tuketimNoktalari.Add(model);

            TempData["BasariMesaji"] = "Harika! Yeni tüketim noktası ve abone başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(string id)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.tekil_kod == id);
            if (item == null)
            {
                return NotFound();
            }

            var viewModel = new KcetasWeb.ViewModels.TuketimNoktasiViewModels
            {
                TuketimNoktasiId = item.TuketimNoktasiId,
                tekil_kod = item.tekil_kod,
                musteri_ad = item.musteri_ad,
                musteri_soyad = item.musteri_soyad,
                musteri_unvan = item.musteri_unvan,
                tckn = item.tckn,
                vkn = item.vkn,
                telefon = item.telefon,
                e_posta = item.e_posta,
                iletisim_tercihi = item.iletisim_tercihi,
                ilce_id = item.ilce_id,
                il_adi = item.il_adi ?? "",
                ilce_adi = item.ilce_adi ?? "",
                mahalle = item.mahalle,
                bina_no = item.bina_no,
                bagimsiz_bolum_no = item.bagimsiz_bolum_no,
                acik_adres = item.acik_adres,
                BaglantiGucuKw = item.BaglantiGucuKw,
                Enlem = item.Enlem,
                Boylam = item.Boylam,
                tuketici_grubu = item.tuketici_grubu,
                baglanti_grubu = item.baglanti_grubu,
                status = item.status,
                sayac_id = long.TryParse(item.sayac_id, out long sid) ? sid : 0,
                sozlesme_id = long.TryParse(item.sozlesme_id, out long sozid) ? sozid : 0,
                is_emri_no = item.is_emri_no ?? "",
                okuma_id = long.TryParse(item.okuma_id, out long oid) ? oid : 0,
                Ad = item.musteri_ad ?? "",
                Soyad = item.musteri_soyad ?? "",
                Unvan = item.musteri_unvan
            };

            return View(viewModel);
        }

        public IActionResult Duzenle(string id)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.tekil_kod == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Duzenle(TuketimNoktasi model)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.tekil_kod == model.tekil_kod);
            if (item != null)
            {
                item.musteri_ad = model.musteri_ad;
                item.musteri_soyad = model.musteri_soyad;
                item.musteri_unvan = model.musteri_unvan;
                item.tckn = model.tckn;
                item.vkn = model.vkn;
                item.telefon = model.telefon;
                item.e_posta = model.e_posta;
                item.iletisim_tercihi = model.iletisim_tercihi;
                item.il_adi = model.il_adi;
                item.ilce_id = model.ilce_id;
                
                switch (model.ilce_id)
                {
                    case 1: item.ilce_adi = "Melikgazi"; break;
                    case 2: item.ilce_adi = "Kocasinan"; break;
                    case 3: item.ilce_adi = "Talas"; break;
                    case 4: item.ilce_adi = "Akkışla"; break;
                    case 5: item.ilce_adi = "Bünyan"; break;
                    case 6: item.ilce_adi = "Develi"; break;
                    case 7: item.ilce_adi = "Felahiye"; break;
                    case 8: item.ilce_adi = "Hacılar"; break;
                    case 9: item.ilce_adi = "İncesu"; break;
                    case 10: item.ilce_adi = "Özvatan"; break;
                    case 11: item.ilce_adi = "Pınarbaşı"; break;
                    case 12: item.ilce_adi = "Sarıoğlan"; break;
                    case 13: item.ilce_adi = "Sarız"; break;
                    case 14: item.ilce_adi = "Tomarza"; break;
                    case 15: item.ilce_adi = "Yahyalı"; break;
                    case 16: item.ilce_adi = "Yeşilhisar"; break;
                    case 99: item.ilce_adi = "Merkez İlçe"; break;
                    default: item.ilce_adi = "Bilinmeyen İlçe"; break;
                }

                item.mahalle = model.mahalle;
                item.bina_no = model.bina_no;
                item.bagimsiz_bolum_no = model.bagimsiz_bolum_no;
                item.acik_adres = model.acik_adres;
                item.BaglantiGucuKw = model.BaglantiGucuKw;
                item.Enlem = model.Enlem;
                item.Boylam = model.Boylam;
                item.tuketici_grubu = model.tuketici_grubu;
                item.baglanti_grubu = model.baglanti_grubu;
                item.status = model.status;
                item.UpdatedAt = DateTime.Now;
            }
            TempData["BasariMesaji"] = model.tekil_kod + " kodlu nokta başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.tekil_kod });
        }
    }
}