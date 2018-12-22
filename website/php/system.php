<?

function system_GetPostVar($Name){
	if (substr(phpversion(),0,5) < "4.0.0"){
		print "Version not compatible";
		exit;
	}
	elseif (substr(phpversion(),0,5) <= "4.0.4"){
		global $HTTP_POST_VARS;
		if (@isset($HTTP_POST_VARS[$Name]) > 0){
			return $HTTP_POST_VARS[$Name];
		} 
	}
	else {
		if (@isset($_POST[$Name])){
			return $_POST[$Name];
		}
		else {
			return "";
		}
	}
}

function system_GetVar($Name){
	$st = system_GetPostVar($Name);
	if (strlen($st) == 0){
		$st = system_GetUrlVar($Name);
	}
	return $st;
}

function system_GetMySQLToday(){
	return date("Y-m-d H:i:s",mktime());
}

function system_GetUrlVar($Name){
	if (substr(phpversion(),0,5) < "4.0.0"){
		print "Version not compatible";
		exit;
	}
	elseif (substr(phpversion(),0,5) <= "4.0.4"){
		global $HTTP_GET_VARS;
		if (isset($HTTP_GET_VARS[$Name])){
			return $HTTP_GET_VARS[$Name];
		} 
	}
	else {
		if (isset($_GET[$Name])){
			return $_GET[$Name];
		}
		else{
			return "";
		}
	}
	$ret = "";
}


?>