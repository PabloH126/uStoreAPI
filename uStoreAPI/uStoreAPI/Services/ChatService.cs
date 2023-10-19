using Humanizer.Localisation.DateToOrdinalWords;
using Microsoft.EntityFrameworkCore;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class ChatService
    {
        private readonly UstoreContext context;
        public ChatService(UstoreContext _context)
        {
            context = _context;
        }


        public async Task<Chat?> GetChat(int id)
        {
            return await context.Chats.FindAsync(id);
        }

        public async Task<Chat?> GetChat(int idDestinatario, int idRemitente)
        {
            return await context.Chats.FirstOrDefaultAsync(p => p.IdMiembro1 == idRemitente && p.IdMiembro2 == idDestinatario);
        }

        public async Task<IEnumerable<ChatDto?>> GetChats(int idSolicitante, string typeUsuarioChat)
        {
            var mensajesRecientes = context.Mensajes
                                            .GroupBy(m => m.IdChat)
                                            .Select(g => new 
                                                         { 
                                                           IdChat = g.Key, 
                                                           FechaMensaje = g.Max(m => m.FechaMensaje), 
                                                           contenidoMensaje = g.First(m => m.FechaMensaje == g.Max(m => m.FechaMensaje)).Contenido
                                                         });

            var chats = await (from chat in context.Chats
                               join mensajeReciente in mensajesRecientes on chat.IdChat equals mensajeReciente.IdChat
                               where (chat.IdMiembro1 == idSolicitante || chat.IdMiembro2 == idSolicitante)
                                     && (chat.TypeMiembro1 == typeUsuarioChat || chat.TypeMiembro2 == typeUsuarioChat)
                               orderby mensajeReciente.FechaMensaje descending
                               select new ChatDto
                               {
                                   IdChat = chat.IdChat,
                                   FechaCreacion = chat.FechaCreacion,
                                   IdMiembro1 = chat.IdMiembro1,
                                   IdMiembro2 = chat.IdMiembro2,
                                   TypeMiembro1 = chat.TypeMiembro1,
                                   TypeMiembro2 = chat.TypeMiembro2,
                                   UltimoMensaje = mensajeReciente.contenidoMensaje
                               }
                  ).ToListAsync();

            return chats;

        }
        public async Task<IEnumerable<Chat?>> GetChatsGerente(int idSolicitante)
        {
            return await context.Chats
                                .Where(p => (p.IdMiembro1 == idSolicitante || p.IdMiembro2 == idSolicitante)
                                         && (p.TypeMiembro1 == "Gerente" || p.TypeMiembro2 == "Gerente"))
                                .AsNoTracking()
                                .ToListAsync();
        }

        public async Task<Chat?> GetChatAdministrador(int idSolicitante)
        {
            return await context.Chats.FirstOrDefaultAsync(p => (p.IdMiembro1 == idSolicitante || p.IdMiembro2 == idSolicitante)
                                                       && (p.TypeMiembro1 == "Administrador" || p.TypeMiembro2 == "Administrador"));
        }

        public async Task<Mensaje?> GetMensaje(int id)
        {
            return await context.Mensajes.FindAsync(id);
        }

        public async Task<Mensaje?> GetUltimoMensaje(int idChat)
        {
            return await context.Mensajes.FirstOrDefaultAsync(p => p.IdChat == idChat);
        }

        public async Task<IEnumerable<Mensaje>> GetMensajesChat(int idChat)
        {
            return await context.Mensajes.Where(p => p.IdChat == idChat).AsNoTracking().ToListAsync();
        }

        public async Task<Chat> CreateChat(Chat newChat)
        {
            await context.Chats.AddAsync(newChat);
            await context.SaveChangesAsync();
            return newChat;
        }

        public async Task<Mensaje> CreateMensaje(Mensaje newMensaje)
        {
            await context.Mensajes.AddAsync(newMensaje);
            await context.SaveChangesAsync();
            return newMensaje;
        }

        public async Task UpdateMensaje(Mensaje mensaje)
        {
            context.Mensajes.Update(mensaje);
            await context.SaveChangesAsync();
        }

        public async Task DeleteChat(Chat chat)
        {
            var mensajesChat = await GetMensajesChat(chat.IdChat);
            context.Mensajes.RemoveRange(mensajesChat);
            context.Chats.Remove(chat);
            await context.SaveChangesAsync();
        }
    }
}
