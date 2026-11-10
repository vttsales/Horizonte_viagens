// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
using System.ComponentModel.DataAnnotations;

namespace HorizonteViagens.Modelos.Dtos;

// DTO de ENTRADA: o que o usuário PREENCHE para cadastrar ou alterar um cliente.
//   - A Api recebe este objeto no corpo do POST e do PUT.
//   - A Web usa este objeto nos formulários de Criar e Editar.
// Não tem Id, DataCadastro nem DataAtualizacao: quem decide esses valores é o
// sistema, não o usuário. Se o DTO não tem o campo, ninguém consegue forjá-lo.
// As mensagens abaixo aparecem tanto embaixo do campo na tela quanto no 400 da Api.
public class ClienteSalvarDto
{
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
    // A mesma regex do método ApenasLetras() da versão em console.
    [RegularExpression(@"^[a-zA-ZáàâãéèêíïóôõöúçñÁÀÂÃÉÈÊÍÏÓÔÕÖÚÇÑ\s]+$",
        ErrorMessage = "O nome deve conter apenas letras.")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "Informe o e-mail.")]
    [StringLength(150, ErrorMessage = "O e-mail pode ter no máximo 150 caracteres.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    // O usuário pode digitar com ponto e traço; o Negocio limpa e confere os 11 dígitos.
    [Display(Name = "CPF")]
    [Required(ErrorMessage = "Informe o CPF.")]
    public string Cpf { get; set; } = string.Empty;

    // O usuário pode digitar "(16) 9.8899-4455"; o Negocio limpa e confere os 11 dígitos.
    [Display(Name = "Celular")]
    [Required(ErrorMessage = "Informe o celular.")]
    public string Celular { get; set; } = string.Empty;

    [Display(Name = "CEP")]
    [StringLength(9)]
    public string? Cep { get; set; }

    [Display(Name = "Rua")]
    [StringLength(100)]
    public string? Rua { get; set; }

    [Display(Name = "Bairro")]
    [StringLength(50)]
    public string? Bairro { get; set; }

    [Display(Name = "Cidade")]
    [StringLength(50)]
    public string? Cidade { get; set; }

    // Não é [Required] aqui de propósito:
    //   - no CADASTRO, o ClienteNegocio exige a senha e confere se ela é forte;
    //   - na EDIÇÃO, vazio quer dizer "manter a senha atual".
    [Display(Name = "Senha")]
    [DataType(DataType.Password)]
    public string? Senha { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    // Converte o que foi digitado na entidade que o Negocio entende.
    // A senha NÃO vai para a entidade: o Negocio recebe a senha separada,
    // valida e guarda só o hash.
    public Cliente ParaCliente(int id = 0) => new()
    {
        Id = id,
        Nome = Nome,
        Email = Email,
        Cpf = Cpf,
        Celular = Celular,
        Cep = Cep,
        Rua = Rua,
        Bairro = Bairro,
        Cidade = Cidade,
        Ativo = Ativo
    };

    // Caminho inverso: preenche o formulário de edição com os dados atuais.
    // A senha fica de fora: ela nunca volta para a tela.
    public static ClienteSalvarDto De(Cliente c) => new()
    {
        Nome = c.Nome,
        Email = c.Email,
        Cpf = c.Cpf,
        Celular = c.Celular,
        Cep = c.Cep,
        Rua = c.Rua,
        Bairro = c.Bairro,
        Cidade = c.Cidade,
        Ativo = c.Ativo
    };
}
