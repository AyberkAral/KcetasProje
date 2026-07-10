using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace KcetasWeb.Controllers
{
    public class EndeksOkumaController : Controller
    {
        private readonly Services.Interfaces.IEndeksOkumaService _endeksOkumaService;
        public EndeksOkumaController(Services.Interfaces.IEndeksOkumaService endeksOkumaService)
        {
            _endeksOkumaService = endeksOkumaService;
        }

        public IActionResult Index(string? kaynak, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var okumalar = _endeksOkumaService.Filtrele(kaynak, durum, baslangic, bitis, arama);
            
            var viewModels = okumalar.Select(o => {
                var sozlesme = KcetasWeb.Controllers.SozlesmeController._sozlesmeler.FirstOrDefault(s => s.sozlesme_id == o.sozlesme_id);
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
            ViewBag.TuketimNoktalari = KcetasWeb.Controllers.TuketimNoktasiController._tuketimNoktalari;
            ViewBag.Sayaclar = KcetasWeb.Controllers.SayacController._sayaclar;
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(long TuketimNoktasiId, long SayacId, decimal onceki_endeks, decimal yeni_endeks, string okuma_tipi, string okuma_kaynagi)
        {
            // Gerçek projede veritabanına ekleme yapılır, mock'ta listeye eklenir. 
            // Burada basit tutuyoruz.
            TempData["OkumaMesaji"] = "Seçilen Tüketim Noktası (" + TuketimNoktasiId + ") ve Sayaç (" + SayacId + ") için endeks okuma işlemi başarıyla kaydedildi.";
            return RedirectToAction("Index");
        }
    }
}
