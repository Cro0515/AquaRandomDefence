<?php
	$game_id = $_POST['game_id'];

	$con = mysqli_connect("localhost","devwhale","qlalfqjsgh486^","devwhale");

	if(!$con)
		die('Could not Connet:' . mysql_error());

	$sql = mysqli_query($con, "SELECT EXISTS(SELECT * FROM ARD_GameList WHERE game_id ='".$game_id."') AS cnt");

    $numrows = mysqli_num_rows($sql);
	if ($numrows == 0)
    {
        die("[SELECT_GameCode.php] No does not exist. \n");
        $isSuccess = false;
    }
	else {
		$row = mysqli_fetch_array($sql);
        $RowDatas = array();

		$RowDatas['cnt'] = $row['cnt'];
        header("Content-type:application/json");
		
        //JSON ���� ����    
        $output = json_encode($RowDatas);

		// ��� echo = Log
		echo urldecode($output); //�ѱ� ���Ե� ���
        echo ("\n");
        die("Select-Success!!");
	}


?>