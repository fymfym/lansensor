# LanSensor.CliNotifier

Monitors one sensor and its data in LanSensor repository

## Message integrations

### Slack integration 

    slackApiKey: Api key from Slack
    slackChannelName: Channel to post message in


## LanSensor Repository

    mySqlConnectionString

## LanSensor Monitoring 


### Monitor related data
    monitorValueIntervalMinutes
    monitorValueDataType
    monitorValueDataValue
    monitorValueOkDataValue
    monitorNotifyText: Message to post to message integration

### Device realted data

Monitor Device group id (configugred on the monitor device)

    deviceGroupId
    deviceId

General keepalie monitor, globally:

    keepaliveDataType
