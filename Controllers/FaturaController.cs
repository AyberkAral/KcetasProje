using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;


namespace KcetasWeb.Controllers
{
    

    [Authorize]
    public class FaturaController : Controller
    {
        private readonly KcetasWeb.Services.Interfaces.ITuketimNoktasiService _tuketimNoktasiService;
        private readonly KcetasWeb.Services.Interfaces.IFaturaService _faturaService;

        public FaturaController(
            KcetasWeb.Services.Interfaces.ITuketimNoktasiService tuketimNoktasiService,
            KcetasWeb.Services.Interfaces.IFaturaService faturaService)
        {
            _tuketimNoktasiService = tuketimNoktasiService;
            _faturaService = faturaService;
        }

        public IActionResult Index(string FiltreFaturaNo, string FiltreTekilKod, string FiltreDonem, long? FiltreSozlesmeId, string FiltreDurum)
        {
            var faturalar = _faturaService.GetAll();
            
            var viewModels = faturalar.Select(f => new KcetasWeb.ViewModels.FaturaSimulasyonViewModel
            {
                fatura_id = f.fatura_id,
                fatura_no = f.fatura_no,
                sozlesme_id = f.sozlesme_id,
                okuma_id = f.okuma_id,
                tekil_kod = f.tekil_kod ?? "",
                fatura_tipi = f.fatura_tipi,
                fatura_tarihi = f.fatura_tarihi,
                son_odeme_tarihi = f.son_odeme_tarihi,
                donem = f.donem,
                ilk_endeks = f.ilk_endeks,
                son_endeks = f.son_endeks,
                tuketim_kwh = f.tuketim_kwh,
                carpan = f.carpan,
                enerji_bedeli = f.enerji_bedeli,
                dagatim_bedeli = f.dagatim_bedeli,
                vergi_fon_toplama = f.vergi_fon_toplama,
                toplam_tutar = f.toplam_tutar,
                durum = f.durum,
                status = f.status,
                created_at = f.created_at,
                // Validasyon hatalarını önlemek için FaturaSimulasyonViewModel'in required (zorunlu) alanlarını dolduruyoruz:
                TarifeGrubu = f.fatura_tipi ?? "Bilinmiyor",
                TuketimMiktari = f.tuketim_kwh ?? 0
            }).ToList();

            if (!string.IsNullOrEmpty(FiltreFaturaNo))
                viewModels = viewModels.Where(x => x.fatura_no != null && x.fatura_no.Contains(FiltreFaturaNo, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!string.IsNullOrEmpty(FiltreTekilKod))
                viewModels = viewModels.Where(x => x.tekil_kod != null && x.tekil_kod.Contains(FiltreTekilKod, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!string.IsNullOrEmpty(FiltreDonem))
                viewModels = viewModels.Where(x => x.donem != null && x.donem.Contains(FiltreDonem, StringComparison.OrdinalIgnoreCase)).ToList();

            if (FiltreSozlesmeId.HasValue)
                viewModels = viewModels.Where(x => x.sozlesme_id == FiltreSozlesmeId.Value).ToList();

            if (!string.IsNullOrEmpty(FiltreDurum))
                viewModels = viewModels.Where(x => x.durum != null && x.durum.Equals(FiltreDurum, StringComparison.OrdinalIgnoreCase)).ToList();

            // ViewBag'e filtre değerlerini atıyoruz ki formda seçili gelsinler
            ViewBag.FiltreFaturaNo = FiltreFaturaNo;
            ViewBag.FiltreTekilKod = FiltreTekilKod;
            ViewBag.FiltreDonem = FiltreDonem;
            ViewBag.FiltreSozlesmeId = FiltreSozlesmeId;
            ViewBag.FiltreDurum = FiltreDurum;

            return View(viewModels);
        }

        public IActionResult Olustur()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Olustur(Fatura fatura)
        {
            fatura.fatura_tarihi = DateTime.Now;
            fatura.son_odeme_tarihi = DateTime.Now.AddDays(15);
            fatura.donem = DateTime.Now.ToString("yyyy-MM");
            fatura.durum = "Bekliyor";
            fatura.status = "Active";
            fatura.created_at = DateTime.Now;
            
            _faturaService.Ekle(fatura);
            
            TempData["BasariMesaji"] = fatura.fatura_no + " numaralı fatura başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(int id)
        {
            var fatura = _faturaService.GetById(id);
            if (fatura == null)
            {
                return NotFound();
            }

            var tuketimNoktasi = _tuketimNoktasiService.GetById(fatura.tekil_kod);
            string aboneBilgisi = tuketimNoktasi != null ? $"{tuketimNoktasi.musteri_ad} {tuketimNoktasi.musteri_soyad} {tuketimNoktasi.musteri_unvan}".Trim() : "Bilinmiyor";
            ViewBag.AboneBilgisi = aboneBilgisi;

            var viewModel = new KcetasWeb.ViewModels.FaturaSimulasyonViewModel
            {
                fatura_id = fatura.fatura_id,
                fatura_no = fatura.fatura_no,
                sozlesme_id = fatura.sozlesme_id,
                okuma_id = fatura.okuma_id,
                tekil_kod = fatura.tekil_kod ?? "",
                fatura_tipi = fatura.fatura_tipi,
                fatura_tarihi = fatura.fatura_tarihi,
                son_odeme_tarihi = fatura.son_odeme_tarihi,
                donem = fatura.donem,
                ilk_endeks = fatura.ilk_endeks,
                son_endeks = fatura.son_endeks,
                tuketim_kwh = fatura.tuketim_kwh,
                carpan = fatura.carpan,
                enerji_bedeli = fatura.enerji_bedeli,
                dagatim_bedeli = fatura.dagatim_bedeli,
                vergi_fon_toplama = fatura.vergi_fon_toplama,
                toplam_tutar = fatura.toplam_tutar,
                durum = fatura.durum,
                status = fatura.status,
                created_at = fatura.created_at,
                TarifeGrubu = fatura.fatura_tipi ?? "Bilinmiyor",
                TuketimMiktari = fatura.tuketim_kwh ?? 0,
                Kalemler = new List<KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel>
                {
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Enerji Bedeli", Miktar = fatura.tuketim_kwh ?? 0, BirimFiyat = 2.85m, Tutar = fatura.enerji_bedeli ?? 0 },
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Dağıtım Bedeli", Miktar = fatura.tuketim_kwh ?? 0, BirimFiyat = 0.65m, Tutar = fatura.dagatim_bedeli ?? 0 },
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Vergi ve Fonlar", Miktar = 1, BirimFiyat = fatura.vergi_fon_toplama ?? 0, Tutar = fatura.vergi_fon_toplama ?? 0 },
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Hizmet Bedeli", Miktar = 1, BirimFiyat = 0, Tutar = 0 },
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Kesme / Bağlama Bedeli", Miktar = 1, BirimFiyat = 0, Tutar = 0 },
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Gecikme Zammı", Miktar = 1, BirimFiyat = 0, Tutar = 0 }
                }
            };

            return View(viewModel);
        }

        public IActionResult OdemeYap(int id)
        {
            var fatura = _faturaService.GetById(id);
            if (fatura != null)
            {
                fatura.durum = "Ödendi";
                fatura.updated_at = DateTime.Now;
                _faturaService.Guncelle(fatura);
                TempData["FaturaMesaji"] = fatura.fatura_no + " numaralı fatura başarıyla ödendi.";
            }
            return RedirectToAction("Index");
        }
    }
}
