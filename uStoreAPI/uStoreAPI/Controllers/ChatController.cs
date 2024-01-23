using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.Hubs;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatService chatService;
        private readonly UploadService uploadService;
        private readonly TiendasService tiendasService;
        private readonly GerentesService gerentesService;
        private readonly IHubContext<ChatHub> hubContext;
        private IMapper mapper;
        public ChatController(IMapper _mapper, ChatService _chatService, IHubContext<ChatHub> _hubContext, UploadService _uploadService, TiendasService _tiendasService, GerentesService _gerentesService)
        {
            chatService = _chatService;
            mapper = _mapper;
            hubContext = _hubContext;
            uploadService = _uploadService;
            tiendasService = _tiendasService;
            gerentesService = _gerentesService;
        }
        
        //Get chats de usuario por medio de Id
        [HttpGet("GetChats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ChatDto?>>> GetChats(string typeChat)
        {
            if(typeChat is null)
            {
                return BadRequest("Se debe ingresar un tipo de Chat");
            }
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            if (typeUser == "Administrador" && typeChat == "Gerente")
            {
                return Ok(await chatService.GetChatsGerentes(idUser));
            }
            else if(typeUser == typeChat)
            {
                return Conflict("El tipo de solicitante y el tipo de chat son los mismos");
            }
            else if (typeUser == "Gerente" && typeChat == "Administrador")
            {
                var chatAdminDto = mapper.Map<ChatDto>(await chatService.GetChatAdministrador(idUser));
                return Ok(chatAdminDto);
            }
            else
            {
                return Ok(await chatService.GetChatsUsuarios(idUser, typeUser));
            }
        }
        

        [HttpGet("GetChat")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MensajeDto?>>> GetChat(int idChat)
        {
            if (idChat == 0)
            {
                return Ok();
            }
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            var mensajesChat = await chatService.GetMensajesChat(idChat);
            var chat = mapper.Map<IEnumerable<MensajeDto>>(mensajesChat);
            foreach(var mensaje in chat)
            {
                if (mensaje.IdRemitente != idUser)
                {
                    mensaje.isRecieved = true;
                }
                else
                {
                    mensaje.isRecieved = false;
                }
            }
            return Ok(chat);
        }
        
        [HttpPost("CreateChat")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ChatDto?>> CreateChat(IFormFile? image, [FromForm] MensajeDto newMensaje, int idMiembro2, string typeMiembro2, int? idTienda)
        {
            if (idMiembro2 == 0)
            {
                return BadRequest();
            }
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            if (typeUser == "Usuario" && idTienda is null)
            {
                return BadRequest("Es necesario agregar un id de la tienda donde se hace la solicitud");
            }
            try
            {
                var chatGuardado = await chatService.CreateChat(idUser, typeUser, idMiembro2, typeMiembro2, idTienda);

                var tiendaChat = await tiendasService.GetOneTienda(chatGuardado!.IdTienda);
                var mensaje = mapper.Map<Mensaje>(newMensaje);
                mensaje.IdChat = chatGuardado.IdChat;
                mensaje.FechaMensaje = DateTime.UtcNow;
                
                if (chatGuardado.TypeMiembro1 == "Usuario" || chatGuardado.TypeMiembro2 == "Usuario")
                {
                    if (typeUser == "Gerente" || typeUser == "Administrador")
                    {
                        mensaje.IdRemitente = tiendaChat!.IdTienda;
                        mensaje.TypeRemitente = "Tienda";
                    }
                    else
                    {
                        mensaje.IdRemitente = idUser;
                        mensaje.TypeRemitente = typeUser;
                    }
                }
                else
                {
                    mensaje.IdRemitente = idUser;
                    mensaje.TypeRemitente = typeUser;
                }
                var mensajeCreado = await chatService.CreateMensaje(mensaje);

                if (image is not null)
                {
                    var imageUrl = await uploadService.UploadImageMensaje(image, mensajeCreado.IdMensaje.ToString());
                    mensajeCreado.Contenido = imageUrl;
                    mensajeCreado.IsImage = true;
                }
                else
                {
                    mensajeCreado.IsImage = false;
                }
                await chatService.UpdateMensaje(mensajeCreado);
                var gerenteTienda = await gerentesService.GetGerenteTienda(chatGuardado.IdTienda);
                chatGuardado.ImagenTienda = tiendaChat!.LogoTienda;
                chatGuardado.TiendaNameChat = tiendaChat.NombreTienda;
                chatGuardado.UltimoMensaje = mensajeCreado.Contenido;

                var mensajeCreadoDto = mapper.Map<MensajeDto>(mensajeCreado);
                
                if (chatGuardado.TypeMiembro1 == "Tienda" || chatGuardado.TypeMiembro2 == "Tienda")
                {
                    var groupMiembroChat = (chatGuardado.TypeMiembro1 == "Tienda") ? $"{chatGuardado.IdMiembro2}{chatGuardado.TypeMiembro2}Chats" : $"{chatGuardado.IdMiembro1}{chatGuardado.TypeMiembro1}Chats";
                    if (gerenteTienda is not null)
                    {
                        await hubContext.Clients.Group($"{gerenteTienda!.IdGerente}GerenteChats").SendAsync("ChatCreated", chatGuardado, mensajeCreadoDto);
                    }
                    await hubContext.Clients.Group($"{tiendaChat!.IdAdministrador}AdministradorChats").SendAsync("ChatCreated", chatGuardado, mensajeCreadoDto);
                    await hubContext.Clients.Group(groupMiembroChat).SendAsync("ChatCreated", chatGuardado, mensajeCreadoDto);
                }
                else
                {
                    await hubContext.Clients.Group($"{chatGuardado.IdMiembro1}{chatGuardado.TypeMiembro1}Chats").SendAsync("ChatCreated", chatGuardado, mensajeCreadoDto);
                    await hubContext.Clients.Group($"{chatGuardado.IdMiembro2}{chatGuardado.TypeMiembro2}Chats").SendAsync("ChatCreated", chatGuardado, mensajeCreadoDto);
                }
                return Ok(chatGuardado);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }    
        }

        [HttpPost("CreateMensaje")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ChatDto>> CreateMensaje(IFormFile? image, [FromForm] MensajeDto newMensaje, int idChat)
        {
            if (idChat == 0)
            {
                return BadRequest("No se puede poner un idChat de 0");
            }
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;

            var chat = await chatService.GetChat(idChat);
            if(chat is null)
            {
                return NotFound("No se encontró el chat especificado");
            }

            var tiendaChat = await tiendasService.GetOneTienda(chat.IdTienda);

            var mensaje = mapper.Map<Mensaje>(newMensaje);
            mensaje.IdChat = chat.IdChat;
            mensaje.FechaMensaje = DateTime.UtcNow;

            if (chat.TypeMiembro1 == "Usuario" || chat.TypeMiembro2 == "Usuario")
            {
                if (typeUser == "Gerente" || typeUser == "Administrador")
                {
                    mensaje.IdRemitente = tiendaChat!.IdTienda;
                    mensaje.TypeRemitente = "Tienda";
                }
                else
                {
                    mensaje.IdRemitente = idUser;
                    mensaje.TypeRemitente = typeUser;
                }
            }
            else
            {
                mensaje.IdRemitente = idUser;
                mensaje.TypeRemitente = typeUser;
            }

            try
            {
                var mensajeCreado = await chatService.CreateMensaje(mensaje);

                if (image is not null)
                {
                    var imageUrl = await uploadService.UploadImageMensaje(image, mensajeCreado.IdMensaje.ToString());
                    mensajeCreado.Contenido = imageUrl;
                    mensajeCreado.IsImage = true;
                }
                else
                {
                    mensajeCreado.IsImage = false;
                }

                await chatService.UpdateMensaje(mensajeCreado);

                var mensajeRespuesta = mapper.Map<MensajeDto>(mensajeCreado);
                var chatRespuesta = mapper.Map<ChatDto>(chat);
                chatRespuesta.UltimoMensaje = mensajeCreado.Contenido;

                var gerenteTienda = await gerentesService.GetGerenteTienda(chat.IdTienda);

                if (chat.TypeMiembro1 == "Tienda" || chat.TypeMiembro2 == "Tienda")
                {
                    var groupMiembroChat = (chat.TypeMiembro1 == "Tienda") ? $"{chat.IdMiembro2}{chat.TypeMiembro2}Chats" : $"{chat.IdMiembro1}{chat.TypeMiembro1}Chats";
                    if (gerenteTienda is not null)
                    {
                        await hubContext.Clients.Group($"{gerenteTienda!.IdGerente}GerenteChats").SendAsync("NewMessage", mensajeRespuesta, chatRespuesta);
                    }
                    await hubContext.Clients.Group($"{tiendaChat!.IdAdministrador}AdministradorChats").SendAsync("NewMessage", mensajeRespuesta, chatRespuesta);
                    await hubContext.Clients.Group(groupMiembroChat).SendAsync("NewMessage", mensajeRespuesta, chatRespuesta);
                    await hubContext.Clients.Group($"Chat{idChat}").SendAsync("RecieveMessage", mensajeRespuesta, chatRespuesta);
                }
                else
                {
                    await hubContext.Clients.Group($"{chat.IdMiembro1}{chat.TypeMiembro1}Chats").SendAsync("NewMessage", mensajeRespuesta, chatRespuesta);
                    await hubContext.Clients.Group($"{chat.IdMiembro2}{chat.TypeMiembro2}Chats").SendAsync("NewMessage", mensajeRespuesta, chatRespuesta);
                    await hubContext.Clients.Group($"Chat{idChat}").SendAsync("RecieveMessage", mensajeRespuesta, chatRespuesta);
                }
                return Ok(chatRespuesta);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
