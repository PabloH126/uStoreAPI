using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class CuentaAdministradorDto
    {
        public int IdCuentaAdministrador { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public int? IdAdministrador { get; set; }
    }
}
