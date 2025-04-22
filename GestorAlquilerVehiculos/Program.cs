using GestorAlquilerVehiculos.Data;
using Microsoft.EntityFrameworkCore;
using GestorAlquilerVehiculos.Services;
using GestorAlquilerVehiculos.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<GestorAlquilerVehiculosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GestorAlquilerVehiculosDb")));

builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailConfiguration>>().Value);
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<NotificacionDevolucionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
