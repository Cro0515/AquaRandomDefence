<?php
	$user_id = $_POST['user_id'];
	$game_id = $_POST['game_id'];
	
	$con = mysqli_connect("localhost","devwhale","qlalfqjsgh486^","devwhale");

	if(!$con)
		die('Could not Connect:' . mysqli_connect_error());  //������ �������� ��� �� ��ũ��Ʈ�� �ݾ��ְڴٴ� ��

	
	//userid�� gameid�� ������ֳ� Ȯ��
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
		
        //JSON ���� ����    
        $output = json_encode($RowDatas);

		// ��� echo = Log
        echo urldecode($output); //�ѱ� ���Ե� ���
        echo ("\n");
        die("Select-Success!!");
	}
?>