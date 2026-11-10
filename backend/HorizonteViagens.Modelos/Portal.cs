// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
using System.ComponentModel.DataAnnotations;
namespace HorizonteViagens.Modelos;

public class Favorito
{
    public int ClienteId { get; set; }
    public int ComboId { get; set; }
}
public class DocumentoViagem
{
    public int Id { get; set; }
    public int CompraId { get; set; }
    public string Tipo { get; set; } = "Voucher";
    public string Titulo { get; set; } = "";
    public string Url { get; set; } = "";
    public string Codigo { get; set; } = "";
    public DateTime EmitidoEm { get; set; } = DateTime.UtcNow;
}
public class ReservaEntrada
{
    [Range(1,int.MaxValue)] public int ComboId { get; set; }
    [Range(1,6)] public int Viajantes { get; set; } = 1;
    public Guid RequisicaoId { get; set; }
    // Limites usam ponto decimal independentemente da cultura pt-BR da requisição.
    [Range(typeof(decimal),"0.01","999999999", ParseLimitsInInvariantCulture=true)] public decimal TotalEsperado { get; set; }
}
public class DocumentoEntrada
{
    [Required, StringLength(40)] public string Tipo { get; set; } = "Voucher";
    [Required, StringLength(140)] public string Titulo { get; set; } = "";
    [Required, StringLength(1000), Url] public string Url { get; set; } = "";
    [Required, StringLength(100)] public string Codigo { get; set; } = "";
}
public record Orcamento(int ComboId, int Viajantes, int Quartos, int Noites, decimal Transporte, decimal Hospedagem, decimal Experiencias, decimal Total);
