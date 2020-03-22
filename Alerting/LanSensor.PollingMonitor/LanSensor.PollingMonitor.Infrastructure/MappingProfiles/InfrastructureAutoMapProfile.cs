using AutoMapper;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Infrastructure.Models.MongoDb;

namespace LanSensor.PollingMonitor.Infrastructure.MappingProfiles
{
    public class InfrastructureAutoMapProfile : Profile
    {
        public InfrastructureAutoMapProfile()
        {
            CreateMap<DeviceStateEntityMongoDb, DeviceStateEntity>();
            CreateMap<DeviceStateEntity, DeviceStateEntityMongoDb>();

            CreateMap<MonitorStateMongoDb, MonitorState>();
            CreateMap<MonitorState, MonitorStateMongoDb>();
        }
    }
}
