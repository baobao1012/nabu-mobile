using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nabu_Mobile.Models;

namespace Nabu_Mobile.Pages.Product
{
    public class DetailModel : PageModel
    {
        private readonly PRN221DBContext prn221DBContext;
        public DetailModel(PRN221DBContext dBContext)
            => this.prn221DBContext = dBContext;

        [BindProperty]
        public Nabu_Mobile.Models.Product? product { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            if (id != 0)
            {
                product = await prn221DBContext.Products.SingleOrDefaultAsync(x => x.ProductId == id);
                return Page();
            }
            return Redirect("index");
        }
    }
}
