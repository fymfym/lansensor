<?

include('config.php');


$db_conn = 0;
$db_conn = mysqli_connect($host,$user,$password);

mysqli_select_db($db_conn, $database);


$type = system_GetVar("type");
$data = system_GetVar("data");
$Date = system_GetMySQLToday();
print "data:$data<br>";
print "type:$type<br>";
print "Date:" . $Date . "<br>";

if ($type == "") exit;

$sql = "insert into lawnmowerpresence (DateTime,datatype,datavalue) values ('$Date','$type','$data')";
mysqli_query ($db_conn,$sql);
?>