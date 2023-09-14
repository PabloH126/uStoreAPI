using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uStoreAPI.Dtos;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly AdminService adminService;
        private readonly UserService userService;
        private readonly TokenService tokenService;
        public RegisterController(IMapper _mapper, AdminService _adminService, TokenService _tokenService, UserService _userService)
        {
            mapper = _mapper;
            adminService = _adminService;
            tokenService = _tokenService;
            userService = _userService;
        }
        //Crear administradores de tienda y cuenta de administrador
        [HttpPost("RegisterAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CuentaAdministradorDto>> CreateAdminTienda([FromBody] RegisterDto datos)
        {
            if (datos is null || !ModelState.IsValid)
            {
                return BadRequest(datos);
            }

            if (await adminService.VerifyEmail(datos.Email!) is not null)
            {
                return Conflict("Ese email ya esta registrado");
            }

            var cuentaAdmin = await adminService.CreateAdminTienda(datos);

            CuentaAdministradorDto cuentaAdminDto = mapper.Map<CuentaAdministradorDto>(cuentaAdmin);

            return CreatedAtRoute("GetCuentaAdminTienda", 
                      new { controller = "AdminsTienda", id = cuentaAdminDto.IdAdministrador }, 
                      cuentaAdminDto);

        }

        //Crear usuarios y cuenta de usuario
        [HttpPost("RegisterUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CuentaUsuarioDto>> CreateUser([FromBody] RegisterDto datos)
        {
            if (datos is null || !ModelState.IsValid)
            {
                return BadRequest(datos);
            }

            if (await userService.VerifyEmail(datos.Email!) is not null)
            {
                return Conflict("Ese email ya esta registrado");
            }

            var cuentaUser = await userService.CreateUsuario(datos);

            CuentaUsuarioDto cuentaUserDto = mapper.Map<CuentaUsuarioDto>(cuentaUser);
            try
            {
                return CreatedAtRoute("GetCuentaUser",
                      new { controller = "Users", id = cuentaUserDto.IdUsuario },
                      cuentaUserDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Recover")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyEmail([FromBody] RecoverDto recoverDto)
        {
            if (!ModelState.IsValid) 
            { 
                return BadRequest("El email no puede ser nulo"); 
            }
            string email = recoverDto.email!;
            var admin = await adminService.VerifyEmail(email);
            if (admin is null)
            {
                return NotFound("Email no registrado");
            }
            else
            {
                var detallesAdmin = await adminService.GetDetallesAdmin(admin!.IdAdministrador);
                var datoAdmin = await adminService.GetDatoAdmin(detallesAdmin!.IdDatos);
                string token = tokenService.tokenGeneratorMail(admin, datoAdmin!);

                return Ok(new { token });
            }
        }

        [HttpPost("RecoverUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyEmailUser([FromBody] RecoverDto recoverDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("El email no puede ser nulo");
            }
            string email = recoverDto.email!;
            var user = await userService.VerifyEmail(email);
            if (user is null)
            {
                return NotFound("Email no registrado");
            }
            else
            {
                var detallesUser = await userService.GetDetallesUsuario(user!.IdUsuario);
                var datoAdmin = await userService.GetDatoUsuario(detallesUser!.IdDatos);
                string token = tokenService.tokenGeneratorMailUser(user, datoAdmin!);

                return Ok(new { token });
            }
        }
    }
}
