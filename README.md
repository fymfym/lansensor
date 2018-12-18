# lansensor

A collection of scripts and programs to turn any computer unit into a monitor..


Alerting
---

Code to send alerting when a sensor reads the wrong value at the right time.

Plans:

- c#


MonitorDevice
---

Code to turn a device into a monitor.

RaspberryPi/Python
--

Python script and configuration file to read the local configuration, read the 
configuration for the device on what to monitor and how to react to it


website
---

Website for monitor devices to get configuration, send sesor data to and let users monitor the sensors


php
--

A simole website using MySQL to store sensor data

* store.php
  * PHP page to receive the sesor data and store it in MySQL
* index.php
  * A page to show the last 50 entries 
* config.php
  * MySQL access configuration
* system.php
  * Collection of methods