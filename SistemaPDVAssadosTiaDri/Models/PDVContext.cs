namespace SistemaPDVAssadosTiaDri.Models
{
    using Microsoft.EntityFrameworkCore;

    public class PDVContext : DbContext
    {
        public PDVContext(DbContextOptions<PDVContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemVenda>()
                .HasOne(iv => iv.Venda)
                .WithMany(v => v.Itens)
                .HasForeignKey(iv => iv.VendaId);

            modelBuilder.Entity<ItemVenda>()
                .HasOne(iv => iv.Produto)
                .WithMany()
                .HasForeignKey(iv => iv.ProdutoId);

            // Definindo precisão para as colunas decimal
            modelBuilder.Entity<ItemVenda>()
                .Property(iv => iv.Preco)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Produto>()
                .Property(p => p.Preco)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Venda>()
                .Property(v => v.Total)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Venda>()
                .HasMany(v => v.Itens)
                .WithOne(iv => iv.Venda)
                .HasForeignKey(iv => iv.VendaId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
