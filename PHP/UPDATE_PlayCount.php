<?php
	$game_id = $_POST['game_id'];

	$con = mysqli_connect("localhost","devwhale","qlalfqjsgh486^","devwhale");

	if(!$con)
		die('Could not Connet:' . mysql_error());

    //정보를 삽입해주는 쿼리문.
    $Result = mysqli_query($con, 
	"UPDATE `ARD_GameList` SET `play_count`= `play_count` + 1 WHERE `game_id` = '".$game_id."';" ); 
        

    if($Result)
        die("UPDATE-Success!!\n");
    else
        die("UPDATE error. \n");
    
?>