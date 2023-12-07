using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class TiendaDto
    {
        public int IdTienda { get; set; }
        [Required]
        public string? NombreTienda { get; set; }
        public string? RangoPrecio { get; set; }
        [Required]
        public int? IdCentroComercial { get; set; }
        public int? IdAdministrador { get; set; }
        public int? Apartados { get; set; }
        public int? Vistas { get; set; }
        public string? LogoTienda { get; set; }
        public string? LogoTiendaThumbNail { get; set; }
        public string? IsFavorito { get; set; }
        public int IdChatUsuario { get; set; }
    }
}
