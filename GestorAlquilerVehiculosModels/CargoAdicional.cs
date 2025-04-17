using System.ComponentModel.DataAnnotations;

namespace GestorAlquilerVehiculos.Models
{
    public class CargoAdicional
    {
        [Key]
        public int CargoID { get; set; }

        [Required]
        public int ReservaID { get; set; }

        [Required]
        public required Reserva Reserva { get; set; }

        [Required]
        public string? Descripcion { get; set; }

        [Range(0, 10000)]
        public decimal Monto { get; set; }
    }

}
