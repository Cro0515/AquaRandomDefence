<?php
	$user_id = $_POST['user_id'];
	$game_id = $_POST['game_id'];
	$top_round = $_POST['top_round'];
	$heart = $_POST['heart'];
	$clear_time = $_POST['clear_time'];
	$clear_date = $_POST['clear_date'];
	$like_count = $_POST['like_count'];
	
	$con = mysqli_connect("localhost","devwhale","qlalfqjsgh486^","devwhale");

	if(!$con)
		die('Could not Connect:' . mysqli_connect_error());  //연결이 실패했을 경우 스크립트를 닫기

	mysqli_query($con, "set session character_set_connection=utf8;");
	mysqli_query($con, "set session character_set_results=utf8;");
	mysqli_query($con, "set session character_set_client=utf8;");

	//userid와 gameid로 기록이있나 확인
    $check = mysqli_query($con, "SELECT * FROM ARD_UserInfo WHERE user_id ='".$user_id."' AND game_id ='".$game_id."'");
    $numrows = mysqli_num_rows($check);
	$row = mysqli_fetch_array($check);

	//기록이 없으면 Insert
	if($numrows == 0){
		
		if($game_id == 0){
			$Result = mysqli_query($con, "INSERT INTO ARD_UserInfo VALUES ('".$user_id."', '".$game_id."' , '".$top_round."', '".$heart."' , '".$clear_time."' , '".$clear_date."', '".$like_count."');" ); 
		}
		//게임오버시 4개만
		else if($heart == 0 || $heart == null || $heart == "" || $heart == "0"){
			$Result = mysqli_query($con, "INSERT INTO ARD_UserInfo (`user_id`, `game_id`, `top_round`, `clear_date`) VALUES ('".$user_id."', '".$game_id."' , '".$top_round."', '".$clear_date."', '".$like_count."');" ); 
		}
		//클리어시
		else{
			$Result = mysqli_query($con, "INSERT INTO ARD_UserInfo VALUES ('".$user_id."', '".$game_id."' , '".$top_round."', '".$heart."' , '".$clear_time."' , '".$clear_date."', '".$like_count."');" ); 
		}

		if($Result)
            die("FirstGame - Score Record!!\n");
        else
            die("INSERT error. \n");
	}
	//기록이 있으면 Update
	else{
		//검색결과의 topround보다 입력받은 topround가 높을때
		if($row['top_round'] <= $top_round ){
		
			//게임오버시 3개만
			if($heart == 0 || $heart == null || $heart == "" || $heart == "0"){
				mysqli_query($con, "UPDATE ARD_UserInfo SET top_round = '".$top_round."', clear_date = '".$clear_date."' WHERE user_id = '".$user_id."' AND game_id = '" .$game_id."';");
			
				die('GameOver - Score Record!!\n');
			}
			//클리어시
			else{
				mysqli_query($con, "UPDATE ARD_UserInfo SET top_round = '".$top_round."' WHERE user_id = '".$user_id."' AND game_id = '" .$game_id."';");
				mysqli_query($con, "UPDATE ARD_UserInfo SET heart = '".$heart."' WHERE user_id = '".$user_id."' AND game_id = '" .$game_id."';");
				mysqli_query($con, "UPDATE ARD_UserInfo SET clear_time = '".$clear_time."' WHERE user_id = '".$user_id."' AND game_id = '" .$game_id."';");
				mysqli_query($con, "UPDATE ARD_UserInfo SET clear_date = '".$clear_date."' WHERE user_id = '".$user_id."' AND game_id = '" .$game_id."';");
			
				die('GameClear - Score Record!!\n');
			}
		}else{
			die('TopRound Low!!\n');
		}
	}
?>