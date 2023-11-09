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
            var userType = user!.Claims.FirstOrDefault(c => c.Type == "UserType")?.Value;

            if (idUser != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"{idUser}{userType}Chats");
                await Clients.Group($"{idUser}{userType}Chats").SendAsync("Notify", $"{Context.ConnectionId} se ha unido al chat: {idUser}{userType}Chats");
            }
            await base.OnConnectedAsync();
        }

        public async Task JoinGroupChat(string idChat)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat{idChat}");
            await Clients.Group($"Chat{idChat}").SendAsync("Leave", $"Se ha ingresado al chat {idChat}");
        }

        public async Task LeaveGroupChat(string idChat)
        {
            await Clients.Group($"Chat{idChat}").SendAsync("Leave", $"Se ha abandonado el chat {idChat}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Chat{idChat}");
        }
    }
}
