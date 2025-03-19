using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SistemaPDVAssadosTiaDri.Models;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPDVAssadosTiaDri.Services
{
    public class VendaService
    {
        private readonly PDVContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public VendaService(PDVContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private List<Produto> Carrinho
        {
            get
            {
                var data = Session.GetString("Carrinho");
                return string.IsNullOrEmpty(data) ? new List<Produto>() : JsonConvert.DeserializeObject<List<Produto>>(data);
            }
            set
            {
                Session.SetString("Carrinho", JsonConvert.SerializeObject(value));
            }
        }

        public Produto ProcessarCodigoBarras(string codigoBarras)
        {
            if (!string.IsNullOrEmpty(codigoBarras) && codigoBarras.Length == 13)
            {
                string codigoProduto = codigoBarras.Substring(1, 6);
                string precoString = codigoBarras.Substring(7, 5);
                decimal preco = decimal.Parse(precoString) / 100;

                var produto = _context.Produtos.FirstOrDefault(p => p.CodigoDeBarras == codigoProduto);

                if (codigoProduto == "000000") {
                    return new Produto
                    {
                        ProdutoId = produto.ProdutoId,
                        Nome = produto.Nome,
                        Preco = preco * -1
                    };
                }
                if (produto != null)
                {
                    return new Produto
                    {
                        ProdutoId = produto.ProdutoId,
                        Nome = produto.Nome,
                        Preco = preco
                    };
                }

            }
            return null;
        }



        //public int FinalizarVendas(Venda venda)
        //{
        //    _context.Vendas.Add(venda);
        //    _context.SaveChanges();

        //    return venda.VendaId; // Retorna o ID da venda
        //}


        public async Task<int> FinalizarVenda()
        {
            var venda = new Venda
            {
                DataVenda = DateTime.Now,
                Total = CalcularTotal(),
                Itens = Carrinho.Select(p => new ItemVenda
                {
                    ProdutoId = p.ProdutoId,
                    Preco = p.Preco

                }).ToList()
            };

            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();

            LimparCarrinho(); // Limpa o carrinho após finalizar a venda
            return venda.VendaId;
        }

        public void AdicionarAoCarrinho(Produto produto)
        {
            var carrinho = Carrinho;
            carrinho.Add(produto);
            Carrinho = carrinho;
        }

        public decimal CalcularTotal()
        {
            return Carrinho.Sum(p => p.Preco);
        }

        public List<Produto> ObterCarrinho()
        {
            return Carrinho;
        }

        public void LimparCarrinho()
        {
            Carrinho = new List<Produto>();
        }

        public void RemoverItemDoCarrinho(int produtoId, decimal preco)
        {
            var carrinho = Carrinho;
            var produto = carrinho.FirstOrDefault(p => p.ProdutoId == produtoId && p.Preco == preco);
            if (produto != null)
            {
                carrinho.Remove(produto);
                Carrinho = carrinho;
            }
        }



    }
}