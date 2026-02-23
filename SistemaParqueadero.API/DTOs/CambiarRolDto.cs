using System.ComponentModel.DataAnnotations;

namespace SistemaParqueadero.API.DTOs
{
    public class CambiarRolDto
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [RegularExpression("^(Administrador|Operador)$", ErrorMessage = "Rol debe ser 'Administrador' o 'Operador'")]
        public string NuevoRol { get; set; } = null!;
    }
}