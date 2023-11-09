using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class ComentariosTiendaDto
    {
        public int IdComentarioTienda { get; set; }
        [Required]
        public string Comentario { get; set; } = null!;

        public DateTime FechaComentario { get; set; }

        public int IdUsuario { get; set; }
        [Required]
        public int IdTienda { get; set; }
    }
}
