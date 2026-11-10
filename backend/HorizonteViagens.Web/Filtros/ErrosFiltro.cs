// ALTERAÇÃO PIM: erros previsíveis não expõem SQL, credenciais ou stack traces.
using HorizonteViagens.Negocio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
namespace HorizonteViagens.Web.Filtros;
public class ErrosFiltro: IExceptionFilter
{
 public void OnException(ExceptionContext c){
 var status=c.Exception is UnauthorizedAccessException?403:c.Exception is RegraDeNegocioException or FormatException?400:c.Exception is DbUpdateException?409:503;
 var texto=c.Exception is RegraDeNegocioException?c.Exception.Message:status==403?"Acesso restrito.":status==409?"Os dados mudaram ou possuem vínculos. Atualize a página e tente novamente.":"Não foi possível concluir. Verifique o serviço e tente novamente.";
 c.Result=new ObjectResult(new{erro=texto}){StatusCode=status};c.ExceptionHandled=true;
 }
}
