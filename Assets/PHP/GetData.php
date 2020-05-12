<?php

$servername = "localhost";
$username = "root";
$password = "";
$dbname = "momdb";

//Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

//Check connection
if ($conn->connect_error)
{
    die("Connection failed: " . $conn->connect_error);
}
//echo "Connected successfully. <br><br>";

$sql = "SELECT DIRID, QRID, NickName, CharValues, DateUpdate FROM appdata";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
    // output data of each row
    while($row = $result->fetch_assoc()) 
    {
    echo "DIRID: " . $row["DIRID"] . " - QRID: " . $row["QRID"] . " - NickName: " . $row["NickName"] . " - CharValues: " . $row["CharValues"] . " - DateUpdate: " . $row["DateUpdate"] .  "<br>";
    }
} else {
    echo "0 results";
}
$conn->close();
?>