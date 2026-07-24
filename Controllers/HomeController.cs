using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;

namespace ProjeStaj.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly KcetasWeb.Services.Interfaces.IAboneService _aboneService;
    private readonly KcetasWeb.Services.Interfaces.ISozlesmeService _sozlesmeService;
    private readonly KcetasWeb.Services.Interfaces.IIsEmriService _isEmriService;
    private readonly KcetasWeb.Services.Interfaces.IFaturaService _faturaService;

    public HomeController(
        ILogger<HomeController> logger,
        KcetasWeb.Services.Interfaces.IAboneService aboneService,
        KcetasWeb.Services.Interfaces.ISozlesmeService sozlesmeService,
        KcetasWeb.Services.Interfaces.IIsEmriService isEmriService,
        KcetasWeb.Services.Interfaces.IFaturaService faturaService)
    {
        _logger = logger;
        _aboneService = aboneService;
        _sozlesmeService = sozlesmeService;
        _isEmriService = isEmriService;
        _faturaService = faturaService;
    }

    public IActionResult Index()
    {
        var aboneCount = 0;
        var sozlesmeCount = 0;
        var aktifIsEmriCount = 0;
        var bekleyenFaturaCount = 0;

        try { aboneCount = _aboneService.GetAll().Count(); } catch { }
        try { sozlesmeCount = _sozlesmeService.GetAll().Count(s => s.durum == KcetasWeb.Models.Enums.SozlesmeDurumu.Aktif); } catch { }
        try { 
            var isEmirleri = _isEmriService.GetAll();
            aktifIsEmriCount = isEmirleri.Count(i => i.durum != KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi && i.durum != KcetasWeb.Models.Enums.IsEmriDurumu.Iptal);
        } catch { }
        try { bekleyenFaturaCount = _faturaService.GetAll().Count(f => f.durum == "TASLAK" || f.durum == "HESAPLANDI" || f.durum == "HATALI" || f.durum == "ODENMEDI"); } catch { }

        ViewBag.AboneCount = aboneCount;
        ViewBag.SozlesmeCount = sozlesmeCount;
        ViewBag.AktifIsEmriCount = aktifIsEmriCount;
        ViewBag.BekleyenFaturaCount = bekleyenFaturaCount;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
