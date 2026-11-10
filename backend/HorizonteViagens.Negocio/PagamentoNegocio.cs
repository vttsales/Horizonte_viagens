// ALTERAÇÃO PIM: Checkout exclusivamente sandbox, referência/valor do servidor e consulta autenticada do pagamento.
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using HorizonteViagens.Modelos;
using HorizonteViagens.Repositorio;
namespace HorizonteViagens.Negocio;
public record PagamentoConfiguracao(string Token,string Retorno);
// Integração exclusivamente sandbox, sem receber números de cartão ou chaves Pix.
public class PagamentoNegocio(HttpClient http,PagamentoConfiguracao config,PortalNegocio portal,PortalRepositorio repo)
{
    public bool Configurado=>!string.IsNullOrWhiteSpace(config.Token);
    private async Task<JsonElement> Chamar(HttpMethod method,string caminho,object? body=null)
    {
        if(!Configurado)throw new RegraDeNegocioException("Pagamento indisponível. Configure a conta de testes do Mercado Pago.");
        using var req=new HttpRequestMessage(method,"https://api.mercadopago.com"+caminho);
        req.Headers.Authorization=new AuthenticationHeaderValue("Bearer",config.Token);
        if(body!=null)req.Content=JsonContent.Create(body);
        using var response=await http.SendAsync(req);
        if(!response.IsSuccessStatusCode)throw new RegraDeNegocioException("O serviço de pagamento não concluiu a solicitação. Tente novamente.");
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }
    public async Task<string> Iniciar(int compraId,int clienteId)
    {
        var c=portal.Reserva(compraId,clienteId);
        if(c.Status!="Pendente" || c.ExpiraEm<=DateTime.UtcNow)throw new RegraDeNegocioException("Esta reserva não está disponível para pagamento.");
        if(c.PagamentoUrl!=null)return c.PagamentoUrl;
        // Recusa uma conta real mesmo que uma credencial produtiva seja configurada por engano.
        var usuario=await Chamar(HttpMethod.Get,"/users/me");
        if(!usuario.TryGetProperty("tags",out var tags)||!tags.EnumerateArray().Any(t=>t.GetString()=="test_user"))throw new RegraDeNegocioException("Use somente uma conta vendedora de teste do Mercado Pago.");
        // Retorno é opcional: no uso local o cliente volta à reserva e consulta o pagamento.
        var preferencia=new Dictionary<string,object>{
            ["external_reference"]=$"HV-{c.Id}-{c.RequisicaoId:N}",
            ["items"]=new[]{new{id=c.ComboId.ToString(),title=c.NomePacote,quantity=1,currency_id="BRL",unit_price=c.Valor}},
            ["expires"]=true,["expiration_date_to"]=c.ExpiraEm.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
        };
        if(!string.IsNullOrWhiteSpace(config.Retorno)){
            if(!Uri.TryCreate(config.Retorno,UriKind.Absolute,out var retorno)||retorno.Scheme!="https")throw new RegraDeNegocioException("O retorno opcional precisa usar HTTPS.");
            preferencia["back_urls"]=new{success=config.Retorno,pending=config.Retorno,failure=config.Retorno};
        }
        var result=await Chamar(HttpMethod.Post,"/checkout/preferences",preferencia);
        var link=result.GetProperty("sandbox_init_point").GetString();
        if(!Uri.TryCreate(link,UriKind.Absolute,out var uri)||uri.Scheme!="https"||!(uri.Host.Equals("mercadopago.com.br",StringComparison.OrdinalIgnoreCase)||uri.Host.EndsWith(".mercadopago.com.br",StringComparison.OrdinalIgnoreCase)))throw new RegraDeNegocioException("Link de pagamento inválido.");
        if(c.ExpiraEm<=DateTime.UtcNow)throw new RegraDeNegocioException("A reserva expirou. Faça uma nova reserva.");
        c.PreferenciaId=result.GetProperty("id").GetString();c.PagamentoUrl=link;repo.Salvar();return link!;
    }
    public async Task<Compra> Verificar(int compraId,int clienteId)
    {
        var c=portal.Reserva(compraId,clienteId);
        var referencia=$"HV-{c.Id}-{c.RequisicaoId:N}";
        var result=await Chamar(HttpMethod.Get,"/v1/payments/search?sort=date_created&criteria=desc&external_reference="+Uri.EscapeDataString(referencia));
        foreach(var payment in result.GetProperty("results").EnumerateArray())
        {
            var status=payment.GetProperty("status").GetString()!;
            if(status is not ("approved" or "refunded" or "charged_back"))continue;
            repo.AplicarPagamento(c.Id,payment.GetProperty("id").ToString(),status,payment.GetProperty("transaction_amount").GetDecimal(),payment.GetProperty("external_reference").GetString()!,!payment.GetProperty("live_mode").GetBoolean());
        }
        return portal.Reserva(compraId,clienteId);
    }
}
