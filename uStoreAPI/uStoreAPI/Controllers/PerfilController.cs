using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilController : ControllerBase
    {
        private readonly AdminService adminService;
        private readonly UserService userService;
        private readonly GerentesService gerentesService;
        private readonly SolicitudesApartadoService solicitudesService;
        public PerfilController(SolicitudesApartadoService _sS, AdminService _as, UserService _us, GerentesService _gs)
        {
            adminService = _as;
            userService = _us;
            gerentesService = _gs;
            solicitudesService = _sS;
        }

        [Authorize]
        [HttpGet("GetPerfil")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> GetPerfil()
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            if(typeUser == null)
            {
                return BadRequest();
            }
            else if (typeUser == "Administrador")
            {
                return await adminService.GetPerfilAdmin(idUser);
            }
            else if (typeUser == "Gerente")
            {
                return await gerentesService.GetPerfilGerente(idUser);
            }
            else if (typeUser == "Usuario")
            {
                var perfilUsuario = await userService.GetPerfilUsuario(idUser);
                perfilUsuario!.ProductosApartados = await solicitudesService.GetSolicitudesApartadoUsuario(idUser);
                perfilUsuario!.FavoritosUsuario = await userService.GetFavoritosUsuario(idUser);
                perfilUsuario!.HistorialUsuario = await userService.GetHistorialUsuario(idUser);

                return Ok(perfilUsuario);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpGet("GetHistorialUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HistorialUsuarioDto>> GetHistorialUsuario()
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            if (typeUser != "Usuario")
            {
                return Unauthorized("Debe ser una cuenta de usuario");
            }
            var historial = await userService.GetHistorialUsuario(idUser);
            if (historial == null)
            {
                return NotFound();
            }
            return Ok(historial);
        }
    }
}
