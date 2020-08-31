using System;
using LanSensor.PollingMonitor.Domain.Services;

namespace LanSensor.PollingMonitor.Application.Services
{
    public class ReadEnvironmentService : IReadEnvironmentService

    {
        public string GetEnvironmentVariable(string environmentName)
        {
            return Environment.GetEnvironmentVariable(environmentName);
        }
    }
}
