using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class PublicacionesService
    {
        private readonly UstoreContext context;
        private readonly UploadService uploadService;
        public PublicacionesService(UstoreContext _context, UploadService _uploadService)
        {
            context = _context;
            uploadService = _uploadService;

        }

        public async Task<IEnumerable<Publicacione>> GetPublicacionesRecientesApp(int idCentroComercial)
        {
            DateTime fechaMaxima = DateTime.UtcNow.AddMonths(-2);
            return await context.Publicaciones.Where(p => p.FechaPublicacion >= fechaMaxima && p.IdCentroComercial == idCentroComercial)
                                              .OrderByDescending(p => p.FechaPublicacion)
                                              .ThenByDescending(p => p.IdPublicacion)
                                              .AsNoTracking()
                                              .ToListAsync();
        }

        public async Task<IEnumerable<Publicacione>> GetPublicacionesRecientes(int idTienda)
        {
            DateTime fechaMaxima = DateTime.UtcNow.AddMonths(-2);
            return await context.Publicaciones.Where(p => p.FechaPublicacion >= fechaMaxima && p.IdTienda == idTienda)
                                              .OrderByDescending(p => p.FechaPublicacion)
                                              .ThenByDescending(p => p.IdPublicacion)
                                              .AsNoTracking()
                                              .ToListAsync();
        }

        public async Task<Publicacione?> GetPublicacion(int id)
        {
            return await context.Publicaciones.FindAsync(id);
        }

        public async Task<Publicacione> CreatePublicacion(Publicacione publicacion)
        {
            await context.Publicaciones.AddAsync(publicacion);
            await context.SaveChangesAsync();
            return publicacion;
        }

        public async Task UpdatePublicacion(Publicacione publicacion)
        {
            context.Publicaciones.Update(publicacion);
            await context.SaveChangesAsync();
        }

        public async Task DeletePublicacion(Publicacione publicacion)
        {
            context.Publicaciones.Remove(publicacion);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAllPublicaciones(int idTienda)
        {
            var publicaciones = await context.Publicaciones.Where(p => p.IdTienda == idTienda).ToListAsync();
            if (!publicaciones.IsNullOrEmpty())
            {
                foreach (var publicacion in publicaciones)
                {
                    await uploadService.DeleteImagePublicacion(publicacion.IdPublicacion.ToString());
                    context.Publicaciones.Remove(publicacion);
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
