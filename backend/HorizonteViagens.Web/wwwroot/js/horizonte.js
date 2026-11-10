// ALTERAÇÃO PIM: preço visível responde imediatamente à quantidade; servidor recalcula ao reservar.
(() => {
 const moeda=v=>v.toLocaleString('pt-BR',{style:'currency',currency:'BRL'});
 document.querySelectorAll('[data-viajantes]').forEach(select=>select.addEventListener('change',()=>{
  const n=Number(select.value); if(!Number.isInteger(n)||n<1||n>6)return;
  document.querySelectorAll('[data-combo]').forEach(card=>{
   const total=Math.round((Number(card.dataset.pessoa)*n+Number(card.dataset.quarto)*Math.ceil(n/2))*100)/100;
   card.querySelector('[data-total]').textContent=moeda(total);
   card.querySelector('[data-unitario]').textContent=moeda(total/n);
   card.querySelector('[data-quantidade]').textContent=n;
   const link=card.querySelector('[data-detalhes]'),url=new URL(link.href);url.searchParams.set('viajantes',n);link.href=url.href;
  });
 }));
 // Evita duplo clique; idempotência também é garantida no banco.
 document.querySelectorAll('form[method="post"]').forEach(form=>form.addEventListener('submit',()=>{if(!form.checkValidity())return;form.querySelectorAll('button[type="submit"],button:not([type])').forEach(b=>{b.disabled=true;b.textContent='Aguarde…';});}));
})();
