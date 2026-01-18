using BackendSoftContable.Data;
using BackendSoftContable.Data.Repositories;
using BackendSoftContable.Interfaces.Services;
using BackendSoftContable.Mapping;
using BackendSoftContable.Middleware;
using BackendSoftContable.Repositories.ITerceroRepositories;
using BackendSoftContable.Repositories.ITercerosCategoria;
using BackendSoftContable.Repositories.TerceroCategoriaRepository;
using BackendSoftContable.Repositories.TerceroRepository;
using BackendSoftContable.Services;
using BackendSoftContable.Services.Auth;
using BackendSoftContable.Services.Colegio;
using BackendSoftContable.Services.Storage;
using BackendSoftContable.Services.TerceroService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHttpContextAccessor();

// 🔹 Repositorios
builder.Services.AddScoped<IColegioRepository, ColegioRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

// 🔹 Servicios
builder.Services.AddScoped<IColegioService, ColegioService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITerceroService, TerceroService>();
builder.Services.AddScoped<ITerceroRepository, TerceroRepository>();
builder.Services.AddScoped<ITerceroCategoriaRepository, TerceroCategoriaRepository>();
builder.Services.AddScoped<IAuditService, AuditService>();


// 🔹 Controllers
builder.Services.AddControllers();

// 🔹 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BackendSoftContable API",
        Version = "v1"
    });

    //  Configuración JWT para Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese el token JWT en el formato: Bearer {tu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// 🔹 DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQL_Connection")));

// 🔹 JWT CONFIGURATION
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

// 🔹 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseMiddleware<AuditAndErrorMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
