using _123Vendas.Core.Business;
using _123Vendas.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace _123Vendas.API.Controllers
{
    [ApiController]
    [Route("api/123Vendas/[controller]")]
    public class VendaController : ControllerBase
    {
        private readonly IVendaBusiness _vendaBusiness;

        public VendaController(IVendaBusiness vendaBusiness)
        {
            _vendaBusiness = vendaBusiness;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CriarVenda([FromBody] VendaDto vendaDto)
        {
            var venda = await _vendaBusiness.CriarVendaAsync(vendaDto);
            return CreatedAtAction(nameof(GetVenda), new { id = venda.Id }, venda);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> ObterTodasVendas()
        {
            var vendas = await _vendaBusiness.ObterTodasVendasAsync();
            if (vendas == null || vendas.Count == 0)
                return NotFound("Nenhuma venda encontrada");

            return Ok(vendas);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetVenda(int id)
        {
            var venda = await _vendaBusiness.GetByIdAsync(id);
            if (venda == null)
                return NotFound("Venda não encontrada");

            return Ok(venda);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> AtualizarVenda(int id, [FromBody] VendaDto vendaDto)
        {
            if (id != vendaDto.Id)
                return BadRequest("ID da venda não corresponde");

            try
            {
                await _vendaBusiness.AtualizarVendaAsync(vendaDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("CancelItem/{vendaId}/{itemId}")]
        public async Task<IActionResult> CancelarItem(int vendaId, int itemId)
        {
            try
            {
                await _vendaBusiness.CancelarItemAsync(vendaId, itemId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("CancelVenda/{id}")]
        public async Task<IActionResult> CancelarVenda(int id)
        {
            try
            {
                await _vendaBusiness.CancelarVendaAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
