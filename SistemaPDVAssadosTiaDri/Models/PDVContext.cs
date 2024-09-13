namespace SistemaPDVAssadosTiaDri.Models
{
    using Microsoft.EntityFrameworkCore;

    public class PDVContext : DbContext
    {
        public PDVContext(DbContextOptions<PDVContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        
    }
}
