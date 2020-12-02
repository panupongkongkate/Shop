using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess.Data;
using Shop.Models;
using Shop.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Type = Shop.Models.Type;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var types = _context.ProductType.ToList();
          

         //   List<Type> types = new List<Type>()
         //   {
         //       new Type{ID="ต่างหูแบบที่1",Name="ต่างหูแบบที่1"},
         //     new Type{ID="ต่างหูแบบที่2",Name="ต่างหูแบบที่2"},
         //  new Type{ID="ต่างหูแบบที่3",Name="ต่างหูแบบที่3"},
         //new Type{ID="ต่างหูแบบที่4",Name="ต่างหูแบบที่4"},
         // new Type{ID="ต่างหูแบบที่5",Name="ต่างหูแบบที่5"},
         //         new Type{ID="ต่างหูแบบที่6",Name="ต่างหูแบบที่6"},
         //      new Type{ID="ต่างหูแบบที่7",Name="ต่างหูแบบที่7"},
         //   new Type{ID="ต่างหูแบบที่8",Name="ต่างหูแบบที่8"},
         //   };
         //types.Insert(0, new Type {ID="-1" , Name = "------Please Select Job------" });
            ViewBag.Data = types;

            if (id == null)
            {
                //สร้าง
                Product product = new Product();
                ViewBag.Img = "Create";
                return View(product);
            }
            else
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }
                //แก้ไข
                ViewBag.Img = "Edit";
                return View(product);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product product)
        {
          
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extenstion = Path.GetExtension(files[0].FileName);
                    if (product.ImageUrl != null)
                    {
                        //this is an edit and we need to remove old image
                        var imagePath = Path.Combine(webRootPath, product.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extenstion), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    product.ImageUrl = @"\images\products\" + fileName + extenstion;
                }
                else
                {
                    //update when they do not change the image
                    if (product.Id != 0)
                    {
                        var objFromDb = _context.Products.AsNoTracking().Where(x =>x.Id.Equals(product.Id)).FirstOrDefault();
                        product.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                if(product.Id ==0)
                {
                    _context.Products.Add(product);
                }
                else
                {
                    _context.Products.Update(product);
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }    
            return View();
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _context.Products.ToList();
            //var allObj = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _context.Products.Find(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string webRootPath = _hostEnvironment.WebRootPath;

            var imagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.Products.Remove(objFromDb);
            _context.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion API CALLS
    }
}