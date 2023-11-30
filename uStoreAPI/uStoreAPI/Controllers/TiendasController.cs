using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Versioning;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TiendasController : ControllerBase
    {
        private readonly TiendasService tiendasService;
        private readonly UploadService uploadService;
        private readonly PlazasService plazasService;
        private readonly HorariosService horariosService;
        private readonly CategoriasService categoriasService;
        private readonly PeriodosPredeterminadosService periodosPredeterminadosService;
        private readonly CalificacionesService calificacionesService;
        private readonly ProductosService productosService;
        private readonly PublicacionesService publicacionesService;
        private readonly SolicitudesApartadoService solicitudesService;
        private readonly GerentesService gerentesService;
        private readonly ChatService chatService;
        private readonly ComentariosService comentariosService;
        private readonly UserService userService;
        private IMapper mapper;
        public TiendasController(UserService _userService, ComentariosService _comentariosService, ChatService _chatService, GerentesService _gerentesService, SolicitudesApartadoService _solicitudesService, PublicacionesService _publicacionesService, CalificacionesService _calificacionesService, PeriodosPredeterminadosService _periodosPredeterminadosService,HorariosService _horariosService,TiendasService _tiendasService, IMapper _mapper, UploadService _uploadService, PlazasService _plazasService, CategoriasService _categoriasService, ProductosService _productosService)
        {
            tiendasService = _tiendasService;
            mapper = _mapper;
            uploadService = _uploadService;
            plazasService = _plazasService;
            horariosService = _horariosService;
            categoriasService = _categoriasService;
            periodosPredeterminadosService = _periodosPredeterminadosService;
            calificacionesService = _calificacionesService;
            productosService = _productosService;
            publicacionesService = _publicacionesService;
            solicitudesService = _solicitudesService;
            gerentesService = _gerentesService;
            chatService = _chatService;
            comentariosService = _comentariosService;
            userService = _userService;
        }

        [HttpGet("GetAllTiendas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ListaTiendasAppDto>>> GetAllTiendas(int idCentroComercial)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            if (await plazasService.GetOneMall(idCentroComercial) is null)
            {
                return BadRequest("No hay una plaza registrada con ese id");
            }
            try
            {
                var tiendas = await tiendasService.GetAllTiendas(idCentroComercial);
                if (tiendas.IsNullOrEmpty())
                {
                    return NotFound("No hay tiendas registradas en esta plaza");
                }
                var tiendasFavoritas = await userService.GetFavoritosTiendaUsuario(tiendas.Select(p => p.IdTienda), idUser);
                if (!tiendasFavoritas.IsNullOrEmpty())
                {
                    foreach (var tienda in tiendas)
                    {
                        if (tiendasFavoritas.Contains(tienda.IdTienda))
                        {
                            tienda.IsFavorito = "corazon_lleno.png";
                        }
                        else
                        {
                            tienda.IsFavorito = "corazon_vacio.png";
                        }
                    }
                }
                else
                {
                    foreach (var tienda in tiendas)
                    {
                        tienda.IsFavorito = "corazon_vacio.png";
                    }
                }
                return Ok(tiendas);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetDatosTiendaApp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TiendaDto>> GetDatosTiendaApp(int idTienda)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            try
            {
                var tienda = await tiendasService.GetDatosTienda(idTienda, idUser);
                if (tienda is null)
                {
                    return NotFound("No se encontró la tienda solicitada");
                }
                return Ok(tienda);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetTiendaApp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TiendaAppDto>> GetTiendaApp(int idTienda)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            try {
                var tienda = await tiendasService.GetTiendaApp(idTienda, idUser);
                if (tienda is null)
                {
                    return NotFound("Tienda no encontrada");
                }
                else if (typeUser != "Usuario")
                {
                    return Unauthorized("Tienda no autorizada");
                }
                tienda.ComentariosTienda = await comentariosService.GetAllComentariosTienda(tienda.IdTienda);
                tienda.IsFavorito = (await userService.VerifyFavoritoTienda(idUser, tienda.IdTienda)) ? "corazon_lleno.png" : "corazon_vacio.png";
                
                return Ok(tienda);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetTiendas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TiendaDto>>> GetTiendas(int idCentroComercial)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            if (await plazasService.GetOneMall(idCentroComercial) is null)
            {
                return BadRequest("No hay una plaza registrada con ese id");
            }
            var tiendas = await tiendasService.GetTiendas(idCentroComercial, idUser);
            if(tiendas.IsNullOrEmpty())
            {
                return NotFound("No hay tiendas registradas en esta plaza");
            }
            return Ok(tiendas);
        }

        [HttpGet("GetImagenesTienda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TiendaDto>>> GetImagenesTienda(int idTienda)
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
            else if(tienda.IdAdministrador != idUser && idTiendaClaimValue != tienda.IdTienda)
            {
                return Unauthorized("Tienda no autorizada");
            }

            var imagenesTienda = await tiendasService.GetImagenesTienda(idTienda);

            if (imagenesTienda.IsNullOrEmpty())
            {
                return NotFound("No hay imagenes registradas en esta tienda");
            }
            return Ok(imagenesTienda);
        }

        [HttpGet(Name = "GetTienda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TiendaDto>> GetTienda(int id)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            string? idTiendaClaim = user.Claims.FirstOrDefault(u => u.Type == "IdTienda")?.Value;
            int idTiendaClaimValue = 0;
            int.TryParse(idTiendaClaim, out idTiendaClaimValue);
            var tienda = await tiendasService.GetOneTienda(id);
            if(tienda is null) 
            { 
                return NotFound("Tienda no encontrada");
            }
            else if(typeUser != "Usuario" && tienda.IdAdministrador != idUser && idTiendaClaimValue != tienda.IdTienda)
            {
                return Unauthorized("Tienda no autorizada");
            }
            return Ok(mapper.Map<TiendaDto>(tienda));
        }

        [HttpPost("CreateTienda")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TiendaDto>> CreateTienda([FromForm] TiendaDto tiendaDto, IFormFile logoTienda)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (logoTienda is null || logoTienda.Length == 0)
            {
                return BadRequest("Imagen invalida");
            }
            else
            {
                var user = HttpContext.User;
                var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

                if(await plazasService.GetOneMall(tiendaDto.IdCentroComercial) is null)
                {
                    return BadRequest("No hay una plaza registrada con ese id");
                }

                tiendaDto.IdAdministrador = idUser;
                var tienda = mapper.Map<Tiendum>(tiendaDto);

                await tiendasService.CreateTienda(tienda);
                var logoUrl = await uploadService.UploadImageTiendas(logoTienda, $"{tienda.IdTienda}/{tienda.IdTienda}");
                
                tienda.LogoTienda = logoUrl;

                await tiendasService.UpdateTienda(tienda);

                var tiendaDtoSalida = mapper.Map<TiendaDto>(tienda);
                return CreatedAtRoute("GetTienda", new { id = tiendaDtoSalida.IdTienda }, tiendaDtoSalida);
            }
        }

        [HttpPost("CreateImagenTienda")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ImagenesTienda>> CreateImagenesTienda(int idTienda, IFormFile imagen)
        {
            if (imagen is null || imagen.Length == 0)
            {
                return BadRequest("Imagen invalida");
            }
            else
            {
                var user = HttpContext.User;
                var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

                var tienda = await tiendasService.GetOneTienda(idTienda);

                if (tienda is null)
                {
                    return NotFound("Ninguna tienda registrada con ese id");
                }
                else if (tienda.IdAdministrador != idUser)
                {
                    return Unauthorized("Tienda no autorizada");
                }

                var imagenesTotal = await tiendasService.GetImagenesTienda(idTienda);
                var imagenesCounter = imagenesTotal.Count() + 1;

                await tiendasService.CreateImagenesTienda(
                                        await CreateImagenTienda(
                                                    tienda.IdTienda, 
                                                    imagen, 
                                                    $"{tienda.IdTienda}/{imagenesCounter}"
                                              )
                                        );

                return Ok();
            }
        }

        [HttpPut("UpdateImagenTienda")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateImagenTienda(int idTienda, int idImagenTienda, IFormFile? imagen)
        {
            if (imagen is null || imagen.Length == 0)
            {
                return BadRequest("Imagen invalida");
            }
            else
            {
                var user = HttpContext.User;
                var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

                var tienda = await tiendasService.GetOneTienda(idTienda);

                if (tienda is null)
                {
                    return NotFound("Ninguna tienda registrada con ese id");
                }
                else if (tienda.IdAdministrador != idUser)
                {
                    return Unauthorized("Tienda no autorizada");
                }
                else if(idImagenTienda == 0)
                {
                    var imagenesTotal = await tiendasService.GetImagenesTienda(idTienda);
                    var imagenesCounter = imagenesTotal.Count() + 1;

                    await tiendasService.CreateImagenesTienda(
                                            await CreateImagenTienda(
                                                        tienda.IdTienda,
                                                        imagen,
                                                        $"{tienda.IdTienda}/{imagenesCounter}"
                                                  )
                                            );
                    return NoContent();
                }

                var imagenTienda = await tiendasService.GetImagenTienda(idImagenTienda);
                var newImagenTienda = await CreateImagenTienda(tienda.IdTienda, imagen, $"{tienda.IdTienda}/{uploadService.GetBlobNameFromUrl(imagenTienda!.ImagenTienda)}");
                imagenTienda.ImagenTienda = newImagenTienda.ImagenTienda;

                await tiendasService.UpdateImagenTienda(imagenTienda);
                                        

                return NoContent();
            }
        }

        [HttpPut("UpdateTienda")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTienda([FromForm] TiendaUpdateDto tiendaDto, IFormFile? logoTienda)
        {
            try
            {
                var user = HttpContext.User;
                var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

                var tienda = await tiendasService.GetOneTienda(tiendaDto.IdTienda);

                if (tienda is null)
                {
                    return NotFound("Tienda no registrada");
                }

                if (tienda!.IdAdministrador != idUser)
                {
                    return Unauthorized("Tienda no autorizada");
                }

                if (logoTienda is not null)
                {
                    var logoUrl = await uploadService.UploadImageTiendas(logoTienda, $"{tienda.IdTienda}/{tienda.IdTienda}");
                    tienda.LogoTienda = logoUrl;
                }

                tienda.NombreTienda = tiendaDto.NombreTienda;
                await tiendasService.UpdateTienda(tienda);

                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DeleteTienda")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTienda(int id)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            if (id == 0)
            {
                return BadRequest("Id invalido");
            }

            var tienda = await tiendasService.GetOneTienda(id);

            if (tienda is null)
            {
                return NotFound("tienda no encontrada");
            }
            else if(!(tienda.IdAdministrador == idUser))
            {
                return Unauthorized("Tienda no autorizada");
            }

            var productosTienda = await productosService.GetProductos(tienda.IdTienda);

            await solicitudesService.DeleteSolicitudesTienda(tienda.IdTienda);

            if (!productosTienda.IsNullOrEmpty())
            {
                foreach (var producto in productosTienda)
                {
                    await uploadService.DeleteImagenesProductos($"{producto.IdProductos}");
                    await productosService.DeleteImagenesProductoWithId(producto.IdProductos);
                    await calificacionesService.DeleteAllCalificacionesProducto(producto.IdProductos);
                    await comentariosService.DeleteAllComentariosProducto(producto.IdProductos);
                    await categoriasService.DeleteAllCategoriasProducto(producto.IdProductos);
                    await productosService.DeleteProducto(producto);  
                }
            }
            await comentariosService.DeleteAllComentariosTienda(tienda.IdTienda);
            await chatService.DeleteChatWithIdTienda(tienda.IdTienda);
            await gerentesService.DeleteAccountGerente(null, tienda.IdTienda);
            await publicacionesService.DeleteAllPublicaciones(tienda.IdTienda);
            await uploadService.DeleteImagenesTiendas($"{tienda.IdTienda}");
            await horariosService.DeleteAllHorarios(tienda.IdTienda);
            await categoriasService.DeleteAllCategoriasTienda(tienda.IdTienda);
            await periodosPredeterminadosService.DeleteAllPeriodosPredeterminados(tienda.IdTienda);
            await tiendasService.DeleteImagenesTiendaWithId(tienda.IdTienda);
            await calificacionesService.DeleteAllCalificacionesTienda(tienda.IdTienda);
            await tiendasService.DeleteTienda(tienda);

            return NoContent();
        }

        [HttpDelete("DeleteImagenTienda")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteImagenTienda(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var imagenTienda = await tiendasService.GetImagenTienda(id);
            if(imagenTienda is null)
            {
                return NotFound();
            }
            await uploadService.DeleteImageTiendas(imagenTienda.IdTienda.ToString()!, uploadService.GetBlobNameFromUrl(imagenTienda.ImagenTienda));
            await tiendasService.DeleteImagenTienda(imagenTienda);
            return NoContent();
        }

        private async Task<ImagenesTienda> CreateImagenTienda(int idTienda, IFormFile imagen, string fileName)
        {
            var imagenUrl = await uploadService.UploadImageTiendas(imagen, fileName);
            return new ImagenesTienda
            {
                IdTienda = idTienda,
                ImagenTienda = imagenUrl
            };
        }
    }
}
