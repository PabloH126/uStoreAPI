using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class ProductoUpdateDto
    {
        [Required]
        public int IdProductos { get; set; }
        public string? NombreProducto { get; set; }
        public double? PrecioProducto { get; set; }
        public int? CantidadApartado { get; set; }
        public string? Descripcion { get; set; }
    }
}
