﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nabu_Mobile.common;
using Nabu_Mobile.Models;

namespace MyRazorPage.Pages.Account
{
    public class SignUpModel : PageModel
    {
        
        private readonly PRN221DBContext prn221DBContext;
        private readonly Random _random = new();
        private readonly int lENGTH_CUSTOMER_ID = 5;
        private readonly CommonRole commonRole = new();

        public SignUpModel(PRN221DBContext prn221DBContext) => this.prn221DBContext = prn221DBContext;

        //Khai báo 2 thuộc tính đại diện cho 2 đối tượng : Account, Customer
        [BindProperty]
        public Customer customer { get;set; }
        [BindProperty]
        public Nabu_Mobile.Models.Account account { get; set; }

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var accountInDB = await prn221DBContext
                    .Accounts
                    .SingleOrDefaultAsync(x => x.Email.Equals(account.Email));
                if (accountInDB == null)
                {
                    var _customer = new Customer
                    {
                        CustomerId = generatedCustomerId(),
                        CompanyName = customer.CompanyName,
                        ContactName = customer.ContactName,
                        ContactTitle = customer.ContactTitle,
                        Address = customer.Address
                    };

                    var _account = new Nabu_Mobile.Models.Account
                    {
                        Email = account.Email,
                        Password = account.Password,
                        CustomerId = _customer.CustomerId,
                        Role = commonRole.CUSTOMER_ROLE
                    };

                    await prn221DBContext.Customers.AddAsync(_customer);
                    await prn221DBContext.Accounts.AddAsync(_account);
                    await prn221DBContext.SaveChangesAsync();
                    return RedirectToPage("/index");
                }
                    ViewData["message"] = "This account exists";
                    return Page();
            } else
            {
                return Page();
            }
        }

        private string generatedCustomerId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, lENGTH_CUSTOMER_ID)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
       
    }
}
