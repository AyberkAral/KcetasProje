using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi,Denetci ")]    
    public class TuketimNoktasiController : Controller
    {
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly ISayacService _sayacService;
        private readonly ISozlesmeService _sozlesmeService;
        private readonly IIsEmriService _isEmriService;
        private readonly IEndeksOkumaService _endeksOkumaService;
        private readonly IAboneService _aboneService;

        public TuketimNoktasiController(
            ITuketimNoktasiService tuketimNoktasiService,
            ISayacService sayacService,
            ISozlesmeService sozlesmeService,
            IIsEmriService isEmriService,
            IEndeksOkumaService endeksOkumaService,
            IAboneService aboneService)
        {
            _tuketimNoktasiService = tuketimNoktasiService;
            _sayacService = sayacService;
            _sozlesmeService = sozlesmeService;
            _isEmriService = isEmriService;
            _endeksOkumaService = endeksOkumaService;
            _aboneService = aboneService;
        }

        public IActionResult Index()
        {
            var data = _tuketimNoktasiService.GetAll();
            var viewModels = data.Select(item => new KcetasWeb.ViewModels.TuketimNoktasiViewModels
            {
                TuketimNoktasiId = item.tuketim_noktasi_id,
                tekil_kod = item.tekil_kod,
                baglanti_gucu_kw = item.baglanti_gucu_kw,
                ilce_id = item.ilce_id,
                ilce_adi = item.ilce_id switch
                {
                    1 => "Melikgazi", 2 => "Kocasinan", 3 => "Talas", 4 => "Akkışla", 5 => "Bünyan",
                    6 => "Develi", 7 => "Felahiye", 8 => "Hacılar", 9 => "İncesu", 10 => "Özvatan",
                    11 => "Pınarbaşı", 12 => "Sarıoğlan", 13 => "Sarız", 14 => "Tomarza", 15 => "Yahyalı",
                    16 => "Yeşilhisar", 99 => "Merkez İlçe", _ => "Bilinmeyen İlçe"
                },
                bina_no = item.bina_no,
                bagimsiz_bolum_no = item.bagimsiz_bolum_no,
                tuketici_grubu = item.tuketici_grubu,
                status = item.status
            }).ToList();
            return View(viewModels);
        }

        public IActionResult Yeni()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(KcetasWeb.ViewModels.TuketimNoktasiViewModels model)
        {
            var allData = _tuketimNoktasiService.GetAll();
            
            if (model.koordinat_lat != 0 && model.koordinat_lot != 0 &&
                allData.Any(x => x.koordinat_lat == model.koordinat_lat && x.koordinat_lot == model.koordinat_lot))
            {
                ModelState.AddModelError("koordinat_lat", "HATA: Bu koordinatlara sahip başka bir tüketim noktası sistemde zaten mevcut! Lütfen farklı koordinatlar girin.");
            }

            var aboneler = _aboneService.GetAll();
            if (!string.IsNullOrEmpty(model.tckn) && aboneler.Any(a => a.tckn == model.tckn))
                ModelState.AddModelError("tckn", "HATA: Bu TC Kimlik Numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.vkn) && aboneler.Any(a => a.vkn == model.vkn))
                ModelState.AddModelError("vkn", "HATA: Bu Vergi Kimlik Numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.telefon) && aboneler.Any(a => a.telefon == model.telefon))
                ModelState.AddModelError("telefon", "HATA: Bu telefon numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.e_posta) && aboneler.Any(a => a.e_posta == model.e_posta))
                ModelState.AddModelError("e_posta", "HATA: Bu e-posta adresi sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int maxId = allData.Any() ? (int)allData.Max(x => x.tuketim_noktasi_id) : 0;
            
            var yeniNokta = new TuketimNoktasi
            {
                tuketim_noktasi_id = maxId + 1,
                tekil_kod = $"TK-2026-{(maxId + 1).ToString().PadLeft(3, '0')}",
                ilce_id = model.ilce_id,
                mahalle = model.mahalle,
                bina_no = model.bina_no,
                bagimsiz_bolum_no = model.bagimsiz_bolum_no,
                acik_adres = model.acik_adres,
                baglanti_gucu_kw = model.baglanti_gucu_kw,
                koordinat_lat = model.koordinat_lat,
                koordinat_lot = model.koordinat_lot,
                tuketici_grubu = model.tuketici_grubu,
                baglanti_durumu = model.baglanti_durumu,
                status = "Pasif"
            };

            _tuketimNoktasiService.Create(yeniNokta);

            // Abone bilgilerini ayır ve API'ye gönder
            if (!string.IsNullOrEmpty(model.tckn) || !string.IsNullOrEmpty(model.vkn))
            {
                var abone = new Abone
                {
                    tckn = model.tckn,
                    vkn = model.vkn,
                    telefon = model.telefon ?? "0000000000",
                    e_posta = model.e_posta,
                    abone_tipi = !string.IsNullOrEmpty(model.vkn) ? "KURUMSAL" : "BIREYSEL",
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    Unvan = model.Unvan
                };
                
                try 
                {
                    _aboneService.Create(abone);
                    
                    // Eklenen abonenin ID'sini bulmak için listeyi çekiyoruz
                    var guncelAboneler = _aboneService.GetAll();
                    var eklenenAbone = guncelAboneler.OrderByDescending(a => a.abone_id).FirstOrDefault(a => (a.tckn == model.tckn && !string.IsNullOrEmpty(model.tckn)) || (a.vkn == model.vkn && !string.IsNullOrEmpty(model.vkn)));

                    if (eklenenAbone != null)
                    {
                        // Müşteri isteği: Tüketim noktası ile birlikte otomatik sözleşme oluşmayacak.
                        // Sözleşme işlemi Sözleşmeler sayfasından manuel yapılacaktır.
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Abone/Sözleşme kaydı API hatası: " + ex.Message);
                    // Tüketim noktası kaydedildiği için devam ediyoruz
                }
            }

            TempData["BasariMesaji"] = "Harika! Yeni tüketim noktası ve abone başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(string id)
        {
            var item = _tuketimNoktasiService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }

            var viewModel = new KcetasWeb.ViewModels.TuketimNoktasiViewModels
            {
                TuketimNoktasiId = item.tuketim_noktasi_id,
                tekil_kod = item.tekil_kod,
                ilce_id = item.ilce_id,
                mahalle = item.mahalle,
                bina_no = item.bina_no,
                bagimsiz_bolum_no = item.bagimsiz_bolum_no,
                acik_adres = item.acik_adres,
                baglanti_gucu_kw = item.baglanti_gucu_kw,
                koordinat_lat = item.koordinat_lat ?? 0m,
                koordinat_lot = item.koordinat_lot ?? 0m,
                tuketici_grubu = item.tuketici_grubu,
                baglanti_durumu = item.baglanti_durumu,
                status = item.status
            };

            // İlişkili verilerin çekilmesi
            var sayaclar = _sayacService.GetAll().Where(s => s.tuketim_noktasi_id == item.tuketim_noktasi_id).ToList();
            ViewBag.Sayaclar = sayaclar;
            var sozlesmeler = _sozlesmeService.GetAll().Where(s => s.tuketim_noktasi_id == item.tuketim_noktasi_id).ToList();
            ViewBag.Sozlesmeler = sozlesmeler;
            ViewBag.IsEmirleri = _isEmriService.GetAll().Where(i => i.tuketim_noktasi_id == item.tuketim_noktasi_id).OrderByDescending(i => i.created_at).ToList();

            var aktifSozlesme = sozlesmeler.OrderByDescending(s => s.baslangic_tarihi).FirstOrDefault(s => s.durum == "Aktif" || s.durum == "AKTIF");
            if (aktifSozlesme != null)
            {
                var abone = _aboneService.GetById((int)aktifSozlesme.abone_id);
                if (abone != null)
                {
                    viewModel.Ad = abone.Ad;
                    viewModel.Soyad = abone.Soyad;
                    viewModel.Unvan = abone.Unvan;
                    viewModel.tckn = abone.tckn;
                    viewModel.vkn = abone.vkn;
                    viewModel.telefon = abone.telefon;
                    viewModel.e_posta = abone.e_posta;
                }
            }
            
            var sayacIds = sayaclar.Select(s => s.sayac_id).ToList();
            ViewBag.Endeksler = _endeksOkumaService.GetAll()
                .Where(e => e.sayac_id.HasValue && sayacIds.Contains(e.sayac_id.Value))
                .OrderByDescending(e => e.okuma_zamani)
                .ToList();

            return View(viewModel);
        }

        public IActionResult Duzenle(string id)
        {
            var item = _tuketimNoktasiService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Duzenle(TuketimNoktasi model)
        {
            var allData = _tuketimNoktasiService.GetAll();
            if (model.koordinat_lat.HasValue && model.koordinat_lot.HasValue &&
                allData.Any(x => x.koordinat_lat == model.koordinat_lat && x.koordinat_lot == model.koordinat_lot && x.tekil_kod != model.tekil_kod))
            {
                ModelState.AddModelError("koordinat_lat", "HATA: Bu koordinatlara sahip başka bir tüketim noktası sistemde zaten mevcut! Lütfen farklı koordinatlar girin.");
                return View(model);
            }

            var item = _tuketimNoktasiService.GetById(model.tekil_kod);
            if (item != null)
            {
                // Abone bilgileri UI'dan kaldırıldığı için burada güncellenmemeli.
                item.ilce_id = model.ilce_id;
                
                item.mahalle = model.mahalle;
                item.bina_no = model.bina_no;
                item.bagimsiz_bolum_no = model.bagimsiz_bolum_no;
                item.acik_adres = model.acik_adres;
                item.baglanti_gucu_kw = model.baglanti_gucu_kw;
                item.koordinat_lat = model.koordinat_lat;
                item.koordinat_lot = model.koordinat_lot;
                item.tuketici_grubu = model.tuketici_grubu;
                item.baglanti_durumu = model.baglanti_durumu;
                item.status = model.status;
                item.updated_at = DateTime.Now;

                _tuketimNoktasiService.Update(item);
            }
            TempData["BasariMesaji"] = model.tekil_kod + " kodlu nokta başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.tekil_kod });
        }
    }
}