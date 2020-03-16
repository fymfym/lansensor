﻿using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.Repository.Models.MongoDb;

namespace LanSensor.Repository
{
    public interface IDeviceStateRepository : IRepository<DeviceStateEntity, DeviceStateEntityMongoDb>
    {
    }
}