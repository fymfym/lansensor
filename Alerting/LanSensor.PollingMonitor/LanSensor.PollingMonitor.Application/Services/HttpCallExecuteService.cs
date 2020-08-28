using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;

namespace LanSensor.PollingMonitor.Application.Services
{
    public class HttpCallExecuteService : IHttpCallExecuteService
    {
        private readonly Uri _baseUrl;

        public HttpCallExecuteService(
            Uri baseUrl
            )
        {
            _baseUrl = baseUrl;
        }

        public HttpClient MakeHttpClient(LoginToken loginToken)
        {
            var httpClient = MakeHttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", loginToken.Token);
            return httpClient;
        }

        public HttpClient MakeHttpClient()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = _baseUrl
            };

            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        public async Task<string> HttpGet(LoginToken loginToken, string partialUrl)
        {
            var httpClient = MakeHttpClient(loginToken);
            var response = await httpClient.GetAsync(partialUrl);
            var str = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode) return str;

            Console.WriteLine(str);
            throw new Exception("No content to fetch");
        }

        public async Task HttpDelete(LoginToken loginToken, string partialUrl)
        {
            var httpClient = MakeHttpClient(loginToken);
            var response = await httpClient.DeleteAsync(partialUrl);
            var str = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("No content deleted");
            }
        }

        public StringContent BuildStringContent(string content)
        {
            return new StringContent(content,
                Encoding.UTF8,
                "application/json");
        }

        public async Task<string> HttpPut(LoginToken loginToken, string partialUrl, string body)
        {
            var httpClient = MakeHttpClient(loginToken);
            var response = await httpClient.PutAsync(partialUrl, BuildStringContent(body));
            var str = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) return str;

            throw new Exception("Environment not updated");
        }

        public async Task<string> HttpPost(LoginToken loginToken, string partialUrl, string body)
        {
            var httpClient = MakeHttpClient(loginToken);
            var content = BuildStringContent(body ?? "");
            var response = await httpClient.PostAsync(partialUrl, content);
            var str = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) return str;
            throw new Exception($"Data not created: {str}");
        }

        public async Task<string> HttpPost(string partialUrl, string body)
        {
            var httpClient = MakeHttpClient();
            var content = BuildStringContent(body);
            var response = await httpClient.PostAsync(partialUrl, content);
            var str = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) return str;
            throw new Exception($"Data not created: {str}");
        }
    }
}
