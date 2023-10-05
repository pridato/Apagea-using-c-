using Apagea2023.Models.Servicios;
using Apagea2023.Models.Servicios.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Servicio sqlServer
builder.Services.AddScoped<IDBAccess, SqlServerDBAccess>();

// Servicio mailJet
builder.Services.AddScoped<IClienteCorreo, MailJetServices>();

//acceso a la sesion
builder.Services.AddSession((SessionOptions opciones) =>
{
    opciones.Cookie.HttpOnly = true; // inhabilita acceso a través de javascript a las cookies,evitando por consiguiente su modif.
    opciones.Cookie.MaxAge = new TimeSpan(1, 0, 0); // a este tiempo se elimina toda la sesion 
});

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

app.UseSession(); // <-- 4º middleware para extraer la cabecera de la peticion http-request la cookie con el idsession para el user 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tienda}/{action=MostrarLibros}/{id?}");

app.Run();

