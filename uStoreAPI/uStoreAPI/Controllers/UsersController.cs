using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService usersService;
        private readonly UploadService uploadService;
        private readonly IMapper mapper;
        public UsersController(UserService _userService, UploadService _uploadService, IMapper _mapper)
        {
            usersService = _userService;
            uploadService = _uploadService;
            mapper = _mapper;
        }

        [Authorize]
        //Get cuenta de administrador de tienda por medio de Id
        [HttpGet("Account", Name = "GetCuentaUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CuentaUsuarioDto>> GetCuentaUser(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var CuentaUsuario = await usersService.GetCuentaUsuario(id);

            if (CuentaUsuario is null)
            {
                return NotFound();
            }
            try
            {
                return Ok(mapper.Map<CuentaUsuarioDto>(CuentaUsuario));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        //Get Administradores de tienda especifico por Id
        [HttpGet("User", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioDto>> GetUser(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var user = await usersService.GetUsuario(id);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<UsuarioDto>(user));
        }

        [Authorize]
        //Get Administradores de tienda especifico por Id
        [HttpGet("GetTiempoPenalizacion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetTiempoPenalizacion()
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var tiempoPenalizacion = await usersService.GetTiempoPenalizacion(idUser);
            return Ok(tiempoPenalizacion);
        }

        [Authorize]
        [HttpPost("UpdateProfileImage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchUserImage(IFormFile image)
        {
            if (image is null || image.Length == 0)
            {
                return BadRequest("La imagen no es valida");
            }

            var user = HttpContext.User;
            var idUser = user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);

            var imageUrl = await uploadService.UploadImageUser(image, $"{idUser!.Value}");

            await usersService.PatchUserImage(imageUrl[0], imageUrl[1], int.Parse(idUser!.Value));

            return Ok(new { ImageUrl = imageUrl });
        }

        [Authorize]
        [HttpPost("SettingsApp")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SettingsApp(ConfiguracionAppUsuarioDto configuracion)
        {
            if (configuracion is null)
            {
                return BadRequest("Configuraciones nulas");
            }

            var user = HttpContext.User;
            var idUser = user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);
            try
            {
                await usersService.PatchSettingsAppUsuario(configuracion);
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("CreateFavorito")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateFavorito(int idProducto = 0, int idTienda = 0)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            if (idProducto == 0 && idTienda == 0)
            {
                return BadRequest("No pueden ser ambos id de cero");
            }

            try
            {
                await usersService.CreateFavorito(idUser, idTienda, idProducto);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [Authorize]
        [HttpPatch("UpdatePass")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchUserPassword([FromBody] JsonPatchDocument<CuentaUsuario> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest("Patch no valido");
            }

            var user = HttpContext.User;

            var idUser = user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);

            if (idUser is not null)
            {
                var usuario = await usersService.GetCuentaUsuario(int.Parse(idUser.Value));
                if (usuario is null)
                {
                    return NotFound("No se encontro el usuario");
                }

                try
                {
                    patchDoc.ApplyTo(usuario, ModelState);
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    await usersService.PatchUserPassword(usuario);
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
        public async Task<IActionResult> DeleteAccountUser(int idUser)
        {
            if (idUser == 0) return BadRequest();

            CuentaUsuario? cuentaUsuario = await usersService.GetCuentaUsuario(idUser);

            if (cuentaUsuario == null) return NotFound("No se encontro la cuenta de usuario");

            DetallesCuentaUsuario? detallesCuentaUsuario = await usersService.GetDetallesCuentaUsuario(cuentaUsuario.IdDetallesCuentaUsuario);

            if (detallesCuentaUsuario == null) return NotFound("No se encontro detallesCuentaUsuario");

            ImagenPerfil? imgPerfilUsuario = await usersService.GetImagenPerfil(detallesCuentaUsuario.IdImagenPerfil);

            if (imgPerfilUsuario == null) return NotFound("No se encontro imgPerfilUsuario");

            Usuario? usuario = await usersService.GetUsuario(idUser);

            if (usuario == null) return NotFound("No se encontro usuario");

            DetallesUsuario? detallesUsuario = await usersService.GetDetallesUsuario(usuario.IdDetallesUsuario);

            if (detallesUsuario == null) return NotFound("No se encontro detallesUsuario");

            Dato? datosUsuario = await usersService.GetDatoUsuario(detallesUsuario.IdDatos);

            if (datosUsuario == null) return NotFound("No se encontro datosUsuario");

            await usersService.DeleteAccountUser(cuentaUsuario, detallesCuentaUsuario, imgPerfilUsuario, usuario, detallesUsuario, datosUsuario);
            await uploadService.DeleteImageUser(idUser.ToString());

            return NoContent();
        }

        [Authorize]
        [HttpDelete("DeleteFavorito")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFavorito(int idProducto = 0, int idTienda = 0)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            if (idProducto == 0 && idTienda == 0)
            {
                return BadRequest("No pueden ser ambos id de cero");
            }

            try
            {
                await usersService.DeleteFavorito(idUser, idTienda, idProducto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }


        [Authorize]
        //Delete Administradores sin cuenta
        [HttpDelete("DeleteUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAdmin(int idUser)
        {
            if (idUser == 0) return BadRequest();

            Usuario? usuario = await usersService.GetUsuario(idUser);

            if (usuario == null) return NotFound("No se encontro usuario");

            DetallesUsuario? detallesUsuario = await usersService.GetDetallesUsuario(usuario.IdDetallesUsuario);

            if (detallesUsuario == null) return NotFound("No se encontro detallesUsuario");

            Dato? datosUsuario = await usersService.GetDatoUsuario(detallesUsuario.IdDatos);

            if (datosUsuario == null) return NotFound("No se encontro datosUsuario");

            await usersService.DeleteUser(usuario, detallesUsuario, datosUsuario);

            return NoContent();
        }

    }
}