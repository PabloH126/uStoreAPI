using AutoMapper;
using Microsoft.EntityFrameworkCore;
using uStoreAPI.Dtos;
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

        public async Task<Horario?> GetHorarioTiendaWithDay(int? idTienda, string? dia)
        {
            return await context.Horarios.FirstOrDefaultAsync(p =>  p.IdTienda == idTienda && p.Dia == dia);
        }

        public async Task<Horario> CreateHorario(Horario horario)
        {
            await context.Horarios.AddAsync(horario);
            await context.SaveChangesAsync();

            return horario;
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

        public async Task UpdateHorario(Horario horario)
        {
            context.Horarios.Update(horario);
            await context.SaveChangesAsync();
        }

        public async Task DeleteHorario(Horario horario)
        {
            context.Horarios.Remove(horario);
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
