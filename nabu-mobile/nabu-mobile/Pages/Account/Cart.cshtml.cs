using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System;
using System.Security.Principal;
using System.Text.Json;

namespace MyRazorPage.Pages.Account
{
    public class CartModel : PageModel
    {
        private readonly PRN221_DBContext prn221DBContext;
        private readonly int lENGTH_CUSTOMER_ID = 5;
        private readonly Random _random = new();
        public CartModel(PRN221_DBContext prn221DBContext)
            =>  this.prn221DBContext = prn221DBContext;
        [BindProperty]
        public List<Cart>? carts { get; set; }

        [BindProperty]
        public Models.Customer? customer { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string? getCart = HttpContext.Session.GetString("cart");
            string? accountSession = HttpContext.Session.GetString("account");

            if (getCart is not null)
            {
                carts = JsonSerializer.Deserialize<List<Cart>>(getCart);
                if (carts is not null)
                {
                    ViewData["TotalPrice"] = carts.Sum(x => x.Quantity * x.Product.UnitPrice);
                    ViewData["Quantity"] = carts.Sum(x => x.Quantity);
                }
                else
                {
                    return Redirect("/index");
                }
            }

            if (accountSession is not null)
            {
                var account = JsonSerializer.Deserialize<Models.Account>(accountSession);
                if (account is not null)
                {
                    customer = await findByCustomerId(account.CustomerId);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnGetBuyNow(int id)
        {

            bool isAddToCart = await AddToCart(id);
            if(isAddToCart)
            {
                return RedirectToPage("/account/cart");
            }
            return Redirect("/index");
        }

        public async Task<IActionResult> OnGetAddToCart(int id)
        {
            bool isAddToCart = await AddToCart(id);
            return Redirect("/product/detail?id="+id);
        }

        public IActionResult OnGetMinus(int id)
        {
            string? getCart = HttpContext.Session.GetString("cart");
            if (getCart is not null)
            {
                carts = JsonSerializer.Deserialize<List<Cart>>(getCart);
            }
            if (carts is not null)
            {
                int cartIndex = Exists(carts, id);
                if (cartIndex != -1)
                {
                    if (carts[cartIndex].Quantity > 1)
                    {
                        carts[cartIndex].Quantity--;
                    }
                }
                string saveCart = JsonSerializer.Serialize(carts);
                HttpContext.Session.SetString("cart", saveCart);
            }

            return RedirectToPage("/account/cart");
        }


        public IActionResult OnGetDelete(int id)
        {
            string? getCart = HttpContext.Session.GetString("cart");
            List<Cart>? carts = JsonSerializer.Deserialize<List<Cart>>(getCart);
            int index = Exists(carts, id);
            carts.RemoveAt(index);
            string savesjoncart = JsonSerializer.Serialize(carts);
            HttpContext.Session.SetString("cart", savesjoncart);
            if(carts.Count > 0)
            {
                return RedirectToPage("/account/cart");
            }
            return Redirect("/index");
        }

        public async Task<IActionResult> OnPostOrder()
        {
            var session = HttpContext.Session.GetString("account");
            var acc = new Models.Account();
            var cus = new Customer();

            if (session is not null)
            {
                acc = JsonSerializer.Deserialize<MyRazorPage.Models.Account>(session);
                cus = prn221DBContext.Customers.SingleOrDefault(x => x.CustomerId == acc.CustomerId);
            }
            else
            {
                cus = new Customer
                {
                    CustomerId = generatedCustomerId(),
                    CompanyName = customer.CompanyName,
                    ContactName = customer.ContactName,
                    ContactTitle = customer.ContactTitle,
                    Address = customer.Address
                };
                await prn221DBContext.Customers.AddAsync(cus);
                await prn221DBContext.SaveChangesAsync();
            }
            Models.Order order = new Models.Order()
            {
                CustomerId = cus.CustomerId,
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now.AddDays(1),
            };

            await prn221DBContext.Orders.AddAsync(order);
            await prn221DBContext.SaveChangesAsync();

            string? jsoncart = HttpContext.Session.GetString("cart");
            List<Cart>? carts = JsonSerializer.Deserialize<List<Cart>>(jsoncart);
            {
                foreach (var product in carts)
                {
                    OrderDetail orderDetail = new();
                    orderDetail.OrderId = order.OrderId;
                    orderDetail.ProductId = product.Product.ProductId;
                    orderDetail.UnitPrice = (decimal)product.Product.UnitPrice;
                    orderDetail.Quantity = product.Quantity;
                    orderDetail.Discount = 0;
                    await prn221DBContext.OrderDetails.AddAsync(orderDetail);
                    await prn221DBContext.SaveChangesAsync();
                }
            };

            HttpContext.Session.Remove("cart");
            return Redirect("/index");
        }

        private async Task<bool> AddToCart(int id)
        {
            string? getCart = HttpContext.Session.GetString("cart");
            if (getCart is not null)
            {
                carts = JsonSerializer.Deserialize<List<Cart>>(getCart);
            }
            if (carts is null)
            {
                carts = new List<Cart>();
                carts.Add(new Cart
                {
                    Product = await prn221DBContext.Products.SingleOrDefaultAsync(x => x.ProductId == id),
                    Quantity = 1
                });
                string saveCart = JsonSerializer.Serialize(carts);
                HttpContext.Session.SetString("cart", saveCart);
                return true;
            }
            else
            {
                int cartIndex = Exists(carts, id);
                if (cartIndex == -1)
                {
                    carts.Add(new Cart
                    {
                        Product = await prn221DBContext.Products.SingleOrDefaultAsync(x => x.ProductId == id),
                        Quantity = 1
                    });
                }
                else
                {
                    carts[cartIndex].Quantity++;
                }
                string saveCart = JsonSerializer.Serialize(carts);
                HttpContext.Session.SetString("cart", saveCart);
                return true;

            }
            return false;
        }

        private int Exists(List<Cart> cart, int id)
        {
            for (var i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.ProductId == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public async Task<Models.Customer?> findByCustomerId(String? customerId)
        {
            var customer = await prn221DBContext.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
            if (customer is not null)
            {
                return customer;
            }
            return null;
        }

        private string generatedCustomerId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, lENGTH_CUSTOMER_ID)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }

}
