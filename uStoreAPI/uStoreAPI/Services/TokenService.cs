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
                new Claim(ClaimTypes.NameIdentifier, cuentaAdmin.IdAdministrador.ToString()!),
                new Claim("UserType", "Administrador")
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
                new Claim(ClaimTypes.NameIdentifier, cuentaUser.IdUsuario.ToString()!),
                new Claim("UserType", "Usuario")
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

        public string TokenGeneratorGuestUser()
        {
            var idGuest = GenerateRandomAlphaNumeric();
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, $"Guest{idGuest}"),
                new Claim("UserType", "Invitado"),
                new Claim(ClaimTypes.NameIdentifier, idGuest)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                                    claims: claims,
                                    expires: DateTime.UtcNow.Date.AddDays(1),
                                    signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string TokenGeneratorGerente(CuentaGerente gerente, Dato gerenteData, bool remember, string idTienda)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, gerenteData.PrimerNombre!),
                new Claim(ClaimTypes.Email, gerente.Email!),
                new Claim(ClaimTypes.NameIdentifier, gerente.IdGerente.ToString()!),
                new Claim("UserType", "Gerente"),
                new Claim("IdTienda", idTienda)
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

        public string tokenGeneratorMailGerente(CuentaGerente gerente, Dato datosGerente)
        {

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, gerente.Email!),
                new Claim(ClaimTypes.NameIdentifier, gerente.IdCuentaGerente.ToString()!),
                new Claim(ClaimTypes.Name, datosGerente.PrimerNombre!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                         claims: claims,
                         expires: DateTime.UtcNow.AddMinutes(5),
                         signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string tokenGeneratorRegisterUser(RegisterDto datos)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, datos.Email!),
                new Claim("PrimerNombre", datos.PrimerNombre!),
                new Claim("PrimerApellido", datos.PrimerApellido!),
                new Claim("Password", datos.Password!)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                         claims: claims,
                         expires: DateTime.UtcNow.AddMinutes(5),
                         signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRandomAlphaNumeric()
        {
            Random random = new Random();
            const string chars = "0123456789";
            var stringChars = new char[6];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
    }
}
