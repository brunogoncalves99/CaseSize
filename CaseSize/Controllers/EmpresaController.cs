using CaseSize.Context;
using CaseSize.DTO;
using CaseSize.Entitades;
using CaseSize.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaseSize.Controllers
{
    [ApiController]
    [Route("api/empresas")]
    public class EmpresaController : ControllerBase
    {

        private readonly AntecipacaoService _service; 

        public EmpresaController(AntecipacaoService service) // Injetando o Serviço
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CriarEmpresa([FromBody] EmpresaDto dto)
        {
            try
            {
                var empresa = await _service.CadastrarEmpresa(dto);
                return CreatedAtAction(nameof(Get), new { id = empresa.EmpresaId }, empresa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao cadastrar empresa.", details = ex.Message });
            }
        }

        public async Task<IActionResult> Get(int id)
        {
            var empresa = await _service.GetEmpresaById(id); // Usando o Serviço para obter a empresa
            if (empresa == null)
            {
                return NotFound();
            }
            return Ok(empresa);
        }
    }
}
