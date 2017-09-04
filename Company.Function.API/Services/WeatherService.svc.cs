using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Company.Function.API.Services
{
    public class WeatherService : IWeatherService
    {
        public string GetTemperature(string city)
        {
            if ("Bristol" == city)
            {
                return "4";
            }

            else
            {
                return "24";
            }
        }
    }
}
