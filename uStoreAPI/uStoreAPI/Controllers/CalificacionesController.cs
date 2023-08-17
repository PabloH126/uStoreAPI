using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalificacionesController : ControllerBase
    {
        private readonly CalificacionesService calificacionesService;
        private readonly ProductosService productsService;
        private readonly TiendasService tiendasService;
        private IMapper mapper;

        public CalificacionesController(CalificacionesService _calificacionesService, ProductosService _productosService, TiendasService _tiendasService, IMapper _mapper)
        {
            calificacionesService = _calificacionesService;
            productsService = _productosService;
            tiendasService = _tiendasService;
            mapper = _mapper;
        }
    }
}
