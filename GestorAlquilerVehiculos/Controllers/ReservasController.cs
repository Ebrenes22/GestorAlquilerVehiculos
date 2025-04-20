using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Models;

namespace GestorAlquilerVehiculos.Controllers
{
    public class ReservasController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public ReservasController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var reservas = await _context.Reservas
                .Include(r => r.ClienteReserva)
                .Include(r => r.Vehiculo)
                .ToListAsync();
            return View(reservas);
        }

        // GET: Reservas/Create
        public IActionResult Create(int clienteReservaID)
        {
            var cliente = _context.ClientesReserva
                                 .FirstOrDefault(c => c.ClienteReservaID == clienteReservaID);
            if (cliente == null)
                return NotFound();

            ViewBag.NombreCliente = cliente.NombreCompleto;
            ViewBag.ClienteReservaID = clienteReservaID;

            ViewData["VehiculoID"] = new SelectList(
                _context.Vehiculos.Select(v => new {
                    v.VehiculoID,
                    Nombre = v.Marca + " " + v.Modelo + " - ₡" + v.PrecioPorDia
                }),
                "VehiculoID", "Nombre");

            ViewData["VehiculosInfo"] = _context.Vehiculos
                .ToDictionary(v => v.VehiculoID, v => v.PrecioPorDia);
            var modelo = new Reserva
            {
                ClienteReservaID = clienteReservaID,
                Estado = "Pendiente",
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now,
                FechaRegistro = DateTime.Now
            };
            return View(modelo);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva)
        {

            
            reserva.Estado = "Pendiente";
            reserva.FechaRegistro = DateTime.Now;

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Reserva creada correctamente.";

            return RedirectToAction("Details", "Reservas", new { id = reserva.ReservaID });
        }


        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.ClienteReserva)
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r => r.ReservaID == id);

            if (reserva == null) return NotFound();

            ViewData["VehiculoID"] = new SelectList(_context.Vehiculos, "VehiculoID", "Marca", reserva.VehiculoID);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservaID,ClienteReservaID,VehiculoID,FechaInicio,FechaFin,CostoTotal,Estado,FechaRegistro")] Reserva reserva)
        {
            if (id != reserva.ReservaID) return NotFound();

            ModelState.Remove("CargosAdicionales");
            ModelState.Remove("ClienteReserva");
            ModelState.Remove("EntregasDevoluciones");
            ModelState.Remove("Notificaciones");
            ModelState.Remove("Usuario");
            ModelState.Remove("UsuarioID");
            ModelState.Remove("Notificaciones");
            ModelState.Remove("Vehiculo");

            if (!ModelState.IsValid)
            {
                ViewData["VehiculoID"] = new SelectList(_context.Vehiculos, "VehiculoID", "Marca", reserva.VehiculoID);
                return View(reserva);
            }

            try
            {
                _context.Update(reserva);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reserva actualizada correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(reserva.ReservaID))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Reservas/DeleteConfirmed/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reserva eliminada correctamente.";
            }
            else
            {
                TempData["Error"] = "No se encontró la reserva.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.ClienteReserva)
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r => r.ReservaID == id.Value);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }


        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.ReservaID == id);
        }
    }
}
