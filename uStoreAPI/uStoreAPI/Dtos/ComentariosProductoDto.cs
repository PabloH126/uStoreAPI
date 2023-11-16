namespace uStoreAPI.Dtos
{
    public class ComentariosProductoDto
    {
        public int IdComentarioProducto { get; set; }

        public string Comentario { get; set; } = null!;

        public DateTime FechaComentario { get; set; }

        public int IdUsuario { get; set; }

        public int IdProducto { get; set; }
        public string? ImagenUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public int? CalificacionEstrellas { get; set; }
    }
}
