using System.ComponentModel.DataAnnotations;

namespace GestorAlquilerVehiculos.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }

        [Required, StringLength(100)]
        public string NombreCompleto { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string CorreoElectronico { get; set; }

        [Required]
        public string ContrasenaHash { get; set; }

        [Required]
        [RegularExpression("Cliente|Administrador", ErrorMessage = "Rol inválido")]
        public string Rol { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<Reserva> Reservas { get; set; }
        public ICollection<Reporte> Reportes { get; set; }
    }

}