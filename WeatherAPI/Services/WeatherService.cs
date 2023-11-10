using System.Text.Json;
using WeatherAPI.Data;
using WeatherAPI.Models;

namespace WeatherAPI.Services;

/// <summary>
/// Serviço para obtenção de dados climáticos.
/// </summary>
public class WeatherService
{
	/// <summary>
	/// Cria uma nova instância do serviço de clima.
	/// </summary>
	/// <param name="httpClient">Cliente HTTP para fazer chamadas de API.</param>
	public WeatherService(HttpClient httpClient)
	{
		if (httpClient is null)
		{
			throw new ArgumentNullException(nameof(httpClient));
		}
	}

	/// <summary>
	/// Obtém os dados climáticos de uma cidade.
	/// </summary>
	/// <param name="cityName">Nome da cidade.</param>
	/// <returns>Os dados de temperatura atual, máxima e mínima em formato JSON.</returns>
	public async Task<string> GetWeatherByCityAsync(string cityName)
	{
		var db = new WeatherDbContext();

		var cachedData = TryGetCachedData(cityName, db);

		if (cachedData != null)
		{
			var temperatureJson = new
			{
				cachedData.CurrentTemperature,
				cachedData.MaxTemperature,
				cachedData.MinTemperature
			};

			return JsonSerializer.Serialize(temperatureJson);
		}

		var weatherData = await RequestWeatherDataFromApi(cityName, new HttpClient());

		SaveWeatherDataToDatabase(cityName, weatherData, db);

		return SerializeWeatherData(weatherData);
	}

	/// <summary>
	/// Tenta obter os dados climáticos da última consulta salva no cache.
	/// </summary>
	/// <param name="cityName">Nome da cidade.</param>
	/// <param name="db">Contexto do banco de dados.</param>
	/// <returns>Os dados climáticos em cache ou null se não houver.</returns>
	protected static WeatherData? TryGetCachedData(string cityName, WeatherDbContext db)
	{
		var lastUpdatedTime = DateTime.Now.AddMinutes(-20);

		return db.WeatherData
				 .Where(w => w.CityName == cityName && w.LastUpdated >= lastUpdatedTime)
				 .FirstOrDefault();
	}

	/// <summary>
	/// Faz uma chamada à API do OpenWeatherMap para obter os dados climáticos.
	/// </summary>
	/// <param name="cityName">Nome da cidade.</param>
	/// <returns>Os dados climáticos obtidos da API.</returns>
	protected static async Task<WeatherData> RequestWeatherDataFromApi(string cityName, HttpClient httpClient)
	{
		string apiUrl = $"http://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid=31503a0b2961cffcd70a67c76555b052";
		HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

		if (response.IsSuccessStatusCode)
		{
			string json = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<WeatherData>(json)!;
		}
		else
		{
			throw new Exception($"Erro ao chamar a API: {response.StatusCode}");
		}
	}

	/// <summary>
	/// Salva os dados climáticos no banco de dados.
	/// </summary>
	/// <param name="cityName">Nome da cidade.</param>
	/// <param name="weatherData">Os dados climáticos a serem salvos.</param>
	/// <param name="db">Contexto do banco de dados.</param>
	protected static void SaveWeatherDataToDatabase(string cityName, WeatherData weatherData, WeatherDbContext db)
	{
		double currentTemp = weatherData.ForecastList[0].Temperature.CurrentTemperature - 273.15;
		double minTemp = weatherData.ForecastList[0].Temperature.MinTemperature - 273.15;
		double maxTemp = weatherData.ForecastList[0].Temperature.MaxTemperature - 273.15;

		db.WeatherData.Add(new WeatherData
		{
			CityName = cityName,
			CurrentTemperature = Math.Round(currentTemp, 2),
			MinTemperature = Math.Round(minTemp, 2),
			MaxTemperature = Math.Round(maxTemp, 2),
			LastUpdated = DateTime.Now
		});

		db.SaveChanges();
	}

	/// <summary>
	/// Serializa os dados climáticos em formato JSON.
	/// </summary>
	/// <param name="weatherData">Os dados climáticos a serem serializados.</param>
	/// <returns>Os dados climáticos em formato JSON.</returns>
	protected static string SerializeWeatherData(WeatherData weatherData)
	{
		var temperatureJson = new
		{
			CurrentTemperature = Math.Round(weatherData.ForecastList[0].Temperature.CurrentTemperature - 273.15, 2),
			MinTemperature = Math.Round(weatherData.ForecastList[0].Temperature.MinTemperature - 273.15, 2),
			MaxTemperature = Math.Round(weatherData.ForecastList[0].Temperature.MaxTemperature - 273.15, 2)
		};

		return JsonSerializer.Serialize(temperatureJson);
	}
}
