using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaCST.Data;

namespace PracticaCST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly SocMediaDb _mediaDb;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, SocMediaDb mediaDb)
        {
            _logger = logger;
            _mediaDb = mediaDb;
    }

        [Authorize]
        [HttpGet("GiveWeatherOnDays")] 
        public IEnumerable<WeatherForecast> Get(int numberOfDays, bool isHoliday)
        {
            var users = _mediaDb.Users.ToList();

            var toReturn = Enumerable.Range(1, numberOfDays).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
            return toReturn;
        }
    }
}