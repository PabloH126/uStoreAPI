using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class PublicacionUpdateDto
    {
        [Required]
        public int IdPublicacion { get; set; }
        [Required]
        public string? Contenido { get; set; }
        public string? Imagen { get; set; }
    }
}
