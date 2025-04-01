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
    public class MantenimientosController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public MantenimientosController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        // GET: Mantenimientoes
        public async Task<IActionResult> Index()
        {
            var gestorAlquilerVehiculosDbContext = _context.Mantenimientos.Include(m => m.Vehiculo);
            return View(await gestorAlquilerVehiculosDbContext.ToListAsync());
        }

        // GET: Mantenimientoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mantenimiento = await _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .FirstOrDefaultAsync(m => m.MantenimientoID == id);
            if (mantenimiento == null)
            {
                return NotFound();
            }

            return View(mantenimiento);
        }

        // GET: Mantenimientoes/Create
        public IActionResult Create()
        {
            ViewData["VehiculoID"] = new SelectList(_context.Vehiculos, "VehiculoID", "Estado");
            return View();
        }

        // POST: Mantenimientoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MantenimientoID,VehiculoID,Descripcion,FechaMantenimiento,Costo,Tipo")] Mantenimiento mantenimiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mantenimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VehiculoID"] = new SelectList(_context.Vehiculos, "VehiculoID", "Estado", mantenimiento.VehiculoID);
            return View(mantenimiento);
        }

        // GET: Mantenimientoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mantenimiento = await _context.Mantenimientos.FindAsync(id);
            if (mantenimiento == null)
            {
                return NotFound();
            }
            ViewData["VehiculoID"] = new SelectList(_context.Vehiculos, "VehiculoID", "Estado", mantenimiento.VehiculoID);
            return View(mantenimiento);
        }

        // POST: Mantenimientoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MantenimientoID,VehiculoID,Descripcion,FechaMantenimiento,Costo,Tipo")] Mantenimiento mantenimiento)
        {
            if (id != mantenimiento.MantenimientoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mantenimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MantenimientoExists(mantenimiento.MantenimientoID))
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
            ViewData["VehiculoID"] = new SelectList(_context.Vehiculos, "VehiculoID", "Estado", mantenimiento.VehiculoID);
            return View(mantenimiento);
        }

        // GET: Mantenimientoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mantenimiento = await _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .FirstOrDefaultAsync(m => m.MantenimientoID == id);
            if (mantenimiento == null)
            {
                return NotFound();
            }

            return View(mantenimiento);
        }

        // POST: Mantenimientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mantenimiento = await _context.Mantenimientos.FindAsync(id);
            if (mantenimiento != null)
            {
                _context.Mantenimientos.Remove(mantenimiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MantenimientoExists(int id)
        {
            return _context.Mantenimientos.Any(e => e.MantenimientoID == id);
        }
    }
}
