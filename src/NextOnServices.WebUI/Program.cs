using Dapper;


using NCR = NextOnServices.Core.Repository;
using NS = NextOnServices.Services;
using NSC = NextOnServices.Services.Configurations;
using NSD = NextOnServices.Services.DBContext;

using GRPCR = GRP.Core.Repository;
using GRPS = GRP.Services;
using GRPSC = GRP.Services.Configurations;
using GRPSD = GRP.Services.DBContext;

using System.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using NextOnServices.Infrastructure.BL;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddControllers().AddControllersAsServices();

builder.Services.AddDbContext<NSD.NextOnServicesDbContext>();
builder.Services.AddScoped<NSD.DapperDBSetting>();
builder.Services.AddScoped<NCR.IUnitOfWork, NSD.UnitOfWork>();
builder.Services.AddAutoMapper(typeof(NSC.MapperInitializer));


builder.Services.AddDbContext<GRPSD.GRPDbContext>();
builder.Services.AddScoped<GRPSD.DapperDBSetting>();
builder.Services.AddScoped<GRPCR.IUnitOfWork, GRPSD.UnitOfWork>();
builder.Services.AddAutoMapper(typeof(GRPSC.MapperInitializer));


builder.Services.AddHttpContextAccessor();
//SqlMapper.AddTypeHandler(new SqlTimeOnlyTypeHandler());
//SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.ExpireTimeSpan = TimeSpan.FromSeconds(5); // Session expiration time
//        options.SlidingExpiration = true; // Sliding expiration (extend expiration on activity)
//        options.AccessDeniedPath = "/VT/Account/ErrorMessage"; // Access denied path
//        options.LoginPath = "/VT/Account/Login"; // Default login path

//        // Custom login redirection based on LoginSource claim
//        options.Events.OnRedirectToLogin = async context =>
//        {
//            var loginSourceClaim = context.HttpContext.User.Claims
//                .FirstOrDefault(c => c.Type == "LoginSource")?.Value;

//            if (loginSourceClaim == "SupplierLogin")
//            {
//                // Redirect to the Supplier login page if LoginSource is SupplierLogin
//                context.Response.Redirect("/VT/Supplier/Login");
//            }
//            else
//            {
//                // Default redirect for other sources
//                context.Response.Redirect("/VT/Account/Login");
//            }

//            await Task.CompletedTask;
//        };
//    });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session expiration time
        options.SlidingExpiration = true; // Sliding expiration (extend expiration on activity)
        options.AccessDeniedPath = "/VT/Account/ErrorMessage"; // Access denied path
        options.LoginPath = "/VT/Account/Login"; // Default login path

        // Ensure cookie settings are correct
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Ensure cookie is sent over HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax; // Adjust SameSite policy based on your needs
        options.Cookie.HttpOnly = true; // Ensure the cookie is not accessible via JavaScript

        // Custom login redirection based on LoginSource claim
        options.Events.OnRedirectToLogin = async context =>
        {
            var loginSourceClaim = context.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == "LoginSource")?.Value;

            if (string.IsNullOrEmpty(loginSourceClaim))
            {
                // If claims are not available, try fetching the cookie directly
                var cookie = context.HttpContext.Request.Cookies[CookieAuthenticationDefaults.AuthenticationScheme];

                if (string.IsNullOrEmpty(cookie))
                {
                    // Cookie is missing, the user is likely unauthenticated
                    // Redirect to the default login page
                    context.Response.Redirect("/VT/Account/Login");
                }
                else
                {
                    // If the cookie exists, you may want to parse it or check for relevant claims
                    // You might need a custom logic here to parse the cookie or use a different fallback
                }
            }
            else if (loginSourceClaim == "SupplierLogin")
            {
                // Redirect to the Supplier login page if LoginSource is SupplierLogin
                context.Response.Redirect("/VT/Supplier/Login");
            }
            else
            {
                // Default redirect for other sources
                context.Response.Redirect("/VT/Account/Login");
            }

            await Task.CompletedTask;
        };
    });




var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


//app.UseMiddleware<LoginSourceMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
//For Sessions
app.UseSession();
//For Sessions end
app.MapRazorPages();

app.UseEndpoints(endpoints =>
{

    //endpoints.MapControllerRoute(
    //name: "GRP",
    //pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    //endpoints.MapControllerRoute(
    //    name: "default",
    //    pattern: "{controller=Account}/{action=Login}/{id?}");


    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");

    endpoints.MapGet("/", context =>
    {
        context.Response.Redirect("/GRP/Home/Index");
        return Task.CompletedTask;
    });
});
app.Run();


