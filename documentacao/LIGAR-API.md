# Ligar o site à API — roteiro para o colega do backend

Situação atual: o frontend está publicado no GitHub Pages; a API ainda não foi iniciada pela equipe. Por isso cadastro, login, reservas e pagamentos não funcionam no site público. Nenhuma conta é criada no navegador.

## Caminho dos dados

Navegador no GitHub Pages → API C# por HTTPS → SQL Server.

A conexão/senha do SQL pertence ao processo da API. O arquivo público config.js contém somente a URL da API. O banco pode estar em outro PC, desde que o PC da API consiga acessá-lo.

## 1. No PC do backend

Instale SDK .NET 9 e obtenha o repositório atualizado. Peça ao colega do banco o endereço da instância SQL e o acesso apropriado. Abra PowerShell na raiz do repositório. O exemplo abaixo só vale quando o SQL Express está no mesmo PC e aceita autenticação Windows; substitua a conexão pela fornecida pelo responsável:

```powershell
.\scripts\preparar-banco.ps1 -Conexao 'Server=.\SQLEXPRESS;Database=HorizonteViagensPIM;Trusted_Connection=True;TrustServerCertificate=True' -AcessosTeste
.\scripts\iniciar-api.ps1 -Conexao 'Server=.\SQLEXPRESS;Database=HorizonteViagensPIM;Trusted_Connection=True;TrustServerCertificate=True'
```

Preparação é feita uma vez (e novamente quando houver migrations). O segundo comando fica executando; fechar o terminal encerra a API. AcessosTeste cria contas acadêmicas com prazo de sete dias; as senhas estão no guia COMO-EXECUTAR.md e só servem ao ambiente controlado da equipe.

## 2. Conferir antes de conectar o site

Abra no navegador do PC da API:

- http://localhost:5218/health — confirma apenas que o processo está respondendo.
- http://localhost:5218/api/portal/catalogo — confirma também o acesso às tabelas do banco; deve retornar JSON, não erro SQL.
- http://localhost:5218/scalar — documentação dos endpoints em Development.

Se o catálogo falhar, resolva conexão/migrations antes de configurar o site. Não envie senha nem connection string em repositório público.

## 3. Teste totalmente local, sem publicar a API

No PC do frontend, execute `node server.mjs`. No config.js LOCAL, informe a URL HTTP da API na rede privada; o backend deve escutar no endereço da rede e permitir a conexão necessária. Consulte COMO-EXECUTAR.md para a execução em computadores separados. Não envie esse endereço privado para a versão publicada no GitHub Pages.

## 4. Teste do site público sem contratar hospedagem

Somente depois de a API e o SQL funcionarem, o responsável pelo PC pode usar o cloudflared oficial e executar em outro terminal:

```powershell
cloudflared tunnel --url http://localhost:5218
```

Isso disponibiliza a API por uma URL HTTPS temporária `https://....trycloudflare.com`. Quem tem a URL pode alcançar a API, ainda sujeito às regras de autenticação. Use um banco isolado com dados fictícios para essa conferência; não disponibilize contas administrativas de senha pública junto com dados reais.

O túnel gratuito é para desenvolvimento/testes, não hospedagem definitiva. Computador, API e túnel precisam permanecer ligados; o endereço muda ao reiniciar. Nenhum túnel foi aberto por esta atualização.

Documentação oficial: https://developers.cloudflare.com/tunnel/get-started/quick-tunnels/

## 5. Configurar o frontend

Envie somente a URL HTTPS gerada ao responsável pelo frontend. Configure em config.js:

```js
window.HORIZONTE_CONFIG = Object.freeze({apiBaseUrl: "https://ENDERECO-REAL-DA-API"});
```

A API já permite CORS para `https://vttsales.github.io`. Atualize o repositório e aguarde o workflow Pages. A API precisa ser a desta versão do repositório para reconhecer os contratos de cadastro, orçamento, reservas e administração.

## 6. Conferência final

- CPF curto/repetido/verificador errado: rejeição no formulário e no servidor.
- Senha fora das regras: rejeição; confirmação de senha é conferida no formulário.
- Cadastro com dados controlados válidos: deve responder 201; confirmar a linha em Clientes sem senha em texto puro.
- E-mail/CPF duplicados: rejeição.
- Login: token e acesso apenas aos dados do próprio cliente.
- Atualizar a página encerra a sessão do frontend por opção de armazenamento somente em memória.

Pagamentos ainda exigem configuração/homologação sandbox à parte. O teste matemático do CPF não comprova identidade, titularidade ou situação na Receita.
