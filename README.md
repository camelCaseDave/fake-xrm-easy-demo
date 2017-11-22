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

The workflow <b>SetCityTemperature</b> is dependent on <b>IWeatherService</b> so it is added as a property:
```cs
public sealed class SetCityTemperature : CodeActivity
{
    public IWeatherService WeatherService { get; set; }
    ...
}
```
`WeatherService` can now be passed to the workflow (when it is invoked) either as a <i>real</i> version, or as a mocked version in your tests.

The full <b>SetCityTemperature.cs</b> class looks as follows:
```cs
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
```

In your tests class, mock the <b>IWeatherService</b> by instantiating a fake instance of it, and specifying what a call to the service should return in your tests:

```cs
IWeatherService WeatherService = A.Fake<IWeatherService>();

A.CallTo(() => WeatherService.GetTemperature("Bristol")).ReturnsLazily((string city) =>
{
    return "4";
});
```

Finally create a test that invokes the <b>SetCityTemperature</b> workflow and asserts against the temperature formatted as a friendly string:
```cs
[Test]
public void Outputs_4_For_Bristol()
{
    var inputs = new Dictionary<string, object>()
    {
        { "City", "Bristol" }
    };

    var results = Context.ExecuteCodeActivity<SetCityTemperature>(inputs, TemperatureWorkflow);
            
    Assert.AreEqual(results["Temperature"], "The temperature in Bristol is 4");
}
```

### Example 2: Plugins and testing with an in-memory CRM database 

### Dependencies
Dependencies can be viewed in the individual project's `packages.config` file. They will be installed automatically when the solution is built. 

This solution requires the following [nuget](https://www.nuget.org/) packages:
- FakeItEasy
- FakeXrmEasy
- Microsoft.CrmSdk.CoreAssemblies
- Microsoft.CrmSdk.Workflow


