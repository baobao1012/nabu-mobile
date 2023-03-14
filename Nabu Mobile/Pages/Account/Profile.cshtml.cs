using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nabu_Mobile.common;
using Nabu_Mobile.Models;
using System.Security.Principal;
using System.Text.Json;

namespace Nabu_Mobile.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly PRN221DBContext prn221DBContext;
        private readonly CommonRole commonRole = new();

        public ProfileModel(PRN221DBContext prn221DBContext) => this.prn221DBContext = prn221DBContext;

        [BindProperty]
        public Models.Account? account { get; set; }

        [BindProperty]
        public Models.Customer? customer { get; set; }

        public async Task<IActionResult> OnGet()
        {
            String? accountSession = HttpContext.Session.GetString("account");
            if (accountSession is not null)
            {
                account = JsonSerializer.Deserialize<Models.Account>(accountSession);
                if (account is not null && account.Role == commonRole.CUSTOMER_ROLE)
                {
                    customer = await findByCustomerId(account.CustomerId);
                    return Page();
                }
            }
            return Redirect("signIn"); 
        }

        public async Task<Models.Customer?> findByCustomerId(String? customerId)
        {
            var customer = await prn221DBContext.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId);
            if(customer is not null)
            {
                return customer;
            }
            return null;
        }

    }
}
