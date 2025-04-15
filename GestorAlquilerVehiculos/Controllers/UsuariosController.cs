using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Models;
using GestorAlquilerVehiculos.Utils; // 🔐 Asegúrate de tener este using para usar PasswordHasher

namespace GestorAlquilerVehiculos.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public UsuariosController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Success = TempData["Success"]?.ToString();
            ViewBag.Error = TempData["Error"]?.ToString();
            ViewBag.NoPermisos = TempData["NoPermisos"]?.ToString();
            TempData.Keep("Rol");
            return View(await _context.Usuarios.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            TempData.Keep("Rol");
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioID == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        public IActionResult Create()
        {
            if (TempData["Rol"]?.ToString() != "Administrador")
            {
                TempData["Error"] = "Acceso no autorizado.";
                return RedirectToAction(nameof(Index));
            }

            TempData.Keep("Rol");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreCompleto,CorreoElectronico,ContrasenaHash,Rol")] Usuario usuario)
        {
            if (TempData["Rol"]?.ToString() != "Administrador")
            {
                TempData["Error"] = "Acceso no autorizado.";
                return RedirectToAction(nameof(Index));
            }

            TempData.Keep("Rol");

            if (await _context.Usuarios.AnyAsync(u => u.CorreoElectronico == usuario.CorreoElectronico))
                ModelState.AddModelError("CorreoElectronico", "El correo electrónico ya está registrado.");

            if (await _context.Usuarios.AnyAsync(u => u.NombreCompleto == usuario.NombreCompleto))
                ModelState.AddModelError("NombreCompleto", "El nombre de usuario ya existe.");

            if (ModelState.IsValid)
            {
                usuario.FechaRegistro = DateTime.Now;

                // 🔐 HASHEAR CONTRASEÑA antes de guardar
                usuario.ContrasenaHash = PasswordHasher.HashPassword(usuario.ContrasenaHash);

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usuario creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (TempData["Rol"]?.ToString() != "Administrador")
            {
                TempData["NoPermisos"] = "Usted no tiene permisos para editar usuarios.";
                return RedirectToAction(nameof(Index));
            }

            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            TempData.Keep("Rol");
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (TempData["Rol"]?.ToString() != "Administrador")
            {
                TempData["Error"] = "Acceso no autorizado.";
                return RedirectToAction(nameof(Index));
            }

            TempData.Keep("Rol");

            if (id != usuario.UsuarioID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioExistente = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.UsuarioID == id);
                    if (usuarioExistente == null) return NotFound();

                    usuario.ContrasenaHash = usuarioExistente.ContrasenaHash;
                    usuario.FechaRegistro = usuarioExistente.FechaRegistro;

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Usuario actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioID)) return NotFound();
                    throw;
                }
            }

            return View(usuario);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (TempData["Rol"]?.ToString() != "Administrador")
            {
                TempData["Error"] = "Acceso no autorizado.";
                return RedirectToAction(nameof(Index));
            }

            TempData.Keep("Rol");

            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioID == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (TempData["Rol"]?.ToString() != "Administrador")
            {
                TempData["Error"] = "Acceso no autorizado.";
                return RedirectToAction(nameof(Index));
            }

            TempData.Keep("Rol");

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
                _context.Usuarios.Remove(usuario);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioID == id);
        }
    }
}