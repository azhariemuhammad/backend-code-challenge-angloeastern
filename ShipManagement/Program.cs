using Microsoft.EntityFrameworkCore;
using ShipManagement.Data;
using ShipManagement.Middlewares;
using ShipManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on all interfaces
// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(8080);
// });


// DB
builder.Services.AddDbContext<ShipManagementContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Redis"));

// Controllers
builder.Services.AddControllers();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddScoped<IShipService, ShipService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRedisCacheService, RedisService>();


var app = builder.Build();



app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ShipManagementContext>();
    context.Database.Migrate();
}

app.Run();
