namespace SistemaPDVAssadosTiaDri.Models
{
    public class Venda
    {
        public int VendaId { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal Total { get; set; }
        public List<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
    }

}
