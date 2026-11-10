// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
namespace HorizonteViagens.Modelos;

//'abstract' é necessário pois o conceito 'Viagem' é genérico: o cliente sempre adquire
// um 'Pacote Econômico' ou um 'Pacote de Luxo', nunca uma "viagem" sozinha.
public abstract class Viagem
{
    // Chave da tabela no banco. Quem gera o número é o próprio banco.
    public int Id { get; set; }

    //'Destino' é uma propriedade da classe mãe 'Viagem'.
    public string Destino { get; set; } = string.Empty;

    //'Preco' agora é 'decimal': para dinheiro, 'double' erra centavos nas contas.
    public decimal Preco { get; set; }

    //'DuracaoDias' é uma propriedade da classe mãe 'Viagem'.
    public int DuracaoDias { get; set; }

    // Quantas vagas o pacote ainda tem. A Reserva vai conferir isso antes de reservar.
    public int Vagas { get; set; }

    /*
    As propriedades são as mesmas tanto para Pacotes de Luxo quanto para Pacotes Econômicos.
    O que muda nas classes filhas é o 'Resumo()' e a 'Categoria'.
    */

    //Construtor responsável pela inicialização das propriedades da classe mãe.
    //Os valores recebidos são passados pelas classes filhas via 'base'.
    //Os nomes dos parâmetros batem com os das propriedades (destino -> Destino):
    //é assim que o Entity Framework consegue usar este construtor ao ler do banco.
    public Viagem(string destino, decimal preco, int duracaoDias, int vagas)
    {
        Destino = destino;
        Preco = preco;
        DuracaoDias = duracaoDias;
        Vagas = vagas;
    }

    //'virtual' permite que as classes filhas SOBRESCREVAM este método via 'override'.
    // Antes ele escrevia no console; agora ele DEVOLVE o texto, e a tela decide onde mostrar.
    public virtual string Resumo() =>
        $"{Destino} · {DuracaoDias} dias · R$ {Preco:N2}";

    //'abstract' obriga cada classe filha a dizer qual é a sua categoria.
    public abstract string Categoria { get; }
}