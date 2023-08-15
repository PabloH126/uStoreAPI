using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class PlazasService
    {
        private readonly UstoreContext context;
        private IMapper mapper;
        public PlazasService(UstoreContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        public async Task<IEnumerable<CentroComercialDto>> GetMalls()
        {
            var malls = mapper.Map<IEnumerable<CentroComercialDto>>(await context.CentroComercials.AsNoTracking().ToListAsync());
            return malls;
        }

        public async Task<CentroComercial?> GetOneMall(int? id)
        {
            return await context.CentroComercials.FindAsync(id);
        }

        public async Task<CentroComercial?> CreateMall(CentroComercial mall)
        {
            await context.CentroComercials.AddAsync(mall);
            await context.SaveChangesAsync();

            return mall;
        }

        public async Task UpdateMall(CentroComercial mall)
        {
            context.CentroComercials.Update(mall);
            await context.SaveChangesAsync();
        }

        public async Task DeleteMall(CentroComercial mall)
        {
            context.CentroComercials.Remove(mall);
            await context.SaveChangesAsync();
        }
    }
}
