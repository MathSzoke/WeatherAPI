using Microsoft.EntityFrameworkCore;
using WeatherAPI.Data;
using WeatherAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<WeatherService>();

builder.Services.AddDbContext<WeatherDbContext>(options =>
	options.UseMySql(DatabaseInitializer.GetDefaultConnectionString(), new MySqlServerVersion(new Version(5, 6))));

var app = builder.Build();

// Setup the database
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var dbContext = services.GetRequiredService<WeatherDbContext>();
	DatabaseInitializer.Initialize(dbContext);
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
