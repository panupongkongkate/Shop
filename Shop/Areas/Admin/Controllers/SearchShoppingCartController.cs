using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess.Data;
using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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

        public IActionResult ShowDetail(string id)
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

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _context.Appointments.ToList();
            return Json(new { data = allObj });
        }

        //POST Delete
        [HttpDelete]
        public IActionResult Delete(string id)
        {
            var appointment = _context.Appointments.Where(x=>x.Receiptnumber.Equals(Convert.ToInt32(id))).FirstOrDefault();


            var ProductsSelectedForAppointment = _context.ProductsSelectedForAppointment.Where(x=>x.Id.Equals(appointment.Id)).FirstOrDefault();

            if(appointment != null && ProductsSelectedForAppointment != null )
            {
                _context.Appointments.Remove(appointment);

                _context.ProductsSelectedForAppointment.Remove(ProductsSelectedForAppointment);


                 _context.SaveChanges();
            }

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion API CALLS 


    }
}