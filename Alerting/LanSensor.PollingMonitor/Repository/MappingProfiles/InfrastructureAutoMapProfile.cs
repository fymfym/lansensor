using AutoMapper;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.Repository.Models.MongoDb;

namespace LanSensor.Repository.MappingProfiles
{
    public class InfrastructureAutoMapProfile : Profile
    {
        public InfrastructureAutoMapProfile()
        {
            CreateMap<DeviceStateEntityMongoDb, DeviceStateEntity>();
            CreateMap<DeviceStateEntity, DeviceStateEntityMongoDb>();
        }
    }
}
