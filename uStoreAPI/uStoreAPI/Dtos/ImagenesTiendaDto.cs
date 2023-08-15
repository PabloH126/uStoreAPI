using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class ImagenesTiendaDto
    {
        public int IdImagenesTiendas { get; set; }
        [Required]
        public string? ImagenTienda { get; set; }
        [Required]
        public int? IdTienda { get; set; }
    }
}
