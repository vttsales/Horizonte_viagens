// ALTERAÇÃO PIM: sessão somente em memória; credenciais e pagamento nunca são armazenados no navegador.
import {apiBase} from './domain.js';
export class Api {
 // O wrapper preserva o contexto nativo de fetch nos navegadores.
 constructor(base, protocol, fetcher=(...args)=>fetch(...args)){this.base=apiBase(base,protocol);this.fetcher=fetcher;this.session=null;}
 async call(path,{method='GET',body,anonymous=false}={}){
  if(!this.base)throw Error('O atendimento online está temporariamente indisponível. Tente novamente mais tarde.');
  const controller=new AbortController(), timeout=setTimeout(()=>controller.abort(),20000);
  try{
   const headers={Accept:'application/json'};
   if(body!==undefined)headers['Content-Type']='application/json';
   if(this.session&&!anonymous)headers.Authorization='Bearer '+this.session.token;
   let response;
   try{response=await this.fetcher(this.base+path,{method,headers,body:body===undefined?undefined:JSON.stringify(body),signal:controller.signal,credentials:'omit',cache:'no-store',redirect:'error'});}
   catch{throw Error('Não foi possível conectar ao atendimento online. Confira sua conexão e tente novamente.');}
   const raw=await response.text();let data=null;
   try{data=raw?JSON.parse(raw):null;}catch{throw Error('O serviço não respondeu corretamente. Tente novamente.');}
   if(!response.ok){
    if(response.status===401&&!anonymous){this.session=null;window.dispatchEvent(new Event('session-ended'));}
    const detail=data?.erro || (data?.errors?Object.values(data.errors).flat().join(' '):null);
    throw Error(detail || ({401:'E-mail ou senha incorretos.',403:'Você não tem acesso a esta operação.',404:'Registro não encontrado.',409:'Os dados mudaram ou possuem vínculos. Atualize e tente novamente.',429:'Muitas tentativas. Aguarde um minuto.'}[response.status]) || 'Não foi possível concluir esta operação.');
   }
   return data;
  }finally{clearTimeout(timeout);}
 }
 async login(email,senha,admin){const data=await this.call('/api/autenticacao/'+(admin?'login':'login-cliente'),{method:'POST',body:{email,senha},anonymous:true});this.session={...data,admin};return this.session;}
 logout(){this.session=null;}
}
