// ALTERAÇÃO PIM: Regras de validação, autenticação, concorrência e inicialização compartilhadas.
using HorizonteViagens.Modelos;
using HorizonteViagens.Repositorio.Interfaces;

namespace HorizonteViagens.Negocio;

// Regras da compra. O enunciado pede que regras como "verificar a disponibilidade
// antes de uma venda" fiquem nesta camada, e não no controller nem no repositório.
public class CompraNegocio
{
    private readonly ICompraRepositorio _compras;
    private readonly IClienteRepositorio _clientes;
    private readonly IComboRepositorio _combos;

    // Pede os três repositórios de que precisa. Os três já estão registrados no
    // Program.cs, então o ASP.NET Core entrega todos prontos (INJECAO-DE-DEPENDENCIA.md, seção 6).
    public CompraNegocio(ICompraRepositorio compras, IClienteRepositorio clientes, IComboRepositorio combos)
    {
        _compras = compras;
        _clientes = clientes;
        _combos = combos;
    }

    public Compra Comprar(int clienteId, int comboId)
    {
        // Regra 1: só cliente cadastrado e ativo compra.
        var cliente = _clientes.ObterPorId(clienteId);
        if (cliente is null || !cliente.Ativo)
            throw new RegraDeNegocioException("Cliente não encontrado ou inativo.");

        // Regra 2: o combo precisa existir.
        var combo = _combos.ObterPorId(comboId)
            ?? throw new RegraDeNegocioException("Combo não encontrado.");

        // Regra 3: só compra se tiver vaga.
        if (combo.Vagas <= 0)
            throw new RegraDeNegocioException("Não há vagas disponíveis para este combo.");

        // Regra 4: cada compra ocupa uma vaga.
        combo.Vagas -= 1;

        // Regra 5: a compra guarda o valor do combo no momento da venda.
        // Se o preço mudar depois, a compra antiga continua com o valor pago.
        var compra = new Compra { ClienteId = clienteId, ComboId = comboId, Valor = combo.Valor };

        // O repositório grava a compra e a vaga descontada juntas. Se outra pessoa
        // comprou a última vaga entre a leitura e a gravação, o RowVersion do combo
        // não confere, nada é gravado, e o repositório devolve false.
        if (!_compras.Registrar(compra))
            throw new RegraDeNegocioException("As vagas deste combo acabaram de mudar. Tente novamente.");

        return compra;
    }

    public List<Compra> Listar() => _compras.Listar();

    public bool Monitorar(int compraId, int administradorId) => _compras.Monitorar(compraId, administradorId);
}
