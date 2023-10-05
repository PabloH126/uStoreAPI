using Microsoft.EntityFrameworkCore;
using System.Linq;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class TendenciasService
    {
        private readonly UstoreContext context;
        public TendenciasService(UstoreContext _context)
        {
            context = _context;
        }

        public async Task<IEnumerable<TendenciaDto>> GetTendencias(filtrosGraficaDto filtros)
        {
            List<TendenciaDto> tendencias = new List<TendenciaDto>();
            DateTime periodoTiempo;
            
            switch(filtros.periodoTiempo)
            {
                case "quincenal":
                    periodoTiempo = DateTime.UtcNow.AddDays(-15);
                    break;
                case "mensual":
                    periodoTiempo = DateTime.UtcNow.AddMonths(-1);
                    break;
                case "semanal":
                    periodoTiempo = DateTime.UtcNow.AddDays(-7);
                    break;
                default : throw new ArgumentException("No se especifico ningun periodo");
            }

            IQueryable<int> productosFiltrados = context.Productos.Select(p => p.IdProductos);
            IQueryable<int> tiendasFiltradas = context.Tienda.Select(t => t.IdTienda);

            if (filtros.categorias != null && filtros.categorias.Any())
            {
                productosFiltrados = from p in context.Productos
                                     join cp in context.CategoriasProductos on p.IdProductos equals cp.IdProductos
                                     where filtros.categorias.Contains((int)cp.IdCategoria!)
                                     select p.IdProductos;

                tiendasFiltradas = from t in context.Tienda
                                   join ct in context.CategoriasTiendas on t.IdTienda equals ct.IdTienda
                                   where filtros.categorias.Contains((int)ct.IdCategoria!)
                                   select t.IdTienda;
            }

            var solicitudesEnPeriodo = context.SolicitudesApartados.Where(p => p.FechaSolicitud >= periodoTiempo);

            if (filtros.isTienda)
            {
                solicitudesEnPeriodo = solicitudesEnPeriodo.Where(p => tiendasFiltradas.ToList().Contains((int)p.IdTienda!));
            }
            else
            {
                solicitudesEnPeriodo = solicitudesEnPeriodo.Where(p => productosFiltrados.ToList().Contains((int)p.IdProductos!));
            }

            var usuariosEnSolicitudes = await solicitudesEnPeriodo
                                                .GroupBy(p => filtros.isTienda ? p.IdTienda : p.IdProductos)
                                                .Select(g => new
                                                {
                                                    Id = g.Key,
                                                    CantidadUsuarios = g.Select(s => s.IdUsuario).Distinct().Count() 
                                                })
                                                .OrderByDescending(p => p.CantidadUsuarios)
                                                .Take(50)
                                                .ToListAsync();

            foreach (var item in usuariosEnSolicitudes)
            {
                string nombre = "";
                if (filtros.isTienda)
                {
                    var tienda = await context.Tienda.FindAsync(item.Id);
                    if (tienda != null)
                    {
                        nombre = tienda.NombreTienda!;
                    }
                }
                else
                {
                    var productoData = await context.Productos.FindAsync(item.Id);
                    if (productoData != null)
                    {
                        nombre = productoData.NombreProducto!;
                    }
                }

                tendencias.Add(new TendenciaDto
                {
                    Nombre = nombre,
                    NumeroSolicitudes = item.CantidadUsuarios
                });
            }

            return tendencias;
        }

        public async Task<IEnumerable<TendenciaDto>> GetTendenciasAdmin(filtrosGraficaDto filtros, int idAdmin, int? idTienda)
        {
            List<TendenciaDto> tendencias = new List<TendenciaDto>();
            DateTime periodoTiempo;

            switch (filtros.periodoTiempo)
            {
                case "quincenal":
                    periodoTiempo = DateTime.UtcNow.AddDays(-15);
                    break;
                case "mensual":
                    periodoTiempo = DateTime.UtcNow.AddMonths(-1);
                    break;
                case "semanal":
                    periodoTiempo = DateTime.UtcNow.AddDays(-7);
                    break;
                default: throw new ArgumentException("No se especifico ningun periodo");
            }

            IQueryable<int> productosFiltrados = context.Productos.Where(p => p.IdTienda == idTienda).Select(p => p.IdProductos);
                                                 
            IQueryable<int> tiendasFiltradas = context.Tienda.Where(p => p.IdAdministrador == idAdmin).Select(t => t.IdTienda);

            if (filtros.categorias != null && filtros.categorias.Any())
            {
                productosFiltrados = from p in context.Productos
                                     join t in context.Tienda on p.IdTienda equals t.IdTienda
                                     join cp in context.CategoriasProductos on p.IdProductos equals cp.IdProductos
                                     where t.IdAdministrador == idAdmin && filtros.categorias.Contains((int)cp.IdCategoria!)
                                     select p.IdProductos;

                tiendasFiltradas = from t in context.Tienda
                                   join ct in context.CategoriasTiendas on t.IdTienda equals ct.IdTienda
                                   where t.IdAdministrador == idAdmin && filtros.categorias.Contains((int)ct.IdCategoria!)
                                   select t.IdTienda;
            }

            var solicitudesEnPeriodo = context.SolicitudesApartados.Where(p => p.FechaSolicitud >= periodoTiempo);

            if (filtros.isTienda)
            {
                solicitudesEnPeriodo = solicitudesEnPeriodo.Where(p => tiendasFiltradas.ToList().Contains((int)p.IdTienda!));
            }
            else
            {
                solicitudesEnPeriodo = solicitudesEnPeriodo.Where(p => productosFiltrados.ToList().Contains((int)p.IdProductos!));
            }

            var usuariosEnSolicitudes = await solicitudesEnPeriodo
                                                .GroupBy(p => filtros.isTienda ? p.IdTienda : p.IdProductos)
                                                .Select(g => new
                                                {
                                                    Id = g.Key,
                                                    CantidadUsuarios = g.Select(s => s.IdUsuario).Distinct().Count()
                                                })
                                                .OrderByDescending(p => p.CantidadUsuarios)
                                                .Take(50)
                                                .ToListAsync();

            foreach (var item in usuariosEnSolicitudes)
            {
                string nombre = "";
                if (filtros.isTienda)
                {
                    var tienda = await context.Tienda.FindAsync(item.Id);
                    if (tienda != null)
                    {
                        nombre = tienda.NombreTienda!;
                    }
                }
                else
                {
                    var productoData = await context.Productos.FindAsync(item.Id);
                    if (productoData != null)
                    {
                        nombre = productoData.NombreProducto!;
                    }
                }

                tendencias.Add(new TendenciaDto
                {
                    Nombre = nombre,
                    NumeroSolicitudes = item.CantidadUsuarios
                });
            }

            return tendencias;
        }
    }
}
