using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UspgPOS.Data;
using UspgPOS.Models;

namespace UspgPOS.Controllers
{
    public class ProductosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public ProductosController(AppDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Productos.Include(p => p.Clasificaciones).Include(p => p.Marca);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productos = await _context.Productos
                .Include(p => p.Clasificaciones)
                .Include(p => p.Marca)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productos == null)
            {
                return NotFound();
            }

            return View(productos);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["ClasificacionesId"] = new SelectList(_context.Clasificaciones, "Id", "Nombre");
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Precio,Cantidad,MarcaId,ClasificacionesId")] Productos productos, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                   productos.ImgUrl = uploadResult.SecureUrl.ToString();
                        
                   var thumbnailParams = new Transformation().Width(150).Height(150).Crop("thumb");
                   productos.ThumbnailUrl = _cloudinary.Api.UrlImgUp.Transform(thumbnailParams).BuildUrl(uploadResult.PublicId);

                }
                _context.Add(productos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClasificacionesId"] = new SelectList(_context.Clasificaciones, "Id", "Nombre", productos.ClasificacionesId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", productos.MarcaId);
            return View(productos);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productos = await _context.Productos.FindAsync(id);
            if (productos == null)
            {
                return NotFound();
            }
            ViewData["ClasificacionesId"] = new SelectList(_context.Clasificaciones, "Id", "Nombre", productos.ClasificacionesId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", productos.MarcaId);
            return View(productos);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("Id,Nombre,Precio,Cantidad,MarcaId,ClasificacionesId")] Productos productos, IFormFile imageFile)
        {
            if (id != productos.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        productos.ImgUrl = uploadResult.SecureUrl.ToString();

                        var thumbnailParams = new Transformation().Width(150).Height(150).Crop("thumb");
                        productos.ThumbnailUrl = _cloudinary.Api.UrlImgUp.Transform(thumbnailParams).BuildUrl(uploadResult.PublicId);
                    }
                    _context.Update(productos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductosExists(productos.Id))
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
            ViewData["ClasificacionesId"] = new SelectList(_context.Clasificaciones, "Id", "Nombre", productos.ClasificacionesId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", productos.MarcaId);
            return View(productos);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productos = await _context.Productos
                .Include(p => p.Clasificaciones)
                .Include(p => p.Marca)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productos == null)
            {
                return NotFound();
            }

            return View(productos);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var productos = await _context.Productos.FindAsync(id);
            if (productos != null)
            {
                _context.Productos.Remove(productos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductosExists(long? id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
