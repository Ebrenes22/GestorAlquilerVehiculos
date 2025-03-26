using System.ComponentModel.DataAnnotations;

namespace GestorAlquilerVehiculos.Models
{
    public class ClienteReserva
    {
        public int ClienteReservaID { get; set; }

        [Required, StringLength(100)]
        public string NombreCompleto { get; set; }

        [Required, StringLength(20)]
        public string Identificacion { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string CorreoElectronico { get; set; }

        [Required, StringLength(100)]
        public string Direccion { get; set; }

        [Required, Phone, StringLength(20)]
        public string Telefono { get; set; }

        public ICollection<Reserva> Reservas { get; set; }
    }


}
