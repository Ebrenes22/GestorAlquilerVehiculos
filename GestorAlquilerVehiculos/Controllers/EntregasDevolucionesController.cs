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
    public class EntregasDevolucionesController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public EntregasDevolucionesController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        // GET: EntregaDevolucions
        public async Task<IActionResult> Index()
        {
            var gestorAlquilerVehiculosDbContext = _context.EntregasDevoluciones.Include(e => e.Reserva);
            return View(await gestorAlquilerVehiculosDbContext.ToListAsync());
        }

        // GET: EntregaDevolucions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entregaDevolucion = await _context.EntregasDevoluciones
                .Include(e => e.Reserva)
                .FirstOrDefaultAsync(m => m.EntregaDevolucionID == id);
            if (entregaDevolucion == null)
            {
                return NotFound();
            }

            return View(entregaDevolucion);
        }

        // GET: EntregaDevolucions/Create
        public IActionResult Create()
        {
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado");
            return View();
        }

        // POST: EntregaDevolucions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntregaDevolucionID,ReservaID,EstadoInicial,EstadoFinal,CargosAdicionales,FechaEntrega,FechaDevolucion")] EntregaDevolucion entregaDevolucion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entregaDevolucion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado", entregaDevolucion.ReservaID);
            return View(entregaDevolucion);
        }

        // GET: EntregaDevolucions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entregaDevolucion = await _context.EntregasDevoluciones.FindAsync(id);
            if (entregaDevolucion == null)
            {
                return NotFound();
            }
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado", entregaDevolucion.ReservaID);
            return View(entregaDevolucion);
        }

        // POST: EntregaDevolucions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EntregaDevolucionID,ReservaID,EstadoInicial,EstadoFinal,CargosAdicionales,FechaEntrega,FechaDevolucion")] EntregaDevolucion entregaDevolucion)
        {
            if (id != entregaDevolucion.EntregaDevolucionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entregaDevolucion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntregaDevolucionExists(entregaDevolucion.EntregaDevolucionID))
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
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado", entregaDevolucion.ReservaID);
            return View(entregaDevolucion);
        }

        // GET: EntregaDevolucions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entregaDevolucion = await _context.EntregasDevoluciones
                .Include(e => e.Reserva)
                .FirstOrDefaultAsync(m => m.EntregaDevolucionID == id);
            if (entregaDevolucion == null)
            {
                return NotFound();
            }

            return View(entregaDevolucion);
        }

        // POST: EntregaDevolucions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entregaDevolucion = await _context.EntregasDevoluciones.FindAsync(id);
            if (entregaDevolucion != null)
            {
                _context.EntregasDevoluciones.Remove(entregaDevolucion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntregaDevolucionExists(int id)
        {
            return _context.EntregasDevoluciones.Any(e => e.EntregaDevolucionID == id);
        }
    }
}
