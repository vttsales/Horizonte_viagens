// ALTERAÇÃO PIM: Rotas de conta, catálogo, gestão, reservas e documentos com autorização.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HorizonteViagens.Negocio;
using HorizonteViagens.Modelos.Dtos;

namespace HorizonteViagens.Web.Controllers;

// Controller MVC: recebe a requisição, chama o Negocio e devolve uma view.
// Nenhuma regra de negócio aqui — elas estão em ClienteNegocio.
//
// [Authorize] em cima da CLASSE tranca todas as telas de uma vez:
// listar, buscar, editar e excluir exigem login de Administrador.
// Só o cadastro fica aberto, com [AllowAnonymous], para o viajante se cadastrar.
// É mais seguro trancar tudo e abrir uma porta do que trancar porta por porta.
[Authorize(Roles = "Administrador")]
public class ClientesController : Controller
{
    private readonly ClienteNegocio _negocio;

    // INJEÇÃO DE DEPENDÊNCIA: o controller não escreve "new ClienteNegocio(...)".
    // Ele pede um ClienteNegocio no construtor e o ASP.NET Core entrega pronto,
    // já com o repositório e o banco montados por dentro. A lista do que o ASP.NET
    // sabe montar está no Program.cs. Explicação completa: INJECAO-DE-DEPENDENCIA.md.
    public ClientesController(ClienteNegocio negocio)
    {
        _negocio = negocio;
    }

    // GET /Clientes            → lista todos
    // GET /Clientes?busca=ana  → usa a stored procedure
    public IActionResult Index(string? busca)
    {
        var clientes = string.IsNullOrWhiteSpace(busca)
            ? _negocio.Listar()
            : _negocio.BuscarPorNome(busca);

        ViewData["Busca"] = busca;
        return View(clientes.Select(ClienteDto.De).ToList());
    }

    // GET /Clientes/Criar   (aberto: é o cadastro do viajante)
    [AllowAnonymous]
    public IActionResult Criar() => View(new ClienteSalvarDto());

    // POST /Clientes/Criar
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Criar(ClienteSalvarDto modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        try
        {
            _negocio.Cadastrar(modelo.ParaCliente(), modelo.Senha);

            // O administrador volta para a lista; o viajante não pode ver a lista,
            // então vai para uma tela de "cadastro concluído".
            if (User.IsInRole("Administrador"))
            {
                TempData["Mensagem"] = "Cliente cadastrado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(CadastroConcluido));
        }
        catch (RegraDeNegocioException ex)
        {
            // A mensagem da regra aparece no topo do formulário.
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(modelo);
        }
    }

    // GET /Clientes/CadastroConcluido
    [AllowAnonymous]
    public IActionResult CadastroConcluido() => View();

    // GET /Clientes/Editar/5
    public IActionResult Editar(int id)
    {
        var cliente = _negocio.ObterPorId(id);
        if (cliente is null) return NotFound();

        // O DTO de entrada não tem Id; a view usa este valor para montar a URL do formulário.
        ViewData["Id"] = id;
        return View(ClienteSalvarDto.De(cliente));
    }

    // POST /Clientes/Editar/5   (o id vem da URL, os campos vêm do formulário)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(int id, ClienteSalvarDto modelo)
    {
        ViewData["Id"] = id;
        if (!ModelState.IsValid) return View(modelo);

        try
        {
            // O formulário de edição não tem campo de senha: o administrador
            // corrige os dados, mas não troca a senha do cliente.
            _negocio.Atualizar(modelo.ParaCliente(id));
            TempData["Mensagem"] = "Cliente atualizado.";
            return RedirectToAction(nameof(Index));
        }
        catch (RegraDeNegocioException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(modelo);
        }
    }

    // GET /Clientes/Excluir/5  → tela de confirmação
    public IActionResult Excluir(int id)
    {
        var cliente = _negocio.ObterPorId(id);
        if (cliente is null) return NotFound();
        return View(ClienteDto.De(cliente));
    }

    // POST /Clientes/Excluir/5
    [HttpPost, ActionName("Excluir")]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmarExclusao(int id)
    {
        _negocio.Remover(id);
        TempData["Mensagem"] = "Cliente excluído.";
        return RedirectToAction(nameof(Index));
    }
}
