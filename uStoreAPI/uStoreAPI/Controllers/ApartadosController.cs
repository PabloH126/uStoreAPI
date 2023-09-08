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
    public class ApartadosController : ControllerBase
    {
        private readonly TiendasService tiendasService;
        private readonly ProductosService productosService;
        private readonly PeriodosPredeterminadosService periodosService;
        private readonly SolicitudesApartadoService solicitudesApartadoService;
        private IMapper mapper;
        public ApartadosController(TiendasService _tiendasService, ProductosService _productosService, SolicitudesApartadoService _solicitudesApartadoService, IMapper _mapper, PeriodosPredeterminadosService _periodosService)
        {
            tiendasService = _tiendasService;
            productosService = _productosService;
            solicitudesApartadoService = _solicitudesApartadoService;
            mapper = _mapper;
            periodosService = _periodosService;

        }

        #region GetSolicitudes
        [HttpGet("GetSolicitudesPendientes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> GetSolicitudesApartadoPendientes(int idTienda)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var tienda = await tiendasService.GetOneTienda(idTienda);
            var periodos = await periodosService.GetPeriodosPredeterminados(idTienda);
            if(tienda is null)
            {
                return BadRequest("Tienda no registrada");
            }
            else if(tienda.IdAdministrador != idUser)
            {
                return Unauthorized("Tienda no autorizada");
            }

            var SolicitudesApartado = mapper.Map<IEnumerable<SolicitudesApartadoDto>>(await solicitudesApartadoService.GetSolicitudesApartadoPendientesWithIdTienda(idTienda));
            
            if (SolicitudesApartado.IsNullOrEmpty())
            {
                return NotFound("No hay solicitudes pendientes en esta tienda");
            }
            
            var productosApartados = new List<ProductoDto>();
            
            foreach (var solicitud in SolicitudesApartado)
            {
                var producto = mapper.Map<ProductoDto>(await productosService.GetOneProducto(solicitud.IdProductos));
                if (producto is not null)
                {
                    var imagenProducto = await productosService.GetPrincipalImageProducto(producto.IdProductos);
                    if (imagenProducto is not null)
                    {
                        producto.ImageProducto = imagenProducto.ImagenProducto;
                    }
                    productosApartados.Add(producto);
                }
                else
                {
                    return BadRequest("Hubo un error en la obtencion del producto");
                }

                foreach (var periodo in periodos)
                {
                    if(solicitud.PeriodoApartado == periodo.ApartadoPredeterminado)
                    {
                        solicitud.personalizado = false;
                    }
                    else
                    {
                        solicitud.personalizado = true;
                    }
                }
            }

            var result = new
            {
                solicitudes = SolicitudesApartado,
                productos = productosApartados,
            };

            return Ok(result);
        }

        [HttpGet("GetSolicitudesActivas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> GetSolicitudesApartadoActivas(int idTienda)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var tienda = await tiendasService.GetOneTienda(idTienda);
            var periodos = await periodosService.GetPeriodosPredeterminados(idTienda);
            if (tienda is null)
            {
                return BadRequest("Tienda no registrada");
            }
            else if (tienda.IdAdministrador != idUser)
            {
                return Unauthorized("Tienda no autorizada");
            }

            var SolicitudesApartado = mapper.Map<IEnumerable<SolicitudesApartadoDto>>(await solicitudesApartadoService.GetSolicitudesApartadoActivasWithIdTienda(idTienda));

            if (SolicitudesApartado.IsNullOrEmpty())
            {
                return NotFound("No hay solicitudes pendientes en esta tienda");
            }

            var productosApartados = new List<ProductoDto>();

            foreach (var solicitud in SolicitudesApartado)
            {
                var producto = mapper.Map<ProductoDto>(await productosService.GetOneProducto(solicitud.IdProductos));
                if (producto is not null)
                {
                    var imagenProducto = await productosService.GetPrincipalImageProducto(producto.IdProductos);
                    if (imagenProducto is not null)
                    {
                        producto.ImageProducto = imagenProducto.ImagenProducto;
                    }
                    productosApartados.Add(producto);
                }
                else
                {
                    return BadRequest("Hubo un error en la obtencion del producto");
                }

                foreach (var periodo in periodos)
                {
                    if (solicitud.PeriodoApartado == periodo.ApartadoPredeterminado)
                    {
                        solicitud.personalizado = false;
                    }
                    else
                    {
                        solicitud.personalizado = true;
                    }
                }
            }

            var result = new
            {
                solicitudes = SolicitudesApartado,
                productos = productosApartados,
            };

            return Ok(result);
        }

        [HttpGet(Name = "GetSolicitud")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SolicitudesApartadoDto>> GetSolicitudApartado(int id)
        {
            var solicitud = await solicitudesApartadoService.GetOneSolicitudApartado(id);
            return Ok(mapper.Map<SolicitudesApartadoDto>(solicitud));
        }
        #endregion

        [HttpPost("CreateSolicitud")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SolicitudesApartadoDto>> CreateSolicitud([FromBody] SolicitudesApartadoDto solicitud)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if(await productosService.GetOneProducto(solicitud.IdProductos) is null) 
            {
                return NotFound("No se ha encontrado un producto registrado");
            }
            
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var solicitudApartado = mapper.Map<SolicitudesApartado>(solicitud);
            await solicitudesApartadoService.CreateSolicitud(solicitudApartado);

            return Ok(mapper.Map<SolicitudesApartadoDto>(solicitudApartado));
        }

        [HttpPut("UpdateSolicitud")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSolicitud([FromBody] SolicitudesApartadoUpdateDto solicitud)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (await productosService.GetOneProducto(solicitud.IdProductos) is null)
            {
                return NotFound("No se ha encontrado un producto registrado");
            }

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var solicitudApartado = await solicitudesApartadoService.GetOneSolicitudApartado(solicitud.IdSolicitud);
            
            if(solicitudApartado is null)
            {
                return NotFound("No se ha encontrado una solicitud registrada");
            }

            solicitudApartado.PeriodoApartado = solicitud.PeriodoApartado;
            solicitudApartado.UnidadesProducto = solicitud.UnidadesProducto;
            solicitudApartado.StatusSolicitud = solicitud.StatusSolicitud;

            await solicitudesApartadoService.UpdateSolicitud(solicitudApartado);

            return NoContent();
        }

        [HttpDelete("DeleteSolicitud")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSolicitud(int id)
        {
            if (id == 0)
            {
                return BadRequest("Id invalido");
            }

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var solicitud = await solicitudesApartadoService.GetOneSolicitudApartado(id);

            if (solicitud is null)
            {
                return NotFound("Solicitud no encontrada");
            }

            await solicitudesApartadoService.DeleteSolicitud(solicitud);

            return NoContent();
        }
    }
}
