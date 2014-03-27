using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Models
{
    public class WeatherForecastViewModel
    {
        public string ForecastDate { get; set; }
        public string Summary { get; set; }
        public string Temperature { get; set; }
        public string ChanceOfPrecip { get; set; }
        public string WindSpeed { get; set; }
    }
}