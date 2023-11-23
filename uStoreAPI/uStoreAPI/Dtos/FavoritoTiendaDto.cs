namespace uStoreAPI.Dtos
{
    public class FavoritoTiendaDto
    {
        public int IdFavoritoTienda { get; set; }

        public int IdUsuario { get; set; }

        public int IdTienda { get; set; }
        public TiendaDto? TiendaFavorita { get; set; }
    }
}
