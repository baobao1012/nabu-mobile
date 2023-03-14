using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nabu_Mobile.common;
using Nabu_Mobile.Models;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MyRazorPage.Pages.Account
{
    public class SignInModel : PageModel
    {

        private readonly PRN221DBContext prn221DBContext;
        private readonly CommonRole commonRole = new();

        public SignInModel(PRN221DBContext prn221DBContext) => this.prn221DBContext = prn221DBContext;

        [BindProperty]
        public Nabu_Mobile.Models.Account? account { get; set; }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                account = await findByEmailAndPassword(account.Email, account.Password);

                if (account is not null)
                {
                    HttpContext.Session.SetString("account", JsonSerializer.Serialize(account));
                    if (account.Role == commonRole.CUSTOMER_ROLE) return RedirectToPage("/index");
                    else if (account.Role == commonRole.EMPLOYEE_ROLE) return RedirectToPage("/admin/product/index");
                    else return Page();
                }
                else
                {
                    ViewData["message"] = "This account doesn't exist";
                    return Page();
                }
            }
            return Page();
        }

        public async Task<Nabu_Mobile.Models.Account?> findByEmailAndPassword(String? email, String? password)
        {
            var accountInDB = await prn221DBContext.Accounts
                .FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            if (accountInDB is not null)
            {
                return accountInDB;
            }
            return null;
        }

    }
}
