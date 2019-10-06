using System;

namespace LanSensor.Repository.DataStore
{
    public interface IDataStoreRepository
    {
        DateTime GetLatestPresence(string dataType);
        DateTime GetLatestPresence(string dataType, string dataValue);
    }
}
