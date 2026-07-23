using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using KcetasWeb.Models.entities;



namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi,Denetci")]
    public class AboneSorgulamaController : Controller
    {
        private readonly IAboneService _aboneService;

        public AboneSorgulamaController(IAboneService aboneService)
        {
            _aboneService = aboneService;
        }

        public IActionResult Index(string q)
        {
            ViewBag.Query = q;
            if (string.IsNullOrEmpty(q))
            {
                return View(null);
            }

            var tumAboneler = _aboneService.GetAll();

            if (q.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                return View(tumAboneler);
            }

            var results = tumAboneler.Where(a =>
            {
                string adSoyad = $"{a.Ad} {a.Soyad}".Trim();
                
                return (a.abone_no != null && a.abone_no.Contains(q, StringComparison.CurrentCultureIgnoreCase)) ||
                       (adSoyad.Contains(q, StringComparison.CurrentCultureIgnoreCase)) ||
                       (a.tckn != null && a.tckn.Contains(q, StringComparison.CurrentCultureIgnoreCase)) ||
                       (a.vkn != null && a.vkn.Contains(q, StringComparison.CurrentCultureIgnoreCase)) ||
                       (a.Unvan != null && a.Unvan.Contains(q, StringComparison.CurrentCultureIgnoreCase));
            }).ToList();

            return View(results);
        }

        public IActionResult Detay(long id)
        {
            var abone = _aboneService.GetById((int)id);
            if (abone == null)
            {
                return NotFound();
            }
            return View(abone);
        }
    }
}