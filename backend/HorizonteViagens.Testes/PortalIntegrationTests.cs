// ALTERAÇÃO PIM: exercita controllers, Razor, autorização, banco relacional e cálculo sem serviços pagos.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using HorizonteViagens.Modelos;
using HorizonteViagens.Negocio;
using HorizonteViagens.Repositorio;
using HorizonteViagens.Repositorio.Contexto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;
namespace HorizonteViagens.Testes;

// SQLite é somente o banco dos testes. O projeto Web/Api mantém SQL Server e suas migrations.
public class BancoTeste(DbContextOptions<AppDbContext> options):AppDbContext(options)
{
 protected override void OnModelCreating(ModelBuilder m){base.OnModelCreating(m);m.Entity<Cliente>().Property(c=>c.DataCadastro).HasDefaultValueSql("CURRENT_TIMESTAMP");m.Entity<Combo>().Property(c=>c.RowVersion).ValueGeneratedNever();m.Entity<Compra>().Property(c=>c.RowVersion).ValueGeneratedNever();}
 public override int SaveChanges(){foreach(var e in ChangeTracker.Entries<Combo>().Where(e=>e.State is EntityState.Added or EntityState.Modified))e.Entity.RowVersion=Guid.NewGuid().ToByteArray();foreach(var e in ChangeTracker.Entries<Compra>().Where(e=>e.State is EntityState.Added or EntityState.Modified))e.Entity.RowVersion=Guid.NewGuid().ToByteArray();return base.SaveChanges();}
 public override Task<int> SaveChangesAsync(CancellationToken c=default)=>Task.FromResult(SaveChanges());
}
public class AplicacaoTeste<T>:WebApplicationFactory<T> where T:class
{
 private readonly SqliteConnection conn=new("Data Source=:memory:");
 public AplicacaoTeste(){conn.Open();}
 protected override void ConfigureWebHost(IWebHostBuilder b){b.UseEnvironment("Development");b.ConfigureServices(s=>{
  foreach(var d in s.Where(d=>d.ServiceType==typeof(AppDbContext)||d.ServiceType==typeof(DbContextOptions<AppDbContext>)||d.ServiceType.Name.StartsWith("IDbContextOptionsConfiguration")).ToList())s.Remove(d);
  s.AddScoped<AppDbContext>(_=>new BancoTeste(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(conn).Options));
 });}
 public void Preparar(){using var scope=Services.CreateScope();var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();db.Database.EnsureCreated();
 db.Clientes.Add(new Cliente{Nome="Cliente Teste",Email="cliente@teste.invalid",Cpf="52998224725",Celular="11999999999",SenhaHash=SenhaHasher.Gerar("TesteSeguro!2026"),Ativo=true});
 db.Combos.Add(new Combo{Nome="Ubatuba + experiências",Destino="Ubatuba",Evento="Surf",Categoria="Econômico",Imagem="coast",Valor=300,DiariaQuarto=550,ValorExperiencias=350,Noites=6,Vagas=10,Publicado=true,PrecoConfirmado=true,Partida=DateOnly.FromDateTime(DateTime.Today.AddDays(30))});db.SaveChanges();}
 protected override void Dispose(bool disposing){base.Dispose(disposing);if(disposing)conn.Dispose();}
}
public class PortalIntegrationTests
{
 [Theory][InlineData(1,1,3950)][InlineData(2,1,4600)][InlineData(3,2,8550)][InlineData(6,3,13800)]
 public void PrecoPorGrupo_ConsideraQuartosInteiros(int pessoas,int quartos,decimal esperado){var c=new Combo{Valor=300,DiariaQuarto=550,ValorExperiencias=350,Noites=6};var o=Precificacao.Calcular(c,pessoas);Assert.Equal(quartos,o.Quartos);Assert.Equal(esperado,o.Total);}
 [Theory][InlineData(0)][InlineData(7)]public void Preco_RecusaQuantidadeInvalida(int n)=>Assert.Throws<RegraDeNegocioException>(()=>Precificacao.Calcular(new Combo(),n));
 [Fact]public async Task Api_ReservaIdempotente_TotalAutoritativo_Estoque()
 {
  using var f=new AplicacaoTeste<HorizonteViagens.Api.Controllers.PortalController>();f.Preparar();using var c=f.CreateClient(new(){BaseAddress=new Uri("https://localhost")});await Login(c);
  var entrada=new ReservaEntrada{ComboId=1,Viajantes=3,TotalEsperado=1,RequisicaoId=Guid.NewGuid()};
  Assert.Equal(HttpStatusCode.BadRequest,(await c.PostAsJsonAsync("/api/portal/reservas",entrada)).StatusCode);
  entrada.TotalEsperado=8550;
  var a=await c.PostAsJsonAsync("/api/portal/reservas",entrada);Assert.Equal(HttpStatusCode.OK,a.StatusCode);var id=(await a.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt32();
  var b=await c.PostAsJsonAsync("/api/portal/reservas",entrada);Assert.Equal(id,(await b.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt32());
  using var scope=f.Services.CreateScope();var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();Assert.Equal(7,db.Combos.Single().Vagas);Assert.Single(db.Compras);
  entrada.Viajantes=2;Assert.Equal(HttpStatusCode.BadRequest,(await c.PostAsJsonAsync("/api/portal/reservas",entrada)).StatusCode);
 }
 [Fact]public async Task Api_ClienteNaoAdmin_NemReservaDeOutro()
 {
  using var f=new AplicacaoTeste<HorizonteViagens.Api.Controllers.PortalController>();f.Preparar();using var c=f.CreateClient(new(){BaseAddress=new Uri("https://localhost")});
  Assert.Equal(HttpStatusCode.Unauthorized,(await c.GetAsync("/api/portal/reservas")).StatusCode);await Login(c);
  Assert.Equal(HttpStatusCode.Forbidden,(await c.GetAsync("/api/clientes")).StatusCode);
  Assert.Equal(HttpStatusCode.BadRequest,(await c.GetAsync("/api/portal/reservas/987/documentos")).StatusCode);
  Assert.Equal(HttpStatusCode.Forbidden,(await c.PostAsJsonAsync("/api/portal/documentos/1",new DocumentoEntrada{Titulo="Teste",Url="https://example.invalid",Codigo="XX"})).StatusCode);
  await c.PutAsync("/api/portal/favoritos/1",null);await c.PutAsync("/api/portal/favoritos/1",null);Assert.Equal(new[]{1},await c.GetFromJsonAsync<int[]>("/api/portal/favoritos"));
 }
 [Fact]public async Task Api_ContaDesativada_InvalidaTokenEmitido()
 {
  using var f=new AplicacaoTeste<HorizonteViagens.Api.Controllers.PortalController>();f.Preparar();using var c=f.CreateClient(new(){BaseAddress=new Uri("https://localhost")});await Login(c);
  using(var scope=f.Services.CreateScope()){var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();db.Clientes.Single().Ativo=false;db.SaveChanges();}
  Assert.Equal(HttpStatusCode.Unauthorized,(await c.GetAsync("/api/portal/perfil")).StatusCode);
 }
 [Fact]public async Task Api_CancelamentoDevolveVagasApenasUmaVez()
 {
  using var f=new AplicacaoTeste<HorizonteViagens.Api.Controllers.PortalController>();f.Preparar();using var c=f.CreateClient(new(){BaseAddress=new Uri("https://localhost")});await Login(c);
  var a=await c.PostAsJsonAsync("/api/portal/reservas",new ReservaEntrada{ComboId=1,Viajantes=2,TotalEsperado=4600,RequisicaoId=Guid.NewGuid()});var id=(await a.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt32();
  Assert.Equal(HttpStatusCode.NoContent,(await c.PostAsync($"/api/portal/reservas/{id}/cancelar",null)).StatusCode);
  Assert.Equal(HttpStatusCode.NoContent,(await c.PostAsync($"/api/portal/reservas/{id}/cancelar",null)).StatusCode);
  using var scope=f.Services.CreateScope();Assert.Equal(10,scope.ServiceProvider.GetRequiredService<AppDbContext>().Combos.Single().Vagas);
 }
 [Fact]public async Task Web_Razor_Login_CrudCombo_Csrf()
 {
  using var f=new AplicacaoTeste<HorizonteViagens.Web.Controllers.HomeController>();f.Preparar();using var c=f.CreateClient(new(){AllowAutoRedirect=false,BaseAddress=new Uri("https://localhost")});
  var html=await c.GetStringAsync("/");Assert.Contains("Ubatuba",html);Assert.Contains("4.600",html);
  Assert.Equal(HttpStatusCode.Redirect,(await c.GetAsync("/Combos/Gerenciar")).StatusCode);

  var token=Token(await c.GetStringAsync("/Conta/Login?admin=true"));
  var login=await c.PostAsync("/Conta/Login",new FormUrlEncodedContent(new Dictionary<string,string>{{"__RequestVerificationToken",token},{"Email","admin@horizonteviagens.com"},{"Senha","Horizonte@2026"},{"admin","true"}}));Assert.Equal(HttpStatusCode.Redirect,login.StatusCode);
  Assert.Equal(HttpStatusCode.BadRequest,(await c.PostAsync("/Combos/Criar",new FormUrlEncodedContent(new Dictionary<string,string>()))).StatusCode);
  token=Token(await c.GetStringAsync("/Combos/Criar"));
  var form=new Dictionary<string,string>{{"__RequestVerificationToken",token},{"Nome","Novo pacote"},{"Destino","Recife"},{"Categoria","Econômico"},{"Noites","2"},{"Valor","400,50"},{"DiariaQuarto","200,00"},{"ValorExperiencias","50,00"},{"Vagas","5"},{"Imagem","beach"},{"Origem","São Paulo"},{"Publicado","true"}};
  var criar=await c.PostAsync("/Combos/Criar",new FormUrlEncodedContent(form));Assert.Equal(HttpStatusCode.Redirect,criar.StatusCode);Assert.Contains("Novo pacote",await c.GetStringAsync("/Combos/Gerenciar"));
 }
 [Fact]public void Pagamento_RecusaValorOuModoReal_ExpiracaoNaoConfirma()
 {
  using var f=new AplicacaoTeste<HorizonteViagens.Api.Controllers.PortalController>();f.Preparar();using var scope=f.Services.CreateScope();var p=scope.ServiceProvider.GetRequiredService<PortalNegocio>();var repo=scope.ServiceProvider.GetRequiredService<PortalRepositorio>();var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();
  var c=p.Reservar(1,new ReservaEntrada{ComboId=1,Viajantes=2,TotalEsperado=4600,RequisicaoId=Guid.NewGuid()});var referencia=$"HV-{c.Id}-{c.RequisicaoId:N}";
  Assert.Throws<InvalidOperationException>(()=>repo.AplicarPagamento(c.Id,"42","approved",1,referencia,true));
  Assert.Throws<InvalidOperationException>(()=>repo.AplicarPagamento(c.Id,"42","approved",4600,referencia,false));
  c.ExpiraEm=DateTime.UtcNow.AddMinutes(-1);db.SaveChanges();repo.Expirar();repo.Expirar();Assert.Equal(10,db.Combos.Single().Vagas);
  repo.AplicarPagamento(c.Id,"42","approved",4600,referencia,true);Assert.Equal("Revisão de pagamento",c.Status);
 }
 // ALTERAÇÃO PIM Pages: cobre gestão por API, bloqueio de cliente e concorrência do formulário HTML.
 [Fact] public async Task Api_GestaoPages_ExigeAdmin_E_CrudPreservaVersao()
 {
  using var f=new AplicacaoTeste<HorizonteViagens.Api.Controllers.PortalController>();f.Preparar();using var c=f.CreateClient(new(){BaseAddress=new Uri("https://localhost")});
  await Login(c);Assert.Equal(HttpStatusCode.Forbidden,(await c.GetAsync("/api/portal/gestao/reservas")).StatusCode);
  var login=await c.PostAsJsonAsync("/api/autenticacao/login",new{email="admin@horizonteviagens.com",senha="Horizonte@2026"});login.EnsureSuccessStatusCode();
  c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",(await login.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("token").GetString());
  Assert.Equal(HttpStatusCode.OK,(await c.GetAsync("/api/portal/gestao/reservas")).StatusCode);
  var dto=new HorizonteViagens.Modelos.Dtos.ComboSalvarDto{Nome="Recife",Destino="Recife",Categoria="Econômico",Valor=400,Noites=2,Imagem="beach"};
  var created=await c.PostAsJsonAsync("/api/combos",dto);Assert.Equal(HttpStatusCode.Created,created.StatusCode);
  var item=await created.Content.ReadFromJsonAsync<JsonElement>();var id=item.GetProperty("id").GetInt32();dto.Versao=item.GetProperty("versao").GetString();dto.Nome="Recife atualizado";
  Assert.Equal(HttpStatusCode.NoContent,(await c.PutAsJsonAsync($"/api/combos/{id}",dto)).StatusCode);
  Assert.Equal(HttpStatusCode.BadRequest,(await c.PutAsJsonAsync($"/api/combos/{id}",dto)).StatusCode);
  Assert.Equal(HttpStatusCode.NoContent,(await c.DeleteAsync($"/api/combos/{id}")).StatusCode);
 }
 private static async Task Login(HttpClient c){var r=await c.PostAsJsonAsync("/api/autenticacao/login-cliente",new{email="cliente@teste.invalid",senha="TesteSeguro!2026"});r.EnsureSuccessStatusCode();var token=(await r.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("token").GetString();c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",token);}
 private static string Token(string html)=>WebUtility.HtmlDecode(Regex.Match(html,"name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value);
}
