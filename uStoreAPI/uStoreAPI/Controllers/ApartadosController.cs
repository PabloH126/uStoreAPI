using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using uStoreAPI.Hubs;

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
        private readonly UserService userService;
        private readonly NotificacionesApartadoService notificacionesApartadoService;
        private readonly IHubContext<ApartadosHub> hubContext;
        private IMapper mapper;
        public ApartadosController(IHubContext<ApartadosHub> _hubContext, NotificacionesApartadoService _notificacionesApartadoService, UserService _userService, TiendasService _tiendasService, ProductosService _productosService, SolicitudesApartadoService _solicitudesApartadoService, IMapper _mapper, PeriodosPredeterminadosService _periodosService)
        {
            tiendasService = _tiendasService;
            productosService = _productosService;
            solicitudesApartadoService = _solicitudesApartadoService;
            mapper = _mapper;
            periodosService = _periodosService;
            userService = _userService;
            notificacionesApartadoService = _notificacionesApartadoService;
            hubContext = _hubContext;
        }

        #region GetSolicitudes
        [HttpGet("GetSolicitudesPendientes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SolicitudesApartadoDto>> GetSolicitudesApartadoPendientes(int idTienda)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var userType = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            string? idTiendaGerente = null;
            if(userType == "Gerente")
            {
                idTiendaGerente = user.Claims.FirstOrDefault(u => u.Type == "IdTienda")!.Value;
            }

            var tienda = await tiendasService.GetOneTienda(idTienda);
            var periodos = await periodosService.GetPeriodosPredeterminados(idTienda);
            if(tienda is null)
            {
                return BadRequest("Tienda no registrada");
            }
            else if((tienda.IdAdministrador != idUser) && !(userType == "Gerente" && int.Parse(idTiendaGerente!) == tienda.IdTienda))
            {
                return Unauthorized("Tienda no autorizada");
            }

            var solicitudesApartado = mapper.Map<IEnumerable<SolicitudesApartadoDto>>(await solicitudesApartadoService.GetSolicitudesApartadoPendientesWithIdTienda(idTienda));
            
            if (solicitudesApartado.IsNullOrEmpty())
            {
                return NotFound("No hay solicitudes pendientes en esta tienda");
            }
            
            foreach (var solicitud in solicitudesApartado)
            {
                var usuario = await userService.GetUsuario(solicitud.IdUsuario);
                var detallesUser = await userService.GetDetallesUsuario(usuario!.IdDetallesUsuario);
                solicitud.RatioUsuario = $"{detallesUser!.ApartadosExitosos}/{detallesUser.ApartadosFallidos + detallesUser.ApartadosExitosos}";
                var producto = mapper.Map<ProductoDto>(await productosService.GetOneProducto(solicitud.IdProductos));
                if (producto is not null)
                {
                    var imagenProducto = await productosService.GetPrincipalImageProducto(producto.IdProductos);
                    if (imagenProducto is not null)
                    {
                        solicitud.ImageProducto = imagenProducto.ImagenProducto;
                    }
                    solicitud.NombreProducto = producto.NombreProducto;
                    solicitud.PrecioProducto = producto.PrecioProducto;
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
            notificacionesApartadoService.CancelarSend();
            return Ok(solicitudesApartado);
        }

        [HttpGet("GetSolicitudesActivas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SolicitudesApartadoDto>> GetSolicitudesApartadoActivas(int idTienda)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var userType = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            string? idTiendaGerente = null;
            if (userType == "Gerente")
            {
                idTiendaGerente = user.Claims.FirstOrDefault(u => u.Type == "IdTienda")!.Value;
            }

            var tienda = await tiendasService.GetOneTienda(idTienda);
            var periodos = await periodosService.GetPeriodosPredeterminados(idTienda);
            if (tienda is null)
            {
                return BadRequest("Tienda no registrada");
            }
            else if ((tienda.IdAdministrador != idUser) && !(userType == "Gerente" && int.Parse(idTiendaGerente!) == tienda.IdTienda))
            {
                return Unauthorized("Tienda no autorizada");
            }

            var solicitudesApartado = mapper.Map<IEnumerable<SolicitudesApartadoDto>>(await solicitudesApartadoService.GetSolicitudesApartadoActivasWithIdTienda(idTienda));

            if (solicitudesApartado.IsNullOrEmpty())
            {
                return NotFound("No hay solicitudes activas en esta tienda");
            }

            foreach (var solicitud in solicitudesApartado)
            {
                var producto = mapper.Map<ProductoDto>(await productosService.GetOneProducto(solicitud.IdProductos));
                if (producto is not null)
                {
                    var imagenProducto = await productosService.GetPrincipalImageProducto(producto.IdProductos);
                    if (imagenProducto is not null)
                    {
                        solicitud.ImageProducto = imagenProducto.ImagenProducto;
                    }
                    solicitud.NombreProducto = producto.NombreProducto;
                    solicitud.PrecioProducto = producto.PrecioProducto;
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

            return Ok(solicitudesApartado);
        }

        [HttpGet("GetNumeroSolicitudes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Dictionary<int, int>>> GetNumeroSolicitudes()
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            
            var solicitudesApartado = await solicitudesApartadoService.GetSolicitudesApartadoTiendas(idUser);
            return Ok(solicitudesApartado);
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
            var productoSolicitado = await productosService.GetOneProducto(solicitud.IdProductos);
            if (productoSolicitado is null) 
            {
                return NotFound("No se ha encontrado un producto registrado");
            }
            var tienda = await tiendasService.GetOneTienda(solicitud.IdTienda);
            if (tienda is null)
            {
                return NotFound("Tienda solicitada no registrada");
            }
            else if (!(await productosService.VerificarProductoTienda(productoSolicitado.IdProductos, tienda.IdTienda)))
            {
                return Unauthorized("Producto no perteneciente a esa tienda");
            }

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            if (productoSolicitado.CantidadApartado < solicitud.UnidadesProducto)
            {
                return BadRequest("La cantidad de unidades del producto es mayor a las disponibles actualmente");
            }
            else
            {
                productoSolicitado.CantidadApartado -= solicitud.UnidadesProducto;
                await productosService.UpdateProducto(productoSolicitado);
            }

            var solicitudApartado = mapper.Map<SolicitudesApartado>(solicitud);
            solicitudApartado.StatusSolicitud = "pendiente";
            solicitudApartado.FechaSolicitud = DateTime.UtcNow;
            await solicitudesApartadoService.CreateSolicitud(solicitudApartado);

            var solicitudApartadoDto = mapper.Map<SolicitudesApartadoDto>(solicitudApartado);

            var producto = mapper.Map<ProductoDto>(productoSolicitado);

            var periodos = await periodosService.GetPeriodosPredeterminados(solicitudApartado.IdTienda);

            var usuario = await userService.GetUsuario(solicitud.IdUsuario);
            var detallesUser = await userService.GetDetallesUsuario(usuario!.IdDetallesUsuario);

            solicitudApartadoDto.RatioUsuario = $"{detallesUser!.ApartadosExitosos}/{detallesUser.ApartadosFallidos + detallesUser.ApartadosExitosos}";

            if (producto is not null)
            {
                var imagenProducto = await productosService.GetPrincipalImageProducto(producto.IdProductos);
                if (imagenProducto is not null)
                {
                    solicitudApartadoDto.ImageProducto = imagenProducto.ImagenProducto;
                }
                solicitudApartadoDto.NombreProducto = producto.NombreProducto;
                solicitudApartadoDto.PrecioProducto = producto.PrecioProducto;
            }
            foreach (var periodo in periodos)
            {
                if (solicitudApartadoDto.PeriodoApartado == periodo.ApartadoPredeterminado)
                {
                    solicitudApartadoDto.personalizado = false;
                }
                else
                {
                    solicitudApartadoDto.personalizado = true;
                }
            }

            notificacionesApartadoService.CreateSolicitud(solicitudApartadoDto);

            var idAdmin = tienda.IdAdministrador;
            var solicitudesCount = await solicitudesApartadoService.GetSolicitudesApartadoTiendas((int)idAdmin!);
            await hubContext.Clients.Group(idAdmin.ToString()!).SendAsync("RecieveUpdateNotificaciones", solicitudesCount);

            return Ok(solicitudApartadoDto);
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

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var solicitudApartado = await solicitudesApartadoService.GetOneSolicitudApartado(solicitud.IdSolicitud);
            
            if(solicitudApartado is null)
            {
                return NotFound("No se ha encontrado una solicitud registrada");
            };

            if (solicitudApartado.StatusSolicitud == "completada")
            {
                return BadRequest("La solicitud ya esta completada");
            }

            var productoSolicitado = await productosService.GetOneProducto(solicitudApartado.IdProductos);
            
            if(solicitud.StatusSolicitud == "activa")
            {
                solicitudApartado.StatusSolicitud = "activa";
                solicitudApartado.FechaApartado = DateTime.UtcNow;

                var expiracionApartado = solicitudApartado.PeriodoApartado!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (!int.TryParse(expiracionApartado[0], out int cantidadTiempo))
                {
                    throw new ArgumentException("La cantidad de tiempo en PeriodoApartado no es un número");
                }

                DateTime fechaVencimiento;

                switch(expiracionApartado[1])
                {
                    case "minutos": 
                        fechaVencimiento = DateTime.UtcNow.AddMinutes(cantidadTiempo);
                        break;
                    case "horas":
                        fechaVencimiento = DateTime.UtcNow.AddHours(cantidadTiempo);
                        break;
                    case "dias":
                        fechaVencimiento = DateTime.UtcNow.AddDays(cantidadTiempo);
                        break;
                    default:
                        throw new ArgumentException("Unidad de tiempo desconocida en PeriodoApartado");
                }

                solicitudApartado.FechaVencimiento = fechaVencimiento;

                var jobId = BackgroundJob.Schedule(() => solicitudesApartadoService.MarcarComoVencida(solicitudApartado.IdSolicitud), fechaVencimiento);

                solicitudApartado.IdJob = jobId;
                
            }
            else if (solicitud.StatusSolicitud == "completada")
            {
                solicitudApartado.StatusSolicitud = "completada";
                var usuario = await userService.GetUsuario(solicitudApartado.IdUsuario);
                if(usuario is not null)
                {
                    var detallesUsuario = await userService.GetDetallesUsuario(usuario.IdDetallesUsuario);
                    if(detallesUsuario is not null)
                    {
                        detallesUsuario.ApartadosExitosos += 1;
                        await userService.PatchUserApartados(detallesUsuario);
                    }
                }
                if (!string.IsNullOrEmpty(solicitudApartado.IdJob))
                {
                    BackgroundJob.Delete(solicitudApartado.IdJob);
                    solicitudApartado.IdJob = null;
                }
            }
            else if(solicitud.StatusSolicitud == "rechazada")
            {
                solicitudApartado.StatusSolicitud = "rechazada";

                if (!string.IsNullOrEmpty(solicitudApartado.IdJob))
                {
                    BackgroundJob.Delete(solicitudApartado.IdJob);
                    solicitudApartado.IdJob = null;
                }

                productoSolicitado!.CantidadApartado += solicitudApartado.UnidadesProducto;
                await productosService.UpdateProducto(productoSolicitado);
            }
            else if (solicitud.StatusSolicitud == "cancelada")
            {
                solicitudApartado.StatusSolicitud = "cancelada";

                if (!string.IsNullOrEmpty(solicitudApartado.IdJob))
                {
                    BackgroundJob.Delete(solicitudApartado.IdJob);
                    solicitudApartado.IdJob = null;
                }

                productoSolicitado!.CantidadApartado += solicitudApartado.UnidadesProducto;
                await productosService.UpdateProducto(productoSolicitado);
            }

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
