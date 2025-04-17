using System.ComponentModel.DataAnnotations;

namespace GestorAlquilerVehiculos.Models
{
    public class Notificacion
    {
        public int NotificacionID { get; set; }

        public int ReservaID { get; set; }
        public Reserva Reserva { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        public bool Leido { get; set; } = false;
    }

}
