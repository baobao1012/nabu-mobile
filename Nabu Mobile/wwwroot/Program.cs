using MyRazorPage;
using MyRazorPage.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

//Bổ sung 1 service làm việc với các pages vào container Kestrel
builder.Services.AddRazorPages();

//add DBcontext
builder.Services.AddDbContext<PRN221_DBContext>();
builder.Services.AddSignalR();
//add session
builder.Services.AddSession(opt => opt.IdleTimeout = TimeSpan.FromMinutes(5));

var app = builder.Build();
app.UseStaticFiles();
app.MapRazorPages();
app.MapHub<HubServer>("/hub");
app.UseSession();
app.Run();
