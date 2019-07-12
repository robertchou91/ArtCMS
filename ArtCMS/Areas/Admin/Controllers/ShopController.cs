using ArtCMS.Models.Data;
using ArtCMS.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArtCMS.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            // declare the list of CategoryVM
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                // Init the list
                categoryVMList = db.Categories
                                .ToArray()
                                .OrderBy(x => x.Sorting)
                                .Select(x => new CategoryVM(x))
                                .ToList();
            }
            return View(categoryVMList);
        }
    }
}