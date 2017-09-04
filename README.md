## Dynamics 365 Extensions Unit Testing
Examples which demonstrate how to unit test plugins and code activities easily with [FakeXrmEasy](https://github.com/jordimontana82/fake-xrm-easy).

### Example 1: Workflows and Web Services
This example demonstrates how to
 - mock a Web Service dependency when testing a Code Activity
 - invoke a Code Activity with `Input` parameters
 - `Assert` against an invoked Code Activity's `Output` parameters
 
<b>Scenario:</b> <b>SetCityTemperature.cs</b> is a workflow that when given the name of a city as an `Input`, returns the current temperature of the city as a friendly string `Output`.

The city's temperature is calculated by <b>Company.Function.API.Services</b>.

<b>IWeatherService.cs</b>
```cs
[ServiceContract]
public interface IWeatherService
{
    [OperationContract]
    string GetTemperature(string city);
}
```

<b>WeatherService.cs</b>
```cs
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
```

> Company.Function.API.Services is included here for demo purposes only, to simulate a Code Activity's dependency on an external API.

### Example 2: Plugins and testing with an in-memory CRM database 


