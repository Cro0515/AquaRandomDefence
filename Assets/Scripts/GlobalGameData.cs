using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    Standard,
    Custom,
    Test
}


public class GlobalGameData : MonoBehaviour
{
    //게임 정보
    public static GameMode m_GameMode = GameMode.Standard;  //false = 일반모드 / true = 테스트모드

    public static string m_GameCode_Str = "";




    public static int[,] m_Map_Array;
    public static int[,] m_MonSpawn_Array;
    public static RoundSetting[] m_RoundSet_Array;
    public static MonSet[] m_MonSet_Array;
    public static TowerList m_TowersData_TL;
    public static GameSetting m_GameSetting;

    public static Stack<History> m_undoHistory = new Stack<History>();





    //인스펙터용 변수
    public string i_GameNumber = "";
    public int[,] i_Map_Array;
    public int[,] i_Mon_Array;
    public RoundSetting[] i_RoundSet_Arry;
    public MonSet[] i_MonSet;
    public TowerList i_TowerList_TL;

    public GameSetting i_GameSetting;

    private void Update()
    {
        i_GameNumber = m_GameCode_Str;
        i_Map_Array = m_Map_Array;
        i_Mon_Array = m_MonSpawn_Array;
        i_RoundSet_Arry = m_RoundSet_Array;
        i_MonSet = m_MonSet_Array;
        i_TowerList_TL = m_TowersData_TL;
        i_GameSetting = m_GameSetting;
    }


    public static void Reset()
    {
        m_GameMode = GameMode.Standard;
        m_GameCode_Str = "";
        m_Map_Array = null;
        m_MonSpawn_Array = null;
        m_RoundSet_Array = null;
        m_MonSet_Array = null;
        m_TowersData_TL = null;
        m_GameSetting = null;
        m_undoHistory = new Stack<History>();
    }
}
