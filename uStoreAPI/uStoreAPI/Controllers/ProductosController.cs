using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ProductosService productosService;
        private readonly TiendasService tiendasService;
        private readonly UploadService uploadService;
        private IMapper mapper;
        public ProductosController(ProductosService _productosService, TiendasService _tiendasService, IMapper _mapper, UploadService _uploadService)
        {
            productosService = _productosService;
            tiendasService = _tiendasService;
            uploadService = _uploadService;
            mapper = _mapper;
        }

        [HttpGet("GetProductos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos(int idTienda)
        {
            if (await tiendasService.GetOneTienda(idTienda) is null)
            {
                return BadRequest("No hay una tienda registrada con ese id");
            }

            var productos = await productosService.GetProductos(idTienda);
            if (productos.IsNullOrEmpty())
            {
                return NotFound("No hay productos registrados para esa tienda");
            }
            return Ok(productos);
        }

        [HttpGet(Name = "GetProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var producto = await productosService.GetOneProducto(id);
            if(producto is null)
            {
                return NotFound("Producto no registrado");
            }
            return Ok(producto);
        }

        [HttpPost("CreateProducto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductoDto>> CreateProducto([FromBody] ProductoDto productoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tienda = await tiendasService.GetOneTienda(productoDto.IdTienda);
            
            if(tienda is null)
            {
                return BadRequest("No hay tienda registrada con ese id");
            }

            var producto = mapper.Map<Producto>(productoDto);

            await productosService.CreateProducto(producto);
            await tiendasService.UpdateRangoPrecio(tienda);

            return CreatedAtRoute("GetProducto", new { id = producto.IdProductos }, producto);
        }

        /*[HttpPost("CreateImageProducto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductoDto>> CreateProducto(int idProducto, IFormFile imagen)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (imagen is null || imagen.Length == 0)
            {
                return BadRequest("Imagen no valida");
            }
            else if (await productosService.GetOneProducto(idProducto) is null)
            {
                return NotFound("No hay producto registrado con ese id");
            }

            await uploadService.UploadImageProductos(imagen, $"{idProducto}/{await uploadService.CountBlobs("productos", idProducto.ToString())}");

            return CreatedAtRoute("GetProducto", new { id = producto.IdProductos }, producto);
        }
        */
        [HttpPut("UpdateProducto")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProducto([FromBody] ProductoDto productoDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var producto = await productosService.GetOneProducto(productoDto.IdProductos);

            if (producto is null)
            {
                return NotFound("Producto no registrado");
            }

            var tienda = await tiendasService.GetOneTienda(producto.IdTienda);

            if (tienda!.IdAdministrador != idUser)
            {
                return Unauthorized("Producto no autorizado");
            }

            await productosService.UpdateProducto(mapper.Map<Producto>(productoDto));

            return NoContent();
        }

        [HttpDelete("DeleteProducto")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            if (id == 0)
            {
                return BadRequest("Id invalido");
            }

            var producto = await productosService.GetOneProducto(id);

            if(producto is null)
            {
                return NotFound("Producto no registrado");
            }

            var tienda = await tiendasService.GetOneTienda(producto.IdTienda);

            if(tienda!.IdAdministrador != idUser)
            {
                return Unauthorized("Producto no autorizado");
            }

            await productosService.DeleteProducto(producto);
            return NoContent();
        }
    }
}
