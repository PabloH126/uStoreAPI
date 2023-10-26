using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class PublicacionesDto
    {
        public int IdPublicacion { get; set; }
        [Required]
        public string? Contenido { get; set; }
        [Required]
        public int? IdTienda { get; set; }

        public DateTime? FechaPublicacion { get; set; }
        public int? IdCentroComercial { get; set; }
        public string? Imagen { get; set; }

        public string? NombreTienda { get; set; }
        public string? LogoTienda { get; set; }
    }
}
