<?php
	$game_id = $_POST['game_id'];
	$user_id = $_POST['user_id'];

	$con = mysqli_connect("localhost","devwhale","qlalfqjsgh486^","devwhale");

	if(!$con)
		die('Could not Connet:' . mysql_error());

    //정보를 삽입해주는 쿼리문.
    $Result = mysqli_query($con, 
	"UPDATE `ARD_GameList` SET `like_count` = `like_count` + 1 WHERE `game_id` = '".$game_id."' AND false = (SELECT `like` FROM `ARD_UserInfo` WHERE `user_id` = '".$user_id."' AND `game_id` = '".$game_id."');");
      
    if($Result){
        
        $Result = mysqli_query($con, 
	    "UPDATE `ARD_UserInfo` SET `like` = true WHERE `user_id` = '".$user_id."' AND `game_id` = '".$game_id."';");

        if($Result)
            die("UPDATE-Success!!\n");
        else
            die("UPDATE error_2. \n");
            
    }
    else{
        die("UPDATE error_1. \n");


    }
    
?>