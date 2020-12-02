using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess.Data;
using Shop.Models;
using Shop.Utility;
using ToDataTable;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Exportdata()
        {
            var dt = _context.Products.Select(x => new {
                Title = x.Title ,
                Description = x.Description ,
                Type = x.Type ,
                Price= x.Price ,
                Price50 = x.Price50
            }).ToDataTable();
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Test");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "รายการสินค้าทั้งหมด.xlsx");
                }
            }
        }
        [HttpPost]
        public IActionResult OnPostUpload(List<IFormFile> files)
        {
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (XLWorkbook workBook = new XLWorkbook(formFile.OpenReadStream()))
                    {
                        //Read the first Sheet from Excel file.
                        IXLWorksheet workSheet = workBook.Worksheet(1);

                        //Create a new DataTable.
                        DataTable dt = new DataTable();

                        //Loop through the Worksheet rows.
                        bool firstRow = true;
                        foreach (IXLRow row in workSheet.Rows())
                        {
                            //Use the first row to add columns to DataTable.
                            if (firstRow)
                            {
                                foreach (IXLCell cell in row.Cells())
                                {
                                    dt.Columns.Add(cell.Value.ToString());
                                }
                                firstRow = false;
                            }
                            else
                            {
                                //Add rows to DataTable.
                                dt.Rows.Add();
                                int i = 0;
                                foreach (IXLCell cell in row.Cells())
                                {
                                    dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                    i++;
                                }
                            }
                        }
                        foreach (DataRow item in dt.Rows)
                        {
                            Product product = new Product();
                            product.Title = item["Title"].ToString();
                            product.Description = item["Description"].ToString();
                            product.Type = item["Type"].ToString();
                            product.Price = Convert.ToDouble(item["Price"].ToString());
                            product.Price50 = Convert.ToDouble(item["Price50"].ToString());
                            //อันที่comment คืออันที่ ไม่อัพขึ้น db
                            //_context.Shippers.Add(shipper);
                        }
                        //await _context.SaveChangesAsync();

                    }
                }
            }
            return Redirect("Index");
        }

        public IActionResult Viewer()
        {
            return View();
        }

    }
}
