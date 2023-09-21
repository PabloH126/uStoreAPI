using Microsoft.AspNetCore.SignalR;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Hubs
{
    public class ApartadosHub : Hub
    {
        public async Task SendUpdate(IEnumerable<SolicitudesApartadoDto> update)
        {
            await Clients.All.SendAsync("RecieveUdpate", update);
        }

        public async Task JoinGroupTienda()
        {
            var idTienda = Context.GetHttpContext()!.Request.Query["idTienda"].ToString();
            await Groups.AddToGroupAsync(Context.ConnectionId, idTienda.ToString());
        }
    }
}
