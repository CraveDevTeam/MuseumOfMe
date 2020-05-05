<?php

if(ValidateFunction('SetAppData')){
	echo SetAppData()? "SUCCESS":"ERROR";
}else if(ValidateFunction('GetAppData')){
	echo GetAppData();
}else if(ValidateFunction('UploadImage')){
	echo UploadImage()? "SUCCESS":"ERROR";
}else{
	echo "Connected";
}

function ValidateFunction($FunctionName){
	if(isset($_POST['Function']) && $_POST['Function'] == $FunctionName) return true;
	else if(isset($_GET['Function']) && $_GET['Function'] == $FunctionName) return true;
	else return false;	
}

function GetValue($Value){
	if(isset($_POST[$Value])) return $_POST[$Value];
	else if(isset($_GET[$Value])) return $_GET[$Value];
	else return "";
}

function SetAppData(){
	/*$GUID = GetValue('GUID');
	$AppName = GetValue('AppName');
	$AppValues = addslashes(GetValue('AppValues'));*/
	$FileName = GetValue('FileName');
	
	//neo
	$DIRID = GetValue('DIRID');
	$QRID = GetValue('QRID');
	$NickName = GetValue('NickName');
	$CharValues = GetValue('CharValues');
	
	$Connection = Database();
	$IsDataExist = false;
	if($DIRID != ""){
		$SQL = "SELECT * FROM `appdata` WHERE GUID='$DIRID'";
		$Result = $Connection->query($SQL);
		$IsDataExist = $Result->num_rows  > 0? true:false;
	}else{
		$DIRD = DIRD();
	}
	
	if($FileName == ""){
		$_POST['FileName'] = $DIRID;
	}	
	
	if ($IsDataExist) {
		$SQL = "UPDATE `appdata` SET `QRID`='$QRID',`NickName`='$NickName', 'CharValues'='$CharValues' WHERE DIRID='$DIRID'";
	}else{
		$SQL = "INSERT INTO `appdata`(`QRID`,`NickName`,'CharValues',`DIRID`) VALUES ('$QRID','$NickName','$CharValues','$DIRID' )";
	}
	
	if ($Connection->query($SQL) === TRUE) {
	   UploadImage();
	   return $DIRID;
		
	} else {
	   return "Error";
	}
}

function GetAppData(){
	/*$GUID = GetValue('GUID');
	$AppName = GetValue('AppName');
	$DateCreated = GetValue('DateCreated');
	$Limit = GetValue('Limit');*/	
	
	$DIRID = GetValue('DIRID');
	$QRID = GetValue('QRID');
	$NickName = GetValue('NickName');
	$CharValues = GetValue('CharValues');
	$TimeUpdated = GetValue('TimeUpdated');
	$DateUpdated = GetValue('DateUpdated');

	$PLUS = "";
	$PLUS = $PLUS . ($DIRD != ""? "and `DIRID` = '$DIRID'":"");
	$PLUS = $PLUS . ($QRID != ""? "and `QRID` = '$QRID'":"");
	$PLUS = $PLUS . ($NickName != ""? "and `NickName` = '$NickName'":"");
	$PLUS = $PLUS . ($CharValues != ""? "and `CharValues` = '$CharValues'":"");
	$PLUS = $PLUS . ($TimeUpdated != ""? "and TimeUpdated = 'TimeUpdated'":"");
	$PLUS = $PLUS . ($DateUpdated != ""? "and DateUpdated = 'DateUpdated'":"");
	
	$Connection = Database();
	$SQL = "SELECT * FROM `appdata` WHERE 1 $PLUS order by DateUpdated desc";
	
	$Result = $Connection->query($SQL);
	if ($Result->num_rows > 0) {
		$Records['Items'] = array();
		while($Row = $Result->fetch_assoc()) {
			$Records['Items'][] = $Row;
		}
		
		return json_encode($Records);
	}
	
	$Connection->close();	
}

function UploadImage(){
	//$AppName = GetValue('AppName');
	$DIRID = GetValue('DIRID');
	$FileName = GetValue("FileName");
	
	if ($FileName != "" && isset($_FILES["FileBytes"]) && $_FILES["FileBytes"]["error"] <= 0){
		
		$Base = "Images";
		$Path = "$Base/MOM";
		if (!file_exists($Base)) {
			mkdir($Base, 0777);
		}
		
		if (!file_exists($Path)) {
			mkdir($Path, 0777);
		}
		$IsUploaded = move_uploaded_file($_FILES["FileBytes"]["tmp_name"], "$Path/$FileName.jpg");
		if($IsUploaded || true){
			return true;
		}else{
			return false;
		}
	}else{
		return false;
	}	
}
/*
function GUID()
{
    if (function_exists('com_create_guid') === true)
    {
        return trim(com_create_guid(), '{}');
    }
    return sprintf('%04X%04X%04X%04X%04X%04X%04X%04X', mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(16384, 20479), mt_rand(32768, 49151), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535));
}
*/

function Database(){
	
	$HostName = "localhost";
	$Username = "root";
	$Password = "";
	$DatabaseName = "momdb";

	$Connection = new mysqli($HostName, $Username, $Password, $DatabaseName);
	
	if ($Connection->connect_error) {
		die("ERROR");
	}
	return $Connection;
}


?>

