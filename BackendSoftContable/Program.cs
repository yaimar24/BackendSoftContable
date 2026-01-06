using BackendSoftContable.Data;
using BackendSoftContable.Data.Repositories;
using BackendSoftContable.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Repositorios
builder.Services.AddScoped<IColegioRepository, ColegioRepository>();

// Servicios
builder.Services.AddScoped<IColegioService, ColegioService>();
builder.Services.AddScoped<IUsuarioRepository,UsuarioRepository>();
// 🔹 Controllers
builder.Services.AddControllers();

// 🔹 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQL_Connection")));

// 🔹 Repositories
builder.Services.AddScoped<IColegioRepository, ColegioRepository>();

var app = builder.Build();

// 🔹 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
