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
            var calificacionExistente = await context.CalificacionTienda.FirstOrDefaultAsync(p => p.IdTienda == calificacion.IdTienda && p.IdUsuario ==  calificacion.IdUsuario);
            if (calificacionExistente is not null)
            {
                calificacionExistente.Calificacion = calificacion.Calificacion;
                await UpdateCalificacionTienda(calificacion);
                return calificacionExistente;
            }
            else
            {
                await context.CalificacionTienda.AddAsync(calificacion);
                await context.SaveChangesAsync();
                return calificacion;
            }
        }

        public async Task<CalificacionProducto> CreateCalificacionProducto(CalificacionProducto calificacion)
        {
            var calificacionExistente = await context.CalificacionProductos.FirstOrDefaultAsync(p => p.IdProductos == calificacion.IdProductos && p.IdUsuario == calificacion.IdUsuario);
            if (calificacionExistente is not null)
            {
                calificacionExistente.Calificacion = calificacion.Calificacion;
                await UpdateCalificacionProducto(calificacion);
                return calificacion;
            }
            else
            {
                await context.CalificacionProductos.AddAsync(calificacion);
                await context.SaveChangesAsync();
                return calificacion;
            }
        }

        public async Task UpdateCalificacionTienda(CalificacionTiendum calificacion)
        {
            var calificacionExistente = await context.CalificacionTienda.FirstOrDefaultAsync(p => p.IdUsuario == calificacion.IdUsuario && p.IdTienda == calificacion.IdTienda);
            calificacionExistente!.Calificacion = calificacion.Calificacion;
            context.CalificacionTienda.Update(calificacionExistente!);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCalificacionProducto(CalificacionProducto calificacion)
        {
            var calificacionExistente = await context.CalificacionProductos.FirstOrDefaultAsync(p => p.IdUsuario == calificacion.IdUsuario && p.IdProductos == calificacion.IdProductos);
            calificacionExistente!.Calificacion = calificacion.Calificacion;
            context.CalificacionProductos.Update(calificacionExistente!);
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
