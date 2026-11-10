// ALTERAÇÃO PIM: frontend estático do portal. Rotas por hash funcionam em subpastas do GitHub Pages.
import {Api} from './api.js';
import {money,escapeHtml as e,budget,safeUrl,imageName} from './domain.js';
const main=document.querySelector('main'), notice=document.querySelector('#notice');
let api;
try{api=new Api(window.HORIZONTE_CONFIG?.apiBaseUrl,location.protocol);}catch{api=new Api('',location.protocol);}
let catalog=[], online=false, adults=2, returnTo='#reservas', serial=0;
const pending=new Map();
const field=(name,label,value='',type='text',extra='')=>`<label>${e(label)}<input name="${name}" type="${type}" value="${e(value)}" ${extra}></label>`;
const area=(name,label,value='')=>`<label class="wide">${e(label)}<textarea name="${name}" rows="4" maxlength="2000">${e(value)}</textarea></label>`;
const options=(values,selected)=>values.map(v=>`<option value="${e(v)}" ${String(v)===String(selected)?'selected':''}>${e(v)}</option>`).join('');
const qty=(n=adults)=>`<label>Viajantes adultos<select name="viajantes" id="adults">${[1,2,3,4,5,6].map(v=>`<option value="${v}" ${v===n?'selected':''}>${v} adulto${v>1?'s':''}</option>`).join('')}</select></label>`;
const errorBox='<p class="form-error" role="alert"></p>';
function message(text,error=false){notice.innerHTML=`<div class="notice ${error?'error':''}">${e(text)}</div>`;}
function nav(){
 const s=api.session;
 document.querySelector('#account-nav').innerHTML=s?`<a href="#reservas">${s.admin?'Reservas':'Minhas viagens'}</a><button class="btn btn-outline-dark" data-action="logout">Sair</button>`:'<a class="btn btn-dark" href="#entrar">Entrar</a>';
 const bar=document.querySelector('#portal-nav');bar.hidden=!s;
 bar.innerHTML=s?`<span>Olá, ${e(s.nome)}</span><a href="#reservas">Reservas</a>${s.admin?'<a href="#admin">Gerenciar combos</a><a href="#clientes">Clientes</a>':'<a href="#carteira">Carteira</a><a href="#favoritos">Favoritos</a><a href="#conta">Meu perfil</a>'}`:'';
}
// Cada nova leitura da API prevalece sobre a referência editorial e preserva IDs reais do banco.
async function loadCatalog(){
 try{catalog=await api.call('/api/portal/catalogo');online=true;}
 catch{online=false;const r=await fetch('./catalogo.json');if(!r.ok)throw Error('Não foi possível carregar os destinos.');catalog=await r.json();}
}
const empty=(text='Nenhum resultado encontrado.')=>`<div class="empty-panel"><h2>${e(text)}</h2><p>Explore outros destinos e experiências.</p><a class="btn btn-dark" href="#pacotes">Explorar destinos</a></div>`;
function card(c){const o=budget(c,adults);return `<article class="trip-card" data-card="${c.id}"><div class="trip-image"><img src="./assets/${imageName(c.imagem)}.jpg" alt="Paisagem de ${e(c.destino)}" loading="lazy"><span>${e(c.categoria)}</span></div><div class="trip-body"><small>${c.noites+1} dias / ${c.noites} noites · ${e(c.origem)}</small><h2>${e(c.destino)}</h2><p class="trip-event">${e(c.evento)}</p><p class="included">✈ Transporte　▤ Hospedagem　♫ Experiências</p><p class="departure">${online&&c.partida?`Saída: ${e(date(c.partida))}`:'Próximas saídas sob consulta'}</p><div class="trip-price"><small>${online&&c.precoConfirmado?'Total':'Total estimado'} · <span data-qty>${adults}</span> adulto(s)</small><strong data-total>${money(o.total)}</strong><small><span data-person>${money(o.total/adults)}</span> por pessoa</small></div><a class="btn btn-dark full" href="#pacote/${c.id}">Ver pacote →</a></div></article>`;}
function search(destino='',categoria=''){return `<form id="search" class="travel-search"><div class="search-caption">✈ Passagem + hospedagem + evento</div><div class="search-row">${field('destino','Destino ou evento',destino,'search','placeholder="Para onde você quer ir?"')}<label>Categoria<select name="categoria"><option value="">Todas as experiências</option>${options(['Luxo','Econômico'],categoria)}</select></label>${qty()}<button class="btn btn-gold">Buscar pacotes →</button></div></form>`;}
function date(d){if(!d)return 'Sob consulta';return new Intl.DateTimeFormat('pt-BR').format(new Date(d.length===10?d+'T12:00:00':d));}
function home(){return `<section class="hero"><div><span class="eyebrow">NOVOS DESTINOS. GRANDES HISTÓRIAS.</span><h1>Seu destino é só<br>o <em>começo.</em></h1><p>Passagem, hospedagem e aquele evento que você quer viver.<br>A Horizonte reúne tudo na mesma viagem.</p><a href="#pacotes" class="btn btn-gold">Encontre seu próximo horizonte →</a></div></section><div class="container search-overlap">${search()}</div><section class="container section"><span class="eyebrow">VIAGENS COMPLETAS, DO SEU JEITO</span><div class="section-title"><div><h2>Qual história você quer viver?</h2><p>Compare o total para seu grupo e conheça cada experiência.</p></div><a href="#pacotes">Ver todos os pacotes →</a></div><div class="trip-grid">${catalog.map(card).join('')||empty()}</div><p class="price-note">Valores sob consulta são estimativas de planejamento; datas e disponibilidade precisam de confirmação.</p></section><section class="events-band"><div class="container"><span class="eyebrow">O MOTIVO DA SUA PRÓXIMA VIAGEM</span><h2>Não vá só para conhecer.<br>Vá para viver.</h2><div class="event-grid">${catalog.map(c=>`<a class="event-card" href="#pacote/${c.id}"><img src="./assets/${imageName(c.imagem)}.jpg" alt="" loading="lazy"><span><small>${e(c.destino)}</small><strong>${e(c.evento)}</strong></span></a>`).join('')}</div></div></section>${steps()}`;}
function steps(){return `<section class="container section"><span class="eyebrow">DO PLANEJAMENTO AO EMBARQUE</span><h2>Sua viagem, organizada.</h2><div class="steps-grid"><article><b>01</b><h3>Escolha seu combo</h3><p>Confira transporte, hospedagem, eventos e o total para os viajantes.</p></article><article><b>02</b><h3>Reserve sua saída</h3><p>Entre na sua conta e reserve as vagas disponíveis na data do pacote.</p></article><article><b>03</b><h3>Leve tudo com você</h3><p>Acompanhe sua reserva e os documentos emitidos pelo site ou aplicativo.</p></article></div></section>`;}
function results(params,list=catalog,title='Encontre seu próximo destino'){
 const term=params.get('destino')||'',category=params.get('categoria')||'',sort=params.get('ordem');
 let filtered=list.filter(c=>(c.destino+' '+c.evento).toLocaleLowerCase('pt-BR').includes(term.toLocaleLowerCase('pt-BR'))&&(!category||c.categoria===category));
 if(sort==='preco')filtered.sort((a,b)=>budget(a,adults).total-budget(b,adults).total);
 return `<section class="container section"><h1>${e(title)}</h1>${search(term,category)}<div class="results-bar"><p>${filtered.length} destino(s) para descobrir</p><label>Ordenar <select class="filter-sort" id="sort"><option value="">Destinos</option><option value="preco" ${sort==='preco'?'selected':''}>Menor total</option></select></label></div><div class="trip-grid">${filtered.map(card).join('')||empty()}</div></section>`;
}
async function detail(id){
 const c=catalog.find(c=>c.id===id);if(!c)throw Error('Pacote não encontrado.');
 let o=budget(c,adults);
 if(online)o=await api.call(`/api/portal/orcamento/${id}?viajantes=${adults}`);
 const available=online&&c.precoConfirmado&&c.partida&&new Date(c.partida+'T23:59:59')>new Date()&&c.vagas>=adults;
 const favorite=api.session&&!api.session.admin?`<button class="btn btn-outline-dark" data-action="favorite" data-id="${id}">Salvar nos favoritos</button>`:'';
 return `<section class="container section"><a href="#pacotes">← Todos os destinos</a><h1>${e(c.destino)}</h1><p>${e(c.evento)}</p><div class="detail-layout"><div><img class="detail-cover" src="./assets/${imageName(c.imagem)}.jpg" alt="Paisagem de ${e(c.destino)}"><h2>Uma viagem para viver por inteiro.</h2><p>${e(c.descricao)}</p><h3>O que está previsto</h3><ul>${String(c.inclusoes).split('\n').map(t=>`<li>${e(t)}</li>`).join('')}</ul><p>Origem: ${e(c.origem)} · ${c.noites+1} dias / ${c.noites} noites</p>${favorite}</div><aside class="price-panel"><h2>Seu próximo horizonte</h2>${qty()}<dl><dt>Transporte para o grupo</dt><dd>${money(o.transporte)}</dd><dt>${o.quartos} quarto(s) · ${o.noites} noites</dt><dd>${money(o.hospedagem)}</dd><dt>Experiências para o grupo</dt><dd>${money(o.experiencias)}</dd></dl><p>${c.precoConfirmado&&online?'Total do grupo':'Total estimado do grupo'}</p><strong class="grand-total">${money(o.total)}</strong><p>${money(o.total/adults)} por adulto. Quartos para até dois adultos; grupo ímpar utiliza um quarto adicional.</p><p>Partida: ${e(date(c.partida))}</p>${available?(api.session?(api.session.admin?'<p>Acesse como cliente para reservar.</p>':`<form id="reserve" data-id="${id}" data-total="${o.total}">${errorBox}<button class="btn btn-gold full">Reservar para ${adults} adulto(s)</button></form>`):'<a class="btn btn-gold full" href="#entrar">Entrar para reservar</a>'):'<div class="admin-note">Saída sob consulta. A reserva será liberada após confirmação de data, valor e disponibilidade.</div>'}</aside></div></section>`;
}
function login(admin=false){return `<section class="auth-layout"><div class="auth-photo"><h1>O mundo chama.<br>Seu horizonte espera.</h1></div><div class="auth-content"><span class="eyebrow">${admin?'GESTÃO HORIZONTE':'SUA CONTA HORIZONTE'}</span><h2>${admin?'Entre na administração.':'Sua próxima viagem começa aqui.'}</h2><form id="login" data-admin="${admin}">${field('email','E-mail','','email','required autocomplete="username" maxlength="150"')}${field('senha','Senha','','password','required autocomplete="current-password"')}${errorBox}<button class="btn btn-gold full">Entrar →</button></form>${admin?'':'<p>Ainda não tem conta? <a href="#cadastro">Cadastre-se</a></p>'}<a href="#inicio">← Voltar ao site</a></div></section>`;}
function profileForm(data={},mode='cadastro',id=''){
 const admin=mode==='cliente';
 return `<section class="container section narrow"><h1>${mode==='cadastro'?'Crie sua conta':admin?'Editar cliente':'Meu perfil'}</h1><form id="profile" data-mode="${mode}" data-id="${id}"><div class="form-grid">${field('nome','Nome completo',data.nome,'text','required maxlength="100" autocomplete="name"')}${field('email','E-mail',data.email,'email','required maxlength="150" autocomplete="email"')}${field('cpf','CPF',data.cpf,'text','required maxlength="14" inputmode="numeric"')}${field('celular','Celular',data.celular,'tel','required maxlength="20"')}${field('cep','CEP',data.cep,'text','maxlength="9"')}${field('rua','Rua',data.rua,'text','maxlength="100"')}${field('bairro','Bairro',data.bairro,'text','maxlength="50"')}${field('cidade','Cidade',data.cidade,'text','maxlength="50"')}${mode==='cadastro'?field('senha','Senha',null,'password','required minlength="8" autocomplete="new-password"'):''}${admin?`<label class="field-inline"><input type="checkbox" name="ativo" ${data.ativo?'checked':''}>Conta ativa</label>`:''}</div>${mode==='cadastro'?'<p class="subtitle">Use ao menos oito caracteres, com letra maiúscula, minúscula, número e símbolo.</p>':''}${errorBox}<button class="btn btn-gold">${mode==='cadastro'?'Criar minha conta':'Salvar alterações'}</button></form></section>`;
}
function reservationCard(c){return `<article class="reservation-card"><span class="status">${e(c.status)}</span><h2>${e(c.nomePacote)}</h2><p>HV-${c.id} · ${e(c.destino)}</p><p>${c.viajantes} adulto(s) · ${c.quartos} quarto(s) · ${c.noites} noites</p><p>${e(date(c.partida))} · <strong>${money(c.valor)}</strong></p><a class="btn btn-dark" href="#reserva/${c.id}">Acompanhar reserva →</a></article>`;}
const docCards=docs=>docs.map(d=>{const url=safeUrl(d.url);return `<article class="document-card"><small>${e(d.tipo)}</small><h3>${e(d.titulo)}</h3><p>Localizador: ${e(d.codigo)}</p>${url?`<a href="${e(url)}" target="_blank" rel="noopener noreferrer">Abrir documento do fornecedor →</a>`:'<p>Documento indisponível. Solicite a correção à agência.</p>'}</article>`;}).join('');
async function reservation(id){
 const admin=api.session.admin, list=await api.call(admin?'/api/portal/gestao/reservas':'/api/portal/reservas'), c=list.find(c=>c.id===id);
 if(!c)throw Error('Reserva não encontrada.');
 const docs=await api.call(admin?`/api/portal/gestao/reservas/${id}/documentos`:`/api/portal/reservas/${id}/documentos`);
 return `<section class="container section narrow"><a href="#reservas">← Reservas</a><h1>${e(c.nomePacote)}</h1><p>HV-${id} · <strong>${e(c.status)}</strong></p><p>${c.viajantes} adulto(s) · ${c.quartos} quarto(s) · ${c.noites} noites · ${money(c.valor)}</p>${!admin?`${c.status==='Pendente'?`<div class="notice">Pagamento de teste, sem cobrança real. Prazo da reserva: ${e(new Date(c.expiraEm+'Z'.repeat(!/Z|[+-]\d\d:\d\d$/.test(c.expiraEm))).toLocaleString('pt-BR'))}.</div><div class="actions"><button class="btn btn-gold" data-action="pay" data-id="${id}">Abrir pagamento de teste</button>${!c.pagamentoUrl?`<button class="btn btn-outline-dark" data-action="cancel" data-id="${id}">Cancelar reserva pendente</button>`:''}</div>`:''}<button class="btn btn-outline-dark" data-action="verify" data-id="${id}">Atualizar status do pagamento</button>`:''}<h2>Documentos da viagem</h2>${docCards(docs)||'<p>Os documentos aparecerão após confirmação e emissão pela agência.</p>'}${admin&&c.status==='Confirmada'?`<h2>Adicionar documento emitido</h2><form id="document" data-id="${id}" class="stack">${field('tipo','Tipo','Voucher','text','required maxlength="40"')}${field('titulo','Título','','text','required maxlength="140"')}${field('url','URL HTTPS do fornecedor','','url','required maxlength="1000"')}${field('codigo','Código / localizador','','text','required maxlength="100"')}${errorBox}<button class="btn btn-gold">Adicionar à carteira</button></form>`:''}</section>`;
}
async function adminCombos(){const list=await api.call('/api/combos/todos');return `<section class="container section"><div class="section-title"><h1>Gerenciar combos</h1><a class="btn btn-gold" href="#combo/novo">+ Novo combo</a></div><div class="table-wrap"><table class="table"><thead><tr><th>Destino / evento</th><th>Vagas</th><th>Partida</th><th>Publicado</th><th>Ações</th></tr></thead><tbody>${list.map(c=>`<tr><td><strong>${e(c.nome)}</strong><br>${e(c.evento)}</td><td>${c.vagas}</td><td>${e(date(c.partida))}</td><td>${c.publicado?'Sim':'Não'}</td><td><a href="#combo/${c.id}">Editar</a> <button class="btn-link" data-action="delete-combo" data-id="${c.id}">Excluir</button></td></tr>`).join('')}</tbody></table></div></section>`;}
async function comboForm(id){const c=id==='novo'?{origem:'São Paulo',noites:1,categoria:'Econômico',imagem:'beach'}:await api.call('/api/combos/'+Number(id));return `<section class="container section narrow"><a href="#admin">← Gerenciar combos</a><h1>${id==='novo'?'Novo combo':'Editar combo'}</h1><p class="admin-note">Confirme cotação, data e vagas antes de liberar a reserva. Transporte e experiências são por adulto; diária é por quarto.</p><form id="combo" data-id="${e(id)}"><input type="hidden" name="versao" value="${e(c.versao)}"><div class="form-grid">${field('nome','Nome',c.nome,'text','required maxlength="100"')}${field('destino','Destino',c.destino,'text','required maxlength="100"')}${field('origem','Origem',c.origem,'text','required maxlength="100"')}${field('evento','Evento',c.evento,'text','maxlength="140"')}<label>Categoria<select name="categoria">${options(['Luxo','Econômico'],c.categoria)}</select></label><label>Imagem<select name="imagem">${options(['beach','coast','paris','tokyo','newyork','hero'],c.imagem)}</select></label>${field('noites','Noites',c.noites,'number','required min="1" max="60"')}${field('vagas','Vagas disponíveis',c.vagas??0,'number','required min="0" max="100000"')}${field('valor','Transporte por adulto (R$)',c.valor??0,'number','required min="0" max="99999999" step="0.01"')}${field('diariaQuarto','Diária por quarto (R$)',c.diariaQuarto??0,'number','required min="0" max="99999999" step="0.01"')}${field('valorExperiencias','Experiências por adulto (R$)',c.valorExperiencias??0,'number','required min="0" max="99999999" step="0.01"')}${field('partida','Partida',c.partida,'date')}${area('descricao','Descrição',c.descricao)}${area('inclusoes','Serviços previstos (um por linha)',c.inclusoes)}<label class="field-inline"><input type="checkbox" name="publicado" ${c.publicado?'checked':''}>Publicado</label><label class="field-inline"><input type="checkbox" name="precoConfirmado" ${c.precoConfirmado?'checked':''}>Preço e disponibilidade confirmados</label></div>${errorBox}<button class="btn btn-gold">Salvar combo</button></form></section>`;}
async function customers(){const list=await api.call('/api/clientes');return `<section class="container section"><div class="section-title"><h1>Clientes</h1><a class="btn btn-gold" href="#cadastro">+ Cadastrar cliente</a></div><div class="table-wrap"><table class="table"><thead><tr><th>Nome</th><th>E-mail</th><th>Ativo</th><th>Ações</th></tr></thead><tbody>${list.map(c=>`<tr><td>${e(c.nome)}</td><td>${e(c.email)}</td><td>${c.ativo?'Sim':'Não'}</td><td><a href="#cliente/${c.id}">Editar</a><button class="btn-link" data-action="delete-client" data-id="${c.id}">Excluir</button></td></tr>`).join('')}</tbody></table></div></section>`;}
// Número de navegação evita que resposta lenta substitua uma tela mais recente.
async function route(){const turn=++serial;nav();notice.innerHTML='';const [path,query='']=(location.hash.slice(1)||'inicio').split('?');const [name,id]=path.split('/'), params=new URLSearchParams(query);
 const privateRoutes=['conta','reservas','reserva','carteira','favoritos','admin','combo','clientes','cliente'];
 if(privateRoutes.includes(name)&&!api.session){returnTo=location.hash;main.innerHTML=login(['admin','combo','clientes','cliente'].includes(name));return;}
 if(['admin','combo','clientes','cliente'].includes(name)&&!api.session.admin){main.innerHTML=empty('Acesso restrito à administração.');return;}
 if(['conta','carteira','favoritos'].includes(name)&&api.session?.admin){main.innerHTML=empty('Esta área é exclusiva do cliente.');return;}
 main.innerHTML='<p class="loading" role="status">Carregando…</p>';
 try{let html;
 switch(name){
 case 'inicio':html=home();break;
 case 'pacotes':html=results(params);break;
 case 'eventos':html=results(params,catalog.filter(c=>c.evento),'Viagens com experiências');break;
 case 'como-funciona':html=steps();break;
 case 'pacote':returnTo=location.hash;html=await detail(Number(id));break;
 case 'entrar':html=login();break;
 case 'admin-login':html=login(true);break;
 case 'cadastro':html=profileForm();break;
 case 'conta':html=profileForm(await api.call('/api/portal/perfil'),'conta');break;
 case 'cliente':html=profileForm(await api.call('/api/clientes/'+Number(id)),'cliente',Number(id));break;
 case 'clientes':html=await customers();break;
 case 'admin':html=await adminCombos();break;
 case 'combo':html=await comboForm(id);break;
 case 'reservas':{const list=await api.call(api.session.admin?'/api/portal/gestao/reservas':'/api/portal/reservas');html=`<section class="container section"><h1>${api.session.admin?'Gestão de reservas':'Minhas viagens'}</h1><div class="reservation-grid">${list.map(reservationCard).join('')||empty('Sua próxima história começa aqui.')}</div></section>`;break;}
 case 'reserva':html=await reservation(Number(id));break;
 case 'favoritos':{const ids=await api.call('/api/portal/favoritos');html=results(params,catalog.filter(c=>ids.includes(c.id)),'Meus favoritos');html+=`<section class="container"><div class="actions">${catalog.filter(c=>ids.includes(c.id)).map(c=>`<button class="btn btn-outline-dark" data-action="unfavorite" data-id="${c.id}">Remover ${e(c.destino)}</button>`).join('')}</div></section>`;break;}
 case 'carteira':{const trips=await api.call('/api/portal/reservas'),docs=(await Promise.all(trips.filter(t=>t.status==='Confirmada').map(t=>api.call(`/api/portal/reservas/${t.id}/documentos`)))).flat();html=`<section class="container section narrow"><h1>Carteira de viagem</h1>${docCards(docs)||empty('Seus documentos aparecerão aqui.')}</section>`;break;}
 case 'privacidade':html='<section class="container section narrow"><h1>Privacidade</h1><p>Os dados de cadastro são utilizados para identificar sua conta e organizar reservas. Dados de cartão são tratados pelo serviço de pagamento; não são armazenados neste site.</p><p>A sessão fica somente na memória da página e termina ao recarregar ou sair. Consulte a agência para solicitar correções ou atendimento relacionado aos seus dados.</p></section>';break;
 default:html=empty('Página não encontrada.');
 }
 if(turn!==serial)return;main.innerHTML=html;document.title=(name==='inicio'?'Seu próximo horizonte':main.querySelector('h1')?.innerText.replace(/\s+/g,' ')||'Horizonte')+' | Horizonte Viagens';window.scrollTo(0,0);
 }catch(err){if(turn===serial){main.innerHTML=`<section class="container section narrow"><h1>Não foi possível carregar esta página.</h1><p>${e(err.message)}</p><button class="btn btn-dark" data-action="retry">Tentar novamente</button> <a href="#inicio">Voltar ao início</a></section>`;}}
}
function go(hash){if(location.hash===hash)void route();else location.hash=hash;}
// Confirmações de exclusão ficam no contexto, com cancelamento como primeira opção.
function confirmAction(text){return new Promise(resolve=>{const d=document.createElement('dialog');d.innerHTML=`<h2>Confirmar alteração</h2><p>${e(text)}</p><form method="dialog" class="actions"><button value="cancel" class="btn btn-outline-dark" autofocus>Manter</button><button value="ok" class="btn btn-danger">Confirmar</button></form>`;document.body.append(d);d.addEventListener('close',()=>{resolve(d.returnValue==='ok');d.remove();},{once:true});d.showModal();});}
document.addEventListener('change',event=>{
 if(event.target.id==='adults'){
  adults=Number(event.target.value);
  if(location.hash.startsWith('#pacote/')){void route();return;}
  document.querySelectorAll('[data-card]').forEach(node=>{const c=catalog.find(c=>c.id===Number(node.dataset.card)),o=budget(c,adults);node.querySelector('[data-qty]').textContent=adults;node.querySelector('[data-total]').textContent=money(o.total);node.querySelector('[data-person]').textContent=money(o.total/adults);});
 }
 if(event.target.id==='sort'){const [path,q='']=(location.hash.slice(1)||'pacotes').split('?'),p=new URLSearchParams(q);p.set('ordem',event.target.value);go('#'+path+'?'+p);}
});
document.addEventListener('submit',async event=>{
 const f=event.target;if(!['search','login','profile','reserve','combo','document'].includes(f.id))return;event.preventDefault();
 const d=Object.fromEntries(new FormData(f)),button=f.querySelector('button[type=submit],button:not([type])'),err=f.querySelector('.form-error');
 if(err)err.textContent='';if(button)button.disabled=true;
 try{
 switch(f.id){
 case 'search':adults=Number(d.viajantes);go('#pacotes?'+new URLSearchParams({destino:d.destino,categoria:d.categoria}));break;
 case 'login':{const admin=f.dataset.admin==='true';await api.login(d.email.trim(),d.senha,admin);await loadCatalog();go(admin?'#admin':returnTo);break;}
 case 'profile':{const mode=f.dataset.mode;d.ativo=mode==='cliente'?f.elements.ativo.checked:true;if(mode!=='cadastro')delete d.senha;await api.call(mode==='cadastro'?'/api/clientes':mode==='conta'?'/api/portal/perfil':'/api/clientes/'+f.dataset.id,{method:mode==='cadastro'?'POST':'PUT',body:d,anonymous:mode==='cadastro'});if(mode==='conta'){api.session.nome=d.nome;nav();message('Perfil atualizado.');}else{go(api.session?.admin?'#clientes':'#entrar');setTimeout(()=>message(mode==='cadastro'?'Cadastro concluído. Entre com seu e-mail e senha.':'Cliente atualizado.'),0);}break;}
 case 'reserve':{const id=Number(f.dataset.id),total=Number(f.dataset.total),key=`${id}:${adults}:${total}`;if(!pending.has(key))pending.set(key,crypto.randomUUID());const c=await api.call('/api/portal/reservas',{method:'POST',body:{comboId:id,viajantes:adults,totalEsperado:total,requisicaoId:pending.get(key)}});pending.delete(key);go('#reserva/'+c.id);break;}
 case 'combo':{for(const k of ['noites','vagas','valor','diariaQuarto','valorExperiencias'])d[k]=Number(d[k]);d.publicado=f.elements.publicado.checked;d.precoConfirmado=f.elements.precoConfirmado.checked;d.partida=d.partida||null;d.versao=d.versao||null;await api.call('/api/combos'+(f.dataset.id==='novo'?'':'/'+f.dataset.id),{method:f.dataset.id==='novo'?'POST':'PUT',body:d});await loadCatalog();go('#admin');break;}
 case 'document':if(!safeUrl(d.url))throw Error('Informe uma URL HTTPS válida do fornecedor.');await api.call('/api/portal/documentos/'+f.dataset.id,{method:'POST',body:d});await route();break;
 }
 }catch(error){if(err)err.textContent=error.message;else message(error.message,true);}finally{if(button)button.disabled=false;}
});
document.addEventListener('click',async event=>{
 // O atalho de acessibilidade foca o conteúdo sem criar uma rota de aplicação.
 if(event.target.closest('.skip-link')){event.preventDefault();main.focus();return;}
 const b=event.target.closest('[data-action]');if(!b)return;const a=b.dataset.action,id=Number(b.dataset.id);b.disabled=true;
 try{
 if(a==='logout'){api.logout();pending.clear();go('#inicio');nav();return;}
 if(a==='retry'){await loadCatalog();await route();return;}
 if(a==='favorite'||a==='unfavorite'){await api.call('/api/portal/favoritos/'+id,{method:a==='favorite'?'PUT':'DELETE'});if(a==='unfavorite')await route();else message('Destino salvo nos favoritos.');}
 if(a==='delete-combo'||a==='delete-client'){
  if(!await confirmAction(a==='delete-combo'?'Excluir este combo? Pacotes com reservas devem ser despublicados para preservar o histórico.':'Excluir este cliente? Registros vinculados podem impedir a exclusão.'))return;
  await api.call((a==='delete-combo'?'/api/combos/':'/api/clientes/')+id,{method:'DELETE'});await loadCatalog();await route();
 }
 if(a==='cancel'){if(!await confirmAction('Cancelar esta reserva pendente e devolver as vagas?'))return;await api.call(`/api/portal/reservas/${id}/cancelar`,{method:'POST'});await route();}
 if(a==='verify'){await api.call(`/api/portal/reservas/${id}/verificar`,{method:'POST'});await route();message('Status consultado no serviço de pagamento.');}
 if(a==='pay'){const result=await api.call(`/api/portal/reservas/${id}/pagamento`,{method:'POST'}),url=safeUrl(result.url,true);if(!url)throw Error('Não foi possível abrir um link seguro de pagamento.');window.open(url,'_blank','noopener,noreferrer');message('Se o navegador bloqueou a nova aba, use o link abaixo. Volte à reserva para consultar o status.');const link=document.createElement('a');link.href=url;link.target='_blank';link.rel='noopener noreferrer';link.textContent='Abrir checkout de teste';link.className='btn btn-gold';notice.firstElementChild.append(document.createElement('br'),link);}
 }catch(error){message(error.message,true);}finally{b.disabled=false;}
});
window.addEventListener('hashchange',route);
window.addEventListener('session-ended',()=>{nav();message('Sua sessão terminou. Entre novamente.',true);});
await loadCatalog().catch(()=>{});nav();await route();
