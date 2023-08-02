using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? PrimerNombre { get; set; }
        [Required]
        public string? PrimerApellido { get; set; }
    }
}
