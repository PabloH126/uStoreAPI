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

        public async Task<Producto> CreateProducto(Producto producto)
        {
            await context.Productos.AddAsync(producto);
            await context.SaveChangesAsync();
            return producto;
        }

        public async Task UpdateProducto(Producto producto)
        {
            context.Productos.Update(producto);
            await context.SaveChangesAsync();
        }
        
        public async Task DeleteProducto(Producto producto)
        {
            context.Productos.Remove(producto); 
            await context.SaveChangesAsync();
        }
    }
}
