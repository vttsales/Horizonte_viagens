// script.js
// arquivo unico de javascript do site
// so tem o que nao da pra fazer com html e css puro

// LOGIN E CADASTRO
// os dados ficam salvos no localStorage

// verifica se ja tem alguem logado
var usuarioLogado =
  JSON.parse(localStorage.getItem('horizonte_usuario')) || null

// roda quando a pagina carrega
window.onload = function () {
  atualizarNavbar()
  bloquearDatasPassadas()

  // fecha o menu mobile antes de navegar para a secao
  var links = document.querySelectorAll('.menu-links .nav-link')
  for (var i = 0; i < links.length; i++) {
    links[i].addEventListener('click', function (e) {
      e.preventDefault()
      var destino = this.getAttribute('href')
      var offcanvas = document.getElementById('menuMobile')
      var instancia = bootstrap.Offcanvas.getInstance(offcanvas)
      if (instancia) instancia.hide()
      setTimeout(function () {
        document.querySelector(destino).scrollIntoView({ behavior: 'smooth' })
      }, 320)
    })
  }
}

// atualiza a navbar com o nome do usuario ou os botoes de login
function atualizarNavbar() {
  var desktop = document.getElementById('area-login')
  var mobile = document.getElementById('area-login-mobile')

  if (usuarioLogado) {
    desktop.innerHTML =
      'Olá, <strong>' +
      usuarioLogado.nome +
      '</strong> &nbsp;<button class="nav-link btn-login-nav" onclick="fazerLogout()">Sair</button>'
    mobile.innerHTML =
      '<p style="color:rgba(255,255,255,0.8);margin-bottom:12px;">Olá, <strong>' +
      usuarioLogado.nome +
      '</strong></p><button class="btn-sair-mobile" onclick="fazerLogout()">Sair da conta</button>'
    document.getElementById('saudacao-hero').innerHTML =
      'Olá, <span class="destaque">' +
      usuarioLogado.nome +
      '</span>, para onde quer ir?'
  } else {
    desktop.innerHTML =
      '<a class="nav-link btn-login-nav" href="#" data-bs-toggle="modal" data-bs-target="#modalLogin">Entrar</a> <a class="nav-link btn-cadastro-nav" href="#" data-bs-toggle="modal" data-bs-target="#modalCadastro">Cadastre-se</a>'
    mobile.innerHTML =
      '<a class="btn-entrar-mobile" href="#" data-bs-toggle="modal" data-bs-target="#modalLogin" data-bs-dismiss="offcanvas">Entrar</a><a class="btn-cadastrar-mobile mt-2" href="#" data-bs-toggle="modal" data-bs-target="#modalCadastro" data-bs-dismiss="offcanvas">Cadastre-se grátis</a>'
    document.getElementById('saudacao-hero').innerHTML =
      'Explore o mundo com o <span class="destaque">Horizonte</span>'
  }
}

function fazerCadastro() {
  var nome = document.getElementById('cadNome').value.trim()
  var email = document.getElementById('cadEmail').value.trim()
  var telefone = document.getElementById('cadTelefone').value.trim()
  var senha = document.getElementById('cadSenha').value
  var termos = document.getElementById('aceitarTermos').checked

  if (!nome) {
    alert('Informe seu nome.')
    return
  }
  if (email.indexOf('@') == -1) {
    alert('Informe um e-mail válido.')
    return
  }
  if (telefone.replace(/\D/g, '').length < 10) {
    alert('Informe seu telefone com DDD.')
    return
  }
  if (senha.length < 8) {
    alert('A senha precisa ter pelo menos 8 caracteres.')
    return
  }
  if (!termos) {
    alert('Aceite os termos de uso.')
    return
  }

  var usuarios = JSON.parse(localStorage.getItem('horizonte_usuarios') || '[]')
  for (var i = 0; i < usuarios.length; i++) {
    if (usuarios[i].email == email) {
      alert('Esse e-mail já tem uma conta.')
      return
    }
  }

  var novo = { nome: nome, email: email, telefone: telefone, senha: senha }
  usuarios.push(novo)
  localStorage.setItem('horizonte_usuarios', JSON.stringify(usuarios))

  usuarioLogado = novo
  localStorage.setItem('horizonte_usuario', JSON.stringify(novo))

  bootstrap.Modal.getInstance(document.getElementById('modalCadastro')).hide()
  atualizarNavbar()
  alert('Bem-vindo, ' + nome + '! Conta criada com sucesso.')
}

function fazerLogin() {
  var email = document.getElementById('loginEmail').value.trim()
  var senha = document.getElementById('loginSenha').value

  if (!email || !senha) {
    alert('Preencha e-mail e senha.')
    return
  }

  var usuarios = JSON.parse(localStorage.getItem('horizonte_usuarios') || '[]')
  var encontrado = null
  for (var i = 0; i < usuarios.length; i++) {
    if (usuarios[i].email == email && usuarios[i].senha == senha) {
      encontrado = usuarios[i]
    }
  }

  if (!encontrado) {
    alert('E-mail ou senha incorretos.')
    return
  }

  usuarioLogado = encontrado
  localStorage.setItem('horizonte_usuario', JSON.stringify(encontrado))

  bootstrap.Modal.getInstance(document.getElementById('modalLogin')).hide()
  atualizarNavbar()
  alert('Bem-vindo de volta, ' + encontrado.nome + '!')
}

function fazerLogout() {
  usuarioLogado = null
  localStorage.removeItem('horizonte_usuario')
  atualizarNavbar()
}

// abre os detalhes do combo — so funciona logado
function verCombo(destino) {
  if (!usuarioLogado) {
    alert('Faça login para ver os detalhes e reservar.')
    new bootstrap.Modal(document.getElementById('modalLogin')).show()
    return
  }
  alert('Em breve: página de detalhes do combo ' + destino + '!')
}

// whatsapp
function abrirWhatsapp(destino, preco) {
  var numero = '5516999999999' // trocar pelo numero real
  var msg =
    'Olá! Vi o combo para ' +
    destino +
    ' por ' +
    preco +
    ' e gostaria de mais informações.'
  window.open(
    'https://wa.me/' + numero + '?text=' + encodeURIComponent(msg),
    '_blank'
  )
}

function abrirWhatsappGeral() {
  var numero = '5516999999999'
  var msg = 'Olá! Preciso de ajuda com o site Horizonte Viagens.'
  window.open(
    'https://wa.me/' + numero + '?text=' + encodeURIComponent(msg),
    '_blank'
  )
}

// newsletter
function cadastrarNewsletter() {
  var email = document.getElementById('emailNewsletter').value.trim()
  if (email.indexOf('@') == -1) {
    alert('Informe um e-mail válido.')
    return
  }
  document.getElementById('emailNewsletter').value = ''
  alert('E-mail cadastrado! Você vai receber as melhores ofertas.')
}

// mascara p formulario
function mascaraTelefone(input) {
  var v = input.value.replace(/\D/g, '').substring(0, 11)
  if (v.length <= 10) {
    v = v.replace(/(\d{2})(\d)/, '($1) $2').replace(/(\d{4})(\d)/, '$1-$2')
  } else {
    v = v.replace(/(\d{2})(\d)/, '($1) $2').replace(/(\d{5})(\d)/, '$1-$2')
  }
  input.value = v
}

function mascaraCpf(input) {
  var v = input.value.replace(/\D/g, '').substring(0, 11)
  v = v
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d{1,2})$/, '$1-$2')
  input.value = v
}

function toggleSenha(idCampo, btn) {
  var campo = document.getElementById(idCampo)
  if (campo.type == 'password') {
    campo.type = 'text'
    btn.querySelector('i').className = 'bi bi-eye-slash'
  } else {
    campo.type = 'password'
    btn.querySelector('i').className = 'bi bi-eye'
  }
}

// busca
function buscarCombos() {
  var origem = document.getElementById('origem').value.trim()
  var destino = document.getElementById('destino').value.trim()
  var dataIda = document.getElementById('dataIda').value

  if (!origem || !destino || !dataIda) {
    alert('Preencha origem, destino e data de ida.')
    return
  }

  // rola a pagina ate a secao de combos
  document.getElementById('combos').scrollIntoView({ behavior: 'smooth' })
}

// bloqueia datas passadas nos campos de data
function bloquearDatasPassadas() {
  var hoje = new Date().toISOString().split('T')[0]
  var inputs = document.querySelectorAll('input[type="date"]')
  for (var i = 0; i < inputs.length; i++) {
    inputs[i].min = hoje
  }
}
