# Atualização de 10/11/2026 — HTML e GitHub

## Alterações comentadas

- `index.html`: substitui o redirecionamento antigo pela entrada real do portal; navegação acessível, metadados e recursos relativos.
- `styles.css` e `assets/`: preservam a identidade Horizonte e adaptam as telas ao HTML estático e a dispositivos móveis.
- `app.js`: rotas por hash compatíveis com subdiretórios Pages, busca, preços, formulários, sessão, reservas, carteira e CRUD administrativo. Escape de dados antes de exibir HTML.
- `domain.js`: cálculo monetário em centavos, quartos inteiros, escape de HTML e validação de URLs HTTPS/domínio de checkout.
- `api.js`: contrato HTTP, autenticação em memória, timeout, tratamento de erros e recusa de sucesso offline.
- `config.js`: URL pública da API editável; nenhuma credencial. Mantida vazia porque a equipe ainda não forneceu uma API HTTPS publicada.
- `catalogo.json`: os seis destinos e estimativas da entrega anterior para consulta editorial; sem vagas, data ou preço confirmado. JSON não permite comentários, por isso a finalidade está documentada aqui.
- `server.mjs`: prévia HTTP local, somente com arquivos públicos permitidos e prefixo igual ao Pages.
- `scripts/package-pages.mjs`: separa os arquivos públicos em `.pages-build`, excluindo backend, SQL e documentação do site publicado.
- `.github/workflows/pages.yml`: teste e deploy pelo GitHub Actions, somente ao atualizar main ou executar manualmente.
- `.nojekyll`: impede processamento Jekyll quando houver publicação por branch.
- `tests/frontend.test.js`: dez verificações de preço, sessão, erros e URLs.
- `backend/HorizonteViagens.Api/Controllers/PortalController.cs`: dois endpoints administrativos para consultar reservas/documentos com autorização de administrador.
- `backend/HorizonteViagens.Api/appsettings*.json`: origem do Pages e prévia local na lista CORS. Nenhum segredo acrescentado.
- `backend/HorizonteViagens.Testes/PortalIntegrationTests.cs`: novo teste do CRUD por API, versão antiga recusada e bloqueio de acesso de cliente à gestão.
- `backend/PreviewApi`: ferramenta opcional de teste da API com banco descartável das fixtures; não publicar nem utilizar como backend final.
- `README.md`: execução, index.html, configuração da API e publicação; README anterior preservado em documentacao.
- `.gitignore`/`package.json`: exclusões e comandos de teste/execução sem dependências npm adicionais.

`backend`, `mobile`, `sql`, `scripts` e guias da entrega integrada anterior foram incorporados ao repositório. O resumo daquela etapa está em `RESUMO-ALTERACOES.md`. Os arquivos originais em Downloads não foram alterados.

## Verificação e limites

10 testes JavaScript e 23 testes .NET aprovados. A API de teste usa SQLite; SQL Server, procedures e trigger exigem validação na máquina do banco. O Pages hospeda apenas o frontend. Para operação externa, a equipe precisa configurar API HTTPS, SQL acessível ao backend e credencial sandbox de pagamento. Nada disso é substituído por um login local fictício.

Documentação de publicação: https://docs.github.com/en/pages/getting-started-with-github-pages/using-custom-workflows-with-github-pages

## Verificação no navegador

Fluxos executados contra a API C# com banco descartável: login cliente, consulta de orçamento, reserva de dois adultos por R$ 4.600, cancelamento com confirmação, login admin e edição de combo com gravação. Conferida busca por Ubatuba e troca para três adultos (R$ 8.550); imagens sem falhas e sem largura excedente na tela verificada. A URL local da API foi removida de config.js antes do empacotamento. Nenhum pagamento externo foi iniciado.
