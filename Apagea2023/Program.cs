using Apagea2023.Models.Servicios;
using Apagea2023.Models.Servicios.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Servicio sqlServer
builder.Services.AddScoped<IDBAccess, SqlServerDBAccess>();

// Servicio mailJet
builder.Services.AddScoped<IClienteCorreo, MailJetServices>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cliente}/{action=Login}/{id?}");

app.Run();

