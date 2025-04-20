using System.ComponentModel.DataAnnotations;

namespace GestorAlquilerVehiculos.Models
{
    public class Reserva
    {
        public int ReservaID { get; set; }

        public int? UsuarioID { get; set; }
        public Usuario Usuario { get; set; }

        public int? ClienteReservaID { get; set; }
        public ClienteReserva ClienteReserva { get; set; }

        public int VehiculoID { get; set; }
        public Vehiculo Vehiculo { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public decimal CostoTotal { get; set; }

        [Required]
        [RegularExpression("Pendiente|Confirmada|Cancelada|Finalizada", ErrorMessage = "Estado inválido")]
        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<Notificacion> Notificaciones { get; set; }
        public ICollection<EntregaDevolucion> EntregasDevoluciones { get; set; }
        public ICollection<CargoAdicional> CargosAdicionales { get; set; }
    }


}
