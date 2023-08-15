using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class ProductoDto
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
        [Required]
        public int? Stock { get; set; }
        [Required]
        public int? IdTienda { get; set; }
    }
}
