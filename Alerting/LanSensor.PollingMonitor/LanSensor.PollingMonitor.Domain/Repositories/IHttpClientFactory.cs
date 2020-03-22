using System.Net.Http;

namespace LanSensor.PollingMonitor.Domain.Repositories
{
    public interface IHttpClientFactory
    {
        HttpClient Build();
    }
}
