using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class CategoriasProductoDto
    {
        public int IdCp { get; set; }
        [Required]
        public int? IdProductos { get; set; }
        [Required]
        public int? IdCategoria { get; set; }
        public string? NameCategoria { get; set; }
    }
}
