using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HorizonteViagens.Modelos.Dtos;
using HorizonteViagens.Negocio;

namespace HorizonteViagens.Web.Controllers;

// Login e saída do administrador.
// O controller não confere senha: quem faz isso é o AdministradorNegocio.
// Aqui só se entrega (ou se recolhe) o "crachá" do navegador.
[Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("login")]
public class ContaController : Controller
{
    // ALTERAÇÃO PIM: login de cliente usa a mesma validação de senha da API.
    private readonly ClienteNegocio _clientes;
    private readonly AdministradorNegocio _negocio;

    public ContaController(AdministradorNegocio negocio, ClienteNegocio clientes)
    {
        _negocio = negocio;
        _clientes = clientes;
    }

    // GET /Conta/Login
    // returnUrl: a tela que a pessoa tentou abrir antes de ser mandada para o login.
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl, bool admin=false)
    {
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["Admin"] = admin;
        return View(new LoginDto());
    }

    // POST /Conta/Login
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto modelo, string? returnUrl, bool admin=false)
    {
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["Admin"] = admin;
        if (!ModelState.IsValid) return View(modelo);

        HorizonteViagens.Modelos.Usuario? administrador = admin ? _negocio.Autenticar(modelo.Email, modelo.Senha) : _clientes.Autenticar(modelo.Email, modelo.Senha);
        if (administrador is null)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha incorretos.");
            return View(modelo);
        }

        // As "claims" são as informações escritas no crachá.
        // A Role vem do Perfil (polimorfismo): para um Administrador, "Administrador".
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, administrador.Id.ToString()),
            new(ClaimTypes.Name, administrador.Nome),
            new(ClaimTypes.Email, administrador.Email),
            new(ClaimTypes.Role, administrador.Perfil)
        };
        var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidade));

        // Url.IsLocalUrl impede que alguém use o returnUrl para mandar o
        // administrador para um site falso depois do login.
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Viagens");
    }

    // POST /Conta/Sair
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sair()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // GET /Conta/Negado  → logado, mas sem permissão para a tela
    [AllowAnonymous]
    public IActionResult Negado() => View();

    // ALTERAÇÃO PIM: perfil só do próprio cliente; o Id nunca vem do formulário.
    [Authorize(Roles="Cliente")]
    public IActionResult Perfil(){var c=_clientes.ObterPorId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));return c is null?NotFound():View(ClienteSalvarDto.De(c));}
    [Authorize(Roles="Cliente"),HttpPost,ValidateAntiForgeryToken]
    public IActionResult Perfil(ClienteSalvarDto dto){if(!ModelState.IsValid)return View(dto);try{var c=dto.ParaCliente(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));c.Ativo=true;_clientes.Atualizar(c);TempData["Mensagem"]="Perfil atualizado.";return RedirectToAction(nameof(Perfil));}catch(RegraDeNegocioException e){ModelState.AddModelError("",e.Message);return View(dto);}}
}
