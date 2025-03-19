using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPDVAssadosTiaDri.Models;
using SistemaPDVAssadosTiaDri.Services;
using Newtonsoft.Json;

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
                TempData["SuccessMessage"] = "Produto criado com sucesso!";
                return Redirect("/Produtos/Index");
                //return RedirectToAction("Index", "Produtos");
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
                    TempData["SuccessMessage"] = "Produto Editado com sucesso!";

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
                return Redirect("/Produtos/Index");
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
            var itemRelacionado = await _context.ItensVenda.AnyAsync(iv => iv.ProdutoId == id);
            if (itemRelacionado)
            {
                TempData["DeleteMessage"] = "O produto existe em ocorrências na listagem de produtos e não pode ser excluído.";
                return Redirect("/Produtos/Index");
            }

            //if (ProdutoExists(id))
            //{
            //    _context.Produtos.Remove(produto);
            //    await _context.SaveChangesAsync();
            //    TempData["DeleteMessage"] = "Produto Excluído com sucesso!";
            //}
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
                TempData["DeleteMessage"] = "Produto Excluído com sucesso!";

            }

            return Redirect("/Produtos/Index");
            //return RedirectToAction(nameof(Index));
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
            var carrinho = _vendaService.ObterCarrinho() ?? new List<Produto>(); // Garantir que nunca seja nulo
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
                Thread.Sleep(100); //Não apagar linha pois ela permite que o sistema não rode duas vezes
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
            int vendaId = await _vendaService.FinalizarVenda();
            // Crie uma nova venda antes de passar ao serviço
            //var venda = new Venda
            //{
            //    DataVenda = DateTime.Now,
            //    Total = _vendaService.CalcularTotal()
            //  //   Outros campos que você precisar preencher
            //};

            //var vendaId = _vendaService.FinalizarVendas(venda);
            //_vendaService.FinalizarVenda();

            //Testar se funciona antes de dormir pfvr

            return RedirectToAction("ConfirmacaoVenda", new { id = vendaId });
        }


        public IActionResult ConfirmacaoVenda(int id)
        {
            var venda = _context.Vendas
                .Include(v => v.Itens) // Inclui os itens relacionados à venda
                .ThenInclude(i => i.Produto) // Inclui as informações do produto
                .FirstOrDefault(v => v.VendaId == id);

            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        public IActionResult ListarVendas()
        {
            var venda = _context.Vendas
                .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
                .OrderByDescending(v => v.DataVenda)
                .ToList();
            return View(venda);
        }


        [HttpPost]
        public IActionResult RemoverItem(int produtoId, decimal preco)
        {
            _vendaService.RemoverItemDoCarrinho(produtoId, preco);
            return RedirectToAction("Venda");
        }

        [HttpPost]
        public IActionResult ExcluirVenda(int vendaId)
        {
            var venda = _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefault(v => v.VendaId == vendaId);

            if (venda != null)
            {
                _context.Vendas.Remove(venda);
                _context.SaveChanges();
            }
            return RedirectToAction("ListarVendas");
        }

        public async Task<IActionResult> ImprimirVenda(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens)
                .ThenInclude(iv => iv.Produto)
                .FirstOrDefaultAsync(v => v.VendaId == id);
            return View(venda);
        }

        //        [HttpPost]
        //        public IActionResult FinalizarVenda()
        //        {
        //            ViewBag.Total = _vendaService.CalcularTotal();
        //            _vendaService.FinalizarVenda();
        //            //return RedirectToAction("Venda");
        //            return View("FinalizarVenda", _vendaService.ObterCarrinho());
        //        }


        public IActionResult Dashboard()
        {
            // Agrupa as vendas por produto e soma o valor total vendido
            var dados = _context.ItensVenda
                .GroupBy(iv => iv.Produto)
                .Select(g => new
                {
                    Produto = g.Key.Nome,
                    ValorTotal = g.Sum(iv => iv.Preco)
                })
                .ToList();

            ViewBag.Labels = dados.Select(d => d.Produto).ToList();
            ViewBag.Valores = dados.Select(d => d.ValorTotal).ToList();

            Console.WriteLine($"Total de produtos agrupados : {dados.Count}");
            //return Json(dados);
            return View();
        }

        [HttpPost]
        [Route("Produtos/Dashboard/FecharVendas")]
        public IActionResult FecharVendas([FromBody] FechamentoRequest request)
        {
            if (request == null || request.DataInicio == null || request.DataFim == null)
            {
                return BadRequest("Datas inválidas fornecidas.");
            }

            var inicio = request.DataInicio.Value;
            var fim = request.DataFim.Value;

            // Buscar vendas no intervalo especificado
            var vendas = _context.Vendas
                .Where(v => v.DataVenda.Date >= inicio.Date && v.DataVenda.Date <= fim.Date)
                .Include(v => v.Itens)
                .ThenInclude(iv => iv.Produto)
                .ToList();

            var procuraItens = _context.ItensVenda
                .Where(iv => iv.Venda.DataVenda.Date >= inicio.Date && iv.Venda.DataVenda.Date <= fim.Date)
                .GroupBy(iv => iv.Produto.Nome)        
                .Select(g => new
                
                {
                    Produto = g.Key,
                    Valor = g.Sum(iv => iv.Preco),
                    Preco = g.First().Produto.Preco

                }).ToList();

            var precos = _context.Produtos
                .Select(p => p.Preco)
                .ToList();


            Console.WriteLine($"{fim} Esse é o fim");
            Console.WriteLine($"{inicio.Date} Esse é o inicio");

            if (!vendas.Any())
            {
                return Ok(new { mensagem = "Nenhuma venda encontrada no período informado." });
            }

            

            // Calcular o total
            var valorTotal = vendas.Sum(v => v.Total);

            // Assegurar que os dados sejam válidos
            var detalhes = vendas.Select(v => new
            {
                Data = v.DataVenda.ToString("dd/MM/yyyy"), // Formatar a data
                ValorTotal = v.Total, // Certificar que este valor não é nulo
            }).ToList();           

            var produtos = procuraItens.Select(v => new 
            {
                produto = v.Produto,
                valor = v.Valor,
                preco = v.Preco

            }).ToList();

            

            return Ok(new
            {
                produtos,
                valorTotal,              
                detalhes
            });
        }



    }
    public class FechamentoRequest
        {
            public DateTime? DataInicio { get; set; }
            public DateTime? DataFim { get; set; }
        }

    
}
