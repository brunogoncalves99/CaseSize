using CaseSize.DTO;
using CaseSize.Service;
using Microsoft.AspNetCore.Mvc;

namespace CaseSize.Controllers
{
    [ApiController]
    [Route("api/empresas/{cnpj}/notas")]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly AntecipacaoService _service;

        public NotasFiscaisController(AntecipacaoService service) // Injetando o Serviço
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CriarNotaFiscal([FromBody] NotaFiscalDto dto)
        {
            try
            {
                // Chama o serviço para cadastrar a nota fiscal
                var notaFiscal = await _service.CadastrarNotaFiscal(dto);
                return CreatedAtAction(nameof(GetNota), new { id = notaFiscal.NotaFiscalId }, notaFiscal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao cadastrar nota fiscal.", details = ex.Message });
            }
        }

        public async Task<IActionResult> GetNota(int id)
        {
            var notaFiscal = await _service.GetNotaFiscalById(id); 
            if (notaFiscal == null)
            {
                return NotFound();
            }
            return Ok(notaFiscal);
        }
    }
}
