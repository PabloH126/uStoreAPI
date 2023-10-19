using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class CuentaGerenteDto
    {
        public int IdCuentaGerente { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Email { get; set; }
        public int? IdDetallesCuentaGerente { get; set; }
        [Required]
        public int? IdGerente { get; set; }
    }
}
