namespace uStoreAPI.Dtos
{
    public class NotificacionUsuarioDto
    {
        public int IdNotificacion { get; set; }

        public int IdUsuario { get; set; }

        public int IdPublicacion { get; set; }

        public DateTime FechaNotificacion { get; set; }
        public string? LogoTienda { get; set; }
        public string? NombreTienda { get; set; }
        public string? Contenido {  get; set; }
    }
}
