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
    public class ClienteReservasController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public ClienteReservasController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        // GET: ClienteReservas
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClientesReserva.ToListAsync());
        }

        // GET: ClienteReservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clienteReserva = await _context.ClientesReserva
                .FirstOrDefaultAsync(m => m.ClienteReservaID == id);
            if (clienteReserva == null)
            {
                return NotFound();
            }

            return View(clienteReserva);
        }

        // GET: ClienteReservas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClienteReservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClienteReservaID,NombreCompleto,Identificacion,CorreoElectronico,Direccion,Telefono")] ClienteReserva clienteReserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clienteReserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clienteReserva);
        }

        // GET: ClienteReservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clienteReserva = await _context.ClientesReserva.FindAsync(id);
            if (clienteReserva == null)
            {
                return NotFound();
            }
            return View(clienteReserva);
        }

        // POST: ClienteReservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClienteReservaID,NombreCompleto,Identificacion,CorreoElectronico,Direccion,Telefono")] ClienteReserva clienteReserva)
        {
            if (id != clienteReserva.ClienteReservaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clienteReserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteReservaExists(clienteReserva.ClienteReservaID))
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
            return View(clienteReserva);
        }

        // GET: ClienteReservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clienteReserva = await _context.ClientesReserva
                .FirstOrDefaultAsync(m => m.ClienteReservaID == id);
            if (clienteReserva == null)
            {
                return NotFound();
            }

            return View(clienteReserva);
        }

        // POST: ClienteReservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clienteReserva = await _context.ClientesReserva.FindAsync(id);
            if (clienteReserva != null)
            {
                _context.ClientesReserva.Remove(clienteReserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteReservaExists(int id)
        {
            return _context.ClientesReserva.Any(e => e.ClienteReservaID == id);
        }
    }
}
