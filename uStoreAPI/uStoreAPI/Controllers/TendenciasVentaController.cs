using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TendenciasVentaController : ControllerBase
    {
        private readonly ProductosService productosService;
        private readonly TiendasService tiendasService;
        private readonly TendenciasService tendenciasService;
        private readonly SolicitudesApartadoService solicitudesApartadoService;
        private IMapper mapper;

        public TendenciasVentaController(TendenciasService _tendenciasService, ProductosService _productosService, TiendasService _tiendasService, SolicitudesApartadoService _solicitudesApartadoService, IMapper _mapper)
        {
            productosService = _productosService;
            tiendasService = _tiendasService;
            solicitudesApartadoService = _solicitudesApartadoService;
            tendenciasService = _tendenciasService;
            mapper = _mapper;
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
        
    }
}
