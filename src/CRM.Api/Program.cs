using System.Text;
using CRM.Api.Middleware;
using CRM.Application;
using CRM.Infrastructure;
using CRM.Infrastructure.Identity;
using CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SigningKey"]!));

builder.Services
    .AddAuthentication(options =>
    {
        // AddIdentity() (ở Infrastructure) tự đặt cookie làm scheme mặc định — ghi đè lại
        // thành JWT Bearer vì project này không dùng cookie login.
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // .NET 8+ mặc định KHÔNG map claim rút gọn trong token ("role", "email"...) sang
        // ClaimTypes.* dài — bật lại để [Authorize(Roles=...)]/User.IsInRole() hoạt động đúng.
        options.MapInboundClaims = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Chạy ở mọi môi trường — role, permission và ma trận gán quyền là cấu trúc hệ thống,
// không phải dữ liệu demo, nên không gói trong khối IsDevelopment() dưới đây.
using (var roleSeedScope = app.Services.CreateScope())
{
    var roleManager = roleSeedScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var dbContext = roleSeedScope.ServiceProvider.GetRequiredService<AppDbContext>();

    await IdentitySeeder.SeedRolesAsync(roleManager);
    await IdentitySeeder.SeedPermissionsAsync(dbContext);
    await IdentitySeeder.SeedRolePermissionsAsync(dbContext, roleManager);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(db);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await IdentitySeeder.SeedDevAdminAsync(userManager);
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
