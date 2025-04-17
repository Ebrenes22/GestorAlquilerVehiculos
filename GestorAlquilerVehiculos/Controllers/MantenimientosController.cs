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

        #region CREATE MANTENIMIENTOS
        // GET: Mantenimientoes/Create
        public IActionResult Create()
        {
            ViewData["VehiculoID"] = _context.Vehiculos
                .Where(v => v.Estado == "Disponible")
                .Select(v => new SelectListItem
                {
                    Value = v.VehiculoID.ToString(),
                    Text = $"{v.Marca} {v.Modelo} ({v.Placa})"
                }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehiculoID,Descripcion,FechaMantenimiento,Costo,Tipo")] Mantenimiento mantenimiento)
        {
            ModelState.Remove("Vehiculo");
            ModelState.Remove("MantenimientoID");
            if (ModelState.IsValid)
            {
                // Insertar el mantenimiento
                _context.Mantenimientos.Add(mantenimiento);
                await _context.SaveChangesAsync(); 
                var vehiculo = await _context.Vehiculos.FindAsync(mantenimiento.VehiculoID);
                if (vehiculo != null)
                {
                    vehiculo.Estado = "En Mantenimiento";
                    _context.Vehiculos.Update(vehiculo);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["VehiculoID"] = _context.Vehiculos
                .Where(v => v.Estado == "Disponible")
                .Select(v => new SelectListItem
                {
                    Value = v.VehiculoID.ToString(),
                    Text = $"{v.Marca} {v.Modelo} ({v.Placa})"
                }).ToList();

            return View(mantenimiento);
        }
        #endregion

        #region EDIT MANTENIMIENTOS

        // GET: Mantenimientoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewBag.NombreVehiculo = $"{mantenimiento.Vehiculo.Marca} {mantenimiento.Vehiculo.Modelo} ({mantenimiento.Vehiculo.Placa})";

            return View(mantenimiento);
        }

        // POST: Mantenimientoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MantenimientoID,VehiculoID,Descripcion,FechaMantenimiento,Costo,Tipo")] Mantenimiento mantenimiento)
        {
            ModelState.Remove("Vehiculo");
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

            // En caso de error de validación, recargamos el nombre del vehículo
            ViewBag.NombreVehiculo = _context.Vehiculos
                .Where(v => v.VehiculoID == mantenimiento.VehiculoID)
                .Select(v => $"{v.Marca} {v.Modelo} ({v.Placa})")
                .FirstOrDefault();

            return View(mantenimiento);
        }

        #endregion

        #region DELETE MANTENIMIENTOS
       
        // POST: Mantenimientoes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mantenimiento = await _context.Mantenimientos.FindAsync(id);
            if (mantenimiento != null)
            {
                _context.Mantenimientos.Remove(mantenimiento);
                await _context.SaveChangesAsync();
                ViewBag.Success = "Mantenimiento eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }


        #endregion

        private bool MantenimientoExists(int id)
        {
            return _context.Mantenimientos.Any(e => e.MantenimientoID == id);
        }
    }
}
