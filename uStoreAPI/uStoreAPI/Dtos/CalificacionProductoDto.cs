using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class CalificacionProductoDto
    {
        public int IdCalificacionProducto { get; set; }
        [Required]
        public int? Calificacion { get; set; }
        [Required]
        public int? IdProductos { get; set; }

        public int? IdUsuario { get; set; }
    }
}
