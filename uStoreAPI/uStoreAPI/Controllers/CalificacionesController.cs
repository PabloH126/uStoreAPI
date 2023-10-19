using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CalificacionesController : ControllerBase
    {
        private readonly CalificacionesService calificacionesService;
        private readonly ProductosService productosService;
        private readonly TiendasService tiendasService;
        private IMapper mapper;

        public CalificacionesController(CalificacionesService _calificacionesService, ProductosService _productosService, TiendasService _tiendasService, IMapper _mapper)
        {
            calificacionesService = _calificacionesService;
            productosService = _productosService;
            tiendasService = _tiendasService;
            mapper = _mapper;
        }

        #region GetAll
        [HttpGet("GetCalificacionesTienda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CalificacionTiendaDto>>> GetCalificacionesTienda(int idTienda)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            string? idTiendaClaim = user.Claims.FirstOrDefault(u => u.Type == "IdTienda")?.Value;
            int idTiendaClaimValue = 0;
            int.TryParse(idTiendaClaim, out idTiendaClaimValue);
            var tienda = await tiendasService.GetOneTienda(idTienda);

            if (tienda is null)
            {
                return BadRequest("No hay una tienda registrada con ese id");
            }
            else if (tienda.IdAdministrador != idUser && idTiendaClaimValue != tienda.IdTienda)
            {
                return Unauthorized("Tienda no autorizada");
            }

            var calificaciones = await calificacionesService.GetCalificacionesTienda(idTienda);
            var calificacionesSalida = mapper.Map<IEnumerable<CalificacionTiendaDto>>(calificaciones);

            if(calificacionesSalida.IsNullOrEmpty())
            {
                return Ok("No hay calificaciones registradas para esta tienda");
            }
            return Ok(calificacionesSalida);
        }

        [HttpGet("GetCalificacionesProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CalificacionProductoDto>>> GetCalificacionesProducto(int idProducto)
        {
            var producto = await productosService.GetOneProducto(idProducto);

            if (producto is null)
            {
                return BadRequest("No hay un producto registrado con ese id");
            }

            var calificaciones = await calificacionesService.GetCalificacionesProducto(idProducto);
            var calificacionesSalida = mapper.Map<IEnumerable<CalificacionProductoDto>>(calificaciones);

            if (calificacionesSalida.IsNullOrEmpty())
            {
                return Ok("No hay calificaciones registradas para este producto");
            }

            return Ok(calificacionesSalida);
        }
        #endregion

        #region GetOne
        [HttpGet("GetCalificacionTienda", Name = "GetCalificacionTienda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CalificacionTiendaDto>> GetCalificacionTienda(int id)
        {
            var calificacion = await calificacionesService.GetOneCalificacionTienda(id);
            var calificacionSalida = mapper.Map<CalificacionTiendaDto>(calificacion);
            if (calificacion is null)
            {
                return NotFound("Calificacion no existente");
            }
            return Ok(calificacionSalida);
        }

        [HttpGet("GetCalificacionProducto", Name = "GetCalificacionProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CalificacionProductoDto>> GetCalificacionProducto(int id)
        {
            var calificacion = await calificacionesService.GetOneCalificacionProducto(id);
            var calificacionSalida = mapper.Map<CalificacionProductoDto>(calificacion);
            if (calificacion is null)
            {
                return NotFound("Calificacion no existente");
            }
            return Ok(calificacionSalida);
        }

        #endregion

        #region Create
        [HttpPost("CreateCalificacionTienda")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CalificacionTiendum>> CreateCalificacionTienda([FromBody] CalificacionTiendaDto calificacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (await tiendasService.GetOneTienda(calificacion.IdTienda) is null)
            {
                return NotFound("No se encontro una tienda registrada");
            }
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            calificacion.IdUsuario = idUser;

            var calificacionTienda = mapper.Map<CalificacionTiendum>(calificacion);
            await calificacionesService.CreateCalificacionTienda(calificacionTienda);
            return CreatedAtRoute("GetCalificacionTienda", new { id = calificacionTienda.IdCalificacionTienda }, calificacionTienda);
        }

        [HttpPost("CreateCalificacionProducto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CalificacionProducto>> CreateCalificacionProducto([FromBody] CalificacionProducto calificacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (await productosService.GetOneProducto(calificacion.IdProductos) is null)
            {
                return NotFound("No se encontro un producto registrado");
            }
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            calificacion.IdUsuario = idUser;

            var calificacionProducto = mapper.Map<CalificacionProducto>(calificacion);
            await calificacionesService.CreateCalificacionProducto(calificacionProducto);
            return CreatedAtRoute("GetCalificacionProducto", new { id = calificacionProducto.IdCalificacionProducto }, calificacionProducto);
        }
        #endregion

        #region Update
        [HttpPut("UpdateCalificacionTienda")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCalificacionTienda([FromBody] CalificacionTiendaDto calificacionDto)
        {
            if (calificacionDto is null)
            {
                return BadRequest("Calificacion no valida");
            }
            else if (await tiendasService.GetOneTienda(calificacionDto.IdTienda) is null)
            {
                return NotFound("No hay una tienda registrada con ese id");
            }
            else if(await calificacionesService.GetOneCalificacionTienda(calificacionDto.IdCalificacionTienda) is null)
            {
                return NotFound("Calificacion no registrada con ese id");
            }

            var calificacion = mapper.Map<CalificacionTiendum>(calificacionDto);

            await calificacionesService.UpdateCalificacionTienda(calificacion);
            return NoContent();
        }

        [HttpPut("UpdateCalificacionProducto")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCalificacionProducto([FromBody] CalificacionProductoDto calificacionDto)
        {
            if (calificacionDto is null)
            {
                return BadRequest("Calificacion no valida");
            }
            else if (await tiendasService.GetOneTienda(calificacionDto.IdProductos) is null)
            {
                return NotFound("No hay un producto registrado con ese id");
            }
            else if (await calificacionesService.GetOneCalificacionProducto(calificacionDto.IdCalificacionProducto) is null)
            {
                return NotFound("Calificacion no registrada con ese id");
            }

            var calificacion = mapper.Map<CalificacionProducto>(calificacionDto);

            await calificacionesService.UpdateCalificacionProducto(calificacion);
            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("DeleteCalificacionTienda")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCalificacionTienda(int id)
        {
            if (id == 0)
            {
                return BadRequest("Id invalido");
            }
            var calificacion = await calificacionesService.GetOneCalificacionTienda(id);
            if (calificacion is null)
            {
                return NotFound("Calificacion no registrada");
            }
            await calificacionesService.DeleteOneCalificacionTienda(calificacion);
            return NoContent();
        }

        [HttpDelete("DeleteCalificacionProducto")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCalificacionProducto(int id)
        {
            if (id == 0)
            {
                return BadRequest("Id invalido");
            }
            var calificacion = await calificacionesService.GetOneCalificacionProducto(id);
            if (calificacion is null)
            {
                return NotFound("Calificacion no registrada");
            }
            await calificacionesService.DeleteOneCalificacionProducto(calificacion);
            return NoContent();
        }
        #endregion
    }
}