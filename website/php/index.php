<html>
	
  <head>
    <title>Sensors....</title>
  </head>

<body>
<?

include('system.php');
include('config.php');


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


print "<table border=\"1\"><tr><th>Id</th><th>Date</th><th>Type</th><th>Value</th></tr>";
$sql = "select sensorid, DateTime, DataType, DataValue  from reading order by DateTime desc limit 50
";
$result = mysqli_query ($db_conn,$sql);
print mysqli_error($db_conn);
$numrows = mysqli_num_rows ($result);
    	
	for ($tel = 0; $tel < mysqli_num_rows ($result) ; $tel++)
	{
		$row = mysqli_fetch_row($result);
	  	print "<tr><td>" .$row[0] . "</td><td>" .$row[1] . "</td><td>" . $row[2] . "</td><td>" . $row[3] . "</td></tr>";
	}
print "</table>";


?>	
  </body>
</html>