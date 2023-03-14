using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Nabu_Mobile.Pages.Account
{
    public class SignOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("account") is not null)
            {
                HttpContext.Session.Remove("account");
            }
            return RedirectToPage("/index");
        }
    }
}
