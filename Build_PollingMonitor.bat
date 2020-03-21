dotnet test Alerting\LanSensor.PollingMonitor\LanSensor.PollingMonitor.sln 
dotnet publish Alerting\LanSensor.PollingMonitor\LanSensor.PollingMonitor.sln --output publish\pollingmonitor
"%programfiles%"\WinRAR\rar.exe a pollingmonitor.rar publish\pollingmonitor\*.* -r
pause