using ArtCMS.Models.Data;
using ArtCMS.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArtCMS.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            // init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // check if cart is empty
            if (cart.Count == 0 || Session["cart"] ==  null)
            {
                ViewBag.Message = "Your cart is empty!";
                return View();
            }

            // calculate total and save to ViewBag
            decimal total = 0m;
            foreach(var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // return view with list
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            // init cartVM
            CartVM model = new CartVM();

            // init quantity
            int qty = 0;

            // init price
            decimal price = 0m;

            // check for cart session
            if (Session["cart"] != null)
            {
                // get the total quantity and price
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                // or set quantity and price to 0
                model.Quantity = 0;
                model.Price = 0m;
                
            }
            // return partial view
            return PartialView(model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            // Init CartVM list
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Init CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                // Get the product
                ProductDTO product = db.Products.Find(id);

                // Check if the product is already in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // If not, add new
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    // If it is, increment
                    productInCart.Quantity++;
                }
            }

            // Get total qty and price and add to model

            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // Save cart back to session
            Session["cart"] = cart;

            // Return partial view with model
            return PartialView(model);
        }

        // GET: /Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            // init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // get cartvm from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // increment qty
                model.Quantity++;

                // store needed data
                var result = new { qty = model.Quantity, price = model.Price };

                // return Json
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            
        }

        // GET: /Cart/DecrementProduct
        public JsonResult DecrementProduct(int productId)
        {
            // init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // get model from the list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // decrement the quantity
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                // store needed data
                var result = new { qty = model.Quantity, price = model.Price };

                // return Json
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /Cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            // init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // get model from the list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // remove the model from the list
                cart.Remove(model);
            }
        }

        public ActionResult PaypalPartial()
        {
            // init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            return PartialView(cart);
        }
    }
}