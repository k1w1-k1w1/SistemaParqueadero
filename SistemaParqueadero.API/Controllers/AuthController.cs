using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaParqueadero.API.Data;
using SistemaParqueadero.API.DTOs;
using SistemaParqueadero.API.Services;
using SistemaParqueadero.API.Helpers;
using SistemaParqueadero.Models;

namespace SistemaParqueadero.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ParqueaderoDbContext _db;
        private readonly JwtService _jwt;

        public AuthController(ParqueaderoDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var username = dto.Username.Trim();

            var user = await _db.Usuarios1
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return Unauthorized(new { message = "Credenciales inválidas." });

            if (!user.Activo)
                return Unauthorized(new { message = "Usuario inactivo." });

            var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok)
                return Unauthorized(new { message = "Credenciales inválidas." });

            var token = _jwt.CreateToken(user);

            return Ok(new
            {
                token,
                user = new { user.UsuarioId, user.Username, user.Rol }
            });
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var username = dto.Username.Trim();

            var existe = await _db.Usuarios1.AsNoTracking()
                .AnyAsync(u => u.Username == username);

            if (existe)
                return Conflict(new { message = "Username ya existe." });

            var user = new Usuario1
            {
                NombreCompleto = dto.NombreCompleto.Trim(),
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = Roles.Usuario, // ✅ siempre Usuario
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _db.Usuarios1.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado correctamente." });
        }
    }
}