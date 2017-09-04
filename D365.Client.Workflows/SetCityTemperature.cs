using Company.Function.API.Services;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace D365.Client.Workflows
{
    /// <summary>
    /// Retrieves a city as an input string. Throws if the city is null or white space. 
    /// Gets the current temperature for the given city from a weather API. Outputs the city's
    /// current temperature as a friendly string.
    /// </summary>
    public sealed class SetCityTemperature : CodeActivity
    {   
        [Input("City")]
        public InArgument<string> City { get; set; }

        [Output("Temperature")]
        public OutArgument<string> Temperature { get; set; }

        public IWeatherService WeatherService { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var city = City.Get(context);

            if (string.IsNullOrWhiteSpace(city))
            {
                throw new InvalidWorkflowException("City passed to the workflow was null or whitespace.");
            }

            var temperature = WeatherService.GetTemperature(city);

            Temperature.Set(context, $"The temperature in {city} is {temperature}");
        }
    }
}
