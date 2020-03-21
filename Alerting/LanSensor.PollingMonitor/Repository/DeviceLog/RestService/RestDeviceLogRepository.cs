using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.Repository.Repositories;
using Newtonsoft.Json;

namespace LanSensor.Repository.DeviceLog.RestService
{
    public class RestDeviceLogRepository : IDeviceLogRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RestDeviceLogRepository(
            IHttpClientFactory httpClientFactory
            )
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DeviceLogEntity> GetLatestPresence(string deviceGroupId, string deviceId)
        {
            var client = _httpClientFactory.Build();
            var response =
                await client.GetAsync(
                    $"sensors/api/?devicegroupid={deviceGroupId}&deviceid={deviceId}");
            var str = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<IEnumerable<DeviceLogEntity>>(str);
            return res.OrderByDescending(x => x.DateTime).FirstOrDefault();
        }

        public async Task<DeviceLogEntity> GetLatestKeepAlive(string deviceGroupId, string deviceId)
        {
            var client = _httpClientFactory.Build();
            var response =
                await client.GetAsync(
                    $"sensors/api/?devicegroupid={deviceGroupId}&deviceid={deviceId}&datatype=keepalive");
            var str = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<IEnumerable<DeviceLogEntity>>(str);
            return res.OrderByDescending(x => x.DateTime).FirstOrDefault();
        }

        public async Task<DeviceLogEntity> GetLatestPresence(string deviceGroupId, string deviceId, string dataType)
        {
            var client = _httpClientFactory.Build();
            var response =
                await client.GetAsync(
                    $"sensors/api/?devicegroupid={deviceGroupId}&deviceid={deviceId}&datatype={dataType}");
            var str = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<IEnumerable<DeviceLogEntity>>(str);
            return res.OrderByDescending(x => x.DateTime).FirstOrDefault();
        }

        public async Task<IEnumerable<DeviceLogEntity>> GetPresenceListSince(string deviceGroupId, string deviceId, DateTime lastKnownPresence)
        {
            var client = _httpClientFactory.Build();
            var response =
                await client.GetAsync(
                    $"sensors/api/?devicegroupid={deviceGroupId}&deviceid={deviceId}");
            var str = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<IEnumerable<DeviceLogEntity>>(str);
            return res;
        }

        public async Task<IEnumerable<DeviceLogEntity>> GetPresenceListSince(string deviceGroupId, string deviceId, string dataType, DateTime lastKnownPresence)
        {
            var client = _httpClientFactory.Build();
            var response =
                await client.GetAsync(
                    $"sensors/api/?devicegroupid={deviceGroupId}&deviceid={deviceId}&dataType={dataType}");
            var str = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<IEnumerable<DeviceLogEntity>>(str);
            return res;
        }
    }
}
