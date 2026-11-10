// ALTERAÇÃO PIM: Testes de negócio, integração HTTP, autorização e cálculo sem serviço pago.
using HorizonteViagens.Modelos;
using HorizonteViagens.Negocio;
using Xunit;

namespace HorizonteViagens.Testes;

public class ComboNegocioTests
{
    [Fact]
    public void Cadastrar_ComboValido_GuardaNoRepositorio()
    {
        var repo = new ComboRepositorioFalso();                                       // ARRANGE
        var negocio = new ComboNegocio(repo);

        negocio.Cadastrar(new Combo { Nome = " Paris ", Destino = "Paris", Categoria = "Luxo", Vagas = 20, Valor = 15000m }); // ACT

        Assert.Single(repo.Itens);                                                    // ASSERT
        Assert.Equal("Paris", repo.Itens[0].Nome);
    }

    [Fact]
    public void Cadastrar_VagasNegativas_LancaExcecaoENaoGrava()
    {
        var repo = new ComboRepositorioFalso();
        var negocio = new ComboNegocio(repo);
        var combo = new Combo { Nome = "Paris", Destino = "Paris", Categoria = "Luxo", Vagas = -1, Valor = 15000m };

        var erro = Assert.Throws<RegraDeNegocioException>(() => negocio.Cadastrar(combo));

        Assert.Equal("As vagas não podem ser negativas.", erro.Message);
        Assert.Empty(repo.Itens);
    }

    [Fact]
    public void Atualizar_CopiaOsDadosParaOComboQueVeioDoBanco()
    {
        var existente = new Combo { Id = 1, Nome = "Paris", Destino = "Paris", Categoria = "Luxo", Vagas = 20, Valor = 15000m };
        var repo = new ComboRepositorioFalso(existente);
        var negocio = new ComboNegocio(repo);

        negocio.Atualizar(new Combo { Id = 1, Nome = "Paris Premium", Destino = "Paris", Categoria = "Luxo", Vagas = 18, Valor = 16000m });

        Assert.Equal("Paris Premium", existente.Nome);
        Assert.Equal(18, existente.Vagas);
        Assert.Equal(16000m, existente.Valor);
    }
}
