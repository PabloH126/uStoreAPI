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
        public string? StatusSolicitud { get; set; }
        [Required]
        public int? IdProductos { get; set; }
        public int? IdUsuario { get; set; }
        [Required]
        public int? IdTienda { get; set; }
        public DateTime? FechaApartado { get; set; }

        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public bool? personalizado { get; set; }
        public string? RatioUsuario { get; set; }
        public string? ImageProducto { get; set; }
        public string? NombreProducto { get; set; }
        public double? PrecioProducto { get; set; }
        public string? NombreTienda {  get; set; }
    }
}
