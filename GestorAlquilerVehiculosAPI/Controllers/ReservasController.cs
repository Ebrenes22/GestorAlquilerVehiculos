using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorAlquilerVehiculosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public ReservasController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            return await _context.Reservas
                .Include(r => r.Vehiculo)
                .Include(r => r.Usuario)
                .Include(r => r.ClienteReserva)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> GetReservaById(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Vehiculo)
                .Include(r => r.Usuario)
                .Include(r => r.ClienteReserva)
                .Include(r => r.CargosAdicionales)
                .Include(r => r.EntregasDevoluciones)
                .FirstOrDefaultAsync(r => r.ReservaID == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return reserva;
        }

        [HttpGet("Usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservasByUsuario(int usuarioId)
        {
            return await _context.Reservas
                .Where(r => r.UsuarioID == usuarioId)
                .Include(r => r.Vehiculo)
                .ToListAsync();
        }

        [HttpGet("Vehiculo/{vehiculoId}/Disponibilidad")]
        public async Task<ActionResult<bool>> GetDisponibilidad(int vehiculoId, DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaInicio >= fechaFin)
            {
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin");
            }

            var vehiculo = await _context.Vehiculos.FindAsync(vehiculoId);
            if (vehiculo == null)
            {
                return NotFound("Vehículo no encontrado");
            }

            bool disponible = await CheckDisponibilidadVehiculo(vehiculoId, fechaInicio, fechaFin);
            return disponible;
        }

        private async Task<bool> CheckDisponibilidadVehiculo(int vehiculoId, DateTime fechaInicio, DateTime fechaFin)
        {
            bool disponible = !await _context.Reservas
                .Where(r => r.VehiculoID == vehiculoId &&
                           r.Estado != "Cancelada" &&
                           ((r.FechaInicio <= fechaInicio && r.FechaFin >= fechaInicio) ||
                            (r.FechaInicio <= fechaFin && r.FechaFin >= fechaFin) ||
                            (r.FechaInicio >= fechaInicio && r.FechaFin <= fechaFin)))
                .AnyAsync();

            return disponible;
        }

        [HttpPost]
        public async Task<ActionResult<Reserva>> AgregarReserva([FromBody] Reserva reservaInput)
        {
            var reserva = new Reserva
            {
                UsuarioID = reservaInput.UsuarioID,
                ClienteReservaID = reservaInput.ClienteReservaID,
                VehiculoID = reservaInput.VehiculoID,
                FechaInicio = reservaInput.FechaInicio,
                FechaFin = reservaInput.FechaFin
            };

            if (reserva.FechaInicio >= reserva.FechaFin)
            {
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin");
            }

            if (reserva.FechaInicio < DateTime.Now)
            {
                return BadRequest("La fecha de inicio no puede ser en el pasado");
            }

            bool disponible = await CheckDisponibilidadVehiculo(reserva.VehiculoID, reserva.FechaInicio, reserva.FechaFin);
            if (!disponible)
            {
                return BadRequest("El vehículo no esta disponible en las fechas seleccionadas");
            }

            var vehiculo = await _context.Vehiculos.FindAsync(reserva.VehiculoID);
            if (vehiculo == null)
            {
                return BadRequest("Vehículo no encontrado");
            }

            if (vehiculo.Estado != "Disponible")
            {
                return BadRequest($"El vehiculo no está disponible actualmente. Estado: {vehiculo.Estado}");
            }

            if (reserva.UsuarioID.HasValue)
            {
                var usuario = await _context.Usuarios.FindAsync(reserva.UsuarioID.Value);
                if (usuario == null)
                {
                    return BadRequest("El usuario especificado no existe");
                }
            }

            if (reserva.ClienteReservaID.HasValue)
            {
                var cliente = await _context.ClientesReserva.FindAsync(reserva.ClienteReservaID.Value);
                if (cliente == null)
                {
                    return BadRequest("El cliente especificado no existe");
                }
            }

            int diasAlquiler = (int)(reserva.FechaFin - reserva.FechaInicio).TotalDays;
            reserva.CostoTotal = vehiculo.PrecioPorDia * diasAlquiler;

            reserva.Estado = "Pendiente";
            reserva.FechaRegistro = DateTime.Now;

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReserva", new { id = reserva.ReservaID }, reserva);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModificarReserva(int id, [FromBody] Reserva reservaInput)
        {
            if (id != reservaInput.ReservaID)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del objeto");
            }

            var reservaExistente = await _context.Reservas.FindAsync(id);
            if (reservaExistente == null)
            {
                return NotFound();
            }

            if (reservaExistente.Estado != "Pendiente" && reservaExistente.Estado != "Confirmada")
            {
                return BadRequest($"No se puede modificar una reserva en estado {reservaExistente.Estado}");
            }

            reservaExistente.UsuarioID = reservaInput.UsuarioID;
            reservaExistente.ClienteReservaID = reservaInput.ClienteReservaID;
            reservaExistente.VehiculoID = reservaInput.VehiculoID;
            reservaExistente.FechaInicio = reservaInput.FechaInicio;
            reservaExistente.FechaFin = reservaInput.FechaFin;

            if (reservaExistente.FechaInicio != reservaInput.FechaInicio ||
                reservaExistente.FechaFin != reservaInput.FechaFin ||
                reservaExistente.VehiculoID != reservaInput.VehiculoID)
            {
                if (reservaInput.FechaInicio >= reservaInput.FechaFin)
                {
                    return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin");
                }

                if (reservaExistente.VehiculoID != reservaInput.VehiculoID)
                {
                    bool disponible = await CheckDisponibilidadVehiculo(reservaInput.VehiculoID, reservaInput.FechaInicio, reservaInput.FechaFin);
                    if (!disponible)
                    {
                        return BadRequest("El vehículo no está disponible en las fechas seleccionadas");
                    }
                }
                else
                {
                    bool disponible = !await _context.Reservas
                        .Where(r => r.VehiculoID == reservaInput.VehiculoID &&
                                r.ReservaID != id &&
                                r.Estado != "Cancelada" &&
                                ((r.FechaInicio <= reservaInput.FechaInicio && r.FechaFin >= reservaInput.FechaInicio) ||
                                (r.FechaInicio <= reservaInput.FechaFin && r.FechaFin >= reservaInput.FechaFin) ||
                                (r.FechaInicio >= reservaInput.FechaInicio && r.FechaFin <= reservaInput.FechaFin)))
                        .AnyAsync();

                    if (!disponible)
                    {
                        return BadRequest("El vehículo no esta disponible en las fechas seleccionadas");
                    }
                }

                var vehiculo = await _context.Vehiculos.FindAsync(reservaInput.VehiculoID);
                if (vehiculo == null)
                {
                    return BadRequest("Vehiculo no encontrado");
                }

                int diasAlquiler = (int)(reservaInput.FechaFin - reservaInput.FechaInicio).TotalDays;
                reservaExistente.CostoTotal = vehiculo.PrecioPorDia * diasAlquiler;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(id))
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

        [HttpPatch("{id}/Estado")]
        public async Task<IActionResult> CambiarEstadoReserva(int id, [FromBody] string nuevoEstado)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r => r.ReservaID == id);

            if (reserva == null)
            {
                return NotFound();
            }

            if (nuevoEstado != "Pendiente" && nuevoEstado != "Confirmada" &&
                nuevoEstado != "Cancelada" && nuevoEstado != "Finalizada")
            {
                return BadRequest("Estado inválido");
            }

            if (!EstadoValido(reserva.Estado, nuevoEstado))
            {
                return BadRequest($"No se puede cambiar el estado de {reserva.Estado} a {nuevoEstado}");
            }

            string estadoAnterior = reserva.Estado;

            reserva.Estado = nuevoEstado;

            if (reserva.Vehiculo != null)
            {
                if (nuevoEstado == "Confirmada")
                {
                    reserva.Vehiculo.Estado = "Alquilado";
                }
                else if ((nuevoEstado == "Cancelada" || nuevoEstado == "Finalizada") &&
                         estadoAnterior == "Confirmada")
                {
                    reserva.Vehiculo.Estado = "Disponible";
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(id))
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
        public async Task<IActionResult> EliminarReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            if (reserva.Estado != "Pendiente")
            {
                return BadRequest($"No se puede eliminar una reserva en estado {reserva.Estado}");
            }

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.ReservaID == id);
        }

        private bool EstadoValido(string estadoActual, string nuevoEstado)
        {
            switch (estadoActual)
            {
                case "Pendiente":
                    return nuevoEstado == "Confirmada" || nuevoEstado == "Cancelada";
                case "Confirmada":
                    return nuevoEstado == "Finalizada" || nuevoEstado == "Cancelada";
                case "Finalizada":
                case "Cancelada":
                    return false; 
                default:
                    return false;
            }
        }
    }
}
