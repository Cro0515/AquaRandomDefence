using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUserData : MonoBehaviour
{

    public static string m_Uid_str = "";            //유저 아이디
    public static string m_NickName_str = "";      //유저 닉네임

    public static int m_TopRound_int = 0;          //최대 라운드
    public static int m_ClearHeart_int = 0;        //클리어 했을경우, 남은 목숨
    public static string m_ClearTime_str = "";     //클리어하는데 걸린 시간
    public static string m_ClearDate_str = "";     //클리어 기록 날짜
    public static bool m_Like;





    //인스펙터용 변수
    public string   i_Uid_str = "";               //유저 아이디
    public string   i_NickName_str = "";      //유저 닉네임

    public int      i_TopRound_int = 0;          //최대 라운드
    public int      i_ClearHeart_int = 0;        //클리어 했을경우, 남은 목숨
    public string   i_ClearTime_str = "";     //클리어하는데 걸린 시간
    public string   i_ClearDate_str = "";     //클리어 기록 날짜



    private void Update()
    {
        i_Uid_str = m_Uid_str;
        i_NickName_str = m_NickName_str;

        i_TopRound_int = m_TopRound_int;
        i_ClearHeart_int = m_ClearHeart_int;
        i_ClearTime_str = m_ClearTime_str;
        i_ClearDate_str = m_ClearDate_str;
    }

}
