// ALTERAÇÃO PIM: Mapeamento e persistência do esquema integrado.
using HorizonteViagens.Modelos;

namespace HorizonteViagens.Repositorio.Interfaces;

// Contrato do repositório de administradores.
// O AdministradorNegocio conhece só esta interface, nunca a classe concreta.
public interface IAdministradorRepositorio
{
    Administrador? ObterPorEmail(string email);
    void Atualizar(Administrador administrador);
}
