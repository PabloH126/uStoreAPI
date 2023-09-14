using Microsoft.EntityFrameworkCore;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class UserService
    {
        private readonly UstoreContext context;
        public UserService(UstoreContext _context) 
        { 
            context = _context;
        }

        public async Task<CuentaUsuario?> GetCuentaUsuario(int? id)
        {
            return await context.CuentaUsuarios.FindAsync(id) ?? await context.CuentaUsuarios.FirstOrDefaultAsync(p => p.IdUsuario == id);
        }

        public async Task<DetallesCuentaUsuario?> GetDetallesCuentaUsuario(int? id)
        {
            return await context.DetallesCuentaUsuarios.FindAsync(id);
        }

        public async Task<ImagenPerfil?> GetImagenPerfil(int? id)
        {
            return await context.ImagenPerfils.FindAsync(id);
        }

        public async Task<Usuario?> GetUsuario(int? id)
        {
            return await context.Usuarios.FindAsync(id);
        }

        public async Task<DetallesUsuario?> GetDetallesUsuario(int? id)
        {
            return await context.DetallesUsuarios.FindAsync(id);
        }

        public async Task<Dato?> GetDatoUsuario(int? id)
        {
            return await context.Datos.FindAsync(id);
        }
        
        public async Task<CuentaUsuario> CreateUsuario(RegisterDto datos)
        {
            Dato dato = new Dato()
            {
                PrimerNombre = datos.PrimerNombre,
                PrimerApellido = datos.PrimerApellido,
            };

            await context.Datos.AddAsync(dato);
            await context.SaveChangesAsync();

            DetallesUsuario detallesUser = new DetallesUsuario();
            detallesUser.IdDatos = dato.IdDatos;

            await context.DetallesUsuarios.AddAsync(detallesUser);
            await context.SaveChangesAsync();

            Usuario usuario = new Usuario();
            usuario.IdDetallesUsuario = detallesUser.IdDetallesUsuario;

            await context.Usuarios.AddAsync(usuario);
            await context.SaveChangesAsync();

            ImagenPerfil imgPerfil = new ImagenPerfil
            {
                IconoPerfil = "https://ustoredata.blob.core.windows.net/users/profile_icon.png"
            };

            await context.ImagenPerfils.AddAsync(imgPerfil);
            await context.SaveChangesAsync();

            DetallesCuentaUsuario detallesCuentaUser = new DetallesCuentaUsuario
            {
                FechaRegistro = DateTime.UtcNow,
                IdImagenPerfil = imgPerfil.IdImagenPerfil
            };

            await context.DetallesCuentaUsuarios.AddAsync(detallesCuentaUser);
            await context.SaveChangesAsync();

            CuentaUsuario cuentaUser = new CuentaUsuario()
            {
                Password = datos.Password,
                Email = datos.Email,
                IdDetallesCuentaUsuario = detallesCuentaUser.IdDetallesCuentaUsuario,
                IdUsuario = usuario.IdUsuario

            };

            await context.CuentaUsuarios.AddAsync(cuentaUser);
            await context.SaveChangesAsync();

            return cuentaUser;
        }

        public async Task PatchUserPassword(CuentaUsuario user)
        {
            context.CuentaUsuarios.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task PatchUserImage(string userImageUrl, int id)
        {
            var user = await context.CuentaUsuarios.FindAsync(id);
            var detallesUser = await context.DetallesCuentaUsuarios.FindAsync(user!.IdDetallesCuentaUsuario);
            var imageUser = await context.ImagenPerfils.FindAsync(detallesUser!.IdImagenPerfil);

            imageUser!.IconoPerfil = userImageUrl;
            await context.SaveChangesAsync();
        }

        public async Task DeleteAccountUser(
                                                CuentaUsuario cuentaUser,
                                                DetallesCuentaUsuario detallesCuentaUser,
                                                ImagenPerfil imgPerfilUsuario,
                                                Usuario user,
                                                DetallesUsuario detallesUser,
                                                Dato datosUser
                                            )
        {
            context.CuentaUsuarios.Remove(cuentaUser);
            context.DetallesCuentaUsuarios.Remove(detallesCuentaUser);
            context.ImagenPerfils.Remove(imgPerfilUsuario);
            context.Usuarios.Remove(user);
            context.DetallesUsuarios.Remove(detallesUser);
            context.Datos.Remove(datosUser);

            await context.SaveChangesAsync();
        }

        public async Task DeleteUser(
                                        Usuario user,
                                        DetallesUsuario detallesUser,
                                        Dato datosUser
                                    )
        {
            context.Usuarios.Remove(user);
            context.DetallesUsuarios.Remove(detallesUser);
            context.Datos.Remove(datosUser);

            await context.SaveChangesAsync();
        }

        public async Task<CuentaUsuario?> VerifyEmail(string email)
        {
            return await context.CuentaUsuarios.FirstOrDefaultAsync(p => p.Email == email);
        }
    }
}
