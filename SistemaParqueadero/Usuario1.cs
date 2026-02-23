using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaParqueadero.Models
{
    public class Usuario1
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Rol { get; set; } // "Operador" o "Administrador"

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación: Un usuario puede registrar muchos parqueos
        public virtual ICollection<RegistroParqueo> RegistrosIngreso { get; set; } = new List<RegistroParqueo>();
        public virtual ICollection<RegistroParqueo> RegistrosSalida { get; set; } = new List<RegistroParqueo>();
    }
}