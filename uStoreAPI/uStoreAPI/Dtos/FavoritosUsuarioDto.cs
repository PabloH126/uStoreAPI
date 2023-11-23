namespace uStoreAPI.Dtos
{
    public class FavoritosUsuarioDto
    {
        public int IdUsuario { get; set; }
        public IEnumerable<FavoritoTiendaDto>? TiendasFavoritas { get; set; }
        public IEnumerable<FavoritoProductoDto>? ProductosFavoritos { get; set; }
    }
}
