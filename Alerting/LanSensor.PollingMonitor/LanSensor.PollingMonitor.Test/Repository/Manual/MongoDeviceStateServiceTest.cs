using System.Threading.Tasks;
using AutoMapper;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.Repository.DeviceState.MongoDb;
using LanSensor.Repository.MappingProfiles;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Repository.Manual
{
    public class MongoDeviceStateServiceTest
    {
        private string _deviceGroupId = "deviceGroupIdTest";
        private string _deviceId = "deviceIdTest";

        [Fact(Skip = "Manual")]
        public async Task GetDeviceState()
        {
            IServiceConfiguration appConfig = new ServiceConfiguration();

            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile<InfrastructureAutoMapProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var service = new MongoDeviceStateService(appConfig, mapper);

            var task = await service.GetLatestDeviceStateEntity(_deviceGroupId, _deviceId);
            Assert.NotNull(task);
        }

        [Fact(Skip = "Manual")]
        public async Task SetDeviceState()
        {
            IServiceConfiguration appConfig = new ServiceConfiguration();

            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile<InfrastructureAutoMapProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var service = new MongoDeviceStateService(appConfig, mapper);

            var res = await service.SetDeviceStateEntity(new DeviceStateEntity()
            {
                DeviceGroupId = _deviceGroupId,
                DeviceId = _deviceId
            });

            Assert.NotNull(res);
        }
    }
}
