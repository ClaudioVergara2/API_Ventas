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
    public class VentumsController : Controller
    {
        private readonly ApiContext _context;

        public VentumsController(ApiContext context)
        {
            _context = context;
        }

        // GET: Ventums
        public async Task<IActionResult> Index()
        {
            var apiContext = _context.Venta.Include(v => v.IdProductoNavigation).Include(v => v.NomUsuarioNavigation);
            return View(await apiContext.ToListAsync());
        }

        // GET: Ventums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Venta == null)
            {
                return NotFound();
            }

            var ventum = await _context.Venta
                .Include(v => v.IdProductoNavigation)
                .Include(v => v.NomUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdVenta == id);
            if (ventum == null)
            {
                return NotFound();
            }

            return View(ventum);
        }

        // GET: Ventums/Create
        public IActionResult Create()
        {
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto");
            ViewData["NomUsuario"] = new SelectList(_context.Usuarios, "NomUsuario", "NomUsuario");
            return View();
        }

        // POST: Ventums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVenta,IdProducto,NomUsuario,Cantidad,Total,FechaVenta,Estado")] Ventum ventum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ventum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", ventum.IdProducto);
            ViewData["NomUsuario"] = new SelectList(_context.Usuarios, "NomUsuario", "NomUsuario", ventum.NomUsuario);
            return View(ventum);
        }

        // GET: Ventums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Venta == null)
            {
                return NotFound();
            }

            var ventum = await _context.Venta.FindAsync(id);
            if (ventum == null)
            {
                return NotFound();
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", ventum.IdProducto);
            ViewData["NomUsuario"] = new SelectList(_context.Usuarios, "NomUsuario", "NomUsuario", ventum.NomUsuario);
            return View(ventum);
        }

        // POST: Ventums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVenta,IdProducto,NomUsuario,Cantidad,Total,FechaVenta,Estado")] Ventum ventum)
        {
            if (id != ventum.IdVenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ventum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentumExists(ventum.IdVenta))
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
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", ventum.IdProducto);
            ViewData["NomUsuario"] = new SelectList(_context.Usuarios, "NomUsuario", "NomUsuario", ventum.NomUsuario);
            return View(ventum);
        }

        // GET: Ventums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Venta == null)
            {
                return NotFound();
            }

            var ventum = await _context.Venta
                .Include(v => v.IdProductoNavigation)
                .Include(v => v.NomUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdVenta == id);
            if (ventum == null)
            {
                return NotFound();
            }

            return View(ventum);
        }

        // POST: Ventums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Venta == null)
            {
                return Problem("Entity set 'ApiContext.Venta'  is null.");
            }
            var ventum = await _context.Venta.FindAsync(id);
            if (ventum != null)
            {
                _context.Venta.Remove(ventum);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentumExists(int id)
        {
          return (_context.Venta?.Any(e => e.IdVenta == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Route("ListarVentas")]
        public IActionResult ListarUsuario()
        {
            try
            {
                List<Ventum> venta = new List<Ventum>();
                venta = _context.Venta.FromSqlRaw("SELECT * FROM VENTA").ToList();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Correcto", respuesta = venta });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Error", respuesta = ex.Message });
            }
        }

        [HttpPost]
        [Route("CrearVenta")]
        public IActionResult CrearVenta(string NomUsuario, int IDProducto, int Cantidad)
        {
            try
            {
                var existingUsuario = _context.Usuarios.FirstOrDefault(u => u.NomUsuario == NomUsuario);
                var existingProducto = _context.Productos.Find(IDProducto);

                if (existingUsuario == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Error", respuesta = "Usuario no encontrado" });
                }

                if (existingUsuario.Estado == 0) // 0 representa usuario inhabilitado
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "Usuario inhabilitado, imposible crear venta" });
                }

                // Verificar si el usuario ya ha realizado una venta
                var ventaExistente = _context.Venta.FirstOrDefault(v => v.NomUsuario == existingUsuario.NomUsuario);

                if (ventaExistente != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "El usuario ya ha realizado una venta" });
                }

                if (existingProducto == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Error", respuesta = "Producto no encontrado" });
                }

                // Crear una nueva venta
                var nuevaVenta = new Ventum
                {
                    IdProducto = existingProducto.IdProducto,
                    NomUsuario = existingUsuario.NomUsuario,
                    Cantidad = Cantidad,
                    FechaVenta = DateTime.Now,
                    Total = existingProducto.Precio * Cantidad,
                    Estado = 1
                };

                _context.Venta.Add(nuevaVenta);
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "Venta creada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { respuesta = "Error", mensaje = ex.Message });
            }
        }

        [HttpPost]
        [Route("EditarVenta")]
        public IActionResult EditarVenta(string nomUsuario, int idProducto, int cantidad, int estado)
        {
            try
            {
                // Buscar la venta por el nombre de usuario y otras propiedades, cargando ansiosamente la propiedad IdProductoNavigation
                var existingVenta = _context.Venta
                    .Include(v => v.IdProductoNavigation)
                    .FirstOrDefault(v => v.NomUsuario == nomUsuario && v.IdProducto == idProducto);

                if (existingVenta != null)
                {
                    // Actualizar la cantidad
                    existingVenta.Cantidad = cantidad;
                    existingVenta.Estado = estado;

                    // Recalcular el total si IdProductoNavigation no es nulo
                    if (existingVenta.IdProductoNavigation != null)
                    {
                        existingVenta.Total = cantidad * existingVenta.IdProductoNavigation.Precio;
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error", respuesta = "No se pudo recalcular el total debido a un producto nulo" });
                    }

                    _context.Venta.Update(existingVenta);
                    _context.SaveChanges();

                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "Venta actualizada correctamente" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Error", respuesta = "Venta no encontrada" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { respuesta = "Error", mensaje = ex.Message });
            }
        }

        [HttpDelete]
        [Route("EliminarVenta")]
        public IActionResult EliminarVenta(int IDVenta)
        {
            try
            {
                var venta = _context.Venta.Find(IDVenta);

                if (venta != null)
                {
                    _context.Venta.Remove(venta);
                    _context.SaveChanges();

                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "Venta eliminada correctamente" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Error", respuesta = "Venta no encontrada" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { respuesta = "Error", mensaje = ex.Message });
            }
        }
    }
}
