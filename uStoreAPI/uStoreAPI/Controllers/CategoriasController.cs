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
    public class CategoriasController : ControllerBase
    {
        private readonly CategoriasService categoriasService;
        private readonly TiendasService tiendasService;
        private readonly ProductosService productosService;
        private IMapper mapper;
        public CategoriasController(CategoriasService _categoriasService, TiendasService _tiendasService, ProductosService _productosService,IMapper _mapper)
        {
            categoriasService = _categoriasService;
            tiendasService = _tiendasService;
            productosService = _productosService;
            mapper = _mapper;
        }

        //GET ALL CATEGORIAS
        #region GetALL
        [HttpGet("GetCategorias")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            var categorias = await categoriasService.GetCategorias();
            return Ok(categorias);
        }

        [HttpGet("GetCategoriasTienda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategoriasTienda(int idTienda)
        {
            if(idTienda == 0)
            {
                return BadRequest("Id invalido");
            }
            else if(await tiendasService.GetOneTienda(idTienda) is null)
            {
                return NotFound("Tienda no registrada");
            }
            var categorias = await categoriasService.GetCategoriasTienda(idTienda);
            if(categorias.IsNullOrEmpty())
            {
                return NotFound("No hay categorias registradas para esta tienda");
            }
            return Ok(categorias);
        }

        [HttpGet("GetCategoriasProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategoriasProducto(int idProducto)
        {
            if (idProducto == 0)
            {
                return BadRequest("Id invalido");
            }
            else if (await productosService.GetOneProducto(idProducto) is null)
            {
                return NotFound("Producto no registrado");
            }
            var categorias = await categoriasService.GetCategoriasProducto(idProducto);
            if(categorias.IsNullOrEmpty())
            {
                return NotFound("No hay categorias registradas para este producto");
            }
            return Ok(categorias);
        }

        #endregion

        //GET CATEGORIAS
        #region GetOne
        [HttpGet(Name = "GetCategoria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
        {
            var categoria = await categoriasService.GetOneCategoria(id);
            if(categoria is null)
            {
                return NotFound("Categoria no registrada");
            }
            return Ok(categoria);
        }

        [HttpGet("GetCategoriaTienda", Name = "GetCategoriaTienda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoriasTiendaDto>> GetCategoriaTienda(int id)
        {
            var categoria = await categoriasService.GetOneCategoriaTienda(id);
            if (categoria is null)
            {
                return NotFound("Categoria no registrada para esa tienda");
            }
            return Ok(categoria);
        }

        [HttpGet("GetCategoriaProducto", Name = "GetCategoriaProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoriasProductoDto>> GetCategoriaProducto(int id)
        {
            var categoria = await categoriasService.GetOneCategoriaProducto(id);
            if (categoria is null)
            {
                return NotFound("Categoria no registrada para ese producto");
            }
            return Ok(categoria);
        }
        #endregion

        //CREATE CATEGORIAS
        #region Create
        [HttpPost("CreateCategoria")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoriaDto>> CreateCategoria([FromBody] CategoriaDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoria = mapper.Map<Categoria>(categoriaDto);
            await categoriasService.CreateCategoria(categoria);
            return CreatedAtRoute("GetCategoria", new { id = categoria.IdCategoria}, categoria);
        }

        [HttpPost("CreateCategoriaTienda")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CategoriasTiendaDto>>> CreateCategoriaTienda([FromBody] IEnumerable<CategoriasTiendaDto> categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (await tiendasService.GetOneTienda(categoriaDto.FirstOrDefault()!.IdTienda) is null)
            {
                return NotFound("No se encontro una tienda registrada");
            }

            var categoria = mapper.Map<IEnumerable<CategoriasTienda>>(categoriaDto);
            await categoriasService.CreateAllCategoriasTienda(categoria);
            return Ok();
        }

        [HttpPost("CreateCategoriaProducto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoriasProductoDto>> CreateCategoriaProducto([FromBody] IEnumerable<CategoriasProductoDto> categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (await productosService.GetOneProducto(categoriaDto.FirstOrDefault()!.IdProductos) is null)
            {
                return NotFound("No se encontro un producto registrado");
            }
            var categoria = mapper.Map<IEnumerable<CategoriasProducto>>(categoriaDto);
            await categoriasService.CreateAllCategoriasProducto(categoria);
            return Ok();
        }
        #endregion

        //DELETE CATEGORIAS
        #region Delete
        [HttpDelete("DeleteCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            if(id == 0)
            {
                return BadRequest("Id invalido");
            }
            var categoria = await categoriasService.GetOneCategoria(id);
            if(categoria is null)
            {
                return NotFound("Categoria no registrada");
            }
            await categoriasService.DeleteCategoria(categoria);
            return NoContent();
        }
        #endregion

        //UPDATE CATEGORIAS
        #region Update
        [HttpPut("UpdateCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategoria(string catego, int idCategoria)
        {
            if(catego is null)
            {
                return BadRequest("Categoria no valida");
            }

            var categoria = await categoriasService.GetOneCategoria(idCategoria);
            if(categoria is null)
            {
                return NotFound("Categoria no registrada");
            }
            categoria.Categoria1 = catego;
            await categoriasService.UpdateCategoria(categoria);

            return NoContent();
        }

        [HttpPut("UpdateCategoriasTienda")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategoriasTienda([FromBody] IEnumerable<CategoriasTiendaDto> categorias)
        {
            if (categorias is null || !ModelState.IsValid)
            {
                return BadRequest("Categorias no validas");
            }

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var tienda = await tiendasService.GetOneTienda(categorias.FirstOrDefault()!.IdTienda);

            if (tienda is null)
            {
                return BadRequest("No hay una tienda registrada con ese id");
            }
            else if (!(tienda.IdAdministrador == idUser))
            {
                return Unauthorized("Tienda no autorizada");
            }

            var categoriasLista = mapper.Map<IEnumerable<CategoriasTienda>>(categorias);
            await categoriasService.UpdateAllCategoriasTienda(categoriasLista);


            return NoContent();
        }
        #endregion
    }
}
