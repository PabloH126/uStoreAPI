using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class SolicitudesApartadoUpdateDto
    {
        [Required]
        public int IdSolicitud { get; set; }
        [Required]
        public string? StatusSolicitud { get; set; }
    }
}
