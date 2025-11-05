using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PointOfSale.Business.Contracts;
using PointOfSale.Business.Services;
using PointOfSale.Data.DBContext;
using PointOfSale.Data.Repository;
using PointOfSale.Utilities.Automapper;
using PointOfSale.Utilities.Extensions;
using Microsoft.AspNetCore.Identity;
using PointOfSale.Utilities.Initializer;
using PointOfSale.Model;
using Microsoft.AspNetCore.Identity.UI.Services;
using PointOfSale.Utilities.EmailSender;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<POINTOFSALEContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQL"));
});


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<POINTOFSALEContext>()
.AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Repository registrations
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

// Service registrations
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITypeDocumentSaleService, TypeDocumentSaleService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IDashBoardService, DashBoardService>();
builder.Services.AddScoped<IDBInitializer, DBInitializer>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

// PDF converter setup
try
{
    // Detect architecture
    bool is64Bit = Environment.Is64BitProcess;
    string architectureFolder = is64Bit ? "64bit" : "32bit";

    // Log architecture info
    Console.WriteLine($"=== PDF DLL Loading Info ===");
    Console.WriteLine($"Is 64-bit OS: {Environment.Is64BitOperatingSystem}");
    Console.WriteLine($"Is 64-bit Process: {Environment.Is64BitProcess}");
    Console.WriteLine($"Architecture: {RuntimeInformation.ProcessArchitecture}");
    Console.WriteLine($"Loading from folder: {architectureFolder}");

    // Build path to DLL
    var wkHtmlPath = Path.Combine(Directory.GetCurrentDirectory(), "Utilities", "LibraryPDF", architectureFolder);
    var dllPath = Path.Combine(wkHtmlPath, "libwkhtmltox.dll");

    Console.WriteLine($"DLL Path: {dllPath}");
    Console.WriteLine($"DLL Exists: {File.Exists(dllPath)}");

    if (!File.Exists(dllPath))
    {
        throw new FileNotFoundException($"Could not find libwkhtmltox.dll at: {dllPath}");
    }

    // Load the DLL
    var context = new CustomAssemblyLoadContext();
    context.LoadUnmanagedLibrary(dllPath);

    // Initialize COM for multi-threading
    const uint COINIT_MULTITHREADED = 0x0;
    CoInitializeEx(IntPtr.Zero, COINIT_MULTITHREADED);

    // Register PDF converter
    builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

    Console.WriteLine("PDF DLL loaded successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"!!! ERROR loading PDF DLL: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");

    // Register a dummy converter so app can still run
    builder.Services.AddSingleton(typeof(IConverter), provider =>
    {
        throw new InvalidOperationException("PDF generation is not available. DLL failed to load.");
    });
}

///
[DllImport("ole32.dll")]
static extern int CoInitializeEx(IntPtr pvReserved, uint dwCoInit);

//CoInitializeEx(IntPtr.Zero, COINIT_MULTITHREADED);
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddRazorPages(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Configure routing with proper order and fallbacks
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Dashboard}/{id?}");


// Catch-all route for authentication check
app.MapControllerRoute(
    name: "auth-check",
    pattern: "",
    defaults: new { controller = "Home", action = "CheckUserRole" });



// Default fallback route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



SeedDatabase();
app.Run();

void SeedDatabase()
{

    using (var scope = app.Services.CreateScope())
    {

        var dbinitializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
        dbinitializer.Initialize();
    }
}