using Microsoft.AspNetCore.Mvc;


using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;

using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using System;
using System.Linq;

namespace KcetasWeb.Controllers;

[Authorize(Roles = AppRoles.BTYoneticisi)]
public class OutboxController : Controller
{
    private readonly IOutboxService _outboxService;

    public OutboxController(IOutboxService outboxService)
    {
        _outboxService = outboxService;
    }

    public IActionResult Index(string? durum, string? hedefSistem,
        DateTime? baslangicTarih, DateTime? bitisTarih)
    {
        var kayitlar = _outboxService.Filtrele(durum, hedefSistem, baslangicTarih, bitisTarih);
        var istatistikler = _outboxService.GetIstatistikler();

        var viewModel = new OutboxListeViewModel
        {
            FiltreDurum = durum,
            FiltreHedefSistem = hedefSistem,
            BaslangicTarih = baslangicTarih,
            BitisTarih = bitisTarih,
            ToplamKayit = istatistikler.Toplam,
            BekleyenSayisi = istatistikler.Bekleyen,
            GonderilmisSayisi = istatistikler.Gonderilmis,
            BasarisizSayisi = istatistikler.Basarisiz,
            Kayitlar = kayitlar.Select(k => new OutboxListeViewModel.OutboxSatirViewModel
            {
                OutboxId = k.outbox_id,
                FaturaId = k.fatura_id,
                FaturaNo = k.fatura?.fatura_no,
                ReferansNo = k.corrolation_id ?? k.fatura?.fatura_no ?? k.idempotency_key,
                HedefSistem = k.hedef_sistem,
                IdempotencyKey = k.idempotency_key,
                Durum = k.durum,
                DurumEtiketi = OutboxListeViewModel.GetOutboxDurumEtiketi(k.durum),
                DurumRenk = OutboxListeViewModel.GetOutboxDurumRenk(k.durum),
                DenemeSayisi = k.retry_count,
                HataKodu = k.hata_kodu,
                SonHataMesaji = k.hata_mesaji,
                OlusturulmaZamani = k.created_at,
                SonDenemeTarihi = k.son_deneme_tarihi,
                GonderimZamani = k.gonderim_zamani,
                PayloadOnizleme = k.paload != null && k.paload.Length > 120 ? k.paload[..120] + "..." : (k.paload ?? "")
            }).ToList()
        };

        return View(viewModel);
    }

    public IActionResult Detay(long id)
    {
        var kayit = _outboxService.GetById(id);
        if (kayit == null)
            return NotFound();

        return Json(new
        {
            outboxId = kayit.outbox_id,
            faturaId = kayit.fatura_id,
            faturaNo = kayit.fatura?.fatura_no,
            referansNo = kayit.corrolation_id ?? kayit.fatura?.fatura_no ?? kayit.idempotency_key,
            hedefSistem = kayit.hedef_sistem,
            durum = kayit.durum,
            durumEtiketi = OutboxListeViewModel.GetOutboxDurumEtiketi(kayit.durum),
            idempotencyKey = kayit.idempotency_key,
            correlationId = kayit.corrolation_id,
            payload = kayit.paload,
            denemeSayisi = kayit.retry_count,
            hataKodu = kayit.hata_kodu,
            sonHataMesaji = kayit.hata_mesaji,
            olusturulmaZamani = kayit.created_at.ToString("dd.MM.yyyy HH:mm"),
            sonDenemeTarihi = kayit.son_deneme_tarihi?.ToString("dd.MM.yyyy HH:mm") ?? "-",
            gonderimZamani = kayit.gonderim_zamani?.ToString("dd.MM.yyyy HH:mm") ?? "-"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult YenidenGonder(long id)
    {
        var sonuc = _outboxService.YenidenGonder(id);

        if (sonuc)
        {
            TempData["Mesaj"] = "Kayıt yeniden gönderim kuyruğuna eklendi.";
            TempData["MesajTip"] = "success";
        }
        else
        {
            TempData["Mesaj"] = "Yeniden gönderim başarısız oldu.";
            TempData["MesajTip"] = "danger";
        }

        return RedirectToAction("Index");
    }
}
