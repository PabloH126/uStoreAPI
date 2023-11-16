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

        public async Task<IEnumerable<ListaTiendasAppDto>> GetAllTiendas(int idCentroComercial)
        {
            var tiendas = mapper.Map<IEnumerable<ListaTiendasAppDto>>(
                            await context.Tienda.Where(t => t.IdCentroComercial == idCentroComercial)
                            .AsNoTracking()
                            .ToListAsync());
            foreach (var tiendaDto in tiendas)
            {
                tiendaDto.Horario = mapper.Map<IEnumerable<HorarioDto>>(await context.Horarios.Where(p => p.IdTienda == tiendaDto.IdTienda).AsNoTracking().ToListAsync());
            }
            foreach (var tiendaDto in tiendas)
            {
                tiendaDto.CategoriasTienda = await (from catTienda in context.CategoriasTiendas
                                                    join cat in context.Categorias on catTienda.IdCategoria equals cat.IdCategoria
                                                    where (catTienda.IdTienda == tiendaDto.IdTienda)
                                                    select new CategoriasTiendaDto
                                                    {
                                                        IdCategoria = cat.IdCategoria,
                                                        IdTienda = tiendaDto.IdTienda,
                                                        NameCategoria = cat.Categoria1
                                                    })
                                                    .AsNoTracking()
                                                    .ToListAsync();
            }
            return tiendas;
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

        public async Task<IEnumerable<Tiendum>> GetTiendasOnlyAdmin(int idAdministrador)
        {
            var tiendas = await context.Tienda.Where(t =>
                                                t.IdAdministrador == idAdministrador)
                                                .AsNoTracking()
                                                .ToListAsync();
            return tiendas;
        }

        public async Task<IEnumerable<ImagenesTienda>> GetImagenesTienda(int idTienda)
        {
            return await context.ImagenesTiendas.Where(p => p.IdTienda == idTienda).AsNoTracking().ToListAsync();
        }

        public async Task<ImagenesTienda?> GetImagenTienda(int idImagenTienda)
        {
            return await context.ImagenesTiendas.FindAsync(idImagenTienda);
        }

        public async Task<Tiendum?> GetOneTienda(int? idTienda)
        {
            return await context.Tienda.FindAsync(idTienda);
        }

        public async Task<TiendaAppDto?> GetTiendaApp(int idTienda)
        {
            List<ProductoDto> productosPopulares = new List<ProductoDto>();
            var tienda = await context.Tienda.FindAsync(idTienda);
            if (tienda is null)
            {
                return null;
            }
            var tiendaAppDto = mapper.Map<TiendaAppDto>(tienda);
            tiendaAppDto.CategoriasTienda = await (from catT in context.CategoriasTiendas
                                             join cat in context.Categorias on catT.IdCategoria equals cat.IdCategoria
                                             where catT.IdTienda == tienda.IdTienda
                                             select new CategoriasTiendaDto
                                             {
                                                 IdCt = catT.IdCt,
                                                 IdCategoria = catT.IdCategoria,
                                                 IdTienda = catT.IdTienda,
                                                 NameCategoria = cat.Categoria1
                                             })
                                             .AsNoTracking()
                                             .ToListAsync();
            tiendaAppDto.Horario = mapper.Map<IEnumerable<HorarioDto>>(await context.Horarios.Where(p => p.IdTienda == tienda.IdTienda).AsNoTracking().ToListAsync());
            tiendaAppDto.CalificacionesTienda = mapper.Map<IEnumerable<CalificacionTiendaDto>>(await context.CalificacionTienda.Where(p => p.IdTienda == tienda.IdTienda).AsNoTracking().ToListAsync());
            tiendaAppDto.ComentariosTienda = mapper.Map<IEnumerable<ComentariosTiendaDto>>(await context.ComentariosTiendas.Where(p => p.IdTienda == tienda.IdTienda).AsNoTracking().ToListAsync());

            var solicitudesTienda = context.SolicitudesApartados.Where(p => p.IdTienda == tienda.IdTienda);

            var listaProductosPopulares = await solicitudesTienda
                                                            .GroupBy(p => p.IdProductos)
                                                            .Select(g => new
                                                            {
                                                                Id = g.Key,
                                                                CantidadUsuarios = g.Select(s => s.IdUsuario).Distinct().Count()
                                                            })
                                                            .OrderByDescending(p => p.CantidadUsuarios)
                                                            .Take(16)
                                                            .ToListAsync();
            foreach(var usuario in listaProductosPopulares)
            {
                var producto = mapper.Map<ProductoDto>(await context.Productos.FindAsync(usuario.Id));
                producto.ImageProducto = await context.ImagenesProductos.Where(p => p.IdProductos == producto.IdProductos).Select(p => p.ImagenProducto).FirstOrDefaultAsync();
                productosPopulares.Add(producto);
            }
            tiendaAppDto.ProductosPopularesTienda = productosPopulares;

            tiendaAppDto.ImagenesTienda = mapper.Map<IEnumerable<ImagenesTiendaDto>>(await context.ImagenesTiendas.Where(p => p.IdTienda == tiendaAppDto.IdTienda).AsNoTracking().ToListAsync());
            return tiendaAppDto;
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

        public async Task UpdateTienda(Tiendum updatedTienda)
        {
            context.Tienda.Update(updatedTienda);
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
