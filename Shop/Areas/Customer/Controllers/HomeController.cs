using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop.DataAccess.Data;
using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Utility;
using Microsoft.AspNetCore.Http;
using BulkyBook.Utility;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        //int sumCatalog;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
      

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context )
        {
            _logger = logger;
            _context = context;
         
        }

        public IActionResult Index()
        {
            var listSend = new List<int>();
            HttpContext.Session.SetObject("ssShoppingCart", listSend);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Detail()
        {
            IEnumerable<Product> productList = _context.Products.ToList();
            return View(productList);
        }
        public async Task<IActionResult> ShowDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult DetailsPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            List<int> listcatalogy = new List<int>();
            var converCatalog = HttpContext.Session.GetObject<List<int>>("ssShoppingCart");
            if (converCatalog != null)
            {

                //var Count = HttpContext.Session.GetInt32("ssShoppingCart").GetValueOrDefault();
                //sumCatalog = Count + 1;

                //IList<int> converCatalog = (IList<int>)TempData["lstShoppingCart"];

                //foreach (var item in converCatalog)
                //{
                //    listcatalogy.Add(item);
                //}
                //listcatalogy.Add((int)id);
                //TempData["lstShoppingCart"] = listcatalogy;

                converCatalog.Add((int)id);
                listcatalogy = converCatalog;
            }
            else
            {
                //sumCatalog = 1;
                listcatalogy.Add((int)id);
                //--------------------------------

                //TempData["lstShoppingCart"] = listcatalogy;
               
            }

            HttpContext.Session.SetObject("ssShoppingCart", listcatalogy);
            //HttpContext.Session.SetInt32("ssShoppingCart", sumCatalog);
            return RedirectToAction("Detail", "Home", new { area = "Customer" });

        }


        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult TestSP()
        {
            var y = _context.Products.FromSqlRaw("EXECUTE SP_selcctall");
            return View(y);
        }
        public IActionResult TestDetail(int? id)   
        {
            var z = _context.Products.FromSqlRaw($"EXECUTE SP_selcctone {id}").AsEnumerable().FirstOrDefault();
            return View(z);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
