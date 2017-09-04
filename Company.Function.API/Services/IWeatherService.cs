using System.ServiceModel;

namespace Company.Function.API.Services
{
    [ServiceContract]
    public interface IWeatherService
    {
        [OperationContract]
        string GetTemperature(string city);
    }
}
