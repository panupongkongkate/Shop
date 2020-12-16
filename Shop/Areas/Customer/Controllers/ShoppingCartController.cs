using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess.Data;
using Shop.Models;
using Shop.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BulkyBook.Utility;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }


        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Product = new List<Models.Product>()
            };
        }

        public IActionResult Index()
        {
            //if (!TempData["lstShoppingCart"].Equals(0))
            //{
            //    IList<int> converCatalog = (IList<int>)TempData["lstShoppingCart"];

            //    if (converCatalog.Count > 0)
            //    {
            //        foreach (int cartItem in converCatalog)
            //        {
            //            Product prod = _context.Products.Where(p => p.Id == cartItem).FirstOrDefault();
            //            ShoppingCartVM.Product.Add(prod);
            //        }
            //    }

            //    TempData["lstShoppingCart"] = converCatalog;
            //}
            var converCatalog = HttpContext.Session.GetObject<List<int>>("ssShoppingCart");

            if (converCatalog.Count > 0)
            {
                foreach (int cartItem in converCatalog)
                {
                    Product prod = _context.Products.Where(p => p.Id == cartItem).FirstOrDefault();
                    ShoppingCartVM.Product.Add(prod);
                }
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            if(ModelState.IsValid)
            {
                List<int> lstCartItems = HttpContext.Session.GetObject<List<int>>("ssShoppingCart");

                ShoppingCartVM.Appointments.AppointmentDate = ShoppingCartVM.Appointments.AppointmentDate.AddHours(ShoppingCartVM.Appointments.AppointmentTime.Hour).AddMinutes(ShoppingCartVM.Appointments.AppointmentTime.Minute);

                Appointments appointments = ShoppingCartVM.Appointments;
                appointments.Receiptnumber = GenerateCode();
                _context.Appointments.Add(appointments);
                _context.SaveChanges();

                int appointmentId = appointments.Id;

                foreach (int productId in lstCartItems)
                {
                    ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment()
                    {
                        AppointmentId = appointmentId,
                        ProductId = productId
                    };
                    _context.ProductsSelectedForAppointment.Add(productsSelectedForAppointment);

                }
                _context.SaveChanges();


                lstCartItems = new List<int>();
                HttpContext.Session.SetObject("ssShoppingCart", lstCartItems);

                return RedirectToAction("AppointmentConfirmation", "ShoppingCart", new { Id = appointmentId });
            }
            return RedirectToAction("Index");
        }
        public static int GenerateCode()
        {
            string strDT = DateTime.Now.Ticks.ToString();

            // "1518416541848541848" //19
            return Convert.ToInt32( strDT.Substring(strDT.Length - 5, 5)); //CD41848
        }

        //Get
        public IActionResult AppointmentConfirmation(int? id)
        {
            ShoppingCartVM.Appointments = _context.Appointments.Where(a => a.Id == id).FirstOrDefault();

            List<ProductsSelectedForAppointment> objProdList = _context.ProductsSelectedForAppointment.Where(p => p.AppointmentId == id).ToList();

            foreach (ProductsSelectedForAppointment prodAptObj in objProdList)
            {
                ShoppingCartVM.Product.Add(_context.Products.Where(p => p.Id == prodAptObj.ProductId).FirstOrDefault());
            }
            var listSend = new List<int>() ;
            HttpContext.Session.SetObject("ssShoppingCart", listSend);

            return View(ShoppingCartVM);
        }

        public IActionResult Remove(int? id)
        {
            var lstCartItems = HttpContext.Session.GetObject<List<int>>("ssShoppingCart");
            if (lstCartItems.Count > 0)
            {
                if (lstCartItems.Contains((int)id))
                {
                  lstCartItems.Remove((int)id);
                }
            }
            HttpContext.Session.SetObject("ssShoppingCart", lstCartItems);

            return RedirectToAction(nameof(Index));
        }
    }
}
