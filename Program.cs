using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Areas.Data;
using Pagina_proyecto.Services;




var builder = WebApplication.CreateBuilder(args);

// Cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Servicios personalizados

builder.Services.AddScoped<RecomendacionService>();

// Google Drive API
builder.Services.AddScoped<DriveService>(sp =>
{
    var credential = GoogleCredential
        .FromFile("google-credentials.json")
        .CreateScoped(DriveService.ScopeConstants.DriveReadonly);

    return new DriveService(new BaseClientService.Initializer
    {
        HttpClientInitializer = credential,
        ApplicationName = "PaginaProyecto"
    });
});

// Servicio propio para Drive
builder.Services.AddScoped<GoogleDriveService>();


// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins("https://example.com")
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();
// 🔥 IMPORTANTE para HTTPS detrás de Nginx
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Rutas
app.MapControllerRoute(
    name: "planes",
    pattern: "planes/{idCarrera?}",
    defaults: new { controller = "Materias", action = "Cronograma" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapControllers();

// Crear roles si no existen
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Operador" };

    foreach (var role in roles)
    {
        var roleExists = await roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.Run();
