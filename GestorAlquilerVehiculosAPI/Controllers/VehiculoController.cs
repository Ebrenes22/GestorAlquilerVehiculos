using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace GestorAlquilerVehiculosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculosController : ControllerBase
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public VehiculosController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculos()
        {
            return await _context.Vehiculos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vehiculo>> GetVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);

            if (vehiculo == null)
            {
                return NotFound();
            }

            return vehiculo;
        }

        [HttpGet("Placa/{placa}")]
        public async Task<ActionResult<Vehiculo>> GetVehiculoByPlaca(string placa)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa == placa);

            if (vehiculo == null)
            {
                return NotFound();
            }

            return vehiculo;
        }

        [HttpGet("Disponibles")]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculosDisponibles()
        {
            return await _context.Vehiculos
                .Where(v => v.Estado == "Disponible")
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Vehiculo>> PostVehiculo(Vehiculo vehiculo)
        {
            vehiculo.VehiculoID = 0;

            vehiculo.FechaRegistro = DateTime.Now;

            vehiculo.Reservas = null;
            vehiculo.Mantenimientos = null;

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVehiculo), new { id = vehiculo.VehiculoID }, vehiculo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehiculo(int id, Vehiculo vehiculo)
        {
            vehiculo.VehiculoID = id;

            var originalVehiculo = await _context.Vehiculos.FindAsync(id);
            if (originalVehiculo == null)
            {
                return NotFound();
            }

            vehiculo.FechaRegistro = originalVehiculo.FechaRegistro;

            vehiculo.Reservas = null;
            vehiculo.Mantenimientos = null;

            _context.Entry(originalVehiculo).State = EntityState.Detached;

            _context.Entry(vehiculo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/CambiarEstado")]
        public async Task<IActionResult> CambiarEstadoVehiculo(int id, [FromBody] string nuevoEstado)
        {
            if (nuevoEstado != "Disponible" && nuevoEstado != "Alquilado" && nuevoEstado != "En Mantenimiento")
            {
                return BadRequest("Estado inválido");
            }

            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            vehiculo.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("Buscar")]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> BuscarVehiculos([FromQuery] string marca = null, [FromQuery] string modelo = null, [FromQuery] int? anio = null)
        {
            IQueryable<Vehiculo> query = _context.Vehiculos;

            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(v => EF.Functions.Like(v.Marca.ToLower(), $"%{marca.ToLower()}%"));
            }

            if (!string.IsNullOrEmpty(modelo))
            {
                query = query.Where(v => EF.Functions.Like(v.Modelo.ToLower(), $"%{modelo.ToLower()}%"));
            }

            if (anio.HasValue)
            {
                query = query.Where(v => v.Anio == anio.Value);
            }

            return await query.ToListAsync();
        }

        private bool VehiculoExists(int id)
        {
            return _context.Vehiculos.Any(e => e.VehiculoID == id);
        }
    }
}

