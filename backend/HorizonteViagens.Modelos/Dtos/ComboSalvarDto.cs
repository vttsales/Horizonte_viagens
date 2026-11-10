// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
using System.ComponentModel.DataAnnotations;

namespace HorizonteViagens.Modelos.Dtos;

// DTO de ENTRADA: o que o administrador PREENCHE para cadastrar ou alterar um combo.
// Não tem Id: na alteração, o Id vem da URL. Mesmo desenho do ClienteSalvarDto.
public class ComboSalvarDto
{
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Destino")]
    [Required(ErrorMessage = "Informe o destino.")]
    [StringLength(100, ErrorMessage = "O destino pode ter no máximo 100 caracteres.")]
    public string Destino { get; set; } = string.Empty;

    [Display(Name = "Vagas")]
    [Range(0, int.MaxValue, ErrorMessage = "As vagas não podem ser negativas.")]
    public int Vagas { get; set; }

    [Display(Name = "Categoria")]
    [Required(ErrorMessage = "Informe a categoria.")]
    [StringLength(30, ErrorMessage = "A categoria pode ter no máximo 30 caracteres.")]
    public string Categoria { get; set; } = string.Empty;

    [Display(Name = "Valor")]
    [Range(typeof(decimal), "0", "99999999", ErrorMessage = "O valor não pode ser negativo.")]
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

    public Combo ParaCombo(int id = 0) => new()
    {
        Id = id,
        Nome = Nome,
        Destino = Destino,
        Vagas = Vagas,
        Categoria = Categoria,
        Origem = Origem,
        Evento = Evento,
        Descricao = Descricao,
        Inclusoes = Inclusoes,
        Imagem = Imagem,
        Noites = Noites,
        DiariaQuarto = DiariaQuarto,
        ValorExperiencias = ValorExperiencias,
        Partida = Partida,
        Publicado = Publicado,
        PrecoConfirmado = PrecoConfirmado,
        RowVersion = string.IsNullOrWhiteSpace(Versao) ? [] : Convert.FromBase64String(Versao),
        Valor = Valor
    };

    public static ComboSalvarDto De(Combo c) => new()
    {
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
