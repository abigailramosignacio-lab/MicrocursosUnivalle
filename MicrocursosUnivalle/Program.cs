using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MicrocursosUnivalle.Data;

var constructor = WebApplication.CreateBuilder(args);

// Cadena de conexión
var cadenaConexion = constructor.Configuration.GetConnectionString("ConexionPrincipal")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'ConexionPrincipal'.");

constructor.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer(cadenaConexion));

constructor.Services.AddIdentity<IdentityUser, IdentityRole>(opciones =>
{
    opciones.Password.RequireDigit = true;
    opciones.Password.RequiredLength = 6;
    opciones.Password.RequireNonAlphanumeric = false;
    opciones.Password.RequireUppercase = false;
    opciones.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

constructor.Services.ConfigureApplicationCookie(opciones =>
{
    opciones.LoginPath = "/Cuenta/Ingresar";
    opciones.LogoutPath = "/Cuenta/Salir";
    opciones.AccessDeniedPath = "/Cuenta/Denegado";
});

constructor.Services.AddControllersWithViews();
constructor.Services.AddRazorPages();

var aplicacion = constructor.Build();

using (var ambito = aplicacion.Services.CreateScope())
{
    var servicios = ambito.ServiceProvider;
    try
    {
        await MicrocursosUnivalle.Data.SeedData.InicializarRolesYAdmin(servicios);
    }
    catch (Exception ex)
    {
        var logger = servicios.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos.");
    }
}

if (!aplicacion.Environment.IsDevelopment())
{
    aplicacion.UseExceptionHandler("/Home/Error");
    aplicacion.UseHsts();
}

aplicacion.UseHttpsRedirection();
aplicacion.UseStaticFiles();
aplicacion.UseRouting();
aplicacion.UseAuthentication();
aplicacion.UseAuthorization();

aplicacion.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

aplicacion.MapRazorPages();

aplicacion.Run();
