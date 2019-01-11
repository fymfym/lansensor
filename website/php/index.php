<html>
	
  <head>
    <title>Sensors....</title>
  </head>
<body>
<?

include('system.php');
include('config.php');

$deviceGroupid = system_getvar("devicegroup");
$deviceId = system_getvar("device");


?>

<table>
    <tr>
        <td><a href="/sensors">Device groups</a></td>
        <td>&nbsp;&nbsp;-&nbsp;&nbsp;</td>
		<?
		if (strlen($deviceGroupid) > 0)
		{
			print "<td><a href=\"/sensors/?devicegroup=" .  $deviceGroupid . "\">Devices</a></td>";
		}
		?>
    </tr>
</table>
<?


$db_conn = 0;
$db_conn = mysqli_connect($host,$user,$password);
			
mysqli_select_db($db_conn, $database);
  		
// Get the first and last hour
$today = getdate();
  		
//print "Year:" . $today['year'] . "<br>";
//print "mon:" . $today['mon'] . "<br>";
//print "mday:" . $today['mday'] . "<br>";
//print "hours:" . $today['hours'] . "<br>";
  		
$mktfirst = mktime($today['hours']-1,0,0,$today['mon'],$today['mday'],$today['year']);

if ($deviceGroupid == "")
{
    $sql = "select distinct devicegroupid from devicelog";
    $result = mysqli_query ($db_conn,$sql);
    print mysqli_error($db_conn);
    $numrows = mysqli_num_rows ($result);
    print "<table border=\"1\"><tr><th>Device group</th></tr>";
    for ($tel = 0; $tel < mysqli_num_rows ($result) ; $tel++)
    {
        $row = mysqli_fetch_row($result);
        print "<tr><td><a href=\"?devicegroup=".$row[0]."\">" .$row[0] . "</a></td></tr>";
    	}
    print "</table>";
    exit();
}

if ($deviceId == "")
{
    $sql = "select distinct deviceid from devicelog";
    $result = mysqli_query ($db_conn,$sql);
    print mysqli_error($db_conn);
    $numrows = mysqli_num_rows ($result);
    print "<table border=\"1\"><tr><th>Device</th></tr>";
    for ($tel = 0; $tel < mysqli_num_rows ($result) ; $tel++)
    {
        $row = mysqli_fetch_row($result);
        print "<tr><td><a href=\"?devicegroup=".$deviceGroupid."&device=".$row[0]."\">" .$row[0] . "</a></td></tr>";
    	}
    print "</table>";
    exit();
}


print "<table border=\"1\"><th>Device id</th><th>Reading date</th><th>Reading type</th><th>Value</th></tr>";
$sql = "select deviceid, DateTime, DataType, DataValue  from devicelog where devicegroupid='". $deviceGroupid."' and deviceid='" .$deviceId. "' order by DateTime desc limit 50";
$result = mysqli_query ($db_conn,$sql);
print mysqli_error($db_conn);
$numrows = mysqli_num_rows ($result);
    	
	for ($tel = 0; $tel < mysqli_num_rows ($result) ; $tel++)
	{
		$row = mysqli_fetch_row($result);
	  	print "<tr><td>" .$row[0] . "</td><td>" . $row[1] . "</td><td>" . $row[2] . "</td><td>" .$row[3] . "</td></tr>";
	}
print "</table>";


?>	
  </body>
</html>