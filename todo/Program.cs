using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using todo.Data;
using todo.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(opts =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opts.Filters.Add(new AuthorizeFilter(policy));
});


builder.Services.AddDbContext<DataContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services
    .AddIdentityApiEndpoints<User>(opt =>
    {
        opt.User.RequireUniqueEmail = true;

    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddSignInManager();


builder.Services.AddAuthorization();


builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.Cookie.Name = "auth";
    opt.Cookie.HttpOnly = true;
    opt.Cookie.SameSite = SameSiteMode.Lax;
    opt.Cookie.SecurePolicy = CookieSecurePolicy.None;
});


builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = 429;
    o.AddFixedWindowLimiter("LoginLimiter", cfg => { cfg.PermitLimit = 5; cfg.Window = TimeSpan.FromMinutes(1); cfg.QueueLimit = 0; });
    o.AddFixedWindowLimiter("RegisterLimiter", cfg => { cfg.PermitLimit = 3; cfg.Window = TimeSpan.FromMinutes(1); cfg.QueueLimit = 0; });
    o.AddFixedWindowLimiter("TodoLimiter", cfg => { cfg.PermitLimit = 50; cfg.Window = TimeSpan.FromMinutes(1); cfg.QueueLimit = 5; });
});

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:5001", "http://localhost:5001", "http://127.0.0.1:5500")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();

app.UseCors();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.MapGroup("/api")
   .RequireRateLimiting("LoginLimiter")
   .MapIdentityApi<User>();

await Seed.SeedAsync(app.Services);
app.Run();