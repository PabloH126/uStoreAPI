using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class CuentaUsuarioDto
    {
        public int IdCuentaUsuario { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public int? IdUsuario { get; set; }

    }
}
