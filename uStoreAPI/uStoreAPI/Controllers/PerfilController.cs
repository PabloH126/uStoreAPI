using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly TendenciasService tendenciasService;
        private readonly AdminService adminService;
        private readonly UserService userService;
        private readonly GerentesService gerentesService;
        public PerfilController(TendenciasService _ts, AdminService _as, UserService _us, GerentesService _gs)
        {
            tendenciasService = _ts;
            adminService = _as;
            userService = _us;
            gerentesService = _gs;
        }

        [Authorize]
        [HttpGet("GetPerfil")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PerfilDto>> GetPerfil()
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
            else
            {
                return NotFound();
            }
        }
    }
}
