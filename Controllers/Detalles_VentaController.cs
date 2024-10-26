using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UspgPOS.Data;
using UspgPOS.Models;

namespace UspgPOS.Controllers
{
    public class Detalles_VentaController : Controller
    {
        private readonly AppDbContext _context;

        public Detalles_VentaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Detalles_Venta
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Detalles_Venta.Include(d => d.Producto).Include(d => d.Venta);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Detalles_Venta/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalles_Venta = await _context.Detalles_Venta
                .Include(d => d.Producto)
                .Include(d => d.Venta)
                .FirstOrDefaultAsync(m => m.Ventaid == id);
            if (detalles_Venta == null)
            {
                return NotFound();
            }

            return View(detalles_Venta);

        }

        // GET: Detalles_Venta/Create
        public IActionResult Create()
        {
            ViewData["Productoid"] = new SelectList(_context.Productos, "Id", "Nombre");
            ViewData["Ventaid"] = new SelectList(_context.Ventas, "Id", "Id");
            return View();
        }

        // POST: Detalles_Venta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Precio,Cantidad,Productoid,Ventaid")] Detalles_Venta detalles_Venta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detalles_Venta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Productoid"] = new SelectList(_context.Productos, "Id", "Nombre", detalles_Venta.Productoid);
            ViewData["Ventaid"] = new SelectList(_context.Ventas, "Id", "Id", detalles_Venta.Ventaid);
            return View(detalles_Venta);
        }

        // GET: Detalles_Venta/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalles_Venta = await _context.Detalles_Venta.FindAsync(id);
            if (detalles_Venta == null)
            {
                return NotFound();
            }
            ViewData["Productoid"] = new SelectList(_context.Productos, "Id", "Nombre", detalles_Venta.Productoid);
            ViewData["Ventaid"] = new SelectList(_context.Ventas, "Id", "Id", detalles_Venta.Ventaid);
            return View(detalles_Venta);
        }

        // POST: Detalles_Venta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Precio,Cantidad,Productoid,Ventaid")] Detalles_Venta detalles_Venta)
        {
            if (id != detalles_Venta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detalles_Venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Detalles_VentaExists(detalles_Venta.Id))
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
            ViewData["Productoid"] = new SelectList(_context.Productos, "Id", "Nombre", detalles_Venta.Productoid);
            ViewData["Ventaid"] = new SelectList(_context.Ventas, "Id", "Id", detalles_Venta.Ventaid);
            return View(detalles_Venta);
        }

        // GET: Detalles_Venta/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalles_Venta = await _context.Detalles_Venta
                .Include(d => d.Producto)
                .Include(d => d.Venta)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detalles_Venta == null)
            {
                return NotFound();
            }

            return View(detalles_Venta);
        }

        // POST: Detalles_Venta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var detalles_Venta = await _context.Detalles_Venta.FindAsync(id);
            if (detalles_Venta != null)
            {
                _context.Detalles_Venta.Remove(detalles_Venta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Detalles_VentaExists(long id)
        {
            return _context.Detalles_Venta.Any(e => e.Id == id);
        }
    }
}
