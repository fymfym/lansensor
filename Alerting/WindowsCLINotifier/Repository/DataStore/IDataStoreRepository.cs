using System;
using System.Collections.Generic;
using System.Text;

namespace LanSensor.Repository.DataStore
{
    public interface IDataStoreRepository
    {
        DateTime GetLatestPresence(string dataType);
        DateTime GetLatestPresence(string dataType, string dataValue);
    }
}
