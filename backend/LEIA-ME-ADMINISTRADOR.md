# Horizonte Viagens: administrador e acesso exclusivo

Esta versão parte do template do professor, já renomeado para `HorizonteViagens`,
e acrescenta:

- a classe abstrata `Usuario`, com `Cliente` e `Administrador` herdando dela;
- `Celular` e `Senha` no cadastro do cliente, com as validações da versão em console;
- senha guardada como hash (`SenhaHasher`, na camada Negocio);
- login do administrador na Web (cookie) e na Api (token JWT);
- lista, busca, edição e exclusão de clientes liberadas **só** para o administrador;
- cadastro aberto para o viajante, na Web e no `POST api/clientes`.

## Arquivos novos ou alterados

| Camada | Arquivo | O que mudou |
|---|---|---|
| Modelos | `Usuario.cs` | **novo**: classe mãe abstrata |
| Modelos | `Cliente.cs` | herda de `Usuario`, ganhou `Celular` |
| Modelos | `Administrador.cs` | **novo** |
| Modelos | `Dtos/LoginDto.cs` | **novo** |
| Modelos | `Dtos/ClienteSalvarDto.cs` e `ClienteDto.cs` | `Celular`; `Senha` só no de entrada |
| Repositorio | `Contexto/AppDbContext.cs` | tabela `Administradores` e administrador inicial |
| Repositorio | `IAdministradorRepositorio.cs`, `AdministradorRepositorio.cs` | **novos** |
| Negocio | `SenhaHasher.cs`, `AdministradorNegocio.cs` | **novos** |
| Negocio | `ClienteNegocio.cs` | celular, senha forte e hash |
| Web | `Program.cs` | login por cookie |
| Web | `Controllers/ContaController.cs`, `Views/Conta/*` | **novos**: login e sair |
| Web | `Controllers/ClientesController.cs` | `[Authorize]` na classe, `[AllowAnonymous]` no cadastro |
| Web | `Views/Clientes/*`, `Views/Shared/_Layout.cshtml`, `Models/Formatacao.cs` | campos novos, menu que muda com o login |
| Api | `Program.cs`, `.csproj`, `appsettings.json` | login por token JWT |
| Api | `GeradorDeToken.cs`, `Controllers/AutenticacaoController.cs` | **novos** |
| Api | `Controllers/ClientesController.cs` | `[Authorize]` na classe, POST aberto |

## Como rodar

### 1. Ajustar o SQL Server

Igual ao README do professor (seção 0): confira o `Server=` nos dois
`appsettings.json` (Web e Api).

### 2. Gerar a migration

As entidades mudaram, então o banco precisa de uma migration nova. No Console
do Gerenciador de Pacotes (projeto padrão: `HorizonteViagens.Repositorio`,
projeto de inicialização: `HorizonteViagens.Api`):

```
Add-Migration AdministradorELogin
```

Ou pelo terminal, na pasta do `.sln`:

```bash
dotnet ef migrations add AdministradorELogin --project HorizonteViagens.Repositorio --startup-project HorizonteViagens.Api
```

### 3. Atualizar a procedure dentro da migration (escrito à mão)

A procedure `sp_Clientes_BuscarPorNome` foi criada antes de existir a coluna
`Celular`, então ela não devolve o celular. O EF não mexe em procedure,
então abra o arquivo que o passo 2 criou em `HorizonteViagens.Repositorio/Migrations/`
(termina em `_AdministradorELogin.cs`) e cole:

**No fim do método `Up`:**

```csharp
            // Escrito à mão: a procedure passa a devolver o Celular.
            migrationBuilder.Sql(@"
ALTER PROCEDURE sp_Clientes_BuscarPorNome
    @Trecho NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nome, Email, Cpf, Celular, DataCadastro, DataAtualizacao, Ativo
      FROM Clientes
     WHERE Nome LIKE '%' + @Trecho + '%'
     ORDER BY Nome;
END");
```

**No começo do método `Down`:**

```csharp
            migrationBuilder.Sql(@"
ALTER PROCEDURE sp_Clientes_BuscarPorNome
    @Trecho NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nome, Email, Cpf, DataCadastro, DataAtualizacao, Ativo
      FROM Clientes
     WHERE Nome LIKE '%' + @Trecho + '%'
     ORDER BY Nome;
END");
```

### 4. Atualizar o banco

```
Update-Database
```

(ou `dotnet ef database update --project HorizonteViagens.Repositorio --startup-project HorizonteViagens.Api`)

No SSMS, a tabela `Administradores` aparece com uma linha: o administrador inicial.

> Se o banco já tinha clientes cadastrados antes desta versão, eles ficam
> com celular e senha vazios. Corrija o celular pela tela de edição.

### 5. Testar

**Web** (`/`):
1. Clique em *Cadastre-se* e crie um cliente. Teste uma senha fraca para ver a mensagem.
2. Tente abrir `/Clientes`: o sistema manda para o login.
3. Entre com o administrador inicial:
   - e-mail: `admin@horizonteviagens.com`
   - senha: `Horizonte@2026`
4. A lista aparece, com busca, edição e exclusão. Clique em *Sair* e tente `/Clientes` de novo.

**Api**: use o arquivo `HorizonteViagens.Api.http`, começando pela chamada de
login. As chamadas seguintes já usam o token que ela devolve. No Scalar,
faça o login, copie o `token` e, no *Test Request*, acrescente o cabeçalho
`Authorization` com o valor `Bearer <token>`.

## Antes de entregar

- A chave do JWT não fica mais no `appsettings.json`: veja a seção
  "Segurança e configuração" do README (User Secrets).
- Se quiser outra senha para o administrador inicial, veja o comentário no
  `HasData` do `AppDbContext`.
