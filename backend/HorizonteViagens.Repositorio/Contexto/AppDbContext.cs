// ALTERAÇÃO PIM: Mapeamento e persistência do esquema integrado.
using Microsoft.EntityFrameworkCore;
using HorizonteViagens.Modelos;

namespace HorizonteViagens.Repositorio.Contexto;

// O DbContext é a "porta" do Entity Framework Core para o banco.
// Cada DbSet vira uma tabela.
public class AppDbContext : DbContext
{
    // As "options" trazem qual banco usar (SQL Server) e a connection string.
    // Elas são montadas pela linha AddDbContext do Program.cs e entregues aqui.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Repare: NÃO existe DbSet<Usuario>. Como a classe mãe não está aqui,
    // o EF cria duas tabelas independentes, e cada uma recebe as colunas
    // herdadas de Usuario (Nome, Email, SenhaHash, Ativo).
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<Hotel> Hoteis => Set<Hotel>();
    public DbSet<TelefoneAdministrador> TelefonesAdministradores => Set<TelefoneAdministrador>();
    public DbSet<TelefoneCliente> TelefonesClientes => Set<TelefoneCliente>();
    public DbSet<TelefoneHotel> TelefonesHoteis => Set<TelefoneHotel>();
    public DbSet<Voo> Voos => Set<Voo>();
    public DbSet<Passagem> Passagens => Set<Passagem>();
    public DbSet<ReservaQuarto> ReservasQuartos => Set<ReservaQuarto>();
    public DbSet<Combo> Combos => Set<Combo>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<MonitoramentoCompra> MonitoramentosCompras => Set<MonitoramentoCompra>();
    public DbSet<Passeio> Passeios => Set<Passeio>();
    public DbSet<ItemCombo> ItensCombos => Set<ItemCombo>();

    public DbSet<Favorito> Favoritos => Set<Favorito>();
    public DbSet<DocumentoViagem> Documentos => Set<DocumentoViagem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Favorito>(e => {
            e.HasKey(x=>new {x.ClienteId,x.ComboId});
            e.HasOne<Cliente>().WithMany().HasForeignKey(x=>x.ClienteId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<Combo>().WithMany().HasForeignKey(x=>x.ComboId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<DocumentoViagem>(e => {
            e.HasOne<Compra>().WithMany().HasForeignKey(x=>x.CompraId).OnDelete(DeleteBehavior.Restrict);
            e.Property(x=>x.Tipo).HasMaxLength(40); e.Property(x=>x.Titulo).HasMaxLength(140);
            e.Property(x=>x.Url).HasMaxLength(1000); e.Property(x=>x.Codigo).HasMaxLength(100);
        });
        modelBuilder.Entity<Cliente>(e =>
        {
            e.ToTable("Clientes", t =>
            {
                // OBRIGATÓRIO a partir do EF Core 7 em tabela que tem trigger.
                // Sem esta linha, todo INSERT/UPDATE em Clientes dá erro,
                // porque o EF usa OUTPUT, que o SQL Server não aceita com trigger.
                t.HasTrigger("TR_Clientes_DataAtualizacao");
            });

            e.HasKey(c => c.Id);
            e.Property(c => c.Nome).HasMaxLength(100).IsRequired();
            e.Property(c => c.Email).HasMaxLength(150).IsRequired();
            e.Property(c => c.Cpf).HasMaxLength(11).IsFixedLength().IsRequired();
            e.Property(c => c.Celular).HasMaxLength(11).IsFixedLength().IsRequired();
            e.Property(c => c.Cep).HasColumnName("CEP").HasMaxLength(9).IsUnicode(false);
            e.Property(c => c.Rua).HasMaxLength(100);
            e.Property(c => c.Bairro).HasMaxLength(50);
            e.Property(c => c.Cidade).HasMaxLength(50);
            e.Property(c => c.SenhaHash).HasMaxLength(200).IsRequired();
            e.Property(c => c.DataCadastro).HasDefaultValueSql("GETDATE()");

            // Perfil é só um texto calculado no C# ("Cliente"); não vira coluna.
            e.Ignore(c => c.Perfil);

            // Índices únicos: o banco também barra e-mail e CPF repetidos.
            e.HasIndex(c => c.Email).IsUnique();
            e.HasIndex(c => c.Cpf).IsUnique();
        });

        modelBuilder.Entity<Administrador>(e =>
        {
            e.ToTable("Administradores");

            e.HasKey(a => a.Id);
            e.Property(a => a.Nome).HasMaxLength(100).IsRequired();
            e.Property(a => a.Email).HasMaxLength(150).IsRequired();
            e.Property(a => a.SenhaHash).HasMaxLength(200).IsRequired();
            e.Ignore(a => a.Perfil);
            e.HasIndex(a => a.Email).IsUnique();

            // Administrador inicial: sem ele, ninguém consegue fazer o primeiro login.
            //   E-mail: admin@horizonteviagens.com
            //   Senha:  Horizonte@2026
            // O valor abaixo é o HASH dessa senha (gerado pelo SenhaHasher), não a senha.
            // Para usar outra senha: gere um novo hash com SenhaHasher.Gerar("nova senha"),
            // cole aqui no lugar deste e gere uma migration nova.
            e.HasData(new Administrador
            {
                Id = 1,
                Nome = "Administrador",
                Email = "admin@horizonteviagens.com",
                SenhaHash = "100000.R03eHwaH3+lUVmH+AEHXwg==.sPZ3h/VnYltKhLwXGC43dpu9Ck/HZ65KF4i87G7148g=",
                Ativo = true
            });
        });

        modelBuilder.Entity<Hotel>(e =>
        {
            e.ToTable("Hoteis");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            e.Property(x => x.Endereco).HasMaxLength(200).IsRequired();
            e.Property(x => x.Classificacao).HasMaxLength(20);
        });

        modelBuilder.Entity<TelefoneAdministrador>(e =>
        {
            e.ToTable("TelefonesAdministradores");
            e.HasKey(x => x.Id);
            e.Property(x => x.Numero).HasMaxLength(20).IsUnicode(false).IsRequired();
            e.HasOne(x => x.Administrador).WithMany().HasForeignKey(x => x.AdministradorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TelefoneCliente>(e =>
        {
            e.ToTable("TelefonesClientes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Numero).HasMaxLength(20).IsUnicode(false).IsRequired();
            e.HasOne(x => x.Cliente).WithMany(x => x.Telefones).HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TelefoneHotel>(e =>
        {
            e.ToTable("TelefonesHoteis");
            e.HasKey(x => x.Id);
            e.Property(x => x.Numero).HasMaxLength(20).IsUnicode(false).IsRequired();
            e.HasOne(x => x.Hotel).WithMany(x => x.Telefones).HasForeignKey(x => x.HotelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Voo>(e =>
        {
            e.ToTable("Voos");
            e.HasKey(x => x.Id);
            e.Property(x => x.CidadePartida).HasMaxLength(100).IsRequired();
            e.Property(x => x.CidadeChegada).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Passagem>(e =>
        {
            e.ToTable("Passagens");
            e.HasKey(x => x.Id);
            e.Property(x => x.Classe).HasMaxLength(20).IsRequired();
            e.Property(x => x.Assento).HasMaxLength(10).IsRequired();
            e.HasOne(x => x.Voo).WithMany(x => x.Passagens).HasForeignKey(x => x.VooId)
                .OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t => t.HasCheckConstraint("CK_Passagens_Periodo", "[DataVolta] >= [DataIda]"));
        });

        modelBuilder.Entity<ReservaQuarto>(e =>
        {
            e.ToTable("ReservasQuartos");
            e.HasKey(x => x.Id);
            e.Property(x => x.NumeroQuarto).HasMaxLength(10).IsRequired();
            e.Property(x => x.Tipo).HasMaxLength(30).IsRequired();
            e.HasOne(x => x.Hotel).WithMany(x => x.Quartos).HasForeignKey(x => x.HotelId)
                .OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t => t.HasCheckConstraint("CK_ReservasQuartos_Periodo", "[DataCheckout] > [DataCheckin]"));
        });

        modelBuilder.Entity<Combo>(e =>
        {
            e.ToTable("Combos", t => t.HasCheckConstraint("CK_Combos_Valor", "[Valor] >= 0"));
            e.HasKey(x => x.Id);
            e.Property(x=>x.DiariaQuarto).HasPrecision(10,2);
            e.Property(x=>x.ValorExperiencias).HasPrecision(10,2);
            e.Property(x=>x.Origem).HasMaxLength(100); e.Property(x=>x.Evento).HasMaxLength(140);
            e.Property(x=>x.Descricao).HasMaxLength(2000); e.Property(x=>x.Inclusoes).HasMaxLength(2000);
            e.Property(x=>x.Imagem).HasMaxLength(20);
            e.ToTable(t=>t.HasCheckConstraint("CK_Combos_Precificacao","[Noites] BETWEEN 1 AND 60 AND [DiariaQuarto] >= 0 AND [ValorExperiencias] >= 0"));
            // Restrições do catálogo e do estoque, com nomes e limites do domínio.
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            e.Property(x => x.Destino).HasMaxLength(100).IsRequired();
            e.Property(x => x.Categoria).HasMaxLength(30).IsRequired();
            e.ToTable(t => t.HasCheckConstraint("CK_Combos_Vagas", "[Vagas] >= 0"));
            e.Property(x => x.Valor).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Compra>(e =>
        {
            e.ToTable("Compras", t => t.HasCheckConstraint("CK_Compras_Valor", "[Valor] >= 0"));
            e.HasKey(x => x.Id);
            e.Property(x => x.Valor).HasPrecision(10, 2);
            e.HasIndex(x=>new {x.ClienteId,x.RequisicaoId}).IsUnique();
            e.HasIndex(x=>x.PagamentoId).IsUnique().HasFilter("[PagamentoId] IS NOT NULL");
            e.Property(x=>x.Status).HasMaxLength(30);
            e.Property(x=>x.NomePacote).HasMaxLength(100); e.Property(x=>x.Destino).HasMaxLength(100);
            e.Property(x=>x.PreferenciaId).HasMaxLength(100); e.Property(x=>x.PagamentoId).HasMaxLength(100);
            e.Property(x=>x.PagamentoUrl).HasMaxLength(2000);
            e.ToTable(t=>t.HasCheckConstraint("CK_Compras_Viajantes","[Viajantes] BETWEEN 1 AND 6"));
            // ComboId agora é obrigatório: compra avulsa não atende o fluxo do domínio.
            e.HasOne(x => x.Combo).WithMany(x => x.Compras).HasForeignKey(x => x.ComboId)
                .IsRequired().OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MonitoramentoCompra>(e =>
        {
            e.ToTable("MonitoramentosCompras");
            e.HasKey(x => new { x.AdministradorId, x.CompraId });
            e.HasOne(x => x.Administrador).WithMany().HasForeignKey(x => x.AdministradorId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Compra).WithMany(x => x.Monitoramentos).HasForeignKey(x => x.CompraId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Passeio>(e =>
        {
            e.ToTable("Passeios", t => t.HasCheckConstraint("CK_Passeios_Capacidade", "[Capacidade] > 0"));
            e.HasKey(x => x.Id);
            e.Property(x => x.Lugar).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<ItemCombo>(e =>
        {
            e.ToTable("ItensCombos");
            e.HasKey(x => new { x.QuartoId, x.PassagemId, x.PasseioId, x.ComboId });
            e.HasOne(x => x.Quarto).WithMany(x => x.ItensCombo).HasForeignKey(x => x.QuartoId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Passagem).WithMany(x => x.ItensCombo).HasForeignKey(x => x.PassagemId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Passeio).WithMany(x => x.ItensCombo).HasForeignKey(x => x.PasseioId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Combo).WithMany(x => x.Itens).HasForeignKey(x => x.ComboId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
