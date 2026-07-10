using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,SozlesmeYetkilisi,MusteriTemsilcisi,Yonetici")]
    public class SozlesmeController : Controller
    {
        private readonly ISozlesmeService _sozlesmeService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;

        public SozlesmeController(ISozlesmeService sozlesmeService, ITuketimNoktasiService tuketimNoktasiService)
        {
            _sozlesmeService = sozlesmeService;
            _tuketimNoktasiService = tuketimNoktasiService;
        }

        public IActionResult Index()
        {
            var sozlesmeler = _sozlesmeService.GetAll();
            var tuketimNoktalari = _tuketimNoktasiService.GetAll();

            var viewModels = sozlesmeler.Select(s => new KcetasWeb.ViewModels.SozlesmeViewModels
            {
                sozlesme_id = s.sozlesme_id,
                sozlesme_no = s.sozlesme_no,
                tuketim_noktasi_id = s.tuketim_noktasi_id,
                ad = s.ad ?? "",
                soyad = s.soyad ?? "",
                unvan = s.unvan ?? "",
                tckn = s.tckn ?? "",
                vkn = s.vkn ?? "",
                telefon = s.telefon ?? "",
                e_posta = s.e_posta ?? "",
                iletisim_tercihi = s.iletisim_tercihi ?? "",
                sozlesme_tipi = s.sozlesme_tipi ?? "",
                baslangic_tarihi = s.baslangic_tarihi,
                bitis_tarihi = s.bitis_tarihi,
                statu = s.statu ?? "",
                tarife_grubu = s.tarife_grubu ?? "",
                guvence_bedeli = s.guvence_bedeli,
                status = s.status ?? "",
                created_by = s.created_by,
                updated_by = s.updated_by,
                created_at = s.created_at,
                updated_at = s.updated_at,
                tekil_kod = tuketimNoktalari.FirstOrDefault(t => t.TuketimNoktasiId == s.tuketim_noktasi_id)?.tekil_kod ?? "Bilinmiyor"
            }).ToList();

            return View(viewModels);
        }

        public IActionResult Yeni()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(Sozlesme model)
        {
            var sozlesmeler = _sozlesmeService.GetAll();
            int count = sozlesmeler.Count + 45; // Start from SZL-10045 if using mock generation logic
            
            model.sozlesme_id = count;
            model.sozlesme_no = $"SZL-{10000 + count}";
            model.statu = "Güvence Bekliyor";
            model.status = "Güvence Bekliyor";
            model.baslangic_tarihi = DateTime.Now;
            model.created_at = DateTime.Now;

            _sozlesmeService.Create(model);

            TempData["SozlesmeMesaji"] = model.ad + " " + model.unvan + " için sözleşme başarıyla başlatıldı.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(string id)
        {
            var item = _sozlesmeService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.GecmisSozlesmeler = _sozlesmeService.GetAll()
                .Where(s => s.tuketim_noktasi_id == item.tuketim_noktasi_id && s.sozlesme_id != item.sozlesme_id)
                .OrderByDescending(s => s.baslangic_tarihi)
                .ToList();

            return View(item);
        }

        public IActionResult Duzenle(string id)
        {
            var item = _sozlesmeService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Duzenle(Sozlesme model)
        {
            var item = _sozlesmeService.GetById(model.sozlesme_no);
            if (item != null)
            {
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
                item.sayac = model.sayac;
                item.baslangic_endeksi = model.baslangic_endeksi;
                item.odeme_sekli = model.odeme_sekli;
                item.statu = model.statu;
                item.status = model.statu;
                item.updated_at = DateTime.Now;

                _sozlesmeService.Update(item);
            }
            TempData["SozlesmeMesaji"] = model.sozlesme_no + " numaralı sözleşme başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.sozlesme_no });
        }

        public IActionResult Feshet(string id)
        {
            var item = _sozlesmeService.GetById(id);
            if (item != null)
            {
                item.statu = "Feshedildi";
                item.status = "Feshedildi";
                item.bitis_tarihi = DateTime.Now;
                item.updated_at = DateTime.Now;

                _sozlesmeService.Update(item);
                TempData["SozlesmeMesaji"] = id + " numaralı sözleşme başarıyla feshedildi.";
            }
            return RedirectToAction("Index");
        }
    }
}
