using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WeatherAPI.Models;

public class WeatherData
{
	[JsonPropertyName("list")]
	[NotMapped]
	public Forecast[] ForecastList { get; set; }

	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	[Column("WeatherID", TypeName = "int")]
	public int WeatherID { get; set; }

	[Column("CityName", TypeName = "varchar(150)")]
	public string CityName { get; set; }

	[Column("CurrentTemperature", TypeName = "double")]
	public double CurrentTemperature { get; set; }

	[Column("MinTemperature", TypeName = "double")]
	public double MinTemperature { get; set; }

	[Column("MaxTemperature", TypeName = "double")]
	public double MaxTemperature { get; set; }

	[Column("LastUpdated", TypeName = "datetime")]
	public DateTime LastUpdated { get; set; }
}

public class Forecast
{
	[JsonPropertyName("main")]
	public TemperatureData Temperature { get; set; }

	[JsonPropertyName("dt_txt")]
	public string DateTimeText { get; set; }

	[JsonIgnore]
	public WeatherData WeatherData { get; set; }
}

public class TemperatureData
{
	[JsonPropertyName("temp")]
	public double CurrentTemperature { get; set; }

	[JsonPropertyName("temp_min")]
	public double MinTemperature { get; set; }

	[JsonPropertyName("temp_max")]
	public double MaxTemperature { get; set; }
}