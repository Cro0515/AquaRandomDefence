<?php
	$game_id = $_POST['game_id'];

	$con = mysqli_connect("localhost","devwhale","qlalfqjsgh486^","devwhale");

	if(!$con)
		die('Could not Connet:' . mysql_error());

	$check = mysqli_query($con, "SELECT * FROM ARD_GameList WHERE `game_id`='".$game_id."'");

    $numrows = mysqli_num_rows($check);
	if ($numrows == 0)
    {
        die("[SELECT_GameData.php] No does not exist. \n");
        $isSuccess = false;
    }
	else {
		$row = mysqli_fetch_array($check);

        $RowDatas = array();
		$RowDatas['game_id'] = urlencode($row['game_id']);
		$RowDatas['creator_uid'] = urlencode($row['creator_uid']);
		$RowDatas['map_data'] = urlencode($row['map_data']);
		$RowDatas['map_design'] = urlencode($row['map_design']);
		$RowDatas['round_data'] = urlencode($row['round_data']);
		$RowDatas['round_setting'] = urlencode($row['round_setting']);
		$RowDatas['tower_setting'] = urlencode($row['tower_setting']);
		$RowDatas['monster_setting'] = urlencode($row['monster_setting']);
		$RowDatas['game_setting'] = urlencode($row['game_setting']);
        header("Content-type:application/json");
		
        //JSON 파일 생성    
        $output = json_encode($RowDatas);

		// 출력 echo = Log
        echo urldecode($output); //한글 포함된 경우
        echo ("\n");
        die("Select-Success!!");
	}
?>