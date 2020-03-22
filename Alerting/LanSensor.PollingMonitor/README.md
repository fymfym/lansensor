Lansesnor polling monitor
==

Run every [polling:[env].json:/monitor/pollingIntervalSeconds] seconds and runs through all defined "DeviceMonitors".

For each "DeviceMonitors" if finds 

Environment variables
==

mongoDbConnectionString

  * MongoDb to holds state of the monitors 

deviceRestApiBasePath

  * Base path to the Lansensor API

slackApiKey

  * The API key to slack for sending messages

