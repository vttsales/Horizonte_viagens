// ALTERAÇÃO PIM: respeita validade de acessos temporários na autenticação.
using HorizonteViagens.Modelos;
using HorizonteViagens.Repositorio.Interfaces;

namespace HorizonteViagens.Negocio;

// Regras do administrador (a agência).
// A principal é o login: só quem passa por aqui ganha acesso aos dados dos clientes.
public class AdministradorNegocio
{
    private readonly IAdministradorRepositorio _repositorio;

    public AdministradorNegocio(IAdministradorRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // Devolve o administrador se e-mail e senha estiverem corretos; senão, null.
    public Administrador? Autenticar(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            return null;

        var administrador = _repositorio.ObterPorEmail(email.Trim().ToLowerInvariant());

        // A mesma resposta (null) para "e-mail não existe" e "senha errada".
        // Se o sistema dissesse qual dos dois errou, um invasor descobriria
        // quais e-mails estão cadastrados.
        if (administrador is null || !administrador.Ativo || administrador.AcessoAte<=DateTime.UtcNow)
            return null;

        if (!SenhaHasher.Conferir(senha, administrador.SenhaHash))
            return null;

        administrador.UltimoAcesso = DateTime.Now;
        _repositorio.Atualizar(administrador);

        return administrador;
    }
}
