<?php

try {

	// config database
	include_once("config.php");
	
	//$decodedPost = rawurldecode($_POST['json']);
	
	$tableName = $_POST['table'];
	
	// ================================ Raw tweet-json saving ====================================
	
	$json = json_decode($_POST['json']);
	

	/////////////////// available fields and their saving into vars
	// represents the geographic location of this Tweet as reported by the user or client application. The inner coordinates array is formatted as geoJSON (longitude first, then latitude).
	//$json->coordinates
	// collection of floats (lat and long of tweet location)
	//	$json->coordinates->coordinates
	//$coordinates = $json->coordinates->coordinates;
	// type of data encoded in coordinates field
	//	$json->coordinates->type
	// UTC time when this Tweet was created.
	//$json->created_at
	$created_at = $json->created_at;
	
	$created_at_datetime = new DateTime($created_at);
	$created_at_datetime->setTimezone(new DateTimeZone('Europe/Copenhagen'));
	$mysqldate = $created_at_datetime->format('Y-m-d H:i:s');
	
	//$mysqldate = date('Y-m-d H:i:s', strtotime($created_at));
	//$mysqldate = date('Y-m-d H:i:s', time());
	//echo "Date:" . $mysqldate . ":";
	// id string (use instead of id)
	//$json->id_str
	$id_str = $json->id_str;
	// If the represented Tweet is a reply, this field will contain the integer representation of the original Tweet’s ID.
	//$json->in_reply_to_status_id_str
	$in_reply_to_status_id_str = $json->in_reply_to_status_id_str;
	//$json->in_reply_to_user_id_str
	$in_reply_to_user_id_str = $json->in_reply_to_user_id_str;
	//$json->lang
	$lang = $json->lang;
	//$json->retweet_count
	$retweet_count = $json->retweet_count;
	// URL to original tweet
	//$json->source
	// tweet text
	//$json->text
	
	$text = mysqli_real_escape_string($mysqli, $json->text);
	//$text = $json->text;
	
	// object containing info about user
	//$json->user
	// tweet count
	//	$json->user->statuses_count
	//	$json->user->favourites_count
	//	$json->user->profile_image_url
	//	$json->user->name
	//	$json->user->description
	//	$json->user->location
	//	$json->user->followers_count
	// The screen name, handle, or alias that this user identifies themselves with. screen_names are unique but subject to change. Use id_str as a user identifier whenever possible. Typically a maximum of 15 characters long, but some historical accounts may exist with longer names.
	//	$json->user->screen_name
	
	$user_name = mysqli_real_escape_string($mysqli, $json->user->screen_name);
	//$user_name = $json->user->screen_name;
		
	//	$json->user->friends_count
	//	$json->user->id_str
	$user_id_str = $json->user->id_str;
	// may appear multiple times
	//	$json->user->lang
	//	$json->user->created_at
	// The number of public lists that this user is a member of.
	//	$json->user->listed_count
	// Checks for entities / images:
	//  $json->entities->media
	// if (isset($json->entities->media)) {
		// foreach ($json->entities->media as $media) {
			// $media_url = $media->media_url; // Or $media->media_url_https for the SSL version.
		// }
	// }
	
	// create query
		$query 	=		"INSERT INTO";
		// table name
		$query	.= 		" `{$tableName}` ";
		// columns 
		$query	.=		" (`id`, `tweet_id_str`, `tweet`, `created_at`, `user_id_str`, `user_name`, `in_reply_to_status_id_str`, `in_reply_to_user_id_str`, `lang`, `retweet_count`) ";
		$query	.=		" VALUES ";
		// values of columns
		$query	.=		" (NULL, '{$id_str}', '{$text}', '{$mysqldate}', '{$user_id_str}', '{$user_name}', '{$in_reply_to_status_id_str}', '{$in_reply_to_user_id_str}', '{$lang}', '{$retweet_count}') ";
		
	// post json data to json table
	$result = mysqli_query($mysqli, $query);// or die(mysql_error());
	
	if ($result == FALSE) {
		echo 'RESULT WAS FALSE OH NOES';
		echo mysqli_error($mysqli);
	}
	
	// close connection. or not. test if it matters.
	mysqli_close($mysqli);
	
} 
catch (Exception $e) 
{
    echo 'Caught exception: ',  $e->getMessage(), "\n";
}

//*/
?>