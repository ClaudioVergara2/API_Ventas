using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API_Ventas.Models;

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
        [Route("Listar")]
        public IActionResult ListarUsuario()
        {
            try
            {
                List<Usuario> usuario = new List<Usuario>();
                _context.Usuarios.ToList();
                return StatusCode(StatusCodes.Status200OK, new { respuesta = "Correcto", mensaje = usuario });
            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { respuesta = "Error", mensaje = ex.Message });
            }
        }
        [HttpPost]
        [Route("Insertar")]
        public IActionResult InsetarUsuario(string nombre, string password)
        {
            try
            {
                Usuario usuarios = new Usuario();
                usuarios.NomUsuario = nombre;
                usuarios.Password = password;
                _context.Usuarios.Add(usuarios);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { respuesta = "Correcto" });
            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { respuesta = "Error", mensaje = ex.Message });
            }
        }
        [HttpPost]
        [Route("Eliminar")]
        public IActionResult Eliminar(string nombre)
        {
            try
            {
                Usuario? us = _context.Usuarios.Find(nombre);
                _context.Usuarios.Remove(us);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "Correcto" });
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { respuesta = "Error", mensaje = ex.Message });
            }
        }
        //FALTA EDITAR
    }
}
