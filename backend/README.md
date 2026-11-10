# Horizonte Viagens Backend

Backend da solução Horizonte Viagens, desenvolvido em ASP.NET Core com Entity Framework Core e SQL Server. Este README descreve as camadas, o fluxo dos dados, as migrations e a API. As telas Razor existentes fazem parte do projeto Web; alterações visuais ficam separadas deste guia.

## O que o backend oferece

- Cadastro, consulta, alteração e exclusão de clientes pela API.
- Login de administrador com senha armazenada como hash e emissão de token JWT.
- Persistência de clientes e administradores com Entity Framework Core.
- Busca de clientes por nome por meio da procedure `sp_Clientes_BuscarPorNome`, chamada com Dapper.
- Modelo EF Core para hotéis, telefones, voos, passagens, quartos, combos, compras, monitoramentos e passeios.
- Migrations para criar e atualizar o banco SQL Server.

As entidades de viagens já estão mapeadas no `AppDbContext`, mas ainda não têm repositórios, regras de negócio nem endpoints CRUD na API. A API atualmente expõe operações de clientes e autenticação.

## Projetos e responsabilidades

| Projeto | Responsabilidade |
|---|---|
| `HorizonteViagens.Modelos` | Entidades de domínio e DTOs usados nas requisições e respostas. |
| `HorizonteViagens.Repositorio` | `AppDbContext`, mapeamentos EF Core, repositórios, migrations e script SQL de referência. |
| `HorizonteViagens.Negocio` | Regras de cadastro, validação, autenticação e hash de senha. |
| `HorizonteViagens.Api` | Endpoints REST, autenticação JWT, autorização, OpenAPI e Scalar. |
| `HorizonteViagens.Web` | Aplicação MVC existente, que usa as mesmas regras e repositórios. |

Dependências principais:

```text
API ──► Negocio ──► Repositorio ──► Modelos
Web ──► Negocio ──► Repositorio ──► Modelos
```

O controller recebe uma classe de negócio por injeção de dependência. A classe de negócio recebe uma interface de repositório; o ASP.NET Core monta essa cadeia no `Program.cs`.

## Fluxo do cadastro de cliente

1. `ClientesController` recebe o JSON como `ClienteSalvarDto`.
2. O DTO valida campos de entrada e converte os dados em uma entidade `Cliente`.
3. `ClienteNegocio` normaliza e valida CPF, celular, e-mail e senha. A senha nunca é salva em texto puro: `SenhaHasher` gera um hash.
4. `ClienteRepositorio` grava ou consulta a entidade usando `AppDbContext` e EF Core.
5. A API converte a entidade em `ClienteDto`, que não inclui `SenhaHash`.

A busca por nome segue o mesmo caminho até o repositório, mas chama a stored procedure com Dapper e parâmetro separado, evitando concatenar a entrada do usuário no SQL.

## Entity Framework Core e banco de dados

O `AppDbContext` representa a conexão e o modelo relacional. Cada `DbSet` representa uma tabela. O método `OnModelCreating` configura tamanhos, campos obrigatórios, índices únicos, chaves compostas, relacionamentos e restrições de datas e valores.

`Clientes` tem uma trigger de atualização. O mapeamento declara `HasTrigger("TR_Clientes_DataAtualizacao")` para que o EF Core gere comandos compatíveis com essa tabela.

Migrations existentes:

- `Inicial`: tabela `Clientes`, índices únicos, trigger de atualização e procedure de busca.
- `AdministradorELogin`: senha e celular do cliente, tabela `Administradores` e conta inicial.
- `EsquemaViagens`: endereço do cliente e tabelas do modelo de viagens.

### Aplicar as migrations

Instale a ferramenta do EF Core uma vez na máquina, se ainda não estiver instalada:

```bash
dotnet tool install --global dotnet-ef --version 9.0.22
```

Na pasta onde está `HorizonteViagens.sln`, aplique as migrations:

```bash
dotnet ef database update --project HorizonteViagens.Repositorio --startup-project HorizonteViagens.Api
```

Para criar uma migration depois de alterar uma entidade ou o mapeamento:

```bash
dotnet ef migrations add NomeDaMudanca --project HorizonteViagens.Repositorio --startup-project HorizonteViagens.Api
dotnet ef database update --project HorizonteViagens.Repositorio --startup-project HorizonteViagens.Api
```

O banco é criado **só pelas migrations**. Para ter o script SQL completo (por exemplo, para o documento de banco de dados), gere-o a partir delas: `Script-Migration -Project HorizonteViagens.Repositorio -StartupProject HorizonteViagens.Api -Output banco.sql`.

### Configurar a conexão

A chave `ConnectionStrings:Padrao` está nos `appsettings.json` da API e do projeto Web. O valor de exemplo aponta para `localhost`, banco `HorizonteViagens`, usando autenticação do Windows:

```json
"Padrao": "Server=localhost;Database=HorizonteViagens;Trusted_Connection=True;TrustServerCertificate=True"
```

Troque `Server=` pelo nome da instância mostrado no SSMS. Para SQL Express, por exemplo, a instância costuma ser `localhost\\SQLEXPRESS` dentro do JSON. Se usar autenticação SQL, mantenha usuário e senha fora de arquivos versionados; use User Secrets ou variáveis de ambiente.

## API REST

Em desenvolvimento, a API disponibiliza OpenAPI em `/openapi/v1.json` e a interface Scalar em `/scalar`. Com o perfil HTTP padrão, a base é `http://localhost:5218`.

| Método e rota | Acesso | Resultado |
|---|---|---|
| `POST /api/autenticacao/login` | Público | Valida administrador e devolve token JWT. |
| `POST /api/clientes` | Público | Cadastra cliente; responde `201` ou `400`. |
| `GET /api/clientes` | Administrador | Lista clientes. |
| `GET /api/clientes/{id}` | Administrador | Obtém cliente ou responde `404`. |
| `GET /api/clientes/buscar?nome=ana` | Administrador | Busca pela stored procedure. |
| `PUT /api/clientes/{id}` | Administrador | Atualiza cliente; senha vazia mantém a senha atual. |
| `DELETE /api/clientes/{id}` | Administrador | Exclui cliente. |
| `POST /api/autenticacao/login-cliente` | Público | Autentica cliente e devolve token com papel `Cliente`. |
| `GET /api/combos?destino=Recife` | Público | Lista combos disponíveis; filtro é opcional e usa `sp_Combos_BuscarPorDestino` via Dapper. |
| `POST /api/combos` | Administrador | Cria combo da vitrine. `PUT` e `DELETE /api/combos/{id}` atualizam e removem. |
| `POST /api/compras` | Cliente | Compra o combo do JSON; o cliente é identificado pelo token e a vaga é debitada na mesma transação. |
| `GET /api/compras` | Administrador | Monitora compras com cliente, combo e valor. |
| `POST /api/compras/{id}/monitoramento` | Administrador | Associa a compra ao administrador que a acompanha. |

Envie o token de administrador no cabeçalho `Authorization: Bearer <token>`. Os endpoints de clientes, exceto o cadastro, exigem o papel `Administrador`.

As operações de combo exigem que a migration `CatalogoCombos` tenha sido aplicada. Ela interrompe se houver compras antigas com `ComboId` nulo; revise esses registros antes de atualizar o banco. Para compra, use o token retornado por `login-cliente`.

### Testes automatizados

Execute `dotnet test` na pasta da solution (ou, no Visual Studio, **Teste › Executar Todos os Testes**). São 10 testes unitários, com repositórios falsos e sem banco: dígitos verificadores do CPF e senha guardada como hash (`ClienteNegocioTests`), cadastro e alteração de combo (`ComboNegocioTests`) e as regras da compra — cliente ativo, combo existente, vaga disponível, desconto da vaga e concorrência (`CompraNegocioTests`). A concorrência de verdade (o `RowVersion` no SQL Server) só aparece com banco ligado.

## Executar e conferir

```bash
dotnet restore
dotnet build HorizonteViagens.sln
dotnet run --project HorizonteViagens.Api
```

Abra `http://localhost:5218/scalar`. O arquivo `HorizonteViagens.Api/HorizonteViagens.Api.http` contém chamadas de exemplo para login, cadastro, consulta, alteração e exclusão. Algumas chamadas alteram dados do banco; use dados de teste.

## Segurança e configuração

- O sistema guarda hash da senha, não a senha original.
- O DTO de resposta não expõe a coluna `SenhaHash`.
- A busca Dapper usa parâmetros separados do comando SQL.
- A chave do JWT **não fica no `appsettings.json`**. Em desenvolvimento, cada pessoa guarda a sua no User Secrets (fora da pasta do projeto, nunca vai para o Git):
  ```bash
  dotnet user-secrets set "Jwt:Chave" "uma-frase-longa-e-secreta-com-32-caracteres-ou-mais" --project HorizonteViagens.Api
  ```
  Se ninguém configurar, a Api gera uma chave aleatória ao iniciar, só em desenvolvimento: ela sobe normalmente, mas o login precisa ser refeito toda vez que a Api reinicia. Em produção, a chave vem da variável de ambiente `Jwt__Chave`, e sem ela a Api não sobe.
- `/health` na Api e na Web responde `Healthy` quando o sistema está de pé.
- Os controllers registram log estruturado (`ILogger`) só com Ids, nunca com dados pessoais.
- `TrustServerCertificate=True` é adequado para desenvolvimento local; revise a política de certificados antes de usar em produção.

## Guia do código

O documento [DOCUMENTACAO-BACKEND.docx](DOCUMENTACAO-BACKEND.docx) explica as classes principais, o caminho de uma requisição, o uso do EF Core e do Dapper e o que ainda falta para disponibilizar as entidades de viagem pela API.
