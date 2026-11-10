# Execução com computadores separados

## Organização recomendada

- **PC do banco:** SQL Server Express/Developer e SSMS. O banco pode continuar com o colega responsável.
- **PC do backend:** SDK .NET 9, API e site MVC, ambos alcançando o mesmo SQL Server. É a opção mais simples para a apresentação.
- **PC do frontend:** navegador acessa o endereço do site. Flutter usa o endereço da API.

O site MVC renderiza o HTML no servidor e usa a mesma camada de negócio/repositório da API. Ele não é um arquivo HTML para abrir com duplo clique. O navegador e o Flutter nunca recebem a senha do SQL. Se rodar MVC no PC do frontend, esse processo também precisará alcançar SQL Server; manter MVC/API juntos evita essa necessidade.

## 1. Banco

Na raiz do pacote, em PowerShell, no PC do backend:

```powershell
.\scripts\preparar-banco.ps1 -Conexao 'Server=.\SQLEXPRESS;Database=HorizonteViagensPIM;Trusted_Connection=True;TrustServerCertificate=True' -AcessosTeste
```

Troque `.\SQLEXPRESS` pelo servidor real. Para SQL remoto com autenticação SQL, use a connection string fornecida pelo colega, por variável de ambiente ou user-secrets; não salve senha no Git. Autenticação Windows remota depende das permissões da rede. O SQL precisa permitir a conexão do backend; o responsável deve conferir TCP/IP e acesso da rede privada. Não exponha SQL à internet.

A preparação usa migrations e cria seis destinos apenas se o catálogo estiver vazio. Não apaga dados. Em banco existente, faça backup antes de aplicar a evolução. Ela renova os acessos acadêmicos quando `-AcessosTeste` é informado. Não use o script original de tabelas singulares sobre o esquema EF de tabelas plurais.

Alternativa para o colega do banco: criar/selecionar no SSMS um banco novo `HorizonteViagensPIM`, executar `sql/01-esquema-completo.sql`, depois rodar a preparação acima para inicializar catálogo e contas. O script não contém CREATE DATABASE nem USE: o banco selecionado no SSMS é o destino. O script preserva histórico de migrations. `sql/original.sql` é a cópia intacta recebida, apenas para referência, não para executar junto.

## 2. Site e API

Abra dois terminais no PC do backend e execute, usando a mesma conexão:

```powershell
.\scripts\iniciar-site.ps1 -Conexao 'Server=.\SQLEXPRESS;Database=HorizonteViagensPIM;Trusted_Connection=True;TrustServerCertificate=True' -Endereco 'http://0.0.0.0:5178'
.\scripts\iniciar-api.ps1 -Conexao 'Server=.\SQLEXPRESS;Database=HorizonteViagensPIM;Trusted_Connection=True;TrustServerCertificate=True' -Endereco 'http://0.0.0.0:5218'
```

Cada comando fica no seu terminal. `0.0.0.0` é somente o endereço de escuta; no navegador use `http://IP-DO-BACKEND:5178`. API: `http://IP-DO-BACKEND:5218/health`. Documentação de endpoints em `/scalar` (Development). Use a rede privada da equipe; o responsável pelo PC deve permitir somente o acesso necessário no firewall. Os scripts não alteram firewall.

Sem `-Endereco`, os scripts escutam somente localhost. No mesmo PC abra `http://localhost:5178`. Em produção use HTTPS; estes comandos HTTP são para ambiente acadêmico local. O token de Development é invalidado ao reiniciar a API se nenhuma chave JWT fixa for configurada.

## 3. Logins temporários

Só existem depois da preparação com `-AcessosTeste` em Development; expiram em sete dias:

| Papel | E-mail | Senha |
|---|---|---|
| Administrador | admin@horizonte.test | HorizontePim!2026 |
| Cliente | cliente@horizonte.test | ViagemPim!2026 |

Admin entra pelo link Administração no rodapé. Cliente entra pelo botão Entrar. O antigo admin com senha pública da base recebida é desativado na preparação se ainda usa aquela senha. Nunca publique estes acessos em produção.

## 4. Flutter

Instale Flutter stable compatível com Dart >=3.10 (validado com Flutter 3.47.9/Dart 3.13.7). No PC do app:

```powershell
.\scripts\iniciar-flutter.ps1 -ApiUrl 'http://IP-DO-BACKEND:5218'
```

A versão Chrome usa porta 5300. A API já permite `http://localhost:5300` e `http://127.0.0.1:5300`. Se usar outra origem, configure `Cors__Origens__0` no ambiente da API **antes de iniciá-la**. Não use `*` para resolver uma origem incorreta.

Android emulador com API no mesmo PC: `http://10.0.2.2:5218`. Celular físico: IP do backend na mesma rede. HTTP Android está permitido apenas em debug. Release exige API HTTPS e ambiente Android configurado (SDK/JDK); iOS exige macOS/Xcode. A identidade de pacote nativa recebida foi preservada.

```powershell
cd mobile
flutter pub get
flutter run -d SEU-DISPOSITIVO --dart-define=API_URL=http://IP-DO-BACKEND:5218
flutter build web --dart-define=API_URL=https://ENDERECO-DA-API
```

## 5. Pagamento sem cobrança

A integração aceita somente vendedor de teste Mercado Pago e usa `sandbox_init_point`. Cadastre as contas de teste no painel do provedor. Configure o access token **do vendedor de teste** em `Pagamento__Token` nos processos da API e do site, por ambiente ou user-secrets. Não coloque token no Flutter, HTML, appsettings versionado ou conversa.

`Pagamento__Retorno` é opcional; se informado, deve ser uma URL HTTPS sua. Sem retorno, volte manualmente à reserva depois do checkout e clique em Atualizar status. A confirmação vem da consulta autenticada à API do Mercado Pago, nunca de um parâmetro da URL.

Use comprador e cartões de teste oficiais. A disponibilidade do Pix sandbox depende do fluxo/conta do provedor e precisa ser conferida; não foi prometido Pix real gratuito nem cobrança real. Sem credenciais, o sistema informa indisponibilidade em vez de fingir pagamento aprovado. Em caso de expiração ou pagamento divergente, exige revisão.

Documentação oficial: https://www.mercadopago.com.br/developers/pt/docs/checkout-pro-preferences/test-accounts

## 6. Confirmar uma oferta

Administração > Gerenciar combos > Editar. Confira transporte individual, diária por quarto, noites e experiências por pessoa; informe partida futura e vagas, e marque preço confirmado/publicado. Só faça isso com cotação válida para uma oferta real. Para apresentação, use dados controlados da equipe e pagamentos sandbox. Não há reservas reais de fornecedor neste sistema.

## Falhas comuns

- Site/API não iniciam: confira SDK .NET, conexão e logs do terminal.
- Login retorna erro de SQL: banco precisa ser preparado e acessível pela máquina que executa o serviço.
- Flutter não conecta: localhost no celular aponta para o próprio celular; use IP da API.
- Erro CORS: origem inclui protocolo e porta do navegador, não o endereço do SQL.
- Catálogo vazio ou sob consulta: publique/confirme os combos pelo admin; a aplicação não inventa estoque.
- Conta temporária expirou: preparação com `-AcessosTeste` renova por sete dias.

## Atualização HTML/Pages de 10/11/2026

O frontend estático está agora no index.html da raiz. Para usá-lo em vez do MVC, execute node server.mjs e configure config.js com o endereço da API. A versão publicada no Pages exige HTTPS público. Consulte o README da raiz e ATUALIZACAO-GITHUB-PAGES.md.
