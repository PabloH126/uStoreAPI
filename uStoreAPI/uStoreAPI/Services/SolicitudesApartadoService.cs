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

        public async Task<Dictionary<int, int>> GetSolicitudesApartadoTiendas(int idAdministrador)
        {
            var tiendas = await context.Tienda.Where(p => p.IdAdministrador == idAdministrador).Select(p => p.IdTienda).ToListAsync();
            var solicitudes = await context.SolicitudesApartados.Where(p => tiendas.Contains((int)p.IdTienda!) && p.StatusSolicitud == "pendiente")
                                                                .GroupBy(p => p.IdTienda)
                                                                .ToDictionaryAsync(
                                                                    tienda => (int)tienda.Key!,
                                                                    notificaciones => notificaciones.Count()
                                                                 );
            return solicitudes;
        }

        public async Task<IEnumerable<SolicitudesApartado>> GetSolicitudesApartado(int idProducto)
        {
            return await context.SolicitudesApartados.Where(p => p.IdProductos == idProducto).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<SolicitudesApartado>> GetSolicitudesApartadoWithIdTienda(int idTienda)
        {
            return await context.SolicitudesApartados.Where(p => p.IdTienda == idTienda).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<SolicitudesApartado>> GetSolicitudesApartadoPendientesWithIdTienda(int idTienda)
        {
            return await context.SolicitudesApartados.Where(p => p.IdTienda == idTienda && p.StatusSolicitud == "pendiente").AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<SolicitudesApartado>> GetSolicitudesApartadoActivasWithIdTienda(int idTienda)
        {
            return await context.SolicitudesApartados.Where(p => p.IdTienda == idTienda && (p.StatusSolicitud == "activa" || p.StatusSolicitud == "vencida")).AsNoTracking().ToListAsync();
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

        public async Task DeleteSolicitudesTienda(int idTienda)
        {
            var solicitudes = await context.SolicitudesApartados.Where(p => p.IdTienda == idTienda).ToListAsync();
            context.SolicitudesApartados.RemoveRange(solicitudes);
            await context.SaveChangesAsync();
        }

        public async Task MarcarComoVencida(int idSolicitud)
        {
            var solicitud = await context.SolicitudesApartados.FindAsync(idSolicitud);
            var producto = await context.Productos.FindAsync(solicitud!.IdProductos);
            if (solicitud is not null && solicitud.StatusSolicitud == "activa")
            {
                producto!.CantidadApartado += solicitud.UnidadesProducto;
                solicitud.StatusSolicitud = "vencida";
                context.SolicitudesApartados.Update(solicitud);
                var usuario = await context.Usuarios.FindAsync(solicitud.IdUsuario);
                var detallesUsuario = await context.DetallesUsuarios.FindAsync(usuario!.IdDetallesUsuario);
                detallesUsuario!.ApartadosFallidos += 1;
                context.DetallesUsuarios.Update(detallesUsuario);
                await context.SaveChangesAsync();
            }
        }
    }
}
