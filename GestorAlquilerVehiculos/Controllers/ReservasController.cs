using System;
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
            var reservas = _context.Reservas
                .Include(r => r.ClienteReserva)
                .Include(r => r.Usuario)
                .Include(r => r.Vehiculo);
            return View(await reservas.ToListAsync());
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            // Vehículos: para mostrar en el dropdown Marca + Modelo + PrecioPorDia
            ViewData["VehiculoID"] = new SelectList(
                _context.Vehiculos.Select(v => new
                {
                    v.VehiculoID,
                    Nombre = v.Marca + " " + v.Modelo + " - ₡" + v.PrecioPorDia + "/día"
                }),
                "VehiculoID",
                "Nombre");

            // Diccionario para JavaScript (VehiculoID -> PrecioPorDia)
            ViewData["VehiculosInfo"] = _context.Vehiculos
                .ToDictionary(v => v.VehiculoID, v => v.PrecioPorDia);

            // Simulamos cliente autenticado (puede cambiar esto si luego autentican)
            var cliente = _context.ClientesReserva.FirstOrDefault();
            if (cliente != null)
            {
                ViewBag.ClienteReservaID = cliente.ClienteReservaID;
                ViewBag.CorreoElectronico = cliente.CorreoElectronico;
            }

            return View();
        }

        // POST: Reservas/Create
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

            // Recargar valores si hay error
            ViewData["VehiculoID"] = new SelectList(
                _context.Vehiculos.Select(v => new
                {
                    v.VehiculoID,
                    Nombre = v.Marca + " " + v.Modelo + " - ₡" + v.PrecioPorDia + "/día"
                }),
                "VehiculoID",
                "Nombre");

            ViewData["VehiculosInfo"] = _context.Vehiculos
                .ToDictionary(v => v.VehiculoID, v => v.PrecioPorDia);

            return View(reserva);
        }
    }
}