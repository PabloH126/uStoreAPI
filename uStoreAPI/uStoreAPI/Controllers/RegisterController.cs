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

        public RegisterController(IMapper _mapper, AdminService _adminService)
        {
            mapper = _mapper;
            adminService = _adminService;
        }
        //Crear administradores de tienda y cuenta de administrador
        [HttpPost("Register")]
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
    }
}
