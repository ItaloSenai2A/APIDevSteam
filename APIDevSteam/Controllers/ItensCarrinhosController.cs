﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIDevSteam.Data;
using APIDevSteam.Models;


namespace APIDevSteam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItensCarrinhosController : ControllerBase
    {
        private readonly APIContext _context;

        public ItensCarrinhosController(APIContext context)
        {
            _context = context;
        }

        // GET: api/ItensCarrinhos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemCarrinho>>> GetItensCarrinhos()
        {
            return await _context.ItensCarrinhos.ToListAsync();
        }

        // GET: api/ItensCarrinhos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemCarrinho>> GetItemCarrinho(Guid id)
        {
            var itemCarrinho = await _context.ItensCarrinhos.FindAsync(id);

            if (itemCarrinho == null)
            {
                return NotFound();
            }

            return itemCarrinho;
        }

        // PUT: api/ItensCarrinhos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemCarrinho(Guid id, ItemCarrinho itemCarrinho)
        {
            if (id != itemCarrinho.ItemCarrinhoId)
            {
                return BadRequest();
            }

            _context.Entry(itemCarrinho).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemCarrinhoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ItensCarrinhos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ItemCarrinho>> PostItemCarrinho(ItemCarrinho itemCarrinho)
        {
            // Verifica se o carrinho existe
            var carrinho = await _context.Carrinhos.FindAsync(itemCarrinho.CarrinhoId);
            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado.");
            }

            // Verifica se o jogo existe
            var jogo = await _context.Jogos.FindAsync(itemCarrinho.JogoId);
            if (jogo == null)
            {
                return NotFound("Jogo não encontrado.");
            }

            // Calcula o valor total
            itemCarrinho.ValorTotal = itemCarrinho.Quantidade * jogo.Preco;

            // Adiciona o valor total ao carrinho
            carrinho.ValorTotal += itemCarrinho.ValorTotal;

            _context.ItensCarrinhos.Add(itemCarrinho);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItemCarrinho", new { id = itemCarrinho.ItemCarrinhoId }, itemCarrinho);
        }

        // DELETE: api/ItensCarrinhos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemCarrinho(Guid id)
        {
            var itemCarrinho = await _context.ItensCarrinhos.FindAsync(id);
            if (itemCarrinho == null)
            {
                return NotFound();
            }

            // Verificar se o Carrinho existe
            var carrinho = await _context.Carrinhos.FindAsync(itemCarrinho.CarrinhoId);
            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado.");
            }

            // Remove o valor total do carrinho
            carrinho.ValorTotal -= itemCarrinho.ValorTotal;

            // Verifica se o valor total do carrinho é menor que zero
            if (carrinho.ValorTotal < 0)
            {
                carrinho.ValorTotal = 0;
            }

            _context.ItensCarrinhos.Remove(itemCarrinho);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemCarrinhoExists(Guid id)
        {
            return _context.ItensCarrinhos.Any(e => e.ItemCarrinhoId == id);
        }

        // [HttpGET] : Buscar os itens mais vendidos
        [HttpGet("ListarItensMaisVendidos")]
        public async Task<ActionResult<IEnumerable<object>>> GetTopSellingItems([FromQuery] int top)
        {
            if (top <= 0)
                return BadRequest("O parâmetro 'top' deve ser maior que zero.");

            // Busca os itens de carrinhos finalizados
            var topSellingItems = await _context.ItensCarrinhos
                .Include(ic => ic.Carrinho) // Inclui os dados do carrinho
                .Include(ic => ic.Jogo) // Inclui os dados do jogo
                .Where(ic => ic.Carrinho != null && ic.Carrinho.Finalizado == true) // Verifica se o carrinho está finalizado
                .GroupBy(ic => new { ic.JogoId, ic.Jogo.Titulo }) // Agrupa por JogoId e Título do jogo
                .Select(group => new
                {
                    JogoId = group.Key.JogoId,
                    Titulo = group.Key.Titulo,
                    QuantidadeVendida = group.Sum(ic => ic.Quantidade) // Soma a quantidade vendida
                })
                .OrderByDescending(item => item.QuantidadeVendida) // Ordena pela quantidade vendida em ordem decrescente
                .Take(top) // Limita ao número de itens mais vendidos informado no parâmetro
                .ToListAsync();

            return Ok(topSellingItems);
        }

        // Aumentar a quantidade de um item no carrinho
        [HttpPut("AumentarQuantidade/{id}")]
        public async Task<IActionResult> AumentarQuantidade(Guid id)
        {
            var itemCarrinho = await _context.ItensCarrinhos.FindAsync(id);
            if (itemCarrinho == null)
            {
                return NotFound();
            }

            // Atualiza a quantidade e o valor total
            itemCarrinho.Quantidade++;
            itemCarrinho.ValorTotal = itemCarrinho.Quantidade * itemCarrinho.ValorUnitario;

            // Atualiza o carrinho
            var carrinho = await _context.Carrinhos.FindAsync(itemCarrinho.CarrinhoId);
            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado.");
            }
            // Atualiza o valor total do carrinho
            carrinho.ValorTotal += itemCarrinho.ValorTotal;

            _context.Entry(carrinho).State = EntityState.Modified;
            _context.Entry(itemCarrinho).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Aumentar a quantidade de um item no carrinho
        [HttpPut("DiminuirQuantidade/{id}")]
        public async Task<IActionResult> DiminuirQuantidade(Guid id)
        {
            var itemCarrinho = await _context.ItensCarrinhos.FindAsync(id);
            if (itemCarrinho == null)
            {
                return NotFound();
            }

            // Atualiza a quantidade e o valor total
            itemCarrinho.Quantidade--;
            if (itemCarrinho.Quantidade < 0)
            {
                itemCarrinho.Quantidade = 0;
            }
            itemCarrinho.ValorTotal = itemCarrinho.Quantidade * itemCarrinho.ValorUnitario;

            // Atualiza o carrinho
            var carrinho = await _context.Carrinhos.FindAsync(itemCarrinho.CarrinhoId);
            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado.");
            }
            // Atualiza o valor total do carrinho
            carrinho.ValorTotal -= itemCarrinho.ValorTotal;

            _context.Entry(carrinho).State = EntityState.Modified;
            _context.Entry(itemCarrinho).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}