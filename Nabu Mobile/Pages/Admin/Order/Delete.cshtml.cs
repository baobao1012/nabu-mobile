using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Nabu_Mobile.Models;

namespace Nabu_Mobile.Pages.Admin.Order
{
    public class DeleteModel : PageModel
    {
        private readonly PRN221DBContext prn221DBContext;

        public DeleteModel(PRN221DBContext prn221DBContext)
        {
            this.prn221DBContext = prn221DBContext;
        }
        public async Task<IActionResult> OnGet(int? id)
        {
            if (id is null)
            {
                return RedirectToPage("/admin/order/index");
            }
            var order = await prn221DBContext.Orders.SingleOrDefaultAsync(x => x.OrderId == id);
            if (order is not null)
            {
                order.RequiredDate = null;
                await prn221DBContext.SaveChangesAsync();
            }

            return RedirectToPage("/admin/order/index");
        }
    }
}
