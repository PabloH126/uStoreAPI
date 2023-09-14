using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TendenciasVentaController : ControllerBase
    {
        private readonly ProductosService productosService;
        private readonly TiendasService tiendasService;
        private readonly SolicitudesApartadoService solicitudesApartadoService;
        private IMapper mapper;

        public TendenciasVentaController(ProductosService _productosService, TiendasService _tiendasService, SolicitudesApartadoService _solicitudesApartadoService, IMapper _mapper)
        {
            productosService = _productosService;
            tiendasService = _tiendasService;
            solicitudesApartadoService = _solicitudesApartadoService;
            mapper = _mapper;
        }
    }
}
