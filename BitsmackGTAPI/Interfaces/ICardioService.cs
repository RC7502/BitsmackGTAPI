using System.Collections.Generic;

namespace BitsmackGTAPI.Models
{
    public interface ICardioService
    {
        CardioSummaryViewModel GetSummary();
        List<WeatherForecastViewModel> GetWeatherForecast();
    }
}