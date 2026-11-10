// ALTERAÇÃO PIM: contrato único de reservas para site, API e aplicativo; nunca expõe senha ou navegações EF.
namespace HorizonteViagens.Modelos.Dtos;
public record ReservaDto(int Id,int ComboId,string NomePacote,string Destino,decimal Valor,int Viajantes,int Quartos,int Noites,DateOnly? Partida,string Status,DateTime ExpiraEm,string? PagamentoUrl)
{
 public static ReservaDto De(Compra c)=>new(c.Id,c.ComboId,c.NomePacote,c.Destino,c.Valor,c.Viajantes,c.Quartos,c.Noites,c.Partida,c.Status,c.ExpiraEm,c.PagamentoUrl);
}
