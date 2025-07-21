using Microsoft.EntityFrameworkCore;
using RentReady.API.Data;
using RentReady.API.Interfaces;
using RentReady.API.Mapping;
using RentReady.API.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<RentReadyContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger/OpenAPI setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RentReady API", Version = "v1" });
});

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<ILeaseService, LeaseService>();
builder.Services.AddScoped<ITenantService, TenantService>();

// TODO: other services...

var app = builder.Build();

// Configure Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RentReady API v1");
    });
}

// TODO: other middleware (e.g., app.UseHttpsRedirection(), app.UseAuthorization())

app.MapControllers();

app.Run();