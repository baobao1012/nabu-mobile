using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Nabu_Mobile.Models;

namespace Nabu_Mobile.Pages.Admin.Customer
{
    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext _context;
        private readonly IConfiguration configuration;
        [BindProperty]
        public Pager<Models.Customer> customers { get; set; }

        [BindProperty]
        public int currentPage { get; set; }
        public IndexModel(PRN221DBContext dbContext, IConfiguration configuration)
        {
            _context = dbContext;
            this.configuration = configuration;
        }
        public async Task OnGet( string txtSearch, int? pageIndex)
        {
           
            if (pageIndex < 1) pageIndex = 1;
            await loadCustomer( txtSearch, pageIndex);
        }
        private async Task loadCustomer(string txtSearch, int? pageIndex)
        {
            IQueryable<Models.Customer> productsIQ = from Customer in _context.Customers
                                                    select Customer;
            try
            {
                if (txtSearch is not null)
                {
                    productsIQ = productsIQ.Where(x => x.ContactName.Contains(txtSearch));
                }
                int pageSize = configuration.GetValue("TableSize", 10);
                customers = await Pager<Models.Customer>.CreateAsync(productsIQ.
                   OrderByDescending(x => x.CustomerId).AsNoTracking(), pageIndex ?? 1, pageSize);
                if (customers is not null)
                {
                    currentPage = (int)Math.Ceiling((decimal)customers.Count / (decimal)pageSize);
                }
            }
            catch (Exception ex)
            {
                ViewData["Mess"] = "NO data";
                Console.WriteLine(ex.Message);
            }
            ViewData["Search"] = txtSearch;

        }
    }
}
