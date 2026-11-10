// ALTERAÇÃO PIM: ponte HTTP local para o mesmo MVC, com o banco efêmero dos testes.
// Serve para validar interface no PC sem SQL. Nunca é o modo de execução da entrega.
using HorizonteViagens.Testes;
using HorizonteViagens.Repositorio.Contexto;
using HorizonteViagens.Negocio;
using Microsoft.Extensions.DependencyInjection;
using var factory = new AplicacaoTeste<HorizonteViagens.Web.Controllers.HomeController>();
using(var scope=factory.Services.CreateScope()){
 var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();db.Database.EnsureCreated();
 await Preparacao.Inicializar(db,true,true);
}
using var client=factory.CreateClient(new(){AllowAutoRedirect=false,BaseAddress=new Uri("http://localhost:5157")});
var builder=WebApplication.CreateBuilder(args);builder.WebHost.UseUrls("http://127.0.0.1:5157");var app=builder.Build();
app.Run(async context=>{
 using var req=new HttpRequestMessage(new HttpMethod(context.Request.Method),context.Request.Path+context.Request.QueryString);
 if(context.Request.ContentLength>0 || context.Request.Headers.ContainsKey("Transfer-Encoding"))req.Content=new StreamContent(context.Request.Body);
 foreach(var h in context.Request.Headers){if(h.Key.Equals("Host",StringComparison.OrdinalIgnoreCase))continue;if(!req.Headers.TryAddWithoutValidation(h.Key,h.Value.ToArray()))req.Content?.Headers.TryAddWithoutValidation(h.Key,h.Value.ToArray());}
 using var res=await client.SendAsync(req);context.Response.StatusCode=(int)res.StatusCode;
 foreach(var h in res.Headers)context.Response.Headers[h.Key]=h.Value.ToArray();
 foreach(var h in res.Content.Headers)context.Response.Headers[h.Key]=h.Value.ToArray();
 context.Response.Headers.Remove("transfer-encoding");await res.Content.CopyToAsync(context.Response.Body);
});
await app.RunAsync();
