namespace uStoreAPI.Dtos
{
    public class ConfiguracionAppUsuarioDto
    {
        public int IdConfiguracion { get; set; }

        public int IdUsuario { get; set; }

        public bool? Notificaciones { get; set; }

        public bool? Favoritos { get; set; }

        public bool? Sugerencias { get; set; }
    }
}
