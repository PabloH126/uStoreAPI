using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging.Signing;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GerentesController : ControllerBase
    {
        private readonly GerentesService gerentesService;
        private readonly TiendasService tiendasService;
        private readonly UploadService uploadService;
        private IMapper mapper;
        public GerentesController(IMapper _mapper, UploadService _uploadService ,GerentesService _gerentesService, TiendasService _tiendasService)
        {
            gerentesService = _gerentesService;
            tiendasService = _tiendasService;
            uploadService = _uploadService;
            mapper = _mapper;
        }

        [Authorize]
        //Get cuenta de administrador de tienda por medio de Id
        [HttpGet("Account", Name = "GetCuentaGerente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CuentaGerenteDto>> GetCuentaGerente(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var cuentaGerente = await gerentesService.GetCuentaGerente(id);

            if (cuentaGerente is null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CuentaGerenteDto>(cuentaGerente));
        }

        [Authorize]
        //Get Administradores de tienda especifico por Id
        [HttpGet("Gerente", Name = "GetGerente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GerenteDto>> GetGerente(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var gerente = await gerentesService.GetGerente(id);

            if (gerente is null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<GerenteDto>(gerente));
        }

        [Authorize]
        //Get Administradores de tienda especifico por Id
        [HttpGet("GetUpdateGerente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GerenteUpdateDto?>> GetUpdateGerente(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var gerente = await gerentesService.GetUpdateGerente(id);

            if (gerente is null)
            {
                return NotFound();
            }

            return Ok(gerente);
        }

        [Authorize]
        //Get Administradores de tienda especifico por Id
        [HttpGet("Gerentes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GerenteDto>> GetGerentesAdmin()
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var gerentes = await gerentesService.GetGerentesAdmin(idUser);

            if (gerentes.IsNullOrEmpty())
            {
                return NotFound("No hay gerentes registrados");
            }

            return Ok(gerentes);
        }

        [Authorize]
        //Get Administradores de tienda especifico por Id
        [HttpGet("VerifyEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyEmailGerentes(string email)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var emailVerified = await gerentesService.VerifyEmail(email);

            if(emailVerified)
            {
                return Conflict("Email ya registrado");
            }
            else
            {
                return Ok();
            }
        }

        [Authorize]
        [HttpPost("UpdateProfileImage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchGerenteImage(IFormFile image, int idGerente)
        {
            if (image is null || image.Length == 0)
            {
                return BadRequest("La imagen no es valida");
            }

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            try
            {
                if (idGerente != 0 && (await gerentesService.GetGerente(idGerente) is not null))
                {
                    idUser = idGerente;
                }

                var imageUrl = await uploadService.UploadImageGerente(image, $"{idUser}.png");

                await gerentesService.PatchGerenteImage(imageUrl, idUser);

                return Ok(new { ImageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("UpdatePass")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchGerentePassword([FromBody] JsonPatchDocument<CuentaGerente> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest("Patch no valido");
            }

            var user = HttpContext.User;
            var idUser = user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);
            if (idUser is not null)
            {
                var gerente = await gerentesService.GetCuentaGerente(int.Parse(idUser.Value));
                if (gerente is null)
                {
                    return NotFound("No se encontro el gerente");
                }

                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    patchDoc.ApplyTo(gerente, ModelState);
                    await gerentesService.PatchGerentePassword(gerente);
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
        [HttpPut("UpdateGerente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAccountGerente([FromBody] GerenteUpdateDto newGerente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cuentaGerenteEncontrado = await gerentesService.GetCuentaGerente(newGerente.IdCuentaGerente);
            if (cuentaGerenteEncontrado is null)
            {
                return NotFound("Gerente no encontrado");
            }

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            var gerenteEncontrado = await gerentesService.GetGerente(cuentaGerenteEncontrado.IdGerente);

            if (gerenteEncontrado!.IdAdministrador != idUser && gerenteEncontrado!.IdGerente != idUser)
            {
                return Unauthorized("Gerente no autorizado para modificar");
            }

            var datosGerente = await gerentesService.GetDatoGerente(gerenteEncontrado.IdDatos);
            datosGerente!.PrimerNombre = newGerente.primerNombre;
            datosGerente.PrimerApellido = newGerente.primerApellido;
            gerenteEncontrado.IdTienda = newGerente.IdTienda;
            cuentaGerenteEncontrado.Password = newGerente.password;

            await gerentesService.UpdateGerente(gerenteEncontrado, cuentaGerenteEncontrado, datosGerente);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("DeleteAccount")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccountGerente(int idGerente)
        {
            if (idGerente == 0) return BadRequest();

            if(await gerentesService.GetGerente(idGerente) is null) 
            {
                return NotFound("No hay gerente registrado con ese id");
            }

            await gerentesService.DeleteAccountGerente(idGerente, null);
            await uploadService.DeleteImageGerentes(idGerente.ToString());

            return NoContent();
        }

        [Authorize]
        [HttpDelete("DeleteGerente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGerente(int idGerente)
        {
            if (idGerente == 0) return BadRequest();

            var gerente = await gerentesService.GetGerente(idGerente);

            if (gerente is null)
            {
                return NotFound("No hay gerente registrado con ese id");
            }

            await gerentesService.DeleteGerente(gerente);
            await uploadService.DeleteImageGerentes(idGerente.ToString());

            return NoContent();
        }
    }
}
