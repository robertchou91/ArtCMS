using ArtCMS.Models.Data;
using ArtCMS.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
        [HttpGet]
        public ActionResult Login()
        {
            // confirm that the user is not logged in
            string username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
                return RedirectToAction("user-profile");

            // return view
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            // check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // check if user is valid
            bool isValid = false;

            using(Db db = new Db())
            {
                if (db.Users.Any(x => x.Username == model.Username && x.Password == model.Password))
                {
                    isValid = true;
                }
            }

            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid username or password!");
                return View(model);
            }
            else
            {
                FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
            }
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

        // GET: Account/logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/account/login");
        }

        public ActionResult UserNavPartial()
        {
            // get the username
            string username = User.Identity.Name;

            //declare model
            UserNavPartialVM model;

            using (Db db = new Db())
            {
                // get the user
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == username);

                // build the model
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }

            // return view
            return PartialView(model);
        }

        // GET: Account/userprofile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            // get the username
            string username = User.Identity.Name;

            // declare the model
            UserProfileVM model;

            using (Db db = new Db())
            {
                // get the user
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == username);

                // build model
                model = new UserProfileVM(dto);
            }
            // return the view
            return View("UserProfile", model);
        }

        // POST: Account/userprofile
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            // check model state
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            // check if password match
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if(model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match!");
                    return View("UserProfile", model);
                }
            }

            using (Db db = new Db())
            {
                // get username
                string username = User.Identity.Name;

                // make sure username is unique
                if(db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == username))
                {
                    ModelState.AddModelError("", "Username" + model.Username + "already exist!");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                // edit dto
                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAddress = model.EmailAddress;
                dto.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                // save
                db.SaveChanges();

                if (!db.Users.Any(x => x.Username == username))
                {
                    FormsAuthentication.SignOut();
                    return Redirect("~/account/login");
                }
            }

            // set tempdata
            TempData["SM"] = "You have edited your profile!";

            // redirect view
            return Redirect("~/account/user-profile");
        }

    }
}