using System;
using System.Threading.Tasks;
using AutoMapper;
using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;

namespace LanSensor.Repository.DeviceState.MongoDb
{
    public partial class MongoDeviceStateService : IDeviceState
    {
        private readonly IServiceConfiguration _applicationConfiguration;
        private readonly IMapper _mapper;

        private readonly IDeviceStateRepository _deviceStateRepository;

        public MongoDeviceStateService(
            IServiceConfiguration applicationConfiguration,
            IMapper mapper
            )
        {
            _applicationConfiguration = applicationConfiguration;
            _mapper = mapper;
            _deviceStateRepository = new MongoDeviceStateRepository("lansesnor", "deviceState", applicationConfiguration, mapper);
        }

        public Task<DeviceStateEntity> GetLatestDeviceStateEntity(string deviceGroupId, string deviceId)
        {
            var task = _deviceStateRepository.GetByIdAsync(MakeKey(deviceGroupId, deviceId));
            Task.WaitAll(task);
            return Task.FromResult(_mapper.Map<DeviceStateEntity>(task.Result));
        }

        public Task<DeviceStateEntity> SetDeviceStateEntity(DeviceStateEntity deviceStateEntity)
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
            return $"{deviceGroupId}-{deviceId}";
        }
    }
}
