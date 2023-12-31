﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API_Ventas.Models;

namespace API_Ventas.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApiContext _context;

        public ProductosController(ApiContext context)
        {
            _context = context;
        }

        // GET: Productoes
        public async Task<IActionResult> Index()
        {
            return _context.Productos != null ?
                        View(await _context.Productos.ToListAsync()) :
                        Problem("Entity set 'ApiContext.Productos'  is null.");
        }

        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProducto,DescProducto,Precio")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Productoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProducto,DescProducto,Precio")] Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.IdProducto))
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
            return View(producto);
        }

        // GET: Productoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Productos == null)
            {
                return Problem("Entity set 'ApiContext.Productos'  is null.");
            }
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return (_context.Productos?.Any(e => e.IdProducto == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Route("ListarProducto")]
        public IActionResult Listar(int? IdProducto)
        {
            try
            {
                List<Producto> productos = new List<Producto>();

                if (IdProducto.HasValue)
                {
                    productos = _context.Productos.FromSqlRaw("SELECT * FROM PRODUCTO WHERE Id_Producto = {0}", IdProducto.Value).ToList();
                }
                else
                {
                    productos = _context.Productos.FromSqlRaw("SELECT * FROM PRODUCTO").ToList();
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Correcto", respuesta = productos });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Error", respuesta = ex.Message });
            }
        }

        [HttpPost]
        [Route("InsertarProducto")]
        public IActionResult Insertar(string descProducto, int precio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(descProducto))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "La descripción del producto es obligatoria." });
                }

                if (precio <= 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "El precio es obligatorio y debe ser un número." });
                }

                Producto producto = new Producto();
                producto.DescProducto = descProducto;
                producto.Precio = precio;
                _context.Productos.Add(producto);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { respuesta = "Correcto" });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Error", respuesta = ex.Message });
            }
        }
        [HttpDelete]
        [Route("EliminarProducto")]
        public IActionResult Eliminar(int IdProducto)
        {
            try
            {
                if (IdProducto <= 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "El IDProducto es obligatorio y debe ser un número." });
                }

                Producto? prod = _context.Productos.Find(IdProducto);
                if (prod == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Error", respuesta = "El producto con ID: " + IdProducto + ", no se encontró." });
                }
                _context.Productos.Remove(prod);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "Correcto" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { respuesta = "Error", mensaje = ex.Message });
            }
        }
        [HttpPost]
        [Route("EditarProducto")]
        public IActionResult EditarProducto(int IDProducto, string DescProducto, int Precio)
        {
            try
            {
                if (IDProducto <= 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "El IDProducto es obligatorio y debe ser un número." });
                }

                if (string.IsNullOrWhiteSpace(DescProducto))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "La descripción del producto es obligatorio." });
                }

                if (Precio <= 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", respuesta = "El precio es obligatorio y debe ser un número" });
                }

                var existingProduct = _context.Productos.Find(IDProducto);

                if (existingProduct != null)
                {
                    existingProduct.DescProducto = DescProducto;
                    existingProduct.Precio = Precio;

                    _context.Productos.Update(existingProduct);
                    _context.SaveChanges();

                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "Actualizado correctamente" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Error", respuesta = "Producto no encontrado" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { respuesta = "Error", mensaje = ex.Message });
            }
        }
    }
}
