#############################################################
## IMPORTS

import urllib.request
import urllib.parse
import configparser 
import io
import logging
import logging.handlers

import time
import socket, struct # , fcntl
import os
import datetime
import socket    


#############################################################
def getMac(interface='eth0'):
    # Return the MAC address of the specified interface
    printLog("Fetching MAC Address")
    try:
        str = open('/sys/class/net/%s/address' %interface).read()
    except:
        str = "00:00:00:00:00:00"
    return str[0:17].replace(':','_')


#############################################################
def printLog(message=''):
    # print ("{}".format(message))
    logging.info ("{}".format(message))

#############################################################
## load script config

config = configparser.ConfigParser()
config.read('/home/pi/lansensor.config')
configServerUrl = config["configurationserver"]["serverurl"]
logfile = config["log"]["logfile"]
deviceId = config["device"]["deviceidentification"].strip()
deviceGroupId = config["device"]["devicegroupidentification"].strip()

if (deviceId==""):
    deviceId=getMac('eth0')

if (deviceGroupId==""):
    printLog("DeviceGroupId not present")
    exit()

printLog ("Config server url: {}".format(configServerUrl))
printLog ("Log file: {}".format(logfile))
printLog ("Device id: {}".format(deviceId))
printLog ("Device group id: {}".format(deviceGroupId))
  
#############################################################
## Logger
logger = logging.getLogger()
logger.setLevel(logging.DEBUG)

handler = logging.handlers.RotatingFileHandler(logfile, maxBytes=100000, backupCount=5)
logger.addHandler(handler)

f = logging.Formatter(fmt='%(asctime)s %(levelname)s: %(message)s '
    '',
    datefmt="%Y-%m-%d %H:%M:%S")
handler.setFormatter(f)
printLog("Fetching config")


printLog("Waiting for linx to finish boot")
time.sleep(1)

#############################################################
## load config from server

logging.info("Loading network config")

url = configServerUrl + deviceId
printLog("URL config {}".format(url))

try:
    req = urllib.request.urlopen(url)
    content=io.TextIOWrapper(req)
except Exception as err:
    printLog ("Error fetching config:{}".format(err))
    exit()
  
printLog("Configfile {}".format(url))
printLog("Config content: {}".format(content))

#############################################################
## Parse config

config = configparser.ConfigParser()
config.readfp(content)
postUrl = config["server"]["posturl"]
devicename = config["input"]["devicename"]
inputcount = int(config["input"]["count"])


printLog ("Devicename: {}".format(devicename))
printLog ("Input count: {}".format(inputcount))
printLog ("Post Url: {}".format(postUrl))


#############################################################
## GPIO Setup

printLog ("Importing RPi.GPIO")

if (os.name == "nt"):
    logging.info("Fake GPIOZERO")
    printLog("FAKE - install the right thing via: 'sudo apt install python3-gpiozero'")
    os.environ['GPIOZERO_PIN_FACTORY'] = os.environ.get('GPIOZERO_PIN_FACTORY', 'mock')
    #from gpiozero.pins.mock import MockFactory
    #from gpiozero import Button
    #Device.pin_factory = MockFactory()
else:
    logging.info("GPIOZERO instatiate")
    printLog("gpiozero")
    try:
        from gpiozero import Button
        printLog("Waiting 30 secs for network to come up")
        #time.sleep(30)
    except RuntimeError:
        logging.info("GPIOZERO instatiate- FAILED")
        printLog("Error importing RPi.GPIO!  This is probably because you need superuser privileges.  You can achieve this by using 'sudo' to run your script")

#############################################################
## Reading RPI Data
logging.info("Starting: socket open")
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sockfd = sock.fileno()
SIOCGIFADDR = 0x8915

#############################################################
## Defines

logging.info("Starting sub defines...")
def getCPUtemperature():
    if (os.name == "nt"):
        return "FakedCpuTmp"
    res = os.popen('vcgencmd measure_temp').readline()
    return(res.replace("temp=","").replace("'C\n",""))

def get_ip2(iface = 'eth0'):
    if (os.name == "nt"):
        return "FakedIP"

    ifreq = struct.pack('16sH14s', iface, socket.AF_INET, '\x00'*14)
    try:
        res = fcntl.ioctl(sockfd, SIOCGIFADDR, ifreq)
    except:
        return None
    ip = struct.unpack(b'16sH2x4s8x', res)[2]
    return socket.inet_ntoa(ip)
    
def get_ip(iface = 'eth0'):
    return ((([ip for ip in socket.gethostbyname_ex(socket.gethostname())[2] if not ip.startswith("127.")] or [[(s.connect(("8.8.8.8", 53)), s.getsockname()[0], s.close()) for s in [socket.socket(socket.AF_INET, socket.SOCK_DGRAM)]][0][1]]) + ["no IP found"])[0])

def setstate(datatype, datavalue):
    completeUrl = postUrl + '?devicegroup={}&deviceid={}&type={}&data={}'.format(deviceGroupId, devicename, datatype, datavalue)
    printLog("CompleteURL: {}".format(completeUrl))
    response = urllib.request.urlopen(completeUrl)
    logging.info("State sent type=%s / value=%s",datatype,datavalue)

    
#############################################################
## Spinning up program

logging.info("Sending start state")
setstate('start','')

logging.info("Sending IP")
setstate('ip', get_ip('wlan0'))

logging.info("Sending CPUTMP")
setstate('cputemp',  getCPUtemperature())

logging.info("Setting start time")
starttime = time.time()

inputs = []

#############################################################
## Define classes

class input:
    state = 0
    truestatetosend="true"
    falsestatetosend="false"
    address1button="1"
    button=""
    count = 0

#############################################################
## Read inputs from web fetched config
logging.info("Parsing configuration")

x = 1
while (x <= inputcount):
    inp = input()
    keyname = "input_" + str(x)
    inp.state = "unset"
    inp.truestatetosend = str(config[keyname]["truestatetosend"])
    inp.falsestatetosend = str(config[keyname]["falsestatetosend"])
    inp.addressbutton = str(config[keyname]["addressbutton"])
    inp.button=Button(int(inp.addressbutton))
    inp.inputname = str(config[keyname]["inputname"])
    inp.count = 0
    inp.delay = 5

    if config.has_option(keyname,"delay"):
        keyvalue = config[keyname]["delay"]
        if keyvalue.isnumeric():
            inp.delay = int(keyvalue)
            print("delay changed from <5> to <" + str(inp.delay) + ">")

    inputs.append(inp)
    x = x + 1

#############################################################
## IMPORTS

maxRange = int(config["input"]["count"])

while True:
    logging.info("Starting main loop")
    try:
        while True:
            inpCount = 0
            while (inpCount < len(inputs)):
                activeinput = inputs[inpCount]
                inpCount += 1
                if ((activeinput.button.is_pressed != activeinput.state)):
                    activeinput.count = activeinput.count + 1
                else:
                    activeinput.count = 0
                if (activeinput.count > activeinput.delay):
                    logging.info("Start changed, sending data")
                    activeinput.state = activeinput.button.is_pressed
                    activeinput.count = 0
                    if (activeinput.state):
                        setstate(activeinput.inputname,activeinput.truestatetosend)
                    else:
                        setstate(activeinput.inputname,activeinput.falsestatetosend)
                    logging.info("Changed state send")

                if ((time.time() - starttime) > 3600):
                    logging.info("Sending keepalive")
                    starttime = time.time()
                    setstate('keepalive','')
                    setstate('cputemp',  getCPUtemperature())
                    logging.info("Keeep alive done")

            time.sleep(1)
            
    except Exception as e:
        printLog(e)
        logging.error("Main loop Exception")
        logging.error(e)


