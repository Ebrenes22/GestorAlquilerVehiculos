using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Models;
using System.Text.Json;

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
            var entregas = await _context.EntregasDevoluciones
                .Include(e => e.Reserva)
                    .ThenInclude(r => r.ClienteReserva)
                .ToListAsync();
            return View(entregas);
        }


        // GET: EntregaDevolucions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var entregaDevolucion = await _context.EntregasDevoluciones
                .Include(e => e.Reserva)
                    .ThenInclude(r => r.ClienteReserva)
                .Include(e => e.Reserva)
                    .ThenInclude(r => r.Vehiculo)
                .FirstOrDefaultAsync(m => m.EntregaDevolucionID == id);

            if (entregaDevolucion == null)
                return NotFound();

            return View(entregaDevolucion);
        }

        #region CREATE ENTREGAS/DEVOLUCIONES
        // GET: EntregaDevolucions/Create
        public IActionResult Create()
        {
 
            var reservas = _context.Reservas
                .Include(r => r.ClienteReserva)
                .Select(r => new {
                    Id = r.ReservaID,
                    Nombre = r.ClienteReserva.NombreCompleto,
                    EstadoInicial = r.Estado,        // <— aquí
                    FechaEntrega = r.FechaInicio
                })
                .ToList();

            ViewBag.Reservas = new SelectList(reservas, "Id", "Nombre");
            ViewBag.ReservasJson = JsonSerializer.Serialize(reservas);

            var estados = new[] { "Pendiente", "Confirmada", "Cancelada", "Finalizada" };
            ViewBag.EstadoInicialList =
                estados.Select(e => new SelectListItem(e, e))
                       .ToList();

            return View();
        }


        // POST: EntregaDevolucions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservaID,EstadoInicial,EstadoFinal,CargosAdicionales,FechaEntrega,FechaDevolucion")]EntregaDevolucion entregaDevolucion)
        {
            ModelState.Remove("Reserva");
            if (ModelState.IsValid)
            {
                _context.Add(entregaDevolucion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var reservas = _context.Reservas
                .Include(r => r.ClienteReserva)
                .Select(r => new {
                    Id = r.ReservaID,
                    Nombre = r.ClienteReserva.NombreCompleto,
                    EstadoInicial = r.Estado,    
                    FechaEntrega = r.FechaInicio
                })
                .ToList();

            ViewBag.Reservas = new SelectList(reservas, "Id", "Nombre", entregaDevolucion.ReservaID);
            ViewBag.ReservasJson = JsonSerializer.Serialize(reservas);

            var estados = new[] { "Pendiente", "Confirmada", "Cancelada", "Finalizada" };
            ViewBag.EstadoInicialList =
                estados.Select(e => new SelectListItem(e, e, e == entregaDevolucion.EstadoInicial))
                       .ToList();

            return View(entregaDevolucion);
        }

        #endregion


        #region EDIT ENTREGAS/DEVOLUCIONES
        // GET: EntregaDevolucions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Cargo la entidad con la Reserva y Cliente
            var entrega = await _context.EntregasDevoluciones
                .Include(e => e.Reserva)
                    .ThenInclude(r => r.ClienteReserva)
                .FirstOrDefaultAsync(e => e.EntregaDevolucionID == id);

            if (entrega == null) return NotFound();

            // Lista de reservas con Nombre, EstadoInicial y FechaEntrega
            var reservas = _context.Reservas
                .Include(r => r.ClienteReserva)
                .Select(r => new {
                    Id = r.ReservaID,
                    Nombre = r.ClienteReserva.NombreCompleto,
                    EstadoInicial = r.Estado,
                    FechaEntrega = r.FechaInicio
                })
                .ToList();

            ViewBag.Reservas = new SelectList(reservas, "Id", "Nombre", entrega.ReservaID);
            ViewBag.ReservasJson = JsonSerializer.Serialize(reservas);

            // Opciones de EstadoInicial
            var estados = new[] { "Pendiente", "Confirmada", "Cancelada", "Finalizada" };
            ViewBag.EstadoInicialList =
                estados.Select(e => new SelectListItem(e, e, e == entrega.EstadoInicial))
                       .ToList();

            return View(entrega);
        }

        // POST: EntregaDevolucions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("EntregaDevolucionID,ReservaID,EstadoInicial,EstadoFinal,CargosAdicionales,FechaEntrega,FechaDevolucion")]
        EntregaDevolucion entregaDevolucion)
        {
            if (id != entregaDevolucion.EntregaDevolucionID)
                return NotFound();

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
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }


            var reservas = _context.Reservas
                .Include(r => r.ClienteReserva)
                .Select(r => new {
                    Id = r.ReservaID,
                    Nombre = r.ClienteReserva.NombreCompleto,
                    EstadoInicial = r.Estado,
                    FechaEntrega = r.FechaInicio
                })
                .ToList();
            ViewBag.Reservas = new SelectList(reservas, "Id", "Nombre", entregaDevolucion.ReservaID);
            ViewBag.ReservasJson = JsonSerializer.Serialize(reservas);

            var estados = new[] { "Pendiente", "Confirmada", "Cancelada", "Finalizada" };
            ViewBag.EstadoInicialList =
                estados.Select(e => new SelectListItem(e, e, e == entregaDevolucion.EstadoInicial))
                       .ToList();

            return View(entregaDevolucion);
        }

        #endregion

        // POST: EntregaDevolucions/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entregaDevolucion = await _context.EntregasDevoluciones.FindAsync(id);
            if (entregaDevolucion != null)
            {
                _context.EntregasDevoluciones.Remove(entregaDevolucion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        private bool EntregaDevolucionExists(int id)
        {
            return _context.EntregasDevoluciones.Any(e => e.EntregaDevolucionID == id);
        }
    }
}
