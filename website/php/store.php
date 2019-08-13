<?
include('config.php');

include('system.php');

$db_conn = 0;
$db_conn = mysqli_connect($host,$user,$password);

mysqli_select_db($db_conn, $database);

$devicegroup = system_GetVar("devicegroup");
$deviceid = system_GetVar("deviceid");
$type = system_GetVar("type");
$data = system_GetVar("data");

$date = system_GetMySQLToday();

if ($type == "") exit;
if ($devicegroup == "") exit;
if ($deviceid == "") exit;

$sql = "insert into DeviceLog (DateTime,devicegroupid, deviceid,datatype,datavalue) values ('$date','$devicegroup','$deviceid','$type','$data')";
mysqli_query ($db_conn,$sql);
//print $sql;
print "Done";
?>