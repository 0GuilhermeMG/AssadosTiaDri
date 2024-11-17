using Xunit;
using System.Collections.Generic;

public class CarrinhoTest
{
    private class Produto
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public string CodigoDeBarras { get; set; }
    }

    private class CarrinhoService
    {
        public List<Produto> Carrinho { get; set; } = new List<Produto>();

        public void AdicionarAoCarrinho(Produto produto)
        {
            var carrinho = Carrinho;
            carrinho.Add(produto);
            Carrinho = carrinho;
        }

        public void LimparCarrinho()
        {
            Carrinho = new List<Produto>();
        }
    }

    [Fact]
    public void AdicionarAoCarrinho_DeveAdicionarProduto()
    {
        // Arrange
        var carrinhoService = new CarrinhoService();
        var produto = new Produto { ProdutoId = 1, Nome = "Produto Teste", Preco = 10.00m, CodigoDeBarras = "001000"};

        // Act
        carrinhoService.AdicionarAoCarrinho(produto);

        // Assert
        Assert.Single(carrinhoService.Carrinho);  // Verifica que há 1 produto no carrinho
        Assert.Equal(produto, carrinhoService.Carrinho[0]);  // Verifica se o produto adicionado é o esperado
    }

    [Fact]
    public void LimparCarrinho_DeveEsvaziarCarrinho()
    {
        var carrinhoService = new CarrinhoService();
        var produto1 = new Produto { ProdutoId = 1, Nome = "Batata", Preco = 50.00m, CodigoDeBarras = "003700" };
        var produto2 = new Produto { ProdutoId = 2, Nome = "Farinha", Preco = 60.00m, CodigoDeBarras = "003900" };

        carrinhoService.AdicionarAoCarrinho(produto1);
        carrinhoService.AdicionarAoCarrinho(produto2);

        carrinhoService.LimparCarrinho();

        Assert.Empty(carrinhoService.Carrinho);

    }
}
