<?php

//Variables for database
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "momdb";

//Variables submitted by user
$DirID = $_POST["DIRID"];
$QrID = $_POST["QRID"];
$NickName = $_POST["NickName"];
$CharValues = $_POST["CharValues"];
$DateUpdate = $_POST["DateUpdate"];

//Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

//Check connection
if ($conn->connect_error)
{
    die("Connection failed: " . $conn->connect_error);
}
//echo "Connected successfully. <br><br>";

$sql = "INSERT INTO appdata (DIRID, QRID, NickName, CharValues, DateUpdate)
VALUES ('" . $DirID . "','" . $QrID . "','" . $NickName . "', '" . $CharValues . "', '" . $DateUpdate . "')";

if ($conn->query($sql) === TRUE) 
{
  echo "New record created successfully";
} 
else 
{
  echo "Error: " . $sql . "<br>" . $conn->error;
  echo "DIRID already exist.";
}
$conn->close();
?>