using Microsoft.EntityFrameworkCore;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class CalificacionesService
    {
        private readonly UstoreContext context;
        public CalificacionesService(UstoreContext _context)
        {
            context = _context;
        }

        public async Task<IEnumerable<CalificacionTiendum>> GetCalificacionesTienda(int idTienda)
        {
            var calificaciones = await context.CalificacionTienda.Where(p => p.IdTienda == idTienda).AsNoTracking().ToListAsync();
            return calificaciones;
        }

        public async Task<IEnumerable<CalificacionProducto>> GetCalificacionesProducto(int idProducto)
        {
            var calificaciones = await context.CalificacionProductos.Where(p => p.IdProductos == idProducto).AsNoTracking().ToListAsync();
            return calificaciones;
        }

        public async Task<CalificacionTiendum?> GetOneCalificacionTienda(int idCalificacion)
        {
            var calificacion = await context.CalificacionTienda.FindAsync(idCalificacion);
            return calificacion;
        }

        public async Task<CalificacionProducto?> GetOneCalificacionProducto(int idCalificacionProducto)
        {
            var calificacion = await context.CalificacionProductos.FindAsync(idCalificacionProducto);
            return calificacion;
        }

        public async Task<CalificacionTiendum> CreateCalificacionTienda(CalificacionTiendum calificacion)
        {
            await context.CalificacionTienda.AddAsync(calificacion);
            await context.SaveChangesAsync();
            return calificacion;
        }

        public async Task<CalificacionProducto> CreateCalificacionProducto(CalificacionProducto calificacion)
        {
            await context.CalificacionProductos.AddAsync(calificacion);
            await context.SaveChangesAsync();
            return calificacion;
        }

        public async Task UpdateCalificacionTienda(CalificacionTiendum calificacion)
        {
            context.CalificacionTienda.Update(calificacion);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCalificacionProducto(CalificacionProducto calificacion)
        {
            context.CalificacionProductos.Update(calificacion);
            await context.SaveChangesAsync();
        }

        public async Task DeleteOneCalificacionTienda(CalificacionTiendum calificacion)
        {
            context.CalificacionTienda.Remove(calificacion);
            await context.SaveChangesAsync();
        }

        public async Task DeleteOneCalificacionProducto(CalificacionProducto calificacion)
        {
            context.CalificacionProductos.Remove(calificacion);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAllCalificacionesTienda(int idTienda)
        {
            var calificaciones = await GetCalificacionesTienda(idTienda);
            foreach (var calificacion in calificaciones)
            {
                context.CalificacionTienda.Remove(calificacion);
            }
            await context.SaveChangesAsync();
        }

        public async Task DeleteAllCalificacionesProducto(int idProducto)
        {
            var calificaciones = await GetCalificacionesProducto(idProducto);
            foreach (var calificacion in calificaciones)
            {
                context.CalificacionProductos.Remove(calificacion);
            }
            await context.SaveChangesAsync();
        }
    }
}
