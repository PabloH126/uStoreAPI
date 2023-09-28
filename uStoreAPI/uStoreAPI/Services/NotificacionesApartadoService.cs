using Microsoft.AspNetCore.SignalR;
using System.Timers;
using uStoreAPI.Dtos;
using uStoreAPI.Hubs;

namespace uStoreAPI.Services
{
    public class NotificacionesApartadoService
    {
        private readonly IHubContext<ApartadosHub> hubContext;
        private System.Timers.Timer timer;
        private List<SolicitudesApartadoDto> pendingSolicitudes = new();

        public NotificacionesApartadoService(IHubContext<ApartadosHub> _hubContext)
        {
            hubContext = _hubContext;
            timer = new System.Timers.Timer(5000);
            timer.Elapsed += SendSolicitudes;
            timer.Start();
        }

        public void CreateSolicitud(SolicitudesApartadoDto solicitud)
        {
            lock (pendingSolicitudes)
            {
                pendingSolicitudes.Add(solicitud);
            }
        }

        public void CancelarSend()
        {
            lock (pendingSolicitudes)
            {
                pendingSolicitudes.Clear();
            }
        }

        private void SendSolicitudes(object? sender, ElapsedEventArgs e)
        {
            List<SolicitudesApartadoDto> solicitudesSend;

            lock(pendingSolicitudes)
            {
                solicitudesSend = new List<SolicitudesApartadoDto>(pendingSolicitudes);
                pendingSolicitudes.Clear();
            }

            if(solicitudesSend.Any())
            {
                foreach (var solicitud in solicitudesSend)
                {
                    hubContext.Clients.Group(solicitud.IdTienda.ToString()!).SendAsync("RecieveSolicitudes", new List<SolicitudesApartadoDto> { solicitud });
                }
            }
        }
    }
}
