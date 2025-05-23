﻿using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
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
        private readonly CategoriasService categoriasService;
        private readonly ComentariosService comentariosService;
        private readonly CalificacionesService calificacionesService;
        private readonly SolicitudesApartadoService solicitudesService;
        private readonly UserService userService;
        private readonly UploadService uploadService;
        private IMapper mapper;
        public ProductosController(UserService _userService, SolicitudesApartadoService _solicitudesService, ComentariosService _comentariosService, CalificacionesService _calificacionesService, ProductosService _productosService, TiendasService _tiendasService, IMapper _mapper, UploadService _uploadService, CategoriasService _categoriasService)
        {
            productosService = _productosService;
            tiendasService = _tiendasService;
            uploadService = _uploadService;
            mapper = _mapper;
            categoriasService = _categoriasService;
            comentariosService = _comentariosService;
            calificacionesService = _calificacionesService;
            solicitudesService = _solicitudesService;
            userService = _userService;
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


        [HttpGet("GetProductoApp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductoAppDto>> GetProductoApp(int id)
        {
            try
            {
                var producto = await productosService.GetProductoApp(id);
                if (producto is null)
                {
                    return NotFound("Producto no registrado");
                }

                var user = HttpContext.User;
                var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

                producto.ComentariosProducto = await comentariosService.GetAllComentariosProducto(id);

                producto.IsFavorito = (await userService.VerifyFavoritoProducto(idUser, producto.IdProductos)) ? "corazon_lleno.png" : "corazon_vacio.png";

                var penalizacionUsuario = await userService.GetPenalizacionActualUsuario(idUser);
                if (penalizacionUsuario is not null)
                {
                    var tiempoRestante = penalizacionUsuario.FinPenalizacion!.Value - DateTime.UtcNow;
                    string? mensajeTiempoRestante = null;
                    if (tiempoRestante.Days > 365)
                    {
                        mensajeTiempoRestante = $"Indefinido";
                    }
                    else if ((tiempoRestante.Days / 30) > 0)
                    {
                        mensajeTiempoRestante = $"{tiempoRestante.Days / 30} meses {tiempoRestante.Days % 30} dias";
                    }
                    else if (tiempoRestante.Days > 0)
                    {
                        mensajeTiempoRestante = $"{tiempoRestante.Days} dias {tiempoRestante.Hours} horas";
                    }
                    else if (tiempoRestante.Hours > 0)
                    {
                        mensajeTiempoRestante = $"{tiempoRestante.Hours} horas {tiempoRestante.Minutes} minutos";
                    } 
                    else if (tiempoRestante.Minutes > 0)
                    {
                        mensajeTiempoRestante = $"{tiempoRestante.Minutes} minutos";
                    }
                    else
                    {
                        mensajeTiempoRestante = $"{tiempoRestante.Seconds} segundos";
                    }
                   
                    producto.IsUsuarioPenalizado = mensajeTiempoRestante;
                }
                
                return Ok(producto);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllProductoApp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ListaProductosAppDto>>> GetAllProductosApp(int idTienda)
        {
            if (await tiendasService.GetOneTienda(idTienda) is null)
            {
                return BadRequest("No se encontró una tienda con ese id");
            }
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var productos = await productosService.GetAllProductosTiendaApp(idTienda);

            if (productos is null)
            {
                return NotFound("No hay productos registrados en esta tienda");
            }

            var productosFavoritos = await userService.GetFavoritosProductoUsuario(productos.Select(p => p.IdProductos), idUser);

            if (!productosFavoritos.IsNullOrEmpty())
            {
                foreach (var producto in productos)
                {
                    if (productosFavoritos.Contains(producto.IdProductos))
                    {
                        producto.IsFavorito = "corazon_lleno.png";
                    }
                    else
                    {
                        producto.IsFavorito = "corazon_vacio.png";
                    }
                }
            }
            else
            {
                foreach (var producto in productos)
                {
                    producto.IsFavorito = "corazon_vacio.png";
                }
            }
            return Ok(productos);
        }

        [HttpGet("GetImagenesProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ImagenesProductoDto>>> GetImagenesProducto(int idProducto)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var producto = await productosService.GetOneProducto(idProducto);
            if (producto is null)
            {
                return BadRequest("No hay una producto registrada con ese id");
            }

            var imagenesProducto = mapper.Map<IEnumerable<ImagenesProductoDto>>(await productosService.GetImagenesProducto(idProducto));

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
                IdTienda = productoDto.IdTienda,
                FechaCreacion = DateTime.UtcNow
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
            imagenesTotal = imagenesTotal.Where(p => p != null);
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
            var cantidadApartadoInicial = producto.CantidadApartado;

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
            await tiendasService.UpdateRangoPrecio(tienda);

            if (producto.CantidadApartado > 0 && cantidadApartadoInicial == 0)
            {
                var alertasApartadoProducto = await userService.GetAlertasApartadoProducto(producto.IdProductos);
                foreach(var alertaApartado in alertasApartadoProducto)
                {
                    var alertaApartadoLocal = alertaApartado;
                    BackgroundJob.Enqueue(() => userService.NotificarExistenciaProducto((int)alertaApartadoLocal.IdUsuario!, (int)alertaApartadoLocal.IdProductos!));
                }
            }

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
                    imagenesTotal = imagenesTotal.Where(p => p != null);
                    var imagenesCounter = imagenesTotal.Count() + 1;
                    var nombreImagen = $"{producto.IdProductos}/{imagenesCounter}";
                    if (await productosService.VerifyImagenProducto(nombreImagen) is not null)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            nombreImagen = $"{producto.IdProductos}/{i + 1}";
                            if (await productosService.VerifyImagenProducto(nombreImagen) is null)
                            {
                                break;
                            }
                        }
                    }
                    await productosService.CreateImagenesProducto(
                                            await CreateImagenProducto(
                                                        producto.IdProductos,
                                                        imagen,
                                                        nombreImagen
                                                  )
                                            );
                    return NoContent();
                }

                var imagenProducto = await productosService.GetImagenProducto(idImagenProducto);
                var newImagenProducto = await CreateImagenProducto(producto.IdProductos, imagen, $"{producto.IdProductos}/{uploadService.GetBlobNameFromUrl(imagenProducto!.ImagenProducto)}");
                imagenProducto.ImagenProducto = newImagenProducto.ImagenProducto;
                imagenProducto.ImagenProductoThumbNail = newImagenProducto.ImagenProductoThumbNail;

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
            try
            {
                await solicitudesService.DeleteSolicitudesProducto(producto.IdProductos);
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }
            await uploadService.DeleteImagenesProductos($"{producto.IdProductos}");
            await productosService.DeleteImagenesProductoWithId(producto.IdProductos);
            await comentariosService.DeleteAllComentariosProducto(producto.IdProductos);
            await calificacionesService.DeleteAllCalificacionesProducto(producto.IdProductos);
            await categoriasService.DeleteAllCategoriasProducto(producto.IdProductos);
            await productosService.DeleteProducto(producto);
            return NoContent();
        }

        [HttpDelete("DeleteImagenProducto")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteImagenProducto(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var imagenProducto = await productosService.GetImagenProducto(id);
            if(imagenProducto is null)
            {
                return NotFound();
            }
            await uploadService.DeleteImageProducto(imagenProducto.IdProductos.ToString()!, uploadService.GetBlobNameFromUrl(imagenProducto.ImagenProducto));
            await productosService.DeleteImagenProducto(imagenProducto);

            return NoContent();
        }
        private async Task<ImagenesProducto> CreateImagenProducto(int idProducto, IFormFile imagen, string fileName)
        {
            var imagenUrl = await uploadService.UploadImageProductos(imagen, fileName);
            return new ImagenesProducto
            {
                IdProductos = idProducto,
                ImagenProducto = imagenUrl[0],
                ImagenProductoThumbNail = imagenUrl[1]
            };
        }
    }
}
