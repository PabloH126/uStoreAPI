using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodosPredeterminadosController : ControllerBase
    {
        private readonly PeriodosPredeterminadosService periodosPredeterminadosService;
        private readonly TiendasService tiendasService;
        private IMapper mapper;
        public PeriodosPredeterminadosController(PeriodosPredeterminadosService _periodosPredeterminadosService, TiendasService _tiendasService, IMapper _mapper)
        {
            periodosPredeterminadosService = _periodosPredeterminadosService;
            tiendasService = _tiendasService;
            mapper = _mapper;
        }

        [HttpGet("GetPeriodos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PeriodosPredeterminadosDto>>> GetPeriodosPredeterminados(int idTienda)
        {
            var periodos = await periodosPredeterminadosService.GetPeriodosPredeterminados(idTienda);
            return Ok(periodos);
        }

        [HttpGet(Name = "GetPeriodo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PeriodosPredeterminadosDto>> GetPeriodoPredeterminado(int id)
        {
            var periodo = await periodosPredeterminadosService.GetOnePeriodoPredeterminado(id);
            if (periodo is null)
            {
                return NotFound("Periodo no registrado");
            }
            return Ok(periodo);
        }

        [HttpPost("CreatePeriodos")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PeriodosPredeterminadosDto>>> CreatePeriodosTienda([FromBody] IEnumerable<PeriodosPredeterminadosDto> periodosDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (await tiendasService.GetOneTienda(periodosDto.FirstOrDefault()!.IdTienda) is null)
            {
                return NotFound("No se encontro una tienda registrada");
            }

            var periodos = mapper.Map<IEnumerable<PeriodosPredeterminado>>(periodosDto);
            await periodosPredeterminadosService.CreateAllPeriodoPredeterminado(periodos);
            return Ok();
        }

        [HttpDelete("DeletePeriodo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePeriodo(int id)
        {
            if (id == 0)
            {
                return BadRequest("Id invalido");
            }
            var periodo = await periodosPredeterminadosService.GetOnePeriodoPredeterminado(id);
            if (periodo is null)
            {
                return NotFound("Periodo no registrado");
            }
            await periodosPredeterminadosService.DeletePeriodoPredeterminado(periodo);
            return NoContent();
        }

        [HttpPut("UpdatePeriodo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategoria([FromBody] PeriodosPredeterminadosDto periodo)
        {
            if (periodo is null)
            {
                return BadRequest("Periodo no valido");
            }
            if(await tiendasService.GetOneTienda(periodo.IdTienda) is null)
            {
                return NotFound("No hay una tienda registrada con ese id");
            }
            await periodosPredeterminadosService.UpdatePeriodoPredeterminado(mapper.Map<PeriodosPredeterminado>(periodo));

            return NoContent();
        }
    }
}
