using Microsoft.EntityFrameworkCore;
using WeatherAPI.Models;

namespace WeatherAPI.Data;

public class WeatherDbContext : DbContext
{
	public WeatherDbContext() { }

	public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

	public DbSet<WeatherData> WeatherData { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
		.UseMySql(DatabaseInitializer.GetDefaultConnectionString(), new MySqlServerVersion(new Version(5, 6)));

	protected override void OnModelCreating(ModelBuilder builder)
	{
		if (builder is null)
		{
			throw new ArgumentNullException(nameof(builder));
		}

		var fks = builder
			 .Model
			 .GetEntityTypes()
			 .SelectMany(t => t.GetForeignKeys())
			 .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

		foreach (var item in fks)
		{
			item.DeleteBehavior = DeleteBehavior.Restrict;
		}

		base.OnModelCreating(builder);

		_ = builder
			.Entity<WeatherData>()
			.HasKey(o => o.WeatherID);

		_ = builder
			.Entity<WeatherData>()
			.HasIndex(o => o.CityName);
	}
}