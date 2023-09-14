using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uStoreAPI.Dtos;
using uStoreAPI.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using uStoreAPI.ModelsAzureDB;
using Microsoft.AspNetCore.Authorization;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AdminService adminService;
        private readonly UserService userService;
        private readonly LoginService loginService;
        private readonly TokenService tokenService;
        private readonly IMapper mapper;
        public LoginController(LoginService _loginService, AdminService _adminService,TokenService _tokenService, IMapper _mapper, UserService _userService)
        {
            loginService = _loginService;
            mapper = _mapper;
            adminService = _adminService;
            userService = _userService;
            tokenService = _tokenService;
        }

        //Get cuenta de administrador por username y password
        [HttpPost("AdminAuthenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAdmin(LoginDto loginData)
        {

            var cuentaAdmin = await loginService.GetAdmin(loginData);
            if (!ModelState.IsValid || loginData is null)
            {
                return BadRequest(ModelState);
            }
            else if (cuentaAdmin is null)
            {
                return Unauthorized("Credenciales Incorrectas");
            }
            else
            {
                var admin = await adminService.GetAdminTienda(cuentaAdmin.IdAdministrador);
                var detallesAdmin = await adminService.GetDetallesAdmin(admin!.IdDetallesAdministrador);
                var datoAdmin = await adminService.GetDatoAdmin(detallesAdmin!.IdDatos);

                string token = tokenService.TokenGeneratorAdmin(cuentaAdmin, datoAdmin!, loginData.Remember);

                return Ok(new { token });
            }
        }

        [HttpPost("UserAuthenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUser(LoginDto loginData)
        {

            var cuentaUser = await loginService.GetUser(loginData);
            if (!ModelState.IsValid || loginData is null)
            {
                return BadRequest(ModelState);
            }
            else if (cuentaUser is null)
            {
                return Unauthorized("Credenciales Incorrectas");
            }
            else
            {
                var user = await userService.GetUsuario(cuentaUser.IdUsuario);
                var detallesUser = await userService.GetDetallesUsuario(user!.IdDetallesUsuario);
                var datoUser = await userService.GetDatoUsuario(detallesUser!.IdDatos);

                string token = tokenService.TokenGeneratorUser(cuentaUser, datoUser!, loginData.Remember);

                return Ok(new { token });
            }
        }

        [Authorize]
        [HttpPost("getClaims")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetClaims()
        {
            var user = HttpContext.User;

            var nameUserClaim = user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value;
            var emailUserClaim = user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value;
            var idUserClaim = user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;

            return Ok(new { nombre = nameUserClaim, email = emailUserClaim, id = idUserClaim });
        }
    }
}
