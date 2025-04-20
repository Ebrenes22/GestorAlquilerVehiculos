using System;
using System.Collections.Generic;
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
            var gestorAlquilerVehiculosDbContext = _context.Reservas.Include(r => r.ClienteReserva).Include(r => r.Usuario).Include(r => r.Vehiculo);
            return View(await gestorAlquilerVehiculosDbContext.ToListAsync());
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
                .Include(r => r.Usuario)
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(m => m.ReservaID == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["ClienteReservaID"] = new SelectList(_context.ClientesReserva, "ClienteReservaID", "CorreoElectronico");
            ViewData["UsuarioID"] = new SelectList(_context.Usuarios, "UsuarioID", "ContrasenaHash");
            ViewData["VehiculoID"] = new SelectList(_context.Vehiculos, "VehiculoID", "Estado");
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservaID,UsuarioID,ClienteReservaID,VehiculoID,FechaInicio,FechaFin,CostoTotal,Estado,FechaRegistro")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteReservaID"] = new SelectList(_context.ClientesReserva, "ClienteReservaID", "CorreoElectronico", reserva.ClienteReservaID);
            ViewData["UsuarioID"] = new SelectList(_context.Usuarios, "UsuarioID", "ContrasenaHash", reserva.UsuarioID);
            ViewData["VehiculoID"] = new SelectList(_context.Vehiculos, "VehiculoID", "Estado", reserva.VehiculoID);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        // usando Microsoft.EntityFrameworkCore;

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.ClienteReserva)    // <— aquí
                .Include(r => r.Vehiculo)          // si también necesitas Vehículo
                .FirstOrDefaultAsync(r => r.ReservaID == id);

            if (reserva == null) return NotFound();

            // Prepara sólo el dropdown de Vehículos, pues ClienteReserva no se edita
            ViewBag.VehiculoID = new SelectList(
                _context.Vehiculos,
                "VehiculoID",
                "Marca",
                reserva.VehiculoID);

            return View(reserva);
        }


        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservaID,UsuarioID,ClienteReservaID,VehiculoID,FechaInicio,FechaFin,CostoTotal,Estado,FechaRegistro")] Reserva reserva)
        {
            ModelState.Remove("CargosAdicionales");
            ModelState.Remove("ClienteReserva");
            ModelState.Remove("EntregasDevoluciones");
            ModelState.Remove("Notificaciones");
            ModelState.Remove("Usuario");
            ModelState.Remove("UsuarioID");
            ModelState.Remove("Notificaciones");
            ModelState.Remove("Vehiculo");
            if (id != reserva.ReservaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.ReservaID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteReservaID"] = new SelectList(_context.ClientesReserva, "ClienteReservaID", "CorreoElectronico", reserva.ClienteReservaID);
            ViewData["UsuarioID"] = new SelectList(_context.Usuarios, "UsuarioID", "ContrasenaHash", reserva.UsuarioID);
            ViewData["VehiculoID"] = new SelectList(_context.Vehiculos, "VehiculoID", "Estado", reserva.VehiculoID);
            return View(reserva);
        }



        // POST: Reservas/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
