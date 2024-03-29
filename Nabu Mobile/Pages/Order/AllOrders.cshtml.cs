using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nabu_Mobile.common;
using Nabu_Mobile.Models;
using System.Text.Json;

namespace Nabu_Mobile.Pages.Order
{
    public class AllOrdersModel : PageModel
    {
        private readonly PRN221DBContext prn221DBContext;
        private readonly CommonRole commonRole = new();

        [BindProperty]
        public Models.Account? account { get; set; }

        [BindProperty]
        public Models.Customer? customer { get; set; }

        [BindProperty]
        public List<Models.Order>? orders { get; set; }


        public AllOrdersModel(PRN221DBContext prn221DBContext) => this.prn221DBContext = prn221DBContext;

        public async Task<IActionResult> OnGet() 
        {
            String? accountSession = HttpContext.Session.GetString("account");
            if (accountSession is not null)
            {
                account = JsonSerializer.Deserialize<Models.Account>(accountSession);
                if (account is not null && account.Role == commonRole.CUSTOMER_ROLE)
                {
                    //get customer 
                    customer = await findByCustomerId(account.CustomerId);
                    //get order
                    orders = await getAllOrdersByCustomerId(account.CustomerId);
                    return Page();

                }
            }
            return Redirect("signIn");
        }


        public async Task<Models.Customer?> findByCustomerId(String? customerId)
        {
            var customer = await prn221DBContext.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId);
            if (customer is not null)
            {
                return customer;
            }
            return null;
        }

        public async Task<List<Models.Order>?> getAllOrdersByCustomerId(String? customerId)
        {
            var orders = await prn221DBContext.Orders
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.OrderDate)
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .ToListAsync();
            if(orders is not null)
            {
                return orders;
            }
            return null;
        }
    }
}
