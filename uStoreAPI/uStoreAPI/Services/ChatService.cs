using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public async Task<Chat?> GetChatWithIdDestinatario(int idDestinatario, int idRemitente)
        {
            return await context.Chats.FirstOrDefaultAsync(p => p.IdMiembro1 == idRemitente && p.IdMiembro2 == idDestinatario);
        }
        public async Task<IEnumerable<ChatDto?>> GetChatsUsuarios(int idSolicitante, string typeUsuarioSolicitante)
        {
            var mensajesRecientes = context.Mensajes
                                            .GroupBy(m => m.IdChat)
                                            .Select(g => new
                                            {
                                                IdChat = g.Key,
                                                FechaMensaje = g.Max(m => m.FechaMensaje),
                                                contenidoMensaje = g.First(m => m.FechaMensaje == g.Max(m => m.FechaMensaje)).Contenido
                                            });

            if (typeUsuarioSolicitante == "Gerente")
            {
                var tiendaGerente = await (from tienda in context.Tienda
                                           join gerente in context.Gerentes on tienda.IdTienda equals gerente.IdTienda
                                           where gerente.IdGerente == idSolicitante
                                           select tienda.IdTienda).FirstOrDefaultAsync();
                var chats = await (from chat in context.Chats
                                   join mensajeReciente in mensajesRecientes on chat.IdChat equals mensajeReciente.IdChat
                                   join t in context.Tienda on tiendaGerente equals t.IdTienda
                                   where (

                                            (chat.IdTienda == tiendaGerente && chat.TypeMiembro2 == "Usuario") ||
                                            (chat.IdTienda == tiendaGerente && chat.TypeMiembro1 == "Usuario")

                                         )
                                   orderby mensajeReciente.FechaMensaje descending
                                   select new ChatDto
                                   {
                                       IdChat = chat.IdChat,
                                       FechaCreacion = chat.FechaCreacion,
                                       IdMiembro1 = chat.IdMiembro1,
                                       IdMiembro2 = chat.IdMiembro2,
                                       TypeMiembro1 = chat.TypeMiembro1,
                                       TypeMiembro2 = chat.TypeMiembro2,
                                       UltimoMensaje = mensajeReciente.contenidoMensaje,
                                       ImagenTienda = t.LogoTienda,
                                       TiendaNameChat = t.NombreTienda,
                                       IdTienda = t.IdTienda
                                   }
                 ).ToListAsync();

                foreach (var chat in chats)
                {
                    var id = (chat.TypeMiembro2 == "Usuario") ? chat.IdMiembro2 : chat.IdMiembro1;
                    var usuario = await (from cU in context.CuentaUsuarios
                                         join dCU in context.DetallesCuentaUsuarios on cU.IdDetallesCuentaUsuario equals dCU.IdDetallesCuentaUsuario
                                         join iP in context.ImagenPerfils on dCU.IdImagenPerfil equals iP.IdImagenPerfil
                                         join u in context.Usuarios on cU.IdUsuario equals u.IdUsuario
                                         join dU in context.DetallesUsuarios on u.IdDetallesUsuario equals dU.IdDetallesUsuario
                                         join datosU in context.Datos on dU.IdDatos equals datosU.IdDatos
                                         where (u.IdUsuario == id)
                                         select new
                                         {
                                             ImagenUsuario = iP.IconoPerfil,
                                             NombreUsuario = $"{datosU.PrimerNombre} {datosU.PrimerApellido}"
                                         }).FirstOrDefaultAsync();
                    chat.ImagenUsuario = usuario!.ImagenUsuario;
                    chat.NombreUsuario = usuario!.NombreUsuario;
                }

                return chats;
            }
            else if (typeUsuarioSolicitante == "Usuario")
            {
                var chats = await (from chat in context.Chats
                                   join mR in mensajesRecientes on chat.IdChat equals mR.IdChat
                                   join t in context.Tienda on chat.IdTienda equals t.IdTienda
                                   where (
                                                (chat.IdMiembro1 == idSolicitante && chat.TypeMiembro1 == "Usuario") ||
                                                (chat.IdMiembro2 == idSolicitante && chat.TypeMiembro2 == "Usuario")
                                         )
                                   orderby mR.FechaMensaje descending
                                   select new ChatDto
                                   {
                                       IdChat = chat.IdChat,
                                       FechaCreacion = chat.FechaCreacion,
                                       IdMiembro1 = chat.IdMiembro1,
                                       IdMiembro2 = chat.IdMiembro2,
                                       TypeMiembro1 = chat.TypeMiembro1,
                                       TypeMiembro2 = chat.TypeMiembro2,
                                       UltimoMensaje = mR.contenidoMensaje,
                                       ImagenTienda = t.LogoTienda,
                                       TiendaNameChat = t.NombreTienda,
                                       IdTienda = t.IdTienda
                                   }
                                   ).ToListAsync();

                foreach (var chat in chats)
                {
                    var usuario = await (from cU in context.CuentaUsuarios
                                         join dCU in context.DetallesCuentaUsuarios on cU.IdDetallesCuentaUsuario equals dCU.IdDetallesCuentaUsuario
                                         join iP in context.ImagenPerfils on dCU.IdImagenPerfil equals iP.IdImagenPerfil
                                         join u in context.Usuarios on cU.IdUsuario equals u.IdUsuario
                                         join dU in context.DetallesUsuarios on u.IdDetallesUsuario equals dU.IdDetallesUsuario
                                         join datosU in context.Datos on dU.IdDatos equals datosU.IdDatos
                                         where (u.IdUsuario == idSolicitante)
                                         select new
                                         {
                                             ImagenUsuario = iP.IconoPerfil,
                                             NombreUsuario = $"{datosU.PrimerNombre} {datosU.PrimerApellido}"
                                         }).FirstOrDefaultAsync();
                    chat.ImagenUsuario = usuario!.ImagenUsuario;
                    chat.NombreUsuario = usuario!.NombreUsuario;
                }

                return chats;
            }
            else
            {
                var chats = await (from chat in context.Chats
                                   join mR in mensajesRecientes on chat.IdChat equals mR.IdChat
                                   join t in context.Tienda on chat.IdTienda equals t.IdTienda
                                   where (
                                                (t.IdAdministrador == idSolicitante && chat.TypeMiembro1 == "Usuario") ||
                                                (t.IdAdministrador == idSolicitante && chat.TypeMiembro2 == "Usuario")
                                         )
                                   orderby mR.FechaMensaje descending
                                   select new ChatDto
                                   {
                                       IdChat = chat.IdChat,
                                       FechaCreacion = chat.FechaCreacion,
                                       IdMiembro1 = chat.IdMiembro1,
                                       IdMiembro2 = chat.IdMiembro2,
                                       TypeMiembro1 = chat.TypeMiembro1,
                                       TypeMiembro2 = chat.TypeMiembro2,
                                       UltimoMensaje = mR.contenidoMensaje,
                                       ImagenTienda = t.LogoTienda,
                                       TiendaNameChat = t.NombreTienda,
                                       IdTienda = t.IdTienda
                                   }
                                   ).ToListAsync();

                foreach (var chat in chats)
                {
                    var id = (chat.TypeMiembro2 == "Usuario") ? chat.IdMiembro2 : chat.IdMiembro1;
                    var usuario = await (from cU in context.CuentaUsuarios
                                         join dCU in context.DetallesCuentaUsuarios on cU.IdDetallesCuentaUsuario equals dCU.IdDetallesCuentaUsuario
                                         join iP in context.ImagenPerfils on dCU.IdImagenPerfil equals iP.IdImagenPerfil
                                         join u in context.Usuarios on cU.IdUsuario equals u.IdUsuario
                                         join dU in context.DetallesUsuarios on u.IdDetallesUsuario equals dU.IdDetallesUsuario
                                         join datosU in context.Datos on dU.IdDatos equals datosU.IdDatos
                                         where (u.IdUsuario == id)
                                         select new
                                         {
                                             ImagenUsuario = iP.IconoPerfil,
                                             NombreUsuario = $"{datosU.PrimerNombre} {datosU.PrimerApellido}"
                                         }).FirstOrDefaultAsync();
                    chat.ImagenUsuario = usuario!.ImagenUsuario;
                    chat.NombreUsuario = usuario!.NombreUsuario;
                }

                return chats;
            }
        }

        public async Task<IEnumerable<ChatDto?>> GetChatsGerentes(int idSolicitante)
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
                               join mR in mensajesRecientes on chat.IdChat equals mR.IdChat
                               join t in context.Tienda on chat.IdTienda equals t.IdTienda
                               where (
                                            (t.IdAdministrador == idSolicitante && chat.TypeMiembro1 == "Gerente") ||
                                            (t.IdAdministrador == idSolicitante && chat.TypeMiembro2 == "Gerente")
                                     )
                               orderby mR.FechaMensaje descending
                               select new ChatDto
                               {
                                   IdChat = chat.IdChat,
                                   FechaCreacion = chat.FechaCreacion,
                                   IdMiembro1 = chat.IdMiembro1,
                                   IdMiembro2 = chat.IdMiembro2,
                                   TypeMiembro1 = chat.TypeMiembro1,
                                   TypeMiembro2 = chat.TypeMiembro2,
                                   UltimoMensaje = mR.contenidoMensaje,
                                   ImagenTienda = t.LogoTienda,
                                   TiendaNameChat = t.NombreTienda
                               }
                                   ).ToListAsync();

            foreach (var chat in chats)
            {
                var id = (chat.TypeMiembro2 == "Gerente") ? chat.IdMiembro2 : chat.IdMiembro1;
                var usuario = await (from cG in context.CuentaGerentes
                                     join dCG in context.DetallesCuentaGerentes on cG.IdDetallesCuentaGerente equals dCG.IdDetallesCuentaGerente
                                     join iP in context.ImagenPerfils on dCG.IdImagenPerfil equals iP.IdImagenPerfil
                                     join g in context.Gerentes on cG.IdGerente equals g.IdGerente
                                     join datosG in context.Datos on g.IdDatos equals datosG.IdDatos
                                     where (g.IdGerente == id)
                                     select new
                                     {
                                         idTienda = g.IdTienda,
                                         ImagenUsuario = iP.IconoPerfil,
                                         NombreUsuario = $"{datosG.PrimerNombre} {datosG.PrimerApellido}"
                                     }).FirstOrDefaultAsync();
                chat.ImagenUsuario = usuario!.ImagenUsuario;
                chat.NombreUsuario = usuario!.NombreUsuario;
            }
            return chats;
        }

        public async Task<Chat?> GetChatAdministrador(int idSolicitante)
        {
            var gerente = await context.Gerentes.FindAsync(idSolicitante);
            if(gerente is not null)
            {
                var chatAdmin = await context.Chats.FirstOrDefaultAsync(p => (
                                                          (p.IdMiembro1 == idSolicitante && p.TypeMiembro1 == "Gerente" && p.IdMiembro2 == gerente.IdAdministrador && p.TypeMiembro2 == "Administrador") ||
                                                          (p.IdMiembro2 == idSolicitante && p.TypeMiembro2 == "Gerente" && p.IdMiembro1 == gerente.IdAdministrador && p.TypeMiembro1 == "Administrador")
                                                          ));
                return chatAdmin;
            }
            return null;
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

        public async Task<ChatDto?> CreateChat(int idSolicitante, string typeSolicitante, int idMiembro2, string typeMiembro2, int? idTienda)
        {
            //newChat será el chat que se guardará en la base de datos
            Chat newChat = new Chat();

            //chatCreadoDto será el chat que se guardó de newChat pero con los datos del usuario para respuesta de signalR
            ChatDto? chatCreadoDto = new ChatDto();
            if (typeSolicitante == "Usuario")
            {
                var tienda = await context.Tienda.FindAsync(idTienda);
                if (tienda is not null)
                {
                    newChat.FechaCreacion = DateTime.UtcNow;
                    newChat.IdMiembro1 = tienda.IdTienda;
                    newChat.TypeMiembro1 = "Tienda";
                    newChat.IdMiembro2 = idSolicitante;
                    newChat.TypeMiembro2 = typeSolicitante;
                    newChat.IdTienda = tienda.IdTienda;

                    chatCreadoDto.FechaCreacion = newChat.FechaCreacion;
                    chatCreadoDto.IdMiembro1 = newChat.IdMiembro1;
                    chatCreadoDto.TypeMiembro1 = newChat.TypeMiembro1;
                    chatCreadoDto.IdMiembro2 = newChat.IdMiembro2;
                    chatCreadoDto.TypeMiembro2 = newChat.TypeMiembro2;
                    chatCreadoDto.IdTienda = newChat.IdTienda;

                    var usuario = await (from cU in context.CuentaUsuarios
                                            join dCU in context.DetallesCuentaUsuarios on cU.IdDetallesCuentaUsuario equals dCU.IdDetallesCuentaUsuario
                                            join iP in context.ImagenPerfils on dCU.IdImagenPerfil equals iP.IdImagenPerfil
                                            join u in context.Usuarios on cU.IdUsuario equals u.IdUsuario
                                            join dU in context.DetallesUsuarios on u.IdDetallesUsuario equals dU.IdDetallesUsuario
                                            join datosU in context.Datos on dU.IdDatos equals datosU.IdDatos
                                            where (u.IdUsuario == idSolicitante)
                                            select new 
                                            {
                                                NombreUsuario = $"{datosU.PrimerNombre} {datosU.PrimerApellido}",
                                                ImagenUsuario = iP.IconoPerfil
                                            }).FirstOrDefaultAsync();

                    chatCreadoDto.NombreUsuario = usuario!.NombreUsuario;
                    chatCreadoDto.ImagenUsuario = usuario!.ImagenUsuario;
                }
            }
            else 
            {
                var gerente = new Gerente();
                if (typeSolicitante == "Gerente" && typeMiembro2 == "Administrador")
                {
                    gerente = await context.Gerentes.FindAsync(idSolicitante);
                }
                else
                {
                    gerente = await context.Gerentes.FindAsync(idMiembro2);
                }
                    
                newChat.FechaCreacion = DateTime.UtcNow;
                newChat.IdMiembro1 = idSolicitante;
                newChat.TypeMiembro1 = typeSolicitante;
                newChat.IdMiembro2 = idMiembro2;
                newChat.TypeMiembro2 = typeMiembro2;
                newChat.IdTienda = gerente!.IdTienda;

                chatCreadoDto.FechaCreacion = newChat.FechaCreacion;
                chatCreadoDto.IdMiembro1 = newChat.IdMiembro1;
                chatCreadoDto.TypeMiembro1 = newChat.TypeMiembro1;
                chatCreadoDto.IdMiembro2 = newChat.IdMiembro2;
                chatCreadoDto.TypeMiembro2 = newChat.TypeMiembro2;
                chatCreadoDto.IdTienda = newChat.IdTienda;
            }

            await context.Chats.AddAsync(newChat);
            await context.SaveChangesAsync();

            chatCreadoDto.IdChat = newChat.IdChat;
            return chatCreadoDto!;
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

        public async Task DeleteChatWithIdTienda(int idTienda)
        {
            var chats = await context.Chats.Where(p => p.IdTienda == idTienda).ToListAsync();
            if (!chats.IsNullOrEmpty())
            {
                foreach (var chat in chats)
                {
                    var mensajesChat = await GetMensajesChat(chat.IdChat);
                    context.Mensajes.RemoveRange(mensajesChat);
                }
                context.Chats.RemoveRange(chats);
                await context.SaveChangesAsync();
            }
        }
    }
}
