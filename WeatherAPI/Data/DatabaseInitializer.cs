namespace WeatherAPI.Data;

public class DatabaseInitializer
{
	public static void Initialize(WeatherDbContext context)
	{
		// Cria o banco de dados se não existir
		context.Database.EnsureCreated();
	}
	public static string GetDefaultConnectionString()
	{
		return $"Server=localhost;Database=WeatherDB;User=root;Password=root;";
	}
}
