// ALTERAÇÃO PIM: Arquivo de integração/configuração; consulte o resumo por componente.
namespace HorizonteViagens.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
