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
    public class RegisterController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly AdminService adminService;
        private readonly UserService userService;
        private readonly GerentesService gerentesService;
        private readonly TiendasService tiendasService;
        private readonly TokenService tokenService;
        public RegisterController(IMapper _mapper, AdminService _adminService, TokenService _tokenService, UserService _userService, GerentesService _gerentesService, TiendasService _tiendasService)
        {
            mapper = _mapper;
            adminService = _adminService;
            tokenService = _tokenService;
            userService = _userService;
            gerentesService = _gerentesService;
            tiendasService = _tiendasService;
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

            if (await adminService.VerifyEmail(datos.Email!))
            {
                return Conflict("Ese email ya esta registrado");
            }

            var cuentaAdmin = await adminService.CreateAdminTienda(datos);

            CuentaAdministradorDto cuentaAdminDto = mapper.Map<CuentaAdministradorDto>(cuentaAdmin);

            return CreatedAtRoute("GetCuentaAdminTienda", 
                      new { controller = "AdminsTienda", id = cuentaAdminDto.IdAdministrador }, 
                      cuentaAdminDto);

        }

        [Authorize]
        //Crear gerentes de tienda y cuenta de gerentes
        [HttpPost("RegisterGerente")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CuentaAdministradorDto>> CreateGerente([FromBody] RegisterDto datos, int idTienda)
        {
            if (datos is null || !ModelState.IsValid)
            {
                return BadRequest(datos);
            }

            if (idTienda == 0)
            {
                return BadRequest("Id de tienda no especificado");
            }

            if (await gerentesService.VerifyEmail(datos.Email!))
            {
                return Conflict("Ese email ya esta registrado");
            }

            if (await tiendasService.GetOneTienda(idTienda) is null) 
            {
                return BadRequest("Tienda no registrada");
            }
            else if (await gerentesService.VerifyGerente(idTienda) is not null)
            {
                return Conflict("Ya esta asignado un gerente en esta tienda");
            }
            try
            {
                var user = HttpContext.User;
                var idAdmin = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

                var cuentaGerente = await gerentesService.CreateGerente(datos, idAdmin, idTienda);

                CuentaGerenteDto cuentaGerenteDto = mapper.Map<CuentaGerenteDto>(cuentaGerente);

                return CreatedAtRoute("GetCuentaGerente",
                          new { controller = "Gerentes", id = cuentaGerenteDto.IdCuentaGerente },
                          cuentaGerenteDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
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
            var admin = await adminService.VerifyCuentaAdministrador(email);
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

        [HttpPost("RecoverGerente")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyEmailGerente([FromBody] RecoverDto recoverDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("El email no puede ser nulo");
            }
            string email = recoverDto.email!;
            var cuentaGerenteVerify = await gerentesService.VerifyEmail(email);
            if (!cuentaGerenteVerify)
            {
                return NotFound("Email no registrado");
            }
            else
            {
                var cuentaGerente = await gerentesService.GetCuentaGerenteWithEmail(email);
                var gerente = await gerentesService.GetGerente(cuentaGerente!.IdGerente);
                var datoGerente = await gerentesService.GetDatoGerente(gerente!.IdDatos);
                string token = tokenService.tokenGeneratorMailGerente(cuentaGerente, datoGerente!);

                return Ok(new { token });
            }
        }
    }
}
