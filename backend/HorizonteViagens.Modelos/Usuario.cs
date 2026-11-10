using System.ComponentModel.DataAnnotations;

namespace HorizonteViagens.Modelos;

// Classe MÃE de todo mundo que tem cadastro no sistema.
// 'abstract' porque ninguém é "só um usuário": ou é Cliente (o viajante),
// ou é Administrador (a agência). É a mesma ideia da classe Viagem,
// que nunca existe sozinha, só como PacoteLuxo ou PacoteEconomico.
public abstract class Usuario
{
    // ALTERAÇÃO PIM: contas acadêmicas opcionais expiram; contas regulares deixam o campo vazio.
    public DateTime? AcessoAte { get; set; }

    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required, StringLength(150), EmailAddress]
    public string Email { get; set; } = string.Empty;

    // ENCAPSULAMENTO: a senha digitada nunca é guardada.
    // O que fica no banco é o "hash", um resumo que não dá para desfazer.
    // Quem gera o hash é a camada Negocio (classe SenhaHasher).
    public string SenhaHash { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    // POLIMORFISMO: cada classe filha responde do seu jeito.
    // É usado no login para dizer ao ASP.NET qual é o perfil de quem entrou.
    // Não vira coluna no banco (ver o Ignore no AppDbContext).
    public abstract string Perfil { get; }
}
