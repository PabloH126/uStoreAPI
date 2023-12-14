using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks.Dataflow;
using uStoreAPI.Dtos;
using uStoreAPI.Hubs;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class UserService
    {
        private readonly UstoreContext context;
        private readonly EmailService emailService;
        private IMapper mapper;
        public UserService(UstoreContext _context, IMapper _mapper, EmailService _emailService) 
        { 
            context = _context;
            mapper = _mapper;
            emailService = _emailService;
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
                                                 .Select(p => p.IconoPerfilThumbNail)
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

        public async Task<AlertaApartado?> GetAlertaApartadoUsuario(int idUsuario, int idProducto)
        {
            return await context.AlertaApartados.FirstOrDefaultAsync(p => p.IdUsuario == idUsuario && p.IdProductos == idProducto);
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
                producto.ImageProducto = await context.ImagenesProductos.Where(p => p.IdProductos == producto.IdProductos).Select(p => p.ImagenProductoThumbNail).FirstAsync();
            }
            
            var historial = new HistorialUsuarioDto
            {
                IdUsuario = idUsuario,
                Productos = productosSolicitudes,
                Tiendas = tiendasSolicitudes
            };

            var favoritosTiendasUsuario = await context.FavoritosTienda.Where(p => idsTiendas.Contains(p.IdTienda) && p.IdUsuario == idUsuario).Select(p => p.IdTienda).ToListAsync();
            var favoritosProductosUsuario = await context.FavoritosProductos.Where(p => idsProductos.Contains(p.IdProducto) && p.IdUsuario == idUsuario).Select(p => p.IdProducto).ToListAsync();

            foreach (var producto in historial.Productos)
            {
                if (favoritosProductosUsuario.Contains(producto.IdProductos))
                {
                    producto.IsFavorito = "corazon_lleno.png";
                }
                else
                {
                    producto.IsFavorito = "corazon_vacio.png";
                }
            }

            foreach (var tienda in historial.Tiendas)
            {
                if (favoritosTiendasUsuario.Contains(tienda.IdTienda))
                {
                    tienda.IsFavorito = "corazon_lleno.png";
                }
                else
                {
                    tienda.IsFavorito = "corazon_vacio.png";
                }
            }

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
                producto.ProductoFavorito!.IsFavorito = "corazon_lleno.png";
                producto.ProductoFavorito!.NombreTienda = await context.Tienda.Where(p => p.IdTienda == producto.ProductoFavorito!.IdTienda).Select(p => p.NombreTienda).FirstOrDefaultAsync();
                producto.ProductoFavorito!.ImageProducto = await context.ImagenesProductos.Where(p => p.IdProductos == producto.IdProducto).Select(p => p.ImagenProductoThumbNail).FirstOrDefaultAsync();
            }
            foreach(var tienda in tiendasFavoritas)
            {
                tienda.TiendaFavorita!.IsFavorito = "corazon_lleno.png";
            }

            var favoritosUsuario = new FavoritosUsuarioDto
            {
                IdUsuario = idUser,
                ProductosFavoritos = productosFavoritos,
                TiendasFavoritas = tiendasFavoritas
            };

            return favoritosUsuario;
        }

        public async Task<IEnumerable<int>> GetFavoritosTiendaUsuario(IEnumerable<int> idsTienda, int idUser)
        {
            return await context.FavoritosTienda.Where(p => idsTienda.Contains(p.IdTienda) && p.IdUsuario == idUser).Select(p => p.IdTienda).ToListAsync();
        }

        public async Task<IEnumerable<int>> GetFavoritosProductoUsuario(IEnumerable<int> idsProducto, int idUser)
        {
            return await context.FavoritosProductos.Where(p => idsProducto.Contains(p.IdProducto) && p.IdUsuario == idUser).Select(p => p.IdProducto).ToListAsync();
        }

        public async Task<IEnumerable<int>> GetUsuariosFavoritosTienda(int idTienda)
        {
            return await context.FavoritosTienda.Where(p => p.IdTienda == idTienda).Select(p => p.IdUsuario).ToListAsync();
        }

        public async Task<string> GetTiempoPenalizacion(int idUser)
        {
            var penalizacionesUsuario = await context.PenalizacionUsuarios.Where(p => p.IdUsuario == idUser).CountAsync();
            switch (penalizacionesUsuario)
            {
                case 0:
                    return "24 horas";

                case 1:
                    return "1 semana";

                case 2:
                    return "1 mes";

                case 3:
                    return "6 meses";

                case 4:
                    return "Indefinida";
                default:
                    return "Error";
            }


        }

        public async Task<IEnumerable<AlertaApartado>> GetAlertasApartadoProducto(int idProducto)
        {
            return await context.AlertaApartados.Where(p => p.IdProductos == idProducto).ToListAsync();
        }

        public async Task<IEnumerable<NotificacionUsuarioDto>> GetNotificacionesUsuario(int idUsuario)
        {
            var notificacionesUsuario = await context.NotificacionUsuarios.Where(p => p.IdUsuario == idUsuario).OrderByDescending(p => p.FechaNotificacion).ToListAsync();
            var notificacionesUsuarioDto = mapper.Map<IEnumerable<NotificacionUsuarioDto>>(notificacionesUsuario);
            foreach (var notificacion in notificacionesUsuarioDto)
            {
                var publicacion = await context.Publicaciones.FindAsync(notificacion.IdPublicacion);
                var tiendaPublicacion = await context.Tienda.FindAsync(publicacion.IdTienda);

                notificacion.LogoTienda = tiendaPublicacion.LogoTienda;
                notificacion.NombreTienda = tiendaPublicacion.NombreTienda;
                notificacion.Contenido = publicacion.Contenido;
            }
            return notificacionesUsuarioDto;
        }

        public async Task<ConfiguracionAppUsuarioDto> GetConfiguracionAppUsuario(int idUsuario)
        {
            var configuracionUsuario = await context.ConfiguracionAppUsuarios.FirstOrDefaultAsync(p => p.IdUsuario == idUsuario);
            return mapper.Map<ConfiguracionAppUsuarioDto>(configuracionUsuario);
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

            ConfiguracionAppUsuario configuracionApp = new ConfiguracionAppUsuario()
            {
                IdUsuario = usuario.IdUsuario,
                Notificaciones = true,
                Favoritos = true,
                Sugerencias = true
            };
            var configuracionAppDto = mapper.Map<ConfiguracionAppUsuarioDto>(configuracionApp);
            await PatchSettingsAppUsuario(configuracionAppDto);

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

        public async Task<AlertaApartado> CreateAlertaApartado(int idUsuario, int idProducto)
        {
            if (await context.Productos.FindAsync(idProducto) is null)
            {
                throw new ("Producto no encontrado");
            }
            else
            {
                var alertaApartadoExistente = await context.AlertaApartados.FirstOrDefaultAsync(p => p.IdUsuario == idUsuario && p.IdProductos == idProducto);
                if (alertaApartadoExistente is null)
                {
                    var alertaApartado = new AlertaApartado
                    {
                        IdProductos = idProducto,
                        IdUsuario = idUsuario
                    };

                    await context.AlertaApartados.AddAsync(alertaApartado);
                    await context.SaveChangesAsync();

                    return alertaApartado;
                }
                else
                {
                    return alertaApartadoExistente;
                }
            }
        }

        public async Task PatchSettingsAppUsuario(ConfiguracionAppUsuarioDto configuracionDto)
        {
            var configuracion = mapper.Map<ConfiguracionAppUsuario>(configuracionDto);
            var configuracionUsuario = await context.ConfiguracionAppUsuarios.FirstOrDefaultAsync(p => p.IdUsuario == configuracionDto.IdUsuario);
            if (configuracionUsuario is not null)
            {
                configuracionUsuario.Notificaciones = configuracion.Notificaciones;
                configuracionUsuario.Favoritos = configuracion.Favoritos;
                configuracionUsuario.Sugerencias = configuracion.Sugerencias;
                context.ConfiguracionAppUsuarios.Update(configuracionUsuario);
                await context.SaveChangesAsync();
            }
            else
            {
                await context.ConfiguracionAppUsuarios.AddAsync(configuracion);
                await context.SaveChangesAsync();
            }

            if (configuracion.Notificaciones == true && configuracion.Sugerencias == true)
            {
                RecurringJob.AddOrUpdate(
                $"SugeridasUsuario_{configuracion.IdUsuario}",
                () => NotificarUsuarioPromocionesSugerenciasSincrono(configuracion.IdUsuario),
                "*/5 * * * *"
            );
            }
            else
            {
                RecurringJob.RemoveIfExists($"SugeridasUsuario_{configuracion.IdUsuario}");
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

        public async Task PatchUserImage(string userImageUrl, string userImageThumbNailUrl, int id)
        {
            var user = await context.CuentaUsuarios.FindAsync(id);
            var detallesUser = await context.DetallesCuentaUsuarios.FindAsync(user!.IdDetallesCuentaUsuario);
            var imageUser = await context.ImagenPerfils.FindAsync(detallesUser!.IdImagenPerfil);

            imageUser!.IconoPerfil = userImageUrl;
            imageUser!.IconoPerfilThumbNail = userImageThumbNailUrl;
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

        public async Task DeleteFavorito(int idUser, int idTienda, int idProducto)
        {
            try
            {
                if (idTienda != 0)
                {
                    var favorito = await context.FavoritosTienda.FirstAsync(p => p.IdTienda == idTienda && p.IdUsuario == idUser);
                    context.FavoritosTienda.Remove(favorito);
                    await context.SaveChangesAsync();
                }
                else if (idProducto != 0)
                {
                    var favorito = await context.FavoritosProductos.FirstAsync(p => p.IdProducto == idProducto && p.IdUsuario == idUser);
                    context.FavoritosProductos.Remove(favorito);
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("No se ingreso ningun id del favorito");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> VerifyFavoritoTienda(int idUser, int idTienda)
        {
            if ((await context.FavoritosTienda.FirstOrDefaultAsync(p => p.IdTienda == idTienda && p.IdUsuario == idUser)) is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> VerifyFavoritoProducto(int idUser, int idProducto)
        {
            if ((await context.FavoritosProductos.FirstOrDefaultAsync(p => p.IdProducto == idProducto && p.IdUsuario == idUser)) is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
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

        public async Task NotificarUsuarioPromocionesFavoritas(int idUsuario, PublicacionesDto publicacion)
        {
            var ajustesUsuario = await context.ConfiguracionAppUsuarios.FirstOrDefaultAsync(p => p.IdUsuario == idUsuario);
            var emailUsuario = await context.CuentaUsuarios.Where(p => p.IdUsuario == idUsuario).Select(p => p.Email).FirstOrDefaultAsync();
            var tiendaPublicacion = await context.Tienda.FirstOrDefaultAsync(p => p.IdTienda == publicacion.IdTienda);

            if (ajustesUsuario is not null && ajustesUsuario.Notificaciones == true && ajustesUsuario.Favoritos == true)
            {
                var notificacionUsuario = new NotificacionUsuario
                {
                    IdUsuario = idUsuario,
                    IdPublicacion = publicacion.IdPublicacion,
                    FechaNotificacion = DateTime.UtcNow,
                    IsSugerida = false,
                    IdTienda = tiendaPublicacion.IdTienda
                };

                await context.NotificacionUsuarios.AddAsync(notificacionUsuario);
                await context.SaveChangesAsync();

                BackgroundJob.Schedule(() => EliminarNotificacion(notificacionUsuario.IdNotificacion), DateTime.UtcNow.AddMonths(2));

                Dictionary<string, string> templateData = new Dictionary<string, string>
                {
                    {"tienda", tiendaPublicacion.NombreTienda! },
                    {"imagen", publicacion.Imagen! },
                    {"tipo", "favoritos" },
                    {"descripcion", publicacion.Contenido! },
                };

                await emailService.SendEmailNotificacionApp(emailUsuario, $"¡Nueva publicación de tu tienda favorita {tiendaPublicacion.NombreTienda}!", templateData, publicacion.Imagen == null);
            }
        }

        public void NotificarUsuarioPromocionesFavoritasSincrono(int idUsuario, PublicacionesDto publicacion)
        {
            NotificarUsuarioPromocionesFavoritas(idUsuario, publicacion).GetAwaiter().GetResult();
        }

        public async Task<object> NotificarUsuarioPromocionesSugerencias(int idUsuario)
        {
            var categoriasUsuario = await (from sU in context.SolicitudesApartados
                                            join t in context.Tienda on sU.IdTienda equals t.IdTienda
                                            join cT in context.CategoriasTiendas on t.IdTienda equals cT.IdTienda
                                            join categorias in context.Categorias on cT.IdCategoria equals categorias.IdCategoria
                                            where sU.IdUsuario == idUsuario
                                            select categorias.IdCategoria)
                                            .Distinct()
                                            .ToListAsync();
            var tiendasSolicitudes = await (from sU in context.SolicitudesApartados
                                            join t in context.Tienda on sU.IdTienda equals t.IdTienda
                                            where sU.IdUsuario == idUsuario
                                            select t.IdTienda)
                                            .Distinct()
                                            .ToListAsync();
            var idsTiendasNotificacionesUsuario = await (from notificacion in context.NotificacionUsuarios
                                               join tienda in context.Tienda on notificacion.IdTienda equals tienda.IdTienda
                                               where notificacion.IdUsuario == idUsuario
                                               select tienda.IdTienda)
                                               .Distinct()
                                               .ToListAsync();
            var idsTiendasFavoritasUsuario = await (from tiendasFavoritas in context.FavoritosTienda
                                                         join tienda in context.Tienda on tiendasFavoritas.IdTienda equals tienda.IdTienda
                                                         where tiendasFavoritas.IdUsuario == idUsuario
                                                         select tienda.IdTienda)
                                               .Distinct()
                                               .ToListAsync();
            var tiendasSugeridas = await (from tienda in context.Tienda
                                          join cT in context.CategoriasTiendas on tienda.IdTienda equals cT.IdTienda
                                          join categorias in context.Categorias on cT.IdCategoria equals categorias.IdCategoria
                                          where categoriasUsuario.Contains(categorias.IdCategoria) && 
                                                !tiendasSolicitudes.Contains(tienda.IdTienda) && 
                                                !idsTiendasNotificacionesUsuario.Contains(tienda.IdTienda) && 
                                                !idsTiendasFavoritasUsuario.Contains(tienda.IdTienda)
                                          select tienda.IdTienda)
                                          .Distinct()
                                          .ToListAsync();

            if (tiendasSugeridas.Any())
            {
                var publicacionesSugeridas = mapper.Map<List<PublicacionesDto>>(await context.Publicaciones.Where(p => tiendasSugeridas.Contains((int)p.IdTienda) && p.FechaPublicacion > DateTime.UtcNow.AddMonths(-1)).ToListAsync());

                if (publicacionesSugeridas.Any())
                {
                    Random random = new Random();
                    int indiceAleatorio = random.Next(publicacionesSugeridas.Count());
                    var publicacion = publicacionesSugeridas[indiceAleatorio];

                    var emailUsuario = await context.CuentaUsuarios.Where(p => p.IdUsuario == idUsuario).Select(p => p.Email).FirstOrDefaultAsync();
                    var tiendaPublicacion = await context.Tienda.FirstOrDefaultAsync(p => p.IdTienda == publicacion.IdTienda);

                    var notificacion = new NotificacionUsuario
                    {
                        IdUsuario = idUsuario,
                        IdPublicacion = publicacion.IdPublicacion,
                        FechaNotificacion = DateTime.UtcNow,
                        IsSugerida = true,
                        IdTienda = tiendaPublicacion.IdTienda
                    };

                    await context.NotificacionUsuarios.AddAsync(notificacion);
                    await context.SaveChangesAsync();

                    BackgroundJob.Schedule(() => EliminarNotificacion(notificacion.IdNotificacion), DateTime.UtcNow.AddMonths(1));

                    Dictionary<string, string> templateData = new Dictionary<string, string>
                    {
                        {"tienda", tiendaPublicacion.NombreTienda! },
                        {"imagen", publicacion.Imagen! },
                        {"tipo", "sugerencias" },
                        {"descripcion", publicacion.Contenido! },
                    };

                    await emailService.SendEmailNotificacionApp(emailUsuario, $"¡Nueva publicación de tienda sugerida {tiendaPublicacion.NombreTienda}!", templateData, publicacion.Imagen == null);
                    return publicacionesSugeridas;
                }
            }

            return tiendasSugeridas;
        }

        public void NotificarUsuarioPromocionesSugerenciasSincrono(int idUsuario)
        {
            NotificarUsuarioPromocionesSugerencias(idUsuario).GetAwaiter().GetResult();
        }

        public async Task NotificarExistenciaProducto(int idUsuario, int idProducto)
        {
            var producto = await context.Productos.FindAsync(idProducto);
            var imagenProducto = await context.ImagenesProductos.Where(p => p.IdProductos == producto.IdProductos).Select(p => p.ImagenProducto).FirstOrDefaultAsync();
            var tiendaProducto = await context.Tienda.FindAsync(producto.IdTienda);
            var emailUsuario = await context.CuentaUsuarios.Where(p => p.IdUsuario == idUsuario).Select(p => p.Email).FirstOrDefaultAsync();
            var datosUsuario = await (from u in context.Usuarios
                                      join dU in context.DetallesUsuarios on u.IdDetallesUsuario equals dU.IdDetallesUsuario
                                      join datos in context.Datos on dU.IdDatos equals datos.IdDatos
                                      where u.IdUsuario == idUsuario
                                      select datos).FirstOrDefaultAsync();
            if (producto is not null)
            {
                Dictionary<string, string> templateData = new Dictionary<string, string>
                {
                    {"nombre",  $"{datosUsuario.PrimerNombre} {datosUsuario.PrimerApellido}"},
                    {"producto", producto.NombreProducto },
                    {"tienda",  tiendaProducto.NombreTienda},
                    {"precio", $"${producto.PrecioProducto}" },
                    {"imagen", imagenProducto}
                };

                await emailService.SendEmailNotificacionApartado(emailUsuario, $"¡Producto {producto.NombreProducto} disponible con {producto.CantidadApartado} unidades!", templateData);
                await EliminarAlertaApartado(idUsuario, idProducto);
            }
        }

        public void NotificarExistenciaProductoSincrono(int idUsuario, int idProducto)
        {
            NotificarExistenciaProducto(idUsuario, idProducto).GetAwaiter().GetResult();
        }

        public async Task NotificarSolicitudUsuario(int idUsuario, SolicitudesApartado solicitud)
        {
            var emailUsuario = await context.CuentaUsuarios.Where(p => p.IdUsuario == idUsuario).Select(p => p.Email).FirstOrDefaultAsync();
            var datosUsuario = await (from u in context.Usuarios
                                      join dU in context.DetallesUsuarios on u.IdDetallesUsuario equals dU.IdDetallesUsuario
                                      join datos in context.Datos on dU.IdDatos equals datos.IdDatos
                                      where u.IdUsuario == idUsuario
                                      select datos).FirstOrDefaultAsync();
            var productoSolicitud = await context.Productos.FindAsync(solicitud.IdProductos);
            var imagenProducto = await context.ImagenesProductos.Where(p => p.IdProductos == productoSolicitud.IdProductos).Select(p => p.ImagenProducto).FirstOrDefaultAsync();
            var tiendaSolicitud = await context.Tienda.FindAsync(solicitud.IdTienda);

            TimeZoneInfo zonaHoraria = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time"); // GMT-6 
            string fechaVencimiento = TimeZoneInfo.ConvertTimeFromUtc(solicitud.FechaVencimiento.Value, zonaHoraria).ToString("dd/MM/yyyy");

            Dictionary<string, string> templateData = new Dictionary<string, string>
            {
                {"producto", productoSolicitud.NombreProducto },
                {"tienda", tiendaSolicitud.NombreTienda },
                {"precio", $"${productoSolicitud.PrecioProducto}" },
                {"imagen", imagenProducto },
                {"nombre", $"{datosUsuario.PrimerNombre} {datosUsuario.PrimerApellido}" },
                {"vencimiento", fechaVencimiento }
            };

            await emailService.SendEmailNotificacionSolicitud(emailUsuario, $"¡Tu solicitud de apartado de {productoSolicitud.NombreProducto} ha sido aceptada!", templateData);
        }

        public async Task EliminarNotificacion(int idNotificacion)
        {
            var notificacion = await context.NotificacionUsuarios.FindAsync(idNotificacion);
            if (notificacion is not null)
            {
                context.NotificacionUsuarios.Remove(notificacion);
                await context.SaveChangesAsync();
            }
        }

        public async Task EliminarAlertaApartado(int idUsuario, int idProducto)
        {
            var alertaApartado = await context.AlertaApartados.FirstOrDefaultAsync(p => p.IdUsuario == idUsuario && p.IdProductos == idProducto);
            if (alertaApartado is not null)
            {
                context.AlertaApartados.Remove(alertaApartado);
                await context.SaveChangesAsync();
            }
        }
    }
}
