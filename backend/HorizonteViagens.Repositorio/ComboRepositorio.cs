// ALTERAÇÃO PIM: Mapeamento e persistência do esquema integrado.
using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using HorizonteViagens.Modelos;
using HorizonteViagens.Repositorio.Contexto;
using HorizonteViagens.Repositorio.Interfaces;

namespace HorizonteViagens.Repositorio;

// Única classe que conversa com a tabela Combos.
// Cadastro com EF Core; busca da vitrine pela procedure, com Dapper.
public class ComboRepositorio : IComboRepositorio
{
    private readonly AppDbContext _contexto;

    // INJEÇÃO DE DEPENDÊNCIA: o contexto chega pronto pelo construtor.
    public ComboRepositorio(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    // Leitura para a tela do administrador: todos, inclusive os sem vaga.
    // AsNoTracking porque é só leitura (Aula 03 de Desenvolvimento Web).
    public List<Combo> Listar() =>
        _contexto.Combos.AsNoTracking().OrderBy(c => c.Nome).ToList();

    public Combo? ObterPorId(int id) =>
        _contexto.Combos.FirstOrDefault(c => c.Id == id);

    public void Adicionar(Combo combo)
    {
        _contexto.Combos.Add(combo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Combo combo)
    {
        _contexto.Combos.Update(combo);
        _contexto.SaveChanges();
    }

    public void Remover(Combo combo)
    {
        _contexto.Combos.Remove(combo);
        _contexto.SaveChanges();
    }

    // A procedure recebe o destino como PARÂMETRO (new { Destino = ... }):
    // o valor nunca é concatenado no SQL, o que impede SQL Injection.
    // O Dapper usa a mesma conexão do EF Core, sem connection string própria.
    public List<Combo> BuscarDisponiveis(string? destino = null)
    {
        var conexao = _contexto.Database.GetDbConnection();
        return conexao.Query<Combo>(
                "sp_Combos_BuscarPorDestino",
                new { Destino = string.IsNullOrWhiteSpace(destino) ? null : destino.Trim() },
                commandType: CommandType.StoredProcedure)
            .ToList();
    }
}
