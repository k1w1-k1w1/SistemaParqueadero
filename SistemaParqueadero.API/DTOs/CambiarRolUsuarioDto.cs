using System.ComponentModel.DataAnnotations;

namespace SistemaParqueadero.API.DTOs
{
    public class CambiarRolUsuarioDto
    {
        [Required]
        public int AdminId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [RegularExpression("^(Operador)$", ErrorMessage = "Solo se permite cambiar a 'Operador'")]
        public string NuevoRol { get; set; } = "Operador";
    }
}