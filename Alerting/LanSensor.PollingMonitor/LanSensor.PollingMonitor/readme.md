LanSensor polling monitor
==

A monitoring CLI that monitors LanSensor repository for wanted values from defined deviceex (Device group/device) 


## Moniotors

A list of monitors

    deviceMonitors:[]

### Monitor

Device group Id to search (Configurared on the device)

    deviceMonitor:deviceGroupId
        
Device Id to search (Configurared on the device)

    deviceMonitor:deviceId
       
The data type, the device sends (SOme device can send more than one)

    deviceMonitor:dataType

#### Message medium

Only implemented medium is "slack"

```
            "messageMediums": [
                {
                    "mediumType": "slack",
                    "receiverId": "channel name",
                    "message": "message part to slack"
                }

```        
#### timeInterval

The interval a certian monitor must have send a "dataValue" if not "messageMediums" (Either loal or global) is ued to send a message.

    deviceMonitor:timeInterval

## Data repository

### mysql

MySQL Connections string

    mysql:connectionString

### LiteDb
   Not documented


## Message integration

The medium that polling monitor uses to send messages to recipiants

### Slack

    slack:apiKey

## Monitor configuration

Number os seconds between polling of repository

    monitor:pollingIntervalSeconds

## Sample config file

```
{
    "deviceMonitors": [
        {
            "deviceGroupId": "group",
            "deviceId": "device",
            "dataType": "type",
            "messageMediums": [
                {
                    "mediumType": "slack",
                    "receiverId": "channel name",
                    "message": "message part to slack"
                }
            ],
            "keepalive": {
                "maxMinutesSinceKeepalive": 60,
                "keepaliveDataType": "keepalive"
            },
            "stateChangeNotification": {
                "onDataValueChangeFrom": "",
                "onDataValueChangeTo": "",
                "onEveryChange": ""
            },
            "timeInterval": [
                {
                    "dataValue": "Closed",
                    "messageMediums": [],
                    "weekdays": [
                        "Monday",
                        "Tuesday",
                        "Wednesday",
                        "Thursday",
                        "Friday",
                        "Saturday",
                        "Sunday"
                    ],
                    "times": [
                        {
                            "from": {
                                "hour": "00",
                                "minute": "00"
                            },
                            "to": {
                                "hour": "23",
                                "minute": "59"
                            }

                        }
                    ]
                }
            ]
        }
    ],
    "mysql": {
        "connectionString": "Server=localhost;Database=default;User=root;Password=password"
    },
    "litedb": {
        "filename": ""
    },
    "slack": {
        "apiKey": ""
    },
    "monitor": {
        "pollingIntervalSeconds": "20"
    }

}
```
