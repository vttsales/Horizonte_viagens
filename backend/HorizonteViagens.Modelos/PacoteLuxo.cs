// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
namespace HorizonteViagens.Modelos;//'namespace' precisa ser usado pois identifica a classe através de seu "sobrenome" que é HorizonteViagens.Modelos. O primeiro nome seria o PacoteLuxo.

//'Viagem' é a classe mãe.
//'PacoteLuxo' é a classe filha.
public class PacoteLuxo : Viagem
{
    //Propriedade exclusiva do pacote de luxo.
    public string Atracao { get; set; } = string.Empty;
    //'base' faz com que a classe filha 'PacoteLuxo' herde caracteristicas da classe mãe.
    public PacoteLuxo(string destino, decimal preco, int duracaoDias, int vagas, string atracao) : base(destino, preco, duracaoDias, vagas)
    {
        Atracao = atracao;
    }

    //'override' sobrescreve o método 'Resumo()' da classe mãe,
    // expandindo-o com as informações exclusivas do 'PacoteLuxo' -> Atração.
    public override string Resumo() => $"{base.Resumo()} · Atração: {Atracao}";

    // A categoria deste pacote (polimorfismo: cada filha responde a sua).
    public override string Categoria => "Luxo";
}

//Console.WriteLine("\n===PACOTE DE LUXO==="); --> Foi removido, pois os dados estão separados da exibição, por isso a classe não escreve mais no console.