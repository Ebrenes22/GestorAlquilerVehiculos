using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorAlquilerVehiculosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MantenimientoController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public MantenimientoController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        // GET: api/Mantenimiento
        [HttpGet]
        public ActionResult<IEnumerable<Mantenimiento>> GetMantenimientos()
        {
            return _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .ToList();
        }

        // GET: api/Mantenimiento/5
        [HttpGet("{id}")]
        public ActionResult<Mantenimiento> GetMantenimiento(int id)
        {
            var mantenimiento = _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .FirstOrDefault(m => m.MantenimientoID == id);

            if (mantenimiento == null)
            {
                return NotFound();
            }

            return mantenimiento;
        }

        // GET: api/Mantenimiento/vehiculo/5
        [HttpGet("vehiculo/{vehiculoId}")]
        public ActionResult<IEnumerable<Mantenimiento>> GetMantenimientosPorVehiculo(int vehiculoId)
        {
            var mantenimientos = _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .Where(m => m.VehiculoID == vehiculoId)
                .OrderByDescending(m => m.FechaMantenimiento)
                .ToList();

            if (mantenimientos == null || !mantenimientos.Any())
            {
                return NotFound($"No se encontraron mantenimientos para el vehículo con ID {vehiculoId}");
            }

            return mantenimientos;
        }

        [HttpGet("tipo/{tipo}")]
        public ActionResult<IEnumerable<Mantenimiento>> GetMantenimientosPorTipo(string tipo)
        {
            if (tipo != "Preventivo" && tipo != "Correctivo")
            {
                return BadRequest("El tipo de mantenimiento debe ser 'Preventivo' o 'Correctivo'");
            }

            var mantenimientos = _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .Where(m => m.Tipo == tipo)
                .OrderByDescending(m => m.FechaMantenimiento)
                .ToList();

            return mantenimientos;
        }

        // POST: api/Mantenimiento
        [HttpPost]
        public ActionResult<Mantenimiento> AgregarMantenimiento(Mantenimiento mantenimiento)
        {
            mantenimiento.FechaMantenimiento = DateTime.Now;

            _context.Mantenimientos.Add(mantenimiento);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetMantenimiento), new { id = mantenimiento.MantenimientoID }, mantenimiento);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMantenimiento(int id, Mantenimiento mantenimiento)
        {
            if (id != mantenimiento.MantenimientoID)
            {
                return BadRequest();
            }

            var existingMantenimiento = _context.Mantenimientos.Find(id);
            if (existingMantenimiento == null)
            {
                return NotFound();
            }

            _context.Entry(existingMantenimiento).CurrentValues.SetValues(mantenimiento);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarMantenimiento(int id)
        {
            var mantenimiento = _context.Mantenimientos.Find(id);
            if (mantenimiento == null)
            {
                return NotFound();
            }

            _context.Mantenimientos.Remove(mantenimiento);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/Mantenimiento/resumen
        [HttpGet("resumen")]
        public ActionResult<object> GetResumenMantenimientos()
        {
            var totalMantenimientos = _context.Mantenimientos.Count();
            var totalPreventivos = _context.Mantenimientos.Count(m => m.Tipo == "Preventivo");
            var totalCorrectivos = _context.Mantenimientos.Count(m => m.Tipo == "Correctivo");
            var costoTotal = _context.Mantenimientos.Sum(m => m.Costo);
            var costoPromedio = totalMantenimientos > 0 ? _context.Mantenimientos.Average(m => m.Costo) : 0;

            var mantenimientosPorVehiculo = _context.Mantenimientos
                .GroupBy(m => m.VehiculoID)
                .Select(g => new
                {
                    VehiculoID = g.Key,
                    CantidadMantenimientos = g.Count(),
                    CostoTotal = g.Sum(m => m.Costo)
                })
                .OrderByDescending(x => x.CantidadMantenimientos)
                .ToList();

            var resumen = new
            {
                TotalMantenimientos = totalMantenimientos,
                TotalPreventivos = totalPreventivos,
                TotalCorrectivos = totalCorrectivos,
                CostoTotal = costoTotal,
                CostoPromedio = costoPromedio,
                MantenimientosPorVehiculo = mantenimientosPorVehiculo
            };

            return resumen;
        }

    }
}
