using System.ComponentModel.DataAnnotations;

namespace GestorAlquilerVehiculos.Models
{
    public class Reporte
    {
        public int ReporteID { get; set; }

        public int UsuarioID { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        [RegularExpression("Ingresos|Uso de Vehículos|Costos Operativos", ErrorMessage = "Tipo de reporte inválido")]
        public string TipoReporte { get; set; }

        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    }


}
