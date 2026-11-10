// ALTERAÇÃO PIM: empacota uma lista explícita de arquivos públicos; backend e documentos não vão para o site.
import {mkdir,copyFile,cp} from 'node:fs/promises';
const root=new URL('../',import.meta.url),out=new URL('../.pages-build/',import.meta.url);
await mkdir(out,{recursive:true});
for(const name of ['index.html','styles.css','app.js','api.js','domain.js','config.js','catalogo.json','.nojekyll'])await copyFile(new URL(name,root),new URL(name,out));
await cp(new URL('assets/',root),new URL('assets/',out),{recursive:true});
console.log('Frontend pronto em .pages-build/');
