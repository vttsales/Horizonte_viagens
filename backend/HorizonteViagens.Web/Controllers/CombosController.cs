// ALTERAÇÃO PIM: vitrine e CRUD MVC completos; reutiliza negócio e validação da API.
using HorizonteViagens.Modelos;
using HorizonteViagens.Modelos.Dtos;
using HorizonteViagens.Negocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HorizonteViagens.Web.Controllers;
public class CombosController(ComboNegocio negocio,PortalNegocio portal):Controller
{
 [HttpGet] public IActionResult Index(string? destino,string? categoria,int viajantes=2,string? ordem=null){ViewBag.Destino=destino;ViewBag.Categoria=categoria;ViewBag.Viajantes=Math.Clamp(viajantes,1,6);ViewBag.Ordem=ordem;var lista=portal.Catalogo().Where(c=>string.IsNullOrEmpty(destino)||(c.Destino+" "+c.Evento).Contains(destino,StringComparison.OrdinalIgnoreCase)).Where(c=>string.IsNullOrEmpty(categoria)||c.Categoria==categoria);lista=ordem=="preco"?lista.OrderBy(c=>Precificacao.Calcular(c,ViewBag.Viajantes).Total):lista;return View(lista.Select(ComboDto.De).ToList());}
 [HttpGet] public IActionResult Detalhes(int id,int viajantes=2){var c=negocio.ObterPorId(id);if(c==null||!c.Publicado)return NotFound();ViewBag.Orcamento=Precificacao.Calcular(c,Math.Clamp(viajantes,1,6));return View(ComboDto.De(c));}
 [Authorize(Roles="Administrador")] public IActionResult Gerenciar()=>View(negocio.Listar().Select(ComboDto.De).ToList());
 [Authorize(Roles="Administrador")] public IActionResult Criar()=>View(new ComboSalvarDto());
 [Authorize(Roles="Administrador"),HttpPost] public IActionResult Criar(ComboSalvarDto dto){if(!ModelState.IsValid)return View(dto);try{negocio.Cadastrar(dto.ParaCombo());TempData["Mensagem"]="Combo cadastrado.";return RedirectToAction(nameof(Gerenciar));}catch(RegraDeNegocioException ex){ModelState.AddModelError("",ex.Message);return View(dto);}}
 [Authorize(Roles="Administrador")] public IActionResult Editar(int id){var c=negocio.ObterPorId(id);if(c==null)return NotFound();ViewBag.Id=id;return View(ComboSalvarDto.De(c));}
 [Authorize(Roles="Administrador"),HttpPost] public IActionResult Editar(int id,ComboSalvarDto dto){ViewBag.Id=id;if(!ModelState.IsValid)return View(dto);try{negocio.Atualizar(dto.ParaCombo(id));TempData["Mensagem"]="Combo atualizado.";return RedirectToAction(nameof(Gerenciar));}catch(RegraDeNegocioException ex){ModelState.AddModelError("",ex.Message);return View(dto);}}
 [Authorize(Roles="Administrador")] public IActionResult Excluir(int id){var c=negocio.ObterPorId(id);return c==null?NotFound():View(ComboDto.De(c));}
 [Authorize(Roles="Administrador"),HttpPost,ActionName("Excluir")] public IActionResult ExcluirConfirmado(int id){try{if(!negocio.Remover(id))return NotFound();TempData["Mensagem"]="Combo excluído.";return RedirectToAction(nameof(Gerenciar));}catch(Microsoft.EntityFrameworkCore.DbUpdateException){ModelState.AddModelError("","Este combo possui reservas ou serviços vinculados. Despublique-o para preservar o histórico.");return View(ComboDto.De(negocio.ObterPorId(id)!));}}
}
