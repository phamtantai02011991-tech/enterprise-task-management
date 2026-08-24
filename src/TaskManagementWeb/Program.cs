using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Services;
using TaskManagementWeb.Services.Admin;
using TaskManagementWeb.Services.Manager;

var builder = WebApplication.CreateBuilder(args);

//dang ki dbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));
   
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "CookieAuth";
    options.DefaultChallengeScheme = "CookieAuth";
})
.AddCookie("CookieAuth", options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Dang ki Services (Dependency Injection)
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<IAdminAnnouncementService, AdminAnnouncementService>();
builder.Services.AddScoped<TaskManagementWeb.Services.Common.INotificationService, TaskManagementWeb.Services.Common.NotificationService>();
builder.Services.AddScoped<IProjectManagementService, ProjectManagementService>();
builder.Services.AddScoped<ITaskManagementService, TaskManagementService>();

builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await DbSeeder.SeedAsync(context);
}

app.Run();