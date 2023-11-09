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
    public class ComentariosController : ControllerBase
    {
        private readonly TiendasService tiendasService;
        private readonly ComentariosService comentariosService;
        private readonly ProductosService productosService;
        private IMapper mapper;
        public ComentariosController(ProductosService _productosService, TiendasService _tiendasService, ComentariosService _comentariosService, IMapper _mapper)
        {
            tiendasService = _tiendasService;
            comentariosService = _comentariosService;
            productosService = _productosService;
            mapper = _mapper;
        }
        #region GetAll
        [HttpGet("GetAllComentariosTienda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ComentariosTiendaDto>>> GetAllComentariosTienda(int idTienda)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            if (await tiendasService.GetOneTienda(idTienda) is null)
            {
                return BadRequest("No hay una tienda registrada con ese id");
            }
            try
            {
                var comentarios = await comentariosService.GetAllComentariosTienda(idTienda);
                if (comentarios.IsNullOrEmpty())
                {
                    return NotFound("No hay comentarios registrados en esta tienda");
                }
                return Ok(comentarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllComentariosProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ComentariosProductoDto>>> GetAllComentariosProducto(int idProducto)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            if (await productosService.GetOneProducto(idProducto) is null)
            {
                return BadRequest("No hay un producto registrado con ese id");
            }
            try
            {
                var comentarios = await comentariosService.GetAllComentariosProducto(idProducto);
                if (comentarios.IsNullOrEmpty())
                {
                    return NotFound("No hay comentarios registrados en este producto");
                }
                return Ok(comentarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion
        #region Create
        [HttpPost("CreateComentarioTienda")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ComentariosTiendaDto>> CreateComentarioTienda([FromForm] ComentariosTiendaDto comentarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var user = HttpContext.User;
                var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
                var userType = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
                if(userType != "Usuario")
                {
                    return Unauthorized("Solo se puede crear un comentario con una cuenta de usuario");
                }
                if (await tiendasService.GetOneTienda(comentarioDto.IdTienda) is null)
                {
                    return BadRequest("No hay una tienda registrada con ese id");
                }
                try
                {
                    var comentario = mapper.Map<ComentariosTienda>(comentarioDto);
                    comentario.FechaComentario = DateTime.UtcNow;
                    var comentarioCreado = mapper.Map<ComentariosTiendaDto>(await comentariosService.CreateComentarioTienda(comentario));
                    return Ok(comentarioCreado);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }

        [HttpPost("CreateComentarioProducto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ComentariosProductoDto>> CreateComentarioProducto([FromForm] ComentariosProductoDto comentarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var user = HttpContext.User;
                var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
                var userType = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
                if (userType != "Usuario")
                {
                    return Unauthorized("Solo se puede crear un comentario con una cuenta de usuario");
                }
                if (await productosService.GetOneProducto(comentarioDto.IdProducto) is null)
                {
                    return BadRequest("No hay un producto registrado con ese id");
                }
                try
                {
                    var comentario = mapper.Map<ComentariosProducto>(comentarioDto);
                    var comentarioCreado = mapper.Map<ComentariosProductoDto>(await comentariosService.CreateComentarioProducto(comentario));
                    return Ok(comentarioCreado);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }
        #endregion
    }
}
