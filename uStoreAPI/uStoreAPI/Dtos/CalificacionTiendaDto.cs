using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class CalificacionTiendaDto
    {
        public int IdCalificacionTienda { get; set; }
        [Required]
        public int? Calificacion { get; set; }
        [Required]
        public int? IdTienda { get; set; }

        public int? IdUsuario { get; set; }
    }
}
