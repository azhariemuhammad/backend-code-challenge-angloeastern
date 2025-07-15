using Microsoft.EntityFrameworkCore;
using ShipManagement.Data;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<ShipManagementContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Controllers
builder.Services.AddControllers();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


app.UseHttpsRedirection();
// app.UseAuthorization();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => "Hello World!");

app.Run();
