ping -c4 192.168.1.1 > /home/pi/wificheck.log

 
if [ $? != 0 ] 
then
  sudo /sbin/shutdown -r now
fi
