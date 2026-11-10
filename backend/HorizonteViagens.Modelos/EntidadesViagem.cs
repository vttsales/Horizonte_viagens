// ALTERAÇÃO PIM: Campos e contratos de catálogo, orçamento, reserva, documentos e validade de acesso.
using System.ComponentModel.DataAnnotations;

namespace HorizonteViagens.Modelos;

public class Hotel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string? Classificacao { get; set; }
    public ICollection<TelefoneHotel> Telefones { get; set; } = new List<TelefoneHotel>();
    public ICollection<ReservaQuarto> Quartos { get; set; } = new List<ReservaQuarto>();
}

public class TelefoneAdministrador
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int AdministradorId { get; set; }
    public Administrador Administrador { get; set; } = null!;
}

public class TelefoneCliente
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
}

public class TelefoneHotel
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;
}

public class Voo
{
    public int Id { get; set; }
    public string CidadePartida { get; set; } = string.Empty;
    public TimeOnly HoraPartida { get; set; }
    public string CidadeChegada { get; set; } = string.Empty;
    public TimeOnly HoraChegada { get; set; }
    public ICollection<Passagem> Passagens { get; set; } = new List<Passagem>();
}

public class Passagem
{
    public int Id { get; set; }
    public string Classe { get; set; } = string.Empty;
    public string Assento { get; set; } = string.Empty;
    public DateOnly DataIda { get; set; }
    public DateOnly DataVolta { get; set; }
    public int VooId { get; set; }
    public Voo Voo { get; set; } = null!;
    public ICollection<ItemCombo> ItensCombo { get; set; } = new List<ItemCombo>();
}

public class ReservaQuarto
{
    public int Id { get; set; }
    public string NumeroQuarto { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateOnly DataCheckin { get; set; }
    public DateOnly DataCheckout { get; set; }
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;
    public ICollection<ItemCombo> ItensCombo { get; set; } = new List<ItemCombo>();
}

public class Combo
{
    public int Id { get; set; }
    // Campos da vitrine definidos na lista de pendências da Etapa 4.
    public string Nome { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public int Vagas { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public decimal Valor { get; set; }

    public string Origem { get; set; } = "São Paulo";
    public string Evento { get; set; } = "";
    public string Descricao { get; set; } = "";
    public string Inclusoes { get; set; } = "";
    public string Imagem { get; set; } = "beach";
    public int Noites { get; set; } = 1;
    public decimal DiariaQuarto { get; set; }
    public decimal ValorExperiencias { get; set; }
    public DateOnly? Partida { get; set; }
    public bool Publicado { get; set; }
    public bool PrecoConfirmado { get; set; }

    // Concorrência otimista, como na Aula 03 de Desenvolvimento Web.
    // O SQL Server muda este número sozinho a cada alteração da linha. Ao gravar,
    // o EF Core confere se ele ainda é o mesmo que foi lido: se outra pessoa alterou
    // o combo nesse meio-tempo (por exemplo, comprou a última vaga), o EF Core lança
    // DbUpdateConcurrencyException em vez de sobrescrever em silêncio.
    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;

    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
    public ICollection<ItemCombo> Itens { get; set; } = new List<ItemCombo>();
}

public class Compra
{
    public int Id { get; set; }
    // Compra sem combo foi tratada como inválida, conforme recomendação do PDF.
    public int ComboId { get; set; }
    public Combo Combo { get; set; } = null!;
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public decimal Valor { get; set; }
    public int Viajantes { get; set; } = 1;
    public int Quartos { get; set; } = 1;
    public int Noites { get; set; } = 1;
    public DateOnly? Partida { get; set; }
    public string NomePacote { get; set; } = "";
    public string Destino { get; set; } = "";
    public string Status { get; set; } = "Pendente";
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
    public DateTime ExpiraEm { get; set; } = DateTime.UtcNow.AddMinutes(30);
    public Guid RequisicaoId { get; set; } = Guid.NewGuid();
    public string? PreferenciaId { get; set; }
    public string? PagamentoUrl { get; set; }
    public string? PagamentoId { get; set; }
    [Timestamp] public byte[] RowVersion { get; set; } = default!;
    public ICollection<MonitoramentoCompra> Monitoramentos { get; set; } = new List<MonitoramentoCompra>();
}

public class MonitoramentoCompra
{
    public int AdministradorId { get; set; }
    public Administrador Administrador { get; set; } = null!;
    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;
}

public class Passeio
{
    public int Id { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public DateTime Horario { get; set; }
    public int Capacidade { get; set; }
    public ICollection<ItemCombo> ItensCombo { get; set; } = new List<ItemCombo>();
}

public class ItemCombo
{
    public int QuartoId { get; set; }
    public ReservaQuarto Quarto { get; set; } = null!;
    public int PassagemId { get; set; }
    public Passagem Passagem { get; set; } = null!;
    public int PasseioId { get; set; }
    public Passeio Passeio { get; set; } = null!;
    public int ComboId { get; set; }
    public Combo Combo { get; set; } = null!;
}
