<?php
	$game_id = $_POST['game_id'];
	$title = $_POST['title'];
	$creator_uid = $_POST['creator_uid'];
	$creator_nickname = $_POST['creator_nickname'];
	$map_data = $_POST['map_data'];
	$map_design = $_POST['map_design'];
	$round_data = $_POST['round_data'];
	$round_setting = $_POST['round_setting'];
	$tower_setting = $_POST['tower_setting'];
	$monster_setting = $_POST['monster_setting'];
	$game_setting = $_POST['game_setting'];
	$play_count = $_POST['play_count'];
	$like_count = $_POST['like_count'];
	$make_date = $_POST['make_date'];


	$con = mysqli_connect("localhost","devwhale","qlalfqjsgh486^","devwhale");


	mysqli_query($con, "set session character_set_connection=utf8;");
	mysqli_query($con, "set session character_set_results=utf8;");
	mysqli_query($con, "set session character_set_client=utf8;");



	if(!$con)
		die('Could not Connet:' . mysql_error());


	$tower_setting = addslashes($tower_setting);

    //정보를 삽입해주는 쿼리문.
    $Result = mysqli_query($con, 
	"INSERT INTO `ARD_GameList` VALUES ('".$game_id."','".$title."','".$creator_uid."','".$creator_nickname."','".$map_data."','".$map_design."','".$round_data."','".$round_setting."','".$tower_setting."','".$monster_setting."','".$game_setting."','".$play_count."','".$like_count."','".$make_date."');" ); 
        
    if($Result)
        die("Insert-Success!!\n");
    else
        die("Insert error. \n");
    
?>