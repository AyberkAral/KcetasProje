using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi,SahaOperasyonAmiri,Denetci,FaturalamaUzmani,Yonetici")]
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
            foreach(var item in data)
            {
                switch (item.ilce_id)
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
            }
            return View(data);
        }

        public IActionResult Yeni()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(TuketimNoktasi model)
        {
            var allData = _tuketimNoktasiService.GetAll();
            int maxId = allData.Any() ? (int)allData.Max(x => x.TuketimNoktasiId) : 0;
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

            _tuketimNoktasiService.Create(model);

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
                    Ad = model.musteri_ad,
                    Soyad = model.musteri_soyad,
                    Unvan = model.musteri_unvan
                };
                
                try 
                {
                    _aboneService.Create(abone);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Abone kaydı API hatası: " + ex.Message);
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
                baglanti_gucu_kw = item.baglanti_gucu_kw,
                koordinat_lat = item.koordinat_lat,
                koordinat_lot = item.koordinat_lot,
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

            // İlişkili verilerin çekilmesi
            var sayaclar = _sayacService.GetAll().Where(s => s.tuketim_noktasi_id == item.TuketimNoktasiId).ToList();
            ViewBag.Sayaclar = sayaclar;
            ViewBag.Sozlesmeler = _sozlesmeService.GetAll().Where(s => s.tuketim_noktasi_id == item.TuketimNoktasiId).ToList();
            ViewBag.IsEmirleri = _isEmriService.GetAll().Where(i => i.tuketim_noktasi_id == item.TuketimNoktasiId).OrderByDescending(i => i.CreatedAt).ToList();
            
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
            var item = _tuketimNoktasiService.GetById(model.tekil_kod);
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
                item.baglanti_gucu_kw = model.baglanti_gucu_kw;
                item.koordinat_lat = model.koordinat_lat;
                item.koordinat_lot = model.koordinat_lot;
                item.tuketici_grubu = model.tuketici_grubu;
                item.baglanti_grubu = model.baglanti_grubu;
                item.status = model.status;
                item.UpdatedAt = DateTime.Now;

                _tuketimNoktasiService.Update(item);
            }
            TempData["BasariMesaji"] = model.tekil_kod + " kodlu nokta başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.tekil_kod });
        }
    }
}