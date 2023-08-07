using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ExceptionServices;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using System.Security.Cryptography;
using uStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace uStoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsTiendaController : ControllerBase
    {
        private readonly ILogger<AdminsTiendaController> logger;
        private readonly AdminService service;
        private readonly IMapper mapper;
        public AdminsTiendaController(ILogger<AdminsTiendaController> _logger, AdminService _service, IMapper _mapper)
        {
            logger = _logger;
            service = _service;
            mapper = _mapper;
        }

        [HttpGet("Key")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult getKey()
        {
            var accessToken = Request.Headers["Authorization"].ToString().Split(" ")[1];

            // Extrayendo las credenciales del token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(accessToken);

            // Verificar si el token ha expirado
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                return Unauthorized("Token has expired.");
            }
            byte[] secretKey = new byte[32]; // 256 bits para una clave secreta
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(secretKey);
            }

            string secretKeyString = Convert.ToBase64String(secretKey);

            return Ok(secretKeyString);
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

            var CuentaAdminTienda = await service.GetCuentaAdminTienda(id);

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

            var AdminTienda = await service.GetAdminTienda(id);

            if(AdminTienda is null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<AdministradorTiendaDto>(AdminTienda));
        }

        [HttpDelete("DeleteAccount")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccountAdmin(int idAdmin)
        {
            if(idAdmin == 0) return BadRequest();
            
            CuentaAdministrador? cuentaAdmin = await service.GetCuentaAdminTienda(idAdmin);

            if(cuentaAdmin == null) return NotFound("No se encontro la cuenta de admin");

            DetallesCuentaAdministrador? detallesCuentaAdmin = await service.GetDetallesCuentaAdmin(cuentaAdmin.IdDetallesCuentaAdministrador);

            if (detallesCuentaAdmin == null) return NotFound("No se encontro detallesCuentaAdmin");

            ImagenPerfil? imgPerfilAdmin = await service.GetImagenPerfil(detallesCuentaAdmin.IdImagenPerfil);

            if (imgPerfilAdmin == null) return NotFound("No se encontro imgPerfilAdmin");

            AdministradorTiendum? adminTienda = await service.GetAdminTienda(idAdmin);

            if (adminTienda == null) return NotFound("No se encontro adminTienda");

            DetallesAdministrador? detallesAdmin = await service.GetDetallesAdmin(adminTienda.IdDetallesAdministrador);

            if (detallesAdmin == null) return NotFound("No se encontro detallesAdmin");

            Dato? datosAdmin = await service.GetDatoAdmin(detallesAdmin.IdDatos);

            if (datosAdmin == null) return NotFound("No se encontro datosAdmin");

            await service.DeleteAccountAdmin(cuentaAdmin, detallesCuentaAdmin, imgPerfilAdmin, adminTienda, detallesAdmin, datosAdmin);

            return NoContent();
        }

        [HttpDelete("DeleteAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAdmin(int idAdmin)
        {
            if (idAdmin == 0) return BadRequest();

            AdministradorTiendum? adminTienda = await service.GetAdminTienda(idAdmin);

            if (adminTienda == null) return NotFound("No se encontro adminTienda");

            DetallesAdministrador? detallesAdmin = await service.GetDetallesAdmin(adminTienda.IdDetallesAdministrador);

            if (detallesAdmin == null) return NotFound("No se encontro detallesAdmin");

            Dato? datosAdmin = await service.GetDatoAdmin(detallesAdmin.IdDatos);

            if (datosAdmin == null) return NotFound("No se encontro datosAdmin");

            await service.DeleteAdmin(adminTienda, detallesAdmin, datosAdmin);

            return NoContent();
        }

    }
}
