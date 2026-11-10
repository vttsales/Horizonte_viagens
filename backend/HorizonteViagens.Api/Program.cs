using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HorizonteViagens.Api;
using HorizonteViagens.Negocio;
using HorizonteViagens.Repositorio;
using HorizonteViagens.Repositorio.Contexto;
using HorizonteViagens.Repositorio.Interfaces;
using Scalar.AspNetCore;

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
// ATENÇÃO: este bloco é IGUAL no Program.cs da Web. Mudou aqui, mude lá também.

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
// Catálogo usado pela vitrine do front-end: mesma implementação para API e Web.
builder.Services.AddScoped<IComboRepositorio, ComboRepositorio>();
builder.Services.AddScoped<ComboNegocio>();
// Compras executam regra transacional no repositório e regra de fluxo no negócio.
builder.Services.AddScoped<ICompraRepositorio, CompraRepositorio>();
builder.Services.AddScoped<CompraNegocio>();

// AddScoped = um objeto novo para cada requisição (cada clique na Web, cada chamada
// da API), jogado fora quando a resposta sai. O AddDbContext funciona do mesmo jeito:
// cada requisição ganha o seu contexto, e a conexão é fechada quando ele é descartado.

// Cada entidade nova ganha duas linhas aqui, uma de repositório e uma de negócio:
// builder.Services.AddScoped<IIngressoRepositorio, IngressoRepositorio>();
// builder.Services.AddScoped<IngressoNegocio>();
// --------------------------------------------

builder.Services.AddControllers();

// Health check: o endereço /health responde se a Api está de pé (Aula 07 de Desenvolvimento Web).
builder.Services.AddHealthChecks();

// ---------- Login do administrador (token JWT) ----------
// A Api não usa cookie, porque quem chama é o app Flutter, não um navegador.
// No lugar do cookie, o login devolve um TOKEN: um texto assinado com a chave
// secreta do appsettings.json. O app manda esse token em toda chamada, no
// cabeçalho "Authorization: Bearer <token>". Se alguém alterar o token,
// a assinatura deixa de bater e a Api recusa (401).
// A CHAVE é um segredo: não fica no appsettings.json, que vai para o Git
// (Aula 07 de Desenvolvimento Web, "Onde cada segredo mora").
// Em desenvolvimento ela mora no User Secrets, fora da pasta do projeto:
//   dotnet user-secrets set "Jwt:Chave" "uma-frase-longa-e-secreta-com-32-caracteres-ou-mais" --project HorizonteViagens.Api
// No servidor, numa variável de ambiente chamada Jwt__Chave.
var jwt = builder.Configuration.GetSection("Jwt");
var chave = jwt["Chave"];
if (string.IsNullOrWhiteSpace(chave))
{
    if (!builder.Environment.IsDevelopment())
        throw new InvalidOperationException(
            "Chave 'Jwt:Chave' não configurada. Defina a variável de ambiente Jwt__Chave no servidor.");

    // Só em desenvolvimento: sem User Secrets configurado, gera uma chave aleatória
    // ao iniciar, para a Api subir em qualquer máquina (inclusive na apresentação).
    // Efeito colateral: ao reiniciar a Api, os tokens antigos deixam de valer
    // e é preciso fazer login de novo.
    chave = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
    builder.Configuration["Jwt:Chave"] = chave; // o GeradorDeToken lê daqui
}

builder.Services.AddSingleton<GeradorDeToken>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        // ALTERAÇÃO PIM: desativação e validade da conta valem também para JWTs já emitidos.
        opcoes.Events = new JwtBearerEvents { OnTokenValidated = context => {
          var usuario=context.Principal!;var id=int.TryParse(usuario.FindFirstValue(ClaimTypes.NameIdentifier),out var n)?n:0;
          var db=context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
          HorizonteViagens.Modelos.Usuario? conta=usuario.IsInRole("Administrador")?db.Administradores.Find(id):usuario.IsInRole("Cliente")?db.Clientes.Find(id):null;
          if(conta==null||!conta.Ativo||conta.AcessoAte<=DateTime.UtcNow)context.Fail("Conta indisponível.");
          return Task.CompletedTask;
        }};
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Emissor"],
            ValidateAudience = true,
            ValidAudience = jwt["Publico"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
            ClockSkew = TimeSpan.FromSeconds(30),
            RoleClaimType = ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization();
// ALTERAÇÃO PIM: somente as origens configuradas podem consumir a API pelo navegador.
var origens=builder.Configuration.GetSection("Cors:Origens").Get<string[]>()??[];
builder.Services.AddCors(o=>o.AddDefaultPolicy(p=>p.WithOrigins(origens).AllowAnyHeader().AllowAnyMethod()));
// ---------------------------------------------------------

// OpenAPI: gera a documentação da API (quais endpoints existem, o que cada um
// recebe e devolve) num arquivo JSON padrão, em /openapi/v1.json.
// É esse documento que o grupo usa para mostrar ao app Flutter o que a API oferece.
builder.Services.AddOpenApi();

// ALTERAÇÃO PIM: serviços compartilhados do portal e pagamento apenas sandbox.
builder.Services.AddScoped<PortalRepositorio>();
builder.Services.AddScoped<PortalNegocio>();
builder.Services.AddSingleton(new PagamentoConfiguracao(builder.Configuration["Pagamento:Token"]??"",builder.Configuration["Pagamento:Retorno"]??""));
builder.Services.AddHttpClient<PagamentoNegocio>(c=>c.Timeout=TimeSpan.FromSeconds(20));
builder.Services.AddControllers(options=>options.Filters.Add<HorizonteViagens.Api.Filtros.ErrosFiltro>());
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


if (app.Environment.IsDevelopment())
{
    // Publica o documento só durante o desenvolvimento.
    app.MapOpenApi();

    // Scalar: tela gráfica que lê o documento OpenAPI acima e permite ver e
    // testar cada endpoint pelo navegador, em /scalar.
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Primeiro descobre QUEM é (lê o token), depois confere se PODE entrar.
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// ALTERAÇÃO PIM: permite testes HTTP com WebApplicationFactory.
public partial class Program { }
