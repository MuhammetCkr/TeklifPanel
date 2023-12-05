using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PdfSharp.Charting;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Business.Concrete;
using TeklifPanel.Data.Abstract;
using TeklifPanel.Data.Concrete.EfCore;
using TeklifPanel.Entity;
using TeklifPanelWebUI.Models;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<TeklifPanelContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnectionLocal")));
//builder.Services.AddDbContext<TeklifPanelContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnectionTest")));
//builder.Services.AddDbContext<TeklifPanelContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));
builder.Services.AddDbContext<TeklifPanelContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SqlLiteConnection")));


builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<TeklifPanelContext>().AddDefaultTokenProviders();

builder.Services.AddSession();

builder.Services.Configure<IdentityOptions>(options =>
{
    //Login ayarlar�
    options.Lockout.MaxFailedAccessAttempts = 5; //En fazla 5 kez giri� denemesi yaps�n. 5 kez hatal� giri� yapm�� olursa hesab� kilitlensin!
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);//Yeniden giri� yapabilmek i�in 5 dk ge�mesi gerek
    options.Lockout.AllowedForNewUsers = true;//Her yeni kullan�c� i�in hesap kilitlemeyi devreye al

    //User ayarlar�
    options.User.RequireUniqueEmail = true; //Benzersiz email adresi ile kay�t olunabilir
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Index"; // E�er a��labilmesi i�in login olman�n zorunluu olduu bir sayfaya gireye �al���yorsa, y�nlendirilecek login sayfas�na y�nlendirir
    //options.LogoutPath = "/account/logout"; // ��k�� yapt�ktan sonra gidilecek sayfaya y�nlendirir
    //options.AccessDeniedPath = "/account/accessdenied"; // yetkisiz giri� s�ras�nda y�nlendirilecek sayfa
    options.SlidingExpiration = true; // ��k�� i�in varsay�lan olan 20 dkn�n her istekte s�f�rlanams�n� sa�l�yoruz.
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10); // varsay�lan olan 20 dk l�k s�reyi bu kod ile 10 dk s�reye d���r�r.

});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    
});


builder.Services.AddScoped<IAddressRepository, EfCoreAddressRepository>();
builder.Services.AddScoped<ICategoryRepository,EfCoreCategoryRepository>();
builder.Services.AddScoped<ICompanyRepository,EfCoreCompanyRepository>();
builder.Services.AddScoped<IContactPersonRepository,EfCoreContactRepository>();
builder.Services.AddScoped<ICustomerRepository,EfCoreCustomerRepository>();
builder.Services.AddScoped<IImageRepository,EfCoreImageRepository>();
builder.Services.AddScoped<IOfferRepository,EfCoreOfferRepository>();
builder.Services.AddScoped<IProductRepository, EfCoreProductRepository>();
builder.Services.AddScoped<IUserRepository,EfCoreUserRepository>();
builder.Services.AddScoped<ICompanySettingsRepository,EfCoreCompanySettingsRepository>();
builder.Services.AddScoped<IIbanRepository, EfCoreIbanRepository>();
builder.Services.AddScoped<ILogRepository, EfCoreLogRepository>();
builder.Services.AddScoped<IOfferTableRepository, EfCoreOfferTableRepository>();
builder.Services.AddScoped<IProductOffersRepository, EfCoreProductOffersRepository>();

builder.Services.AddScoped<IAddressService, AddressManager>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<ICompanyService, CompanyManager>();
builder.Services.AddScoped<IContactPersonService, ContactPersonManager>();
builder.Services.AddScoped<ICustomerService, CustomerManager>();
builder.Services.AddScoped<IImageService, ImageManager>();
builder.Services.AddScoped<IOfferService, OfferManager>();
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<ICompanySettingsService, CompanySettingsManager>();
builder.Services.AddScoped<IIbanService, IbanManager>();
builder.Services.AddScoped<ILogService, LogManager>();
builder.Services.AddScoped<IOfferTableService, OfferTableManager>();
builder.Services.AddScoped<IProductOffersService, ProductOffersManager>();

builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


// Merkez Bankas� D�viz Kurlar�
builder.Services.AddTransient<IWritableOptions<CurrenciesModel>>(provider =>
{
    string file = "appsettings.json";
    var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
    var environment = provider.GetService<IWebHostEnvironment>();
    var options = provider.GetService<IOptionsMonitor<CurrenciesModel>>();
    return new WritableOptions<CurrenciesModel>(environment, options, configuration, "CurrenciesModel", file);
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

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name:"ProductCard",
    defaults: new {controller="Product", action="ProductCard"},
    pattern: "product/productcard/{url}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
