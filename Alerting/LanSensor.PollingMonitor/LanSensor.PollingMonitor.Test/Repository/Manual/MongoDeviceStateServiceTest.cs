using System.Threading.Tasks;
using AutoMapper;
using LanSensor.PollingMonitor.Application.Services;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.PollingMonitor.Infrastructure.DeviceState.MongoDb;
using LanSensor.PollingMonitor.Infrastructure.MappingProfiles;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Repository.Manual
{
    public class MongoDeviceStateServiceTest
    {
        private const string DeviceGroupId = "deviceGroupIdTest";
        private const string DeviceId = "deviceIdTest";

        [Fact(Skip = "Manual")]
        public async Task GetDeviceState()
        {
            IReadEnvironmentService readEnv = new ReadEnvironmentService();
            IServiceConfiguration appConfig = new ServiceConfiguration(readEnv);

            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile<InfrastructureAutoMapProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var service = new MongoDeviceStateService(appConfig, mapper);

            var task = await service.GetDeviceState(DeviceGroupId, DeviceId);
            Assert.NotNull(task);
        }

        [Fact(Skip = "Manual")]
        public async Task SetDeviceState()
        {
            IReadEnvironmentService readEnv = new ReadEnvironmentService();

            IServiceConfiguration appConfig = new ServiceConfiguration(readEnv);

            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile<InfrastructureAutoMapProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var service = new MongoDeviceStateService(appConfig, mapper);

            var res = await service.SaveDeviceState(new DeviceStateEntity
            {
                DeviceGroupId = DeviceGroupId,
                DeviceId = DeviceId
            });

            Assert.NotNull(res);
        }
    }
}
