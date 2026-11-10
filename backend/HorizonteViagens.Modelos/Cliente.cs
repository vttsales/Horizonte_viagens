// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
using System.ComponentModel.DataAnnotations;

namespace HorizonteViagens.Modelos;

// O viajante que se cadastra no Horizonte Viagens.
// HERANÇA: Nome, Email, SenhaHash e Ativo vêm da classe mãe Usuario.
// Aqui ficam só os dados que o administrador não tem.
// A camada Modelos não referencia nenhum outro projeto da solução.
public class Cliente : Usuario
{
    // Só os 11 dígitos, sem ponto nem traço.
    [Required, StringLength(11, MinimumLength = 11)]
    public string Cpf { get; set; } = string.Empty;

    // Só os 11 dígitos (DDD + número), sem parênteses, espaço nem traço.
    [Required, StringLength(11, MinimumLength = 11)]
    public string Celular { get; set; } = string.Empty;

    // Endereço informado na documentação do banco. CEP permanece texto para
    // preservar zeros à esquerda e permitir o formato 00000-000.
    [StringLength(9)]
    public string? Cep { get; set; }

    [StringLength(100)]
    public string? Rua { get; set; }

    [StringLength(50)]
    public string? Bairro { get; set; }

    [StringLength(50)]
    public string? Cidade { get; set; }

    public ICollection<TelefoneCliente> Telefones { get; set; } = new List<TelefoneCliente>();

    public DateTime DataCadastro { get; set; }

    // Preenchida pela trigger TR_Clientes_DataAtualizacao, não pelo C#.
    public DateTime? DataAtualizacao { get; set; }

    public override string Perfil => "Cliente";
}
