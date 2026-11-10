// ALTERAÇÃO PIM: Mapeamento e persistência do esquema integrado.
using Microsoft.EntityFrameworkCore;
using HorizonteViagens.Modelos;
using HorizonteViagens.Repositorio.Contexto;
using HorizonteViagens.Repositorio.Interfaces;

namespace HorizonteViagens.Repositorio;

// Só acesso a dados. As regras da compra (cliente ativo, combo existe, tem vaga,
// descontar a vaga) ficam no CompraNegocio; aqui a compra é gravada.
public class CompraRepositorio : ICompraRepositorio
{
    private readonly AppDbContext _contexto;

    public CompraRepositorio(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    // Transação + concorrência otimista, como na Aula 03 de Desenvolvimento Web.
    // O combo chega aqui já com a vaga descontada pelo CompraNegocio e acompanhado
    // pelo EF Core (foi lido pelo ComboRepositorio no mesmo DbContext da requisição).
    // O SaveChanges grava as duas coisas: o UPDATE do combo, com
    // "WHERE Id = @id AND RowVersion = @original", e o INSERT da compra.
    public bool Registrar(Compra compra)
    {
        using var transacao = _contexto.Database.BeginTransaction();
        try
        {
            _contexto.Compras.Add(compra);
            _contexto.SaveChanges();
            transacao.Commit();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Outra pessoa alterou o combo entre a leitura e a gravação
            // (por exemplo, comprou a última vaga). Nada é gravado.
            transacao.Rollback();
            return false;
        }
    }

    public List<Compra> Listar() => _contexto.Compras.AsNoTracking()
        .Include(c => c.Cliente)
        .Include(c => c.Combo)
        .OrderByDescending(c => c.Id)
        .ToList();

    public bool Monitorar(int compraId, int administradorId)
    {
        if (!_contexto.Compras.Any(c => c.Id == compraId)) return false;

        var jaMonitorada = _contexto.MonitoramentosCompras
            .Any(m => m.CompraId == compraId && m.AdministradorId == administradorId);
        if (jaMonitorada) return true;

        _contexto.MonitoramentosCompras.Add(new MonitoramentoCompra { CompraId = compraId, AdministradorId = administradorId });
        _contexto.SaveChanges();
        return true;
    }
}
