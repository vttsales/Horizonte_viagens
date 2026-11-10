// ALTERAÇÃO PIM: Mapeamento e persistência do esquema integrado.
using HorizonteViagens.Modelos;

namespace HorizonteViagens.Repositorio.Interfaces;

public interface ICompraRepositorio
{
    // Grava a compra e a vaga já descontada do combo, numa transação.
    // Devolve false se outra pessoa alterou o combo antes (concorrência).
    bool Registrar(Compra compra);
    List<Compra> Listar();
    bool Monitorar(int compraId, int administradorId);
}
