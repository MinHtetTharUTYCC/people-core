using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PeopleCore.Api.Middleware;
using PeopleCore.Api.Services;
using PeopleCore.Application.Attendance.Services;
using PeopleCore.Application.Common;
using PeopleCore.Application.Departments.Services;
using PeopleCore.Application.Employees.Services;
using PeopleCore.Application.Identity;
using PeopleCore.Application.Identity.Services;
using PeopleCore.Application.Leave.Services;
using PeopleCore.Application.Payroll.Services;
using PeopleCore.Domain.Identity;
using PeopleCore.Infrastructure.Data;
using PeopleCore.Infrastructure.Security;
using PeopleCore.Infrastructure.Services;
using System.Text;
using System.Text.Json.Serialization;
using DbContext = PeopleCore.Infrastructure.Data.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<DbContext>()
.AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings.GetValue<string>("SecretKey") ?? "YourDefaultSecretKeyThatIsAtLeast32CharactersLong!";
var issuer = jwtSettings.GetValue<string>("Issuer") ?? "PeopleCore";
var audience = jwtSettings.GetValue<string>("Audience") ?? "PeopleCore";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(RoleConstants.Admin));
    options.AddPolicy("HROnly", policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.HR));
    options.AddPolicy("EmployeeOnly", policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.HR, RoleConstants.Employee));
});

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ApiResponse<object>>();
builder.Services.AddFluentValidationAutoValidation();

// CORS
builder.Services.AddCors(options =>
{   
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();

var app = builder.Build();

// Configure the HTTP request pipeline
// Temporarily disabled Swagger due to OpenApi compatibility issue
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseExceptionHandling();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DbContext>();
    db.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedData.SeedRolesAsync(roleManager);
}

app.Run();
