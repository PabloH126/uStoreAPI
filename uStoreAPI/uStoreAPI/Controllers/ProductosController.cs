using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
            var productosSalida = mapper.Map<IEnumerable<ProductoDto>>(productos);
            foreach(var producto in productosSalida)
            {
                var imagenProducto = await productosService.GetPrincipalImageProducto(producto.IdProductos);
                if(imagenProducto is not null)
                {
                    producto.ImageProducto = imagenProducto.ImagenProducto;
                }
            }
            return Ok(productosSalida);
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

            var productoDto = mapper.Map<ProductoDto>(producto);

            var imagenProducto = await productosService.GetPrincipalImageProducto(producto.IdProductos);

            if (imagenProducto is not null)
            {
                productoDto.ImageProducto = imagenProducto.ImagenProducto;
            }
            return Ok(productoDto);
        }

        [HttpGet("GetImagenesProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ImagenesProducto>>> GetImagenesProducto(int idProducto)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var producto = await productosService.GetOneProducto(idProducto);
            if (producto is null)
            {
                return BadRequest("No hay una producto registrada con ese id");
            }

            var imagenesProducto = await productosService.GetImagenesProducto(idProducto);

            if (imagenesProducto.IsNullOrEmpty())
            {
                return NotFound("No hay imagenes registradas de este producto");
            }

            return Ok(imagenesProducto);
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

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            if(tienda.IdAdministrador != idUser)
            {
                return Unauthorized("Tienda no autorizada");
            }

            var producto = new Producto()
            {
                NombreProducto = productoDto.NombreProducto,
                PrecioProducto = productoDto.PrecioProducto,
                CantidadApartado = productoDto.CantidadApartado,
                Descripcion = productoDto.Descripcion,
                Stock = productoDto.Stock,
                IdTienda = productoDto.IdTienda
            };


            var productoCreado = await productosService.CreateProducto(producto);
            await tiendasService.UpdateRangoPrecio(tienda);

            return Ok(productoCreado.IdProductos);
        }

        [HttpPost("CreateImageProducto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductoDto>> CreateImagenProducto(int idProducto, IFormFile imagen)
        {
            if (imagen is null || imagen.Length == 0)
            {
                return BadRequest("Imagen no valida");
            }

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var producto = await productosService.GetOneProducto(idProducto);

            if (producto is null)
            {
                return NotFound();
            }

            var imagenesTotal = await productosService.GetImagenesProducto(producto.IdProductos);
            var imagenesCounter = imagenesTotal.Count() + 1;

            await productosService.CreateImagenesProducto(
                                    await CreateImagenProducto(
                                                    producto.IdProductos,
                                                    imagen,
                                                    $"{producto.IdProductos}/{imagenesCounter}")
                                    );

            return Ok();
        }

        [HttpPut("UpdateProducto")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProducto([FromBody] ProductoUpdateDto productoDto)
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

            producto.NombreProducto = productoDto.NombreProducto;
            producto.PrecioProducto = productoDto.PrecioProducto;
            producto.CantidadApartado = productoDto.CantidadApartado;
            producto.Descripcion = productoDto.Descripcion;

            await productosService.UpdateProducto(producto);

            return NoContent();
        }

        [HttpPut("UpdateImagenProducto")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateImagenProducto(int idProducto, int idImagenProducto, IFormFile? imagen)
        {
            if (imagen is null || imagen.Length == 0)
            {
                return BadRequest("Imagen invalida");
            }
            else
            {
                var user = HttpContext.User;
                var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

                var producto = await productosService.GetOneProducto(idProducto);
                
                if (producto is null)
                {
                    return NotFound("Ningun producto registrado con ese id");
                }

                var tienda = await tiendasService.GetOneTienda(producto.IdTienda);

                if (tienda!.IdAdministrador != idUser)
                {
                    return Unauthorized("Producto no autorizado");
                }
                else if (idImagenProducto == 0)
                {
                    var imagenesTotal = await productosService.GetImagenesProducto(idProducto);
                    var imagenesCounter = imagenesTotal.Count();

                    await productosService.CreateImagenesProducto(
                                            await CreateImagenProducto(
                                                        producto.IdProductos,
                                                        imagen,
                                                        $"{producto.IdProductos}/{imagenesCounter}"
                                                  )
                                            );
                    return NoContent();
                }

                var imagenProducto = await productosService.GetImagenProducto(idImagenProducto);
                var newImagenProducto = await CreateImagenProducto(producto.IdProductos, imagen, $"{producto.IdProductos}/{uploadService.GetBlobNameFromUrl(imagenProducto!.ImagenProducto)}");
                imagenProducto.ImagenProducto = newImagenProducto.ImagenProducto;

                await productosService.UpdateImagenesProducto(imagenProducto);


                return NoContent();
            }
        }

        [HttpPut("UpdateStockProducto")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStockProducto(int idProducto, int stock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var producto = await productosService.GetOneProducto(idProducto);

            if (producto is null)
            {
                return NotFound("Producto no registrado");
            }

            var tienda = await tiendasService.GetOneTienda(producto.IdTienda);

            if (tienda!.IdAdministrador != idUser)
            {
                return Unauthorized("Producto no autorizado");
            }

            producto.Stock = stock;

            await productosService.UpdateProducto(producto);

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
        private async Task<ImagenesProducto> CreateImagenProducto(int idProducto, IFormFile imagen, string fileName)
        {
            var imagenUrl = await uploadService.UploadImageProductos(imagen, fileName);
            return new ImagenesProducto
            {
                IdProductos = idProducto,
                ImagenProducto = imagenUrl
            };
        }
    }
}
