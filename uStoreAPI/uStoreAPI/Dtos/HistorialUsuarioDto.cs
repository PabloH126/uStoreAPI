namespace uStoreAPI.Dtos
{
    public class HistorialUsuarioDto
    {
        public int IdUsuario {  get; set; }
        public IEnumerable<ProductoDto>? Productos { get; set; }
        public IEnumerable<TiendaDto>? Tiendas { get; set; }
    }
}
