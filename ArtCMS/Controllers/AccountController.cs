using ArtCMS.Models.Data;
using ArtCMS.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArtCMS.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            // confirm that the user is not logged in
            string username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
                return RedirectToAction("user-profile");

            // return view
            return View();
        }

        // GET: Account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: Account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            // check model state
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            // check if password match
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Password do not match!");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {
                // make sure username is unique
                if (db.Users.Any(x => x.Username == model.Username))
                {
                    ModelState.AddModelError("", "Password do not match!");
                    return View("CreateAccount", model);
                }

                // Create userDTO
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    Username = model.Username,
                    Password = model.Password
                };

                // add the DTO
                db.Users.Add(userDTO);

                // Save
                db.SaveChanges();

                // add UserRolesDTO
                int id = userDTO.Id;

                UserRoleDTO userRolesDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };

                db.UserRoles.Add(userRolesDTO);
                db.SaveChanges();
            }

            // Create Tempdata message
            TempData["SM"] = "You are now registered and can log in!";

            return Redirect("~/account/login");
        }


    }
}