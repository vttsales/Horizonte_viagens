// ALTERAÇÃO PIM: servidor local estático, sem dependências; publica somente os arquivos do frontend.
import http from 'node:http';
import {readFile} from 'node:fs/promises';
import {fileURLToPath} from 'node:url';
const root=new URL('./',import.meta.url);
const types={'.html':'text/html; charset=utf-8','.js':'text/javascript; charset=utf-8','.css':'text/css; charset=utf-8','.json':'application/json; charset=utf-8','.jpg':'image/jpeg','.svg':'image/svg+xml'};
http.createServer(async(req,res)=>{
 try{
  let path=decodeURIComponent(new URL(req.url,'http://localhost').pathname);
  // Permite verificar o prefixo real do projeto no Pages sem disponibilizar backend, SQL ou arquivos Git.
  path=path.replace(/^\/Horizonte_viagens(?=\/)/,'');
  if(path==='/')path='/index.html';
  if(!/^\/(index\.html|styles\.css|app\.js|api\.js|domain\.js|config\.js|catalogo\.json|assets\/[a-z]+\.(jpg|svg))$/.test(path)){res.writeHead(404);res.end('Não encontrado');return;}
  const data=await readFile(fileURLToPath(new URL('.'+path,root))),ext=path.slice(path.lastIndexOf('.'));
  res.writeHead(200,{'Content-Type':types[ext]||'application/octet-stream','Cache-Control':'no-store'});res.end(data);
 }catch{res.writeHead(404);res.end('Não encontrado');}
}).listen(5180,'127.0.0.1',()=>console.log('Local: http://127.0.0.1:5180/Horizonte_viagens/'));
