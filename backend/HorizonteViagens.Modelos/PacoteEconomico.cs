// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
namespace HorizonteViagens.Modelos;//'namespace' precisa ser usado pois identifica a classe através de seu "sobrenome" que é HorizonteViagens.Modelos. O primeiro nome seria o PacoteEconomico.

//'Viagem' é a classe mãe.
//'PacoteEconomico' é a classe filha.
public class PacoteEconomico : Viagem
{
    //Propriedade exclusiva do tipo economico.
    public string TipoAcomodacao { get; set; } = string.Empty;
    //'base' repassa os parâmetros comuns para o construtor da classe mãe 'Viagem'.
    public PacoteEconomico(string destino, decimal preco, int duracaoDias, int vagas, string tipoAcomodacao) : base(destino, preco, duracaoDias, vagas)
    {
        TipoAcomodacao = tipoAcomodacao;
    }

    //'public' pois será usada de fora da classe, como pelas telas 
    //'override' sobrescreve o método 'Resumo()' da classe mãe,
    // 'Resumo()' nome do metodo criado
    // acrescentando a informação exclusiva do 'PacoteEconomico' -> Acomodação.
    public override string Resumo() => $"{base.Resumo()} · Acomodação: {TipoAcomodacao}";

    // A categoria deste pacote (polimorfismo: cada filha responde a sua).
    public override string Categoria => "Economico";

}