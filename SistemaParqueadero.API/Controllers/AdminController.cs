using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaParqueadero.API.Data;
using SistemaParqueadero.API.Helpers;
using SistemaParqueadero.Models;

namespace SistemaParqueadero.API.Controllers
{
    [Authorize(Roles = Roles.Administrador)]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly ParqueaderoDbContext _db;
        public AdminController(ParqueaderoDbContext db) => _db = db;

        // GET: api/admin/usuarios?search=abc
        [HttpGet("usuarios")]
        public async Task<IActionResult> ListarUsuarios([FromQuery] string? search = null)
        {
            var q = _db.Usuarios1.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(u => u.Username.ToLower().Contains(s));
            }

            var usuarios = await q
                .OrderBy(u => u.UsuarioId)
                .Select(u => new
                {
                    usuarioId = u.UsuarioId,
                    nombreCompleto = u.NombreCompleto,
                    username = u.Username,
                    rol = u.Rol,
                    activo = u.Activo,
                    fechaCreacion = u.FechaCreacion
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        public class CambiarRolBody
        {
            public string Rol { get; set; } = "";
        }

        // PUT: api/admin/usuarios/5/rol   body: { "rol": "Operador" } o { "rol": "Usuario" }
        [HttpPut("usuarios/{id:int}/rol")]
        public async Task<IActionResult> CambiarRol(int id, [FromBody] CambiarRolBody body)
        {
            var nuevoRol = (body.Rol ?? "").Trim();

            if (nuevoRol != Roles.Operador && nuevoRol != Roles.Usuario)
                return BadRequest(new { message = $"Rol inválido. Use '{Roles.Usuario}' o '{Roles.Operador}'." });

            var usuario = await _db.Usuarios1.FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (usuario == null) return NotFound(new { message = "Usuario no existe." });

            // No permitir tocar administradores
            if (usuario.Rol == Roles.Administrador)
                return BadRequest(new { message = "No se puede modificar el rol de un Administrador." });

            // Reglas: solo Usuario <-> Operador
            if (usuario.Rol == Roles.Usuario && nuevoRol != Roles.Operador)
                return BadRequest(new { message = $"Solo se puede promover '{Roles.Usuario}' a '{Roles.Operador}'." });

            if (usuario.Rol == Roles.Operador && nuevoRol != Roles.Usuario)
                return BadRequest(new { message = $"Solo se puede degradar '{Roles.Operador}' a '{Roles.Usuario}'." });

            usuario.Rol = nuevoRol;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Rol actualizado.",
                usuarioId = usuario.UsuarioId,
                username = usuario.Username,
                rol = usuario.Rol
            });
        }

        public class CambiarActivoBody
        {
            public bool Activo { get; set; }
        }

        // PUT: api/admin/usuarios/5/activo   body: { "activo": true/false }
        [HttpPut("usuarios/{id:int}/activo")]
        public async Task<IActionResult> CambiarActivo(int id, [FromBody] CambiarActivoBody body)
        {
            var usuario = await _db.Usuarios1.FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (usuario == null) return NotFound(new { message = "Usuario no existe." });

            // No permitir desactivar admin
            if (usuario.Rol == Roles.Administrador)
                return BadRequest(new { message = "No se puede desactivar un Administrador." });

            usuario.Activo = body.Activo;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Estado actualizado.",
                usuarioId = usuario.UsuarioId,
                username = usuario.Username,
                activo = usuario.Activo
            });
        }
    }
}