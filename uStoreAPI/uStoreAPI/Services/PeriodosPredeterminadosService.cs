using Microsoft.EntityFrameworkCore;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class PeriodosPredeterminadosService
    {
        private readonly UstoreContext context;
        public PeriodosPredeterminadosService(UstoreContext _context)
        {
            context = _context;
        }

        public async Task<IEnumerable<PeriodosPredeterminado>> GetPeriodosPredeterminados(int? idTienda)
        {
            return await context.PeriodosPredeterminados.Where(p => p.IdTienda == idTienda).AsNoTracking().ToListAsync();
        }

        public async Task<PeriodosPredeterminado?> GetOnePeriodoPredeterminado(int? id)
        {
            return await context.PeriodosPredeterminados.FindAsync(id);
        }

        public async Task<IEnumerable<PeriodosPredeterminado>> CreateAllPeriodoPredeterminado(IEnumerable<PeriodosPredeterminado> periodos)
        {
            foreach(var periodo in periodos)
            {
                await context.PeriodosPredeterminados.AddAsync(periodo);
            }
            await context.SaveChangesAsync();

            return periodos;
        }

        public async Task UpdateAllPeriodosPredeterminados(IEnumerable<PeriodosPredeterminado> periodos)
        {
            foreach (var newPeriodo in periodos)
            {
                var periodo = await context.PeriodosPredeterminados.FindAsync(newPeriodo.IdApartadoPredeterminado);
                if(periodo is not null)
                { 
                    periodo.ApartadoPredeterminado = newPeriodo.ApartadoPredeterminado;
                    context.PeriodosPredeterminados.Update(periodo);
                }
                else
                {
                    await context.PeriodosPredeterminados.AddAsync(newPeriodo);
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task DeletePeriodoPredeterminado(PeriodosPredeterminado periodo)
        {
            context.PeriodosPredeterminados.Remove(periodo);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAllPeriodosPredeterminados(int idTienda)
        {
            var periodos = await GetPeriodosPredeterminados(idTienda);
            foreach (var periodo in periodos)
            {
                context.PeriodosPredeterminados.Remove(periodo);
            }
            await context.SaveChangesAsync();
        }
    }
}
