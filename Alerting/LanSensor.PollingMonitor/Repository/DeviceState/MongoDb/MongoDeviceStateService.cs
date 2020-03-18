using System.Threading.Tasks;
using AutoMapper;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.Repository.Repositories;

namespace LanSensor.Repository.DeviceState.MongoDb
{
    public class MongoDeviceStateService : IDeviceStateService
    {
        private readonly IMapper _mapper;

        private readonly IDeviceStateRepository _deviceStateRepository;

        public MongoDeviceStateService(
            IServiceConfiguration applicationConfiguration,
            IMapper mapper
            )
        {
            _mapper = mapper;
            _deviceStateRepository = new MongoDeviceStateRepository("lansesnor", "deviceState", applicationConfiguration, mapper);
        }

        public Task<DeviceStateEntity> GetDeviceState(string deviceGroupId, string deviceId)
        {
            var task = _deviceStateRepository.GetByIdAsync(MakeKey(deviceGroupId, deviceId));
            Task.WaitAll(task);
            var resValue = _mapper.Map<DeviceStateEntity>(task.Result);
            return Task.FromResult(resValue);
        }

        public Task<DeviceStateEntity> SaveDeviceState(DeviceStateEntity deviceStateEntity)
        {
            var task = _deviceStateRepository.GetByIdAsync(MakeKey(deviceStateEntity.DeviceGroupId, deviceStateEntity.DeviceId));
            Task.WaitAll(task);
            var res = _mapper.Map<DeviceStateEntity>(task.Result);
            deviceStateEntity.EntityId = MakeKey(deviceStateEntity.DeviceGroupId, deviceStateEntity.DeviceId);
            if (res == null)
            {
                var task2 = _deviceStateRepository.CreateAsync(deviceStateEntity);
                Task.WaitAll(task2);
            }
            else
            {
                var task2 = _deviceStateRepository.UpdateAsync(deviceStateEntity);
                Task.WaitAll(task2);
            }

            return Task.FromResult(deviceStateEntity);
        }

        private static string MakeKey(string deviceGroupId, string deviceId)
        {
            return $"<{deviceGroupId}>-<{deviceId}>";
        }
    }
}
