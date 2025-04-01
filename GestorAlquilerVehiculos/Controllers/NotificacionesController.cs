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
    public class NotificacionesController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public NotificacionesController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        // GET: Notificacions
        public async Task<IActionResult> Index()
        {
            var gestorAlquilerVehiculosDbContext = _context.Notificaciones.Include(n => n.Reserva);
            return View(await gestorAlquilerVehiculosDbContext.ToListAsync());
        }

        // GET: Notificacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificaciones
                .Include(n => n.Reserva)
                .FirstOrDefaultAsync(m => m.NotificacionID == id);
            if (notificacion == null)
            {
                return NotFound();
            }

            return View(notificacion);
        }

        // GET: Notificacions/Create
        public IActionResult Create()
        {
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado");
            return View();
        }

        // POST: Notificacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotificacionID,ReservaID,Mensaje,FechaEnvio,Leido")] Notificacion notificacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notificacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado", notificacion.ReservaID);
            return View(notificacion);
        }

        // GET: Notificacions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null)
            {
                return NotFound();
            }
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado", notificacion.ReservaID);
            return View(notificacion);
        }

        // POST: Notificacions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NotificacionID,ReservaID,Mensaje,FechaEnvio,Leido")] Notificacion notificacion)
        {
            if (id != notificacion.NotificacionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notificacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificacionExists(notificacion.NotificacionID))
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
            ViewData["ReservaID"] = new SelectList(_context.Reservas, "ReservaID", "Estado", notificacion.ReservaID);
            return View(notificacion);
        }

        // GET: Notificacions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificaciones
                .Include(n => n.Reserva)
                .FirstOrDefaultAsync(m => m.NotificacionID == id);
            if (notificacion == null)
            {
                return NotFound();
            }

            return View(notificacion);
        }

        // POST: Notificacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion != null)
            {
                _context.Notificaciones.Remove(notificacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificacionExists(int id)
        {
            return _context.Notificaciones.Any(e => e.NotificacionID == id);
        }
    }
}
