using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaParqueadero.API.Data;
using SistemaParqueadero.API.DTOs;
using SistemaParqueadero.API.Helpers;
using SistemaParqueadero.Models;

namespace SistemaParqueadero.API.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly ParqueaderoDbContext _db;
        public UsuariosController(ParqueaderoDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDto dto)
        {
            var username = dto.Username.Trim();

            var existe = await _db.Usuarios1.AsNoTracking()
                .AnyAsync(u => u.Username == username);

            if (existe)
                return Conflict(new { message = "Username ya existe." });

            var usuario = new Usuario1
            {
                NombreCompleto = dto.NombreCompleto.Trim(),
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = Roles.Usuario, // ✅ SIEMPRE Usuario
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _db.Usuarios1.Add(usuario);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Usuario creado.",
                usuario.UsuarioId,
                usuario.Username,
                usuario.Rol
            });
        }
    }
}