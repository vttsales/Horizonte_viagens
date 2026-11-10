// ALTERAÇÃO PIM: Mapeamento e persistência do esquema integrado.
using HorizonteViagens.Modelos;

namespace HorizonteViagens.Repositorio.Interfaces;

// Contrato do repositório. A camada Negocio conhece só esta interface,
// nunca a classe concreta — assim dá para trocar EF por Dapper sem mexer no negócio.
public interface IClienteRepositorio
{
    List<Cliente> Listar();
    Cliente? ObterPorId(int id);
    // Login do viajante usa leitura só para autenticar, nunca senha devolvida à API.
    Cliente? ObterPorEmail(string email);
    bool EmailEmUso(string email, int? ignorarId = null);
    bool CpfEmUso(string cpf, int? ignorarId = null);
    void Adicionar(Cliente cliente);
    void Atualizar(Cliente cliente);
    void Remover(Cliente cliente);

    // Executa a stored procedure sp_Clientes_BuscarPorNome.
    List<Cliente> BuscarPorNome(string trecho);
}
