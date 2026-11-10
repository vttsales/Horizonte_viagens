// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
namespace HorizonteViagens.Modelos.Dtos;

// DTO de SAÍDA: o que a vitrine e o aplicativo recebem de um combo.
// Não leva as navegações do EF (Compras, Itens) nem o RowVersion.
public class ComboDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public int Vagas { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public decimal Valor { get; set; }

    public string Origem { get; set; } = "São Paulo";
    public string Evento { get; set; } = "";
    public string Descricao { get; set; } = "";
    public string Inclusoes { get; set; } = "";
    public string Imagem { get; set; } = "beach";
    public int Noites { get; set; } = 1;
    public decimal DiariaQuarto { get; set; } = 0;
    public decimal ValorExperiencias { get; set; } = 0;
    public DateOnly? Partida { get; set; } = null;
    public bool Publicado { get; set; } = false;
    public bool PrecoConfirmado { get; set; } = false;
    public string? Versao { get; set; }

    // Mesmo formato do ClienteDto.De: converte a entidade no que pode sair do sistema.
    public static ComboDto De(Combo c) => new()
    {
        Id = c.Id,
        Nome = c.Nome,
        Destino = c.Destino,
        Vagas = c.Vagas,
        Categoria = c.Categoria,
        Origem = c.Origem,
        Evento = c.Evento,
        Descricao = c.Descricao,
        Inclusoes = c.Inclusoes,
        Imagem = c.Imagem,
        Noites = c.Noites,
        DiariaQuarto = c.DiariaQuarto,
        ValorExperiencias = c.ValorExperiencias,
        Partida = c.Partida,
        Publicado = c.Publicado,
        PrecoConfirmado = c.PrecoConfirmado,
        Versao = Convert.ToBase64String(c.RowVersion ?? []),
        Valor = c.Valor
    };
}
