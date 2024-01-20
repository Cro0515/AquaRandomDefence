<?php
	$user_id = $_POST['user_id'];
	$game_id = $_POST['game_id'];
	
	$con = mysqli_connect("localhost","devwhale","qlalfqjsgh486^","devwhale");

	if(!$con)
		die('Could not Connect:' . mysqli_connect_error());  //연결이 실패했을 경우 이 스크립트를 닫아주겠다는 뜻

	
	//userid와 gameid로 기록이있나 확인
    $check = mysqli_query($con, "SELECT * FROM ARD_UserInfo WHERE user_id ='".$user_id."' AND game_id ='".$game_id."'");
    $numrows = mysqli_num_rows($check);

	if ($numrows == 0)
    {
        die("[UserInfo_Select.php] User does not exist. \n");
        $isSuccess = false;
    }
	else {
		$row = mysqli_fetch_array($check);

        $RowDatas = array();
		$RowDatas['top_round'] = $row['top_round'];
		$RowDatas['heart'] = $row['heart'];
		$RowDatas['clear_time'] = urlencode($row['clear_time']);
		$RowDatas['clear_date'] = urlencode($row['clear_date']);
		$RowDatas['like'] = $row['like'];
        header("Content-type:application/json");
		
        //JSON 파일 생성    
        $output = json_encode($RowDatas);

		// 출력 echo = Log
        echo urldecode($output); //한글 포함된 경우
        echo ("\n");
        die("Select-Success!!");
	}
?>