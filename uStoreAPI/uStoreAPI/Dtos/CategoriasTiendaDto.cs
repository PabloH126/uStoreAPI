using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class CategoriasTiendaDto
    {
        public int IdCt { get; set; }
        [Required]
        public int? IdTienda { get; set; }
        [Required]
        public int? IdCategoria { get; set; }
    }
}
