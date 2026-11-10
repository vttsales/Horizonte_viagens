using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HorizonteViagens.Modelos.Dtos;
using HorizonteViagens.Negocio;

namespace HorizonteViagens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly CompraNegocio _negocio;
    private readonly ILogger<ComprasController> _log;

    public ComprasController(CompraNegocio negocio, ILogger<ComprasController> log)
    {
        _negocio = negocio;
        _log = log;
    }

    // POST api/compras   → 200 com a compra, ou 400 (regra de negócio)
    // O cliente vem do TOKEN, não do JSON: assim ninguém compra em nome de outra pessoa.
    // ALTERAÇÃO PIM: reserva unificada com passageiros, idempotência e preço calculado no servidor.
    [Authorize(Roles="Cliente"),HttpPost]
    public IActionResult Comprar(HorizonteViagens.Modelos.ReservaEntrada dto,[FromServices] PortalNegocio portal)
    {
        var id=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(ReservaDto.De(portal.Reservar(id,dto)));
    }

    // GET api/compras   → 200, monitoramento exclusivo do administrador
    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public ActionResult<IEnumerable<CompraDto>> Listar() =>
        Ok(_negocio.Listar().Select(CompraDto.De));

    // POST api/compras/5/monitoramento   → 204 ou 404
    [Authorize(Roles = "Administrador")]
    [HttpPost("{id:int}/monitoramento")]
    public IActionResult Monitorar(int id)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var administradorId))
            return Unauthorized();

        return _negocio.Monitorar(id, administradorId) ? NoContent() : NotFound();
    }
}
