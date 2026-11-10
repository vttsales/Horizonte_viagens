// ALTERAÇÃO PIM: reservas do cliente, gestão do administrador e carteira com documentos emitidos pelo fornecedor.
using System.Security.Claims;
using HorizonteViagens.Modelos;
using HorizonteViagens.Modelos.Dtos;
using HorizonteViagens.Negocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HorizonteViagens.Web.Controllers;
[Authorize]
public class ViagensController(PortalNegocio portal,PagamentoNegocio pagamentos):Controller
{
 private int Id=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 private int? ClienteId=>User.IsInRole("Administrador")?null:Id;
 public IActionResult Index()=>View(portal.Reservas(ClienteId));
 public IActionResult Detalhes(int id){var c=portal.Reserva(id,ClienteId);ViewBag.Documentos=portal.Documentos(id,ClienteId);ViewBag.PagamentoConfigurado=pagamentos.Configurado;return View(c);}
 [Authorize(Roles="Cliente"),HttpPost] public IActionResult Reservar(ReservaEntrada dto){if(!ModelState.IsValid){TempData["Erro"]="Confira os dados da reserva.";return RedirectToAction("Detalhes","Combos",new{id=dto.ComboId});}try{var c=portal.Reservar(Id,dto);return RedirectToAction(nameof(Detalhes),new{id=c.Id});}catch(RegraDeNegocioException e){TempData["Erro"]=e.Message;return RedirectToAction("Detalhes","Combos",new{id=dto.ComboId,viajantes=dto.Viajantes});}}
 [Authorize(Roles="Cliente"),HttpPost] public async Task<IActionResult> Pagar(int id){try{return Redirect(await pagamentos.Iniciar(id,Id));}catch(RegraDeNegocioException e){TempData["Erro"]=e.Message;return RedirectToAction(nameof(Detalhes),new{id});}}
 [Authorize(Roles="Cliente"),HttpPost] public async Task<IActionResult> Verificar(int id){try{await pagamentos.Verificar(id,Id);TempData["Mensagem"]="Status consultado no serviço de pagamento.";}catch(RegraDeNegocioException e){TempData["Erro"]=e.Message;}return RedirectToAction(nameof(Detalhes),new{id});}
 [Authorize(Roles="Cliente"),HttpPost] public IActionResult Cancelar(int id){try{portal.Cancelar(id,Id);TempData["Mensagem"]="Reserva cancelada.";}catch(InvalidOperationException e){TempData["Erro"]=e.Message;}return RedirectToAction(nameof(Detalhes),new{id});}
 [Authorize(Roles="Administrador"),HttpPost] public IActionResult Emitir(int id,DocumentoEntrada dto){if(!ModelState.IsValid)TempData["Erro"]="Confira título, tipo, código e URL do documento.";else try{portal.Emitir(id,dto);TempData["Mensagem"]="Documento adicionado à carteira do cliente.";}catch(RegraDeNegocioException e){TempData["Erro"]=e.Message;}return RedirectToAction(nameof(Detalhes),new{id});}
 [Authorize(Roles="Cliente")] public IActionResult Carteira(){var lista=portal.Reservas(Id).Where(c=>c.Status=="Confirmada").SelectMany(c=>portal.Documentos(c.Id,Id));return View(lista.ToList());}
 [Authorize(Roles="Cliente")] public IActionResult Favoritos(){var ids=portal.Favoritos(Id);ViewBag.Viajantes=2;return View("~/Views/Combos/Index.cshtml",portal.Catalogo().Where(c=>ids.Contains(c.Id)).Select(ComboDto.De).ToList());}
 [Authorize(Roles="Cliente"),HttpPost] public IActionResult Favoritar(int id,bool remover=false){portal.Favoritar(Id,id,!remover);TempData["Mensagem"]=remover?"Favorito removido.":"Destino salvo.";return RedirectToAction(nameof(Favoritos));}
}
