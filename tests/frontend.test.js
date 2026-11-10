// ALTERAÇÃO PIM: valida preço, segurança de URLs, sessão e chamadas reais do contrato HTTP com transporte controlado.
import test from 'node:test';
import assert from 'node:assert/strict';
import {readFile} from 'node:fs/promises';
import {budget,escapeHtml,apiBase,safeUrl} from '../domain.js';
import {Api} from '../api.js';
const c={valor:300,valorExperiencias:350,diariaQuarto:550,noites:6};
test('Preço inclui o quarto extra para grupo ímpar',()=>{for(const [n,total] of [[1,3950],[2,4600],[3,8550],[6,13800]])assert.equal(budget(c,n).total,total);});
test('Quantidade inválida é recusada',()=>{for(const n of [0,7,1.2,'x'])assert.throws(()=>budget(c,n));});
test('Centavos não acumulam erro de ponto flutuante',()=>assert.equal(budget({valor:0.1,valorExperiencias:0.2,diariaQuarto:0.1,noites:1},3).total,1.1));
test('HTML de dados não executa marcação',()=>assert.equal(escapeHtml('<img onerror="x">'), '&lt;img onerror=&quot;x&quot;&gt;'));
test('Checkout só aceita domínio HTTPS do provedor',()=>{assert.ok(safeUrl('https://sandbox.mercadopago.com.br/checkout',true));for(const url of ['javascript:alert(1)','http://mercadopago.com.br','https://falsomercadopago.com.br','https://mercadopago.com.br.exemplo.com'])assert.equal(safeUrl(url,true),null);});
test('Site HTTPS recusa API HTTP e URL com credencial',()=>{assert.throws(()=>apiBase('http://localhost:5218','https:'));assert.throws(()=>apiBase('https://user:pass@site.test'));assert.equal(apiBase('http://localhost:5218/','http:'),'http://localhost:5218');});
test('Login usa endpoint certo e autentica a próxima chamada sem persistir token',async()=>{
 const calls=[];const api=new Api('https://api.example.test','https:',async(url,opts)=>{calls.push({url,opts});return new Response(JSON.stringify(url.endsWith('login')?{token:'token-controlado',nome:'Admin'}:[]),{status:200});});
 await api.login('admin@example.test','senha-controlada',true);await api.call('/api/combos/todos');assert.ok(calls[0].url.endsWith('/api/autenticacao/login'));assert.equal(calls[0].opts.headers.Authorization,undefined);assert.equal(calls[1].opts.headers.Authorization,'Bearer token-controlado');assert.equal(calls[1].opts.credentials,'omit');api.logout();assert.equal(api.session,null);
});
test('Sem API não inventa login ou reserva',async()=>{const api=new Api('','https:',()=>{throw Error('Não deveria chamar');});await assert.rejects(api.login('a','b',false),/indisponível/);});
test('Erros do contrato são legíveis',async()=>{const api=new Api('https://api.test','https:',async()=>new Response(JSON.stringify({erro:'Sem vagas'}),{status:400}));await assert.rejects(api.call('/api/portal/reservas',{method:'POST',body:{}}),/Sem vagas/);});
test('Catálogo editorial nunca confirma estoque ou saída',async()=>{const data=JSON.parse(await readFile(new URL('../catalogo.json',import.meta.url)));assert.equal(data.length,6);for(const c of data){assert.equal(c.vagas,0);assert.equal(c.precoConfirmado,false);assert.equal(c.partida,null);}});
