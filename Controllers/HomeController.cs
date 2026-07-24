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

    public async System.Threading.Tasks.Task<IActionResult> Index()
    {
        var aboneCount = 0;
        var sozlesmeCount = 0;
        var aktifIsEmriCount = 0;
        var bekleyenFaturaCount = 0;

        try { 
            var aboneResponse = await _aboneService.GetPagedAsync(1, 1); 
            aboneCount = aboneResponse.TotalCount; 
        } catch { }

        try { 
            var sozlesmeResponse = await _sozlesmeService.GetPagedAsync(1, 1); 
            sozlesmeCount = sozlesmeResponse.TotalCount; 
        } catch { }

        try { 
            var isEmirleri = await _isEmriService.GetAllAsync();
            aktifIsEmriCount = isEmirleri.Count(i => i.durum != KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi && i.durum != KcetasWeb.Models.Enums.IsEmriDurumu.Iptal);
        } catch { }

        try { 
            var faturaResponse = await _faturaService.GetPagedAsync(1, 1); 
            bekleyenFaturaCount = faturaResponse.TotalCount; 
        } catch { }

        ViewBag.AboneCount = aboneCount;
        ViewBag.SozlesmeCount = sozlesmeCount;
        ViewBag.AktifIsEmriCount = aktifIsEmriCount;
        ViewBag.BekleyenFaturaCount = bekleyenFaturaCount;

        return View();
    }

    public async System.Threading.Tasks.Task<IActionResult> Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async System.Threading.Tasks.Task<IActionResult> Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
