using Microsoft.EntityFrameworkCore;
using RentReady.API.Data;
using RentReady.API.Interfaces;
using RentReady.API.Mapping;
using RentReady.API.Services;

var builder = WebApplication.CreateBuilder(args);

//adding DbContext
builder.Services.AddDbContext<RentReadyContext>(options =>

    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<ILeaseService, LeaseService>();

//TODO other services... 

var app = builder.Build();
// TODO: Middleware

app.MapControllers();

app.Run();

// 
