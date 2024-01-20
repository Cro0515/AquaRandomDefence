using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;  //SimpleJSON을 사용하기 위해 네임스페이스를 추가

public class Edit_Upload : MonoBehaviour
{
    


    GameObject m_UploadPanel;
    InputField Title_Input;
    Button Upload_Btn;

    bool m_Btnflag = true;

    //타이틀
    string CustomGameTitle_str = "";        //타이틀
    string MapData_Json = "";               //맵데이터 //배열->string
    string SpawnData_Json = "";             //스폰데이터 //배열->string
    string RoundSet_Json = "";              //라운드옵션 //JSON
    string TowerData_Json = "";             //타워 데이터 //JSON
    string MonsterData_Json = "";           //몬스터 셋팅 //
    string GameSet_Json = "";               //게임셋팅 //JSON
    string MakeDate_Str = "";               //생성일



    //-----------------
    GameObject m_CodePanel;
    Text m_CodeText;
    Button m_Copy_Btn;
    Button m_Lobby_Btn;

    





    string INSERT_CustomGame = "http://devwhale.dothome.co.kr/ARD_Test/INSERT_GameData.php";
    string SELECT_GameCode = "http://devwhale.dothome.co.kr/ARD_Test/SELECT_GameCode.php";


    string g_Message = "";


    void ObjecLoad(){

        m_UploadPanel = GameObject.Find("UploadPanel").gameObject;
        Title_Input = m_UploadPanel.transform.Find("Panel").Find("GameTitle_Input").GetComponent<InputField>();
        Upload_Btn = m_UploadPanel.transform.Find("Panel").Find("Finisht_Btn").GetComponent<Button>();
        
        m_CodePanel = GameObject.Find("GameCodePanel").gameObject;
        m_CodeText = m_CodePanel.transform.Find("GameCode_Text").GetComponent<Text>();
        m_Copy_Btn = m_CodePanel.transform.Find("Copy_Btn").GetComponent<Button>();
        m_Lobby_Btn = m_CodePanel.transform.Find("Lobby_Btn").GetComponent<Button>();


    }


    void Awake()
    {
            
        ObjecLoad();


        Upload_Btn.onClick.AddListener(() => {
           
            if (Title_Input.text == "" || Title_Input.text == null || m_Btnflag == false)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);


            m_Btnflag = false;

            //데이터 확인
            if (Tower_DataCheck() && Mon_DataCheck() &&
            Spawn_DataCheck() && Round_DataCheck() && Game_DataCheck()){



                //타이틀   stirng
                CustomGameTitle_str = Title_Input.text;

                //맵데이터(int 2차 배열) -> string = 직접 구현 메서드
                MapData_Json = LobbyMgr.Inst.ArrayToString(GlobalGameData.m_Map_Array);

                //스폰(int 2차 배열) -> string = 직접 구현 메서드
                SpawnData_Json = LobbyMgr.Inst.ArrayToString(GlobalGameData.m_MonSpawn_Array);

                //라운드 옵션(배열) -> JSON = JsonHelper
                RoundSet_Json = JsonHelper.ToJson(GlobalGameData.m_RoundSet_Array);

                //타워 데이터(class) -> JSON = JsonUtility
                TowerData_Json = JsonUtility.ToJson(GlobalGameData.m_TowersData_TL);

                //몬스터 데이터(배열) -> JSON = JsonHelper
                MonsterData_Json = JsonHelper.ToJson(GlobalGameData.m_MonSet_Array);

                //게임 셋팅(class) -> JSON = JsonUtility
                GameSet_Json = JsonUtility.ToJson(GlobalGameData.m_GameSetting);


                DateTime DT = DateTime.Now;
                MakeDate_Str = DT.ToString("yyyy'/'MM'/'dd");

                //게임ID
                //코드생성 및 데이터 삽입
                MakeGameCode();


            }
            else
            {

                //데이터 이상
                Debug.Log("잘못된 데이터");
                m_Btnflag = true;

            }
            
        });





        m_Copy_Btn.onClick.AddListener(() => {

            GameObject InfoWindow = Instantiate(EditorMgr.g_Inst.InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "복사 되었습니다.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button1_Sound, 1.0f);

                Destroy(InfoWindow);
            });



            UniClipboard.SetText(m_CodeText.text);

        });

        m_Lobby_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");

        });

    }

    // Start is called before the first frame update
    void Start()
    {
        m_Btnflag = true;
        m_UploadPanel.SetActive(true);
        m_CodePanel.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool Tower_DataCheck()
    {
        int cnt = 0;

        ClamTower tempClam = JsonUtility.FromJson<ClamTower>(GlobalGameData.m_TowersData_TL.ClamTower);
        cnt += (Array.Exists(tempClam.m_Damage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempClam.m_AttackSpeed, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempClam.m_Range, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempClam.m_Cost, x => x < 0) ? 1 : 0);

        CrabTower tempCrab = JsonUtility.FromJson<CrabTower>(GlobalGameData.m_TowersData_TL.CrabTower);
        cnt += (Array.Exists(tempCrab.m_Damage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempCrab.m_AttackSpeed, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempCrab.m_Cost, x => x < 0) ? 1 : 0);

        ElectricEelTower tempElec = JsonUtility.FromJson<ElectricEelTower>(GlobalGameData.m_TowersData_TL.ElectricEelTower);
        cnt += (Array.Exists(tempElec.m_Damage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempElec.m_AttackSpeed, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempElec.m_Range, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempElec.m_ChainCnt, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempElec.m_Cost, x => x < 0) ? 1 : 0);

        PufferTower tempPuffer = JsonUtility.FromJson<PufferTower>(GlobalGameData.m_TowersData_TL.PufferTower);
        cnt += (Array.Exists(tempPuffer.m_Damage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempPuffer.m_AttackSpeed, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempPuffer.m_Range, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempPuffer.m_FreezeTime, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempPuffer.m_Cost, x => x < 0) ? 1 : 0);

        PoisonPufferTower tempPoison = JsonUtility.FromJson<PoisonPufferTower>(GlobalGameData.m_TowersData_TL.PoisonPufferTower);
        cnt += (Array.Exists(tempPoison.m_Damage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempPoison.m_AttackSpeed, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempPoison.m_Range, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempPoison.m_PoisonTime, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempPoison.m_PoisonDamage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(tempPoison.m_Cost, x => x < 0) ? 1 : 0);

        return cnt <= 0;
    }

    public bool Mon_DataCheck()
    {

        int cnt = 0;

        for (int i = 0; i < System.Enum.GetValues(typeof(MonName)).Length; i++)
        {

            cnt += (GlobalGameData.m_MonSet_Array[i].Hp >= 0 ? 0 : 1);
            cnt += (GlobalGameData.m_MonSet_Array[i].Damage >= 0 ? 0 : 1);
            cnt += (GlobalGameData.m_MonSet_Array[i].Speed >= 0.0f ? 0 : 1);
        }

        return cnt <= 0;
    }

    public bool Round_DataCheck()
    {

        int cnt = 0;


        //라운드 옵션
        for (int i = 0; i < EditorMgr.g_Inst.temp_RoundSet_Data.Length; i++)
        {
            cnt += GlobalGameData.m_RoundSet_Array[i].Hp_Mult < 0.0f ? 1 : 0;
            cnt += GlobalGameData.m_RoundSet_Array[i].Speed_Mult < 0.0f ? 1 : 0;
            cnt += GlobalGameData.m_RoundSet_Array[i].Spawn_Delay <= 0.0f ? 1 : 0;
        }

        return cnt <= 0;

    }

    public bool Spawn_DataCheck()
    {

        int cnt = 0;

        //스폰 데이터
        for (int i = 0; i < EditorMgr.g_Inst.temp_RoundSet_Data.Length; i++)
        {
            for (int j = 0; j < System.Enum.GetValues(typeof(MonName)).Length; j++)
            {
                cnt += GlobalGameData.m_MonSpawn_Array[i, j] < 0 ? 1 : 0;
            }
        }


        return cnt <= 0;

    }

    public bool Game_DataCheck()
    {

        int cnt = 0;

        cnt += GlobalGameData.m_GameSetting.Gold < 0 ? 1 : 0;
        cnt += GlobalGameData.m_GameSetting.Heart < 0 ? 1 : 0;
        cnt += GlobalGameData.m_GameSetting.ReloadCost < 0 ? 1 : 0;

        return cnt <= 0;
    }


    void MakeGameCode()
    {

        
        string strRandomChar = "QWERTYUIOPASDFGHJKLZXCVBNM0123456789"; //랜덤으로 들어갈 문자 및 숫자 

        
        UnityEngine.Random.InitState((int)(DateTime.Now.Ticks));


        StringBuilder rs = new StringBuilder();

        for (int i = 0; i < 6; i++)
        {
            rs.Append(strRandomChar[(int)(UnityEngine.Random.Range(0.1f, 0.9f) * strRandomChar.Length)]);
        }
        GlobalGameData.m_GameCode_Str = rs.ToString();
        

        //뽑아낸 코드가 검색결과에 있는지? 체크
        StartCoroutine(GameCode_SELECT());


    }
    






    IEnumerator CustomGame_INSERT()
    {
        WWWForm form = new WWWForm();

        //PHP 필드값 매핑
        form.AddField("game_id", GlobalGameData.m_GameCode_Str, System.Text.Encoding.UTF8);
        form.AddField("title", CustomGameTitle_str, System.Text.Encoding.UTF8);
        form.AddField("creator_uid", GlobalUserData.m_Uid_str, System.Text.Encoding.UTF8);
        form.AddField("creator_nickname", GlobalUserData.m_NickName_str, System.Text.Encoding.UTF8);

        form.AddField("map_data", MapData_Json, System.Text.Encoding.UTF8);
        form.AddField("map_design", "");
        form.AddField("round_data", SpawnData_Json, System.Text.Encoding.UTF8);
        form.AddField("round_setting", RoundSet_Json, System.Text.Encoding.UTF8);
        form.AddField("tower_setting", TowerData_Json, System.Text.Encoding.UTF8);
        form.AddField("monster_setting", MonsterData_Json, System.Text.Encoding.UTF8);
        form.AddField("game_setting", GameSet_Json, System.Text.Encoding.UTF8);

        form.AddField("play_count", 0);
        form.AddField("like_count", 0);
        form.AddField("make_date", MakeDate_Str, System.Text.Encoding.UTF8);

        //PHP 필드값 매핑


        WWW webRequest = new WWW(INSERT_CustomGame, form);
        yield return webRequest;
        g_Message = webRequest.text;
    }





    //코드 존재여부 확인
    IEnumerator GameCode_SELECT()
    {

        WWWForm form = new WWWForm();
        form.AddField("game_id", GlobalGameData.m_GameCode_Str, System.Text.Encoding.UTF8);

        WWW webRequest = new WWW(SELECT_GameCode, form);
        yield return webRequest;


        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(webRequest.bytes);
        g_Message = sz;
        if (sz.Contains("Select-Success!!") == true)
        {
            //text로 이름과 점수 출력해주고
            //JSON 파싱
            if (sz.Contains("cnt") == true)
            {
                var N = JSON.Parse(sz);
                //g_Message = N;

                if (N != null && N["cnt"] != null)
                {
                    //g_Message = N["cnt"];

                    //값이 없다면
                    if (N["cnt"] == "0") 
                    {
                        //데이터 삽입 실행
                        StartCoroutine(CustomGame_INSERT());


                        
                        m_Btnflag = false;

                        //게임코드 복사 안내창
                        m_CodeText.text = GlobalGameData.m_GameCode_Str;

                        m_UploadPanel.SetActive(false);
                        m_CodePanel.SetActive(true);

                        //글로벌데이터 및 JSON 변수 값 초기화
                        GlobalGameData.m_GameMode = GameMode.Standard;

                        GlobalGameData.m_GameCode_Str = "";

                        GlobalGameData.m_Map_Array = default(int[,]);
                        GlobalGameData.m_MonSpawn_Array = default(int[,]);
                        GlobalGameData.m_RoundSet_Array = default(RoundSetting[]);
                        GlobalGameData.m_MonSet_Array = default(MonSet[]);
                        GlobalGameData.m_TowersData_TL = default(TowerList);
                        GlobalGameData.m_GameSetting = default(GameSetting);

                        GlobalGameData.m_undoHistory = default(Stack<History>);



                        CustomGameTitle_str = "";        //타이틀

                        MapData_Json = "";               //맵데이터 //배열->string
                        SpawnData_Json = "";             //스폰데이터 //배열->string
                        RoundSet_Json = "";              //라운드옵션 //JSON
                        TowerData_Json = "";             //타워 데이터 //JSON
                        MonsterData_Json = "";           //몬스터 셋팅 //
                        GameSet_Json = "";               //게임셋팅 //JSON

                        MakeDate_Str = "";               //생성일



                    }
                    //값이 있다면,
                    else
                    {
                        //다시 코드 생성
                        MakeGameCode();

                    }
                }
            }//if (sz.Contains("nick_name") == true)
        }//if (sz.Contains("Login-Success!!") == true)
        else
        {

            //데이터가 없다면,
            //g_Message = "등록된 정보가 없습니다.";
            Debug.Log("정보 없음");
            m_Btnflag = true;
        }




    }

    /*
    void OnGUI()
    {
        if (g_Message != "")
        {
            GUILayout.Label("<color=White><size=25>" + g_Message + "</size></color>");
        }
    }

    */
}
