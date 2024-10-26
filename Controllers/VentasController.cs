using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UspgPOS.Data;
using UspgPOS.Models;

namespace UspgPOS.Controllers
{
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Ventas.Include(v => v.Cliente).Include(v => v.Sucursal);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Sucursal)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venta == null)
            {
                return NotFound();
            }

            return RedirectToAction("Details", "Detalles_venta", new { id = venta.Id });
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nit");
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Id");
            return View();
        }

        // POST: Ventas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fecha,Total,ClienteId,SucursalId")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venta);
                await _context.SaveChangesAsync();

                // Aquí creas el detalle de venta
                var detalleVenta = new Detalles_Venta
                {
                    Ventaid = venta.Id ?? 0, 
                    Precio = venta.Total, 
                    Cantidad = 1, 
                    Productoid = 1 
                };

                _context.Detalles_Venta.Add(detalleVenta);
                await _context.SaveChangesAsync(); // Guarda el detalle de venta

                return RedirectToAction(nameof(Index));

            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nit", venta.ClienteId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Id", venta.SucursalId);
            return View(venta);
        }

        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nit", venta.ClienteId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Id", venta.SucursalId);
            return View(venta);
        }

        // POST: Ventas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("Id,Fecha,Total,ClienteId,SucursalId")] Venta venta)
        {
            if (id != venta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nit", venta.ClienteId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Id", venta.SucursalId);
            return View(venta);
        }

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalles = await _context.Detalles_Venta.Where(d => d.Ventaid == id).ToListAsync();

            _context.Detalles_Venta.RemoveRange(detalles);
            await _context.SaveChangesAsync();

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Sucursal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta != null)
            {
                _context.Ventas.Remove(venta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentaExists(long? id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }
    }
}
