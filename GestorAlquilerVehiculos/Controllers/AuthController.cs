﻿using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Models;
using GestorAlquilerVehiculos.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorAlquilerVehiculos.Controllers
{
    public class AuthController : Controller
    {
        private readonly GestorAlquilerVehiculosDbContext _context;

        public AuthController(GestorAlquilerVehiculosDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hashed = PasswordHasher.HashPassword(model.Contrasena);
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == model.CorreoElectronico && u.ContrasenaHash == hashed);

            if (usuario != null)
            {
                TempData["Rol"] = usuario.Rol;
                TempData["Nombre"] = usuario.NombreCompleto;
                TempData["Success"] = $"Bienvenido, {usuario.NombreCompleto}";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View(model);
        }

        // GET: /Auth/CambiarContrasena
        public IActionResult CambiarContrasena()
        {
            return View();
        }

        // POST: /Auth/CambiarContrasena
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContrasena(string correo, string nuevaContrasena)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.CorreoElectronico == correo);
            if (usuario != null)
            {
                usuario.ContrasenaHash = PasswordHasher.HashPassword(nuevaContrasena);
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Contraseña actualizada correctamente.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "El correo ingresado no está registrado.";
            return View();
        }
    }
}
