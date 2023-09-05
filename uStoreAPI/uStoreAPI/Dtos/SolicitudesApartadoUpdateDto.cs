using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class SolicitudesApartadoUpdateDto
    {
        [Required]
        public int IdSolicitud { get; set; }
        
        public string? PeriodoApartado { get; set; }
        
        public int? UnidadesProducto { get; set; }
        
        public string? StatusSolicitud { get; set; }
        
        public int? IdRatioUsuario { get; set; }
        [Required]
        public int? IdProductos { get; set; }
        [Required]
        public int? IdUsuario { get; set; }
        [Required]
        public int? IdTienda { get; set; }
    }
}
