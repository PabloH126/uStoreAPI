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
using System.Security.Claims;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.OpenApi.Validations;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsTiendaController : ControllerBase
    {
        private readonly ILogger<AdminsTiendaController> logger;
        private readonly AdminService service;
        private readonly UploadService uploadService;
        private readonly IMapper mapper;
        public AdminsTiendaController(ILogger<AdminsTiendaController> _logger, AdminService _service, UploadService _uploadService, IMapper _mapper)
        {
            logger = _logger;
            service = _service;
            mapper = _mapper;
            uploadService = _uploadService;
        }
        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        [HttpPost("UpdateProfileImage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchAdminImage(IFormFile image)
        {
            if (image is null || image.Length == 0)
            {
                return BadRequest("La imagen no es valida");
            }

            var user = HttpContext.User;
            var idUser = user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);

            var imageUrl = await uploadService.UploadImageAdmin(image, $"{idUser!.Value}.png");

            await service.PatchAdminImage(imageUrl, int.Parse(idUser!.Value));

            return Ok(new { ImageUrl = imageUrl });
        }

        [Authorize]
        [HttpPatch("UpdatePass")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchAdminPassword([FromBody] JsonPatchDocument<CuentaAdministrador> patchDoc)
        {
            if(patchDoc is null)
            {
                return BadRequest("Patch no valido");
            }

            var user = HttpContext.User;

            var idUser = user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);
            if (idUser is not null)
            {
                var admin = await service.GetCuentaAdminTienda(int.Parse(idUser.Value));
                if(admin is null)
                {
                    return NotFound("No se encontro el administrador");
                }

                try
                {
                    patchDoc.ApplyTo(admin, ModelState);
                    if(!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    await service.PatchAdminPassword(admin);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
                
                return NoContent();
            }
            else
            {
                return BadRequest("Id no encontrado en los claims");
            }

        }


        [Authorize]
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
            await uploadService.DeleteImageAdmins(idAdmin.ToString());

            return NoContent();
        }


        [Authorize]
        //Delete Administradores sin cuenta
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
