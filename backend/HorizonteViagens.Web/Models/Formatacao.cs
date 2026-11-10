// ALTERAÇÃO PIM: Arquivo de integração/configuração; consulte o resumo por componente.
namespace HorizonteViagens.Web.Models;

// Máscaras só para EXIBIR na tela. No banco, CPF e celular ficam só com os dígitos.
// Formatar é papel da camada de apresentação (Web), não do Negocio.
// É a mesma lógica do ToString(@"000\.000\.000\-00") da versão em console.
public static class Formatacao
{
    public static string Cpf(string cpf) =>
        cpf.Length == 11 && long.TryParse(cpf, out long n)
            ? n.ToString(@"000\.000\.000\-00")
            : cpf;

    public static string Celular(string celular) =>
        celular.Length == 11 && long.TryParse(celular, out long n)
            ? n.ToString(@"\(00\)\ 0\.0000\-0000")
            : celular;
}
