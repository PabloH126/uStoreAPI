using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class UserService
    {
        private readonly UstoreContext context;
        private IMapper mapper;
        public UserService(UstoreContext _context, IMapper _mapper) 
        { 
            context = _context;
            mapper = _mapper;
        }

        public async Task<PerfilUsuarioDto?> GetPerfilUsuario(int idUsuario)
        {
            var perfilUsuario = await (from cU in context.CuentaUsuarios
                                       join dCU in context.DetallesCuentaUsuarios on cU.IdDetallesCuentaUsuario equals dCU.IdDetallesCuentaUsuario
                                       join u in context.Usuarios on cU.IdUsuario equals u.IdUsuario
                                       join dU in context.DetallesUsuarios on u.IdDetallesUsuario equals dU.IdDetallesUsuario
                                       join datos in context.Datos on dU.IdDatos equals datos.IdDatos
                                       where cU.IdUsuario == idUsuario
                                       select new PerfilUsuarioDto
                                       {
                                           IdUsuario = u.IdUsuario,
                                           NombreUsuario = $"{datos.PrimerNombre} {datos.PrimerApellido}",
                                           Correo = cU.Email,
                                           FechaRegistro = dCU.FechaRegistro,
                                       })
                                       .FirstOrDefaultAsync();

            perfilUsuario!.ImagenPerfil = await (from cU in context.CuentaUsuarios
                                                 join dCU in context.DetallesCuentaUsuarios on cU.IdDetallesCuentaUsuario equals dCU.IdDetallesCuentaUsuario
                                                 join iP in context.ImagenPerfils on dCU.IdImagenPerfil equals iP.IdImagenPerfil
                                                 where cU.IdUsuario == perfilUsuario.IdUsuario
                                                 select (iP)
                                                 )
                                                 .Select(p => p.IconoPerfil)
                                                 .FirstOrDefaultAsync();

            return perfilUsuario;
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

        public async Task<IEnumerable<PenalizacionUsuario>?> GetPenalizacionesUsuario(int idUsuario)
        {
            return await context.PenalizacionUsuarios.Where(p => p.IdUsuario == idUsuario).AsNoTracking().ToListAsync();
        }

        public async Task<PenalizacionUsuario?> GetPenalizacionActualUsuario(int idUsuario)
        {
            return await context.PenalizacionUsuarios.Where(p => p.IdUsuario == idUsuario && DateTime.UtcNow < p.FinPenalizacion).OrderByDescending(p => p.FinPenalizacion).FirstOrDefaultAsync();
        }

        public async Task<HistorialUsuarioDto?> GetHistorialUsuario(int idUsuario)
        {
            var tiendasSolicitudes = new List<TiendaDto>();
            var productosSolicitudes = new List<ProductoDto>();

            var solicitudes = await context.SolicitudesApartados.Where(p => p.IdUsuario == idUsuario).OrderByDescending(p => p.FechaSolicitud).ToListAsync();

            var idsTiendasConFecha = solicitudes
                                        .Select(p => new { p.IdTienda, p.FechaSolicitud })
                                        .ToList();
            var idsProductosConFecha = solicitudes
                                        .Select(p => new { p.IdProductos, p.FechaSolicitud })
                                        .ToList();

            var idsTiendas = idsTiendasConFecha
                                .GroupBy(p => p.IdTienda)
                                .Select(p => p.First().IdTienda) 
                                .ToList();
            var idsProductos = idsProductosConFecha
                                .GroupBy(p => p.IdProductos)
                                .Select(p => p.First().IdProductos)
                                .ToList();

            foreach(var idProducto in idsProductos)
            {
                productosSolicitudes.Add(mapper.Map<ProductoDto>(await context.Productos.FindAsync(idProducto)));
            }

            foreach(var idTienda in idsTiendas)
            {
                tiendasSolicitudes.Add(mapper.Map<TiendaDto>(await context.Tienda.FindAsync(idTienda)));
            }

            foreach(var producto in productosSolicitudes)
            {
                producto.NombreTienda = await context.Tienda.Where(p => p.IdTienda == producto.IdTienda).Select(p => p.NombreTienda).FirstAsync();
                producto.ImageProducto = await context.ImagenesProductos.Where(p => p.IdProductos == producto.IdProductos).Select(p => p.ImagenProducto).FirstAsync();
            }
            
            var historial = new HistorialUsuarioDto
            {
                IdUsuario = idUsuario,
                Productos = productosSolicitudes,
                Tiendas = tiendasSolicitudes
            };

            return historial;
        }

        public async Task<FavoritosUsuarioDto> GetFavoritosUsuario(int idUser)
        {
            var tiendasFavoritas = mapper.Map<IEnumerable<FavoritoTiendaDto>>(await context.FavoritosTienda.Where(p => p.IdUsuario ==  idUser).OrderBy(p => p.IdFavoritoTienda).ToListAsync());
            foreach(var tienda in tiendasFavoritas)
            {
                tienda.TiendaFavorita = mapper.Map<TiendaDto>(await context.Tienda.FindAsync(tienda.IdTienda));
            }
            var productosFavoritos = await (from fav in context.FavoritosProductos
                                            join p in context.Productos on fav.IdProducto equals p.IdProductos
                                            where fav.IdUsuario == idUser
                                            select new FavoritoProductoDto
                                            {
                                                IdFavoritoProducto = fav.IdFavoritoProducto,
                                                IdUsuario = fav.IdUsuario,
                                                IdProducto = fav.IdProducto,
                                                ProductoFavorito = mapper.Map<ProductoDto>(p),
                                            })
                                            .ToListAsync();
            foreach (var producto in productosFavoritos)
            {
                producto.ProductoFavorito!.ImageProducto = await context.ImagenesProductos.Where(p => p.IdProductos == producto.IdProducto).Select(p => p.ImagenProducto).FirstOrDefaultAsync();
            }

            var favoritosUsuario = new FavoritosUsuarioDto
            {
                IdUsuario = idUser,
                ProductosFavoritos = productosFavoritos,
                TiendasFavoritas = tiendasFavoritas
            };

            return favoritosUsuario;
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

        public async Task<PenalizacionUsuario> CreatePenalizacion(int idUsuario)
        {
            var penalizacionesUsuario = await context.PenalizacionUsuarios.Where(p => p.IdUsuario ==  idUsuario).ToListAsync();
            var cantidadPenalizaciones = penalizacionesUsuario.Count();
            DateTime finPenalizacion;

            switch(cantidadPenalizaciones)
            {
                case 0: 
                    finPenalizacion = DateTime.UtcNow.AddDays(1); 
                    break;

                case 1:
                    finPenalizacion = DateTime.UtcNow.AddDays(7);
                    break;

                case 2:
                    finPenalizacion = DateTime.UtcNow.AddMonths(1);
                    break;

                case 3:
                    finPenalizacion = DateTime.UtcNow.AddMonths(6);
                    break;

                case 4:
                    finPenalizacion = DateTime.UtcNow.AddYears(100);
                    break;
                default:
                    finPenalizacion = DateTime.UtcNow;
                    break;
            }

            var penalizacionUsuario = new PenalizacionUsuario()
            {
                IdUsuario = idUsuario,
                InicioPenalizacion = DateTime.UtcNow,
                FinPenalizacion = finPenalizacion,
            };

            await context.PenalizacionUsuarios.AddAsync(penalizacionUsuario);
            await context.SaveChangesAsync();
            return penalizacionUsuario;
        }

        public async Task CreateFavorito(int idUser, int idTienda, int idProducto)
        {
            try
            {
                if (idTienda != 0)
                {
                    var favoritoTienda = new FavoritosTiendum
                    {
                        IdTienda = idTienda,
                        IdUsuario = idUser
                    };
                    await context.FavoritosTienda.AddAsync(favoritoTienda);
                    await context.SaveChangesAsync();
                }
                else if (idProducto != 0)
                {
                    var favoritoProducto = new FavoritosProducto
                    {
                        IdProducto = idProducto,
                        IdUsuario = idUser
                    };
                    await context.FavoritosProductos.AddAsync(favoritoProducto);
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("No se ingreso ningun id del favorito");
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task PatchPenalizacionUsuario(PenalizacionUsuario penalizacion)
        {
            context.PenalizacionUsuarios.Update(penalizacion);
            await context.SaveChangesAsync();
        }

        public async Task PatchUserPassword(CuentaUsuario user)
        {
            context.CuentaUsuarios.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task PatchUserApartados(DetallesUsuario detallesUser)
        {
            context.DetallesUsuarios.Update(detallesUser);
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

        public async Task DeletePenalizacionesUser(int idUsuario)
        {
            var penalizacionesUsuario = await context.PenalizacionUsuarios.Where(p => p.IdUsuario == idUsuario).ToListAsync();
            context.PenalizacionUsuarios.RemoveRange(penalizacionesUsuario);
            await context.SaveChangesAsync();
        }

        public async Task<CuentaUsuario?> VerifyEmail(string email)
        {
            return await context.CuentaUsuarios.FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<bool> VerifyEmailRegistro(string email)
        {
            bool emailExistente = false;
            if (await context.CuentaUsuarios.FirstOrDefaultAsync(p => p.Email == email) is not null)
            {
                emailExistente = true;
            }
            else if (await context.CuentaAdministradors.FirstOrDefaultAsync(p => p.Email == email) is not null)
            {
                emailExistente = true;
            }
            else if (await context.CuentaGerentes.FirstOrDefaultAsync(p => p.Email == email) is not null)
            {
                emailExistente = true;
            }

            return emailExistente;
        }
    }
}
