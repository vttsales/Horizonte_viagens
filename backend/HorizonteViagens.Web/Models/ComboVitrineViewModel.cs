// ALTERAÇÃO PIM: Arquivo de integração/configuração; consulte o resumo por componente.
namespace HorizonteViagens.Web.Models;

// ViewModel dedicado à vitrine MVC, com somente os campos que a tela exibe.
public class ComboVitrineViewModel
{
    public string? DestinoPesquisado { get; init; }
    public IReadOnlyList<ComboVitrineItemViewModel> Combos { get; init; } = [];
}

public class ComboVitrineItemViewModel
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Destino { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public int Vagas { get; init; }
    public decimal Valor { get; init; }
}
