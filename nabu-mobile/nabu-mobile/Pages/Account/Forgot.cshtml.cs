using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyRazorPage.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;

namespace MyRazorPage.Pages.Account
{
    public class ForgotModel : PageModel
    {
        private readonly PRN221_DBContext prn221DBContext;
        private readonly Random _random = new();
        private readonly int passwordLength = 5;


        public ForgotModel(PRN221_DBContext prn221DBContext)
           => this.prn221DBContext = prn221DBContext;

        [BindProperty]
        public Models.Account acc { get; set; }

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPost()
        {
            bool isSend = false;
            string resetPassword = generatedPassword();
            acc = await findByEmail(acc.Email);
            if (acc is not null)
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient();
                    mail.From = new MailAddress("huynthe153411@fpt.edu.vn");
                    mail.To.Add(acc.Email);
                    mail.Subject = "Password Recovery";
                    mail.Body = string
                        .Format("Hi {0},<br /><br />Your reset password is {1}.<br /><br />Thank You.", acc.Email, resetPassword);
                    mail.IsBodyHtml = true;
                    SmtpServer.UseDefaultCredentials = false;
                    NetworkCredential NetworkCred = new NetworkCredential("huynthe153411@fpt.edu.vn", "nhrssjrvwuchzwut");
                    SmtpServer.Credentials = NetworkCred;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Port = 587;
                    SmtpServer.Host = "smtp.gmail.com";
                    SmtpServer.Send(mail);
                    ViewData["message"] = "Send mail success";
                    isSend = true;
                }
                catch (Exception e)
                {
                    ViewData["message"] = "Send mail error";
                    isSend = false;
                }

            }
            else
            {
                ViewData["message"] = "Email doesn't register";
                return Page();
            }

            if (isSend)
            {
                acc.Password = resetPassword;
                await prn221DBContext.SaveChangesAsync();
            }

            return Page();
        }

        private async Task<Models.Account?> findByEmail(String? email)
        {
            
            var accountInDB = await prn221DBContext.Accounts
                .FirstOrDefaultAsync(x => x.Email == email);

            if (accountInDB is not null)
            {
                return accountInDB;
            }
            return null;
        }

        private String generatedPassword()
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[passwordLength];
            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }
    }
}
