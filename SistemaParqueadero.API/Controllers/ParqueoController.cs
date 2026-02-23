using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaParqueadero.API.Data;
using SistemaParqueadero.API.DTOs;
using SistemaParqueadero.Models;
using System.Security.Claims;

namespace SistemaParqueadero.API.Controllers
{
    [ApiController]
    [Route("api/parqueo")]
    public class ParqueoController : ControllerBase
    {
        private readonly ParqueaderoDbContext _db;
        public ParqueoController(ParqueaderoDbContext db) => _db = db;

        private int GetUserId()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(id))
                throw new UnauthorizedAccessException("Token sin NameIdentifier.");
            return int.Parse(id);
        }

        // ✅ Operador/Admin: registrar entrada por placa
        // POST: api/parqueo/entrada
        [HttpPost("entrada")]
        [Authorize(Roles = "Operador,Administrador")]
        public async Task<IActionResult> RegistrarEntrada([FromBody] EntradaDto dto)
        {
            var userId = GetUserId();
            var placa = dto.Placa.Trim().ToUpper();

            // Buscar vehículo por placa; si no existe, crearlo automáticamente
            var vehiculo = await _db.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa == placa);

            if (vehiculo == null)
            {
                vehiculo = new Vehiculo { Placa = placa };
                _db.Vehiculos.Add(vehiculo);
                await _db.SaveChangesAsync();
            }

            // Evitar 2 tickets activos para el mismo vehículo
            var yaActivo = await _db.RegistrosParqueo.AsNoTracking()
                .AnyAsync(r => r.VehiculoId == vehiculo.VehiculoId && r.Estado == "Activo");

            if (yaActivo)
                return BadRequest(new { message = "El vehículo ya se encuentra dentro del parqueadero." });

            var registro = new RegistroParqueo
            {
                VehiculoId = vehiculo.VehiculoId,
                TipoTarifa = dto.TipoTarifa,
                FechaHoraIngreso = DateTime.UtcNow,
                UsuarioIngresoId = userId,
                ObservacionesIngreso = dto.Observaciones,
                Estado = "Activo"
            };

            _db.RegistrosParqueo.Add(registro);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Entrada registrada correctamente.",
                registro.RegistroId,
                Placa = placa,
                registro.TipoTarifa,
                registro.FechaHoraIngreso
            });
        }

        // ✅ Operador/Admin: registrar salida por placa
        // POST: api/parqueo/salida
        [HttpPost("salida")]
        [Authorize(Roles = "Operador,Administrador")]
        public async Task<IActionResult> RegistrarSalida([FromBody] SalidaDto dto)
        {
            if (!dto.ConfirmarCierre)
                return BadRequest(new { message = "Debe confirmar el cierre del registro." });

            var userId = GetUserId();
            var placa = dto.Placa.Trim().ToUpper();

            // Buscar el registro activo por placa
            var registro = await _db.RegistrosParqueo
                .Include(r => r.Vehiculo)
                .Where(r => r.Vehiculo.Placa == placa && r.Estado == "Activo")
                .OrderByDescending(r => r.FechaHoraIngreso)
                .FirstOrDefaultAsync();

            if (registro == null)
                return NotFound(new { message = "No hay un ticket activo para esa placa." });

            registro.FechaHoraSalida = DateTime.UtcNow;
            registro.UsuarioSalidaId = userId;

            var horas = (decimal)(registro.FechaHoraSalida.Value - registro.FechaHoraIngreso).TotalHours;

            registro.HorasCalculadas = (decimal)Math.Ceiling((double)horas);
            registro.MontoCobrado = Tarifa.CalcularMonto(registro.TipoTarifa, horas);
            registro.MetodoPago = dto.MetodoPago;
            registro.ObservacionesSalida = dto.Observaciones;
            registro.Estado = "Cerrado";

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Salida registrada correctamente.",
                registro.RegistroId,
                Placa = placa,
                registro.HorasCalculadas,
                registro.MontoCobrado,
                registro.MetodoPago
            });
        }

        // ✅ Operador/Admin: ver tickets activos (lista)
        // GET: api/parqueo/activos
        [HttpGet("activos")]
        [Authorize(Roles = "Operador,Administrador")]
        public async Task<IActionResult> Activos()
        {
            var data = await _db.RegistrosParqueo
                .AsNoTracking()
                .Where(r => r.Estado == "Activo")
                .OrderByDescending(r => r.FechaHoraIngreso)
                .Select(r => new
                {
                    r.RegistroId,
                    Placa = r.Vehiculo.Placa,
                    r.TipoTarifa,
                    r.FechaHoraIngreso
                })
                .ToListAsync();

            return Ok(data);
        }

        // ✅ Operador/Admin: buscar ticket activo por placa
        // GET: api/parqueo/activo-por-placa/ABC123
        [HttpGet("activo-por-placa/{placa}")]
        [Authorize(Roles = "Operador,Administrador")]
        public async Task<IActionResult> ActivoPorPlaca(string placa)
        {
            var p = placa.Trim().ToUpper();

            var registro = await _db.RegistrosParqueo
                .AsNoTracking()
                .Where(r => r.Estado == "Activo" && r.Vehiculo.Placa == p)
                .OrderByDescending(r => r.FechaHoraIngreso)
                .Select(r => new
                {
                    r.RegistroId,
                    Placa = r.Vehiculo.Placa,
                    r.TipoTarifa,
                    r.FechaHoraIngreso,
                    r.ObservacionesIngreso
                })
                .FirstOrDefaultAsync();

            if (registro == null)
                return NotFound(new { message = "No existe ticket activo para esa placa." });

            return Ok(registro);
        }

        // ✅ Operador/Admin: detalle ticket (activo o cerrado)
        // GET: api/parqueo/ticket/5
        [HttpGet("ticket/{id:int}")]
        [Authorize(Roles = "Operador,Administrador")]
        public async Task<IActionResult> Ticket(int id)
        {
            var t = await _db.RegistrosParqueo
                .AsNoTracking()
                .Where(r => r.RegistroId == id)
                .Select(r => new
                {
                    r.RegistroId,
                    r.Estado,
                    Placa = r.Vehiculo.Placa,
                    r.TipoTarifa,
                    r.FechaHoraIngreso,
                    r.FechaHoraSalida,
                    r.HorasCalculadas,
                    r.MontoCobrado,
                    r.MetodoPago,
                    r.ObservacionesIngreso,
                    r.ObservacionesSalida
                })
                .FirstOrDefaultAsync();

            if (t == null) return NotFound(new { message = "Ticket no existe." });
            return Ok(t);
        }

        // ✅ Operador/Admin: historial completo (todos los registros)
        // GET: api/parqueo/historial?placa=ABC&estado=Activo&page=1&pageSize=50
        [HttpGet("historial")]
        [Authorize(Roles = "Operador,Administrador")]
        public async Task<IActionResult> HistorialCompleto(
            [FromQuery] string? placa = null,
            [FromQuery] string? estado = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 200) pageSize = 50;

            var query = _db.RegistrosParqueo
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(placa))
                query = query.Where(r => r.Vehiculo.Placa.Contains(placa.Trim().ToUpper()));

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(r => r.Estado == estado);

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(r => r.FechaHoraIngreso)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new
                {
                    r.RegistroId,
                    Placa = r.Vehiculo.Placa,
                    r.Estado,
                    r.TipoTarifa,
                    r.FechaHoraIngreso,
                    r.FechaHoraSalida,
                    r.HorasCalculadas,
                    r.MontoCobrado,
                    r.MetodoPago
                })
                .ToListAsync();

            return Ok(new { Total = total, Page = page, PageSize = pageSize, Registros = data });
        }

        // ✅ Operador/Admin/Usuario: historial por placa
        // GET: api/parqueo/historial/ABC123
        [HttpGet("historial/{placa}")]
        [Authorize(Roles = "Usuario,Operador,Administrador")]
        public async Task<IActionResult> HistorialPorPlaca(string placa)
        {
            var p = placa.Trim().ToUpper();

            var data = await _db.RegistrosParqueo
                .AsNoTracking()
                .Where(r => r.Vehiculo.Placa == p)
                .OrderByDescending(r => r.FechaHoraIngreso)
                .Select(r => new
                {
                    r.RegistroId,
                    r.Estado,
                    r.TipoTarifa,
                    r.FechaHoraIngreso,
                    r.FechaHoraSalida,
                    r.HorasCalculadas,
                    r.MontoCobrado,
                    r.MetodoPago
                })
                .ToListAsync();

            if (data.Count == 0)
                return NotFound(new { message = "No hay historial para esa placa." });

            return Ok(new { Placa = p, Total = data.Count, Historial = data });
        }
    }
}