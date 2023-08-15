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
    public class TiendasController : ControllerBase
    {
        private readonly TiendasService tiendasService;
        private readonly UploadService uploadService;
        private readonly PlazasService plazasService;
        private readonly HorariosService horariosService;
        private readonly CategoriasService categoriasService;
        private readonly PeriodosPredeterminadosService periodosPredeterminadosService;
        private IMapper mapper;
        public TiendasController(PeriodosPredeterminadosService _periodosPredeterminadosService,HorariosService _horariosService,TiendasService _tiendasService, IMapper _mapper, UploadService _uploadService, PlazasService _plazasService, CategoriasService _categoriasService)
        {
            tiendasService = _tiendasService;
            mapper = _mapper;
            uploadService = _uploadService;
            plazasService = _plazasService;
            horariosService = _horariosService;
            categoriasService = _categoriasService;
            periodosPredeterminadosService = _periodosPredeterminadosService;
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
            var tienda = await tiendasService.GetOneTienda(id);
            if(tienda is null) 
            { 
                return NotFound("Tienda no encontrada");
            }
            else if(tienda.IdAdministrador != idUser)
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

        [HttpPost("CreateImagenNewTienda")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ImagenesTienda>> CreateImagenesNewTienda(int idTienda, IFormFile primera, IFormFile segunda, IFormFile tercera)
        {
            if (primera is null || primera.Length == 0 || segunda is null || segunda.Length == 0 || tercera is null || tercera.Length == 0)
            {
                return BadRequest("Imagen invalida");
            }
            else
            {
                var tienda = await tiendasService.GetOneTienda(idTienda);
                if(tienda is null)
                {
                    return NotFound("Ninguna tienda registrada con ese id");
                }

                
                await tiendasService.CreateImagenesTienda(await CreateImagenTienda(tienda.IdTienda, primera, $"{tienda.IdTienda}/{primera.Name}"));
                await tiendasService.CreateImagenesTienda(await CreateImagenTienda(tienda.IdTienda, segunda, $"{tienda.IdTienda}/{segunda.Name}"));
                await tiendasService.CreateImagenesTienda(await CreateImagenTienda(tienda.IdTienda, tercera, $"{tienda.IdTienda}/{tercera.Name}"));

                return Ok();
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
                var tienda = await tiendasService.GetOneTienda(idTienda);
                if (tienda is null)
                {
                    return NotFound("Ninguna tienda registrada con ese id");
                }


                await tiendasService.CreateImagenesTienda(
                                        await CreateImagenTienda(
                                                    tienda.IdTienda, 
                                                    imagen, 
                                                    $"{tienda.IdTienda}/{await uploadService.CountBlobs("tiendas", $"{tienda.IdTienda}")}"
                                              )
                                        );

                return Ok();
            }
        }

        [HttpPut("UpdateTienda")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTienda([FromBody] TiendaDto tiendaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var tienda = await tiendasService.GetOneTienda(tiendaDto.IdTienda);

            if (tienda is null)
            {
                return NotFound("Tienda no registrada");
            }

            if (tienda!.IdAdministrador != idUser)
            {
                return Unauthorized("Producto no autorizado");
            }

            await tiendasService.UpdateTienda(mapper.Map<Tiendum>(tiendaDto));

            return NoContent();
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

            await uploadService.DeleteImagenesTiendas($"{tienda.IdTienda}");
            await horariosService.DeleteAllHorarios(tienda.IdTienda);
            await categoriasService.DeleteAllCategoriasTienda(tienda.IdTienda);
            await periodosPredeterminadosService.DeleteAllPeriodosPredeterminados(tienda.IdTienda);
            await tiendasService.DeleteImagenesTiendaWithId(tienda.IdTienda);
            await tiendasService.DeleteTienda(tienda);

            return NoContent();
        }


        private async Task<ImagenesTienda> CreateImagenTienda(int idTienda, IFormFile imagen, string fileName)
        {
            var imagenUrl = await uploadService.UploadImageTiendas(imagen, $"{fileName}");
            return new ImagenesTienda
            {
                IdTienda = idTienda,
                ImagenTienda = imagenUrl
            };
        }
    }
}
