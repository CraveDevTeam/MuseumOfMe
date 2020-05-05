<?php

//Variables for database
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "momdb";

//Variables submitted by user
$DirID = $_POST["DirID"];

//Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

//Check connection
if ($conn->connect_error)
{
    die("Connection failed: " . $conn->connect_error);
}
//echo "Connected successfully. <br><br>";

$sql = "SELECT DIRID FROM appdata WHERE DIRID = '" . $DirID . "'";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
    // output data of each row
    while($row = $result->fetch_assoc())
    {
        if ($row["DIRID"] == $DirID)
        {
            echo "DirID exists!";
        }
        else
        {
            echo "User DirID Is Wrong!";
        }
    }
} 
else 
{
    echo "DirID Does Not Exists!";
}
$conn->close();
?>