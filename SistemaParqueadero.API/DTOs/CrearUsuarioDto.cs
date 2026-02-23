using System.ComponentModel.DataAnnotations;

namespace SistemaParqueadero.API.DTOs
{
    public class CrearUsuarioDto
    {
        [Required, StringLength(100)]
        public string NombreCompleto { get; set; } = null!;

        [Required, StringLength(50)]
        public string Username { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;
    }
}