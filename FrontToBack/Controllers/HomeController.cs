using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrontToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //List<Slide> Slide = _context.Slides.ToList();
            //List<Category> categories = _context.Categories.ToList();
            //List<Product> products = _context.Products.ToList();

            //HomeVM homeVM = new HomeVM
            //{
            //    Slides = _context.Slides,
            //    Categories = _context.Categories,
            //    Products = _context.Products
            //};

            //Session Set + Startup
            //HttpContext.Session.SetString("Freedi", "Vugar");
            //Cookie Set
            //Response.Cookies.Append("Sarvan", "Ceyhun", new CookieOptions { MaxAge = TimeSpan.FromMinutes(20)});

            

            return View(new HomeVM
            {
                Slides = _context.Slides,
                Categories = _context.Categories,
                PageSign = _context.PageSigns.FirstOrDefault(),
                About = _context.Abouts.Include(a=>a.AboutItems).FirstOrDefault(),
                Experts = _context.Experts,
                Bio = _context.Bios.FirstOrDefault()
            });
        }

        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id == null) return NotFound();
            Product product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            List<BasketVM> products;
            string existBasket = Request.Cookies["basket"];
            if (existBasket == null)
            {
                products = new List<BasketVM>();
            }
            else
            {
                products = JsonConvert.DeserializeObject<List<BasketVM>>(existBasket);
            }

            BasketVM existProduct = products.FirstOrDefault(p => p.Id == id);
            if (existProduct == null)
            {
                BasketVM newProduct = new BasketVM
                {
                    Id = product.Id,
                    Count = 1
                };
                products.Add(newProduct);
            }
            else
            {
                existProduct.Count++;
            }


            string basket = JsonConvert.SerializeObject(products);
            Response.Cookies.Append("basket", basket, new CookieOptions { MaxAge = TimeSpan.FromDays(14) });

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Basket()
        {
            //Session Get
            //string session = HttpContext.Session.GetString("Freedi");
            //Cookie Get
            //string cookie = Request.Cookies["Sarvan"];

            string basket = Request.Cookies["Basket"];
            List<BasketVM> products = new List<BasketVM>();
            if (basket != null)
            {
                products = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
                foreach (BasketVM item in products)
                {
                    Product dbProduct = await _context.Products.FindAsync(item.Id);

                    item.Price = dbProduct.Price;
                    item.Image = dbProduct.Image;
                    item.Name = dbProduct.Name;
                }
            }

            return View(products);
        }
    }
}
