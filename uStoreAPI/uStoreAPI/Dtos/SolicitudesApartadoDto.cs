using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class SolicitudesApartadoDto
    {
        public int IdSolicitud { get; set; }
        [Required]
        public string? PeriodoApartado { get; set; }
        [Required]
        public int? UnidadesProducto { get; set; }
        [Required]
        public string? StatusSolicitud { get; set; }
        [Required]
        public int? IdRatioUsuario { get; set; }
        [Required]
        public int? IdProductos { get; set; }

        public int? IdUsuario { get; set; }
    }
}
