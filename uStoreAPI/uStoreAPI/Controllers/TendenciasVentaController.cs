using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TendenciasVentaController : ControllerBase
    {
        private readonly TendenciasService tendenciasService;

        public TendenciasVentaController(TendenciasService _tendenciasService)
        {
            tendenciasService = _tendenciasService;
        }

        [HttpPost("GetTendencias")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TendenciaDto>>> GetTendencias([FromBody] filtrosGraficaDto filtros)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var tendencias = await tendenciasService.GetTendencias(filtros);


            return Ok(tendencias);
        }

        [HttpPost("GetTendenciasPerfil")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TendenciaDto>>> GetTendenciasPerfil([FromBody] filtrosGraficaDto filtros, int? idTienda)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var tendencias = await tendenciasService.GetTendenciasAdmin(filtros, idUser, idTienda);

            return Ok(tendencias);
        }

    }
}
