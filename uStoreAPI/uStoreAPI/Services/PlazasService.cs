using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class PlazasService
    {
        private readonly UstoreContext context;
        private IMapper mapper;
        public PlazasService(UstoreContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        public async Task<IEnumerable<CentroComercialDto>> GetMalls()
        {
            var malls = mapper.Map<IEnumerable<CentroComercialDto>>(await context.CentroComercials.AsNoTracking().ToListAsync());
            return malls;
        }

        public async Task<CentroComercial?> GetOneMall(int? id)
        {
            return await context.CentroComercials.FindAsync(id);
        }

        public async Task<IEnumerable<ListaProductosAppDto>> GetProductosPopulares(int idMall)
        {
            List<ListaProductosAppDto> productosPopulares = new List<ListaProductosAppDto>();
            var productosPopularesPorTienda = await context.CentroComercials
                                                            .Where(mall => mall.IdCentroComercial == idMall)
                                                            .SelectMany(mall => mall.Tienda)
                                                            .SelectMany(tienda => tienda.SolicitudesApartados)
                                                            .GroupBy(solicitud => new
                                                            {
                                                                solicitud.IdTienda,
                                                                solicitud.IdProductos
                                                            })
                                                            .Select(group => new
                                                            {
                                                                TiendaId = group.Key.IdTienda,
                                                                ProductoId = group.Key.IdProductos,
                                                                CantidadSolicitudes = group.Select(x => x.IdUsuario).Distinct().Count()
                                                            })
                                                            .ToListAsync();
            var listaProductosPopulares = productosPopularesPorTienda
                                            .GroupBy(p => p.TiendaId)
                                            .Select(p => p.OrderByDescending(p => p.CantidadSolicitudes).FirstOrDefault())
                                            .Where(p => p != null)
                                            .Select(p => new
                                            {
                                                IdProductos = (int)p.ProductoId!,
                                                IdTienda = p.TiendaId,
                                                CantidadSolicitudes = p.CantidadSolicitudes
                                            })
                                            .OrderByDescending(p => p.CantidadSolicitudes).ToList();
            foreach (var productoPopular in listaProductosPopulares)
            {
                var producto = mapper.Map<ListaProductosAppDto>(await context.Productos.FindAsync(productoPopular.IdProductos));
                producto.ImageProducto = await context.ImagenesProductos.Where(p => p.IdProductos == producto.IdProductos).Select(p => p.ImagenProductoThumbNail).FirstOrDefaultAsync();
                var tiendaProducto = await context.Tienda.FindAsync(producto.IdTienda);
                producto.NumeroSolicitudes = productoPopular.CantidadSolicitudes;
                producto.NombreTienda = tiendaProducto!.NombreTienda;
                producto.IconoTienda = tiendaProducto!.LogoTienda;
                productosPopulares.Add(producto);
            }

            return productosPopulares;
        }

        public async Task<CentroComercial> CreateMall(CentroComercial mall)
        {
            await context.CentroComercials.AddAsync(mall);
            await context.SaveChangesAsync();

            return mall;
        }

        public async Task UpdateMall(CentroComercial mall)
        {
            context.CentroComercials.Update(mall);
            await context.SaveChangesAsync();
        }

        public async Task DeleteMall(CentroComercial mall)
        {
            context.CentroComercials.Remove(mall);
            await context.SaveChangesAsync();
        }
    }
}
