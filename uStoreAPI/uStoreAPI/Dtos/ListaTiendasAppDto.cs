using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class ListaTiendasAppDto
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
        public IEnumerable<CategoriasTiendaDto>? CategoriasTienda { get; set; }
        public IEnumerable<HorarioDto>? Horario { get; set;}
        public string? HorarioDiaTienda { get; set; }
    }
}
