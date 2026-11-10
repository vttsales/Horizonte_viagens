// ALTERAÇÃO PIM: Testes de negócio, integração HTTP, autorização e cálculo sem serviço pago.
using HorizonteViagens.Modelos;
using HorizonteViagens.Negocio;
using HorizonteViagens.Repositorio.Interfaces;
using Xunit;

namespace HorizonteViagens.Testes;

public class ClienteNegocioTests
{
    [Fact]
    public void Cadastrar_RejeitaCpfComDigitoVerificadorIncorreto()
    {
        var negocio = new ClienteNegocio(new RepositorioClienteFalso());
        var cliente = new Cliente { Nome = "Teste", Email = "teste@example.com", Cpf = "529.982.247-24", Celular = "11999999999" };
        var excecao = Assert.Throws<RegraDeNegocioException>(() => negocio.Cadastrar(cliente, "SenhaForte1!"));
        Assert.Equal("O CPF informado é inválido.", excecao.Message);
    }

    [Fact]
    public void Cadastrar_AceitaCpfValidoFormatadoEArmazenaSomenteHash()
    {
        var repo = new RepositorioClienteFalso();
        var negocio = new ClienteNegocio(repo);
        var cliente = new Cliente { Nome = "Teste", Email = "teste@example.com", Cpf = "529.982.247-25", Celular = "(11) 99999-9999" };
        negocio.Cadastrar(cliente, "SenhaForte1!");
        Assert.Equal("52998224725", repo.Salvo!.Cpf);
        Assert.NotEqual("SenhaForte1!", repo.Salvo.SenhaHash);
        Assert.True(SenhaHasher.Conferir("SenhaForte1!", repo.Salvo.SenhaHash));
    }

    private sealed class RepositorioClienteFalso : IClienteRepositorio
    {
        public Cliente? Salvo { get; private set; }
        public List<Cliente> Listar() => [];
        public Cliente? ObterPorId(int id) => Salvo?.Id == id ? Salvo : null;
        public Cliente? ObterPorEmail(string email) => null;
        public bool EmailEmUso(string email, int? ignorarId = null) => false;
        public bool CpfEmUso(string cpf, int? ignorarId = null) => false;
        public void Adicionar(Cliente cliente) { cliente.Id = 1; Salvo = cliente; }
        public void Atualizar(Cliente cliente) => Salvo = cliente;
        public void Remover(Cliente cliente) => Salvo = null;
        public List<Cliente> BuscarPorNome(string trecho) => [];
    }
}
