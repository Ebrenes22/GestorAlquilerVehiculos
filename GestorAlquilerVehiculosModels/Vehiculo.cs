using System.ComponentModel.DataAnnotations;

namespace GestorAlquilerVehiculos.Models
{
    public class Vehiculo
    {
        public int VehiculoID { get; set; }

        [Required, StringLength(50)]
        public string Marca { get; set; }

        [Required, StringLength(50)]
        public string Modelo { get; set; }

        [Range(1900, 2100)]
        public int Anio { get; set; }

        [Required, StringLength(20)]
        public string Placa { get; set; }

        [Required]
        [RegularExpression("Disponible|Alquilado|En Mantenimiento", ErrorMessage = "Estado inválido")]
        public string Estado { get; set; } = "Disponible";

        [Range(0, 10000)]
        public decimal PrecioPorDia { get; set; }

        [Required]
        public string ImagenURL { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<Reserva> Reservas { get; set; }
        public ICollection<Mantenimiento> Mantenimientos { get; set; }
    }

}
