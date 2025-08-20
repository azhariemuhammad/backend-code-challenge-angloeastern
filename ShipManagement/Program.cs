using ShipManagement.Configuration;
using ShipManagement.Middlewares;
using ShipManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// // Configure Kestrel to listen on all interfaces
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
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();
builder.Services.AddHostedService<ShipSimulatorService>();

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.UseMiddleware<ErrorHandlerMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ShipManagementContext>();
    context.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    try
    {
        var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
        Console.WriteLine("RabbitMQ publisher initialized successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing RabbitMQ publisher: {ex.Message}");
        // Handle or log the exception as needed
    }
}

app.Run();
