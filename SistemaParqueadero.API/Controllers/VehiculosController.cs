using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaParqueadero.API.Data;
using SistemaParqueadero.API.DTOs;
using SistemaParqueadero.API.Helpers;
using SistemaParqueadero.API.Security;
using SistemaParqueadero.Models;

namespace SistemaParqueadero.API.Controllers
{
    [ApiController]
    [Route("api/vehiculos")]
    public class VehiculosController : ControllerBase
    {
        private readonly ParqueaderoDbContext _db;
        public VehiculosController(ParqueaderoDbContext db) => _db = db;

        // ✅ Usuario / Operador / Admin: listar
        // GET: api/vehiculos?placa=ABC123
        [HttpGet]
        [RequireRoles(Roles.Usuario, Roles.Operador, Roles.Administrador)]
        public async Task<IActionResult> Listar([FromQuery] string? placa = null)
        {
            var query = _db.Vehiculos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var p = placa.Trim().ToUpper();
                query = query.Where(v => v.Placa == p);
            }

            var data = await query
                .OrderBy(v => v.Placa)
                .Select(v => new
                {
                    v.VehiculoId,
                    v.Placa,
                    v.Marca,
                    v.Modelo,
                    v.Color
                })
                .ToListAsync();

            return Ok(data);
        }

        // ✅ Usuario / Operador / Admin: obtener por id
        // GET: api/vehiculos/5
        [HttpGet("{id:int}")]
        [RequireRoles(Roles.Usuario, Roles.Operador, Roles.Administrador)]
        public async Task<IActionResult> Obtener(int id)
        {
            var v = await _db.Vehiculos.AsNoTracking()
                .Where(x => x.VehiculoId == id)
                .Select(x => new
                {
                    x.VehiculoId,
                    x.Placa,
                    x.Marca,
                    x.Modelo,
                    x.Color
                })
                .FirstOrDefaultAsync();

            if (v == null) return NotFound(new { message = "Vehículo no existe." });
            return Ok(v);
        }

        // ✅ Operador/Admin: crear
        // POST: api/vehiculos
        [HttpPost]
        [RequireRoles(Roles.Operador, Roles.Administrador)]
        public async Task<IActionResult> Crear([FromBody] CrearVehiculoDto dto)
        {
            var placa = dto.Placa.Trim().ToUpper();

            var existe = await _db.Vehiculos.AsNoTracking()
                .AnyAsync(v => v.Placa == placa);

            if (existe)
                return Conflict(new { message = "Ya existe un vehículo con esa placa." });

            var vehiculo = new Vehiculo
            {
                Placa = placa,
                Marca = dto.Marca?.Trim(),
                Modelo = dto.Modelo?.Trim(),
                Color = dto.Color?.Trim()
            };

            _db.Vehiculos.Add(vehiculo);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Obtener), new { id = vehiculo.VehiculoId }, new
            {
                vehiculo.VehiculoId,
                vehiculo.Placa,
                vehiculo.Marca,
                vehiculo.Modelo,
                vehiculo.Color
            });
        }

        // ✅ Operador/Admin: actualizar
        // PUT: api/vehiculos/5
        [HttpPut("{id:int}")]
        [RequireRoles(Roles.Operador, Roles.Administrador)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarVehiculoDto dto)
        {
            var vehiculo = await _db.Vehiculos.FirstOrDefaultAsync(v => v.VehiculoId == id);
            if (vehiculo == null) return NotFound(new { message = "Vehículo no existe." });

            var placaNueva = dto.Placa.Trim().ToUpper();

            // Si cambia la placa, verificar que no exista en otro vehículo
            if (vehiculo.Placa != placaNueva)
            {
                var existe = await _db.Vehiculos.AsNoTracking()
                    .AnyAsync(v => v.Placa == placaNueva && v.VehiculoId != id);

                if (existe)
                    return Conflict(new { message = "Ya existe otro vehículo con esa placa." });
            }

            vehiculo.Placa = placaNueva;
            vehiculo.Marca = dto.Marca?.Trim();
            vehiculo.Modelo = dto.Modelo?.Trim();
            vehiculo.Color = dto.Color?.Trim();

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Vehículo actualizado.",
                vehiculo.VehiculoId,
                vehiculo.Placa
            });
        }

        // ✅ Admin: eliminar (opcional)
        // DELETE: api/vehiculos/5
        [HttpDelete("{id:int}")]
        [RequireRoles(Roles.Administrador)]
        public async Task<IActionResult> Eliminar(int id)
        {
            var vehiculo = await _db.Vehiculos.FirstOrDefaultAsync(v => v.VehiculoId == id);
            if (vehiculo == null) return NotFound(new { message = "Vehículo no existe." });

            // Regla típica: no borrar si tiene registros
            var tieneRegistros = await _db.RegistrosParqueo.AsNoTracking()
                .AnyAsync(r => r.VehiculoId == id);

            if (tieneRegistros)
                return BadRequest(new { message = "No se puede eliminar: el vehículo tiene registros de parqueo." });

            _db.Vehiculos.Remove(vehiculo);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Vehículo eliminado." });
        }
    }
}