using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicacionesController : ControllerBase
    {
        private readonly PublicacionesService publicacionesService;
        private readonly TiendasService tiendasService;
        private readonly PlazasService plazasService;
        private readonly UploadService uploadService;
        private readonly UserService userService;
        private IMapper mapper;
        public PublicacionesController(UserService _userService, UploadService _uploadService, PlazasService _plazasService, PublicacionesService _publicacionesService, TiendasService _tiendasService, IMapper _mapper)
        {
            publicacionesService = _publicacionesService;
            tiendasService = _tiendasService;
            mapper = _mapper;
            plazasService = _plazasService;
            uploadService = _uploadService;
            userService = _userService;
        }

        [HttpGet("GetPublicacionesRecientesApp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PublicacionesDto>>> GetPublicacionesRecientesApp(int idMall)
        {
            var publicaciones = await publicacionesService.GetPublicacionesRecientesApp(idMall);

            if (publicaciones.IsNullOrEmpty())
            {
                return NotFound("No hay ninguna publicacion para esta plaza");
            }
            var publicacionesDto = mapper.Map<IEnumerable<PublicacionesDto>>(publicaciones);
            foreach (var publicacion in publicacionesDto)
            {
                var tienda = await tiendasService.GetOneTienda(publicacion.IdTienda);
                publicacion.NombreTienda = tienda!.NombreTienda;
                publicacion.LogoTienda = tienda!.LogoTienda;
            }
            return Ok(publicacionesDto);
        }

        [HttpGet("GetPublicacionesRecientes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PublicacionesDto>>> GetPublicacionesRecientes(int idTienda)
        {
            if (await tiendasService.GetOneTienda(idTienda) is null)
            {
                return BadRequest("No hay ninguna tienda registrada con ese id");
            }

            var publicaciones = await publicacionesService.GetPublicacionesRecientes(idTienda);
            
            if (publicaciones.IsNullOrEmpty())
            {
                return NotFound("No hay ninguna publicacion para esta plaza");
            }
            var publicacionesDto = mapper.Map<IEnumerable<PublicacionesDto>>(publicaciones);
            foreach (var publicacion in publicacionesDto)
            {
                var tienda = await tiendasService.GetOneTienda(publicacion.IdTienda);
                publicacion.NombreTienda = tienda!.NombreTienda;
                publicacion.LogoTienda = tienda!.LogoTienda;
            }
            return Ok(publicacionesDto);
        }

        [HttpGet(Name = "GetPublicacion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PublicacionesDto>> GetPublicacion(int id)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var publicacion = await publicacionesService.GetPublicacion(id);

            if (publicacion is null)
            {
                return NotFound("Publicacion no encontrada");
            }

            return Ok(mapper.Map<PublicacionesDto>(publicacion));
        }

        [Authorize]
        [HttpPost("CreatePublicacion")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PublicacionesDto>> CreatePublicacion([FromForm] PublicacionesDto publicacion, IFormFile? imagen)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            if(typeUser == "Usuario")
            {
                return Unauthorized("Solo los administradores o gerentes pueden hacer publicaciones");
            }
            var tienda = await tiendasService.GetOneTienda(publicacion.IdTienda);
            if (tienda is null)
            {
                return BadRequest("No se encontró la tienda");
            }
            else if (publicacion.IdCentroComercial is null)
            {
                publicacion.IdCentroComercial = tienda!.IdCentroComercial;
            }

            var plaza = await plazasService.GetOneMall(publicacion.IdCentroComercial);

            if(plaza is null)
            {
                return BadRequest("No se encontró el centro comercial");
            }

            publicacion.FechaPublicacion = DateTime.Now.Date;
            
            var publicacionGuardada = mapper.Map<Publicacione>(publicacion);
            try
            {
                var publicacionCreada = await publicacionesService.CreatePublicacion(publicacionGuardada);
                if (imagen is not null && imagen!.Length != 0)
                {
                    var urlImagen = await uploadService.UploadImagePublicacion(imagen, publicacionCreada.IdPublicacion.ToString());
                    publicacionCreada.Imagen = urlImagen;
                    await publicacionesService.UpdatePublicacion(publicacionCreada);
                }
                var publicacionCreadaDto = mapper.Map<PublicacionesDto>(publicacionCreada);
                var idsUsuario = await userService.GetUsuariosFavoritosTienda((int)publicacionCreada.IdTienda!);
                if (idsUsuario.Any())
                {
                    foreach (var id in idsUsuario)
                    {
                        var idUsuarioEnqueue = id;
                        var publicacionEnqueue = publicacionCreadaDto;

                        BackgroundJob.Enqueue(() => userService.NotificarUsuarioPromocionesFavoritasSincrono(idUsuarioEnqueue, publicacionEnqueue));
                    }
                }
                return CreatedAtRoute("GetPublicacion", new { id = publicacionCreada.IdPublicacion }, publicacionCreadaDto);
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPut("UpdatePublicacion")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePublicacion([FromForm] PublicacionUpdateDto publicacionUpdated, IFormFile imagen)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var publicacion = await publicacionesService.GetPublicacion(publicacionUpdated.IdPublicacion);

            if (publicacion is null)
            {
                return BadRequest("No se encontró la publicacion");
            }
            
            publicacion.Contenido = publicacionUpdated.Contenido;

            if (imagen is not null || imagen!.Length != 0)
            {
                var urlImagen = await uploadService.UploadImagePublicacion(imagen, publicacion.IdPublicacion.ToString());
                publicacion.Imagen = urlImagen;
                await publicacionesService.UpdatePublicacion(publicacion);
            }

            return NoContent();
        }

        [HttpDelete("DeletePublicacion")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePublicacion(int idPublicacion)
        {
            var publicacion = await publicacionesService.GetPublicacion(idPublicacion);

            if (publicacion is null)
            {
                return BadRequest("No se encontró la publicacion");
            }
            await uploadService.DeleteImagePublicacion(publicacion.IdPublicacion.ToString());
            await publicacionesService.DeletePublicacion(publicacion);
            return NoContent();
        }
    }
}
