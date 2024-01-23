using Microsoft.EntityFrameworkCore;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class HorariosService
    {
        private readonly UstoreContext context;
        public HorariosService(UstoreContext _context)
        {
            context = _context;
        }

        public async Task<IEnumerable<Horario>> GetHorariosTienda(int idTienda)
        {
            var horarios = await context.Horarios.Where(t =>
                                        t.IdTienda == idTienda
                                        ).AsNoTracking().ToListAsync();
            return horarios;
        }

        public async Task<Horario?> GetOneHorarioTienda(int idHorario)
        {
            return await context.Horarios.FindAsync(idHorario);
        }

        public async Task<IEnumerable<Horario>> CreateAllHorarios(IEnumerable<Horario> horarios)
        {
            foreach (var horario in horarios)
            {
                await context.Horarios.AddAsync(horario);
                await context.SaveChangesAsync();
            }

            return horarios;
        }

        public async Task UpdateAllHorarios(IEnumerable<Horario> horarios)
        {
            foreach (var horario in horarios)
            {
                var horarioDia = await context.Horarios.FirstOrDefaultAsync(p => p.IdTienda == horario.IdTienda && p.Dia == horario.Dia);
                if(horarioDia is not null)
                {
                    horarioDia.HorarioApertura = horario.HorarioApertura;
                    horarioDia.HorarioCierre = horario.HorarioCierre;
                    context.Horarios.Update(horarioDia);
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task DeleteAllHorarios(int idTienda)
        {
            var horarios = await GetHorariosTienda(idTienda);
            foreach (var horario in horarios)
            {
                context.Horarios.Remove(horario);
            }

            await context.SaveChangesAsync();
        }
    }
}
