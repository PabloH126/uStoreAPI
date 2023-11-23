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
    public class MallsController : ControllerBase
    {
        private readonly PlazasService plazasService;
        private readonly UploadService uploadService;
        private IMapper mapper;
        public MallsController(PlazasService _plazasService, UploadService _uploadService, IMapper _mapper)
        {
            plazasService = _plazasService;
            uploadService = _uploadService;
            mapper = _mapper;
        }

        [HttpGet("GetProductosMall")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductosPopularesMall(int idMall)
        {
            var productosPopulares = await plazasService.GetProductosPopulares(idMall);
            if (productosPopulares.IsNullOrEmpty())
            {
                return NotFound("No se encontraron productos populares");
            }
            return Ok(productosPopulares);
        }

        [HttpGet("GetAllMalls")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<CentroComercialDto>>> GetMalls()
        {
            var malls = await plazasService.GetMalls();
            if(malls is null)
            {
                return NotFound();
            }
            return Ok(malls);
        }
        
        [HttpGet(Name = "GetMall")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CentroComercialDto>> GetMall(int id)
        {
            var mall = await plazasService.GetOneMall(id);
            if(mall is null)
            {
                return NotFound("Plaza no encontrada");
            }
            return Ok(mall);
        }

        [HttpPost("CreateMall")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CentroComercialDto>> CreateMall([FromForm] CentroComercialDto datosMall, IFormFile iconoMall, IFormFile imagenMall)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (iconoMall == null || iconoMall.Length == 0 || imagenMall == null || imagenMall.Length == 0)
            {
                return BadRequest("La imagen es invalida");
            }
            else
            {
                var user = HttpContext.User;
                var idUser = user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);

                var iconoUrl = await uploadService.UploadImagePlazas(iconoMall, "iconos", $"{datosMall.DireccionCentroComercial}.png");
                var imagenUrl = await uploadService.UploadImagePlazas(imagenMall, "imagenes", $"{datosMall.DireccionCentroComercial}.png");

                CentroComercial mall = mapper.Map<CentroComercial>(datosMall);

                mall.IconoCentroComercial = iconoUrl;
                mall.ImagenCentroComercial = imagenUrl;

                await plazasService.CreateMall(mall);

                return CreatedAtRoute("GetMall", new { id = mall.IdCentroComercial }, mall);
            }
            
        }

        [HttpDelete("DeleteMall")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMalls(int id)
        {
            if(id == 0)
            {
                return BadRequest("Id invalido");
            }
            
            var mall = await plazasService.GetOneMall(id);
            
            if(mall is null)
            {
                return NotFound("Mall no encontrado");
            }

            await uploadService.DeleteImagesPlazas("iconos", $"{mall.DireccionCentroComercial}.png");
            await uploadService.DeleteImagesPlazas("imagenes", $"{mall.DireccionCentroComercial}.png");
            await plazasService.DeleteMall(mall);

            return NoContent();
        }
        
    }
}
