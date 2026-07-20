using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = $"{AppRoles.FaturalamaUzmani},{AppRoles.SozlesmeYetkilisi},{AppRoles.Denetci},{AppRoles.BTYoneticisi},Yonetici")]
    public class BelgelerController : Controller
    {
        private readonly IFaturaService _faturaService;
        private readonly IIsEmriService _isEmriService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly ISozlesmeService _sozlesmeService;
        private readonly IAboneService _aboneService;

        public BelgelerController(IFaturaService faturaService, IIsEmriService isEmriService, ITuketimNoktasiService tuketimNoktasiService, ISozlesmeService sozlesmeService, IAboneService aboneService)
        {
            _faturaService = faturaService;
            _isEmriService = isEmriService;
            _tuketimNoktasiService = tuketimNoktasiService;
            _sozlesmeService = sozlesmeService;
            _aboneService = aboneService;
        }

        public IActionResult Index()
        {
            var viewModel = new BelgelerListeViewModel();
            var tnMap = _tuketimNoktasiService.GetAll().ToDictionary(t => t.tuketim_noktasi_id);

            // 1. Faturaları Çek
            var faturalar = _faturaService.GetAll();
            foreach (var f in faturalar)
            {
                viewModel.Belgeler.Add(new BelgeSatirViewModel
                {
                    BelgeTipi = "Fatura",
                    BelgeNo = f.fatura_no,
                    TuketimNoktasiKodu = f.tekil_kod,
                    Tarih = f.fatura_tarihi ?? f.created_at ?? DateTime.Now,
                    Tutar = f.toplam_tutar,
                    Aciklama = "Dönem Faturası",
                    Url = $"/Belgeler/Goruntule?tip=Fatura&id={f.fatura_id}"
                });
            }

            // 2. Tutanakları Çek (İş Emri tablosunda tutanak_no dolu olanlar)
            var isEmirleri = _isEmriService.GetAll().Where(ie => !string.IsNullOrEmpty(ie.tutanak_no));
            foreach (var ie in isEmirleri)
            {
                var tn = tnMap.ContainsKey(ie.tuketim_noktasi_id) ? tnMap[ie.tuketim_noktasi_id] : null;
                viewModel.Belgeler.Add(new BelgeSatirViewModel
                {
                    BelgeTipi = "Tutanak",
                    BelgeNo = ie.tutanak_no,
                    TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{ie.tuketim_noktasi_id}",
                    Tarih = ie.updated_at ?? ie.created_at,
                    Tutar = null,
                    Aciklama = ie.tip switch
                    {
                        "BAGLAMA" => "Sayaç Bağlama Tutanağı",
                        "SOKME" => "Sayaç Sökme Tutanağı",
                        "DEGISTIRME" => "Sayaç Değişim Tutanağı",
                        "KESME" => "Enerji Kesme Tutanağı",
                        "ACMA" => "Enerji Açma Tutanağı",
                        _ => "Saha Operasyon Tutanağı"
                    },
                    Url = $"/Belgeler/Goruntule?tip=Tutanak&id={ie.is_emri_id}"
                });
            }

            // 3. Sözleşmeleri Çek
            var sozlesmeler = _sozlesmeService.GetAll();
            foreach (var s in sozlesmeler)
            {
                var tn = tnMap.ContainsKey(s.tuketim_noktasi_id) ? tnMap[s.tuketim_noktasi_id] : null;
                viewModel.Belgeler.Add(new BelgeSatirViewModel
                {
                    BelgeTipi = "Sözleşme",
                    BelgeNo = s.sozlesme_no,
                    TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{s.tuketim_noktasi_id}",
                    Tarih = s.created_at,
                    Tutar = null,
                    Aciklama = $"{s.sozlesme_tipi} Sözleşmesi",
                    Url = $"/Belgeler/Goruntule?tip=Sozlesme&id={s.sozlesme_id}"
                });
            }

            // Sıralama (En yeni en üstte)
            viewModel.Belgeler = viewModel.Belgeler.OrderByDescending(b => b.Tarih).ToList();

            return View(viewModel);
        }

        public IActionResult Goruntule(string tip, int id)
        {
            var viewModel = new BelgeGoruntuleViewModel();
            viewModel.BelgeTipi = tip;

            string GetAboneAdi(int? aboneId)
            {
                if (!aboneId.HasValue || aboneId.Value <= 0) return "Abone Bilgisi Yok";
                var abone = _aboneService.GetById(aboneId.Value);
                if (abone == null) return "Bilinmeyen Abone";
                return (abone.abone_tipi == "Gerçek" || abone.abone_tipi == "BIREYSEL") 
                    ? $"{abone.Ad} {abone.Soyad} (TC: {abone.tckn})" 
                    : $"{abone.Unvan} (VKN: {abone.vkn})";
            }

            if (tip == "Fatura")
            {
                var fatura = _faturaService.GetById(id);
                if (fatura == null) return NotFound();

                viewModel.BelgeNo = fatura.fatura_no;
                viewModel.Tarih = fatura.fatura_tarihi ?? fatura.created_at ?? DateTime.Now;
                viewModel.TuketimNoktasiKod = fatura.tekil_kod;
                viewModel.FaturaTutar = fatura.toplam_tutar;
                viewModel.FaturaTuketim = fatura.tuketim_kwh;
                viewModel.FaturaDurum = fatura.durum;

                var sozlesme = _sozlesmeService.GetAll().FirstOrDefault(s => s.sozlesme_id == fatura.sozlesme_id);
                if (sozlesme != null)
                {
                    viewModel.AboneBilgisi = GetAboneAdi(sozlesme.abone_id);
                }
            }
            else if (tip == "Sozlesme")
            {
                var sozlesme = _sozlesmeService.GetAll().FirstOrDefault(s => s.sozlesme_id == id);
                if (sozlesme == null) return NotFound();

                viewModel.BelgeNo = sozlesme.sozlesme_no;
                viewModel.Tarih = sozlesme.created_at;
                viewModel.SozlesmeTipi = sozlesme.sozlesme_tipi;
                viewModel.SozlesmeDurum = sozlesme.durum;
                viewModel.GuvenceBedeli = sozlesme.guvence_bedeli;
                viewModel.AboneBilgisi = GetAboneAdi(sozlesme.abone_id);

                var tn = _tuketimNoktasiService.GetAll().FirstOrDefault(t => t.tuketim_noktasi_id == sozlesme.tuketim_noktasi_id);
                viewModel.TuketimNoktasiKod = tn?.tekil_kod ?? $"TK-ID-{sozlesme.tuketim_noktasi_id}";
            }
            else if (tip == "Tutanak")
            {
                var isEmri = _isEmriService.GetById(id);
                if (isEmri == null || string.IsNullOrEmpty(isEmri.tutanak_no)) return NotFound();

                viewModel.BelgeNo = isEmri.tutanak_no;
                viewModel.Tarih = isEmri.updated_at ?? isEmri.created_at;
                viewModel.TutanakIslemTipi = isEmri.tip;
                viewModel.TutanakSahaSonucu = isEmri.saha_sonucu;
                viewModel.TutanakGerekce = isEmri.gerekce;
                viewModel.TutanakDurum = isEmri.durum;
                
                var tn = _tuketimNoktasiService.GetAll().FirstOrDefault(t => t.tuketim_noktasi_id == isEmri.tuketim_noktasi_id);
                if (tn != null)
                {
                    viewModel.TuketimNoktasiKod = tn.tekil_kod;
                    var aboneSozlesme = _sozlesmeService.GetAll().FirstOrDefault(s => s.tuketim_noktasi_id == tn.tuketim_noktasi_id && s.durum == "AKTIF");
                    if (aboneSozlesme != null)
                    {
                        viewModel.AboneBilgisi = GetAboneAdi(aboneSozlesme.abone_id);
                    }
                }
            }
            else
            {
                return BadRequest();
            }

            return View(viewModel);
        }
    }
}
