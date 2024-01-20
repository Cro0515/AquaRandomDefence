<?php
	$user_id = $_POST['user_id'];
	$game_id = $_POST['game_id'];

	$con = mysqli_connect("localhost","devwhale","qlalfqjsgh486^","devwhale");

	if(!$con)
		die('Could not Connet:' . mysql_error());

	$sql = mysqli_query($con, "SELECT * FROM `ARD_UserInfo` WHERE `user_id` = '".$user_id."' AND `game_id` = '".$game_id."'");
    
	$numrows = mysqli_num_rows($sql);
	
	if ($numrows == 0)
    {
        die("[SELECT_MyRecord.php] data not exist. \n");
    }
	else {
       
		$row = mysqli_fetch_array($sql);
	    $RowDatas = array();

		$RowDatas['top_round'] = $row['top_round'];
		$RowDatas['heart'] = $row['heart'];
		$RowDatas['clear_time'] = urlencode($row['clear_time']);
		$RowDatas['clear_date'] = urlencode($row['clear_date']);
 
		header("Content-type:application/json");

		//JSON 파일 생성    
        $output = json_encode($RowDatas);

		echo urldecode($output); //한글 포함된 경우
		echo ("\n");
        die("RecordSearch-Success!!");
	}


?>