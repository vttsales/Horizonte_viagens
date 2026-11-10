// ALTERAÇÃO PIM: Regras de validação, autenticação, concorrência e inicialização compartilhadas.
using HorizonteViagens.Modelos;
using HorizonteViagens.Repositorio.Interfaces;

namespace HorizonteViagens.Negocio;

// Regras do catálogo de combos, compartilhadas pela Web e pela Api.
// Mesmo formato do ClienteNegocio: Cadastrar, Atualizar e Remover separados.
public class ComboNegocio
{
    private readonly IComboRepositorio _repositorio;

    // INJEÇÃO DE DEPENDÊNCIA: pede o contrato (a interface), nunca a classe concreta.
    public ComboNegocio(IComboRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // Todos os combos, inclusive sem vaga: é a lista do administrador.
    public List<Combo> Listar() => _repositorio.Listar();

    // Só os que têm vaga, pela procedure: é a vitrine.
    public List<Combo> ListarDisponiveis(string? destino = null) =>
        _repositorio.BuscarDisponiveis(destino);

    public Combo? ObterPorId(int id) => _repositorio.ObterPorId(id);

    public Combo Cadastrar(Combo combo)
    {
        Normalizar(combo);
        Validar(combo);
        _repositorio.Adicionar(combo);
        return combo;
    }

    public void Atualizar(Combo combo)
    {
        // Os dados novos são copiados para o combo que veio do banco.
        // Mandar salvar o objeto recebido direto daria erro no EF Core
        // ("already being tracked"), porque o lido com o mesmo Id já está
        // sendo acompanhado. É o mesmo cuidado do ClienteNegocio.Atualizar.
        var existente = _repositorio.ObterPorId(combo.Id)
            ?? throw new RegraDeNegocioException("Combo não encontrado.");

        Normalizar(combo);
        Validar(combo);

        // Exige a versão lida para impedir que um formulário antigo sobrescreva vagas reservadas.
        if (existente.RowVersion is {Length: > 0} && !combo.RowVersion.SequenceEqual(existente.RowVersion))
            throw new RegraDeNegocioException("O combo mudou desde que você abriu a tela. Reabra a edição.");
        existente.Origem = combo.Origem;
        existente.Evento = combo.Evento;
        existente.Descricao = combo.Descricao;
        existente.Inclusoes = combo.Inclusoes;
        existente.Imagem = combo.Imagem;
        existente.Noites = combo.Noites;
        existente.DiariaQuarto = combo.DiariaQuarto;
        existente.ValorExperiencias = combo.ValorExperiencias;
        existente.Partida = combo.Partida;
        existente.Publicado = combo.Publicado;
        existente.PrecoConfirmado = combo.PrecoConfirmado;
        existente.Nome = combo.Nome;
        existente.Destino = combo.Destino;
        existente.Categoria = combo.Categoria;
        existente.Vagas = combo.Vagas;
        existente.Valor = combo.Valor;

        _repositorio.Atualizar(existente);
    }

    // Devolve false quando o combo não existe.
    public bool Remover(int id)
    {
        var combo = _repositorio.ObterPorId(id);
        if (combo is null) return false;

        _repositorio.Remover(combo);
        return true;
    }

    // ---------- regras ----------

    private static void Normalizar(Combo combo)
    {
        combo.Nome = (combo.Nome ?? "").Trim();
        combo.Destino = (combo.Destino ?? "").Trim();
        combo.Categoria = (combo.Categoria ?? "").Trim();
        combo.Origem ??= "";combo.Evento ??= "";combo.Descricao ??= "";combo.Inclusoes ??= "";
    }

    private static void Validar(Combo combo)
    {
        if(combo.Nome.Length>100 || combo.Destino.Length>100 || combo.Categoria.Length>30 || combo.Origem.Length>100 || combo.Evento.Length>140 || combo.Descricao.Length>2000 || combo.Inclusoes.Length>2000)
            throw new RegraDeNegocioException("Um dos campos excedeu o limite permitido.");
        if (combo.Noites is < 1 or > 60 || combo.DiariaQuarto < 0 || combo.ValorExperiencias < 0)
            throw new RegraDeNegocioException("Informe de 1 a 60 noites e custos não negativos.");
        if (combo.Publicado && combo.PrecoConfirmado && (combo.Partida is null || combo.Partida <= DateOnly.FromDateTime(DateTime.Today)))
            throw new RegraDeNegocioException("Uma oferta reservável precisa de partida futura.");
        if (!new[]{"hero","beach","coast","paris","tokyo","newyork"}.Contains(combo.Imagem))
            throw new RegraDeNegocioException("Imagem inválida.");
        if (combo.Nome.Length == 0 || combo.Destino.Length == 0 || combo.Categoria.Length == 0)
            throw new RegraDeNegocioException("Nome, destino e categoria são obrigatórios.");

        if (combo.Vagas < 0)
            throw new RegraDeNegocioException("As vagas não podem ser negativas.");

        if (combo.Valor < 0)
            throw new RegraDeNegocioException("O valor não pode ser negativo.");
    }
}
