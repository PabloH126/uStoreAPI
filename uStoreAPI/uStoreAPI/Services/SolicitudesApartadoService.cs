using Microsoft.EntityFrameworkCore;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class SolicitudesApartadoService
    {
        private readonly UstoreContext context;
        public SolicitudesApartadoService(UstoreContext _context)
        {
            context= _context;
        }

        public async Task<IEnumerable<SolicitudesApartado>> GetSolicitudesApartado(int idProducto)
        {
            return await context.SolicitudesApartados.Where(p => p.IdProductos == idProducto).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<SolicitudesApartado>> GetSolicitudesApartadoWithIdTienda(int idTienda)
        {
            return await context.SolicitudesApartados.Where(p => p.IdTienda == idTienda).AsNoTracking().ToListAsync();
        }

        public async Task<SolicitudesApartado?> GetOneSolicitudApartado(int idSolicitud)
        {
            return await context.SolicitudesApartados.FindAsync(idSolicitud);
        }

        public async Task<SolicitudesApartado> CreateSolicitud(SolicitudesApartado solicitud)
        {
            await context.SolicitudesApartados.AddAsync(solicitud);
            await context.SaveChangesAsync();
            return solicitud;
        }

        public async Task UpdateSolicitud(SolicitudesApartado solicitud)
        {
            context.SolicitudesApartados.Update(solicitud);
            await context.SaveChangesAsync();
        }

        public async Task DeleteSolicitud(SolicitudesApartado solicitud)
        {
            context.SolicitudesApartados.Remove(solicitud);
            await context.SaveChangesAsync();
        }
    }
}
