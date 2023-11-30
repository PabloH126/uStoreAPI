using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class ListaProductosAppDto
    {
        public int IdProductos { get; set; }
        [Required]
        public string? NombreProducto { get; set; }
        [Required]
        public double? PrecioProducto { get; set; }
        [Required]
        public int? CantidadApartado { get; set; }
        [Required]
        public string? Descripcion { get; set; }
        public int? Stock { get; set; }
        [Required]
        public int? IdTienda { get; set; }
        public string? ImageProducto { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int NumeroSolicitudes {  get; set; } 
        public string? NombreTienda { get; set; }
        public string? IconoTienda { get; set;}
        public string? IsFavorito {  get; set; }
    }
}
