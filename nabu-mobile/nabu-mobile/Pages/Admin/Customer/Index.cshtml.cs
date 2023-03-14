using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;

namespace MyRazorPage.Pages.Admin.Customer
{
    public class IndexModel : PageModel
    {
        private readonly PRN221_DBContext _context;
        private readonly IConfiguration configuration;
        private readonly IHubContext<HubServer> hubContext;
        [BindProperty]
        public Pager<Models.Customer> customers { get; set; }

        [BindProperty]
        public int currentPage { get; set; }
        public IndexModel(PRN221_DBContext dbContext, IConfiguration configuration, IHubContext<HubServer> hubContext)
        {
            _context = dbContext;
            this.configuration = configuration;
            this.hubContext = hubContext;
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
