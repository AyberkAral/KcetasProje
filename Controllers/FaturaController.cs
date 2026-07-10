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

        public FaturaController(KcetasWeb.Services.Interfaces.ITuketimNoktasiService tuketimNoktasiService)
        {
            _tuketimNoktasiService = tuketimNoktasiService;
        }

        public static List<Fatura> Faturalar = new List<Fatura>
        {
            new Fatura {
                fatura_id = 1, fatura_no = "FAT-2026-001", sozlesme_id = 1001, okuma_id = 5001, tekil_kod = "1001", fatura_tipi = "Mesken",
                fatura_tarihi = new DateTime(2026, 6, 30), son_odeme_tarihi = new DateTime(2026, 7, 15), donem = "2026-06",
                ilk_endeks = 15200, son_endeks = 15520, tuketim_kwh = 320, carpan = 1, enerji_bedeli = 912, dagatim_bedeli = 208,
                vergi_fon_toplama = 224, toplam_tutar = 1344.00m, durum = "Ödendi", status = "Active", created_at = new DateTime(2026, 6, 30)
            },
            new Fatura {
                fatura_id = 2, fatura_no = "FAT-2026-002", sozlesme_id = 1002, okuma_id = 5002, tekil_kod = "1002", fatura_tipi = "Ticarethane",
                fatura_tarihi = new DateTime(2026, 6, 30), son_odeme_tarihi = new DateTime(2026, 7, 15), donem = "2026-06",
                ilk_endeks = 18400, son_endeks = 18750, tuketim_kwh = 350, carpan = 1, enerji_bedeli = 1207.50m, dagatim_bedeli = 227.50m,
                vergi_fon_toplama = 287, toplam_tutar = 1722.00m, durum = "Bekliyor", status = "Active", created_at = new DateTime(2026, 6, 30)
            }
        };

        public IActionResult Index()
        {
            var viewModels = Faturalar.Select(f => new KcetasWeb.ViewModels.FaturaSimulasyonViewModel
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

            return View(viewModels);
        }

        public IActionResult Olustur()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Olustur(Fatura fatura)
        {
            fatura.fatura_id = Faturalar.Count > 0 ? Faturalar.Max(f => f.fatura_id) + 1 : 1;
            fatura.fatura_no = $"FAT-{DateTime.Now.Year}-{(fatura.fatura_id).ToString().PadLeft(3, '0')}";
            fatura.fatura_tarihi = DateTime.Now;
            fatura.son_odeme_tarihi = DateTime.Now.AddDays(15);
            fatura.donem = DateTime.Now.ToString("yyyy-MM");
            fatura.durum = "Bekliyor";
            fatura.status = "Active";
            fatura.created_at = DateTime.Now;
            
            Faturalar.Add(fatura);
            
            TempData["BasariMesaji"] = fatura.fatura_no + " numaralı fatura başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(int id)
        {
            var fatura = Faturalar.FirstOrDefault(x => x.fatura_id == id);
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
            var fatura = Faturalar.FirstOrDefault(x => x.fatura_id == id);
            if (fatura != null)
            {
                fatura.durum = "Ödendi";
                fatura.updated_at = DateTime.Now;
                TempData["FaturaMesaji"] = fatura.fatura_no + " numaralı fatura başarıyla ödendi.";
            }
            return RedirectToAction("Index");
        }
    }
}
