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
    public class ApartadosHub : Hub
    {
        private readonly SolicitudesApartadoService solicitudesService;
        public ApartadosHub(SolicitudesApartadoService _solicitudesService)
        {
            solicitudesService = _solicitudesService;
        }
        public async Task SendUpdateNotificaciones()
        {
            var user = Context.User;
            var idUser = user!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
            await Groups.AddToGroupAsync(Context.ConnectionId, idUser);
            await Clients.Group(idUser).SendAsync("NameGroup", idUser);
        }

        public async Task JoinGroupTienda()
        {
            var idTienda = Context.GetHttpContext()!.Request.Query["idTienda"].ToString();
            await Groups.AddToGroupAsync(Context.ConnectionId, idTienda.ToString());
        }
    }
}
