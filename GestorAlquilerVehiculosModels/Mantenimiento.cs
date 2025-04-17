using System.ComponentModel.DataAnnotations;

namespace GestorAlquilerVehiculos.Models
{
    public class Mantenimiento
    {
        public int MantenimientoID { get; set; }

        public int VehiculoID { get; set; }
        public virtual Vehiculo Vehiculo { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public DateTime FechaMantenimiento { get; set; }

        [Range(0, 1000000)]
        public decimal Costo { get; set; }


        [Required]
        [RegularExpression("Preventivo|Correctivo", ErrorMessage = "Tipo inválido")]
        public string Tipo { get; set; }
    }


}
