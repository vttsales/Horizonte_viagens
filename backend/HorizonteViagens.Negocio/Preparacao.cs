// ALTERAÇÃO PIM: inicialização explícita e idempotente do catálogo; não confirma ofertas nem cria pagamentos.
using HorizonteViagens.Modelos;
using HorizonteViagens.Repositorio.Contexto;
using Microsoft.EntityFrameworkCore;
namespace HorizonteViagens.Negocio;
public static class Preparacao
{
 public static async Task Inicializar(AppDbContext db,bool acessosTeste,bool desenvolvimento)
 {
  if(acessosTeste&&!desenvolvimento)throw new InvalidOperationException("Acessos acadêmicos só podem ser criados em Development.");
  if(!await db.Combos.AnyAsync()){
   db.Combos.AddRange(
    Pacote("Paris","Luxo","Semana de Moda de Paris","paris",14,17000,3900,1800,"Aéreo executivo ida e volta\nHotel categoria 5 estrelas, sujeito a cotação\nRoteiro de moda e jantar; desfiles oficiais dependem de convite\nCity tour e passeio no Sena"),
    Pacote("Tóquio","Luxo","Festival Hanami (Cherry Blossom)","tokyo",19,29500,2600,5000,"Aéreo executivo ida e volta\nHotel categoria 5 estrelas, sujeito a cotação\nRoteiro de contemplação das cerejeiras e Monte Fuji\nOrçamento para transporte ferroviário e guia; confirmar o passe"),
    Pacote("Nova York","Luxo","Broadway + Times Square NYE","newyork",9,19500,4600,2800,"Aéreo executivo ida e volta\nHotel categoria 4 estrelas\nBroadway e passeios sujeitos a cotação\nCelebração pública em Times Square é gratuita, sem lugar garantido"),
    Pacote("Porto Seguro","Econômico","Carnaval de Porto Seguro","beach",9,1200,450,600,"Aéreo econômico ida e volta\nPousada, acomodação de até dois adultos\nProgramação de Carnaval a confirmar\nPasseios e serviços dependem de cotação"),
    Pacote("Ubatuba","Econômico","Festival de Surf de Ubatuba","coast",6,300,550,350,"Transporte rodoviário ida e volta\nPousada, acomodação de até dois adultos\nProgramação de surfe a confirmar\nAula de surfe e caiaque sujeitos a cotação"),
    Pacote("Florianópolis","Econômico","Fenaostra — Festival da Ostra","coast",7,850,470,500,"Aéreo econômico ida e volta\nHospedagem de categoria econômica\nProgramação da Fenaostra a confirmar\nGastronomia e passeio de barco sujeitos a cotação"));
  }
  // Desativa apenas o administrador de senha pública da versão recebida, preservando contas alteradas.
  var antigo=await db.Administradores.FirstOrDefaultAsync(a=>a.Id==1);
  if(antigo!=null&&SenhaHasher.Conferir("Horizonte@2026",antigo.SenhaHash))antigo.Ativo=false;
  if(acessosTeste){
   var admin=await db.Administradores.FirstOrDefaultAsync(a=>a.Email=="admin@horizonte.test");
   if(admin==null){admin=new Administrador{Nome="Administrador PIM",Email="admin@horizonte.test"};db.Administradores.Add(admin);}
   admin.Ativo=true;admin.SenhaHash=SenhaHasher.Gerar("HorizontePim!2026");admin.AcessoAte=DateTime.UtcNow.AddDays(7);
   var cliente=await db.Clientes.FirstOrDefaultAsync(c=>c.Email=="cliente@horizonte.test");
   if(cliente==null){cliente=new Cliente{Nome="Cliente PIM",Email="cliente@horizonte.test",Cpf="52998224725",Celular="11999999999"};db.Clientes.Add(cliente);}
   cliente.Ativo=true;cliente.SenhaHash=SenhaHasher.Gerar("ViagemPim!2026");cliente.AcessoAte=DateTime.UtcNow.AddDays(7);
  }
  await db.SaveChangesAsync();
 }
 private static Combo Pacote(string cidade,string categoria,string evento,string imagem,int noites,decimal transporte,decimal diaria,decimal experiencias,string inclusoes)=>new(){Nome=cidade+" + experiências",Destino=cidade,Categoria=categoria,Evento=evento,Imagem=imagem,Noites=noites,Valor=transporte,DiariaQuarto=diaria,ValorExperiencias=experiencias,Inclusoes=inclusoes,Descricao="Estimativa de planejamento baseada em referências públicas de novembro de 2026. Datas, hospedagem, ingresso e valor final dependem de confirmação. Consulte a composição do preço.",Publicado=true,PrecoConfirmado=false,Vagas=0};
}
