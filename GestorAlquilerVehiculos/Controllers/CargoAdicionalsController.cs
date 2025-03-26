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
    public class CargoAdicionalsController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public CargoAdicionalsController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        // GET: CargoAdicionals
        public async Task<IActionResult> Index()
        {
            var gestorAlquilerVehiculosDbContext = _context.CargosAdicionales.Include(c => c.Reserva);
            return View(await gestorAlquilerVehiculosDbContext.ToListAsync());
        }

        // GET: CargoAdicionals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargoAdicional = await _context.CargosAdicionales
                .Include(c => c.Reserva)
                .FirstOrDefaultAsync(m => m.CargoID == id);
            if (cargoAdicional == null)
            {
                return NotFound();
            }

            return View(cargoAdicional);
        }

        // GET: CargoAdicionals/Create
        public IActionResult Create()
        {
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado");
            return View();
        }

        // POST: CargoAdicionals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CargoID,ReservaID,Descripcion,Monto")] CargoAdicional cargoAdicional)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cargoAdicional);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado", cargoAdicional.ReservaID);
            return View(cargoAdicional);
        }

        // GET: CargoAdicionals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargoAdicional = await _context.CargosAdicionales.FindAsync(id);
            if (cargoAdicional == null)
            {
                return NotFound();
            }
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado", cargoAdicional.ReservaID);
            return View(cargoAdicional);
        }

        // POST: CargoAdicionals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CargoID,ReservaID,Descripcion,Monto")] CargoAdicional cargoAdicional)
        {
            if (id != cargoAdicional.CargoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cargoAdicional);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CargoAdicionalExists(cargoAdicional.CargoID))
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
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado", cargoAdicional.ReservaID);
            return View(cargoAdicional);
        }

        // GET: CargoAdicionals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargoAdicional = await _context.CargosAdicionales
                .Include(c => c.Reserva)
                .FirstOrDefaultAsync(m => m.CargoID == id);
            if (cargoAdicional == null)
            {
                return NotFound();
            }

            return View(cargoAdicional);
        }

        // POST: CargoAdicionals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cargoAdicional = await _context.CargosAdicionales.FindAsync(id);
            if (cargoAdicional != null)
            {
                _context.CargosAdicionales.Remove(cargoAdicional);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CargoAdicionalExists(int id)
        {
            return _context.CargosAdicionales.Any(e => e.CargoID == id);
        }
    }
}
