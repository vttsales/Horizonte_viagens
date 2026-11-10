# Horizonte Viagens

![Horizonte Viagens](docs/assets/horizonte-banner.svg)

Passagem, hospedagem e experiências em uma viagem completa. Frontend HTML/CSS/JavaScript para GitHub Pages, API C#, SQL Server e aplicativo Flutter no mesmo repositório.

## Onde está o index.html?

**Na raiz deste repositório:** [index.html](index.html). O site estático usa `styles.css`, `app.js`, `api.js`, `domain.js`, `config.js`, `catalogo.json` e `assets/`. Não precisa converter Razor nem compilar Flutter para publicar o site.

## Executar o frontend

Com Node.js 22 ou superior:

```sh
node server.mjs
```

Abra http://127.0.0.1:5180/Horizonte_viagens/. Use um servidor HTTP, não duplo clique/file://, pois o site carrega módulos JavaScript e JSON.

## Conectar à API

Edite **somente a URL pública da API** em `config.js`:

```js
window.HORIZONTE_CONFIG = Object.freeze({apiBaseUrl: "https://SUA-API"});
```

Não coloque senha, token de pagamento ou conexão SQL nesse arquivo. No Pages a API precisa de HTTPS público; localhost ou IP privado não funcionam para visitantes da internet. A origem `https://vttsales.github.io` já está na configuração CORS da API. Para outra conta/domínio, ajuste a origem no backend.

Sem URL de API, o catálogo de referência pode ser consultado, com preços explicitamente estimados e saídas sob consulta. Login, gestão, reservas e pagamento exigem API operacional; nenhum sucesso é inventado. A referência editorial não representa estoque de fornecedores.

## Publicar

Em **Settings → Pages → Build and deployment → Source**, selecione **GitHub Actions**. O workflow `.github/workflows/pages.yml` testa e publica somente os arquivos estáticos quando `main` é atualizada. A URL esperada para este repositório é https://vttsales.github.io/Horizonte_viagens/.

O workflow não hospeda C# ou SQL e não configura serviços pagos. A equipe precisa disponibilizar a API HTTPS para usar a área transacional pela internet. Durante desenvolvimento, site e API podem rodar gratuitamente nos computadores da equipe.

## Estrutura

| Local | Conteúdo |
|---|---|
| Raiz + assets | Site HTML, CSS e JavaScript |
| backend | API, MVC alternativo e testes .NET |
| mobile | Aplicativo Flutter |
| sql | SQL recebido e migrations consolidadas |
| scripts | Execução local e empacotamento Pages |
| documentacao | Guias da equipe, fontes de preço e resumos |
| PIM - Horizonte (Web) | Versão anterior preservada como histórico; não usada pelo novo deploy |

## Funcionalidades

Busca por destino/evento, filtros, cálculo por 1 a 6 adultos com quartos inteiros, detalhes, cadastro/login, perfil, favoritos, reservas, carteira de documentos e administração de combos/clientes. Pagamento integrado **somente ao sandbox**; nenhuma cobrança real ou emissão automática de fornecedor.

A sessão do site fica em memória e termina ao recarregar a página. Autorizações e valor de reserva são validados na API. Dados de cartão nunca passam pelo frontend da Horizonte.

## Verificação

- 14 testes JavaScript aprovados.
- 34 testes .NET aprovados, incluindo autorização e edição concorrente de combos pela API.
- Flutter preservado da entrega anterior (3 testes aprovados e Web compilado naquela etapa).
- SQL Server e pagamento externo ainda dependem da homologação da equipe. Testes locais usam SQLite isolado.

Leia [o resumo desta atualização](documentacao/ATUALIZACAO-GITHUB-PAGES.md), [a execução do backend](documentacao/COMO-EXECUTAR.md) e [as fontes dos preços](documentacao/PRECOS-E-FONTES.md).

Para habilitar cadastro/login no site publicado, siga [Ligar a API](documentacao/LIGAR-API.md). Enquanto a API não estiver configurada, os formulários informam a indisponibilidade antes do envio.
