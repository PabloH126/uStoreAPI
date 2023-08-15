using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class PeriodosPredeterminadosDto
    {
        public int IdApartadoPredeterminado { get; set; }
        [Required]
        public string? ApartadoPredeterminado { get; set; }
        [Required]
        public int? IdTienda { get; set; }
    }
}
