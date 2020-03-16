using System;
using System.Threading.Tasks;
using FakeItEasy;
using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.Repository.DeviceState.MySqlDeviceState;
using NLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Repository.Manual
{
    public class MySqlDeviceStateRepositoryTest
    {
        private readonly ILogger _loggerFaked;
        private readonly IServiceConfiguration _configurationFaked;

        public const string DeviceId = "deviceIdTest";
        public const string DeviceGroupId = "deviceGroupIdTest";

        public MySqlDeviceStateRepositoryTest()
        {
            _loggerFaked = A.Fake<ILogger>();
            _configurationFaked = A.Fake<IServiceConfiguration>();

            A.CallTo(() => _configurationFaked.ApplicationConfiguration)
                .Returns(
                    new ApplicationConfiguration
                    {
                        MySqlConfiguration = new MySqlConfiguration
                        {
                            ConnectionString = "Server=127.0.01;Database=lansensor;User=fym;Password=password"
                        }
                    });
        }

        [Fact(Skip = "Manually test")]
        public async Task GetLatestDeviceStateEntity_FullObject_ReturnsFoundData()
        {
            var repo = new MySqlDeviceStateRepository(_configurationFaked, _loggerFaked);

            var result = await repo.GetLatestDeviceStateEntity(DeviceGroupId, DeviceId);

            Assert.NotNull(result);
        }

        [Fact(Skip = "Manually test")]
        public async Task SetDeviceStateEntity_FullObject_WriteToSql()
        {
            var repo = new MySqlDeviceStateRepository(_configurationFaked, _loggerFaked);

            var deviceEntity = new DeviceStateEntity
            {
                DeviceGroupId = DeviceGroupId,
                DeviceId = DeviceId,
                LastExecutedKeepAliveCheckDate = DateTime.Now,
                LastKeepAliveAlert = DateTime.Now,
                LastKnownDataValue = "test value",
                LastKnownDataValueDate = DateTime.Now,
                LastKnownKeepAliveDate = DateTime.Now
            };

            var result = await repo.SetDeviceStateEntity(deviceEntity);

            Assert.NotNull(result);
        }
    }
}
