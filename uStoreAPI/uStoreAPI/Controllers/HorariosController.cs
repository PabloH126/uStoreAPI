using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;

namespace uStoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HorariosController : ControllerBase
    {
        private readonly TiendasService tiendasService;
        private readonly HorariosService horariosService;
        private IMapper mapper;

        public HorariosController(TiendasService _tiendasService, HorariosService _horariosService, IMapper _mapper)
        {
            tiendasService = _tiendasService;
            horariosService = _horariosService;
            mapper = _mapper;
        }

        [HttpGet("GetHorarios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<HorarioDto>>> GetHorariosTienda(int idTienda)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var typeUser = user.Claims.FirstOrDefault(u => u.Type == "UserType")!.Value;
            string? idTiendaClaim = user.Claims.FirstOrDefault(u => u.Type == "IdTienda")?.Value;
            int idTiendaClaimValue = 0;
            int.TryParse(idTiendaClaim, out idTiendaClaimValue);
            var tienda = await tiendasService.GetOneTienda(idTienda);

            if (tienda is null)
            {
                return BadRequest("No hay una tienda registrada con ese id");
            }
            else if (typeUser != "Usuario" && tienda.IdAdministrador != idUser && idTiendaClaimValue != tienda.IdTienda)
            {
                return Unauthorized("Tienda no autorizada");
            }

            var horarios = mapper.Map<IEnumerable<HorarioDto>>(await horariosService.GetHorariosTienda(idTienda));

            if(horarios.IsNullOrEmpty())
            {
                return NotFound("No hay horarios para esta tienda");
            }

            return Ok(horarios);
        }

        [HttpGet(Name = "GetHorario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HorarioDto>> GetHorario(int id)
        {
            var horario = await horariosService.GetOneHorarioTienda(id);
            if (horario is null)
            {
                return NotFound("Horario no registrado");
            }
            return Ok(mapper.Map<HorarioDto>(horario));
        }

        [HttpPost("CreateHorario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HorarioDto>> CreateHorario([FromBody] IEnumerable<HorarioDto> horarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var user = HttpContext.User;
                var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
                var tienda = await tiendasService.GetOneTienda(horarioDto.FirstOrDefault()!.IdTienda);
                if (tienda is null)
                {
                    return BadRequest("No hay una tienda registrada con ese id");
                }
                else if(!(tienda.IdAdministrador == idUser))
                {
                    return Unauthorized("Tienda no autorizada");
                }
                
                var horario = mapper.Map<IEnumerable<Horario>>(horarioDto);

                var horarioSalida = mapper.Map<IEnumerable<HorarioDto>>(await horariosService.CreateAllHorarios(horario));

                return Ok(horarioSalida);
            }
        }

        [HttpDelete("DeleteHorarios")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHorarios(int idTienda)
        {
            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);

            if (idTienda == 0)
            {
                return BadRequest("Id invalido");
            }

            var tienda = await tiendasService.GetOneTienda(idTienda);

            if (tienda is null)
            {
                return NotFound("tienda no encontrada");
            }
            else if (!(tienda.IdAdministrador == idUser))
            {
                return Unauthorized("Tienda no autorizada");
            }

            await horariosService.DeleteAllHorarios(tienda.IdTienda);
            return NoContent();
        }

        [HttpPut("UpdateHorario")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHorario([FromBody] IEnumerable<HorarioDto> horarioDto)
        {
            if(horarioDto is null || !ModelState.IsValid)
            {
                return BadRequest("Horario invalido");
            }

            var user = HttpContext.User;
            var idUser = int.Parse(user.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
            var tienda = await tiendasService.GetOneTienda(horarioDto.FirstOrDefault()!.IdTienda);

            if (tienda is null)
            {
                return BadRequest("No hay una tienda registrada con ese id");
            }
            else if (!(tienda.IdAdministrador == idUser))
            {
                return Unauthorized("Tienda no autorizada");
            }

            var horario = mapper.Map<IEnumerable<Horario>>(horarioDto);


            await horariosService.UpdateAllHorarios(horario);

            return NoContent();
        }
    }
}
