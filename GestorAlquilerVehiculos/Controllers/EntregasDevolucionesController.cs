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
using GestorAlquilerVehiculos.Services;

namespace GestorAlquilerVehiculos.Controllers
{
    public class EntregasDevolucionesController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;
        private readonly IEmailService _emailService;

        public EntregasDevolucionesController(GestorAlquilerVehiculosDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
        public async Task<IActionResult> Create([Bind("ReservaID,EstadoInicial,EstadoFinal,CargosAdicionales,FechaEntrega,FechaDevolucion")] EntregaDevolucion entregaDevolucion)
        {
            ModelState.Remove("Reserva");
            if (ModelState.IsValid)
            {
                _context.Add(entregaDevolucion);
                await _context.SaveChangesAsync();

                var reserva = await _context.Reservas
                    .Include(r => r.ClienteReserva)
                    .Include(r => r.Vehiculo)
                    .FirstOrDefaultAsync(r => r.ReservaID == entregaDevolucion.ReservaID);

                if (reserva?.ClienteReserva?.CorreoElectronico != null)
                {
                    string subject = $"Confirmación de devolución - Reserva #{reserva.ReservaID}";
                    string body = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        h1 {{ color: #2d6a4f; }}
        .details {{ margin-top: 20px; }}
        .details p {{ margin: 5px 0; }}
        .footer {{ margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>¡Gracias por devolver el vehículo!</h1>
        <p>Estimado/a {reserva.ClienteReserva.NombreCompleto},</p>
        <p>Hemos registrado la devolución del vehículo asociado a su reserva <strong>#{reserva.ReservaID}</strong>.</p>

        <div class='details'>
            <p><strong>Vehículo:</strong> {reserva.Vehiculo.Marca} {reserva.Vehiculo.Modelo}</p>
            <p><strong>Fecha de Entrega:</strong> {entregaDevolucion.FechaEntrega:dd/MM/yyyy}</p>
            <p><strong>Fecha de Devolución:</strong> {entregaDevolucion.FechaDevolucion:dd/MM/yyyy}</p>
            <p><strong>Estado Final:</strong> {entregaDevolucion.EstadoFinal}</p>
        </div>

        <p>Gracias por confiar en nuestro servicio. Esperamos verlo/a de nuevo pronto.</p>

        <div class='footer'>
            <p>Gestor de Alquiler de Vehículos</p>
            <p>Este es un mensaje automático. No responda a este correo.</p>
        </div>
    </div>
</body>
</html>";

                    try
                    {
                        await _emailService.SendEmailAsync(reserva.ClienteReserva.CorreoElectronico, subject, body);
                        Console.WriteLine($"[INFO] Correo de devolución enviado a {reserva.ClienteReserva.CorreoElectronico}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Falló el envío de correo: {ex.Message}");
                    }
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
