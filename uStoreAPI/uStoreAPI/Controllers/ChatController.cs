using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
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
        private readonly IHubContext<ChatHub> hubContext;
        private IMapper mapper;
        public ChatController(IMapper _mapper, ChatService _chatService, IHubContext<ChatHub> _hubContext, UploadService _uploadService)
        {
            chatService = _chatService;
            mapper = _mapper;
            hubContext = _hubContext;
            uploadService = _uploadService;
        }
        
        //Get chats de usuario por medio de Id
        [HttpGet("GetChats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Chat?>>> GetChats(string typeChat)
        {
            if(typeChat is null)
            {
                return BadRequest("Se debe ingresar un tipo de Chat");
            }
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var chats = await chatService.GetChats(idUser, typeChat);

            return Ok(chats);
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
        public async Task<ActionResult<Chat?>> CreateChat(IFormFile? image, [FromForm] MensajeDto newMensaje, int idMiembro2, string typeMiembro2)
        {
            if (idMiembro2 == 0)
            {
                return BadRequest();
            }
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;

            Chat newChat = new Chat()
            {
                FechaCreacion = DateTime.UtcNow,
                IdMiembro1 = idUser,
                TypeMiembro1 = typeUser,
                TypeMiembro2 = typeMiembro2,
            };

            var chatGuardado = await chatService.CreateChat(newChat);

            var mensaje = mapper.Map<Mensaje>(newMensaje);
            mensaje.IdChat = chatGuardado.IdChat;
            mensaje.FechaMensaje = DateTime.UtcNow;
            mensaje.IdRemitente = idUser;

            await chatService.CreateMensaje(mensaje);

            if(image is not null)
            {
                var imageUrl = await uploadService.UploadImageMensaje(image, mensaje.IdMensaje.ToString());
                mensaje.Contenido = imageUrl;
                mensaje.IsImage = true;
            }
            else
            {
                mensaje.IsImage = false;
            }

            await chatService.UpdateMensaje(mensaje);
            await hubContext.Clients.Group($"{idUser}Chats").SendAsync("ChatCreated", chatGuardado, mensaje);
            return Ok(chatGuardado);
        }

        [HttpPost("CreateMensaje")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Mensaje?>> CreateMensaje(IFormFile? image, [FromForm] MensajeDto newMensaje, int idChat)
        {
            if (idChat == 0)
            {
                return BadRequest("No se puede poner un idChat de 0");
            }
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;

            var mensaje = mapper.Map<Mensaje>(newMensaje);
            mensaje.IdChat = idChat;
            mensaje.FechaMensaje = DateTime.UtcNow;
            mensaje.IdRemitente = idUser;

            await chatService.CreateMensaje(mensaje);

            if (image is not null)
            {
                var imageUrl = await uploadService.UploadImageMensaje(image, mensaje.IdMensaje.ToString());
                mensaje.Contenido = imageUrl;
                mensaje.IsImage = true;
            }
            else
            {
                mensaje.IsImage = false;
            }

            await chatService.UpdateMensaje(mensaje);
            await hubContext.Clients.Group($"Chat{idChat}").SendAsync("RecieveMessage", mensaje);
            return Ok(mensaje);
        }
    }
}
