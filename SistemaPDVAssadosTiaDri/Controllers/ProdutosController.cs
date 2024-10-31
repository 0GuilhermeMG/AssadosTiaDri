using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPDVAssadosTiaDri.Models;
using SistemaPDVAssadosTiaDri.Services;

namespace SistemaPDVAssadosTiaDri.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly PDVContext _context;
        private readonly VendaService _vendaService;

        public ProdutosController(PDVContext context, VendaService vendaService)
        {
            _context = context;
            _vendaService = vendaService;
        }

        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("Fui carregado");
            return View(await _context.Produtos.ToListAsync());
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FirstOrDefaultAsync(m => m.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProdutoId,Nome,Preco,CodigoDeBarras")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
        }

        // POST: Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProdutoId,Nome,Preco,CodigoDeBarras")] Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.ProdutoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FirstOrDefaultAsync(m => m.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }



        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.ProdutoId == id);
        }

        // Vendas de Produtos

        // GET: Venda
        public IActionResult Venda()
        {
            ViewBag.Total = _vendaService.CalcularTotal();
            var carrinho = _vendaService.ObterCarrinho() ?? new List<Produto>(); // Garanta que nunca seja nulo
            return View(carrinho);
        }


        // POST: Produtos/AdicionarProduto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdicionarProduto(string codigoBarras)
        {

            Console.WriteLine($"Código de barras recebido: {codigoBarras}");

            var produto = _vendaService.ProcessarCodigoBarras(codigoBarras);
            //Console.WriteLine($"Informações da tentativa de retirar: Nome={produto.Nome}, Preço={produto.Preco}");
            if (produto != null)
            {
                Console.WriteLine($"Produto encontrado: Nome={produto.Nome}, Preço={produto.Preco}"); 

                _vendaService.AdicionarAoCarrinho(produto);
                ViewBag.Total = _vendaService.CalcularTotal();
                return View("Venda", _vendaService.ObterCarrinho());   
            }
            else
            {
                Console.WriteLine("Produto não encontrado"); // Adiciona uma mensagem de log
                ModelState.AddModelError("", "Produto não encontrado.");
                ViewBag.Total = _vendaService.CalcularTotal();
                return View("Venda", _vendaService.ObterCarrinho());
            }

            //return View("Venda", _vendaService.ObterCarrinho());
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarVenda()
        {
            // Crie uma nova venda antes de passar ao serviço
            var venda = new Venda
            {
                DataVenda = DateTime.Now,
                Total = _vendaService.CalcularTotal()
                // Outros campos que você precisar preencher
            };

            var vendaId = _vendaService.FinalizarVendas(venda);
            _vendaService.FinalizarVenda();

            return RedirectToAction("ConfirmacaoVenda", new { id = vendaId });
        }


        public IActionResult ConfirmacaoVenda(int id)
        {
            var venda = _context.Vendas
                .Include(v => v.Itens) // Inclui os itens relacionados à venda
                .ThenInclude(iv => iv.Produto) // Inclui as informações do produto
                .FirstOrDefault(v => v.VendaId == id);

            if (venda == null)
            {
                return NotFound(); // Caso a venda não seja encontrada
            }

            return View(venda); // Retorna a View com os detalhes da venda
        }

        public IActionResult ListarVendas()
        {
            // Pega todas as vendas do banco de dados
            var vendas = _context.Vendas
                .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
                .OrderByDescending(v => v.DataVenda) // Ordena por data da venda em ordem decrescente
                .ToList();

            // Retorna as vendas para a view
            return View(vendas);
        }
       

        //        [HttpPost]
        //        public IActionResult FinalizarVenda()
        //        {
        //            ViewBag.Total = _vendaService.CalcularTotal();
        //            _vendaService.FinalizarVenda();
        //            //return RedirectToAction("Venda");
        //            return View("FinalizarVenda", _vendaService.ObterCarrinho());
        //        }

    }
}
