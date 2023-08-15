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
                         expires: DateTime.UtcNow.AddDays(7),
                         signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            else
            {
                var token = new JwtSecurityToken(
                         claims: claims,
                         expires: DateTime.UtcNow.AddHours(3),
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
    }
}
