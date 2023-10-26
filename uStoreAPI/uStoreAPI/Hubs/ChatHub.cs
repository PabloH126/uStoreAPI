using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ChatService chatService;
        public ChatHub(ChatService _cS)
        {
            chatService = _cS;
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var idUser = user!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (idUser != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"{idUser}Chats");
                await Clients.Group($"{idUser}Chats").SendAsync("Notify", $"{Context.ConnectionId} se ha unido al chat: {idUser}Chats");
            }
            await base.OnConnectedAsync();
        }

        public async Task JoinGroupChat(string idChat)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat{idChat}");
        }
    }
}
