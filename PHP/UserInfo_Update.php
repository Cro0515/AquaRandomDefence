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
		die('Could not Connect:' . mysqli_connect_error());  //������ �������� ��� ��ũ��Ʈ�� �ݱ�

	mysqli_query($con, "set session character_set_connection=utf8;");
	mysqli_query($con, "set session character_set_results=utf8;");
	mysqli_query($con, "set session character_set_client=utf8;");

	//userid�� gameid�� ������ֳ� Ȯ��
    $check = mysqli_query($con, "SELECT * FROM ARD_UserInfo WHERE user_id ='".$user_id."' AND game_id ='".$game_id."'");
    $numrows = mysqli_num_rows($check);
	$row = mysqli_fetch_array($check);

	//����� ������ Insert
	if($numrows == 0){
		
		if($game_id == 0){
			$Result = mysqli_query($con, "INSERT INTO ARD_UserInfo VALUES ('".$user_id."', '".$game_id."' , '".$top_round."', '".$heart."' , '".$clear_time."' , '".$clear_date."', '".$like_count."');" ); 
		}
		//���ӿ����� 4����
		else if($heart == 0 || $heart == null || $heart == "" || $heart == "0"){
			$Result = mysqli_query($con, "INSERT INTO ARD_UserInfo (`user_id`, `game_id`, `top_round`, `clear_date`) VALUES ('".$user_id."', '".$game_id."' , '".$top_round."', '".$clear_date."', '".$like_count."');" ); 
		}
		//Ŭ�����
		else{
			$Result = mysqli_query($con, "INSERT INTO ARD_UserInfo VALUES ('".$user_id."', '".$game_id."' , '".$top_round."', '".$heart."' , '".$clear_time."' , '".$clear_date."', '".$like_count."');" ); 
		}

		if($Result)
            die("FirstGame - Score Record!!\n");
        else
            die("INSERT error. \n");
	}
	//����� ������ Update
	else{
		//�˻������ topround���� �Է¹��� topround�� ������
		if($row['top_round'] <= $top_round ){
		
			//���ӿ����� 3����
			if($heart == 0 || $heart == null || $heart == "" || $heart == "0"){
				mysqli_query($con, "UPDATE ARD_UserInfo SET top_round = '".$top_round."', clear_date = '".$clear_date."' WHERE user_id = '".$user_id."' AND game_id = '" .$game_id."';");
			
				die('GameOver - Score Record!!\n');
			}
			//Ŭ�����
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