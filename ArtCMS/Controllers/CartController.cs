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
            // init CartVM list
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // init CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                // get the product
                ProductDTO product = db.Products.Find(id);

                // check if product is already in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // if not add new product to cart
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
                    // if there is , increment
                    productInCart.Quantity++;
                    
                }
            }
            // get total quantity and price of the shopping cart and add to model
            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // save back to the session
            Session["cart"] = cart;

            // return partial view
            return PartialView(model);
        }
    }
}