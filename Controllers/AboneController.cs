// using Microsoft.AspNetCore.Mvc;
// using KcetasWeb.Models;
// using KcetasWeb.Services.Interfaces;
// using Microsoft.AspNetCore.Authorization;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using KcetasWeb.Models.entities;
// using KcetasWeb;
// using KcetasWeb.ViewModels;  // Loglama yaparken doğrudan kullanmak için

// namespace KcetasWeb.Controllers
// {
//     [Authorize(Roles = AppRoles.FaturalamaUzmani + "," + AppRoles.BTYoneticisi)]
//     public class AboneController : Controller
//     {
//         private readonly ILogger<AboneController> _logger;

//         public AboneController(ILogger<AboneController> logger)
//         {
//             _logger = logger;
//         }
//         public IActionResult Index()
//         {
//             // Veritabanından orijinal verilerin geldiğini hayal edelim
//             var personelDb = new 
//             {
//                 Ad = "Caner Kısa",
//                 Tc = "98765432101",
//                 Email = "canerkisa@firma.com"
//             };

//             // ViewModel'e eşleme yapıyoruz
//             var viewModel = new AboneDetayViewModel
//             {
//                 AdSoyadUnvan = personelDb.Ad,
//                 OrijinalTcNo = personelDb.Tc,
//                 EPosta = personelDb.Email
//             };

//             // GÜVENLİ LOGLAMA: Log dosyasına TC No yazarken sansürlü yazıyoruz
//             _logger.LogInformation("Personel görüntülendi: {Tc}", personelDb.Tc.Maskele());

//             return View(viewModel);
//         }
//     }

// }