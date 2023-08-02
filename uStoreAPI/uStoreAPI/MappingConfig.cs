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
        }
    }
}
