using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FrontToBack.DAL;
using FrontToBack.Extensions;
using FrontToBack.Helpers;
using FrontToBack.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_context.Slides);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Slide slide = await _context.Slides.FindAsync(id);
            if (slide == null) return NotFound();
            return View(slide);
        }

        public IActionResult Create()
        {
            if (_context.Slides.Count() >= 5)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slide slide)
        {
            //if (_context.Slides.Count() >= 5)
            //{
            //    return RedirectToAction(nameof(Index));
            //}

            int canuploadfilecount = 5 - (_context.Slides.Count());
            if (canuploadfilecount < slide.Files.Length)
            {
                ModelState.AddModelError("Files", $"Maksimum Yukluye Bileceyin Sekil Sayisi BU {canuploadfilecount} qederdir");
                return View();
            }

            foreach (IFormFile file in slide.Files)
            {
                if (ModelState["Files"].ValidationState == ModelValidationState.Invalid)
                {
                    ModelState.AddModelError("Files", "Zehmet Olmasa Shekil Secin");
                    return View();
                }

                if (!file.IsImage())
                {
                    ModelState.AddModelError("Files", $"Bu {file.FileName} adda file duzgun secilmiyib");
                    return View();
                }

                if (file.CheckFileSize(150))
                {
                    ModelState.AddModelError("Files", $" Secilen {file.FileName} adda faylin olcusu 150 kb -dan artiqdir");
                    return View();
                }

                Slide newSlide = new Slide
                {
                    Name = await file.SaveFileAsync(_env.WebRootPath, "img")
                };

                await _context.Slides.AddAsync(newSlide);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Slide slide = await _context.Slides.FindAsync(id);
            if (slide == null) return NotFound();
            return View(slide);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteSlide(int? id)
        {
            if (id == null) return NotFound();
            Slide slide = await _context.Slides.FindAsync(id);
            if (slide == null) return NotFound();

            Helper.DeleteFile(_env.WebRootPath, "img", slide.Name);

            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Slide slide = await _context.Slides.FindAsync(id);
            if (slide == null) return NotFound();
            return View(slide);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Slide slide)
        {
            if (id == null) return NotFound();
            Slide dbSlide = await _context.Slides.FindAsync(id);
            if (slide == null) return NotFound();

            if (ModelState["File"].ValidationState == ModelValidationState.Invalid)
            {
                ModelState.AddModelError("File", "Zehmet Olmasa Shekil Secin");
                return View();
            }

            if (!slide.File.IsImage())
            {
                ModelState.AddModelError("File", "Duzgun File Secin");
                return View();
            }

            if (slide.File.CheckFileSize(150))
            {
                ModelState.AddModelError("File", "150 kb -da artiq olcude fayl yuklemek olmaz");
                return View();
            }

            Helper.DeleteFile(_env.WebRootPath, "img", dbSlide.Name);

            dbSlide.Name = await slide.File.SaveFileAsync(_env.WebRootPath, "img");
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
