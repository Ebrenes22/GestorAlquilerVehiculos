using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Models;
using GestorAlquilerVehiculos.Services;
using System.Text.Json;

namespace GestorAlquilerVehiculos.Controllers
{
    public class ReservasController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;
        private readonly IEmailService _emailService;

        public ReservasController(GestorAlquilerVehiculosDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
        public IActionResult Create(int clienteReservaID)
        {
            var cliente = _context.ClientesReserva
                                 .FirstOrDefault(c => c.ClienteReservaID == clienteReservaID);
            if (cliente == null)
                return NotFound();

            ViewBag.NombreCliente = cliente.NombreCompleto;
            ViewBag.ClienteReservaID = clienteReservaID;

            ViewData["VehiculoID"] = new SelectList(
                _context.Vehiculos.Select(v => new
                {
                    v.VehiculoID,
                    Nombre = v.Marca + " " + v.Modelo + " - ₡" + v.PrecioPorDia
                }),
                "VehiculoID", "Nombre");

            ViewData["VehiculosImagen"] = _context.Vehiculos
                .ToDictionary(v => v.VehiculoID, v => v.ImagenURL);


            ViewData["VehiculosInfo"] = _context.Vehiculos
                .ToDictionary(v => v.VehiculoID, v => v.PrecioPorDia);

            var reservasExistentes = _context.Reservas
                .Where(r => r.Estado != "Cancelada")
                .Select(r => new {
                    r.VehiculoID,
                    r.FechaInicio,
                    r.FechaFin
                })
                .ToList();

            ViewData["ReservasExistentes"] = JsonSerializer.Serialize(reservasExistentes);

            var modelo = new Reserva
            {
                ClienteReservaID = clienteReservaID,
                Estado = "Pendiente",
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now,
                FechaRegistro = DateTime.Now
            };
            return View(modelo);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva)
        {
            bool vehiculoDisponible = await VerificarDisponibilidadVehiculo(reserva.VehiculoID, reserva.FechaInicio, reserva.FechaFin);

            if (!vehiculoDisponible)
            {
                ModelState.AddModelError("", "El vehículo seleccionado no está disponible para las fechas elegidas.");

                var cliente = _context.ClientesReserva
                                    .FirstOrDefault(c => c.ClienteReservaID == reserva.ClienteReservaID);

                ViewBag.NombreCliente = cliente?.NombreCompleto;
                ViewBag.ClienteReservaID = reserva.ClienteReservaID;

                ViewData["VehiculoID"] = new SelectList(
                    _context.Vehiculos.Select(v => new
                    {
                        v.VehiculoID,
                        Nombre = v.Marca + " " + v.Modelo + " - ₡" + v.PrecioPorDia
                    }),
                    "VehiculoID", "Nombre", reserva.VehiculoID);

                ViewData["VehiculosImagen"] = _context.Vehiculos
                    .ToDictionary(v => v.VehiculoID, v => v.ImagenURL);

                ViewData["VehiculosInfo"] = _context.Vehiculos
                    .ToDictionary(v => v.VehiculoID, v => v.PrecioPorDia);

                var reservasExistentes = _context.Reservas
                    .Where(r => r.Estado != "Cancelada")
                    .Select(r => new {
                        r.VehiculoID,
                        r.FechaInicio,
                        r.FechaFin
                    })
                    .ToList();

                ViewData["ReservasExistentes"] = JsonSerializer.Serialize(reservasExistentes);

                return View(reserva);
            }

            reserva.Estado = "Pendiente";
            reserva.FechaRegistro = DateTime.Now;

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            var clienteInfo = await _context.ClientesReserva.FindAsync(reserva.ClienteReservaID);
            var vehiculo = await _context.Vehiculos.FindAsync(reserva.VehiculoID);

            if (clienteInfo != null && vehiculo != null)
            {
                string subject = $"Confirmación de Reserva #{reserva.ReservaID}";
                string body = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    h1 {{ color: #333366; }}
                    .details {{ margin-top: 20px; }}
                    .details p {{ margin: 5px 0; }}
                    .footer {{ margin-top: 30px; font-size: 12px; color: #666; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>¡Reserva Confirmada!</h1>
                    <p>Estimado/a {clienteInfo.NombreCompleto},</p>
                    <p>Le confirmamos que hemos recibido su reserva con los siguientes detalles:</p>
                    
                    <div class='details'>
                        <p><strong>Número de Reserva:</strong> #{reserva.ReservaID}</p>
                        <p><strong>Vehículo:</strong> {vehiculo.Marca} {vehiculo.Modelo}</p>
                        <p><strong>Fecha de Inicio:</strong> {reserva.FechaInicio.ToString("dd/MM/yyyy")}</p>
                        <p><strong>Fecha de Fin:</strong> {reserva.FechaFin.ToString("dd/MM/yyyy")}</p>
                        <p><strong>Costo Total:</strong> ₡{reserva.CostoTotal}</p>
                        <p><strong>Estado:</strong> {reserva.Estado}</p>
                    </div>
                    
                    <p>Si tiene alguna pregunta o necesita realizar cambios en su reserva, por favor contáctenos.</p>
                    
                    <p>¡Gracias por confiar en nosotros!</p>
                    
                    <div class='footer'>
                        <p>Gestor de Alquiler de Vehículos</p>
                        <p>Este es un correo automático, por favor no responda a este mensaje.</p>
                    </div>
                </div>
            </body>
            </html>";

                try
                {
                    await _emailService.SendEmailAsync(clienteInfo.CorreoElectronico, subject, body);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar email: {ex.Message}");
                }
            }

            TempData["Success"] = "Reserva creada correctamente.";
            return RedirectToAction("Details", "Reservas", new { id = reserva.ReservaID });
        }

        private async Task<bool> VerificarDisponibilidadVehiculo(int vehiculoID, DateTime fechaInicio, DateTime fechaFin)
        {
            var reservasExistentes = await _context.Reservas
                .Where(r => r.VehiculoID == vehiculoID &&
                            r.Estado != "Cancelada" &&
                           (r.FechaInicio < fechaFin && r.FechaFin > fechaInicio))
                .AnyAsync();

            return !reservasExistentes;
        }

        [HttpGet]
        public async Task<IActionResult> VerificarDisponibilidad(int vehiculoID, DateTime fechaInicio, DateTime fechaFin)
        {
            bool disponible = await VerificarDisponibilidadVehiculo(vehiculoID, fechaInicio, fechaFin);
            return Json(new { disponible });
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

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.ClienteReserva)
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r => r.ReservaID == id.Value);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }


        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.ReservaID == id);
        }
    }
}