using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class CentroComercialDto
    {
        public int IdCentroComercial { get; set; }
        public string? IconoCentroComercial { get; set; }
        public string? ImagenCentroComercial { get; set; }
        [Required]
        public string? NombreCentroComercial { get; set; }
        [Required]
        public string? HorarioCentroComercial { get; set; }
        [Required]
        public string? DireccionCentroComercial { get; set; }
        public string? IconoCentroComercialThumbNail { get; set; }
        public string? ImagenCentroComercialThumbNail { get; set; }
    }
}
