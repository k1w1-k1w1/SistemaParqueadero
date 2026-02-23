using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SistemaParqueadero.API.Data;

namespace SistemaParqueadero.API.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequireRolesAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string[] _rolesPermitidos;

        public RequireRolesAttribute(params string[] rolesPermitidos)
        {
            _rolesPermitidos = rolesPermitidos ?? Array.Empty<string>();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1) Leer el userId desde header
            if (!context.HttpContext.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) ||
                !int.TryParse(userIdHeader.ToString(), out var userId))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "Falta header X-User-Id o no es válido."
                });
                return;
            }

            // 2) Obtener DbContext desde DI
            var db = context.HttpContext.RequestServices.GetService(typeof(ParqueaderoDbContext)) as ParqueaderoDbContext;
            if (db == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            // 3) Buscar usuario y validar activo
            var usuario = await db.Usuarios1.AsNoTracking()
                .FirstOrDefaultAsync(u => u.UsuarioId == userId);

            if (usuario == null)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "Usuario no existe."
                });
                return;
            }

            if (!usuario.Activo)
            {
                context.Result = new ObjectResult(new { message = "Usuario inactivo." }) { StatusCode = 403 };
                return;
            }

            // 4) Validar rol
            if (_rolesPermitidos.Length > 0 && !_rolesPermitidos.Contains(usuario.Rol))
            {
                context.Result = new ObjectResult(new
                {
                    message = "No tienes permisos para esta acción.",
                    rolActual = usuario.Rol,
                    rolesPermitidos = _rolesPermitidos
                })
                { StatusCode = 403 };

                return;
            }

            // (Opcional) guardar el usuario en HttpContext para usarlo en controllers
            context.HttpContext.Items["CurrentUser"] = usuario;

            await next();
        }
    }
}