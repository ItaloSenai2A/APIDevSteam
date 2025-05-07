using APIDevSteam.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace APIDevSteam.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly APIContext _context;

        public DashBoardController(APIContext context)
        {
            _context = context;
        }

        // Total de Vendas do dia atual, onde vendas é o numero de carrinhos finalizados hoje  
        [HttpGet("TotalVendasDia")]
        public IActionResult TotalVendasDia()
        {
            var totalVendas = _context.Carrinhos
                .Where(c => c.DataFinalizacao == DateTime.Today)
                .Count();
            return Ok(totalVendas);
        }

        // Jogo mais vendido nos ultimos 30 dias  
        [HttpGet("JogoMaisVendido")]
        public IActionResult VendasDia()
        {
            // Buscar os carrinhos finalizados nos ultimos 30 dias  
            var carrinhos = _context.Carrinhos
                .Where(c => c.DataFinalizacao >= DateTime.Today.AddDays(-30))
                .ToList();

            // Trazer a Lista de itensCarrinhos que contenham os ids de carrinhos  
            var carrinhoIds = carrinhos.Select(c => c.CarrinhoId).ToList();
            var itensCarrinhos = _context.ItensCarrinhos
                .Where(ic => carrinhoIds.Contains(ic.CarrinhoId.Value))
                .ToList();

            // Agrupar os itensCarrinhos por JogoId e contar a quantidade de vendas
            var jogoMaisVendido = itensCarrinhos
                .GroupBy(ic => ic.JogoId)
                .Select(g => new
                {
                    JogoId = g.Key,
                    QuantidadeVendida = g.Sum(ic => ic.Quantidade)
                })
                .OrderByDescending(g => g.QuantidadeVendida)
                .FirstOrDefault();

            // Obter o título do jogo mais vendido
            var jogoId = jogoMaisVendido?.JogoId;
            if (jogoId == null)
            {
                return NotFound("Nenhum jogo encontrado.");
            }

            var jogo = _context.Jogos.FirstOrDefault(j => j.JogoId == jogoId);
            if (jogo == null)
            {
                return NotFound("Nenhum jogo encontrado.");
            }

            return Ok(new { jogo.Titulo, jogoMaisVendido.QuantidadeVendida });
        }


    }
}
