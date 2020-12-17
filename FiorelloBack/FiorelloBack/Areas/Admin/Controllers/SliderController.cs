using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FiorelloBack.DAL;
using FiorelloBack.Extentions;
using FiorelloBack.Helpers;
using FiorelloBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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
            ViewBag.SlideCount = _context.Sliders.Count();
            return View(_context.Sliders.ToList());
        }
        public IActionResult Create()
        {
            int count = _context.Sliders.Count();
            if (count >= 5)
            {
                return Content("sile bilmezsiz");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            #region file upload
            //if (slider.Photo == null)
            //{
            //    return View();
            //}
            //if (!slider.Photo.IsImage())
            //{
            //    ModelState.AddModelError("Photo", "Please select image type");
            //    return View();
            //}
            //if (!slider.Photo.MaxSize(200))
            //{
            //    ModelState.AddModelError("Photo", "Max 200kb");
            //    return View();
            //}

            //int count = _context.Sliders.Count();
            //if (count >= 5)
            //{
            //    return Content("sile bilmezsiz");
            //}


            //slider.Image = await slider.Photo.SaveImageAsync(_env.WebRootPath, "img");

            //await _context.Sliders.AddAsync(slider);
            //await _context.SaveChangesAsync();
            #endregion
            if (slider.Photos == null)
            {
                return View();
            }
            int count = _context.Sliders.Count();
            if (count+slider.Photos.Length > 5)
            {
                ModelState.AddModelError("Photos", $"{5-count}- image can select");
                return View();
            }
            foreach (IFormFile photo in slider.Photos)
            {
                
                if (!photo.IsImage())
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName}-not image type");
                    return View();
                }
                if (!photo.MaxSize(200))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName}-max length 200kb");
                    return View();
                }

                Slider newSlider = new Slider();
                newSlider.Image = await photo.SaveImageAsync(_env.WebRootPath, "img");
                await _context.Sliders.AddAsync(newSlider);
               
            }
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Slider slider = await _context.Sliders.FindAsync(id);
            if (slider == null) return NotFound();
            return View(slider);

        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Slider slider =await _context.Sliders.FindAsync(id);
            if (slider == null) return NotFound();
            int count = _context.Sliders.Count();
            if (count == 1)
            {
                return Content("sile bilmezsiz");
            }
            return View(slider);
          
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
           
            if (id == null) return NotFound();
            Slider slider = await _context.Sliders.FindAsync(id);
            if (slider == null) return NotFound();
            int count = _context.Sliders.Count();
            if (count == 1)
            {
                return Content("sile bilmezsiz");
            }
            bool isDeleted = Helper.DeleteImage(_env.WebRootPath, "img", slider.Image);
            if (!isDeleted)
            {
                ModelState.AddModelError(" ", "Some problem exists");
            }
          
            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Slider sliderD = await _context.Sliders.FindAsync(id);
            if (sliderD == null) return NotFound();
            return View(sliderD);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Update")]
        public async Task<IActionResult> Update(int? id, Slider slider)
        {
            Slider sliderD = await _context.Sliders.FindAsync(id);
            if (id == null) return NotFound();
            if (slider.Photo == null)
            {
                return View(sliderD);
            }
            if (!slider.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Artiq bu adda  movcuddur");
                return View(sliderD);
            }
            if (!slider.Photo.MaxSize(200))
            {
                ModelState.AddModelError("Photo", "Artiq bu size  movcud deyil");
                return View(sliderD);
            }

            //Slider sliderD = await _context.Sliders.FindAsync(id);
            if (sliderD == null) return NotFound();
            bool isDeleted = Helper.DeleteImage(_env.WebRootPath, "img", sliderD.Image);
            if (!isDeleted)
            {
                ModelState.AddModelError(" ", "Some problem exists");
            }

            _context.Sliders.Remove(sliderD);
            slider.Image = await slider.Photo.SaveImageAsync(_env.WebRootPath, "img");
           

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



            //Slider slide = await _context.Sliders.FindAsync(id);
            //Slider isExist = _context.Sliders.FirstOrDefault(s => s.Image.ToLower() == slide.Image.ToLower());
            //if (isExist != null)
            //{
            //    if (isExist != slide)
            //    {
            //        ModelState.AddModelError("Name", "Artiq bu adda  movcuddur");
            //        return View();
            //    }
            //}

            //await _context.Sliders.AddAsync(slider);
            //slide.Image = slide.Image;
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));

        }
    }
}
