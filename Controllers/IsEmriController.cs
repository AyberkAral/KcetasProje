using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;
using System;
using System.Linq;

namespace KcetasWeb.Controllers;

[Authorize(Roles = "BTYoneticisi,SahaOperasyonAmiri,SayacOkumaPersoneli")]
public class IsEmriController : Controller
{
    private readonly IIsEmriService _isEmriService;
    private readonly IKullaniciDeposu _kullaniciDeposu;
    private readonly ITuketimNoktasiService _tuketimNoktasiService;
    private readonly ISozlesmeService _sozlesmeService;
    private readonly ISayacService _sayacService;
    private readonly IAuditLogService _auditLogService;

    public IsEmriController(
        IIsEmriService isEmriService, 
        IKullaniciDeposu kullaniciDeposu,
        ITuketimNoktasiService tuketimNoktasiService,
        ISozlesmeService sozlesmeService,
        ISayacService sayacService,
        IAuditLogService auditLogService)
    {
        _isEmriService = isEmriService;
        _kullaniciDeposu = kullaniciDeposu;
        _tuketimNoktasiService = tuketimNoktasiService;
        _sozlesmeService = sozlesmeService;
        _sayacService = sayacService;
        _auditLogService = auditLogService;
    }

    public IActionResult Index(IsEmriListeViewModel filtre)
    {
        // Önce temel tarih ve durum/arama filtrelerini API üzerinden çekiyoruz.
        // Tip filtresini burada göndermiyoruz çünkü DB'deki değer (BAGLAMA) ile UI'daki (Sayaç Bağlama) farklı olabiliyor.
        var isEmirleri = _isEmriService.Filtrele(null, filtre.FiltreDurum, filtre.BaslangicTarih, filtre.BitisTarih, filtre.AramaMetni);

        filtre.IsEmirleri = isEmirleri.Select(ie => {
            var kullanici = ie.atanan_kullanici_id.HasValue ? _kullaniciDeposu.BulId(ie.atanan_kullanici_id.Value) : null;
            var tn = _tuketimNoktasiService.GetAll().FirstOrDefault(t => t.TuketimNoktasiId == ie.tuketim_noktasi_id);

            return new IsEmriSatirViewModel
            {
                IsEmriId = ie.is_emri_id,
                IsEmriNo = ie.is_emri_no,
                Tip = ie.tip switch
                {
                    "SAYAC_ARIZA" => "Sayaç Arıza",
                    "DEGISTIRME" => "Sayaç Değiştirme",
                    "SAYAC DEGISIMI" => "Sayaç Değiştirme",
                    "KESME" => "Enerji Kesme",
                    "YENI_BAGLANTI" => "Yeni Bağlantı",
                    "BAGLAMA" => "Sayaç Bağlama",
                    "ENDEKS_OKUMA" => "Endeks Okuma",
                    _ => string.IsNullOrEmpty(ie.tip) ? "Belirtilmedi" : ie.tip
                },
                TuketimNoktasiId = ie.tuketim_noktasi_id,
                tekil_kod = tn != null ? tn.tekil_kod : $"TK-ID-{ie.tuketim_noktasi_id}",
                TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{ie.tuketim_noktasi_id}",
                musteri_ad = tn?.musteri_ad,
                musteri_soyad = tn?.musteri_soyad,
                musteri_unvan = tn?.musteri_unvan,
                PlanlananTarih = ie.planlanan_tarih ?? ie.CreatedAt.AddDays(1),
                olusturulma_tarihi = ie.CreatedAt,
                oncelik = ie.oncelik,
                AtananKullaniciAdi = kullanici != null ? kullanici.ad_soyad : "Atanmadı",
                Durum = ie.durum,
                DurumRenk = IsEmriListeViewModel.GetDurumRenk(ie.durum),
                Adres = tn != null ? tn.acik_adres : "Adres bilgisi alınamadı"
            };
        }).ToList();

        // Şimdi UI uyumlu hale gelmiş nesneler üzerinde detaylı string filtrelerini uyguluyoruz
        if (!string.IsNullOrEmpty(filtre.FiltreTip))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.Tip != null && x.Tip.Equals(filtre.FiltreTip, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreIsEmriNo))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.IsEmriNo != null && x.IsEmriNo.Contains(filtre.FiltreIsEmriNo, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreTekilKod))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.tekil_kod != null && x.tekil_kod.Contains(filtre.FiltreTekilKod, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreAboneAdi))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.musteriDurum != null && x.musteriDurum.Contains(filtre.FiltreAboneAdi, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltrePersonel))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.AtananKullaniciAdi != null && x.AtananKullaniciAdi.Contains(filtre.FiltrePersonel, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreOncelik))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.oncelik != null && x.oncelik.Equals(filtre.FiltreOncelik, StringComparison.OrdinalIgnoreCase)).ToList();

        return View(filtre);
    }

    public IActionResult Yeni()
    {
        ViewBag.TuketimNoktalari = _tuketimNoktasiService.GetAll()
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.TuketimNoktasiId.ToString(), Text = $"{x.tekil_kod} - {x.musteri_ad} {x.musteri_soyad} {x.musteri_unvan}" })
            .ToList();
            
        ViewBag.Personeller = _kullaniciDeposu.Listele()
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.kullanici_id.ToString(), Text = x.ad_soyad })
            .ToList();
            
        ViewBag.Sozlesmeler = _sozlesmeService.GetAll()
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.sozlesme_id.ToString(), Text = $"{x.sozlesme_no} - {x.ad} {x.sozlesme_tipi}" })
            .ToList();

        ViewBag.Sayaclar = _sayacService.GetAll()
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.sayac_id.ToString(), Text = $"{x.seri_no} - {x.marka} {x.model}" })
            .ToList();

        var sayacMap = _sayacService.GetAll()
            .Where(s => s.tuketim_noktasi_id != null && s.tuketim_noktasi_id > 0)
            .GroupBy(s => s.tuketim_noktasi_id.Value.ToString())
            .ToDictionary(g => g.Key, g => g.First().sayac_id);

        var sozlesmeMap = _sozlesmeService.GetAll()
            .Where(s => s.tuketim_noktasi_id > 0)
            .GroupBy(s => s.tuketim_noktasi_id.ToString())
            .ToDictionary(g => g.Key, g => g.First().sozlesme_id);

        ViewBag.SayacMapJson = System.Text.Json.JsonSerializer.Serialize(sayacMap);
        ViewBag.SozlesmeMapJson = System.Text.Json.JsonSerializer.Serialize(sozlesmeMap);
            
        return View(new YeniIsEmriViewModel());
    }

    [HttpPost]
    public IActionResult Yeni(YeniIsEmriViewModel model)
    {
        if (ModelState.IsValid)
        {
            var isEmri = new IsEmri
            {
                tuketim_noktasi_id = model.TuketimNoktasiId,
                tip = model.Tip,
                oncelik = model.Oncelik,
                planlanan_tarih = model.PlanlananTarih,
                atanan_kullanici_id = model.AtananKullaniciId,
                gerekce = model.Aciklama,
                sayac_id = model.SayacId
            };
            
            _isEmriService.Ekle(isEmri);
            
            TempData["Mesaj"] = "İş emri başarıyla oluşturuldu.";
            TempData["MesajTip"] = "success";
            return RedirectToAction("Index");
        }

        ViewBag.TuketimNoktalari = _tuketimNoktasiService.GetAll()
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.TuketimNoktasiId.ToString(), Text = $"{x.tekil_kod} - {x.musteri_ad} {x.musteri_soyad} {x.musteri_unvan}" })
            .ToList();
            
        ViewBag.Personeller = _kullaniciDeposu.Listele()
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.kullanici_id.ToString(), Text = x.ad_soyad })
            .ToList();

        ViewBag.Sozlesmeler = _sozlesmeService.GetAll()
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.sozlesme_id.ToString(), Text = $"{x.sozlesme_no} - {x.ad} {x.sozlesme_tipi}" })
            .ToList();

        ViewBag.Sayaclar = _sayacService.GetAll()
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.sayac_id.ToString(), Text = $"{x.seri_no} - {x.marka} {x.model}" })
            .ToList();

        var sayacMap = _sayacService.GetAll()
            .Where(s => s.tuketim_noktasi_id != null && s.tuketim_noktasi_id > 0)
            .GroupBy(s => s.tuketim_noktasi_id.Value.ToString())
            .ToDictionary(g => g.Key, g => g.First().sayac_id);

        var sozlesmeMap = _sozlesmeService.GetAll()
            .Where(s => s.tuketim_noktasi_id > 0)
            .GroupBy(s => s.tuketim_noktasi_id.ToString())
            .ToDictionary(g => g.Key, g => g.First().sozlesme_id);

        ViewBag.SayacMapJson = System.Text.Json.JsonSerializer.Serialize(sayacMap);
        ViewBag.SozlesmeMapJson = System.Text.Json.JsonSerializer.Serialize(sozlesmeMap);

        return View(model);
    }

        public IActionResult Detay(long id)
        {
            var isEmri = _isEmriService.GetById(id);
            if (isEmri == null)
                return NotFound();

            var tn = _tuketimNoktasiService.GetAll().FirstOrDefault(t => t.TuketimNoktasiId == isEmri.tuketim_noktasi_id);

            var viewModel = new IsEmriDetayViewModel
            {
                IsEmriId = isEmri.is_emri_id,
                IsEmriNo = isEmri.is_emri_no,
                Tip = isEmri.tip,
                Durum = isEmri.durum,
                DurumRenk = IsEmriListeViewModel.GetDurumRenk(isEmri.durum),
                Oncelik = isEmri.oncelik,
                PlanlananTarih = isEmri.planlanan_tarih,
                AtananKullaniciAdi = isEmri.atanan_kullanici_id.HasValue 
                    ? (_kullaniciDeposu.BulId(isEmri.atanan_kullanici_id.Value)?.ad_soyad ?? "Atanmadı") 
                    : "Atanmadı",
                musteri_ad = tn?.musteri_ad,
                musteri_soyad = tn?.musteri_soyad,
                musteri_unvan = tn?.musteri_unvan,
                telefon = tn?.telefon,
                TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{isEmri.tuketim_noktasi_id}",
                Adres = tn != null ? tn.acik_adres : "Adres bilgisi alınamadı",
                SayacSeriNo = "Sayaç bilgisi yok",
                SahaSonucu = isEmri.saha_sonucu,
                Gerekce = isEmri.gerekce,
                MuhurNo = isEmri.muhur_no,
                TutanakNo = isEmri.tutanak_no,
                CreatedAt = isEmri.CreatedAt,
                UpdatedAt = isEmri.UpdatedAt
            };

            ViewBag.AuditLogs = _auditLogService.GetirByVarlik("IsEmri", id);

            return View(viewModel);
        }

    [HttpPost]
    public IActionResult DurumGuncelle(long id, string yeniDurum)
    {
        var isEmri = _isEmriService.GetById(id);
        if (isEmri == null) return NotFound();

        string eskiDurum = isEmri.durum;
        
        // Validation for status change
        if (yeniDurum == "Tamamlandı" && string.IsNullOrEmpty(isEmri.saha_sonucu))
        {
            TempData["Mesaj"] = "Saha sonucu veya tutanak girilmeden iş emri 'Tamamlandı' statüsüne alınamaz! Lütfen 'Tamamla' butonunu kullanın.";
            TempData["MesajTip"] = "danger";
            return RedirectToAction("Detay", new { id = id });
        }

        _isEmriService.DurumGuncelle(id, yeniDurum);
        
        // Currently there's no real user authentication to get User ID. Using 1 as a mock ID.
        int mockUserId = 1; 

        _auditLogService.Ekle(
            varlikTipi: "IsEmri",
            varlikId: id,
            islemTipi: "Durum Degisikligi",
            eskiDeger: eskiDurum,
            yeniDeger: yeniDurum,
            kullaniciId: mockUserId
        );

        TempData["Mesaj"] = $"İş emri durumu '{yeniDurum}' olarak güncellendi.";
        TempData["MesajTip"] = "success";
        return RedirectToAction("Detay", new { id = id });
    }

    public IActionResult TutanakGiris(long id)
    {
        var isEmri = _isEmriService.GetById(id);
        if (isEmri == null)
            return NotFound();

        var viewModel = new TutanakGirisViewModel
        {
            IsEmriId = isEmri.is_emri_id,
            IsEmriNo = isEmri.is_emri_no,
            Tip = isEmri.tip,
            IslemTarihi = DateTime.Now
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TutanakKaydet(TutanakGirisViewModel model)
    {
        // İş Kuralları Validasyonu
        if (model.Tip == "Kesme")
        {
            if (!model.KesmeEndeksi.HasValue) ModelState.AddModelError("KesmeEndeksi", "Kesme işlemi için Kesme Endeksi zorunludur.");
            if (string.IsNullOrWhiteSpace(model.SahaSonucu)) ModelState.AddModelError("SahaSonucu", "İşlem sonucu alanı zorunludur.");
        }
        else if (model.Tip == "Sayaç Değiştirme" || model.Tip == "SayacDegisim")
        {
            if (!model.EskiSonEndeksi.HasValue) ModelState.AddModelError("EskiSonEndeksi", "Sayaç değişimi için Eski Son Endeks zorunludur.");
            if (!model.YeniIlkEndeksi.HasValue) ModelState.AddModelError("YeniIlkEndeksi", "Sayaç değişimi için Yeni İlk Endeks zorunludur.");
        }
        else if (model.Tip == "Sayaç Bağlama" || model.Tip == "SayacTakma")
        {
            if (!model.YeniIlkEndeksi.HasValue) ModelState.AddModelError("YeniIlkEndeksi", "Sayaç bağlama işlemi için İlk Endeks zorunludur.");
            if (string.IsNullOrWhiteSpace(model.MuhurNo)) ModelState.AddModelError("MuhurNo", "Sayaç bağlama işlemi için Mühür No zorunludur.");
        }

        if (string.IsNullOrWhiteSpace(model.SahaSonucu))
        {
             ModelState.AddModelError("SahaSonucu", "İş emrini tamamlayabilmek için Saha Sonucu girilmesi zorunludur.");
        }

        if (!ModelState.IsValid)
        {
            // Eğer hata varsa Tamamlama sayfasına geri dön.
            // Fakat model tipi TutanakGirisViewModel, Tamamlama ise IsEmriDetayViewModel bekliyor.
            // O yüzden şimdilik geri dönmeden hata mesajını TempData'ya atıp yönlendireceğiz veya TutanakGiris sayfasına döndüreceğiz.
            TempData["Mesaj"] = "Lütfen formdaki zorunlu alanları (Endeks, Sonuç vb.) doldurunuz.";
            TempData["MesajTip"] = "danger";
            
            // Kullanıcıyı Tamamlama formuna geri yönlendiriyoruz ki düzeltip tekrar denesin.
            return RedirectToAction("Tamamlama", new { id = model.IsEmriId });
        }

        _isEmriService.TutanakKaydet(
            model.IsEmriId,
            model.TutanakNo,
            model.SahaSonucu,
            model.Gerekce,
            model.MuhurNo,
            model.KesmeEndeksi,
            model.AcmaEndeksi,
            model.EskiSayacNo,
            model.YeniSayacNo,
            model.EskiSonEndeksi,
            model.YeniIlkEndeksi
        );
        
        string mesaj = "Tutanak başarıyla kaydedildi.";
        
        // Simülasyon: Kesme veya Açma yapıldıysa
        if (model.Tip == "Kesme" && model.KesmeEndeksi.HasValue)
        {
            mesaj = $"Tutanak başarıyla kaydedildi. (Kesme Endeksi: {model.KesmeEndeksi}). Borç veya sözleşme nedeniyle yapılan kesme için, bir sonraki faturaya yansıtılacak olan 'Kesme-Bağlama Bedeli' (Örn: 118,50 TL) simülasyonu başlatılmıştır.";
        }
        else if (model.Tip == "Açma" && model.AcmaEndeksi.HasValue)
        {
            mesaj = $"Tutanak başarıyla kaydedildi. (Açma Endeksi: {model.AcmaEndeksi}). Ödeme/Sözleşme onayı sonrası açma işlemi tamamlandı.";
        }

        TempData["Mesaj"] = mesaj;
        TempData["MesajTip"] = "success";

        // İş emri durumu 'Tamamlandı' olduysa Audit Log atalım
        _auditLogService.Ekle(
            varlikTipi: "IsEmri",
            varlikId: model.IsEmriId,
            islemTipi: "Tamamlama / Durum Degisikligi",
            eskiDeger: "Herhangi (Tamamlanmadan Önce)",
            yeniDeger: "Tamamlandı",
            kullaniciId: 1
        );

        return RedirectToAction("Detay", new { id = model.IsEmriId });
    }

    public IActionResult TutanakGoruntule(long id)
    {
        var isEmri = _isEmriService.GetById(id);
        if (isEmri == null || string.IsNullOrEmpty(isEmri.tutanak_no))
            return NotFound();

        var tn = _tuketimNoktasiService.GetAll().FirstOrDefault(t => t.TuketimNoktasiId == isEmri.tuketim_noktasi_id);

        var viewModel = new IsEmriDetayViewModel
        {
            IsEmriId = isEmri.is_emri_id,
            IsEmriNo = isEmri.is_emri_no,
            Tip = isEmri.tip,
            Durum = isEmri.durum,
            DurumRenk = IsEmriListeViewModel.GetDurumRenk(isEmri.durum),
            Oncelik = isEmri.oncelik,
            PlanlananTarih = isEmri.planlanan_tarih,
            SahaSonucu = isEmri.saha_sonucu,
            Gerekce = isEmri.gerekce,
            MuhurNo = isEmri.muhur_no,
            TutanakNo = isEmri.tutanak_no,
            TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{isEmri.tuketim_noktasi_id}",
            Adres = tn != null ? tn.acik_adres : "Adres bilgisi alınamadı",
            UpdatedAt = isEmri.UpdatedAt,
            EskiSayacNo = isEmri.eski_sayac_no,
            YeniSayacNo = isEmri.yeni_sayac_no,
            EskiSonEndeksi = isEmri.eski_son_endeksi,
            YeniIlkEndeksi = isEmri.yeni_ilk_endeksi,
            KesmeEndeksi = isEmri.kesme_endeksi,
            AcmaEndeksi = isEmri.acma_endeksi
        };

        return View(viewModel);
    }

    public IActionResult Tamamlama(long id)
    {
        var isEmri = _isEmriService.GetById(id);
        if (isEmri == null)
            return NotFound();

        var tn = _tuketimNoktasiService.GetAll().FirstOrDefault(t => t.TuketimNoktasiId == isEmri.tuketim_noktasi_id);

        var viewModel = new IsEmriDetayViewModel
        {
            IsEmriId = isEmri.is_emri_id,
            IsEmriNo = isEmri.is_emri_no,
            Tip = isEmri.tip,
            Durum = isEmri.durum,
            DurumRenk = IsEmriListeViewModel.GetDurumRenk(isEmri.durum),
            Oncelik = isEmri.oncelik,
            PlanlananTarih = isEmri.planlanan_tarih,
            SahaSonucu = isEmri.saha_sonucu,
            Gerekce = isEmri.gerekce,
            MuhurNo = isEmri.muhur_no,
            TutanakNo = isEmri.tutanak_no,
            TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{isEmri.tuketim_noktasi_id}",
            Adres = tn != null ? tn.acik_adres : "Adres bilgisi alınamadı",
            UpdatedAt = isEmri.UpdatedAt,
            EskiSayacNo = isEmri.eski_sayac_no,
            YeniSayacNo = isEmri.yeni_sayac_no,
            EskiSonEndeksi = isEmri.eski_son_endeksi,
            YeniIlkEndeksi = isEmri.yeni_ilk_endeksi,
            KesmeEndeksi = isEmri.kesme_endeksi,
            AcmaEndeksi = isEmri.acma_endeksi,
            son_endeks = isEmri.eski_son_endeksi ?? 0m,
            sökme_nedeni = isEmri.gerekce ?? string.Empty,
            aciklama = isEmri.saha_sonucu ?? string.Empty,
            CreatedAt = isEmri.CreatedAt
        };

        return View("Tamamlama", viewModel);
    }
}
