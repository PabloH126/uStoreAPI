using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class HorarioDto
    {
        public int IdHorario { get; set; }
        [Required]
        public string? Dia { get; set; }
        [Required]
        public string? HorarioApertura { get; set; }
        [Required]
        public string? HorarioCierre { get; set; }
        [Required]
        public int? IdTienda { get; set; }
    }
}
