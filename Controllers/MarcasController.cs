﻿using System;
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
    public class MarcasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public MarcasController(AppDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: Marcas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Marcas.ToListAsync());
        }

        // GET: Marcas/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marcas = await _context.Marcas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (marcas == null)
            {
                return NotFound();
            }

            return View(marcas);
        }

        // GET: Marcas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Marcas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Marcas marcas, IFormFile imageFile)
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

                    if (uploadResult.Error != null)
                    {
                        ModelState.AddModelError("", $"Error al subir imagen: {uploadResult.Error.Message}");
                        return View(marcas); // Regresa a la vista si falla la subida
                    }

                    marcas.ImgUrl = uploadResult.SecureUrl.ToString();

                    var thumbnailParams = new Transformation().Width(150).Height(150).Crop("thumb");
                    marcas.ThumbnailUrl = _cloudinary.Api.UrlImgUp.Transform(thumbnailParams).BuildUrl(uploadResult.PublicId);

                }
                _context.Add(marcas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(marcas);
        }

        // GET: Marcas/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marcas = await _context.Marcas.FindAsync(id);
            if (marcas == null)
            {
                return NotFound();
            }
            return View(marcas);
        }

        // POST: Marcas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("Id,Nombre")] Marcas marcas, IFormFile imageFile)
        {
            if (id != marcas.Id)
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
                        marcas.ImgUrl = uploadResult.SecureUrl.ToString();

                        var thumbnailParams = new Transformation().Width(150).Height(150).Crop("thumb");
                        marcas.ThumbnailUrl = _cloudinary.Api.UrlImgUp.Transform(thumbnailParams).BuildUrl(uploadResult.PublicId);
                    }
                    _context.Update(marcas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarcasExists(marcas.Id))
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
            return View(marcas);
        }

        // GET: Marcas/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marcas = await _context.Marcas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (marcas == null)
            {
                return NotFound();
            }

            return View(marcas);
        }

        // POST: Marcas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var marcas = await _context.Marcas.FindAsync(id);
            if (marcas != null)
            {
                _context.Marcas.Remove(marcas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarcasExists(long? id)
        {
            return _context.Marcas.Any(e => e.Id == id);
        }
    }
}
