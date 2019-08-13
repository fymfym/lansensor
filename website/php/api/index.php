<?

include('../system.php');
include('../config.php');

$deviceGroupid = system_getvar("devicegroup");
$deviceId = system_getvar("device");

$db_conn = 0;
$db_conn = mysqli_connect($host,$user,$password);
			
mysqli_select_db($db_conn, $database);
  	
$deviceGroupid = system_getvar("devicegroupid");
$deviceId = system_getvar("deviceid");
	

$rows = array();

if ($deviceGroupid == "")
{
	$sql = "select distinct devicegroupid from devicelog";
	$result = mysqli_query ($db_conn,$sql);
	while($r = mysqli_fetch_assoc($result)) {
   		$actual_link = 'http://'.$_SERVER['HTTP_HOST'].$_SERVER['PHP_SELF'];
	    	$r["url"] = $actual_link . "?devicegroupid=" . $r["devicegroupid"];
    		$rows[] = $r;
	}
}


if (($deviceId == "") and ($deviceGroupid <> ""))
{
    $sql = "select distinct deviceid from devicelog";
    $result = mysqli_query ($db_conn,$sql);
	while($r = mysqli_fetch_assoc($result)) {
   		$actual_link = 'http://'.$_SERVER['HTTP_HOST'].$_SERVER['PHP_SELF'];
	    	$r["url"] = $actual_link . "?devicegroupid=" . $deviceGroupid . "&deviceid=" . $r["deviceid"];
    		$rows[] = $r;
	}
}

if (($deviceId <> "") and ($deviceGroupid <> ""))
{
    $sql = "select deviceid, DateTime, DataType, DataValue  from devicelog where devicegroupid='". $deviceGroupid."' and deviceid='" .$deviceId. "' order by DateTime desc limit 50";
    $result = mysqli_query ($db_conn,$sql);
    while($r = mysqli_fetch_assoc($result)) {
        $rows[] = $r;
    }
}


print json_encode($rows);
?>