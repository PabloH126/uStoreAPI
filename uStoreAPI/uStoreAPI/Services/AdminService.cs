using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Dtos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace uStoreAPI.Services
{
    public class AdminService
    {
        private readonly UstoreContext context;
        public AdminService(UstoreContext _context)
        {
            context = _context;
        }

        public async Task<CuentaAdministrador?> GetCuentaAdminTienda(int? id)
        {
            return await context.CuentaAdministradors.FindAsync(id) ?? await context.CuentaAdministradors.FirstOrDefaultAsync(p => p.IdAdministrador == id);
        }

        public async Task<DetallesCuentaAdministrador?> GetDetallesCuentaAdmin(int? id)
        {
             return await context.DetallesCuentaAdministradors.FindAsync(id);
        }

        public async Task<ImagenPerfil?> GetImagenPerfil(int? id)
        {
            return await context.ImagenPerfils.FindAsync(id);
        }

        public async Task<AdministradorTiendum?> GetAdminTienda(int? id)
        {
            return await context.AdministradorTienda.FindAsync(id);
        }

        public async Task<DetallesAdministrador?> GetDetallesAdmin(int? id)
        {
            return await context.DetallesAdministradors.FindAsync(id);
        }

        public async Task<Dato?> GetDatoAdmin(int? id)
        {
            return await context.Datos.FindAsync(id);
        }

        public async Task<CuentaAdministrador?> CreateAdminTienda(RegisterDto datos)
        {
            Dato dato = new Dato()
            {
                PrimerNombre = datos.PrimerNombre,
                PrimerApellido = datos.PrimerApellido
            };

            await context.Datos.AddAsync(dato);
            await context.SaveChangesAsync();

            DetallesAdministrador detallesAdministrador = new DetallesAdministrador();
            detallesAdministrador.IdDatos = dato.IdDatos;

            await context.DetallesAdministradors.AddAsync(detallesAdministrador);
            await context.SaveChangesAsync();

            AdministradorTiendum adminTienda = new AdministradorTiendum();
            adminTienda.IdDetallesAdministrador = detallesAdministrador.IdDetallesAdministrador;

            await context.AdministradorTienda.AddAsync(adminTienda);
            await context.SaveChangesAsync();

            ImagenPerfil imgPerfil = new ImagenPerfil
            {
                IconoPerfil = "https://ustoredata.blob.core.windows.net/admins/profile_icon.png"
            };

            await context.ImagenPerfils.AddAsync(imgPerfil);
            await context.SaveChangesAsync();

            DetallesCuentaAdministrador detallesCuentaAdmin = new DetallesCuentaAdministrador
            {
                FechaRegistro = DateTime.UtcNow,
                IdImagenPerfil = imgPerfil.IdImagenPerfil
            };

            await context.DetallesCuentaAdministradors.AddAsync(detallesCuentaAdmin);
            await context.SaveChangesAsync();

            CuentaAdministrador cuentaAdmin = new CuentaAdministrador()
            {
                Password = datos.Password,
                Email = datos.Email,
                IdDetallesCuentaAdministrador = detallesCuentaAdmin.IdDetallesCuentaAdministrador,
                IdAdministrador = adminTienda.IdAdministrador
            };

            await context.CuentaAdministradors.AddAsync(cuentaAdmin);
            await context.SaveChangesAsync();

            return cuentaAdmin;
        }

        public async Task PatchAdminPassword(CuentaAdministrador admin)
        {
           context.CuentaAdministradors.Update(admin);
           await context.SaveChangesAsync();
        }

        public async Task PatchAdminImage(string adminImageUrl, int id)
        {
            var admin = await context.CuentaAdministradors.FirstOrDefaultAsync(p => p.IdAdministrador == id);
            var detallesAdmin = await context.DetallesCuentaAdministradors.FindAsync(admin!.IdDetallesCuentaAdministrador);
            var imageAdmin = await context.ImagenPerfils.FindAsync(detallesAdmin!.IdImagenPerfil);

            imageAdmin!.IconoPerfil = adminImageUrl;
            await context.SaveChangesAsync();
        }

        public async Task DeleteAccountAdmin(
                                                CuentaAdministrador cuentaAdmin, 
                                                DetallesCuentaAdministrador detallesCuentaAdmin,
                                                ImagenPerfil imgPerfilAdmin,
                                                AdministradorTiendum adminTienda,
                                                DetallesAdministrador detallesAdmin, 
                                                Dato datosAdmin
                                             )
        {
            context.CuentaAdministradors.Remove(cuentaAdmin);
            context.DetallesCuentaAdministradors.Remove(detallesCuentaAdmin!);
            context.ImagenPerfils.Remove(imgPerfilAdmin!);
            context.AdministradorTienda.Remove(adminTienda!);
            context.DetallesAdministradors.Remove(detallesAdmin!);
            context.Datos.Remove(datosAdmin!);

            await context.SaveChangesAsync();
        }

        public async Task DeleteAdmin(
                                        AdministradorTiendum adminTienda,
                                        DetallesAdministrador detallesAdmin,
                                        Dato datosAdmin
                                     )
        {
            context.AdministradorTienda.Remove(adminTienda);
            context.DetallesAdministradors.Remove(detallesAdmin!);
            context.Datos.Remove(datosAdmin!);

            await context.SaveChangesAsync();
        }

        public async Task<CuentaAdministrador?> VerifyEmail(string email)
        {
            return await context.CuentaAdministradors.FirstOrDefaultAsync(p => p.Email == email);
        }
    }
}
