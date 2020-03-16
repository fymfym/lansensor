﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using MySql.Data.MySqlClient;
using NLog;

namespace LanSensor.Repository.DeviceState.MySqlDeviceState
{
    public class MySqlDeviceStateRepository : IDeviceState
    {
        private readonly IServiceConfiguration _configuration;
        private readonly ILogger _logger;

        public MySqlDeviceStateRepository
        (
            IServiceConfiguration configuration,
            ILogger logger
        )
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<DeviceStateEntity> GetLatestDeviceStateEntity(string deviceGroupId, string deviceId)
        {
            DeviceStateEntity result;
            var sql = "select DeviceGroupId, DeviceId, LastKnownDataValue, LastKnownDataValueDate, LastKnownKeepAliveDate, LastExecutedKeepAliveCheckDate, LastKeepAliveAlert ";
            sql += " from devicestate where ";

            sql += "DeviceGroupId = '" + deviceGroupId + "' " +
                   "and DeviceId='" + deviceId + "' ";

            try
            {
                using (var mysql =
                    new MySqlConnection(_configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString))
                {
                    result = mysql.Query<DeviceStateEntity>(sql).FirstOrDefault();
                }

                return Task.Run(() => result);
            }
            catch (Exception ex)
            {
                _logger.Fatal($"InsertDeviceStateEntity Execution failes: {ex.Message}");
            }

            return null;
        }

        public async Task<DeviceStateEntity> SetDeviceStateEntity(DeviceStateEntity deviceStateEntity)
        {
            var result = await GetLatestDeviceStateEntity(deviceStateEntity.DeviceGroupId, deviceStateEntity.DeviceId);
            if (result == null)
            {
                await InsertDeviceStateEntity(deviceStateEntity);
            }
            else
            {
                await UpdateDeviceStateEntity(deviceStateEntity);
            }

            return deviceStateEntity;
        }

        private async Task<DeviceStateEntity> UpdateDeviceStateEntity(DeviceStateEntity deviceStateEntity)
        {
            var sql = "update devicestate set LastKnownDataValue=@LastKnownDataValue, LastKnownDataValueDate=@LastKnownDataValueDate, LastKnownKeepAliveDate=@LastKnownKeepAlive, LastExecutedKeepAliveCheckDate=@LastExecutedKeepAliveCheckDate,LastKeepAliveAlert=@LastKeepAliveAlert ";
            sql += "where ";

            sql += "DeviceGroupId = '" + deviceStateEntity.DeviceGroupId + "' " +
                   "and DeviceId='" + deviceStateEntity.DeviceId + "' ";

            try
            {
                using (var mysql =
                    new MySqlConnection(_configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString))
                {
                    await mysql.ExecuteAsync(sql, deviceStateEntity);
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal($"UpdateDeviceStateEntity Execution failes: {ex.Message}");
            }

            return deviceStateEntity;
        }

        private Task<DeviceStateEntity> InsertDeviceStateEntity(DeviceStateEntity deviceStateEntity)
        {
            const string sql = "insert into devicestate (DeviceGroupId, DeviceId, LastKnownDataValue, LastKnownDataValueDate, LastKnownKeepAliveDate, LastExecutedKeepAliveCheckDate,LastKeepAliveAlert) value (@DeviceGroupId, @DeviceId, @LastKnownDataValue, @LastKnownDataValueDate, @LastKnownKeepAlive, @LastExecutedKeepAliveCheckDate, @LastKeepAliveAlert) ";
            try
            {
                using (var mysql =
                    new MySqlConnection(_configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString))
                {
                    mysql.Open();
                    var cmd = new MySqlCommand {CommandText = sql, Connection = mysql};
                    cmd.Parameters.AddWithValue("DeviceGroupId", deviceStateEntity.DeviceGroupId);
                    cmd.Parameters.AddWithValue("DeviceId", deviceStateEntity.DeviceId);
                    cmd.Parameters.AddWithValue("LastKnownDataValue", deviceStateEntity.LastKnownDataValue);
                    cmd.Parameters.AddWithValue("LastKnownDataValueDate", deviceStateEntity.LastKnownDataValueDate);
                    cmd.Parameters.AddWithValue("LastKnownKeepAlive", deviceStateEntity.LastKnownKeepAliveDate);
                    cmd.Parameters.AddWithValue("LastExecutedKeepAliveCheckDate", deviceStateEntity.LastExecutedKeepAliveCheckDate);
                    cmd.Parameters.AddWithValue("LastKeepAliveAlert", deviceStateEntity.LastKeepAliveAlert);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal($"InsertDeviceStateEntity Execution failes: {ex.Message}");
            }

            return Task.FromResult(deviceStateEntity);
        }
    }
}