using System.Net.Http;
using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Domain.Services
{
    public interface IHttpCallExecuteService
    {
        HttpClient MakeHttpClient(LoginToken loginToken);
        HttpClient MakeHttpClient();
        Task<string> HttpGet(LoginToken loginToken, string partialUrl);
        Task HttpDelete(LoginToken loginToken, string partialUrl);
        StringContent BuildStringContent(string content);
        Task<string> HttpPut(LoginToken loginToken, string partialUrl, string body);
        Task<string> HttpPost(LoginToken loginToken, string partialUrl, string body);
        Task<string> HttpPost(string partialUrl, string body);
    }
}
