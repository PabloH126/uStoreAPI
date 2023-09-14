using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class TokenService
    {
        private IConfiguration config;
        public TokenService(IConfiguration _config)
        {
            config = _config;
        }
        public string TokenGeneratorAdmin(CuentaAdministrador cuentaAdmin, Dato adminData, bool remember)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, adminData.PrimerNombre!),
                new Claim(ClaimTypes.Email, cuentaAdmin.Email!),
                new Claim(ClaimTypes.NameIdentifier, cuentaAdmin.IdAdministrador.ToString()!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            if (remember == true)
            {
                var token = new JwtSecurityToken(
                         claims: claims,
                         expires: DateTime.UtcNow.Date.AddDays(15),
                         signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            else
            {
                var token = new JwtSecurityToken(
                         claims: claims,
                         expires: DateTime.UtcNow.Date.AddDays(1),
                         signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }

        public string TokenGeneratorUser(CuentaUsuario cuentaUser, Dato userData, bool remember)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userData.PrimerNombre!),
                new Claim(ClaimTypes.Email, cuentaUser.Email!),
                new Claim(ClaimTypes.NameIdentifier, cuentaUser.IdUsuario.ToString()!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            if (remember == true)
            {
                var token = new JwtSecurityToken(
                         claims: claims,
                         expires: DateTime.UtcNow.Date.AddDays(30),
                         signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            else
            {
                var token = new JwtSecurityToken(
                         claims: claims,
                         expires: DateTime.UtcNow.Date.AddDays(1),
                         signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }

        public string tokenGeneratorMail(CuentaAdministrador administrador, Dato datoAdmin)
        {

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, administrador.Email!),
                new Claim(ClaimTypes.NameIdentifier, administrador.IdCuentaAdministrador.ToString()!),
                new Claim(ClaimTypes.Name, datoAdmin.PrimerNombre!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                         claims: claims,
                         expires: DateTime.UtcNow.AddMinutes(5),
                         signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string tokenGeneratorMailUser(CuentaUsuario usuario, Dato datoUsuario)
        {

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, usuario.Email!),
                new Claim(ClaimTypes.NameIdentifier, usuario.IdCuentaUsuario.ToString()!),
                new Claim(ClaimTypes.Name, datoUsuario.PrimerNombre!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                         claims: claims,
                         expires: DateTime.UtcNow.AddMinutes(5),
                         signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
