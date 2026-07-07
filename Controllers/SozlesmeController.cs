using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,SozlesmeYetkilisi,MusteriTemsilcisi")]
    public class SozlesmeController : Controller
    {
        private static List<Sozlesme> _sozlesmeler = new List<Sozlesme>
        {
            new Sozlesme { sozlesme_id = 1, sozlesme_no = "SZL-10045", tuketim_noktasi_id = 1001, ad = "Ahmet", soyad = "Yılmaz", tckn = "12345678901", telefon = "05321234567", sozlesme_tipi = "Perakende Satış", statu = "Aktif", status = "Aktif", baslangic_tarihi = DateTime.Now.AddMonths(-12), guvence_bedeli = 1500.0m, tarife_grubu = "Mesken", created_at = DateTime.Now.AddMonths(-12) },
            new Sozlesme { sozlesme_id = 2, sozlesme_no = "SZL-10046", tuketim_noktasi_id = 1002, unvan = "Örnek Ltd. Şti.", vkn = "1234567890", telefon = "02121234567", sozlesme_tipi = "İkili Anlaşma", statu = "Güvence Bekliyor", status = "Güvence Bekliyor", baslangic_tarihi = DateTime.Now.AddDays(-2), guvence_bedeli = 5000.0m, tarife_grubu = "Ticarethane", created_at = DateTime.Now.AddDays(-2) },
            new Sozlesme { sozlesme_id = 3, sozlesme_no = "SZL-10047", tuketim_noktasi_id = 1003, ad = "Ayşe", soyad = "Demir", tckn = "11122233344", telefon = "05339998877", sozlesme_tipi = "Perakende Satış", statu = "Feshedildi", status = "Feshedildi", baslangic_tarihi = DateTime.Now.AddMonths(-24), bitis_tarihi = DateTime.Now.AddDays(-10), guvence_bedeli = 1200.0m, tarife_grubu = "Mesken", created_at = DateTime.Now.AddMonths(-24) }
        };

        public IActionResult Index()
        {
            return View(_sozlesmeler);
        }

        public IActionResult Yeni()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(Sozlesme model)
        {
            int count = _sozlesmeler.Count + 45; // Start from SZL-10045
            
            model.sozlesme_id = count;
            model.sozlesme_no = $"SZL-{10000 + count}";
            model.statu = "Güvence Bekliyor";
            model.status = "Güvence Bekliyor";
            model.baslangic_tarihi = DateTime.Now;
            model.created_at = DateTime.Now;

            _sozlesmeler.Add(model);

            TempData["SozlesmeMesaji"] = model.ad + " " + model.unvan + " için sözleşme başarıyla başlatıldı.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(string id)
        {
            var item = _sozlesmeler.FirstOrDefault(x => x.sozlesme_no == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        public IActionResult Duzenle(string id)
        {
            var item = _sozlesmeler.FirstOrDefault(x => x.sozlesme_no == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Duzenle(Sozlesme model)
        {
            var item = _sozlesmeler.FirstOrDefault(x => x.sozlesme_no == model.sozlesme_no);
            if (item != null)
            {
                item.tuketim_noktasi_id = model.tuketim_noktasi_id;
                item.ad = model.ad;
                item.soyad = model.soyad;
                item.unvan = model.unvan;
                item.tckn = model.tckn;
                item.vkn = model.vkn;
                item.telefon = model.telefon;
                item.e_posta = model.e_posta;
                item.iletisim_tercihi = model.iletisim_tercihi;
                item.sozlesme_tipi = model.sozlesme_tipi;
                item.tarife_grubu = model.tarife_grubu;
                item.guvence_bedeli = model.guvence_bedeli;
                item.statu = model.statu;
                item.status = model.statu;
                item.updated_at = DateTime.Now;
            }
            TempData["SozlesmeMesaji"] = model.sozlesme_no + " numaralı sözleşme başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.sozlesme_no });
        }

        public IActionResult Feshet(string id)
        {
            var item = _sozlesmeler.FirstOrDefault(x => x.sozlesme_no == id);
            if (item != null)
            {
                item.statu = "Feshedildi";
                item.status = "Feshedildi";
                item.bitis_tarihi = DateTime.Now;
                item.updated_at = DateTime.Now;
                TempData["SozlesmeMesaji"] = id + " numaralı sözleşme başarıyla feshedildi.";
            }
            return RedirectToAction("Index");
        }
    }
}
