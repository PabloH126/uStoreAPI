using Microsoft.EntityFrameworkCore;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class GerentesService
    {
        private readonly UstoreContext context;
        private readonly ChatService chatService;
        public GerentesService(UstoreContext _context, ChatService _chatService)
        {
            context = _context;
            chatService = _chatService;
        }

        public async Task<PerfilDto?> GetPerfilGerente(int? id)
        {
            var perfilGerente = await (from cG in context.CuentaGerentes
                                       join dCG in context.DetallesCuentaGerentes on cG.IdDetallesCuentaGerente equals dCG.IdDetallesCuentaGerente
                                       join iP in context.ImagenPerfils on dCG.IdImagenPerfil equals iP.IdImagenPerfil
                                       join g in context.Gerentes on cG.IdGerente equals g.IdGerente
                                       join datos in context.Datos on g.IdDatos equals datos.IdDatos
                                       where cG.IdGerente == id
                                       select new PerfilDto
                                       {
                                           Nombre = $"{datos.PrimerNombre} {datos.PrimerApellido}",
                                           FechaRegistro = dCG.FechaRegistro,
                                           ImagenP = iP.IconoPerfil,
                                           correo = cG.Email
                                       }).FirstOrDefaultAsync();
            return perfilGerente;
        }

        public async Task<IEnumerable<GerenteListaDto?>> GetGerentesAdmin(int? idAdmin)
        {
            var gerentes = await (from cG in context.CuentaGerentes
                                  join dCG in context.DetallesCuentaGerentes on cG.IdDetallesCuentaGerente equals dCG.IdDetallesCuentaGerente
                                  join iP in context.ImagenPerfils on dCG.IdImagenPerfil equals iP.IdImagenPerfil
                                  join g in context.Gerentes on cG.IdGerente equals g.IdGerente
                                  join datos in context.Datos on g.IdDatos equals datos.IdDatos
                                  join t in context.Tienda on g.IdTienda equals t.IdTienda
                                  where g.IdAdministrador == idAdmin
                                  select new GerenteListaDto
                                  {
                                      IdGerente = g.IdGerente,
                                      IdTienda = t.IdTienda,
                                      Nombre = $"{datos.PrimerNombre} {datos.PrimerApellido}",
                                      Email = cG.Email,
                                      TiendaName = t.NombreTienda,
                                      TiendaImage = t.LogoTienda,
                                      iconoPerfil = iP.IconoPerfil,
                                  }).ToListAsync();
            return gerentes;
        }

        public async Task<CuentaGerente?> GetCuentaGerente(int? id)
        {
            return await context.CuentaGerentes.FindAsync(id);
        }

        public async Task<GerenteUpdateDto?> GetUpdateGerente(int? idGerente)
        {
            var gerente = await (from cG in context.CuentaGerentes
                                 join dCG in context.DetallesCuentaGerentes on cG.IdDetallesCuentaGerente equals dCG.IdDetallesCuentaGerente
                                 join iP in context.ImagenPerfils on dCG.IdImagenPerfil equals iP.IdImagenPerfil
                                 join g in context.Gerentes on cG.IdGerente equals g.IdGerente
                                 join datos in context.Datos on g.IdDatos equals datos.IdDatos
                                 join t in context.Tienda on g.IdTienda equals t.IdTienda
                                 where g.IdGerente == idGerente
                                 select new GerenteUpdateDto
                                 {
                                     IdCuentaGerente = cG.IdCuentaGerente,
                                     primerNombre = datos.PrimerNombre,
                                     primerApellido = datos.PrimerApellido,
                                     IdTienda = t.IdTienda,
                                     iconoPerfil = iP.IconoPerfil,
                                     password = cG.Password,
                                 }).SingleOrDefaultAsync();

            return gerente;
        }

        public async Task<CuentaGerente?> GetCuentaGerenteWithEmail(string? email)
        {
            return await context.CuentaGerentes.FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<Gerente?> GetGerente(int? id)
        {
            return await context.Gerentes.FindAsync(id);
        }

        public async Task<Gerente?> GetGerenteTienda(int? idTienda)
        {
            return await context.Gerentes.FirstOrDefaultAsync(p => p.IdTienda == idTienda);
        }

        public async Task<Dato?> GetDatoGerente(int? id)
        {
            return await context.Datos.FindAsync(id);
        }

        public async Task<CuentaGerente> CreateGerente(RegisterDto datos, int? idAdmin, int? idTienda)
        {
            Dato dato = new Dato()
            {
                PrimerNombre = datos.PrimerNombre,
                PrimerApellido = datos.PrimerApellido
            };

            await context.Datos.AddAsync(dato);
            await context.SaveChangesAsync();

            Gerente gerente = new Gerente();
            gerente.IdDatos = dato.IdDatos;
            gerente.IdTienda = idTienda;
            gerente.IdAdministrador = idAdmin;

            await context.Gerentes.AddAsync(gerente);
            await context.SaveChangesAsync();

            ImagenPerfil imgPerfil = new ImagenPerfil()
            {
                IconoPerfil = "https://ustoredata.blob.core.windows.net/gerentes/profile_icon.png"
            };

            await context.ImagenPerfils.AddAsync(imgPerfil);
            await context.SaveChangesAsync();

            DetallesCuentaGerente detallesCuentaGerente = new DetallesCuentaGerente()
            {
                FechaRegistro = DateTime.UtcNow,
                IdImagenPerfil = imgPerfil.IdImagenPerfil
            };

            await context.DetallesCuentaGerentes.AddAsync(detallesCuentaGerente);
            await context.SaveChangesAsync();

            CuentaGerente cuentaGerente = new CuentaGerente()
            {
                Password = datos.Password,
                Email = datos.Email,
                IdDetallesCuentaGerente = detallesCuentaGerente.IdDetallesCuentaGerente,
                IdGerente = gerente.IdGerente
            };

            await context.CuentaGerentes.AddAsync(cuentaGerente);
            await context.SaveChangesAsync();

            return cuentaGerente;
        }

        public async Task PatchGerentePassword(CuentaGerente gerente)
        {
            context.CuentaGerentes.Update(gerente);
            await context.SaveChangesAsync();
        }

        public async Task PatchGerenteImage(string gerenteImageUrl, string gerenteImageThumbNailUrl, int id)
        {
            var cuentaGerente = await context.CuentaGerentes.FirstOrDefaultAsync(p => p.IdGerente == id);
            var detallesCuentaGerente = await context.DetallesCuentaGerentes.FindAsync(cuentaGerente!.IdDetallesCuentaGerente);
            var gerenteImage = await context.ImagenPerfils.FindAsync(detallesCuentaGerente!.IdImagenPerfil);

            gerenteImage!.IconoPerfil = gerenteImageUrl;
            gerenteImage!.IconoPerfilThumbNail = gerenteImageThumbNailUrl;
            await context.SaveChangesAsync();
        }

        public async Task UpdateGerente(Gerente gerente, CuentaGerente? cuentaGerente, Dato? datoGerente)
        {
            if (gerente is not null)
            {
                context.Gerentes.Update(gerente);
            }
            if(cuentaGerente is not null)
            {
                context.CuentaGerentes.Update(cuentaGerente);
            }
            if(datoGerente is not null)
            {
                context.Datos.Update(datoGerente);
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteAccountGerente(int? idGerente, int? idTienda)
        {
            Gerente? gerente = null;

            if (idTienda is not null)
            {
                gerente = await context.Gerentes.FirstOrDefaultAsync(p => p.IdTienda == idTienda);
            }
            else
            {
                gerente = await context.Gerentes.FindAsync(idGerente);
            }

            if(gerente is not null)
            {
                await chatService.DeleteChatGerente(gerente.IdGerente);
                var cuentaGerente = await context.CuentaGerentes.FirstOrDefaultAsync(p => p.IdGerente == gerente!.IdGerente);
                var detallesCuentaGerente = await context.DetallesCuentaGerentes.FindAsync(cuentaGerente!.IdDetallesCuentaGerente);
                var imagenGerente = await context.ImagenPerfils.FindAsync(detallesCuentaGerente!.IdImagenPerfil);
                var datosGerente = await context.Datos.FindAsync(gerente!.IdDatos);

                context.CuentaGerentes.Remove(cuentaGerente);
                context.DetallesCuentaGerentes.Remove(detallesCuentaGerente);
                context.ImagenPerfils.Remove(imagenGerente!);
                context.Gerentes.Remove(gerente);
                context.Datos.Remove(datosGerente!);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteGerente(Gerente gerente)          
        {
            var datosGerente = await context.Datos.FindAsync(gerente.IdDatos);
            context.Gerentes.Remove(gerente);
            context.Datos.Remove(datosGerente!);

            await context.SaveChangesAsync();
        }

        public async Task<bool> VerifyEmail(string email)
        {
            var existingEmailGerentes = await context.CuentaGerentes.AnyAsync(p => p.Email == email);
            if (existingEmailGerentes) return  true;

            var existingEmailUsuarios = await context.CuentaUsuarios.AnyAsync(p => p.Email == email);
            if (existingEmailUsuarios) return true;

            var existingEmailAdmin = await context.CuentaAdministradors.AnyAsync(p => p.Email == email);
            if (existingEmailAdmin) return true;

            return false;
        }

        public async Task<Gerente?> VerifyGerente(int idTienda)
        {
            return await context.Gerentes.FirstOrDefaultAsync(p => p.IdTienda == idTienda);
        }
    }
}
