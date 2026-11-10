using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HorizonteViagens.Web.Models;

namespace HorizonteViagens.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // ALTERAÇÃO PIM: vitrine vem da mesma camada de negócio utilizada pela API.
            return View(HttpContext.RequestServices.GetRequiredService<HorizonteViagens.Negocio.PortalNegocio>().Catalogo().Select(HorizonteViagens.Modelos.Dtos.ComboDto.De).ToList());
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
