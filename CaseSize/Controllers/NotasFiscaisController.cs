using CaseSize.DTO;
using CaseSize.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseSize.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/NotaFiscal")]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly AntecipacaoService _service;

        public NotasFiscaisController(AntecipacaoService service) // Injetando o Serviço
        {
            _service = service;
        }

        /// <summary>
        /// Criação de notas fiscais para uma empresa específica.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CriarNotaFiscal")]
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
                return StatusCode(500, new { message = Resources.Resources.ErroCadastroNotaFiscal, details = ex.Message });
            }
        }

        /// <summary>
        /// Buscar Nota Fiscal pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("buscarNota")]
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
