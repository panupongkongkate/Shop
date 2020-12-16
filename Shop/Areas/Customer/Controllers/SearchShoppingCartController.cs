using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess.Data;
using Shop.Models;
using Shop.Models.ViewModels;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SearchShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ShoppingCartViewModel shoppingCart { get; set; }
        public SearchShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Searchproduct(string? id)
        {
            shoppingCart = new ShoppingCartViewModel()
            {
                Product = new List<Product>()
            };
            if (string.IsNullOrEmpty(id))
            {
                return View("Index");
            }
            else
            {

                //shoppingCart.Appointments = (Appointments)_context.Appointments.Where(x =>x.Receiptnumber.Equals(id));

                var listReceip = _context.Appointments.Where(x => x.Receiptnumber.Equals(Convert.ToInt32(id))).ToList();

                foreach (var item in listReceip)
                {
                    shoppingCart.Appointments = item;
                }


                List<ProductsSelectedForAppointment> objProdList = _context.ProductsSelectedForAppointment.Where(p => p.AppointmentId.Equals(shoppingCart.Appointments.Id)).ToList();

                foreach (ProductsSelectedForAppointment prodAptObj in objProdList)
                {
                    Product listProducts = _context.Products.Where(p => p.Id == prodAptObj.ProductId).FirstOrDefault();

                    shoppingCart.Product.Add(listProducts);
                }

            }
            return View(shoppingCart);
        }
    }
}
