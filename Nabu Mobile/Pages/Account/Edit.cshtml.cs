using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nabu_Mobile.Models;
using System.Text.Json;

namespace Nabu_Mobile.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly PRN221DBContext prn221DBContext;

        public EditModel(PRN221DBContext prn221DBContext) => this.prn221DBContext = prn221DBContext;

        [BindProperty]
        public Nabu_Mobile.Models.Account? account { get; set; }

        [BindProperty]
        public Nabu_Mobile.Models.Customer? customer { get; set; }

        public async Task<IActionResult> OnGet()
        {
            String? accountSession = HttpContext.Session.GetString("account");
            if (accountSession is not null)
            {
                account = JsonSerializer.Deserialize<Nabu_Mobile.Models.Account>(accountSession);
                if (account is not null)
                {
                    customer = await findByCustomerId(account.CustomerId);
                    return Page();
                }
            }
            return Redirect("signIn");
        }

        public async Task<ActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                String? getAccount = HttpContext.Session.GetString("account");
                var acc = JsonSerializer.Deserialize<Nabu_Mobile.Models.Account>(getAccount);
                var cus = await findByCustomerId(acc.CustomerId);
                if(cus is not null)
                {
                    cus.CompanyName = customer.CompanyName;
                    cus.ContactName = customer.ContactName;
                    cus.ContactTitle = customer.ContactTitle;
                    cus.Address = customer.Address;
                    await prn221DBContext.SaveChangesAsync();
                }
                if(acc is not null)
                {
                    acc.Email = account.Email;
                    await prn221DBContext.SaveChangesAsync();
                    return RedirectToPage("/account/signout");
                }
            }
            return Page();
        }

        public async Task<Nabu_Mobile.Models.Customer?> findByCustomerId(String? customerId)
        {
            var customer = await prn221DBContext.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
            if (customer is not null)
            {
                return customer;
            }
            return null;
        }
    }
}
