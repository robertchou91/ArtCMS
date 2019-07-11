using ArtCMS.Models.Data;
using ArtCMS.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArtCMS.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Declaring the list of PageVM
            List<PageVM> pagesList;

            
            using (Db db = new Db())
            {
                // Init the list
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();

            }

            // Return the list
            return View(pagesList);
        }


        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Declare slug
                string slug;

                // init PageDTO
                var dto = new PageDTO();

                // DTO title
                dto.Title = model.Title;

                // check for and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // Make sure Title and Slug are unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "The Title or Slug already exist.");
                    return View(model);
                }

                // DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;
                dto.Sorting = 100;

                // Save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            // Set TempData Message
            TempData["SM"] = "You have added a new page!";

            // Redirect
            return RedirectToAction("AddPage");
        }

        // GET: Admin/Pages/EditPage/id
        public ActionResult EditPage(int id)
        {
            // Declare PageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get the page
                PageDTO dto = db.Pages.Find(id);

                // Confirm the page exist
                if (dto == null)
                {
                    return Content("The page does not exist!");
                }

                // Init the page
                model = new PageVM(dto);
            }

            // return the view with the model
            return View(model);
        }
    }
}