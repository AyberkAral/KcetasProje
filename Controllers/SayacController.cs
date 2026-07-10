using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,SayacYetkilisi,Yonetici")]
    public class SayacController : Controller
    {
        private readonly ISayacService _sayacService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;

        public SayacController(ISayacService sayacService, ITuketimNoktasiService tuketimNoktasiService)
        {
            _sayacService = sayacService;
            _tuketimNoktasiService = tuketimNoktasiService;
        }

        public IActionResult Index()
        {
            ViewBag.TuketimNoktalari = _tuketimNoktasiService.GetAll();
            return View(_sayacService.GetAll());
        }

        public IActionResult Bagla(long id)
        {
            var sayac = _sayacService.GetById(id);
            if (sayac == null) return NotFound();

            ViewBag.TuketimNoktalari = _tuketimNoktasiService.GetAll();
            return View(sayac);
        }

        [HttpPost]
        public IActionResult Bagla(long sayac_id, int tuketim_noktasi_id, long muhur_no, decimal ilk_endeks)
        {
            var sayac = _sayacService.GetById(sayac_id);
            if (sayac != null)
            {
                sayac.tuketim_noktasi_id = tuketim_noktasi_id;
                sayac.durum = tuketim_noktasi_id > 0 ? "Bağlı" : "Depoda";
                sayac.status = sayac.durum;
                sayac.muhur_no = muhur_no;
                
                sayac.UpdatedAt = DateTime.Now;

                _sayacService.Update(sayac);

                TempData["BasariMesaji"] = $"Sayaç başarıyla {(tuketim_noktasi_id > 0 ? "bağlandı" : "boşa alındı")}. Mühür No: {muhur_no}, İlk Endeks: {ilk_endeks}";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Yeni()
        {
            return View(new Sayac { carpan = 1.0m, faz = "Monofaze" });
        }

        [HttpPost]
        public IActionResult Yeni(Sayac model)
        {
            if (ModelState.IsValid)
            {
                var sayaclar = _sayacService.GetAll();
                model.sayac_id = sayaclar.Any() ? sayaclar.Max(s => s.sayac_id) + 1 : 1;
                model.durum = "Depoda";
                model.status = "Depoda";
                model.CreatedAt = DateTime.Now;
                
                _sayacService.Create(model);

                TempData["BasariMesaji"] = "Yeni sayaç başarıyla sisteme eklendi.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Detay(long id)
        {
            var sayac = _sayacService.GetById(id);
            if (sayac == null) return NotFound();
            
            ViewBag.TuketimNoktalari = _tuketimNoktasiService.GetAll();
            return View(sayac);
        }
    }
}
