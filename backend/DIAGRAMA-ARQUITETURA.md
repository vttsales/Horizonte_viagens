# Arquitetura Horizonte Viagens

Diagrama das camadas e do fluxo que o front-end Flutter usa para consultar combos, autenticar clientes e registrar compras. A API e a Web compartilham as mesmas regras de negócio e repositórios.

```mermaid
flowchart LR
  Flutter[App Flutter] -->|HTTPS JSON / JWT| API[HorizonteViagens.Api]
  Browser[Navegador] --> Web[HorizonteViagens.Web MVC]
  API --> Negocio[HorizonteViagens.Negocio]
  Web --> Negocio
  Negocio --> Repo[HorizonteViagens.Repositorio]
  Repo --> EF[Entity Framework Core]
  Repo --> Dapper[Dapper / stored procedures]
  EF --> SQL[(SQL Server)]
  Dapper --> SQL
  Modelos[HorizonteViagens.Modelos] -. contratos e entidades .-> API
  Modelos -. contratos e entidades .-> Web
  Modelos -. entidades .-> Negocio
  Modelos -. entidades .-> Repo
```

## Fluxo de compra

```mermaid
sequenceDiagram
  participant F as Flutter
  participant A as API
  participant N as Negócio
  participant R as Repositório
  participant B as SQL Server
  F->>A: POST /api/autenticacao/login-cliente
  A->>N: validar e-mail e senha hash
  N->>R: localizar cliente ativo
  R->>B: consultar Clientes
  A-->>F: JWT com perfil Cliente
  F->>A: POST /api/compras + comboId + JWT
  A->>N: comprar(cliente do token, comboId)
  N->>R: executar compra
  R->>B: transação + UPDATE Vagas > 0 + INSERT Compras
  B-->>R: commit ou sem vaga
  R-->>A: compra ou conflito
  A-->>F: 200 ou 409
```

## Modelo de dados mapeado

O banco mapeia as 14 entidades/tabelas seguintes; o número de 12 citado no PDF parece ser uma contagem antiga e deve ser comparado ao diagrama oficial do grupo antes de remover estruturas:

| Tabela | Relação principal |
|---|---|
| Clientes | possui telefones; pode realizar compras |
| Administradores | acompanha compras e possui telefones |
| TelefonesClientes | pertence a um cliente |
| TelefonesAdministradores | pertence a um administrador |
| Hoteis | possui telefones e reservas de quarto |
| TelefonesHoteis | pertence a um hotel |
| Voos | possui passagens |
| Passagens | pertence a voo e participa de item de combo |
| ReservasQuartos | pertence a hotel e participa de item de combo |
| Passeios | participa de item de combo |
| Combos | possui itens, compras e estoque de vagas |
| ItensCombos | liga combo, passagem, quarto e passeio |
| Compras | liga cliente e combo com preço registrado |
| MonitoramentosCompras | associa administrador e compra acompanhada |

## Rotas para integração

- `GET /api/combos?destino=...`: vitrine pública, pesquisa pela procedure parametrizada `sp_Combos_BuscarPorDestino`.
- `POST /api/autenticacao/login-cliente`: emite token de cliente.
- `POST /api/compras`: aceita `comboId`; o cliente vem do token. A compra baixa uma vaga atomicamente.
- `GET /api/compras`: lista para administradores.
- `POST /api/compras/{id}/monitoramento`: registra acompanhamento administrativo.

O modelo mantém `Viagem`, `PacoteEconomico` e `PacoteLuxo` no domínio, enquanto `Combo` é a entidade persistida de catálogo. A relação conceitual final entre a hierarquia POO e as tabelas precisa seguir a decisão do grupo e seu diagrama relacional aprovado.
