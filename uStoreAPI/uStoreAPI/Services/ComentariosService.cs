using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IEnumerable<ComentariosTienda>> GetAllComentariosTienda(int idTienda)
        {
            var comentarios = await context.ComentariosTiendas.Where(c => c.IdTienda == idTienda).AsNoTracking().ToListAsync();
            return comentarios;
        }

        public async Task<IEnumerable<ComentariosProducto>> GetAllComentariosProducto(int idProducto)
        {
            var comentarios = await context.ComentariosProductos.Where(c => c.IdProducto == idProducto).AsNoTracking().ToListAsync();
            return comentarios;
        }

        public async Task<ComentariosTienda?> GetComentarioTienda(int idComentarioTienda)
        {
            var comentario = await context.ComentariosTiendas.FindAsync(idComentarioTienda);
            return comentario;
        }

        public async Task<ComentariosProducto?> GetComentarioProducto(int idComentarioProducto)
        {
            var comentario = await context.ComentariosProductos.FindAsync(idComentarioProducto);
            return comentario;
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

        public async Task UpdateComentarioTienda(ComentariosTienda newComentario)
        {
            var comentario = await context.ComentariosTiendas.FindAsync(newComentario.IdComentarioTienda);
            comentario.Comentario = newComentario.Comentario;
            context.ComentariosTiendas.Update(comentario);
            await context.SaveChangesAsync();
        }

        public async Task UpdateComentarioProducto(ComentariosProducto newComentario)
        {
            var comentario = await context.ComentariosProductos.FindAsync(newComentario.IdComentarioProducto);
            comentario.Comentario = newComentario.Comentario;
            context.ComentariosProductos.Update(comentario);
            await context.SaveChangesAsync();
        }

        public async Task DeleteComentarioTienda(int idComentario)
        {
            var comentario = await context.ComentariosTiendas.FindAsync(idComentario);
            context.ComentariosTiendas.Remove(comentario);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAllComentariosTienda(int idTienda)
        {
            var comentarios = await context.ComentariosTiendas.Where(p => p.IdTienda == idTienda).ToListAsync();
            context.ComentariosTiendas.RemoveRange(comentarios);
            await context.SaveChangesAsync();
        }

        public async Task DeleteComentarioProducto(int idComentario)
        {
            var comentario = await context.ComentariosProductos.FindAsync(idComentario);
            context.ComentariosProductos.Remove(comentario);
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
