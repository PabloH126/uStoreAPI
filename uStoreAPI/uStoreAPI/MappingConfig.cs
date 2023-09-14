using AutoMapper;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            CreateMap<Dato, DatoDto>().ReverseMap();
            CreateMap<AdministradorTiendum, AdministradorTiendaDto>().ReverseMap();
            CreateMap<CuentaAdministrador, CuentaAdministradorDto>().ReverseMap();
            CreateMap<CuentaAdministrador, AdminLoggedDto>().ReverseMap();
            CreateMap<Dato, AdminLoggedDto>().ReverseMap();
            CreateMap<CentroComercial, CentroComercialDto>().ReverseMap();
            CreateMap<Tiendum, TiendaDto>().ReverseMap();
            CreateMap<Horario, HorarioDto>().ReverseMap();
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<CategoriasTienda, CategoriasTiendaDto>().ReverseMap();
            CreateMap<CategoriasProducto, CategoriasProductoDto>().ReverseMap();
            CreateMap<PeriodosPredeterminado, PeriodosPredeterminadosDto>().ReverseMap();
            CreateMap<Producto, ProductoDto>().ReverseMap();
            CreateMap<CalificacionTiendum, CalificacionTiendaDto>().ReverseMap();
            CreateMap<CalificacionProducto, CalificacionProductoDto>().ReverseMap();
            CreateMap<SolicitudesApartado, SolicitudesApartadoDto>().ReverseMap();
            CreateMap<Publicacione, PublicacionesDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<CuentaUsuario, CuentaUsuarioDto>().ReverseMap();
        }
    }
}
