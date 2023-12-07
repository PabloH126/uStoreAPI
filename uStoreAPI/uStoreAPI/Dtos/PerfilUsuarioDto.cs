using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Dtos
{
    public class PerfilUsuarioDto
    {
        public int IdUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Correo {  get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? ImagenPerfil {  get; set; }
        public string? ImagenPerfilThumbNail { get; set; }
        public IEnumerable<SolicitudesApartadoDto>? ProductosApartados { get; set; }
        public FavoritosUsuarioDto? FavoritosUsuario { get; set; }
        public HistorialUsuarioDto? HistorialUsuario { get; set; }
    }
}
