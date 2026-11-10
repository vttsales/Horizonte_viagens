// ALTERAÇÃO PIM: Rotas de conta, catálogo, gestão, reservas e documentos com autorização.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HorizonteViagens.Modelos.Dtos;
using HorizonteViagens.Negocio;

namespace HorizonteViagens.Api.Controllers;

// Login do administrador pela Api.
// POST api/autenticacao/login  → 200 com o token, ou 401 se errar.
[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("login")]
public class AutenticacaoController : ControllerBase
{
    private readonly AdministradorNegocio _negocio;
    private readonly ClienteNegocio _clienteNegocio;
    private readonly GeradorDeToken _gerador;

    public AutenticacaoController(AdministradorNegocio negocio, ClienteNegocio clienteNegocio, GeradorDeToken gerador)
    {
        _negocio = negocio;
        _clienteNegocio = clienteNegocio;
        _gerador = gerador;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var administrador = _negocio.Autenticar(dto.Email, dto.Senha);
        if (administrador is null)
            return Unauthorized(new { erro = "E-mail ou senha incorretos." });

        var (token, expiraEm) = _gerador.Gerar(administrador);
        return Ok(new { token, expiraEm, nome = administrador.Nome });
    }

    // Login separado para o Flutter obter token com papel Cliente e comprar em nome próprio.
    [AllowAnonymous]
    [HttpPost("login-cliente")]
    public IActionResult LoginCliente(LoginDto dto)
    {
        var cliente = _clienteNegocio.Autenticar(dto.Email, dto.Senha);
        if (cliente is null) return Unauthorized(new { erro = "E-mail ou senha incorretos." });
        var (token, expiraEm) = _gerador.Gerar(cliente);
        return Ok(new { token, expiraEm, nome = cliente.Nome });
    }
}
