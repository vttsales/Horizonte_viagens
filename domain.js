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

// ALTERAÇÃO PIM: validação de formato e dígitos do CPF; não comprova titularidade ou situação cadastral.
export function cpfError(value) {
 const raw=String(value??'').trim();
 if(!/^(?:[0-9]{11}|[0-9]{3}\.[0-9]{3}\.[0-9]{3}-[0-9]{2})$/.test(raw))return 'Informe os 11 dígitos do CPF, com ou sem pontos e traço.';
 const cpf=raw.replace(/\D/g,'');
 if(/^(\d)\1{10}$/.test(cpf))return 'CPF inválido. Confira os dígitos informados.';
 const digit=n=>{let sum=0;for(let i=0;i<n;i++)sum+=Number(cpf[i])*(n+1-i);const rest=sum%11;return rest<2?0:11-rest;};
 return digit(9)===Number(cpf[9])&&digit(10)===Number(cpf[10])?'':'CPF inválido. Confira os dígitos informados.';
}
// As regras acompanham o C#: espaços não contam como símbolo, e a senha nunca é recortada ou normalizada.
export function passwordRules(value='') {return [
 {label:'De 8 a 128 caracteres',valid:value.length>=8&&value.length<=128},
 {label:'Uma letra maiúscula (A–Z)',valid:/[A-Z]/.test(value)},
 {label:'Uma letra minúscula (a–z)',valid:/[a-z]/.test(value)},
 {label:'Um número (0–9)',valid:/[0-9]/.test(value)},
 {label:'Um símbolo, como !, @ ou #',valid:/[^\p{L}\p{N}\s]/u.test(value)},
 {label:'Sem espaços',valid:! /\s/.test(value)}
 ];}
export function profileErrors(data,creating=false){
 const errors={cpf:cpfError(data.cpf)};
 const phone=String(data.celular??'').trim();
 errors.celular=/^[0-9()\s-]+$/.test(phone)&&phone.replace(/\D/g,'').length===11?'':'Informe DDD + celular, com 11 dígitos.';
 const cep=String(data.cep??'').trim();errors.cep=!cep||/^(?:[0-9]{8}|[0-9]{5}-[0-9]{3})$/.test(cep)?'':'Informe um CEP com 8 dígitos ou deixe vazio.';
 if(creating){const pass=String(data.senha??'');errors.senha=passwordRules(pass).filter(r=>!r.valid).map(r=>r.label+'.').join(' ');errors.confirmarSenha=data.confirmarSenha===pass&&pass?'':'As senhas precisam ser iguais.';}
 return errors;
}
