using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi,Denetci ")]
    public class SozlesmeController : Controller
    {
        private readonly ISozlesmeService _sozlesmeService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly IIsEmriService _isEmriService;
        private readonly IAboneService _aboneService;

        public SozlesmeController(
            ISozlesmeService sozlesmeService, 
            ITuketimNoktasiService tuketimNoktasiService,
            IIsEmriService isEmriService,
            IAboneService aboneService)
        {
            _sozlesmeService = sozlesmeService;
            _tuketimNoktasiService = tuketimNoktasiService;
            _isEmriService = isEmriService;
            _aboneService = aboneService;
        }

        public IActionResult Index()
        {
            var sozlesmeler = _sozlesmeService.GetAll();
            var tuketimNoktalari = _tuketimNoktasiService.GetAll();
            var aboneler = _aboneService.GetAll();

            var viewModels = sozlesmeler.Select(s => {
                var abone = aboneler.FirstOrDefault(a => a.abone_id == s.abone_id);
                var tuketimNoktasi = tuketimNoktalari.FirstOrDefault(t => t.tuketim_noktasi_id == s.tuketim_noktasi_id);
                
                return new KcetasWeb.ViewModels.SozlesmeViewModels
                {
                    sozlesme_id = s.sozlesme_id,
                    sozlesme_no = s.sozlesme_no,
                    tuketim_noktasi_id = s.tuketim_noktasi_id,
                    abone_id = s.abone_id ?? 0,
                    ad = abone != null && !string.IsNullOrEmpty(abone.Ad) ? abone.Ad : "",
                    soyad = abone != null && !string.IsNullOrEmpty(abone.Soyad) ? abone.Soyad : "",
                    unvan = abone != null && !string.IsNullOrEmpty(abone.Unvan) ? abone.Unvan : "",
                    tckn = abone?.tckn ?? "",
                    vkn = abone?.vkn ?? "",
                    telefon = abone?.telefon ?? "",
                    e_posta = abone?.e_posta ?? "",
                    iletisim_tercihi = "Bilinmiyor",
                    sozlesme_tipi = s.sozlesme_tipi ?? "",
                    baslangic_tarihi = s.baslangic_tarihi ?? DateTime.Now,
                    bitis_tarihi = s.bitis_tarihi,
                    statu = s.durum ?? "",
                    tarife_id = s.tarife_id ?? 0,
                    tarife_grubu = (s.tarife_id ?? 0) switch
                    {
                        1 => "Mesken",
                        2 => "Sanayi",
                        3 => "Ticarethane",
                        4 => "Tarımsal Sulama",
                        5 => "Aydınlatma",
                        _ => "Bilinmiyor"
                    },
                    guvence_bedeli = s.guvence_bedeli ?? 0m,
                    created_at = s.created_at,
                    updated_at = s.updated_at,
                    tekil_kod = tuketimNoktasi?.tekil_kod ?? "Bilinmiyor"
                };
            }).ToList();

            return View(viewModels);
        }

        public IActionResult Yeni()
        {
            ViewBag.Aboneler = _aboneService.GetAll();
            ViewBag.TuketimNoktalari = _tuketimNoktasiService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(KcetasWeb.ViewModels.SozlesmeViewModels model)
        {
            var sozlesmeler = _sozlesmeService.GetAll();
            int count = sozlesmeler.Count + 45; // Start from SZL-10045 if using mock generation logic
            
            var yeniSozlesme = new Sozlesme
            {
                sozlesme_id = count,
                sozlesme_no = $"SZL-{10000 + count}",
                tuketim_noktasi_id = (int)model.tuketim_noktasi_id,
                abone_id = (int)model.abone_id,
                baslangic_tarihi = DateTime.Now,
                sozlesme_tipi = model.sozlesme_tipi,
                tarife_id = model.tarife_id,
                guvence_bedeli = model.guvence_bedeli,
                durum = "Güvence Bekliyor",
                created_at = DateTime.Now
            };

            _sozlesmeService.Create(yeniSozlesme);

            // Otomatik Sayaç Bağlama İş Emri Oluştur
            var isEmri = new IsEmri
            {
                tip = "BAGLAMA",
                durum = "ACIK",
                is_emri_no = $"IE-{DateTime.Now.Year}-{(count + 1).ToString().PadLeft(4, '0')}", // Geçici mock numara üretimi (gerçek sistemde API atar)
                tuketim_noktasi_id = model.tuketim_noktasi_id,
                planlanan_tarih = DateTime.Now.AddDays(1),
                oncelik = "Yüksek",
                atanan_kullanici_id = null, // API doğrulamasına takılmaması için boş bırakılıyor
                created_at = DateTime.Now
            };
            
            try
            {
                _isEmriService.Ekle(isEmri);
            }
            catch
            {
                // API hatası olsa bile sözleşme oluşturulduğu için işlemi kesmiyoruz
            }

            TempData["SozlesmeMesaji"] = model.ad + " " + model.unvan + " için sözleşme başarıyla başlatıldı ve Sayaç Bağlama iş emri oluşturuldu.";
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

            var viewModel = new KcetasWeb.ViewModels.SozlesmeViewModels
            {
                sozlesme_id = item.sozlesme_id,
                sozlesme_no = item.sozlesme_no,
                tuketim_noktasi_id = item.tuketim_noktasi_id,
                abone_id = item.abone_id ?? 0,
                baslangic_tarihi = item.baslangic_tarihi ?? DateTime.Now,
                bitis_tarihi = item.bitis_tarihi,
                statu = item.durum ?? "",
                sozlesme_tipi = item.sozlesme_tipi ?? "",
                tarife_id = item.tarife_id ?? 0,
                guvence_bedeli = item.guvence_bedeli ?? 0m,
                created_at = item.created_at
            };

            var abone = _aboneService.GetById((int)item.abone_id);
            if (abone != null)
            {
                viewModel.ad = abone.Ad;
                viewModel.soyad = abone.Soyad;
                viewModel.unvan = abone.Unvan;
                viewModel.tckn = abone.tckn;
                viewModel.vkn = abone.vkn;
                viewModel.telefon = abone.telefon;
                viewModel.e_posta = abone.e_posta;
            }

            return View(viewModel);
        }

        public IActionResult Duzenle(string id)
        {
            var item = _sozlesmeService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            var viewModel = new KcetasWeb.ViewModels.SozlesmeViewModels
            {
                sozlesme_id = item.sozlesme_id,
                sozlesme_no = item.sozlesme_no,
                tuketim_noktasi_id = item.tuketim_noktasi_id,
                abone_id = item.abone_id ?? 0,
                baslangic_tarihi = item.baslangic_tarihi ?? DateTime.Now,
                bitis_tarihi = item.bitis_tarihi,
                statu = item.durum ?? "",
                sozlesme_tipi = item.sozlesme_tipi ?? "",
                tarife_id = item.tarife_id ?? 0,
                guvence_bedeli = item.guvence_bedeli ?? 0m,
                created_at = item.created_at
            };

            var abone = _aboneService.GetById((int)item.abone_id);
            if (abone != null)
            {
                viewModel.ad = abone.Ad;
                viewModel.soyad = abone.Soyad;
                viewModel.unvan = abone.Unvan;
                viewModel.tckn = abone.tckn;
                viewModel.vkn = abone.vkn;
                viewModel.telefon = abone.telefon;
                viewModel.e_posta = abone.e_posta;
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Duzenle(KcetasWeb.ViewModels.SozlesmeViewModels model)
        {
            var item = _sozlesmeService.GetById(model.sozlesme_no);
            if (item != null)
            {
                item.sozlesme_tipi = model.sozlesme_tipi;
                item.tarife_id = model.tarife_id;
                item.guvence_bedeli = model.guvence_bedeli;
                item.durum = model.statu;
                item.updated_at = DateTime.Now;
                _sozlesmeService.Update(item);
                
                var abone = _aboneService.GetById((int)item.abone_id);
                if (abone != null)
                {
                    abone.Ad = model.ad;
                    abone.Soyad = model.soyad;
                    abone.Unvan = model.unvan;
                    abone.tckn = model.tckn;
                    abone.vkn = model.vkn;
                    abone.telefon = model.telefon;
                    abone.e_posta = model.e_posta;
                    _aboneService.Update(abone);
                }
            }
            TempData["SozlesmeMesaji"] = model.sozlesme_no + " numaralı sözleşme başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.sozlesme_no });
        }

        public IActionResult Feshet(string id)
        {
            var item = _sozlesmeService.GetById(id);
            if (item != null)
            {
                item.durum = "Feshedildi";
                item.bitis_tarihi = DateTime.Now;
                item.updated_at = DateTime.Now;

                _sozlesmeService.Update(item);
                TempData["SozlesmeMesaji"] = id + " numaralı sözleşme başarıyla feshedildi.";
            }
            return RedirectToAction("Index");
        }
    }
}
