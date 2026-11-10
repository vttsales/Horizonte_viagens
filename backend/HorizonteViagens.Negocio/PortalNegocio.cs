// ALTERAÇÃO PIM: Regras de validação, autenticação, concorrência e inicialização compartilhadas.
using HorizonteViagens.Modelos;
using HorizonteViagens.Repositorio;
namespace HorizonteViagens.Negocio;
public class PortalNegocio(PortalRepositorio repo)
{
    public List<Combo> Catalogo()=>repo.Catalogo();
    public Orcamento Orcar(int id,int viajantes){var c=repo.Combo(id)??throw new RegraDeNegocioException("Pacote não encontrado.");if(!c.Publicado)throw new RegraDeNegocioException("Pacote indisponível.");return Precificacao.Calcular(c,viajantes);}
    public Cliente Cliente(int id)=>repo.Cliente(id)??throw new RegraDeNegocioException("Cliente não encontrado ou inativo.");
    public Compra Reservar(int clienteId,ReservaEntrada entrada)
    {
        Cliente(clienteId);
        if(entrada.RequisicaoId==Guid.Empty)throw new RegraDeNegocioException("Informe o identificador da requisição.");
        var antiga=repo.PorRequisicao(clienteId,entrada.RequisicaoId);
        if(antiga!=null){if(antiga.ComboId!=entrada.ComboId || antiga.Viajantes!=entrada.Viajantes || antiga.Valor!=entrada.TotalEsperado)throw new RegraDeNegocioException("Identificador já utilizado para outra reserva.");return antiga;}
        return repo.Reservar(entrada,clienteId,c=>{
            if(!c.Publicado || !c.PrecoConfirmado || c.Partida is null || c.Partida<=DateOnly.FromDateTime(DateTime.Today))throw new RegraDeNegocioException("Saída sob consulta. Aguarde a confirmação da agência.");
            var o=Precificacao.Calcular(c,entrada.Viajantes);
            if(c.Vagas<entrada.Viajantes)throw new RegraDeNegocioException("Não há vagas para todos os viajantes.");
            if(o.Total<=0)throw new RegraDeNegocioException("A oferta precisa ter valor positivo.");
            if(o.Total!=entrada.TotalEsperado)throw new RegraDeNegocioException("O preço mudou. Consulte o pacote novamente.");
            return o;
        });
    }
    public List<Compra> Reservas(int? clienteId){if(clienteId!=null)Cliente(clienteId.Value);repo.Expirar();return repo.Reservas(clienteId);}
    public Compra Reserva(int id,int? clienteId){if(clienteId!=null)Cliente(clienteId.Value);return repo.Reserva(id,clienteId)??throw new RegraDeNegocioException("Reserva não encontrada.");}
    public List<int> Favoritos(int id)=>repo.Favoritos(id);
    public void Favoritar(int id,int combo,bool adicionar){Cliente(id);repo.Favoritar(id,combo,adicionar);}
    public List<DocumentoViagem> Documentos(int compraId,int? clienteId){var c=Reserva(compraId,clienteId);return c.Status=="Confirmada"?repo.Documentos(compraId):[];}
    public void Emitir(int compraId,DocumentoEntrada dto){var c=Reserva(compraId,null);if(c.Status!="Confirmada")throw new RegraDeNegocioException("A reserva precisa estar confirmada para receber documentos.");if(!Uri.TryCreate(dto.Url,UriKind.Absolute,out var uri)||uri.Scheme!="https")throw new RegraDeNegocioException("Use uma URL HTTPS do documento emitido pelo fornecedor.");repo.Documento(new DocumentoViagem{CompraId=compraId,Tipo=dto.Tipo,Titulo=dto.Titulo,Url=dto.Url,Codigo=dto.Codigo});}
    public bool Cancelar(int id,int clienteId)=>repo.Cancelar(id,clienteId);
}
