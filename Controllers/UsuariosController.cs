﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API_Ventas.Models;
using System.Collections;

namespace API_Ventas.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApiContext _context;

        public UsuariosController(ApiContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
              return _context.Usuarios != null ? 
                          View(await _context.Usuarios.ToListAsync()) :
                          Problem("Entity set 'ApiContext.Usuarios'  is null.");
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.NomUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomUsuario,Password,Estado")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NomUsuario,Password,Estado")] Usuario usuario)
        {
            if (id != usuario.NomUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.NomUsuario))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.NomUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'ApiContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(string id)
        {
          return (_context.Usuarios?.Any(e => e.NomUsuario == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Route("ListarUsuario")]
        public IActionResult ListarUsuario()
        {
            try
            {
                List<Usuario> usuarios = _context.Usuarios.ToList();

                var usuariosViewModel = usuarios.Select(usuario => new
                {
                    NomUsuario = usuario.NomUsuario,
                    Estado = usuario.Estado == 1 ? "Habilitado" : "Inhabilitado"
                }).ToList();

                return Ok(new { mensaje = "Correcto", respuesta = usuariosViewModel });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error", respuesta = ex.Message });
            }
        }

        [HttpPost]
        [Route("InsertarUsuario")]
        public IActionResult InsertarUsuario(string nombre, string password, int estado)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "El nombre de usuario es obligatorios." });
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "La contraseña es obligatorios." });
                }

                if (estado != 0 && estado != 1)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "El campo estado debe ser 0 o 1." });
                }

                if (_context.Usuarios.Any(u => u.NomUsuario == nombre))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "El nombre de usuario " + nombre + " ya existe." });
                }
                Usuario usuario = new Usuario
                {
                    NomUsuario = nombre,
                    Password = password,
                    Estado = estado
                };
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { respuesta = "Correcto" });
            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { respuesta = "Error", mensaje = ex.Message });
            }
        }
        [HttpDelete]
        [Route("EliminarUsuario")]
        public IActionResult Eliminar(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "El nombre de usuario es obligatorio." });
                }
                Usuario? us = _context.Usuarios.Find(nombre);
                if (us == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Error", respuesta = "El usuario con nombre " + nombre + " no se encontró." });
                }
                _context.Usuarios.Remove(us);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "Correcto" });
            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { respuesta = "Error", mensaje = ex.Message });
            }
        }
        [HttpPost]
        [Route("EditarUsuario")]
        public IActionResult EditarUsuario(string nombre, string password, int estado)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "El nombre de usuario es obligatorio." });
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "La contraseña es obligatoria." });
                }
                if (estado != 0 && estado != 1)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "El campo estado debe ser 0 o 1." });
                }
                var existingUsuario = _context.Usuarios.Find(nombre);
                if (existingUsuario != null)
                {
                    existingUsuario.Password = password;
                    existingUsuario.Estado = estado;
                    _context.Usuarios.Update(existingUsuario);
                    _context.SaveChanges();
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "Actualizado correctamente" });
                }else
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Error", respuesta = "Usuario no encontrado" });
                }
            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { respuesta = "Error", mensaje = ex.Message });
            }
        }

    }
}
