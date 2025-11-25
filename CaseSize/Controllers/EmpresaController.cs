using CaseSize.Context;
using CaseSize.DTO;
using CaseSize.Entitades;
using CaseSize.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaseSize.Controllers
{
    [ApiController]
    [Route("api/v1/Empresa")]
    public class EmpresaController : ControllerBase
    {

        private readonly EmpresaService _service; 

        public EmpresaController(EmpresaService service) 
        {
            _service = service;
        }

        /// <summary>
        /// Criação de uma nova empresa
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>]
        /// 
        [HttpPost]
        [Route("CriarEmpresa")]
        public async Task<IActionResult> CriarEmpresa([FromBody] EmpresaDto dto)
        {
            try
            {
                var empresa = await _service.CadastrarEmpresa(dto);
                return CreatedAtAction(nameof(Get), new { id = empresa.EmpresaId }, empresa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Resources.Resources.ErroCadastroEmpresa, details = ex.Message });
            }
        }


        // <summary>
        /// Buscar uma empresa pelo id
        /// </summary>
        [HttpGet]
        [Route("empresaId")]
        public async Task<IActionResult> Get(int empresaId)
        {
            var empresa = await _service.GetEmpresaById(empresaId);
            if (empresa == null)
                throw new ApplicationException(Resources.Resources.EmpresaNaoEncontrada);

            return Ok(empresa);
        }
    }
}
