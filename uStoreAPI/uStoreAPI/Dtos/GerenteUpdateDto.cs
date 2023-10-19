using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class GerenteUpdateDto
    {
        [Required]
        public int IdCuentaGerente { get; set; }
        [Required]
        public string? primerNombre { get; set; }
        [Required]
        public string? primerApellido { get; set; }
        public string? password { get; set; }
        [Required]
        public int IdTienda { get; set; }
        public string? iconoPerfil { get; set; }
        
    }
}
