using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class TiendaAppDto
    {
        public int IdTienda { get; set; }
        [Required]
        public string? NombreTienda { get; set; }
        public string? RangoPrecio { get; set; }
        [Required]
        public int? IdCentroComercial { get; set; }
        public int? IdAdministrador { get; set; }
        public int? Apartados { get; set; }
        public int? Vistas { get; set; }
        public string? LogoTienda { get; set; }
        public int? PromedioCalificacionTienda {  get; set; }
        public IEnumerable<CategoriasTiendaDto>? CategoriasTienda { get; set; }
        public IEnumerable<HorarioDto>? Horario { get; set; }
        public IEnumerable<CalificacionTiendaDto>? CalificacionesTienda { get; set; }
        public IEnumerable<ComentariosTiendaDto>? ComentariosTienda { get; set; }
        public IEnumerable<ProductoDto>? ProductosPopularesTienda {  get; set; }
        public IEnumerable<PublicacionesDto>? PublicacionesTienda { get; set; }
        public string? HorarioDiaTienda { get; set; }
    }
}
