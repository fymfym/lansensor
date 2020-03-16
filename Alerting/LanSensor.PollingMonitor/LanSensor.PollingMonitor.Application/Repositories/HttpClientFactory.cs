using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using LanSensor.PollingMonitor.Domain.Repositories;

namespace LanSensor.PollingMonitor.Application.Repositories
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly IServiceConfiguration _configuration;

        public HttpClientFactory(
            IServiceConfiguration configuration
            )
        {
            _configuration = configuration;
        }

        public HttpClient Build()
        {
            var httpClient = new HttpClient {
                BaseAddress = new Uri(_configuration.ApplicationConfiguration.RestServiceConfiguration.DeviceRestApiBasePath)
                };

            return httpClient;
        }
    }
}
