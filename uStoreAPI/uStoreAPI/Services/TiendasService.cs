using AutoMapper;
using AutoMapper.Configuration.Conventions;
using Microsoft.EntityFrameworkCore;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class TiendasService
    {
        private readonly UstoreContext context;
        private IMapper mapper;
        public TiendasService(UstoreContext _context, IMapper _mapper)
        {
            context = _context;            
            mapper = _mapper;
        }

        public async Task<IEnumerable<TiendaDto>> GetTiendas(int idCentroComercial, int idAdministrador)
        {
            var tiendas = mapper.Map<IEnumerable<TiendaDto>>(
                          await context.Tienda.Where(t => 
                                                t.IdCentroComercial == idCentroComercial && 
                                                t.IdAdministrador == idAdministrador)
                                                .AsNoTracking()
                                                .ToListAsync());
            return tiendas;
        }

        public async Task<IEnumerable<ImagenesTienda>> GetImagenesTienda(int idTienda)
        {
            return await context.ImagenesTiendas.Where(p => p.IdTienda == idTienda).AsNoTracking().ToListAsync();
        }

        public async Task<Tiendum?> GetOneTienda(int? idTienda)
        {
            return await context.Tienda.FindAsync(idTienda);
        }

        public async Task<Tiendum> CreateTienda(Tiendum tienda)
        {
            await context.Tienda.AddAsync(tienda);
            await context.SaveChangesAsync();

            return tienda;
        }

        public async Task<ImagenesTienda> CreateImagenesTienda(ImagenesTienda imagenTienda)
        {
            await context.ImagenesTiendas.AddAsync(imagenTienda);
            await context.SaveChangesAsync();

            return imagenTienda;
        }

        public async Task<Tiendum> UpdateRangoPrecio(Tiendum tienda)
        {
            var rangoPrecio = await context.Productos.Where(p => p.IdTienda == tienda.IdTienda).AverageAsync(p => p.PrecioProducto);
            tienda.RangoPrecio = rangoPrecio.ToString();
            await UpdateTienda(tienda);
            return tienda;
        }

        public async Task UpdateTienda(Tiendum tienda)
        {
            context.Tienda.Update(tienda);
            await context.SaveChangesAsync();
        }
        
        public async Task UpdateImagenTienda(ImagenesTienda imagenTienda)
        {
            context.ImagenesTiendas.Update(imagenTienda);
            await context.SaveChangesAsync();
        }

        public async Task DeleteTienda(Tiendum tienda)
        {
            context.Tienda.Remove(tienda);
            await context.SaveChangesAsync();
        }

        public async Task DeleteImagenTienda(ImagenesTienda imagenTienda)
        {
            context.ImagenesTiendas.Remove(imagenTienda);
            await context.SaveChangesAsync();
        }

        public async Task DeleteImagenesTiendaWithId(int idTienda)
        {
            var imagenesTienda = await context.ImagenesTiendas.Where(p => p.IdTienda == idTienda).AsNoTracking().ToListAsync();
            foreach (var imagen in imagenesTienda)
            {
                context.ImagenesTiendas.Remove(imagen);
            }
            await context.SaveChangesAsync();
        }
    }
}
