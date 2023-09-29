using Microsoft.EntityFrameworkCore;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class ProductosService
    {
        private readonly UstoreContext context;

        public ProductosService(UstoreContext _context)
        {
            context = _context;
        }

        public async Task<IEnumerable<Producto>> GetProductos(int? idTienda)
        {
            return await context.Productos.Where(p => p.IdTienda == idTienda).AsNoTracking().ToListAsync();
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
