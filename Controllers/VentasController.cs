using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Packaging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UspgPOS.Data;
using UspgPOS.Models;
using LicenseType = QuestPDF.Infrastructure.LicenseType;

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


        public IActionResult ImprimirFactura(long id)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var venta = _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Sucursal)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefault(v => v.Id == id);

            if (venta == null)
            {
                return NotFound();
            }

            var logo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/icons/apple-touch-icon-180x180.png");
            var monedaGuatemala = new System.Globalization.CultureInfo("es-GT");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    
                    // --- Encabezado ---
                    page.Header().Column(header =>
                    {
                        // Primera fila: Fecha
                        header.Item().Row(row =>
                        {
                            // Fecha en la esquina superior izquierda
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Background("#d9e1f2").Padding(5).Border(1).Text("FECHA").FontSize(10).Bold();
                                col.Item().Border(1).Padding(5).Height(15).Text(venta.Fecha.ToString("dd/MM/yyyy"));
                            });

                            // Logo en la esquina superior derecha (agrandado)
                            row.RelativeItem().AlignRight().Column(col =>
                            {
                                col.Item().Height(90).Width(90).Image(logo, ImageScaling.FitHeight);
                            });
                        });

                        // Segunda fila: Título centrado y más grande
                        header.Item().PaddingTop(10).AlignCenter().Text("FACTURA COMERCIAL").FontSize(30).Bold();
                    });
                

                    // --- Contenido principal ---
                    page.Content().PaddingVertical(10).Column(content =>
                    {
                        // Información del cliente
                        content.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Padding(5);
                                col.Item().Text($"Nombre: {venta.Cliente?.Nombre ?? "N/A"}").FontSize(14).Bold();
                                col.Item().Padding(5);
                                col.Item().Text($"Dirección: {venta.Cliente?.Correo ?? "N/A"}").FontSize(14);
                            });

                            row.RelativeItem().AlignRight().Column(col =>
                            {
                                col.Item().Padding(5);
                                col.Item().Text($"Teléfono: {venta.Cliente?.Nit ?? "N/A"}").FontSize(14);
                            });
                        });

                        // Tabla de detalles
                        content.Item().PaddingVertical(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);  // Cantidad
                                columns.RelativeColumn();   // Descripción
                                columns.ConstantColumn(80); // Unidad
                                columns.ConstantColumn(80); // Total
                            });

                            // Encabezado de tabla
                            table.Header(header =>
                            {
                                header.Cell().Background("#d9e1f2").Border(1).Padding(5).AlignCenter().Text("Cantidad").FontSize(12).Bold();
                                header.Cell().Background("#d9e1f2").Border(1).Padding(5).AlignCenter().Text("Descripción").FontSize(12).Bold();
                                header.Cell().Background("#d9e1f2").Border(1).Padding(5).AlignCenter().Text("Unidad").FontSize(12).Bold();
                                header.Cell().Background("#d9e1f2").Border(1).Padding(5).AlignCenter().Text("Total").FontSize(12).Bold();
                            });

                            // Filas de la tabla
                            foreach (Detalles_Venta detalle in venta.Detalles)
                            {
                                table.Cell().Border(1).Padding(5).Text(detalle.Cantidad.ToString());
                                table.Cell().Border(1).Padding(5).Text(detalle.Producto?.Nombre ?? "N/A");
                                table.Cell().Border(1).Padding(5).Text(detalle.Precio.ToString("C", monedaGuatemala));
                                table.Cell().Border(1).Padding(5).AlignRight().Text((detalle.Cantidad * detalle.Precio).ToString("C", monedaGuatemala));
                            }

                            // Filas vacías para mantener diseño
                            for (int i = venta.Detalles.Count; i < 15; i++)
                            {
                                table.Cell().Border(1).Height(25).Text("");
                                table.Cell().Border(1).Height(25).Text("");
                                table.Cell().Border(1).Height(25).Text("");
                                table.Cell().Border(1).Height(25).Text("");
                            }
                        });

                        // Sección inferior (Dirección, contacto y total)
                        content.Item().PaddingVertical(10).Row(row =>
                        {
                            // Dirección e información de contacto
                            row.RelativeItem(2).Column(col =>
                            {
                                col.Item().Text("📍 Dirección: Calle cualquiera 123").FontSize(14);
                                col.Item().Text("📧 Hola@sitioincreible.com").FontSize(14);
                            });

                            // Teléfono
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("📞 (55) 1234-5678").FontSize(14);
                            });

                            // Total
                            row.RelativeItem().AlignRight().Column(col =>
                            {
                                col.Item().Background("#d9e1f2").Padding(10).Border(1).Text($"Total: {venta.Detalles.Sum(d => d.Cantidad * d.Precio).ToString("C", monedaGuatemala)}").FontSize(14).Bold();
                            });
                        });

                        // Firma
                        content.Item().PaddingVertical(20).AlignRight().Column(col =>
                        {
                            col.Item().Text("________________________").FontSize(14).AlignCenter();
                            col.Item().Text("Firma del cliente").FontSize(14).AlignCenter();
                        });
                    });

                    // --- Pie de página ---
                    page.Footer().AlignLeft().Text("SERVICIO A DOMICILIO").FontSize(20).Bold();
                });
            });

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", $"Factura_{id}.pdf");
        }
    }
}
