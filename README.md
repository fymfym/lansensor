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

RaspberryPi/Python and shell
--

Python script and configuration file to read the local configuration, read the 
configuration for the device on what to monitor and how to react to it

Prerequisite
* install python3
  * $sudo apt-get update
  * $sudo apt-get install python3
* Install python module gpiozero 
  * $sudo apt install python3-gpiozero
* Scripts
  * Copy lansensor related scripts to /home/pi
    * launcher.sh
    * 777 ping.sh
    * checkwifi.sh
  * Change modes on the files
    * $chmod 777 launcher.sh
    * $chmod 777 ping.sh
    * $chmod 777 checkwifi.sh
* autorun check wifi scripts and ping (to try and stabilize network)
  * In Crontab ($sudo ctrontab -e (Use "Nano", if it asks)), add:
    * */2 * * * * /home/pi/checkwifi.sh 2>&1 >> /home/pi/resetwifi.log
    * */15 * * * * /home/pi/ping.sh > /home/pi/ping.log 2>&1
* Auto start python script on PI start
  * $sudo nano /etc/rc.local
  * Add below line, before "exit" and after"fi" 
    * sudo python3 /home/pi/lawnmower.py 2>&1 >> /home/pi/lawnmower.log &


website
---

Website for monitor devices to get configuration, send sesor data to and let users monitor the sensors

Angular
--

Do anyone have a plan to make a module for Angular?


php
--

A simple website using MySQL to store sensor data

* store.php
  * PHP page to receive the sesor data and store it in MySQL
* index.php
  * A page to show the last 50 entries 
* config.php
  * MySQL access configuration
* system.php
  * Collection of methods


Repository
---

MySQL
--

Use the script "repository\MySQL\CreateTable.sql" to create the table