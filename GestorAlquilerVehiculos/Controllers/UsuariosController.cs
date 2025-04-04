﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Models;

namespace GestorAlquilerVehiculos.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public UsuariosController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            // Mostrar alerta si viene de TempData
            ViewBag.Success = TempData["Success"]?.ToString();
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioID == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreCompleto,CorreoElectronico,ContrasenaHash,Rol")] Usuario usuario)
        {
            // Validación de correo único
            if (await _context.Usuarios.AnyAsync(u => u.CorreoElectronico == usuario.CorreoElectronico))
            {
                ModelState.AddModelError("CorreoElectronico", "El correo electrónico ya está registrado.");
            }

            // Validación de nombre completo único
            if (await _context.Usuarios.AnyAsync(u => u.NombreCompleto == usuario.NombreCompleto))
            {
                ModelState.AddModelError("NombreCompleto", "El nombre de usuario ya existe.");
            }

            if (ModelState.IsValid)
            {
                usuario.FechaRegistro = DateTime.Now;
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // Enviar mensaje de éxito
                TempData["Success"] = "Usuario creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.UsuarioID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioExistente = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.UsuarioID == id);
                    if (usuarioExistente == null)
                        return NotFound();

                    // Mantener los campos que no queremos editar
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

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioID == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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