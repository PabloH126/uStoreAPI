using AutoMapper;
using Microsoft.EntityFrameworkCore;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class ComentariosService
    {
        private readonly UstoreContext context;
        private IMapper mapper;
        public ComentariosService(UstoreContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }
        public async Task<IEnumerable<ComentariosTiendaDto>> GetAllComentariosTienda(int idTienda)
        {
            var comentarios = await (from comentario in context.ComentariosTiendas
                                     join usuario in context.Usuarios on comentario.IdUsuario equals usuario.IdUsuario
                                     join cuentaU in context.CuentaUsuarios on usuario.IdUsuario equals cuentaU.IdUsuario
                                     join detallesCuentaU in context.DetallesCuentaUsuarios on cuentaU.IdDetallesCuentaUsuario equals detallesCuentaU.IdDetallesCuentaUsuario
                                     join iP in context.ImagenPerfils on detallesCuentaU.IdImagenPerfil equals iP.IdImagenPerfil
                                     join detallesU in context.DetallesUsuarios on usuario.IdDetallesUsuario equals detallesU.IdDetallesUsuario
                                     join datosU in context.Datos on detallesU.IdDatos equals datosU.IdDatos
                                     where comentario.IdTienda == idTienda
                                     orderby comentario.FechaComentario descending
                                     select new ComentariosTiendaDto
                                     {
                                         IdComentarioTienda = comentario.IdComentarioTienda,
                                         Comentario = comentario.Comentario,
                                         FechaComentario = comentario.FechaComentario,
                                         IdUsuario = comentario.IdUsuario,
                                         IdTienda = comentario.IdTienda,
                                         ImagenUsuario = iP.IconoPerfil,
                                         NombreUsuario = $"{datosU.PrimerNombre} {datosU.PrimerApellido}"
                                     }).OrderByDescending(p => p.FechaComentario).AsNoTracking().ToListAsync();
            foreach(var comentario in comentarios)
            {
                comentario.CalificacionEstrellas = await context.CalificacionTienda.Where(p => p.IdTienda == comentario.IdTienda && p.IdUsuario == comentario.IdUsuario).Select(p => p.Calificacion).FirstOrDefaultAsync();
            }
            return comentarios;
        }

        public async Task<IEnumerable<ComentariosProductoDto>> GetAllComentariosProducto(int idProducto)
        {
            var comentarios = await (from comentario in context.ComentariosProductos
                                     join usuario in context.Usuarios on comentario.IdUsuario equals usuario.IdUsuario
                                     join cuentaU in context.CuentaUsuarios on usuario.IdUsuario equals cuentaU.IdUsuario
                                     join detallesCuentaU in context.DetallesCuentaUsuarios on cuentaU.IdDetallesCuentaUsuario equals detallesCuentaU.IdDetallesCuentaUsuario
                                     join iP in context.ImagenPerfils on detallesCuentaU.IdImagenPerfil equals iP.IdImagenPerfil
                                     join detallesU in context.DetallesUsuarios on usuario.IdDetallesUsuario equals detallesU.IdDetallesUsuario
                                     join datosU in context.Datos on detallesU.IdDatos equals datosU.IdDatos
                                     where comentario.IdProducto == idProducto
                                     select new ComentariosProductoDto
                                     {
                                         IdComentarioProducto = comentario.IdComentarioProducto,
                                         Comentario = comentario.Comentario,
                                         FechaComentario = comentario.FechaComentario,
                                         IdUsuario = comentario.IdUsuario,
                                         IdProducto = comentario.IdProducto,
                                         ImagenUsuario = iP.IconoPerfil,
                                         NombreUsuario = $"{datosU.PrimerNombre} {datosU.PrimerApellido}"
                                     }).OrderByDescending(p => p.FechaComentario).AsNoTracking().ToListAsync();
            foreach (var comentario in comentarios)
            {
                comentario.CalificacionEstrellas = await context.CalificacionProductos.Where(p => p.IdProductos == comentario.IdProducto && p.IdUsuario == comentario.IdUsuario).Select(p => p.Calificacion).FirstOrDefaultAsync();
            }
            return comentarios;
        }

        public async Task<ComentariosTienda> CreateComentarioTienda(ComentariosTienda comentario)
        {
            await context.ComentariosTiendas.AddAsync(comentario);
            await context.SaveChangesAsync();
            return comentario;
        }

        public async Task<ComentariosProducto> CreateComentarioProducto(ComentariosProducto comentario)
        {
            await context.ComentariosProductos.AddAsync(comentario);
            await context.SaveChangesAsync();
            return comentario;
        }

        public async Task DeleteAllComentariosTienda(int idTienda)
        {
            var comentarios = await context.ComentariosTiendas.Where(p => p.IdTienda == idTienda).ToListAsync();
            context.ComentariosTiendas.RemoveRange(comentarios);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAllComentariosProducto(int idProducto)
        {
            var comentarios = await context.ComentariosProductos.Where(p => p.IdProducto == idProducto).ToListAsync();
            context.ComentariosProductos.RemoveRange(comentarios);
            await context.SaveChangesAsync();
        }
    }
}
