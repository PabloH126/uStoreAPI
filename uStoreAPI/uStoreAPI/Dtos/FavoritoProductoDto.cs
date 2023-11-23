namespace uStoreAPI.Dtos
{
    public class FavoritoProductoDto
    {
        public int IdFavoritoProducto { get; set; }

        public int IdUsuario { get; set; }

        public int IdProducto { get; set; }

        public ProductoDto? ProductoFavorito {  get; set; }
    }
}
