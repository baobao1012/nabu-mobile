using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nabu_Mobile.Models;

namespace Nabu_Mobile.Pages.Admin.Order
{
    public class DetailModel : PageModel
    {
        private readonly PRN221DBContext prn221DBContext;
        public DetailModel(PRN221DBContext prn221DBContext)
            => this.prn221DBContext = prn221DBContext;

        [BindProperty]
        public Nabu_Mobile.Models.Order? order { get; set; }

        [BindProperty]
        public List<Nabu_Mobile.Models.OrderDetail>? orderDetails { get; set; }

        public async Task OnGet(int id)
        {
            await findByOrderId(id);
            await findListByOrderId(id);
        }

        public async Task findByOrderId(int id)
        {
            order = await prn221DBContext.Orders.SingleOrDefaultAsync(x => x.OrderId == id);
        }

        public async Task findListByOrderId(int id)
        {
            orderDetails = await prn221DBContext.OrderDetails.Include(x => x.Product).Where(x => x.OrderId == id).ToListAsync();
        }
    }
}
