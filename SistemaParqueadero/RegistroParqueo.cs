using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaParqueadero.Models
{
    public class RegistroParqueo
    {
        [Key]
        public int RegistroId { get; set; }

        // INGRESO
        [Required]
        public int VehiculoId { get; set; }

        [Required]
        public TipoTarifa TipoTarifa { get; set; }

        [Required]
        public DateTime FechaHoraIngreso { get; set; }

        public int? UsuarioIngresoId { get; set; }

        [StringLength(500)]
        public string? ObservacionesIngreso { get; set; }

        // SALIDA
        public DateTime? FechaHoraSalida { get; set; }

        public int? UsuarioSalidaId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? HorasCalculadas { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MontoCobrado { get; set; }

        [StringLength(50)]
        public string? MetodoPago { get; set; } // "Efectivo", "Tarjeta", "Transferencia"

        [StringLength(500)]
        public string? ObservacionesSalida { get; set; }

        // ESTADO
        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Activo"; // "Activo" (dentro), "Cerrado" (salió)

        // RELACIONES
        [ForeignKey("VehiculoId")]
        public virtual Vehiculo Vehiculo { get; set; }

        [ForeignKey("UsuarioIngresoId")]
        public virtual Usuario1? UsuarioIngreso { get; set; }

        [ForeignKey("UsuarioSalidaId")]
        public virtual Usuario1? UsuarioSalida { get; set; }

        // MÉTODOS CALCULADOS
        [NotMapped]
        public bool EstaActivo => Estado == "Activo";

        [NotMapped]
        public decimal HorasTranscurridas
        {
            get
            {
                if (FechaHoraSalida.HasValue)
                    return (decimal)(FechaHoraSalida.Value - FechaHoraIngreso).TotalHours;
                else
                    return (decimal)(DateTime.UtcNow - FechaHoraIngreso).TotalHours;
            }
        }
    }
}
