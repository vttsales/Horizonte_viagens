// ALTERAÇÃO PIM: Mapeamento e persistência do esquema integrado.
using HorizonteViagens.Modelos;

namespace HorizonteViagens.Repositorio.Interfaces;

// Contrato do repositório de combos, no mesmo formato do IClienteRepositorio:
// Listar, ObterPorId, Adicionar, Atualizar e Remover, mais a busca pela procedure.
public interface IComboRepositorio
{
    List<Combo> Listar();
    Combo? ObterPorId(int id);
    void Adicionar(Combo combo);
    void Atualizar(Combo combo);
    void Remover(Combo combo);

    // Executa a stored procedure sp_Combos_BuscarPorDestino (só combos com vaga).
    List<Combo> BuscarDisponiveis(string? destino = null);
}
