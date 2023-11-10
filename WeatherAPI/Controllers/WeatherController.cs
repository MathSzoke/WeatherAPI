using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
	private readonly WeatherService _weatherService;

	public WeatherController(WeatherService weatherService)
	{
		_weatherService = weatherService;
	}

	/// <summary>
	/// Recupera temperaturas pré-configuradas como temperaturas atuais, temperatura máxima e temperatura mínima, por cidade.
	/// </summary>
	/// <param name="cityName"></param>
	/// <returns>Json de temperaturas</returns>
	[HttpGet]
	public async Task<IActionResult> GetWeatherByCity(string cityName)
	{
		try
		{
			string weatherData = await _weatherService.GetWeatherByCityAsync(cityName);
			return Ok(weatherData);
		}
		catch (Exception ex)
		{
			return StatusCode(500, ex.Message);
		}
	}
}
