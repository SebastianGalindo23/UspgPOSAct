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
    public class ClasificacionesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public ClasificacionesController(AppDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: Clasificaciones
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clasificaciones.ToListAsync());
        }

        // GET: Clasificaciones/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clasificaciones = await _context.Clasificaciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clasificaciones == null)
            {
                return NotFound();
            }

            return View(clasificaciones);
        }

        // GET: Clasificaciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clasificaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Clasificaciones clasificaciones, IFormFile imageFile)
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

                    clasificaciones.ImgUrl = uploadResult.SecureUrl.ToString();

                    var thumbnailParams = new Transformation().Width(150).Height(150).Crop("thumb");
                    clasificaciones.ThumbnailUrl = _cloudinary.Api.UrlImgUp.Transform(thumbnailParams).BuildUrl(uploadResult.PublicId);

                }
                _context.Add(clasificaciones);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clasificaciones);
        }

        // GET: Clasificaciones/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clasificaciones = await _context.Clasificaciones.FindAsync(id);
            if (clasificaciones == null)
            {
                return NotFound();
            }
            return View(clasificaciones);
        }

        // POST: Clasificaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("Id,Nombre")] Clasificaciones clasificaciones, IFormFile imageFile)
        {
            if (id != clasificaciones.Id)
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
                        clasificaciones.ImgUrl = uploadResult.SecureUrl.ToString();

                        var thumbnailParams = new Transformation().Width(150).Height(150).Crop("thumb");
                        clasificaciones.ThumbnailUrl = _cloudinary.Api.UrlImgUp.Transform(thumbnailParams).BuildUrl(uploadResult.PublicId);
                    }
                    _context.Update(clasificaciones);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClasificacionesExists(clasificaciones.Id))
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
            return View(clasificaciones);
        }

        // GET: Clasificaciones/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clasificaciones = await _context.Clasificaciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clasificaciones == null)
            {
                return NotFound();
            }

            return View(clasificaciones);
        }

        // POST: Clasificaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var clasificaciones = await _context.Clasificaciones.FindAsync(id);
            if (clasificaciones != null)
            {
                _context.Clasificaciones.Remove(clasificaciones);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClasificacionesExists(long? id)
        {
            return _context.Clasificaciones.Any(e => e.Id == id);
        }
    }
}
