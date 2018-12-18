<?

// ****************************************************************************************************************
// Get one var (Post/Url) from forms
// Input :
//		Variable name
// Output :
//		The value of the variable, either from Post or URL
function system_GetVar($Name){
	$st = @system_GetPostVar($Name);
	if (@strlen($st) == 0){
		$st = @system_GetUrlVar($Name);
	}
	return $st;
}



?>