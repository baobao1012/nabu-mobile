using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Nabu_Mobile.Pages.Product
{
    public class CreateModel : PageModel
    {
        private readonly Nabu_Mobile.Models.PRN221DBContext _context;

        public CreateModel(Nabu_Mobile.Models.PRN221DBContext _context)
        {
            this._context = _context;
        }

        [BindProperty]
        public Models.Product? Product { get; set; }
        public IActionResult OnGet(int id)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            if (ProductsExists(id))
            {
                Product = _context.Products.FirstOrDefault(x => x.ProductId == id);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
         /*   if (!ModelState.IsValid)
            {
                return Page();
            }*/

                if (Product.ProductId == 0)
            {
             
                    _context.Products.Add(Product);
              
            }
            else
            {
                _context.Products.Update(Product);

            }


            await _context.SaveChangesAsync();
            return RedirectToPage("/admin/product/index");
        }

        public bool ProductsExists(int id)
        {
            return _context.Products.Any(x => x.ProductId == id);
        }
    }
}
