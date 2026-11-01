<div align="center">

![Horizonte Viagens — novas histórias começam no horizonte](docs/assets/horizonte-banner.svg)

### Passagem, hospedagem e experiências. Uma viagem completa.

Um projeto de plataforma de viagens para reunir o destino, a estadia e os eventos em um único pacote.

![Status: em desenvolvimento](https://img.shields.io/badge/status-em_desenvolvimento-F0B45B?style=flat-square)
![HTML5](https://img.shields.io/badge/HTML5-E34F26?style=flat-square&logo=html5&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=flat-square&logo=css&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=flat-square&logo=javascript&logoColor=222222)
![Bootstrap](https://img.shields.io/badge/Bootstrap_5-7952B3?style=flat-square&logo=bootstrap&logoColor=white)

[Sobre](#sobre-o-projeto) · [Funcionalidades](#funcionalidades) · [Como executar](#como-executar) · [Próximos passos](#próximos-passos)

</div>

## Sobre o projeto

O **Horizonte Viagens** nasce com uma proposta: simplificar o planejamento de uma viagem ao reunir **passagens, hospedagem e ingressos para eventos** em um só lugar.

A experiência planejada permite explorar pacotes, escolher uma combinação de serviços e concluir a compra com **cartão ou Pix**. A versão atual apresenta o protótipo de front-end dessa jornada, com catálogo de viagens e interações no navegador.

### Uma viagem, três possibilidades

| Passagens | Hospedagem | Experiências |
| :--- | :--- | :--- |
| O caminho até o próximo destino. | A estadia que combina com a viagem. | Eventos e atrações para completar o roteiro. |

## Funcionalidades

| Recurso | Implementação atual |
| :--- | :--- |
| Catálogo de pacotes | Apresentação de opções de luxo e econômicas, com preços e serviços inclusos. |
| Formulário de busca | Campos de origem, destino, datas e viajantes; validação básica e navegação até o catálogo. Ainda não filtra resultados. |
| Hotéis | Cards com informações de hospedagem. |
| Cadastro e login | Demonstração local com persistência no navegador. |
| Navegação responsiva | Menu adaptado para dispositivos móveis e modais com Bootstrap. |
| Contato por WhatsApp | Abertura de mensagem contextual; número de exemplo a configurar. |
| Detalhes e reservas | Planejados; o botão atual apresenta um aviso. |
| Compra com cartão e Pix | Planejada; sem integração de pagamento nesta versão. |

> **Em desenvolvimento:** o catálogo contém dados ilustrativos. Não há consulta de disponibilidade, reserva ou cobrança real.

## Tecnologias

- **HTML5** — estrutura das páginas e formulários.
- **CSS3** — identidade visual, componentes e responsividade.
- **JavaScript** — interações, validações e armazenamento local.
- **Bootstrap 5.3.3** — grid, modais e menu móvel.
- **Bootstrap Icons** e **Google Fonts (Inter)** — ícones e tipografia.

O projeto não depende de framework JavaScript nem de uma etapa de build.

## Como executar

### 1. Clone o repositório

```bash
git clone https://github.com/vttsales/Horizonte_viagens.git
cd Horizonte_viagens
```

### 2. Abra a aplicação

Abra o arquivo `index.html` da raiz no navegador. Ele redireciona para a página principal do projeto.

Para desenvolvimento, também é possível usar um servidor estático. Se tiver Python instalado:

```bash
python -m http.server 8080
```

Depois, acesse [localhost:8080](http://localhost:8080).

**Requisito:** conexão à internet para carregar Bootstrap, ícones e fontes usados pelo site.

## Estrutura do repositório

```text
Horizonte_viagens/
├── README.md
├── docs/
│   └── assets/
│       └── horizonte-banner.svg
├── index.html                   # Entrada do projeto
└── PIM - Horizonte (Web)/
    ├── index.html               # Página principal e modais
    ├── css/
    │   ├── base.css             # Variáveis e estilos globais
    │   ├── header.css           # Navegação
    │   ├── hero.css             # Destaque e formulário de busca
    │   ├── combos.css           # Pacotes de viagens
    │   ├── secoes.css           # Hotéis e demais seções
    │   ├── footer.css           # Rodapé
    │   ├── modais.css           # Modais
    │   └── widgets.css          # Componentes auxiliares
    └── js/
        └── script.js           # Interações do protótipo
```

## Próximos passos

- [ ] Implementar busca e filtros conectados ao catálogo.
- [ ] Desenvolver a página de detalhes dos pacotes.
- [ ] Criar o backend de usuários e reservas.
- [ ] Integrar disponibilidade de passagens, hospedagens e eventos.
- [ ] Implementar checkout com cartão e Pix.
- [ ] Adicionar histórico e acompanhamento das reservas.
- [ ] Ampliar testes e revisar acessibilidade.

## Limites do protótipo

O cadastro atual utiliza `localStorage`, incluindo o armazenamento de senhas, apenas para demonstrar o fluxo. **Use dados fictícios.** Antes de disponibilizar contas reais, esse mecanismo deve ser substituído por autenticação segura no servidor.

O formulário de newsletter apenas exibe uma confirmação: não envia nem registra inscrições em um serviço. Datas, preços, contatos e demais informações demonstrativas precisam ser revisados antes de uma operação comercial.

## Autoria e sugestões

Projeto mantido por [vttsales](https://github.com/vttsales).

Encontrou um problema ou tem uma ideia? [Abra uma issue](https://github.com/vttsales/Horizonte_viagens/issues) com o contexto e, quando possível, os passos para reproduzir.

---

<div align="center">
  <sub>Horizonte Viagens · Seu próximo destino, uma nova história.</sub>
</div>
