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
        [RegularExpression("Administrador|Empleado", ErrorMessage = "Rol inválido")]
        public string Rol { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Nullables
        public virtual ICollection<Reserva>? Reservas { get; set; }
        public virtual ICollection<Reporte>? Reportes { get; set; }
    }
}