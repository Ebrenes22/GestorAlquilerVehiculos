using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorAlquilerVehiculos.Controllers
{
    public class VehiculosController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public VehiculosController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Vehiculos.ToListAsync());
        }

        public async Task<IActionResult> Listado()
        {
            return View(await _context.Vehiculos.ToListAsync());
        }

        // GET: Vehiculoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(m => m.VehiculoID == id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            return View(vehiculo);
        }

        public async Task<IActionResult> DetailsVehiculo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(m => m.VehiculoID == id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            return View(vehiculo);
        }

        // GET: Vehiculoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vehiculoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehiculoID,Marca,Modelo,Anio,Placa,Estado,PrecioPorDia,ImagenURL")] Vehiculo vehiculo)
        {

            ModelState.Remove("Reservas");
            ModelState.Remove("Mantenimientos");

            if (VehiculoExists(vehiculo.Placa))
            {
                ViewBag.errorPlaca = "Ya existe un vehiculo con esa placa.";
                return View(vehiculo);
            }

            if (ModelState.IsValid)
            {
                _context.Add(vehiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehiculo);
        }

        // GET: Vehiculoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }
            return View(vehiculo);
        }

        // POST: Vehiculoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VehiculoID,Marca,Modelo,Anio,Placa,Estado,PrecioPorDia,ImagenURL,FechaRegistro")] Vehiculo vehiculo)
        {
            if (id != vehiculo.VehiculoID)
            {
                return NotFound();
            }

            if (VehiculoExistsDiffId(vehiculo.Placa, vehiculo.VehiculoID))
            {
                ViewBag.errorPlaca = "Ya existe otro vehiculo con esa placa.";
                return View(vehiculo);
            }

            ModelState.Remove("Reservas");
            ModelState.Remove("Mantenimientos");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehiculoExists(vehiculo.Placa))
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
            return View(vehiculo);
        }

        // GET: Vehiculoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(m => m.VehiculoID == id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            return View(vehiculo);
        }

        // POST: Vehiculoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo != null)
            {
                _context.Vehiculos.Remove(vehiculo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehiculoExists(string placa)
        {
            return _context.Vehiculos.Any(e => e.Placa == placa);
        }

        private bool VehiculoExistsDiffId(string placa, int id)
        {
            return _context.Vehiculos.Any(e => e.Placa == placa && e.VehiculoID != id);
        }
    }
}
