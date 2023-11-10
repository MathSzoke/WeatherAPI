namespace WeatherAPI.Tests;

public class WeatherServiceTests
{
	private WeatherService WeatherService { get; set; } = null!;

	[SetUp]
	public void Setup()
	{
		WeatherService = new WeatherService(new HttpClient());
	}

	[Test]
	public void GetWeatherByCityAsync_ThatTest()
	{
		// Assign

		var cityName = "São Paulo";

		// Act

		var weather = WeatherService.GetWeatherByCityAsync(cityName);

		// Assert

		Assert.That(weather, Is.Not.Null);
	}
}