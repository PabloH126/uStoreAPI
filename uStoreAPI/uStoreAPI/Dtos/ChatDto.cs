namespace uStoreAPI.Dtos
{
    public class ChatDto
    {
        public int IdChat { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public int IdMiembro1 { get; set; }

        public string? TypeMiembro1 { get; set; }

        public int IdMiembro2 { get; set; }

        public string? TypeMiembro2 { get; set; }
        public string? UltimoMensaje {  get; set; }
        public string? ImagenTienda { get; set; }
        public string? TiendaNameChat {  get; set; }
        public string? ImagenUsuario {  get; set; }
        public string? NombreUsuario { get; set; }
    }
}
