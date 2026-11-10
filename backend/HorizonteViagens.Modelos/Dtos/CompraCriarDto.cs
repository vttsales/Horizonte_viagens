// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
using System.ComponentModel.DataAnnotations;

namespace HorizonteViagens.Modelos.Dtos;

// O cliente escolhe apenas o combo; sua identidade é obtida do token, nunca do JSON.
public class CompraCriarDto
{
    [Range(1, int.MaxValue)] public int ComboId { get; set; }
}
