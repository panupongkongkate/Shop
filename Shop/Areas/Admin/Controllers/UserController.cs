using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess.Data;
using Shop.Models;
using Shop.Utility;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var User = await _context.ApplicationUsers.FindAsync(id);
            if (User == null)
            {
                return NotFound();
            }
            return View(User);

        }
        //Post Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(string id, ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = _context.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();
                userFromDb.Name = applicationUser.Name;
                userFromDb.StreetAddress = applicationUser.StreetAddress;
                userFromDb.City = applicationUser.City;
                userFromDb.State = applicationUser.State;
                userFromDb.PostalCode = applicationUser.PostalCode;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(applicationUser);

        }



            #region API CALLS
        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.ToList();
            var userRole = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();
            foreach (var item in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == item.Id).RoleId;
                item.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }
            //var allObj = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = userList });
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            var userRole = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();

            ApplicationUser userFromDb = _context.ApplicationUsers.Where(u => u.Id.Equals(id)).FirstOrDefault();

            var roleId = userRole.FirstOrDefault(u => u.UserId.Equals(userFromDb.Id)).RoleId;
            var itemlist = roles.FirstOrDefault(u => u.Id.Equals(roleId)).Name;

            if (itemlist == SD.Role_Admin)
            {
                return Json(new { success = false, message = "false Successful" });
            }    
            _context.ApplicationUsers.Remove(userFromDb);
            _context.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
