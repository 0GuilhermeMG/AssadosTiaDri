using SistemaPDVAssadosTiaDri.Models;
using SistemaPDVAssadosTiaDri.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPDVAssadosTiaDri.Services
{
    public class VendaService
    {
        private readonly PDVContext _context;
        private List<Produto> _carrinho = new List<Produto>();

        public VendaService(PDVContext context)
        {
            _context = context;
        }

        // Processa o código de barras e encontra o produto no banco de dados
        public Produto ProcessarCodigoBarras(string codigoBarras)
        {

            if (codigoBarras.Length == 13)
            {
                
                string codigoProduto = codigoBarras.Substring(1, 6);
                string precoString = codigoBarras.Substring(7, 5);
                decimal preco = decimal.Parse(precoString) / 100;
                decimal precoCarrinho;
                

                var produto = _context.Produtos.FirstOrDefault(p => p.CodigoDeBarras == codigoProduto);
                Console.WriteLine(produto != null ? "Produto encontrado: " + produto.Nome : "Produto não encontrado");
                
                if (produto != null)
                {
                    produto.Preco = preco;
                    return produto;
                }
            }
            return null;
        }

        // Adiciona o produto ao carrinho
        public void AdicionarAoCarrinho(Produto produto)
        {
            _carrinho.Add(produto);
        }

        // Calcula o total dos produtos no carrinho
        public decimal CalcularTotal()
        {
            return _carrinho.Sum(p => p.Preco);
        }

        // Retorna os itens no carrinho
        public List<Produto> ObterCarrinho()
        {
            return _carrinho;
        }
    }
}
