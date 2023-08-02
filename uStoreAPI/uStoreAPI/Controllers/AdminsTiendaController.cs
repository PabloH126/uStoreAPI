using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ExceptionServices;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using System.Security.Cryptography;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsTiendaController : ControllerBase
    {
        private readonly ILogger<AdminsTiendaController> logger;
        private readonly UstoreContext context;
        private readonly IMapper mapper;
        public AdminsTiendaController(ILogger<AdminsTiendaController> _logger, UstoreContext _context, IMapper _mapper)
        {
            logger = _logger;
            context = _context;
            mapper = _mapper;
        }

        [HttpGet("Key")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult getKey()
        {
            byte[] secretKey = new byte[32]; // 256 bits para una clave secreta
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(secretKey);
            }

            string secretKeyString = Convert.ToBase64String(secretKey);

            return Ok(secretKeyString);
        }

        //Get cuenta de administrador por username y password
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AdminLoggedDto>> GetLogin(LoginDto loginData)
        {
            if (!ModelState.IsValid || loginData == null)
            {
                return BadRequest(ModelState);
            }
            if(await context.CuentaAdministradors.FirstOrDefaultAsync(p => p.Email == loginData.Email && p.Password == loginData.Password) is null)
            {
                return Unauthorized("Credenciales Incorrectas");
            }
            else
            {
                AdminLoggedDto cuentaAdminDto = mapper.Map<AdminLoggedDto>(
                                                        await context.CuentaAdministradors
                                                        .FirstOrDefaultAsync(p => p.Email == loginData.Email && p.Password == loginData.Password));

                AdministradorTiendum? adminTienda = await context.AdministradorTienda.FindAsync(cuentaAdminDto.IdAdministrador);
                DetallesAdministrador? detallesAdministrador = await context.DetallesAdministradors.FindAsync(adminTienda!.IdDetallesAdministrador);
                Dato? datoAdmin = await context.Datos.FindAsync(detallesAdministrador!.IdDatos);
                cuentaAdminDto.PrimerNombre = datoAdmin!.PrimerNombre;

                return Ok(cuentaAdminDto);
            }
        }


        //Get cuenta de administrador de tienda por medio de Id
        [HttpGet("Account", Name = "GetCuentaAdminTienda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdministradorTiendaDto>> GetCuentaAdminTienda(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var CuentaAdminTienda = await context.CuentaAdministradors.FindAsync(id);

            if (CuentaAdminTienda is null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CuentaAdministradorDto>(CuentaAdminTienda));
        }
        //Get Administradores de tienda especifico por Id
        [HttpGet("Admin", Name = "GetAdminTienda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdministradorTiendaDto>> GetAdminTienda(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var AdminTienda = await context.AdministradorTienda.FindAsync(id);

            if(AdminTienda is null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<AdministradorTiendaDto>(AdminTienda));
        }

        //Crear administradores de tienda y cuenta de administrador
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CuentaAdministradorDto>> CreateAdminTienda([FromBody] RegisterDto datos)
        {
            if(datos is null)
            {
                return BadRequest(datos);
            }

            if (await context.CuentaAdministradors.FirstOrDefaultAsync(p => p.Email == datos.Email) is not null)
            {
                return Conflict("Ese email ya esta registrado");
            }

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
                IconoPerfil = "test",
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

            CuentaAdministradorDto cuentaAdminDto = mapper.Map<CuentaAdministradorDto>(cuentaAdmin);

            return CreatedAtRoute("GetCuentaAdminTienda", new { id = cuentaAdminDto.IdAdministrador }, cuentaAdminDto);
            
        }

        [HttpPost("CreateAccount")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CuentaAdministradorDto>> CreateCuentaAdmin([FromBody]CuentaAdministradorDto cuentaAdminDto)
        {
            if(cuentaAdminDto is null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            ImagenPerfil imgPerfil = new ImagenPerfil
            {
                IconoPerfil = "test",
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

            CuentaAdministrador cuentaAdministrador = mapper.Map<CuentaAdministrador>(cuentaAdminDto);
            cuentaAdministrador.IdDetallesCuentaAdministrador = detallesCuentaAdmin.IdDetallesCuentaAdministrador;

            await context.CuentaAdministradors.AddAsync(cuentaAdministrador);
            await context.SaveChangesAsync();

            CuentaAdministradorDto cuenta = mapper.Map<CuentaAdministradorDto>(cuentaAdministrador);

            return CreatedAtRoute("GetCuentaAdminTienda", new { id = cuenta.IdCuentaAdministrador }, cuenta);

        }

        [HttpDelete("DeleteAccount")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccountAdmin(int idAdmin)
        {
            if(idAdmin == 0) return BadRequest();
            
            CuentaAdministrador? cuentaAdmin = await context.CuentaAdministradors.FirstOrDefaultAsync(p => p.IdAdministrador == idAdmin);

            if(cuentaAdmin == null) return NotFound("No se encontro la cuenta de admin");

            DetallesCuentaAdministrador? detallesCuentaAdmin = await context.DetallesCuentaAdministradors.FirstOrDefaultAsync(p => p.IdDetallesCuentaAdministrador == cuentaAdmin.IdDetallesCuentaAdministrador);

            if (detallesCuentaAdmin == null) return NotFound("No se encontro detallesCuentaAdmin");

            ImagenPerfil? imgPerfilAdmin = await context.ImagenPerfils.FirstOrDefaultAsync(p => p.IdImagenPerfil == detallesCuentaAdmin.IdImagenPerfil);

            if (imgPerfilAdmin == null) return NotFound("No se encontro imgPerfilAdmin");

            AdministradorTiendum? adminTienda = await context.AdministradorTienda.FirstOrDefaultAsync(p => p.IdAdministrador == idAdmin);

            if (adminTienda == null) return NotFound("No se encontro adminTienda");

            DetallesAdministrador? detallesAdmin = await context.DetallesAdministradors.FirstOrDefaultAsync(p => p.IdDetallesAdministrador == adminTienda.IdDetallesAdministrador);

            if (detallesAdmin == null) return NotFound("No se encontro detallesAdmin");

            Dato? datosAdmin = await context.Datos.FirstOrDefaultAsync(p => p.IdDatos == detallesAdmin.IdDatos);

            if (datosAdmin == null) return NotFound("No se encontro datosAdmin");

            context.CuentaAdministradors.Remove(cuentaAdmin);
            context.DetallesCuentaAdministradors.Remove(detallesCuentaAdmin!);
            context.ImagenPerfils.Remove(imgPerfilAdmin!);
            context.AdministradorTienda.Remove(adminTienda!);
            context.DetallesAdministradors.Remove(detallesAdmin!);
            context.Datos.Remove(datosAdmin!);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAdmin(int idAdmin)
        {
            if (idAdmin == 0) return BadRequest();

            AdministradorTiendum? adminTienda = await context.AdministradorTienda.FirstOrDefaultAsync(p => p.IdAdministrador == idAdmin);

            if (adminTienda == null) return NotFound("No se encontro adminTienda");

            DetallesAdministrador? detallesAdmin = await context.DetallesAdministradors.FirstOrDefaultAsync(p => p.IdDetallesAdministrador == adminTienda.IdDetallesAdministrador);

            if (detallesAdmin == null) return NotFound("No se encontro detallesAdmin");

            Dato? datosAdmin = await context.Datos.FirstOrDefaultAsync(p => p.IdDatos == detallesAdmin.IdDatos);

            if (datosAdmin == null) return NotFound("No se encontro datosAdmin");

            context.AdministradorTienda.Remove(adminTienda);
            context.DetallesAdministradors.Remove(detallesAdmin!);
            context.Datos.Remove(datosAdmin!);

            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
