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
        public IActionResult Create()
        {
            ViewData["VehiculoID"] = new SelectList(
                _context.Vehiculos.Select(v => new {
                    v.VehiculoID,
                    Nombre = v.Marca + " " + v.Modelo + " - ₡" + v.PrecioPorDia
                }),
                "VehiculoID", "Nombre");
            // Suponiendo que el cliente ya está definido en sesión o similar
            //ViewBag.ClienteReservaID  /* obtén ID del cliente actual */
            return View();
        }

        // POST: Reservas/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservaID,ClienteReservaID,VehiculoID,FechaInicio,FechaFin,CostoTotal,Estado,FechaRegistro")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repoblar dropdown si hay error
            ViewData["VehiculoID"] = new SelectList(
                _context.Vehiculos.Select(v => new {
                    v.VehiculoID,
                    Nombre = v.Marca + " " + v.Modelo + " - ₡" + v.PrecioPorDia
                }),
                "VehiculoID", "Nombre", reserva.VehiculoID);
            ViewBag.ClienteReservaID = reserva.ClienteReservaID;
            return View(reserva);
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

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.ReservaID == id);
        }
    }
}
