using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SistemaParqueadero.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaParqueadero.API.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(Usuario1 user)
        {
            // ✅ Leer settings (y validar que existan)
            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireMinutesStr = _config["Jwt:ExpireMinutes"];

            if (string.IsNullOrWhiteSpace(key))
                throw new Exception("Falta Jwt:Key en appsettings.json");

            if (!int.TryParse(expireMinutesStr, out var expireMinutes))
                expireMinutes = 120; // ✅ default 2 horas si no está configurado

            var claims = new List<Claim>
            {
                // ✅ user id para ClaimTypes.NameIdentifier (importante para tus controllers)
                new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}