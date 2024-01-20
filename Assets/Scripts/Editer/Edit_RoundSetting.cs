using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Edit_RoundSetting : MonoBehaviour
{
    GameObject RoundSettingPanel;

    InputField RoundCnt_Input;
    Button RoundApply_Button;
    Button Reset_Button;

    Text Round_Text;
    Button Prev_Button;
    Button Next_Button;
    Button RoundFinish_Button;

    InputField[] SpawnInput_Array;

    InputField SpeedMult_Input;
    InputField HpMult_Input;
    InputField SpawnDelay_Input;





    Color32 color_Disable = new Color32(90, 90, 90, 255);
    Color32 color_Active = new Color32(255, 255, 255, 255);





    int RoundCnt = 0;
    int Round = 0;


    RoundSetting[] local_RoundSet;

    int[,] local_SpawnData;


    //-----------------GameOptionSetting

    GameObject GameSettingPanel;

    InputField PlayerHeart_Input;
    InputField StartGold_Input;
    InputField ReloadCost_Input;

    Button Close_Button;
    Button GameOptionFinish_Button;



    GameSetting local_GameSet;

    //-----------------GameOptionSetting

    bool m_soundPlay = true;


    void ObjecLoad()
    {

        RoundSettingPanel = GameObject.Find("MonsterSettingPanel").gameObject;

        RoundCnt_Input = GameObject.Find("RoundCntSetBar").transform.Find("RoundCntInput").GetComponent<InputField>();
        RoundApply_Button = GameObject.Find("RoundCntSetBar").transform.Find("RoundApplyBtn").GetComponent<Button>();
        Reset_Button = GameObject.Find("RoundCntSetBar").transform.Find("ResetBtn").GetComponent<Button>();
        RoundFinish_Button = RoundSettingPanel.transform.Find("RoundFinishBtn").GetComponent<Button>();

        Round_Text = GameObject.Find("RoundOptionSetPanel").transform.Find("RoundText").GetComponent<Text>();
        Prev_Button = GameObject.Find("RoundOptionSetPanel").transform.Find("PrevButton").GetComponent<Button>();
        Next_Button = GameObject.Find("RoundOptionSetPanel").transform.Find("NextButton").GetComponent<Button>();
        SpeedMult_Input = GameObject.Find("RoundOptionSetPanel").transform.Find("SpeedMultInput").GetComponent<InputField>();
        HpMult_Input = GameObject.Find("RoundOptionSetPanel").transform.Find("HpMultInput").GetComponent<InputField>();
        SpawnDelay_Input = GameObject.Find("RoundOptionSetPanel").transform.Find("SpawnDelayInput").GetComponent<InputField>();


        SpawnInput_Array[0] = GameObject.Find("PinboomLine").transform.Find("Spawn_Input").GetComponent<InputField>();
        SpawnInput_Array[1] = GameObject.Find("DustLine").transform.Find("Spawn_Input").GetComponent<InputField>();
        SpawnInput_Array[2] = GameObject.Find("BottleLine").transform.Find("Spawn_Input").GetComponent<InputField>();
        SpawnInput_Array[3] = GameObject.Find("PetroleumLine").transform.Find("Spawn_Input").GetComponent<InputField>();
        SpawnInput_Array[4] = GameObject.Find("TrashLine").transform.Find("Spawn_Input").GetComponent<InputField>();




        GameSettingPanel = GameObject.Find("GameSettingPanel").gameObject;

        PlayerHeart_Input = GameObject.Find("PlayerHeartInput").GetComponent<InputField>();
        StartGold_Input = GameObject.Find("StartGoldInput").GetComponent<InputField>();
        ReloadCost_Input = GameObject.Find("ReloadInput").GetComponent<InputField>();

        Close_Button = GameObject.Find("CloseBtn").GetComponent<Button>();
        GameOptionFinish_Button = GameObject.Find("GameOptionFinishBtn").GetComponent<Button>();



        Debug.Log("오브젝트 로드 완료");

    }

    void ResourcesLoad()
    {



    }


    void InputOnEditEnd_Collection()
    {
        //비정상 값 막기
        //음수 0 100이상 수
        //스폰 인풋 = 1~99
        //라운드옵션 = 추후 테스트해가며 범위 정해야함

        RoundCnt_Input.onEndEdit.AddListener(delegate
        {
            ValueVerification(RoundCnt_Input, 1, 50);
        });






        SpawnInput_Array[0].onEndEdit.AddListener(delegate
        {
            ValueVerification(SpawnInput_Array[0], 0, 50);
        });

        SpawnInput_Array[1].onEndEdit.AddListener(delegate
        {
            ValueVerification(SpawnInput_Array[1], 0, 50);
        });

        SpawnInput_Array[2].onEndEdit.AddListener(delegate
        {
            ValueVerification(SpawnInput_Array[2], 0, 50);
        });

        SpawnInput_Array[3].onEndEdit.AddListener(delegate
        {
            ValueVerification(SpawnInput_Array[3], 0, 50);
        });

        SpawnInput_Array[4].onEndEdit.AddListener(delegate
        {
            ValueVerification(SpawnInput_Array[4], 0, 50);
        });








        SpeedMult_Input.onEndEdit.AddListener(delegate
        {
            ValueVerification(SpeedMult_Input, 1.0f, 2.0f);
        });

        HpMult_Input.onEndEdit.AddListener(delegate
        {
            ValueVerification(HpMult_Input, 1.0f, 10.0f);
        });

        SpawnDelay_Input.onEndEdit.AddListener(delegate
        {
            ValueVerification(SpawnDelay_Input, 0.1f, 5.0f);
        });







        PlayerHeart_Input.onEndEdit.AddListener(delegate
        {
            ValueVerification(PlayerHeart_Input, 1, 1000000);
        });

        StartGold_Input.onEndEdit.AddListener(delegate
        {
            ValueVerification(StartGold_Input, 1, 1000000);
        });

        ReloadCost_Input.onEndEdit.AddListener(delegate
        {
            ValueVerification(ReloadCost_Input, 1, 1000000);
        });


    }



    void BtnClick_Collection()
    {
        Prev_Button.onClick.AddListener(() =>
        {
            //비활성 상태시 리턴
            if (Prev_Button.image.color == color_Disable)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_1_Sound, 1.0f);


            //넥스트 버튼 비활성이면, 활성화
            if (Next_Button.image.color == color_Disable)
                Next_Button.image.color = color_Active;


            //데이터 저장
            RoundDataSave();


            //라운드 증가
            Round--;

            //끝인 경우 비활성화
            if (Round == 0)
                Prev_Button.image.color = color_Disable;

            //텍스트 및 인풋 업데이트
            RoundTextUpdate(Round, RoundCnt - 1);


        });

        Next_Button.onClick.AddListener(() =>
        {
            //비활성 상태시 리턴
            if (Next_Button.image.color == color_Disable)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_2_Sound, 1.0f);

            //이전 버튼 비활성이면, 활성화
            if (Prev_Button.image.color == color_Disable)
                Prev_Button.image.color = color_Active;


            //데이터 저장
            RoundDataSave();

            //라운드 증가
            Round++;

            //끝인 경우 비활성화
            if (Round == EditorMgr.g_Inst.temp_RoundSet_Data.Length - 1)
                Next_Button.image.color = color_Disable;

            //텍스트 및 인풋 업데이트
            RoundTextUpdate(Round, RoundCnt - 1);

        });

        RoundApply_Button.onClick.AddListener(() =>
        {
            if (m_soundPlay == true)
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button1_Sound, 1.0f);


            if (RoundCnt_Input.text == "" || RoundCnt_Input.text == "0" || RoundCnt_Input.text == RoundCnt.ToString())
                return;

            InitTempData();

            if (m_soundPlay == false)
                m_soundPlay = true;
        });

        Reset_Button.onClick.AddListener(() =>
        {

            if (RoundCnt_Input.text == "" || RoundCnt_Input.text == "0")
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Reload_Sound, 1.0f);


            InitTempData();
        });

        RoundFinish_Button.onClick.AddListener(() =>
        {

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);


            RoundDataSave();

            if (TempRoundDataCheck() == true)
            {
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.OpenWindow_Sound, 1.0f);

                //라운드 설정 창 끄기
                RoundSettingPanel.SetActive(false);
                //게임 옵션 설정 창 켜기
                GameSettingPanel.SetActive(true);

                //플레이스 홀더 표시
                PlayerHeart_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_GameSet.Heart.ToString();
                StartGold_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_GameSet.Gold.ToString();
                ReloadCost_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_GameSet.ReloadCost.ToString();
                //플레이스 홀더 표시

                //임시변수 데이터 불러오기
                GameOptionTextUpdate();
            }
            else
            {
                //안내창
                Debug.Log("빈칸을 채워주세요");

                GameObject InfoWindow = Instantiate(EditorMgr.g_Inst.InfoMessage, GameObject.Find("Canvas").transform);

                InfoWindow.GetComponent<InfoMessage>().info_Text.text = "빈칸을 채워주세요";
                InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                    Destroy(InfoWindow);

                });
            }

        });



        Close_Button.onClick.AddListener(() =>
        {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.CloseWindow_Sound, 1.0f);


            //게임 옵션 설정 창 끄기
            GameSettingPanel.SetActive(false);

            //라운드 설정 창 켜기
            RoundSettingPanel.SetActive(true);

            //게임 셋팅 저장
            GameOptionDataSave();
        });



        GameOptionFinish_Button.onClick.AddListener(() =>
        {


            //게임 셋팅 저장
            GameOptionDataSave();
            
            //게임셋팅 빈칸 확인
            if(TempGameOptionDataCheck() == true){
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);


                //다음 스탭으로 넘어가기
                EditorMgr.g_Inst.m_EditStep++;
                EditorMgr.g_Inst.m_Blink_Target = EditorMgr.g_Inst.StepBar_Test_Btn.gameObject;

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


            }
            else
            {
                GameObject InfoWindow = Instantiate(EditorMgr.g_Inst.InfoMessage, GameObject.Find("Canvas").transform);

                InfoWindow.GetComponent<InfoMessage>().info_Text.text = "빈칸을 채워주세요";
                InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                    Destroy(InfoWindow);

                });

            }
        });
    }







    void Awake()
    {
        SpawnInput_Array = new InputField[System.Enum.GetValues(typeof(MonName)).Length];


        ObjecLoad();
        ResourcesLoad();
        BtnClick_Collection();
        InputOnEditEnd_Collection();


    }



    // Start is called before the first frame update
    void Start()
    {
        GetLocalData();

        //게임옵션 변수 초기화
        
        if(GlobalGameData.m_GameMode == GameMode.Standard)
        {

            EditorMgr.g_Inst.temp_GameSet_Data = new GameSetting();
            EditorMgr.g_Inst.temp_GameSet_Data.Gold = -1;
            EditorMgr.g_Inst.temp_GameSet_Data.Heart = -1;
            EditorMgr.g_Inst.temp_GameSet_Data.ReloadCost = -1;
            Round = 0;
            RoundCnt_Input.text = "5";

            m_soundPlay = false;
            RoundApply_Button.onClick.Invoke();
        }else
        {
            Round = 0;
            RoundCnt_Input.text = GlobalGameData.m_RoundSet_Array.Length.ToString();
            RoundCnt = int.Parse(RoundCnt_Input.text);

            RoundTextUpdate(Round, RoundCnt);
        }

        //



        GameSettingPanel.SetActive(false);
        RoundSettingPanel.SetActive(true);


        

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {

            for (int i = 0; i < RoundCnt; i++)
            {
                EditorMgr.g_Inst.temp_RoundSet_Data[i].Hp_Mult = 1.5f;
                EditorMgr.g_Inst.temp_RoundSet_Data[i].Spawn_Delay = 0.5f;
                EditorMgr.g_Inst.temp_RoundSet_Data[i].Speed_Mult = 5.0f;

            }

            EditorMgr.g_Inst.temp_MonSpawn_Data = new int[RoundCnt, System.Enum.GetValues(typeof(MonName)).Length];
            for (int i = 0; i < RoundCnt; i++)
                for (int j = 0; j < System.Enum.GetValues(typeof(MonName)).Length; j++)
                {
                    EditorMgr.g_Inst.temp_MonSpawn_Data[i, j] = 1;
                }



            for (int j = 0; j < System.Enum.GetValues(typeof(MonName)).Length; j++)
            {
                SpawnInput_Array[j].text = "1";

            }

            HpMult_Input.text = "1.5";
            SpawnDelay_Input.text = "0.5";
            SpeedMult_Input.text = "5.0";


        }


    }




    void GetLocalData()
    {
        local_RoundSet = EditorMgr.g_Inst.local_Round_Data;
        local_SpawnData = EditorMgr.g_Inst.local_MonSpawn_Data;
        local_GameSet = EditorMgr.g_Inst.local_GameSetting_Data;

        //플레이스홀더 변경
        RoundCnt_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_RoundSet.Length.ToString();
        Round_Text.text = "1 라운드";
        Prev_Button.GetComponent<Image>().color = color_Disable;

        SpeedMult_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_RoundSet[0].Speed_Mult.ToString();
        HpMult_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_RoundSet[0].Hp_Mult.ToString();
        SpawnDelay_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_RoundSet[0].Spawn_Delay.ToString();


        for (int i = 0; i < SpawnInput_Array.Length; i++){
            SpawnInput_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_SpawnData[0, i].ToString();

        }
    }




   


    void ValueVerification(InputField _Input, int _min, int _max)
    {
        if (_Input.text == "")
            return;

        int tempValue = 0;

        //변환 시도
        //변환 성공 경우 tempValue에 저장
        if (int.TryParse(_Input.text, out tempValue) == false)
        {
            //실패 경우 최대 또는 최소치 대입 양수일때 최대값, 음수일때 최소값
            tempValue = _Input.text.IndexOf("-") == -1 ? _max : _min;

        }
        else
        {
            if (tempValue <= _min)
            {
                tempValue = _min;
            }
            //최대값
            else if (tempValue >= _max)
            {
                tempValue = _max;
            }
        }

        _Input.text = tempValue.ToString();

        if (tempValue >= 1000)
            _Input.text = (string.Format("{0:#,###}", tempValue)).ToString();

    }


    void ValueVerification(InputField _Input, float _min, float _max)
    {

        if (_Input.text == "")
            return;

        float tempValue = 0.0f;

        //변환 시도 (데이터 타입 범위 넘어간 경우)
        //변환 성공 경우 tempValue에 저장
        if (float.TryParse(_Input.text, out tempValue) == false)
        {
            //실패 경우 최대 또는 최소치 대입 양수일때 최대값, 음수일때 최소값
            tempValue = _Input.text.IndexOf("-") == -1 ? _max : _min;

        }
        //변환 성공 
        else
        {
            //최소값
            if (tempValue <= _min)
            {
                tempValue = _min;
            }
            //최대값
            else if (tempValue >= _max)
            {
                tempValue = _max;
            }

        }

        _Input.text = tempValue.ToString();

        if (tempValue >= 1000)
            _Input.text = (string.Format("{0:#,###}", tempValue)).ToString();

        if (_Input.text.IndexOf(".") == -1)
            _Input.text = _Input.text + ".0";
    }






    void InitTempData()
    {
        RoundCnt = int.Parse(RoundCnt_Input.text);

        //저장용 임시 변수들 생성 및 초기화
        EditorMgr.g_Inst.temp_RoundSet_Data = new RoundSetting[RoundCnt];

        for (int i = 0; i < RoundCnt; i++)
        {
            EditorMgr.g_Inst.temp_RoundSet_Data[i] = new RoundSetting(-1.0f, -1.0f, -1.0f);

        }

        EditorMgr.g_Inst.temp_MonSpawn_Data = new int[RoundCnt, System.Enum.GetValues(typeof(MonName)).Length];

        for (int i = 0; i < RoundCnt; i++)
            for (int j = 0; j < System.Enum.GetValues(typeof(MonName)).Length; j++)
            {
                EditorMgr.g_Inst.temp_MonSpawn_Data[i, j] = -1;
            }

       
        //적용시 항상 1라운드로 가지게
        Round = 0;
        Round_Text.text = "1";

        //1라운드 기준 플레이스홀더 가져오기
        RoundTextUpdate(0, RoundCnt - 1);

        //next버튼 비활성이라면 활성화
        if (Next_Button.image.color == color_Disable)
            Next_Button.image.color = color_Active;

        //prev버튼 비활성화
        Prev_Button.image.color = color_Disable;
    }

    void RoundDataSave()
    {

        //===============데이터 저장  (비정상적인 값 체크는 InputOnEditEnd에서 체크)
        //스폰 데이터
        for (int i = 0; i < SpawnInput_Array.Length; i++)
        {
            EditorMgr.g_Inst.temp_MonSpawn_Data[Round, i] = (SpawnInput_Array[i].text == "" ? -1 : int.Parse(SpawnInput_Array[i].text.Replace(",", "")));
        }


        //라운드 옵션
        EditorMgr.g_Inst.temp_RoundSet_Data[Round].Hp_Mult = (HpMult_Input.text == "" ? -1 : float.Parse(HpMult_Input.text.Replace(",", "")));
        EditorMgr.g_Inst.temp_RoundSet_Data[Round].Speed_Mult = (SpeedMult_Input.text == "" ? -1 : float.Parse(SpeedMult_Input.text.Replace(",", "")));
        EditorMgr.g_Inst.temp_RoundSet_Data[Round].Spawn_Delay = (SpawnDelay_Input.text == "" ? -1 : float.Parse(SpawnDelay_Input.text.Replace(",", "")));

    
    }

    void RoundTextUpdate(int _round, int _roundCnt){
        //텍스트 업데이트
        Round_Text.text = (_round + 1).ToString() + " 라운드";

        //로컬데이터 플레이스 홀더 업데이트
        //스폰

        for (int i = 0; i < SpawnInput_Array.Length; i++){
            SpawnInput_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = _round < local_RoundSet.Length ? local_SpawnData[_round, i].ToString() : "";
        }

        //라운드옵션
        SpeedMult_Input.transform.Find("Placeholder").GetComponent<Text>().text = _round < local_RoundSet.Length ? local_RoundSet[_round].Speed_Mult.ToString() : "";
        HpMult_Input.transform.Find("Placeholder").GetComponent<Text>().text = _round < local_RoundSet.Length ? local_RoundSet[_round].Hp_Mult.ToString() : "";
        SpawnDelay_Input.transform.Find("Placeholder").GetComponent<Text>().text = _round < local_RoundSet.Length ? local_RoundSet[_round].Spawn_Delay.ToString() : "";



        //임시변수에 데이터가 있는경우 가져오기
        //스폰 데이터
        for (int i = 0; i < System.Enum.GetValues(typeof(MonName)).Length; i++){
            if (EditorMgr.g_Inst.temp_MonSpawn_Data[_round, i] >= 0)
                SpawnInput_Array[i].text = EditorMgr.g_Inst.temp_MonSpawn_Data[_round, i].ToString();
            else
                SpawnInput_Array[i].text = "";

            ValueVerification(SpawnInput_Array[i], 0, 50);
        }


        //라운드 옵션
        if (EditorMgr.g_Inst.temp_RoundSet_Data[_round].Speed_Mult > 0.0f){
            SpeedMult_Input.text = EditorMgr.g_Inst.temp_RoundSet_Data[_round].Speed_Mult.ToString();
            ValueVerification(SpeedMult_Input, 1.0f, 2.0f);
        }
        else{
            SpeedMult_Input.text = "";
        }

        if (EditorMgr.g_Inst.temp_RoundSet_Data[_round].Hp_Mult > 0.0f){
            HpMult_Input.text = EditorMgr.g_Inst.temp_RoundSet_Data[_round].Hp_Mult.ToString();
            ValueVerification(HpMult_Input, 1.0f, 10.0f);
        }
        else{
            HpMult_Input.text = "";
        }

        if (EditorMgr.g_Inst.temp_RoundSet_Data[_round].Spawn_Delay > 0.0f){
            SpawnDelay_Input.text = EditorMgr.g_Inst.temp_RoundSet_Data[_round].Spawn_Delay.ToString();
            ValueVerification(SpawnDelay_Input, 0.1f, 5.0f);
        }
        else{
            SpawnDelay_Input.text = "";
        }

    }

    bool TempRoundDataCheck()
    {
        int cnt = 0;

        //스폰 데이터
        for (int i = 0; i < RoundCnt; i++)
        {
            for (int j = 0; j < System.Enum.GetValues(typeof(MonName)).Length; j++)
            {
                cnt += EditorMgr.g_Inst.temp_MonSpawn_Data[i, j] < 0 ? 1 : 0;
            }
        }

        //라운드 옵션
        for (int i = 0; i < EditorMgr.g_Inst.temp_RoundSet_Data.Length; i++)
        {
            cnt += EditorMgr.g_Inst.temp_RoundSet_Data[i].Hp_Mult < 0.0f ? 1 : 0;
            cnt += EditorMgr.g_Inst.temp_RoundSet_Data[i].Speed_Mult < 0.0f ? 1 : 0;
            cnt += EditorMgr.g_Inst.temp_RoundSet_Data[i].Spawn_Delay <= 0.0f ? 1 : 0;
        }

        return cnt <= 0;
    }





    void GameOptionDataSave(){
        EditorMgr.g_Inst.temp_GameSet_Data.Gold = (StartGold_Input.text == "" ? -1 : int.Parse(StartGold_Input.text.Replace(",", "")));
        EditorMgr.g_Inst.temp_GameSet_Data.Heart = (PlayerHeart_Input.text == "" ? -1 : int.Parse(PlayerHeart_Input.text.Replace(",", "")));
        EditorMgr.g_Inst.temp_GameSet_Data.ReloadCost = (ReloadCost_Input.text == "" ? -1 : int.Parse(ReloadCost_Input.text.Replace(",", "")));
    }

    void GameOptionTextUpdate(){
        if(EditorMgr.g_Inst.temp_GameSet_Data.Gold > 0){
            StartGold_Input.text = EditorMgr.g_Inst.temp_GameSet_Data.Gold.ToString();
            ValueVerification(StartGold_Input, 1, 1000000);
        }
        else{
            StartGold_Input.text = "";
        }

        if (EditorMgr.g_Inst.temp_GameSet_Data.Heart > 0){
            PlayerHeart_Input.text = EditorMgr.g_Inst.temp_GameSet_Data.Heart.ToString();
            ValueVerification(PlayerHeart_Input, 1, 1000000);
        }
        else{
            PlayerHeart_Input.text = "";
        }

        if (EditorMgr.g_Inst.temp_GameSet_Data.ReloadCost > 0)
        {
            ReloadCost_Input.text = EditorMgr.g_Inst.temp_GameSet_Data.ReloadCost.ToString();
            ValueVerification(ReloadCost_Input, 1, 1000000);
        }
        else
        {
            ReloadCost_Input.text = "";
        }
    }
   
    bool TempGameOptionDataCheck()
    {
        int cnt = 0;

        cnt += EditorMgr.g_Inst.temp_GameSet_Data.Gold < 0 ? 1 : 0;
        cnt += EditorMgr.g_Inst.temp_GameSet_Data.Heart < 0 ? 1 : 0;
        cnt += EditorMgr.g_Inst.temp_GameSet_Data.ReloadCost < 0 ? 1 : 0;

        return cnt <= 0;
    }





}






