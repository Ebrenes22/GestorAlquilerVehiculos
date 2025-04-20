using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            var clientes = await _context.ClientesReserva.ToListAsync();
            return View(clientes);
        }

        // GET: ClienteReservas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClienteReservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreCompleto,Identificacion,CorreoElectronico,Direccion,Telefono")] ClienteReserva clienteReserva)
        {
            if (!ModelState.IsValid)
            {
                return View(clienteReserva);
            }

            try
            {
                _context.Add(clienteReserva);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar el cliente: " + ex.Message);
                return View(clienteReserva);
            }
        }

        // GET: ClienteReservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var clienteReserva = await _context.ClientesReserva.FindAsync(id);
            if (clienteReserva == null) return NotFound();

            return View(clienteReserva);
        }

        // POST: ClienteReservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClienteReservaID,NombreCompleto,Identificacion,CorreoElectronico,Direccion,Telefono")] ClienteReserva clienteReserva)
        {
            if (id != clienteReserva.ClienteReservaID) return NotFound();

            if (!ModelState.IsValid) return View(clienteReserva);

            try
            {
                _context.Update(clienteReserva);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ClientesReserva.Any(e => e.ClienteReservaID == id))
                    return NotFound();

                throw;
            }
        }

        // GET: ClienteReservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var clienteReserva = await _context.ClientesReserva
                .FirstOrDefaultAsync(m => m.ClienteReservaID == id);
            if (clienteReserva == null) return NotFound();

            return View(clienteReserva);
        }

        // GET: ClienteReservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var clienteReserva = await _context.ClientesReserva
                .FirstOrDefaultAsync(m => m.ClienteReservaID == id);
            if (clienteReserva == null) return NotFound();

            return View(clienteReserva);
        }

        // POST: ClienteReservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.ClientesReserva.FindAsync(id);
            if (cliente != null)
            {
                _context.ClientesReserva.Remove(cliente);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente eliminado.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}