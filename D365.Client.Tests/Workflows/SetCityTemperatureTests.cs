using System;
using NUnit.Framework;
using Company.Function.API.Services;
using FakeItEasy;
using FakeXrmEasy;
using D365.Client.Workflows;
using System.Collections.Generic;
using System.Activities;

namespace D365.Client.Tests
{
    /// <summary>
    /// Mocks an API from its provided interface.
    /// Mocks the CRM Context.
    /// Mocks CRM workflow input parameters and workflow invocation to retrieve and assert against workflow outputs.
    /// </summary>
    [TestFixture]
    public class SetCityTemperatureTests
    {
        private XrmFakedContext Context { get; set; }
        private SetCityTemperature TemperatureWorkflow { get; set; }
        private IWeatherService WeatherService { get; set; }

        [SetUp]
        public void Init()
        {
            Context = new XrmFakedContext();

            WeatherService = A.Fake<IWeatherService>();

            A.CallTo(() => WeatherService.GetTemperature("Bristol")).ReturnsLazily((string city) =>
            {
                return "4";
            });

            A.CallTo(() => WeatherService.GetTemperature("Ney York")).ReturnsLazily((string city) =>
            {
                return "24";
            });

            TemperatureWorkflow = new SetCityTemperature
            {
                WeatherService = WeatherService
            };
        }

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

        [Test]
        public void Throws_When_City_Is_Whitespace()
        {
            var inputs = new Dictionary<string, object>()
            {
                { "City", "" }
            };

            Assert.Throws<InvalidWorkflowException>(() => Context.ExecuteCodeActivity<SetCityTemperature>(inputs, TemperatureWorkflow));
        }
    }
}
