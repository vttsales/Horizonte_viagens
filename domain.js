// ALTERAÇÃO PIM: funções puras compartilhadas pela vitrine e pelos testes do frontend.
export const money = value => new Intl.NumberFormat('pt-BR', {style:'currency', currency:'BRL'}).format(Number(value));
export const escapeHtml = value => String(value ?? '').replace(/[&<>"']/g, char => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[char]));
export function budget(combo, adults) {
  const n=Number(adults); if(!Number.isInteger(n)||n<1||n>6) throw Error('Escolha de 1 a 6 adultos.');
  const rooms=Math.ceil(n/2), nights=Number(combo.noites);
  const transporte=Math.round(Number(combo.valor)*100)*n;
  const experiencias=Math.round(Number(combo.valorExperiencias)*100)*n;
  const hospedagem=Math.round(Number(combo.diariaQuarto)*100)*rooms*nights;
  return {viajantes:n,quartos:rooms,noites:nights,transporte:transporte/100,experiencias:experiencias/100,hospedagem:hospedagem/100,total:(transporte+experiencias+hospedagem)/100};
}
export function apiBase(value, protocol='https:') {
  if(!value) return '';
  const url=new URL(value);
  if(url.username||url.password||url.search||url.hash) throw Error('Endereço de serviço inválido.');
  if(url.protocol!=='https:' && !(protocol==='http:' && url.protocol==='http:')) throw Error('O serviço precisa usar uma conexão HTTPS.');
  return url.href.replace(/\/$/,'');
}
export function safeUrl(value, payment=false) {
  try {const u=new URL(value);if(u.protocol!=='https:'||u.username||u.password)return null;
    if(payment && !(u.hostname==='mercadopago.com.br'||u.hostname.endsWith('.mercadopago.com.br')))return null;
    return u.href;
  }catch{return null;}
}
export const imageName = name => ['hero','beach','coast','paris','tokyo','newyork'].includes(name)?name:'beach';
