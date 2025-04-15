using TaskManagement.Models;
using TaskManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using TaskManagement.Services.Implementations;
using TaskManagement.Services.Interfaces;
using TaskManagement.Repositories.Implementations;
using TaskManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.SignalR;
using TaskManagement.Hubs;
using TaskManagement.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Thêm d?ch v? DbContext vào DI container và ??c chu?i k?t n?i t? appsettings.json
builder.Services.AddDbContext<TaskListDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskListConnection")));

// Đọc thông tin từ appsettings.json
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

// Cấu hình xác thực
builder.Services
	.AddAuthentication(options =>
	{
		options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
	})
	.AddCookie()
	.AddGoogle(googleOptions =>
	{
		googleOptions.ClientId = googleClientId;
		googleOptions.ClientSecret = googleClientSecret;
		googleOptions.CallbackPath = "/signin-google"; // Đường dẫn callback
	});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
builder.Services.AddScoped<IInvitationService, InvitationService>();

builder.Services.AddScoped<IGroupMemberRepository, GroupMemberRepositoy>();
builder.Services.AddScoped<IGroupMemberService, GroupMemberService>();

builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();

builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>(); // map userId với connection


var app = builder.Build();


// Khởi tạo CSDL nếu chưa có
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DatabaseInitializer.Initialize(services);
}

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

app.UseAuthentication();
app.UseAuthorization();

// Bắt buộc đăng nhập với tất cả controller
app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated && !context.Request.Path.StartsWithSegments("/auth/login"))
    {
        context.Response.Redirect("/auth/login");
        return;
    }
    await next();
});

app.MapHub<NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
