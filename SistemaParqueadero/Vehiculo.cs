using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaParqueadero.Models
{
    public class Vehiculo
    {
        [Key]
        public int VehiculoId { get; set; }

        [Required]
        [StringLength(10)]
        public string Placa { get; set; }

        [StringLength(50)]
        public string? Marca { get; set; }

        [StringLength(50)]
        public string? Modelo { get; set; }

        [StringLength(20)]
        public string? Color { get; set; }

        // Relación: Un vehículo puede tener muchos registros de parqueo
        public virtual ICollection<RegistroParqueo> Registros { get; set; } = new List<RegistroParqueo>();
    }
}
