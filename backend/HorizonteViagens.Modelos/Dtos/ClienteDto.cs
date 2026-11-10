// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
using System.ComponentModel.DataAnnotations;

namespace HorizonteViagens.Modelos.Dtos;

// DTO de SAÍDA: o que o sistema MOSTRA de um cliente.
// DTO = Data Transfer Object, um objeto só para transportar dados.
//   - A Api devolve este objeto como JSON para o aplicativo Flutter.
//   - A Web usa este objeto nas telas de listagem e de exclusão.
// Por que não usar a entidade Cliente direto? Porque aí qualquer coluna nova
// na tabela vazaria para o app e para as telas. O DTO escolhe o que sai.
public class ClienteDto
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "CPF")]
    public string Cpf { get; set; } = string.Empty;

    [Display(Name = "Celular")]
    public string Celular { get; set; } = string.Empty;

    public string? Cep { get; set; }
    public string? Rua { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }

    // Repare: não existe Senha nem SenhaHash aqui. O que não está no DTO
    // nunca sai para a tela nem para o app.

    [Display(Name = "Cadastrado em")]
    public DateTime DataCadastro { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; }

    // Converte a entidade (que vem do Negocio) no DTO (que vai para a tela ou o app).
    public static ClienteDto De(Cliente c) => new()
    {
        Id = c.Id,
        Nome = c.Nome,
        Email = c.Email,
        Cpf = c.Cpf,
        Celular = c.Celular,
        Cep = c.Cep,
        Rua = c.Rua,
        Bairro = c.Bairro,
        Cidade = c.Cidade,
        DataCadastro = c.DataCadastro,
        Ativo = c.Ativo
    };
}
