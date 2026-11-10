// ALTERAÇÃO PIM: Cálculo por viajantes e quartos inteiros compartilhado por site/API.
using HorizonteViagens.Modelos;
namespace HorizonteViagens.Negocio;
public static class Precificacao
{
    public static Orcamento Calcular(Combo combo, int viajantes)
    {
        if(viajantes is < 1 or > 6) throw new RegraDeNegocioException("Escolha de 1 a 6 viajantes adultos.");
        var quartos=(viajantes+1)/2;
        // Valor = transporte e serviços individuais; DiariaQuarto = quarto para até 2 adultos.
        var transporte=decimal.Round(combo.Valor*viajantes,2);
        var hospedagem=decimal.Round(combo.DiariaQuarto*combo.Noites*quartos,2);
        var experiencias=decimal.Round(combo.ValorExperiencias*viajantes,2);
        return new(combo.Id,viajantes,quartos,combo.Noites,transporte,hospedagem,experiencias,transporte+hospedagem+experiencias);
    }
}
