using Core.Config;
using Core.DB;
using Core.Models;
using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Hangfire.States;
using Hangfire.Storage;
using Logic.Helpers;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("ECollegeProject")));

builder.Services.AddSingleton<IEmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
builder.Services.AddSingleton<IGeneralConfiguration>(builder.Configuration.GetSection("GeneralConfiguration").Get<GeneralConfiguration>());
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("ECollegeHangFire")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(43800);
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddScoped<IUserHelper, UserHelper>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAccountHelper, AccountHelper>();
builder.Services.AddScoped<ITemplate, Template>();
builder.Services.AddScoped<ISuperAdminHelper, SuperAdminHelper>();
builder.Services.AddScoped<IDropDownHelper, DropdownHelper>();
builder.Services.AddScoped<IAccountHelper, AccountHelper>();
builder.Services.AddScoped<IPaymentHelper, PaymentHelper>();
builder.Services.AddScoped<IPaystackHelper, PaystackHelper>();

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

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
//UpdateDatabase(app);
app.UseAuthentication();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context?.Database.Migrate();
    var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var templateServices = services.GetRequiredService<ITemplate>();
    var roles = templateServices.DefaultRoles();
    foreach (var role in roles)
    {
        if (!await roleManger.RoleExistsAsync(role))
            await roleManger.CreateAsync(new IdentityRole(role));
    }

    try
    {
        var _onLoadExe = services.GetRequiredService<IAccountHelper>();
        _onLoadExe.CreateSuperAdminAccount();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while see the database.");
    }
}
app.Run();

//static void UpdateDatabase(IApplicationBuilder app)
//{
//    using (var serviceScope = app.ApplicationServices
//        .GetRequiredService<IServiceScopeFactory>()
//        .CreateScope())
//    {
//        using (var context = serviceScope.ServiceProvider.GetService<AppDbContext>())
//        {
//            context.Database.Migrate();
//        }
//    }
//}

//void HangFireConfiguration(IApplicationBuilder app)
//{
//    //var robotDashboardOptions = new DashboardOptions { Authorization = new[] { new MyAuthorizationFilter() } };
//dont add: //Enable Session.

//var robotOptions = new BackgroundJobServerOptions
//{
//    ServerName = String.Format(
//    "{0}.{1}",
//    Environment.MachineName,
//    Guid.NewGuid().ToString())
//};
//app.UseHangfireServer(robotOptions);
//var RobotStorage = new SqlServerStorage(builder.Configuration.GetConnectionString("ECollegeHangFire"));
//JobStorage.Current = RobotStorage;
//app.UseHangfireDashboard("/ECollegeEmails", robotDashboardOptions, RobotStorage);
//}

// This method delays successful and failed jobs on the hangfire dashboard  for 1 month(30 Days) 
//class ExpirationTimeAttribute : JobFilterAttribute, IApplyStateFilter
//{
//    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
//    {
//        context.JobExpirationTimeout = TimeSpan.FromDays(30);
//    }

//    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
//    {
//        context.JobExpirationTimeout = TimeSpan.FromDays(30);
//    }
//}

//class MyAuthorizationFilter : IDashboardAuthorizationFilter
//{
//    public bool Authorize(DashboardContext context)
//    {
//        return true;
//    }
//}