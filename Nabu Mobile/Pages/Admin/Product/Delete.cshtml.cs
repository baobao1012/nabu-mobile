using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Nabu_Mobile.Pages.Product
{
    public class DeleteModel : PageModel
    {
        private readonly Nabu_Mobile.Models.PRN221DBContext _context;

        public DeleteModel(Nabu_Mobile.Models.PRN221DBContext _context)
        {
            this._context = _context;
        }
        public Models.Product? Product { get; set; }
        public async Task<IActionResult> OnGet(int id)
        {
            Product = _context.Products.FirstOrDefault(x => x.ProductId == id);
            if (Product.ProductId != id)
            {
                return NotFound();
            }
            else
            {
                Product.Discontinued = true;

            }
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
