<?php

	$ORDERBY_Option = $_POST['ORDERBY_Option'];
	$WHERE_Option = $_POST['WHERE_Option'];
	$WHERE_Keyword = $_POST['WHERE_Keyword'];

	$con = mysqli_connect("localhost","devwhale","qlalfqjsgh486^","devwhale");

	if(!$con)
		die('Could not Connet:' . mysql_error());
		
	if($WHERE_Keyword == "" || $WHERE_Keyword == null)
		$sqlList = mysqli_query($con, "SELECT * FROM `ARD_GameList` ORDER BY `".$ORDERBY_Option."` desc Limit 0, 50");
	else
		$sqlList = mysqli_query($con, "SELECT * FROM `ARD_GameList` WHERE `".$WHERE_Option."` LIKE '%".$WHERE_Keyword."%' ORDER BY `".$ORDERBY_Option."` desc Limit 0, 50");
		
	//$sqlList = mysqli_query($con, "SELECT * FROM `ARD_GameList` ORDER BY '' desc Limit 0, 50");
	
	
    $rowsCount = mysqli_num_rows($sqlList);
	if ($rowsCount == 0)
    {
        die("[SearchUser.php] User does not exist. \n");
    }
	else {
        $RowDatas = array();
        $Return   = array();

		for($aa = 0; $aa < $rowsCount; $aa++)
        {
            $a_row = mysqli_fetch_array($sqlList);       //�� ���� ��������

            if($a_row != false)
            {
                //JSON ������ ���� ����        
                $RowDatas['game_id'] = urlencode($a_row['game_id']);
				$RowDatas['title'] = urlencode($a_row['title']);
				$RowDatas['creator_uid'] = urlencode($a_row['creator_uid']);
				$RowDatas['creator_nickname'] = urlencode($a_row['creator_nickname']);
				$RowDatas['play_count'] = $a_row['play_count'];
				$RowDatas['like_count'] = $a_row['like_count'];
				$RowDatas['make_date'] = $a_row['make_date'];
				$RowDatas['round_data'] = urlencode($a_row['round_data']);
				
                array_push($Return, $RowDatas); //JSON ������ ������ ���� �迭�� ���ڵ� �� �߰�
            }
        }//for
 
		header("Content-type:application/json");
		echo urldecode(json_encode($Return)); //�ѱ� ���Ե� ���
		echo ("\n");
        die("Game Search-Success!!");
	}
?>