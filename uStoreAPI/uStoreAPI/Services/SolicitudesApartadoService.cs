using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class SolicitudesApartadoService
    {
        private readonly UstoreContext context;
        private readonly UserService userService;
        private IMapper mapper;
        public SolicitudesApartadoService(UstoreContext _context, IMapper _mapper, UserService _us)
        {
            context = _context;
            mapper = _mapper;
            userService = _us;
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
            return await context.SolicitudesApartados.Where(p => p.IdTienda == idTienda && (p.StatusSolicitud == "activa" || p.StatusSolicitud == "vencida" || p.StatusSolicitud == "recogida"))
                                                     .OrderBy(p => p.StatusSolicitud == "vencida" ? 1 : 0)
                                                     .ThenBy(p => p.FechaVencimiento)
                                                     .ThenBy(p => p.FechaSolicitud)
                                                     .AsNoTracking()
                                                     .ToListAsync();
        }

        public async Task<IEnumerable<SolicitudesApartadoDto>> GetSolicitudesApartadoUsuario(int idUsuario)
        {
            var solicitudes = mapper.Map<List<SolicitudesApartadoDto>>(
                                await context.SolicitudesApartados.Where(p => p.IdUsuario == idUsuario && (p.StatusSolicitud == "activa" || p.StatusSolicitud == "pendiente"))
                                                                     .OrderBy(p => p.FechaVencimiento)
                                                                     .ThenBy(p => p.FechaSolicitud)
                                                                     .AsNoTracking()
                                                                     .ToListAsync()
                              );
            foreach (var solicitud in solicitudes)
            {
                var tiendaSolicitud = await context.Tienda.FindAsync(solicitud.IdTienda);
                var productoSolicitud = await context.Productos.FindAsync(solicitud.IdProductos);
                solicitud.ImageProducto = await context.ImagenesProductos.Where(p => p.IdProductos == productoSolicitud!.IdProductos).Select(p => p.ImagenProducto).FirstOrDefaultAsync();
                solicitud.NombreProducto = productoSolicitud.NombreProducto;
                solicitud.PrecioProducto = productoSolicitud.PrecioProducto;
                solicitud.NombreTienda = tiendaSolicitud.NombreTienda;
            }

            return solicitudes;
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
            if (!solicitudes.IsNullOrEmpty())
            {
                context.SolicitudesApartados.RemoveRange(solicitudes);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteSolicitudesProducto(int idProducto)
        {
            var solicitudes = await context.SolicitudesApartados.Where(p => p.IdProductos == idProducto).ToListAsync();
            if (!solicitudes.IsNullOrEmpty())
            {
                context.SolicitudesApartados.RemoveRange(solicitudes);
                await context.SaveChangesAsync();
            }
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

                var penalizacionesUsuario = await userService.GetPenalizacionesUsuario((int)solicitud.IdUsuario!);
                var penalizacionNueva = await userService.CreatePenalizacion((int)solicitud.IdUsuario!);

                if (!penalizacionesUsuario.IsNullOrEmpty())
                {
                    foreach (var penalizacion in penalizacionesUsuario)
                    {
                        if (!string.IsNullOrEmpty(penalizacion.IdJob))
                        {
                            BackgroundJob.Delete(penalizacion.IdJob);
                            penalizacion.IdJob = null;
                        }
                    }
                }

                var jobId = BackgroundJob.Schedule(() => userService.DeletePenalizacionesUser((int)solicitud.IdUsuario!), 
                                                                                              penalizacionNueva.FinPenalizacion!.Value.AddMonths(1));
                
                penalizacionNueva.IdJob = jobId;
                await userService.PatchPenalizacionUsuario(penalizacionNueva);
            }
        }
    }
}
