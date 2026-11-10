using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using HorizonteViagens.Negocio;
using HorizonteViagens.Repositorio;
using HorizonteViagens.Repositorio.Contexto;
using HorizonteViagens.Repositorio.Interfaces;

// ALTERAÇÃO PIM: formatos monetários e datas consistentes no site.
var builder = WebApplication.CreateBuilder(args);

// A connection string vem do appsettings.json — nunca do código-fonte.
var connectionString = builder.Configuration.GetConnectionString("Padrao")
    ?? throw new InvalidOperationException("Connection string 'Padrao' não encontrada no appsettings.json.");

// ---------- Injeção de dependência ----------
// Esta é a LISTA DE MONTAGEM do sistema. Cada linha ensina o ASP.NET Core a
// criar uma peça. Depois, quando um controller pede um ClienteNegocio no
// construtor, o ASP.NET monta a cadeia inteira sozinho:
//
//   ClientesController  pede  ClienteNegocio
//   ClienteNegocio      pede  IClienteRepositorio  -> recebe um ClienteRepositorio
//   ClienteRepositorio  pede  AppDbContext         -> recebe com a connection string
//
// Ninguém escreve "new" para essas classes. Explicação passo a passo, com os
// erros mais comuns: INJECAO-DE-DEPENDENCIA.md, na pasta da solution.
//
// ATENÇÃO: este bloco é IGUAL no Program.cs da Api. Mudou aqui, mude lá também.

// 1) O banco: "quando alguém pedir um AppDbContext, crie um usando SQL Server
//    e esta connection string".
builder.Services.AddDbContext<AppDbContext>(opcoes => opcoes.UseSqlServer(connectionString));

// 2) O repositório: "quando alguém pedir um IClienteRepositorio (a interface),
//    entregue um ClienteRepositorio (a classe)".
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();

// 3) O negócio: "quando alguém pedir um ClienteNegocio, crie um".
builder.Services.AddScoped<ClienteNegocio>();

// 4) O administrador: repositório e negócio, as mesmas duas linhas de sempre.
builder.Services.AddScoped<IAdministradorRepositorio, AdministradorRepositorio>();
builder.Services.AddScoped<AdministradorNegocio>();
// Reuso do mesmo catálogo de combos na aplicação MVC.
builder.Services.AddScoped<IComboRepositorio, ComboRepositorio>();
builder.Services.AddScoped<ComboNegocio>();
builder.Services.AddScoped<ICompraRepositorio, CompraRepositorio>();
builder.Services.AddScoped<CompraNegocio>();

// AddScoped = um objeto novo para cada requisição (cada clique na Web, cada chamada
// da API), jogado fora quando a resposta sai. O AddDbContext funciona do mesmo jeito:
// cada requisição ganha o seu contexto, e a conexão é fechada quando ele é descartado.

// Cada entidade nova ganha duas linhas aqui, uma de repositório e uma de negócio:
// builder.Services.AddScoped<IIngressoRepositorio, IngressoRepositorio>();
// builder.Services.AddScoped<IngressoNegocio>();
// --------------------------------------------

builder.Services.AddControllersWithViews(o=>o.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute()));

// Health check: o endereço /health responde se o site está de pé.
builder.Services.AddHealthChecks();

// ---------- Login do administrador (cookie) ----------
// Depois que o administrador acerta e-mail e senha, o navegador recebe um
// "crachá" (cookie). A cada clique, o crachá volta junto, e o ASP.NET sabe
// quem é. Quem não tem crachá e tenta abrir uma tela com [Authorize] é
// mandado para a tela de login.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opcoes =>
    {
        opcoes.LoginPath = "/Conta/Login";          // sem crachá: vai para cá
        opcoes.AccessDeniedPath = "/Conta/Negado";  // crachá sem permissão: vai para cá
        opcoes.ExpireTimeSpan = TimeSpan.FromHours(8);
        opcoes.SlidingExpiration = true;
        opcoes.Cookie.HttpOnly=true;
        opcoes.Cookie.SameSite=SameSiteMode.Lax;
        opcoes.Cookie.SecurePolicy=builder.Environment.IsDevelopment()?CookieSecurePolicy.SameAsRequest:CookieSecurePolicy.Always;
        // ALTERAÇÃO PIM: cookies não mantêm acesso depois de expiração/desativação da conta.
        opcoes.Events.OnValidatePrincipal=context=>{
          var usuario=context.Principal!;var id=int.TryParse(usuario.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,out var n)?n:0;
          var db=context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
          HorizonteViagens.Modelos.Usuario? conta=usuario.IsInRole("Administrador")?db.Administradores.Find(id):usuario.IsInRole("Cliente")?db.Clientes.Find(id):null;
          if(conta==null||!conta.Ativo||conta.AcessoAte<=DateTime.UtcNow)context.RejectPrincipal();
          return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();
// ------------------------------------------------------

// ALTERAÇÃO PIM: serviços compartilhados do portal e pagamento apenas sandbox.
builder.Services.AddScoped<PortalRepositorio>();
builder.Services.AddScoped<PortalNegocio>();
builder.Services.AddSingleton(new PagamentoConfiguracao(builder.Configuration["Pagamento:Token"]??"",builder.Configuration["Pagamento:Retorno"]??""));
builder.Services.AddHttpClient<PagamentoNegocio>(c=>c.Timeout=TimeSpan.FromSeconds(20));
builder.Services.AddControllers(options=>options.Filters.Add<HorizonteViagens.Web.Filtros.ErrosFiltro>());
// ALTERAÇÃO PIM: limite por IP para dificultar tentativas repetidas de senha.
builder.Services.AddRateLimiter(o=>{
 o.RejectionStatusCode=429;
 o.AddPolicy("login",context=>System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString()??"local",_=>new System.Threading.RateLimiting.FixedWindowRateLimiterOptions{PermitLimit=20,Window=TimeSpan.FromMinutes(1),QueueLimit=0}));
});
var app = builder.Build();
// ALTERAÇÃO PIM: preparar banco é operação explícita, nunca executada em um acesso comum.
if(args.Contains("--preparar-banco"))
{
    using var scope=app.Services.CreateScope();
    var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await HorizonteViagens.Negocio.Preparacao.Inicializar(db,args.Contains("--acessos-teste"),builder.Environment.IsDevelopment());
    return;
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// ALTERAÇÃO PIM: cultura é aplicada antes de model binding e renderização Razor.
app.UseRequestLocalization(new RequestLocalizationOptions().SetDefaultCulture("pt-BR").AddSupportedCultures("pt-BR").AddSupportedUICultures("pt-BR"));
app.UseHttpsRedirection();
app.UseRouting();

// A ordem importa: primeiro descobre QUEM é (UseAuthentication),
// depois confere se PODE entrar (UseAuthorization).
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    ;


app.MapHealthChecks("/health");

app.Run();

// ALTERAÇÃO PIM: permite testes HTTP com WebApplicationFactory.
public partial class Program { }
