using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi, SayacOkumaPersoneli,SahaOperasyonAmir,FaturalamaUzmani,Denetci")]
    public class SayacController : Controller
    {
        private readonly ISayacService _sayacService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly IIsEmriService _isEmriService;
        private readonly IEndeksOkumaService _endeksOkumaService;
        

      public SayacController(
    ISayacService sayacService,
    ITuketimNoktasiService tuketimNoktasiService,
    IIsEmriService isEmriService,
    IEndeksOkumaService endeksOkumaService)
{
    _sayacService = sayacService;
    _tuketimNoktasiService = tuketimNoktasiService;
    _isEmriService = isEmriService;
    _endeksOkumaService = endeksOkumaService;

}
        public IActionResult Index()
        {
            ViewBag.TuketimNoktalari = _tuketimNoktasiService.GetAll();
            return View(_sayacService.GetAll());
        }

        public IActionResult Bagla(long id)
        {
            var sayac = _sayacService.GetById(id);
            if (sayac == null)
                return NotFound();

            ViewBag.TuketimNoktalari = _tuketimNoktasiService.GetAll();

            return View(sayac);
        }

        [HttpPost]
        public IActionResult Bagla(long sayac_id, int tuketim_noktasi_id, string muhur_no, decimal ilk_endeks)
        {
            if (!string.IsNullOrEmpty(muhur_no) && !muhur_no.StartsWith("MHR-"))
            {
                muhur_no = "MHR-" + muhur_no;
            }

            var sayaclar = _sayacService.GetAll();
            if (!string.IsNullOrEmpty(muhur_no) && sayaclar.Any(s => s.muhur_no == muhur_no && s.sayac_id != sayac_id))
            {
                TempData["HataMesaji"] = "HATA: Bu mühür numarası sistemde başka bir sayaçta kayıtlı! Lütfen farklı bir mühür numarası girin.";
                return RedirectToAction("Bagla", new { id = sayac_id });
            }

            var sayac = sayaclar.FirstOrDefault(x => x.sayac_id == sayac_id);

            if (sayac != null)
            {
                sayac.tuketim_noktasi_id = tuketim_noktasi_id;
                sayac.durum = tuketim_noktasi_id > 0 ? "Bağlı" : "Depoda";
                sayac.status = sayac.durum;
                sayac.muhur_no = muhur_no;

                _sayacService.Update(sayac);

                TempData["BasariMesaji"] =
                    $"Sayaç başarıyla {(tuketim_noktasi_id > 0 ? "bağlandı" : "boşa alındı")}. " +
                    $"Mühür No: {muhur_no}, İlk Endeks: {ilk_endeks}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Yeni()
        {
            return View(new Sayac
            {
                carpan = 1.0m,
                faz = "Monofaze"
            });
        }

        [HttpPost]
        public IActionResult Yeni(Sayac model)
        {
            if (!string.IsNullOrEmpty(model.muhur_no) && !model.muhur_no.StartsWith("MHR-"))
            {
                model.muhur_no = "MHR-" + model.muhur_no;
            }

            var sayaclar = _sayacService.GetAll();

            if (!string.IsNullOrEmpty(model.seri_no) && sayaclar.Any(s => s.seri_no == model.seri_no))
            {
                ModelState.AddModelError("seri_no", "HATA: Bu Sayaç Seri Numarası sistemde zaten mevcut! Lütfen farklı bir seri numarası girin.");
            }

            if (ModelState.IsValid)
            {

                model.sayac_id = sayaclar.Any()
                    ? sayaclar.Max(x => x.sayac_id) + 1
                    : 1;

                model.durum = "Depoda";
                model.status = "Depoda";
                model.created_at = DateTime.Now;

                _sayacService.Create(model);
                var endeks = new EndeksOkuma
{
    sayac_id = (int)model.sayac_id,
    yeni_endeks = 0,
    onceki_endeks = 0,
    okuma_tipi = "ILK_ENDEKS",
    okuma_kaynagi = "MANUEL",
    okuma_zamani = DateTime.Now,
    kullanici_id = 1,
    status = "AKTIF"
};

_endeksOkumaService.Create(endeks);

                TempData["BasariMesaji"] = "Yeni sayaç başarıyla sisteme eklendi.";

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Detay(long id)
        {
            var sayac = _sayacService.GetById(id);

            if (sayac == null)
                return NotFound();

            ViewBag.TuketimNoktalari = _tuketimNoktasiService.GetAll();

            ViewBag.IsEmirleri = _isEmriService
                .GetAll()
                .Where(x => x.sayac_id == sayac.sayac_id)
                .ToList();
ViewBag.Endeksler = _endeksOkumaService
    .GetAll()
    .Where(x => x.sayac_id == sayac.sayac_id)
    .OrderByDescending(x => x.okuma_zamani)
    .ToList();
            return View(sayac);
        }
    }
}