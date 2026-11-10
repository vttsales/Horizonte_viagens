// ALTERAÇÃO PIM: Rotas de conta, catálogo, gestão, reservas e documentos com autorização.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HorizonteViagens.Modelos.Dtos;
using HorizonteViagens.Negocio;

namespace HorizonteViagens.Api.Controllers;

// Controller da Api: recebe, chama o Negocio e escolhe o código HTTP.
// A vitrine (GET) é pública; cadastrar, alterar e excluir exigem Administrador.
[ApiController]
[Route("api/[controller]")]
public class CombosController : ControllerBase
{
    private readonly ComboNegocio _negocio;
    private readonly ILogger<CombosController> _log;

    public CombosController(ComboNegocio negocio, ILogger<CombosController> log)
    {
        _negocio = negocio;
        _log = log;
    }

    // GET api/combos?destino=paris   → 200, só combos com vaga (procedure)
    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<ComboDto>> Listar([FromQuery] string? destino = null) =>
        Ok(_negocio.ListarDisponiveis(destino).Select(ComboDto.De));

    // GET api/combos/todos   → 200, todos os combos, inclusive sem vaga (administrador)
    [Authorize(Roles = "Administrador")]
    [HttpGet("todos")]
    public ActionResult<IEnumerable<ComboDto>> ListarTodos() =>
        Ok(_negocio.Listar().Select(ComboDto.De));

    // GET api/combos/5   → 200 ou 404
    [Authorize(Roles = "Administrador")]
    [HttpGet("{id:int}")]
    public ActionResult<ComboDto> Obter(int id)
    {
        var combo = _negocio.ObterPorId(id);
        if (combo is null) return NotFound();
        return Ok(ComboDto.De(combo));
    }

    // POST api/combos   → 201 Created + Location, ou 400
    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public ActionResult<ComboDto> Criar(ComboSalvarDto dto)
    {
        try
        {
            var combo = _negocio.Cadastrar(dto.ParaCombo());
            // Log estruturado: {ComboId} vira um campo pesquisável (Aula 07 de Desenvolvimento Web).
            _log.LogInformation("Combo {ComboId} cadastrado", combo.Id);
            return CreatedAtAction(nameof(Obter), new { id = combo.Id }, ComboDto.De(combo));
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    // PUT api/combos/5   → 204, 400 ou 404
    [Authorize(Roles = "Administrador")]
    [HttpPut("{id:int}")]
    public IActionResult Atualizar(int id, ComboSalvarDto dto)
    {
        if (_negocio.ObterPorId(id) is null) return NotFound();

        try
        {
            _negocio.Atualizar(dto.ParaCombo(id));
            _log.LogInformation("Combo {ComboId} alterado", id);
            return NoContent();
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    // DELETE api/combos/5   → 204 ou 404
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    public IActionResult Remover(int id)
    {
        if (!_negocio.Remover(id)) return NotFound();

        _log.LogInformation("Combo {ComboId} excluído", id);
        return NoContent();
    }
}
