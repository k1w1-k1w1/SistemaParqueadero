using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaParqueadero.API.Data;
using SistemaParqueadero.API.Helpers;
using SistemaParqueadero.API.Security;
using SistemaParqueadero.Models;

namespace SistemaParqueadero.API.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly ParqueaderoDbContext _db;
        public ReportesController(ParqueaderoDbContext db) => _db = db;

        // ✅ Operador/Admin: historial propio (mis actividades)
        // GET: api/reportes/mis-actividades?desde=2026-02-01&hasta=2026-02-22
        [HttpGet("mis-actividades")]
        [RequireRoles(Roles.Operador, Roles.Administrador)]
        public async Task<IActionResult> MisActividades([FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null)
        {
            var currentUser = HttpContext.Items["CurrentUser"] as Usuario1;
            if (currentUser == null) return Unauthorized();

            int userId = currentUser.UsuarioId;

            var query = _db.RegistrosParqueo.AsNoTracking()
                .Where(r => r.UsuarioIngresoId == userId || r.UsuarioSalidaId == userId);

            if (desde.HasValue) query = query.Where(r => r.FechaHoraIngreso >= desde.Value);
            if (hasta.HasValue) query = query.Where(r => r.FechaHoraIngreso <= hasta.Value);

            var data = await query
                .OrderByDescending(r => r.FechaHoraIngreso)
                .Select(r => new
                {
                    r.RegistroId,
                    Placa = r.Vehiculo.Placa,
                    r.TipoTarifa,
                    r.Estado,
                    r.FechaHoraIngreso,
                    r.FechaHoraSalida,
                    r.MontoCobrado,
                    Acciones = new
                    {
                        HizoEntrada = r.UsuarioIngresoId == userId,
                        HizoSalida = r.UsuarioSalidaId == userId
                    }
                })
                .ToListAsync();

            return Ok(new
            {
                OperadorId = userId,
                OperadorUsername = currentUser.Username,
                Desde = desde,
                Hasta = hasta,
                Total = data.Count,
                Actividades = data
            });
        }

        // ✅ Admin: actividad de todos (con usernames)
        // GET: api/reportes/actividades?operadorId=2&desde=2026-02-01&hasta=2026-02-22
        [HttpGet("actividades")]
        [RequireRoles(Roles.Administrador)]
        public async Task<IActionResult> ActividadesGlobal(
    [FromQuery] int? operadorId = null,
    [FromQuery] DateTime? desde = null,
    [FromQuery] DateTime? hasta = null)
        {
            var baseQuery = _db.RegistrosParqueo.AsNoTracking().AsQueryable();

            if (operadorId.HasValue)
                baseQuery = baseQuery.Where(r => r.UsuarioIngresoId == operadorId.Value || r.UsuarioSalidaId == operadorId.Value);

            if (desde.HasValue)
                baseQuery = baseQuery.Where(r => r.FechaHoraIngreso >= desde.Value);

            if (hasta.HasValue)
                baseQuery = baseQuery.Where(r => r.FechaHoraIngreso <= hasta.Value);

            var query =
                from r in baseQuery

                join uIn in _db.Usuarios1.AsNoTracking()
                    on r.UsuarioIngresoId equals uIn.UsuarioId into ingresoJoin
                from uIngreso in ingresoJoin.DefaultIfEmpty()

                join uOut in _db.Usuarios1.AsNoTracking()
                    on r.UsuarioSalidaId equals uOut.UsuarioId into salidaJoin
                from uSalida in salidaJoin.DefaultIfEmpty()

                select new
                {
                    r.RegistroId,
                    Placa = r.Vehiculo.Placa,
                    r.TipoTarifa,
                    r.Estado,
                    r.FechaHoraIngreso,
                    r.FechaHoraSalida,
                    r.MontoCobrado,

                    UsuarioIngreso = r.UsuarioIngresoId == null ? null : new
                    {
                        uIngreso.UsuarioId,
                        uIngreso.Username,
                        uIngreso.NombreCompleto,
                        uIngreso.Rol
                    },

                    UsuarioSalida = r.UsuarioSalidaId == null ? null : new
                    {
                        uSalida.UsuarioId,
                        uSalida.Username,
                        uSalida.NombreCompleto,
                        uSalida.Rol
                    }
                };

            var data = await query
                .OrderByDescending(x => x.FechaHoraIngreso)
                .ToListAsync();

            return Ok(new
            {
                FiltroOperadorId = operadorId,
                Desde = desde,
                Hasta = hasta,
                Total = data.Count,
                Actividades = data
            });
        }

        // ✅ Admin: recaudación (rango) + conteos por tarifa
        // GET: api/reportes/recaudacion?desde=2026-02-01&hasta=2026-02-22
        [HttpGet("recaudacion")]
        [RequireRoles(Roles.Administrador)]
        public async Task<IActionResult> Recaudacion([FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null)
        {
            var query = _db.RegistrosParqueo.AsNoTracking()
                .Where(r => r.Estado == "Cerrado");

            if (desde.HasValue)
                query = query.Where(r => r.FechaHoraSalida != null && r.FechaHoraSalida >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(r => r.FechaHoraSalida != null && r.FechaHoraSalida <= hasta.Value);

            var totalRecaudado = await query.SumAsync(r => r.MontoCobrado ?? 0m);
            var totalTickets = await query.CountAsync();

            var vip = await query.CountAsync(r => r.TipoTarifa == TipoTarifa.VIP);
            var estandar = await query.CountAsync(r => r.TipoTarifa == TipoTarifa.Estandar);

            return Ok(new
            {
                Desde = desde,
                Hasta = hasta,
                TotalTickets = totalTickets,
                TicketsVIP = vip,
                TicketsEstandar = estandar,
                TotalRecaudado = totalRecaudado
            });
        }
    }
}