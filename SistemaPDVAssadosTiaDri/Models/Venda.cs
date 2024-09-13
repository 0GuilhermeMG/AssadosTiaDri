namespace SistemaPDVAssadosTiaDri.Models
{
    public class Venda
    {
        public int VendaId { get; set; }
        public DateTime Data { get; set; }
        public decimal Total { get; set; }
        public List<ItemVenda> ItensVenda { get; set; }
    }

}
