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
