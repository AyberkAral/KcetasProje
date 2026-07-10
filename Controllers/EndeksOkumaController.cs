using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using System;
using System.Linq;

namespace KcetasWeb.Controllers
{
    public class EndeksOkumaController : Controller
    {
        private readonly IEndeksOkumaService _endeksOkumaService;
        private readonly ISozlesmeService _sozlesmeService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly ISayacService _sayacService;
        private readonly IFaturaService _faturaService;

        public EndeksOkumaController(
            IEndeksOkumaService endeksOkumaService,
            ISozlesmeService sozlesmeService,
            ITuketimNoktasiService tuketimNoktasiService,
            ISayacService sayacService,
            IFaturaService faturaService)
        {
            _endeksOkumaService = endeksOkumaService;
            _sozlesmeService = sozlesmeService;
            _tuketimNoktasiService = tuketimNoktasiService;
            _sayacService = sayacService;
            _faturaService = faturaService;
        }

        public IActionResult Index(string? kaynak, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var okumalar = _endeksOkumaService.Filtrele(kaynak, durum, baslangic, bitis, arama);
            var sozlesmeler = _sozlesmeService.GetAll();
            
            var viewModels = okumalar.Select(o => {
                var sozlesme = sozlesmeler.FirstOrDefault(s => s.sozlesme_id == o.sozlesme_id);
                string aboneBilgisi = sozlesme != null ? $"{sozlesme.ad} {sozlesme.soyad} {sozlesme.unvan}".Trim() : "Bilinmiyor";
                
                return new KcetasWeb.ViewModels.EndeksOkumaViewModels
                {
                    okuma_id = o.okuma_id,
                    sayac_id = o.sayac_id,
                    is_emri_id = o.is_emri_id,
                    sozlesme_id = o.sozlesme_id,
                    okuma_tipi = o.okuma_tipi ?? "Normal",
                    okuma_kaynagi = o.okuma_kaynagi,
                    onceki_endeks = o.onceki_endeks,
                    yeni_endeks = o.yeni_endeks,
                    okuma_zamani = o.okuma_zamani,
                    kullanici_id = o.kullanici_id,
                    okunamam_nedeni = o.okunamam_nedeni,
                    dogrulama_durumu = o.dogrulama_durumu,
                    anomali_mi = o.anomali_mi,
                    status = o.status,
                    AnomaliAciklamasi = o.AnomaliAciklamasi,
                    sökme_nedeni = o.sökme_nedeni ?? "",
                    aciklama = o.aciklama ?? "",
                    son_endeks = o.son_endeks,
                    CreatedAt = o.CreatedAt,
                    abone = aboneBilgisi
                };
            }).ToList();

            ViewBag.Istatistikler = _endeksOkumaService.GetIstatistikler();
            return View(viewModels);
        }

        public IActionResult Detay(long id)
        {
            var okuma = _endeksOkumaService.GetById((int)id);
            if (okuma == null) return NotFound();
            return View(okuma);
        }

        public IActionResult Yeni()
        {
            ViewBag.TuketimNoktalari = _tuketimNoktasiService.GetAll();
            ViewBag.Sayaclar = _sayacService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(long TuketimNoktasiId, long SayacId, decimal onceki_endeks, decimal yeni_endeks, string okuma_tipi, string okuma_kaynagi)
        {
            // Tüketim miktarını hesapla
            decimal tuketim = yeni_endeks - onceki_endeks;
            if (tuketim < 0) tuketim = 0; // Eğer negatifse (örneğin hatalı okuma veya sayaç sıfırlanması), şimdilik 0 kabul edelim

            // İlgili tüketim noktasına ait sözleşmeyi bul
            var sozlesmeler = _sozlesmeService.GetAll().Where(s => s.tuketim_noktasi_id == TuketimNoktasiId).ToList();
            var aktifSozlesme = sozlesmeler.FirstOrDefault(s => s.statu != "Feshedildi" && s.statu != "İptal") ?? sozlesmeler.FirstOrDefault();
            
            string tarifeGrubu = aktifSozlesme?.tarife_grubu ?? "Mesken";
            
            // Fatura hesaplamasını yap
            var hesaplama = _faturaService.SimulasyonHesapla(tarifeGrubu, tuketim);

            // Yeni faturayı oluştur
            int yeniFaturaId = FaturaController.Faturalar.Count > 0 ? (int)FaturaController.Faturalar.Max(f => f.fatura_id) + 1 : 1;
            
            var yeniFatura = new Fatura
            {
                fatura_id = yeniFaturaId,
                fatura_no = $"FAT-{DateTime.Now.Year}-{(yeniFaturaId).ToString().PadLeft(3, '0')}",
                sozlesme_id = aktifSozlesme?.sozlesme_id ?? 1000,
                tekil_kod = TuketimNoktasiId.ToString(),
                fatura_tipi = tarifeGrubu,
                fatura_tarihi = DateTime.Now,
                son_odeme_tarihi = DateTime.Now.AddDays(15),
                donem = DateTime.Now.ToString("yyyy-MM"),
                ilk_endeks = onceki_endeks,
                son_endeks = yeni_endeks,
                tuketim_kwh = tuketim,
                carpan = 1,
                enerji_bedeli = hesaplama.EnerjiBedeli,
                dagatim_bedeli = hesaplama.DagitimBedeli,
                vergi_fon_toplama = hesaplama.TrtPayi + hesaplama.EnerjiFonu + hesaplama.KdvTutari,
                toplam_tutar = hesaplama.ToplamTutar,
                durum = "Bekliyor",
                status = "Active",
                created_at = DateTime.Now
            };

            FaturaController.Faturalar.Add(yeniFatura);

            // Gerçek projede Endeks Okuma nesnesi de veritabanına kaydedilecektir.
            TempData["OkumaMesaji"] = "Endeks okuması alındı ve otomatik olarak yeni fatura oluşturuldu. (Fatura No: " + yeniFatura.fatura_no + " - Tutar: " + yeniFatura.toplam_tutar?.ToString("C2") + ")";
            return RedirectToAction("Index");
        }
    }
}
