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

            Console.WriteLine($"Código de barras recebido: {codigoBarras}"); // Adiciona uma mensagem de log

            var produto = _vendaService.ProcessarCodigoBarras(codigoBarras);
            //Console.WriteLine($"Informações da tentativa de retirar: Nome={produto.Nome}, Preço={produto.Preco}");
            if (produto != null)
            {
                Console.WriteLine($"Produto encontrado: Nome={produto.Nome}, Preço={produto.Preco}"); // Adiciona uma mensagem de log
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
        }

    }
}
