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

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context )
        {
            _logger = logger;
            _context = context;
         
        }

        public IActionResult Index()
        {
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
