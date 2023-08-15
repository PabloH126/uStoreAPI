using Microsoft.EntityFrameworkCore;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class CategoriasService
    {
        private readonly UstoreContext context;
        public CategoriasService(UstoreContext _context)
        {
            context = _context;
        }

        public async Task<IEnumerable<Categoria>> GetCategorias()
        {
            return await context.Categorias
                                .AsNoTracking()
                                .OrderBy(c => c.Categoria1)
                                .ToListAsync();
        }

        public async Task<IEnumerable<Categoria>> GetCategoriasTienda(int idTienda)
        {
            var categoriasTienda = await context.CategoriasTiendas.Where(p => p.IdTienda == idTienda)
                                                                  .Select(p => p.IdCategoria)
                                                                  .ToListAsync();
            var categorias = await context.Categorias.Where(p => categoriasTienda.Contains(p.IdCategoria)).AsNoTracking().ToListAsync();
            return categorias;
        }

        public async Task<IEnumerable<Categoria>> GetCategoriasProducto(int idProducto)
        {
            var categoriasProducto = await context.CategoriasProductos.Where(p => p.IdProductos == idProducto)
                                                                      .Select(p => p.IdCategoria)
                                                                      .ToListAsync();

            var categorias = await context.Categorias.Where(p => categoriasProducto.Contains(p.IdCategoria)).AsNoTracking().ToListAsync();
            return categorias;
        }

        public async Task<Categoria?> GetOneCategoria(int idCategoria)
        {
            return await context.Categorias.FindAsync(idCategoria);
        }

        public async Task<CategoriasTienda?> GetOneCategoriaTienda(int id)
        {
            return await context.CategoriasTiendas.FindAsync(id);
        }

        public async Task<CategoriasProducto?> GetOneCategoriaProducto(int id)
        {
            return await context.CategoriasProductos.FindAsync(id);
        }

        public async Task<CategoriasTienda?> GetOneCategoriaTiendaWithIdCategoriaTienda(int idTienda, int idCategoria)
        {
            return await context.CategoriasTiendas.FirstOrDefaultAsync(p => p.IdTienda == idTienda && p.IdCategoria == idCategoria);
        }

        public async Task<CategoriasProducto?> GetOneCategoriaProductoWithIdCategoriaProducto(int idProducto, int idCategoria)
        {
            return await context.CategoriasProductos.FirstOrDefaultAsync(p => p.IdProductos == idProducto && p.IdCategoria == idCategoria);
        }

        public async Task<Categoria> CreateCategoria(Categoria categoria)
        {
            await context.Categorias.AddAsync(categoria);
            await context.SaveChangesAsync();
            return categoria;
        }

        public async Task<IEnumerable<CategoriasTienda>> CreateAllCategoriasTienda(IEnumerable<CategoriasTienda> categorias)
        {
            foreach (var categoria in categorias)
            {
                await context.CategoriasTiendas.AddAsync(categoria);
            }
            await context.SaveChangesAsync();
            return categorias;
        }

        public async Task<IEnumerable<CategoriasProducto>> CreateAllCategoriasProducto(IEnumerable<CategoriasProducto> categorias)
        {
            foreach (var categoria in categorias)
            {
                await context.CategoriasProductos.AddAsync(categoria);
                await context.SaveChangesAsync();
            }

            return categorias;
        }

        public async Task UpdateCategoria(Categoria categoria)
        {
            context.Categorias.Update(categoria);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCategoriaTienda(CategoriasTienda categoria)
        {
            context.CategoriasTiendas.Update(categoria);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCategoriaProducto(CategoriasProducto categoria)
        {
            context.CategoriasProductos.Update(categoria);
            await context.SaveChangesAsync();
        }

        public async Task DeleteCategoria(Categoria categoria)
        {
            context.Categorias.Remove(categoria);
            await context.SaveChangesAsync();
        }

        public async Task DeleteCategoriaTienda(CategoriasTienda categoria)
        {
            context.CategoriasTiendas.Remove(categoria);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAllCategoriasTienda(int idTienda)
        {
            var categorias = await context.CategoriasTiendas.Where(c => c.IdTienda == idTienda).AsNoTracking().ToListAsync();
            foreach (var categoria in categorias)
            {
                context.CategoriasTiendas.Remove(categoria);
                
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteCategoriaProducto(CategoriasProducto categoria)
        {
            context.CategoriasProductos.Remove(categoria);
            await context.SaveChangesAsync();
        }
    }
}
