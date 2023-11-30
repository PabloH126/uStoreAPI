using System.ComponentModel.DataAnnotations;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Dtos
{
    public class ProductoAppDto
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
        public int? PromedioCalificacionProducto { get; set; }
        public IEnumerable<CategoriasProductoDto>? CategoriasProducto { get; set; }
        public IEnumerable<CalificacionProductoDto>? CalificacionesProducto { get; set; }
        public IEnumerable<ProductoDto>? ProductosRelacionados { get; set; }
        public IEnumerable<ComentariosProductoDto>? ComentariosProducto { get; set; }
        public IEnumerable<ImagenesProductoDto>? ImagenesProducto { get; set; }
        public string? IsFavorito { get; set; }
        public string? IsUsuarioPenalizado { get; set; }
    }
}
