﻿using System;

namespace LanSensor.Models.DeviceState
{
    public class DeviceStateEntity
    {
        public string DeviceGroipId { get; set; }
        public string DeviceId { get; set; }
        public string LastKnownDataValue { get; set; }
        public DateTime LastKnownDataValueDate { get; set; }
        public string LastKnownKeepAlive { get; set; }
        public DateTime LastExecutedKeepaliveCheck { get; set; }
    }
}