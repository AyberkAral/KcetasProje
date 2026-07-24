using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;
using KcetasWeb.Models;

namespace KcetasWeb.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            
            // Log the exception if needed
            
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
