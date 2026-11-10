// ALTERAÇÃO PIM: Persistência transacional de reservas, vagas, favoritos, documentos e status de pagamento.
using System.Data;
using HorizonteViagens.Modelos;
using HorizonteViagens.Repositorio.Contexto;
using Microsoft.EntityFrameworkCore;
namespace HorizonteViagens.Repositorio;

public class PortalRepositorio(AppDbContext db)
{
    public List<Combo> Catalogo() => db.Combos.AsNoTracking().Where(c=>c.Publicado).OrderBy(c=>c.Nome).ToList();
    public Combo? Combo(int id) => db.Combos.AsNoTracking().FirstOrDefault(c=>c.Id==id);
    public Cliente? Cliente(int id) => db.Clientes.FirstOrDefault(c=>c.Id==id && c.Ativo && (c.AcessoAte==null || c.AcessoAte>DateTime.UtcNow));
    public List<Compra> Reservas(int? clienteId) => db.Compras.AsNoTracking().Include(c=>c.Cliente).Include(c=>c.Combo).Where(c=>clienteId==null || c.ClienteId==clienteId).OrderByDescending(c=>c.Id).ToList();
    public Compra? Reserva(int id,int? clienteId=null) => db.Compras.Include(c=>c.Combo).Include(c=>c.Cliente).FirstOrDefault(c=>c.Id==id && (clienteId==null || c.ClienteId==clienteId));
    public List<int> Favoritos(int clienteId)=>db.Favoritos.Where(f=>f.ClienteId==clienteId).Select(f=>f.ComboId).ToList();
    public void Favoritar(int clienteId,int comboId,bool adicionar)
    {
        if(adicionar && !db.Combos.Any(c=>c.Id==comboId && c.Publicado)) throw new InvalidOperationException("Pacote não encontrado.");
        var f=db.Favoritos.Find(clienteId,comboId);
        if(adicionar && f==null) db.Favoritos.Add(new Favorito{ClienteId=clienteId,ComboId=comboId});
        if(!adicionar && f!=null)db.Favoritos.Remove(f);
        db.SaveChanges();
    }
    public List<DocumentoViagem> Documentos(int compraId)=>db.Documentos.AsNoTracking().Where(d=>d.CompraId==compraId).ToList();
    public void Documento(DocumentoViagem d){db.Documentos.Add(d);db.SaveChanges();}
    public Compra? PorRequisicao(int clienteId,Guid requisicao)=>db.Compras.FirstOrDefault(c=>c.ClienteId==clienteId && c.RequisicaoId==requisicao);
    // SERIALIZABLE mantém preço, disponibilidade e idempotência na mesma transação.
    public Compra Reservar(ReservaEntrada entrada,int clienteId,Func<Combo,Orcamento> validar)
    {
        using var tx=db.Database.BeginTransaction(IsolationLevel.Serializable);
        var antiga=PorRequisicao(clienteId,entrada.RequisicaoId);
        // Revalida o conteúdo também dentro da transação em requisições concorrentes.
        if(antiga!=null){if(antiga.ComboId!=entrada.ComboId||antiga.Viajantes!=entrada.Viajantes||antiga.Valor!=entrada.TotalEsperado)throw new InvalidOperationException("Identificador já utilizado para outra reserva.");tx.Commit();return antiga;}
        var combo=db.Combos.FirstOrDefault(c=>c.Id==entrada.ComboId) ?? throw new InvalidOperationException("Pacote não encontrado.");
        var total=validar(combo);
        combo.Vagas-=entrada.Viajantes;
        var reserva=new Compra{ComboId=combo.Id,ClienteId=clienteId,Viajantes=entrada.Viajantes,Quartos=total.Quartos,Noites=total.Noites,Valor=total.Total,NomePacote=combo.Nome,Destino=combo.Destino,Partida=combo.Partida,RequisicaoId=entrada.RequisicaoId};
        db.Compras.Add(reserva);db.SaveChanges();tx.Commit();return reserva;
    }
    public void Salvar()=>db.SaveChanges();
    public void Expirar()
    {
        // Alteração condicional garante devolução das vagas uma única vez.
        using var tx=db.Database.BeginTransaction(IsolationLevel.Serializable);
        var vencidas=db.Compras.Include(c=>c.Combo).Where(c=>c.Status=="Pendente" && c.ExpiraEm<DateTime.UtcNow).ToList();
        foreach(var c in vencidas){c.Status="Expirada";c.Combo.Vagas+=c.Viajantes;}
        db.SaveChanges();tx.Commit();
    }
    public bool Cancelar(int id,int clienteId)
    {
        using var tx=db.Database.BeginTransaction(IsolationLevel.Serializable);
        var c=Reserva(id,clienteId);
        if(c==null)return false;
        if(c.Status=="Cancelada")return true;
        if(c.Status!="Pendente" || c.PreferenciaId!=null)throw new InvalidOperationException("Esta reserva exige atendimento para cancelamento. Não há reembolso automático.");
        c.Status="Cancelada";c.Combo.Vagas+=c.Viajantes;db.SaveChanges();tx.Commit();return true;
    }
    public void AplicarPagamento(int id,string pagamentoId,string status,decimal valor,string referencia,bool teste)
    {
        using var tx=db.Database.BeginTransaction(IsolationLevel.Serializable);
        var c=Reserva(id) ?? throw new InvalidOperationException("Reserva não encontrada.");
        if(!teste || referencia!=$"HV-{c.Id}-{c.RequisicaoId:N}" || valor!=c.Valor)
            throw new InvalidOperationException("Pagamento incompatível com a reserva.");
        if(c.PagamentoId!=null && c.PagamentoId!=pagamentoId)throw new InvalidOperationException("Pagamento duplicado requer conferência.");
        if(status=="approved"){
            if(c.Status=="Pendente" && c.ExpiraEm>=DateTime.UtcNow)c.Status="Confirmada";
            else if(c.Status!="Confirmada")c.Status="Revisão de pagamento";
            c.PagamentoId=pagamentoId;
        }
        // Devoluções e chargebacks nunca continuam com documento utilizável.
        if(status is "refunded" or "charged_back"){c.Status="Revisão de pagamento";c.PagamentoId=pagamentoId;}
        db.SaveChanges();tx.Commit();
    }
}
