using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Edit_Test : MonoBehaviour
{

    Button TestBtn;
    Button FinishBtn;






    void ObjectLoad(){
        TestBtn = GameObject.Find("ButtonPanel").transform.Find("TestGameBtn").GetComponent<Button>();
        FinishBtn = GameObject.Find("ButtonPanel").transform.Find("FinishtBtn").GetComponent<Button>();
    }

    void BtnClickCollect()
    {
        TestBtn.onClick.AddListener(() => {

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            //글로벌 변수에 넣고 InGame씬으로 전환
            //InGameMgr에서 데이터 로드 부분 테스트모드 게임모드 구분해서 불러오게끔 하기
            GlobalDataSave();


            UnityEngine.SceneManagement.SceneManager.LoadScene("InGame");


        });

        FinishBtn.onClick.AddListener(() => {

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);


            GlobalDataSave();


            //다음 스탭으로 넘어가기
            EditorMgr.g_Inst.m_EditStep++;
            EditorMgr.g_Inst.m_Blink_Target = EditorMgr.g_Inst.StepBar_Upload_Btn.gameObject;

            if (EditorMgr.g_Inst.m_Blink_Target != null)
            {
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Alarm_Sound, 1.0f);


                EditorMgr.g_Inst.m_Blink_Target.GetComponent<Image>().color = EditorMgr.g_Inst.Color_StepBar_Gray;
                EditorMgr.g_Inst.m_Blink_Start_Color = EditorMgr.g_Inst.Color_StepBar_Gray;
                //끝색 설정
                EditorMgr.g_Inst.m_Blink_End_Color = EditorMgr.g_Inst.Color_StepBar_White;
                //블링크 스위치 온
                EditorMgr.g_Inst.m_BlinkSwitch_flag = true;
            }

        });

    }

    void Awake()
    {
        ObjectLoad();
        BtnClickCollect();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GlobalDataSave()
    {
        //글로벌 변수에 넣고 InGame씬으로 전환
        //InGameMgr에서 데이터 로드 부분 테스트모드 게임모드 구분해서 불러오게끔 하기
        GlobalGameData.m_Map_Array = EditorMgr.g_Inst.temp_Map_Data;
        GlobalGameData.m_TowersData_TL = EditorMgr.g_Inst.temp_Towers_Data;
        GlobalGameData.m_MonSet_Array = EditorMgr.g_Inst.temp_MonSet_Data;
        GlobalGameData.m_MonSpawn_Array = EditorMgr.g_Inst.temp_MonSpawn_Data;
        GlobalGameData.m_RoundSet_Array = EditorMgr.g_Inst.temp_RoundSet_Data;
        GlobalGameData.m_GameSetting = EditorMgr.g_Inst.temp_GameSet_Data;
        GlobalGameData.m_undoHistory = EditorMgr.g_Inst.m_Undo_Stack;

        GlobalGameData.m_GameMode = GameMode.Test;

    }

}
