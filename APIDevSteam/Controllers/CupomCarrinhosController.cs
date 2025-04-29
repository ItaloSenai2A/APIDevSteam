using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIDevSteam.Data;
using APIDevSteam.Models;
using APIDevSteamJau.Models;

namespace APIDevSteam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CupomCarrinhosController : ControllerBase
    {
        private readonly APIContext _context;

        public CupomCarrinhosController(APIContext context)
        {
            _context = context;
        }

        // GET: api/CupomCarrinhos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CupomCarrinho>>> GetCupomCarrinho()
        {
            return await _context.CupomCarrinho.ToListAsync();
        }

        // GET: api/CupomCarrinhos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CupomCarrinho>> GetCupomCarrinho(Guid id)
        {
            var cupomCarrinho = await _context.CupomCarrinho.FindAsync(id);

            if (cupomCarrinho == null)
            {
                return NotFound();
            }

            return cupomCarrinho;
        }

        // PUT: api/CupomCarrinhos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCupomCarrinho(Guid id, CupomCarrinho cupomCarrinho)
        {
            if (id != cupomCarrinho.CupomCarrinhoId)
            {
                return BadRequest();
            }

            _context.Entry(cupomCarrinho).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CupomCarrinhoExists(id))
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

        // POST: api/CupomCarrinhos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CupomCarrinho>> PostCupomCarrinho(CupomCarrinho cupomCarrinho)
        {
            _context.CupomCarrinho.Add(cupomCarrinho);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCupomCarrinho", new { id = cupomCarrinho.CupomCarrinhoId }, cupomCarrinho);
        }

        // DELETE: api/CupomCarrinhos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCupomCarrinho(Guid id)
        {
            var cupomCarrinho = await _context.CupomCarrinho.FindAsync(id);
            if (cupomCarrinho == null)
            {
                return NotFound();
            }

            _context.CupomCarrinho.Remove(cupomCarrinho);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CupomCarrinhoExists(Guid id)
        {
            return _context.CupomCarrinho.Any(e => e.CupomCarrinhoId == id);
        }
        [HttpPost("AplicarCupom")]
        public async Task<IActionResult> AplicarCupom(Guid carrinhoId, Guid cupomId)
        {
            // Verifica se o carrinho existe
            var carrinho = await _context.Carrinhos.FindAsync(carrinhoId);
            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado.");
            }

            // Verifica se o cupom existe
            var cupom = await _context.Cupons.FindAsync(cupomId);
            if (cupom == null)
            {
                return NotFound("Cupom não encontrado.");
            }

            // Verifica se o cupom é válido e está ativo
            if (!cupom.Ativo.HasValue || !cupom.Ativo.Value ||
                (cupom.DataValidade.HasValue && cupom.DataValidade.Value < DateTime.Now))
            {
                return BadRequest("Cupom inválido ou expirado.");
            }

            // Calcula o desconto no total do carrinho
            var desconto = (carrinho.ValorTotal * cupom.Desconto) / 100;
            carrinho.ValorTotal -= desconto;

            // Salva as alterações no banco de dados
            try
            {
                _context.Entry(carrinho).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao aplicar o cupom: {ex.Message}");
            }

            return Ok(new
            {
                Mensagem = "Cupom aplicado com sucesso.",
                CarrinhoId = carrinho.CarrinhoId,
                ValorTotalAtualizado = carrinho.ValorTotal,
                DescontoAplicado = desconto
            });
        }
    }
}