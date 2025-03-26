using System.ComponentModel.DataAnnotations;

namespace GestorAlquilerVehiculos.Models
{
    public class EntregaDevolucion
    {
        public int EntregaDevolucionID { get; set; }

        public int ReservaID { get; set; }
        public Reserva Reserva { get; set; }

        [Required]
        public string EstadoInicial { get; set; }

        [Required]
        public string EstadoFinal { get; set; }

        [Range(0, 10000)]
        public decimal CargosAdicionales { get; set; } = 0;

        public DateTime FechaEntrega { get; set; } = DateTime.Now;

        public DateTime? FechaDevolucion { get; set; }
    }

}
