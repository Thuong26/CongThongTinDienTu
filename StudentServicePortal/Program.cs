using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using StudentServicePortal.Data;
using StudentServicePortal.Middlewares;
using StudentServicePortal.Repositories;
using StudentServicePortal.Services;
using StudentServicePortal.Services.Interfaces;
using StudentServicePortal.Services.Implementations;
using Microsoft.OpenApi.Models;
using StudentServicePortal.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using StudentServicePortal.Repositories.Interfaces;
using StudentServicePortal.Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình HTTP & HTTPS
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5037); // HTTP
        options.ListenAnyIP(7142, listenOptions => listenOptions.UseHttps()); // HTTPS
    });
}
else
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(80); // Chỉ HTTP cho production (Railway)
    });
}

// Lấy chuỗi kết nối từ cấu hình
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Kiểm tra nếu ConnectionString null hoặc rỗng
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("ConnectionString property has not been initialized.");
}

Console.WriteLine($"Connection String: {connectionString}");

// Đăng ký IDbConnection sử dụng Microsoft.Data.SqlClient
builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));

// Đăng ký DbContext
builder.Services.AddDbContext<StudentPortalDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    )
);

// Đăng ký Repository và Service
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddTransient<IRegistrationFormRepository, RegistrationFormRepository>();
builder.Services.AddTransient<IRegistrationFormService, RegistrationFormService>();
builder.Services.AddScoped<IFormRepository, FormRepository>();
builder.Services.AddScoped<IFormService, FormService>();
builder.Services.AddScoped<IRegulationRepository, RegulationRepository>();
builder.Services.AddScoped<IRegulationService, RegulationService>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IRegistrationDetailRepository, RegistrationDetailRepository>();
builder.Services.AddScoped<IRegistrationDetailService, RegistrationDetailService>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
// Đăng ký Introduction Repository và Service
builder.Services.AddScoped<IIntroductionRepository, IntroductionRepository>();
builder.Services.AddScoped<IIntroductionService, IntroductionService>();

// Add services to the container
builder.Services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();

// Cấu hình JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // Xóa ánh xạ mặc định nếu cần

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        RoleClaimType = "role",
        NameClaimType = JwtRegisteredClaimNames.Sub
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var identity = context.Principal.Identity as ClaimsIdentity;
            var username = identity?.Name;
            Console.WriteLine($"Token đã được xác thực cho người dùng: {username}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Xác thực JWT thất bại: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

// Đăng ký Controller và Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Student Service Portal API", Version = "v1" });

    // Cấu hình xác thực Bearer cho Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token theo định dạng: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNgrok", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin == "http://localhost:3000" || 
                origin.Contains(".ngrok-free.app"))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowNgrok");

app.UseMiddleware<ErrorHandlingMiddleware>();

// Bật Swagger cho cả Development và Production
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
