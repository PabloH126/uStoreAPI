using AutoMapper;
using Microsoft.EntityFrameworkCore;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class ProductosService
    {
        private readonly UstoreContext context;
        private IMapper mapper;

        public ProductosService(UstoreContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        public async Task<IEnumerable<Producto>> GetProductos(int? idTienda)
        {
            return await context.Productos.Where(p => p.IdTienda == idTienda).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<ListaProductosAppDto>> GetAllProductosTiendaApp(int idTienda)
        {
            var listaProductosTiendaApp = new List<ListaProductosAppDto>();
            var solicitudesTienda = context.SolicitudesApartados.Where(p => p.IdTienda == idTienda);
            var listaProductosPopulares = await solicitudesTienda
                                                            .GroupBy(p => p.IdProductos)
                                                            .Select(g => new
                                                            {
                                                                Id = g.Key,
                                                                CantidadUsuarios = g.Select(s => s.IdUsuario).Distinct().Count()
                                                            })
                                                            .OrderByDescending(p => p.CantidadUsuarios)
                                                            .ToListAsync();
            foreach(var productoPopular in listaProductosPopulares)
            {
                 var producto = mapper.Map<ListaProductosAppDto>(await context.Productos.FindAsync(productoPopular.Id));
                 producto.ImageProducto = await context.ImagenesProductos.Where(p => p.IdProductos == producto.IdProductos).Select(p => p.ImagenProducto).FirstOrDefaultAsync();
                 producto.NumeroSolicitudes = productoPopular.CantidadUsuarios;
                 listaProductosTiendaApp.Add(producto);
            }
            var idProductosPopulares = listaProductosPopulares.Select(p => p.Id).ToList();
            var allProductsList = await context.Productos
                                                    .Where(p => !idProductosPopulares.Contains(p.IdProductos) && p.IdTienda == idTienda)
                                                    .ToListAsync();
            foreach (var producto in allProductsList)
            {
                var productoDto = mapper.Map<ListaProductosAppDto>(await context.Productos.FindAsync(producto.IdProductos));
                productoDto.ImageProducto = await context.ImagenesProductos.Where(p => p.IdProductos == producto.IdProductos).Select(p => p.ImagenProducto).FirstOrDefaultAsync();
                productoDto.NumeroSolicitudes = 0;
                listaProductosTiendaApp.Add(productoDto);
            }
            return listaProductosTiendaApp;
        }

        public async Task<ProductoAppDto?> GetProductoApp(int idProducto)
        {
            var producto = await context.Productos.FindAsync(idProducto);
            if (producto is null)
            {
                return null;
            }
            var productoAppDto = mapper.Map<ProductoAppDto>(producto);
            productoAppDto.CategoriasProducto = await (from catP in context.CategoriasProductos
                                                       join cat in context.Categorias on catP.IdCategoria equals cat.IdCategoria
                                                       where catP.IdProductos == producto.IdProductos
                                                       select new CategoriasProductoDto
                                                       {
                                                           IdCp = catP.IdCp,
                                                           IdCategoria = catP.IdCategoria,
                                                           IdProductos = catP.IdProductos,
                                                           NameCategoria = cat.Categoria1
                                                       })
                                                       .AsNoTracking()
                                                       .ToListAsync();
            productoAppDto.CalificacionesProducto = mapper.Map<IEnumerable<CalificacionProductoDto>>(await context.CalificacionProductos.Where(p => p.IdProductos == producto.IdProductos).AsNoTracking().ToListAsync());
            productoAppDto.ComentariosProducto = mapper.Map<IEnumerable<ComentariosProductoDto>>(await context.ComentariosProductos.Where(p => p.IdProducto == producto.IdProductos).AsNoTracking().ToListAsync());
            productoAppDto.ImagenesProducto = mapper.Map<IEnumerable<ImagenesProductoDto>>(await context.ImagenesProductos.Where(p => p.IdProductos == producto.IdProductos).AsNoTracking().ToListAsync());
            productoAppDto.ImageProducto = productoAppDto.ImagenesProducto.First().ImagenProducto;

            var categoriasProducto = productoAppDto.CategoriasProducto.Select(p => p.IdCategoria).ToList();
            
            var productosRelacionados = await (from p in context.Productos
                                               join cP in context.CategoriasProductos on p.IdProductos equals cP.IdProductos
                                               join cat in context.Categorias on cP.IdCategoria equals cat.IdCategoria
                                               where p.IdTienda == producto.IdTienda
                                               group cP by p.IdProductos into grupoProductos
                                               select new
                                               {
                                                   idProducto = grupoProductos.Key,
                                                   Coincidencias = grupoProductos.Count(g => categoriasProducto.Contains(g.IdCategoria))
                                               })
                                               .OrderByDescending(p => p.Coincidencias)
                                               .AsNoTracking()
                                               .ToListAsync();
            var idsProductosRelacionados = productosRelacionados.Select(p => p.idProducto).ToList();
            var productosRelacionadosTemp = await context.Productos.Where(p => idsProductosRelacionados.Contains(p.IdProductos)).AsNoTracking().ToListAsync();
            var productosRelacionadosOrdenados = productosRelacionadosTemp
                                                    .Where(p => p.IdProductos != producto.IdProductos)
                                                    .OrderBy(p => idsProductosRelacionados.IndexOf(p.IdProductos))
                                                    .Select(p => mapper.Map<ProductoDto>(p))
                                                    .ToList();

            foreach(var productoRelacionado in productosRelacionadosOrdenados)
            {
                productoRelacionado.ImageProducto = await context.ImagenesProductos.Where(p => p.IdProductos == productoRelacionado.IdProductos).Select(p => p.ImagenProducto).FirstAsync();
            }

            productoAppDto.ProductosRelacionados = productosRelacionadosOrdenados;
            
            return productoAppDto;
        }

        public async Task<Producto?> GetOneProducto(int? id)
        {
            return await context.Productos.FindAsync(id);
        }

        public async Task<IEnumerable<ImagenesProducto>> GetImagenesProducto(int? idProducto)
        {
            return await context.ImagenesProductos.Where(p => p.IdProductos == idProducto).AsNoTracking().ToListAsync();
        }

        public async Task<ImagenesProducto?> GetImagenProducto(int? idImagenProducto)
        {
            return await context.ImagenesProductos.FindAsync(idImagenProducto);
        }

        public async Task<ImagenesProducto?> GetPrincipalImageProducto(int? idProducto)
        {
            var firstImagen = await context.ImagenesProductos.FirstOrDefaultAsync(p => p.IdProductos == idProducto);
            return firstImagen;
        }

        public async Task<Producto> CreateProducto(Producto producto)
        {
            await context.Productos.AddAsync(producto);
            await context.SaveChangesAsync();
            return producto;
        }

        public async Task<ImagenesProducto> CreateImagenesProducto(ImagenesProducto imagenProducto)
        {
            await context.ImagenesProductos.AddAsync(imagenProducto); 
            await context.SaveChangesAsync();
            return imagenProducto;
        }

        public async Task UpdateProducto(Producto producto)
        {
            context.Productos.Update(producto);
            await context.SaveChangesAsync();
        }

        public async Task UpdateImagenesProducto(ImagenesProducto imagenProducto)
        {
            context.ImagenesProductos.Update(imagenProducto);
            await context.SaveChangesAsync();
        }
        
        public async Task DeleteProducto(Producto producto)
        {
            context.Productos.Remove(producto); 
            await context.SaveChangesAsync();
        }

        public async Task DeleteImagenProducto(ImagenesProducto imagenProducto)
        {
            context.ImagenesProductos.Remove(imagenProducto);
            await context.SaveChangesAsync();
        }

        public async Task DeleteImagenesProductoWithId(int idProducto)
        {
            var imagenesProducto = await context.ImagenesProductos.Where(p => p.IdProductos == idProducto).AsNoTracking().ToListAsync();
            context.ImagenesProductos.RemoveRange(imagenesProducto);
            await context.SaveChangesAsync();
        }

        public async Task<bool> VerificarProductoTienda(int idProducto, int idTienda)
        {
            var producto = await context.Productos.FirstOrDefaultAsync(p => p.IdProductos == idProducto && p.IdTienda == idTienda);
            if (producto is null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
