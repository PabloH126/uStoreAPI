using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class CategoriaDto
    {
        public int IdCategoria { get; set; }
        [Required]
        public string? Categoria1 { get; set; }
    }
}
