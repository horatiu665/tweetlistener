<?php
########## MySql details #############
// local config
$username = "root"; //mysql username
$password = "1234"; //mysql password
$hostname = "localhost"; //hostname
$databasename = 'hivemindcloud'; //databasename

// online config hivemindcloud
//$hostname = "mysql10.000webhost.com";
//$databasename = "a3879893_tweet";
//$username = "a3879893_admin";
//$password = "dumnezeu55";

// online config exponential twitter killer
//$username = "a8997625_admin"; //mysql username
//$password = "dumnezeu55"; //mysql password
//$hostname = "mysql4.000webhost.com"; //hostname
//$databasename = 'a8997625_hhh'; //databasename

$mysqli = mysqli_connect($hostname, $username, $password, $databasename);
?>