using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly int _count;
        public ProductController(AppDbContext context)
        {
            _context = context;
            _count = _context.Products.Count();
        }
        public IActionResult Index()
        {
            ViewBag.ProductCount = _count;
            return View();
        }

        public IActionResult Load(int skip)
        {
            if (skip >= _context.Products.Count())
            {
                return Content("Get baska Yerde Oyna");
            }

            IEnumerable<Product> model = _context.Products.Include(p => p.Category).Skip(skip).Take(8);

            return PartialView("_ProductPartial", model);

            #region Old Partial
            //return Json(_context.Products.Include(p => p.Category).Skip(8).Take(8));

            //return Json(_context.Products.Select(p => new ProductVM
            //{
            //    Id = p.Id,
            //    Name = p.Name,
            //    Image = p.Image,
            //    Price = p.Price,
            //    Category = new CategoryVM
            //    {
            //        Id = p.Category.Id,
            //        Name = p.Category.Name
            //    }
            //}).Skip(8).Take(8).ToList());
            #endregion
        }

        public IActionResult Search(string search)
        {
            IEnumerable<Product> model = _context.Products
                .Where(p => p.Name.ToLower().Contains(search.ToLower()))
                .OrderByDescending(p=>p.Id)
                .Take(10);

            return PartialView("_SearchPartial",model);
        }
    }
}
