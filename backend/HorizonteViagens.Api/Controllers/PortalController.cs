// ALTERAÇÃO PIM: endpoints da área do cliente, com identidade derivada exclusivamente do JWT.
using System.Security.Claims;
using HorizonteViagens.Modelos;
using HorizonteViagens.Modelos.Dtos;
using HorizonteViagens.Negocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HorizonteViagens.Api.Controllers;
[ApiController,Route("api/portal"),Authorize]
public class PortalController(PortalNegocio portal,ClienteNegocio clientes,PagamentoNegocio pagamentos):ControllerBase
{
 private int Id=>User.IsInRole("Cliente")?int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!):throw new UnauthorizedAccessException();
 [AllowAnonymous,HttpGet("catalogo")] public IActionResult Catalogo()=>Ok(portal.Catalogo().Select(ComboDto.De));
 [AllowAnonymous,HttpGet("orcamento/{comboId:int}")] public IActionResult Orcamento(int comboId,int viajantes=1)=>Ok(portal.Orcar(comboId,viajantes));
 [HttpGet("perfil")] public IActionResult Perfil()=>Ok(ClienteDto.De(portal.Cliente(Id)));
 [HttpPut("perfil")] public IActionResult Perfil(ClienteSalvarDto dto){var c=dto.ParaCliente(Id);c.Ativo=true;clientes.Atualizar(c);return Ok(ClienteDto.De(portal.Cliente(Id)));}
 [HttpGet("favoritos")] public IActionResult Favoritos()=>Ok(portal.Favoritos(Id));
 [HttpPut("favoritos/{comboId:int}")] public IActionResult Favoritar(int comboId){portal.Favoritar(Id,comboId,true);return NoContent();}
 [HttpDelete("favoritos/{comboId:int}")] public IActionResult Desfavoritar(int comboId){portal.Favoritar(Id,comboId,false);return NoContent();}
 [HttpGet("reservas")] public IActionResult Reservas()=>Ok(portal.Reservas(Id).Select(ReservaDto.De));
 [HttpPost("reservas")] public IActionResult Reservar(ReservaEntrada dto)=>Ok(ReservaDto.De(portal.Reservar(Id,dto)));
 [HttpPost("reservas/{id:int}/cancelar")] public IActionResult Cancelar(int id)=>portal.Cancelar(id,Id)?NoContent():NotFound();
 [HttpGet("reservas/{id:int}/documentos")] public IActionResult Documentos(int id)=>Ok(portal.Documentos(id,Id));
 [HttpPost("reservas/{id:int}/pagamento")] public async Task<IActionResult> Pagar(int id)=>Ok(new{url=await pagamentos.Iniciar(id,Id),ambiente="teste"});
 [HttpPost("reservas/{id:int}/verificar")] public async Task<IActionResult> Verificar(int id)=>Ok(ReservaDto.De(await pagamentos.Verificar(id,Id)));
 // ALTERAÇÃO PIM Pages: gestão usa o mesmo contrato da carteira, sem expor entidades ou dados de outros usuários a clientes.
 [Authorize(Roles="Administrador"),HttpGet("gestao/reservas")] public IActionResult GestaoReservas()=>Ok(portal.Reservas(null).Select(ReservaDto.De));
 [Authorize(Roles="Administrador"),HttpGet("gestao/reservas/{id:int}/documentos")] public IActionResult GestaoDocumentos(int id)=>Ok(portal.Documentos(id,null));
 [Authorize(Roles="Administrador"),HttpPost("documentos/{id:int}")] public IActionResult Documento(int id,DocumentoEntrada dto){portal.Emitir(id,dto);return NoContent();}
}
