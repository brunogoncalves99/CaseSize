using CaseSize.DTO;
using CaseSize.Service;
using Microsoft.AspNetCore.Mvc;

namespace CaseSize.Controllers
{

    public class AntecipacaoController : Controller
    {
        private readonly AntecipacaoService _service;

        public AntecipacaoController(AntecipacaoService service)
        {
            _service = service;
        }

        public class AntecipacaoDtoRequest
        {
            public int EmpresaId { get; set; }
            public List<int> NotasFiscaisId { get; set; }
        }

        /// <summary>
        /// Calcula o valor líquido da antecipação para um conjunto de notas fiscais.
        /// </summary>
        [HttpPost]
        [Route("CalcularAntecipacao")]
        public async Task<IActionResult> CalcularAntecipacao([FromBody] AntecipacaoDtoRequest request) 
        {
            if (request.NotasFiscaisId == null || !request.NotasFiscaisId.Any())
            {
                return BadRequest(new { message = Resources.Resources.ErroProcessamentoAntecipacao });
            }

            try
            {
                var resultado = await _service.ProcessamentoAntecipacao(request.EmpresaId, request.NotasFiscaisId);

                if (resultado == null)
                {
                    return NotFound(new { message = Resources.Resources.Empresa_NotasFiscais_NaoEncontradas });
                }
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Resources.Resources.ErroProcessarAntecipacao, details = ex.Message });
            }
        }
    }
}
