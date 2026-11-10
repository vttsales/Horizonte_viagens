// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
namespace HorizonteViagens.Modelos.Dtos;

// DTO de SAÍDA da compra, para o aplicativo e para a tela administrativa.
public class CompraDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public int ComboId { get; set; }
    public string ComboNome { get; set; } = string.Empty;
    public decimal Valor { get; set; }

    // Cliente e Combo podem não vir carregados (logo depois de comprar, por exemplo);
    // nesse caso os nomes ficam vazios, e os Ids continuam lá.
    public static CompraDto De(Compra c) => new()
    {
        Id = c.Id,
        ClienteId = c.ClienteId,
        ClienteNome = c.Cliente?.Nome ?? string.Empty,
        ComboId = c.ComboId,
        ComboNome = c.Combo?.Nome ?? string.Empty,
        Valor = c.Valor
    };
}
