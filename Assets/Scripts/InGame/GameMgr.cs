using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Text.RegularExpressions;
using SimpleJSON;



//게임 상태
public enum GameState{
    Loading,    //로딩 (맵,
    Ready,      //준비 (시작전 카운트, 다음 라운드 준비
    Playing,    //게임중
    End,        //게임끝 (게임오버, 클리어)
}

public class GameSetting {

    public int Heart = 0;                 //목숨
    public int Gold = 0;                  //돈
    public int ReloadCost = 0;
}

public class GameData_Format{

    public string _mapRoad_Data;
    public string _monSpawn_Data;
    public string _round_Setting;
    public string _tower_Setting;
    public string _monster_Setting;
    public string _game_Setting;
}




public class GameMgr : MonoBehaviour
{
    [HideInInspector] public static GameMgr g_GMGR_Inst = null; //싱글턴 인스턴스 변수


    [HideInInspector] public GameObject GridGroup = null;
    [HideInInspector] private GameObject GridTile = null;

    //그리드 그리는 용도 //추후 MapMgr에서 받아오게끔 수정
    [HideInInspector] public int Grid_X, Grid_Z = 0;
    [HideInInspector] public GameObject GuideCell;

    [HideInInspector] float StartCntTime = 10.0f;            //처음 시작시 카운트

    [HideInInspector] public GameState State;               //게임 상태 변수
    [HideInInspector] public float NextRound_delt = 0.0f;   //다음 라운드 델타타임 감소용 변수
    [HideInInspector] public int Round_Cnt = 0;


    [HideInInspector] private LayerMask m_TowerMask = -1;   //타워 레이어
    [HideInInspector] public GameObject m_TempTowerPick;    //선택 타워 임시저장


    float m_ClearTime = 0.0f;                               //클리어타임 계산을 위한 변수
    bool TimeRecord_flag = false;
    [HideInInspector] public int m_Heart = 0;                                 //목숨
    [HideInInspector] public int m_Gold = 0;                                  //돈
    [HideInInspector] public int m_ReloadCost = 0;

    //--------------- 머리위에 데미지 띄우기용 변수 선언
    Vector3 a_StCacPos = Vector3.zero;
    [HideInInspector] public Transform m_HUD_Canvas = null;
    [HideInInspector] public GameObject m_DamageObj = null;
    //--------------- 머리위에 데미지 띄우기용 변수 선언


    //-------------UI
    GameObject GoldText_obj;
    public Text Gold_Text;  //골드 텍스트
    Text Heart_Text;
    bool GridOnOff_bool = false;
    public Text Round_Text;
    Text Reload_InfoText;

    public Button Retrun_Btn;
    //-------------UI



    //-------------타워 정보 패널
    GameObject m_TowerInfo_Frame;                               //패널이 On상태일때 위치해야할 기준 오브젝트
    GameObject m_TowerInfo_Panel;                               //타워 정보 패널
    Vector3 m_TowerInfoOffPos;                                  //Off상태일때 위치해야할 위치값 (초기 패널위치)

    float OnPos_X = 0.0f;
    float OffPos_X = 0.0f;
    float Canvas_Width = 0.0f;

    private float m_InfoPanelSpeed = 2500.0f;                   //패널 OnOff 속도
    [HideInInspector] public bool m_InfoPanel_flag = false;     // True = On | False = OFF


    [HideInInspector] public Button Info_Close_Btn;                              //닫기 버튼
    [HideInInspector] public Image Info_Tower_Image;                             //타워 이미지
    [HideInInspector] public Text Info_TowerGrade_Text;                          //타워 등급
    [HideInInspector] public Text Info_TowerName_Text;                           //타워 이름
    [HideInInspector] public Text Info_TowerContents_Text;                       //타워 설명
    
    [HideInInspector] public Text Info_NextGrade_Text;                           //타워 다음 등급
    [HideInInspector] public Text Info_UpgradeCost_Text;                         //타워 업그레이드 비용
    [HideInInspector] public Button Info_Upgrade_Btn;                            //타워 업그레이드 버튼
    [HideInInspector] public Image Info_Coin_Image;                              //코인 이미지-레전더리시 액티브 비활성화
    [HideInInspector] public Text Info_NextGradeTitle_Text;                      //다음 등급 타이틀-레전더리시 액티브 비활성화



    [HideInInspector] public UI_InfoCell[] UI_InfoCellArray = new UI_InfoCell[6]; //타워 정보 셀 배열
    [HideInInspector] public Color32[] Grade_ColorArray;

    [HideInInspector] public Sprite UI_Type_Sprite;
    [HideInInspector] public Sprite UI_Target_Sprite;
    [HideInInspector] public Sprite UI_Damage_Sprite;
    [HideInInspector] public Sprite UI_AttackSpeed_Sprite;
    [HideInInspector] public Sprite UI_None_Sprite;


    [HideInInspector] public Sprite UI_ChainAttack_Sprite;
    [HideInInspector] public Sprite UI_DebuffTime;
    [HideInInspector] public Sprite UI_Poison_Sprite;

    //-------------타워 정보 패널


    //-------------타워 리스트 패널
    [HideInInspector] public Button ReLoad_Btn;
    [HideInInspector] public GameObject TowerList_Panel;
    [HideInInspector] public GameObject[] Slot_Array;

    [HideInInspector] public GameObject TL_Clam;
    [HideInInspector] public GameObject TL_Crab;
    [HideInInspector] public GameObject TL_ElectricEel;
    [HideInInspector] public GameObject TL_Puffer;
    [HideInInspector] public GameObject TL_PoisonPuffer;
    //-------------타워 리스트 패널



    //------------게임종료 안내창
    [HideInInspector] GameObject    GameFinishWindow_Obj;
    [HideInInspector] Text          WindowTitle_Text;
    [HideInInspector] Text          TimeTitle_Text;
    [HideInInspector] Text          Time_Text;
    [HideInInspector] Text          InfoTitle_Text;
    [HideInInspector] Text          Info_Text;
    [HideInInspector] Button        FinishLobby_Btn;
    [HideInInspector] Button        Like_Btn;

    //------------게임종료 안내창



    //------------환경설정
    [HideInInspector] Button      Setting_Btn;
    [HideInInspector] GameObject  SettingPanel_Obj;
    //------------환경설정


    //
    public GameObject LevelUp_Effect;
    public GameObject EffectGroup;


    public Text TimeText;
    public Animator TimeAnim;
    Sprite defImage;
    //


    GameObject InfoMessage;




    [HideInInspector] public string g_Message = "";
    string UserInfo_UpdateUrl = "http://devwhale.dothome.co.kr/ARD_Test/UserInfo_Update.php";
    string UserInfo_SelectUrl = "http://devwhale.dothome.co.kr/ARD_Test/UserInfo_Select.php";
    string PlayCount_UpdateUrl = "http://devwhale.dothome.co.kr/ARD_Test/UPDATE_PlayCount.php";
    string LikeCount_UpdateUrl = "http://devwhale.dothome.co.kr/ARD_Test/UPDATE_LikeCount.php";



    string path;


    //리소스 로드
    void ResourceLoad () {

        //그리드 타일 로드
        GridTile = Resources.Load<GameObject>("Prefab/Tile/Grid_Tile");



        //UI 타워 정보 패널 로드
        UI_Type_Sprite = Resources.Load<Sprite>("Image/UI/TowerInfo/Info_Type");
        UI_Target_Sprite = Resources.Load<Sprite>("Image/UI/TowerInfo/Info_Target");
        UI_Damage_Sprite = Resources.Load<Sprite>("Image/UI/TowerInfo/Info_Damage");
        UI_AttackSpeed_Sprite = Resources.Load<Sprite>("Image/UI/TowerInfo/Info_AttackSpeed");
        UI_None_Sprite = Resources.Load<Sprite>("Image/UI/TowerInfo/Info_None");

        UI_ChainAttack_Sprite = Resources.Load<Sprite>("Image/UI/TowerInfo/Info_ChainCnt");
        UI_DebuffTime = Resources.Load<Sprite>("Image/UI/TowerInfo/Info_Time");
        UI_Poison_Sprite = Resources.Load<Sprite>("Image/UI/TowerInfo/Info_Poison");

        TL_Clam = Resources.Load<GameObject>("Prefab/UI/TL_Clam");
        TL_Crab = Resources.Load<GameObject>("Prefab/UI/TL_Crab");
        TL_ElectricEel = Resources.Load<GameObject>("Prefab/UI/TL_ElectricEel");
        TL_Puffer = Resources.Load<GameObject>("Prefab/UI/TL_Puffer");
        TL_PoisonPuffer = Resources.Load<GameObject>("Prefab/UI/TL_PoisonPuffer");

        InfoMessage = Resources.Load<GameObject>("Prefab/UI/InfoMessage");

        LevelUp_Effect = Resources.Load<GameObject>("Prefab/Effect/LevelUp");
        
    }


    void ObjectLoad () {
        //오브젝트 로드
        GuideCell = GameObject.Find("GridGroup").transform.Find("GuideCell").gameObject;
        GoldText_obj = GameObject.Find("Canvas").transform.Find("UserInfo_Panel").Find("GoldUI").Find("Gold_Text").gameObject;
        Gold_Text = GoldText_obj.GetComponent<Text>();
        Heart_Text = GameObject.Find("Canvas").transform.Find("UserInfo_Panel").Find("HeartUI").Find("Heart_Text").GetComponent<Text>();

        Round_Text = GameObject.Find("Round_Panel").transform.Find("Round_Text").GetComponent<Text>();
        Reload_InfoText = GameObject.Find("Update_Panel").transform.Find("ReloadCost_Text").GetComponent<Text>();

        m_TowerInfo_Frame = GameObject.Find("TowerInfo_Frame").transform.gameObject;
        m_TowerInfo_Panel = m_TowerInfo_Frame.transform.Find("TowerInfo_Panel").gameObject;
        TowerList_Panel = GameObject.Find("TowerList_Panel").gameObject;
        ReLoad_Btn = GameObject.Find("Reload_Btn").GetComponent<Button>();
        Retrun_Btn = GameObject.Find("Return_Btn").GetComponent<Button>();

        Info_Close_Btn = GameObject.Find("Info_Close_Btn").GetComponent<Button>();
        Info_Tower_Image = GameObject.Find("Info_Tower_Image").GetComponent<Image>();
        Info_TowerGrade_Text = GameObject.Find("Info_Grade_Text").GetComponent<Text>();
        Info_TowerName_Text = GameObject.Find("Info_TowerName_Text").GetComponent<Text>();
        Info_TowerContents_Text = GameObject.Find("Info_TowerContents_Text").GetComponent<Text>();
        Info_NextGrade_Text = GameObject.Find("Info_NextGrade_Text").GetComponent<Text>();
        Info_UpgradeCost_Text = GameObject.Find("Info_UpgradeCost_Text").GetComponent<Text>();
        Info_Upgrade_Btn = GameObject.Find("Info_Upgrade_Btn").GetComponent<Button>();
        Info_Coin_Image = GameObject.Find("Info_Coin_Image").GetComponent<Image>();
        Info_NextGradeTitle_Text = GameObject.Find("Info_NextGradeTitle_Text").GetComponent<Text>();

        GameFinishWindow_Obj = GameObject.Find("Canvas").transform.Find("GameFinishWindow").gameObject;
        WindowTitle_Text = GameFinishWindow_Obj.transform.Find("TitleBar").Find("Title_Text").GetComponent<Text>();
        TimeTitle_Text = GameFinishWindow_Obj.transform.Find("InfoPanel").Find("TimeTitle_Text").GetComponent<Text>();
        Time_Text = GameFinishWindow_Obj.transform.Find("InfoPanel").Find("Time_Text").GetComponent<Text>();
        InfoTitle_Text = GameFinishWindow_Obj.transform.Find("InfoPanel").Find("InfoTitle_Text").GetComponent<Text>();
        Info_Text = GameFinishWindow_Obj.transform.Find("InfoPanel").Find("Info_Text").GetComponent<Text>();
        FinishLobby_Btn = GameFinishWindow_Obj.transform.Find("Finisht_Btn").GetComponent<Button>();
        Like_Btn = GameFinishWindow_Obj.transform.Find("Like_Btn").GetComponent<Button>();


        Setting_Btn = GameObject.Find("Setting_Btn").GetComponent<Button>();
        SettingPanel_Obj = GameObject.Find("Canvas").transform.Find("SettingPanel").gameObject;

        EffectGroup = GameObject.Find("EffectGroup").gameObject;

        TimeText = GameObject.Find("Time_Panel").transform.Find("Time_Text").GetComponent<Text>();
        TimeAnim = GameObject.Find("Time_Panel").transform.Find("TimeAnim").GetComponent<Animator>();
        defImage = TimeAnim.gameObject.GetComponent<Image>().sprite;


        Slot_Array = new GameObject[3];
        
        
        
        for (int i = 0; i < 3; i++) {
            Slot_Array[i] = TowerList_Panel.transform.Find("Slot" + (i + 1).ToString()).gameObject;

        }


        for (int i = 0; i < UI_InfoCellArray.Length; i++) {

            UI_InfoCellArray[i] = new UI_InfoCell(GameObject.Find("InfoCell_" + i.ToString()).transform.Find("Icon").Find("Icon_Image").gameObject.GetComponent<Image>(),
                GameObject.Find("InfoCell_" + i.ToString()).transform.Find("CellTitle_Text").gameObject.GetComponent<Text>(),
                GameObject.Find("InfoCell_" + i.ToString()).transform.Find("CellContents_Text").gameObject.GetComponent<Text>());

        }

        Grade_ColorArray = new Color32[4]{
            new Color32(0, 40, 255, 255),
            new Color32(100, 0, 255, 255),
            new Color32(255, 255, 0, 255),
            new Color32(0, 255, 0, 255)
        };
    }


    void ButtonClickCollection()
    {

        Info_Upgrade_Btn.onClick.AddListener(() =>
        {
            TowerStatus.Inst.TowerUpgrade(m_TempTowerPick.transform.Find("Tower").gameObject);
        });

        ReLoad_Btn.onClick.AddListener(() => {

            if (m_Gold < m_ReloadCost){
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Error_Sound, 1.0f);

                GameObject emptyGameObject = new GameObject("Message");
                emptyGameObject.transform.parent = ReLoad_Btn.gameObject.transform;
                FloatingText FloatingMessage = emptyGameObject.AddComponent<FloatingText>();
                FloatingMessage.Setting(ReLoad_Btn.gameObject.transform.position,
                                        180.0f, 30, "골드가 부족합니다.", Color.red);
                return;
            }

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Reload_Sound, 2.0f);

            ReloadList();
            MinusGold(m_ReloadCost);
        });

        Retrun_Btn.onClick.AddListener(() => {

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.SceneChange_Sound, 1.0f);

            UnityEngine.SceneManagement.SceneManager.LoadScene("Editor");
        });

        FinishLobby_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.SceneChange_Sound, 1.0f);

            if (GlobalGameData.m_GameMode == GameMode.Test){
                UnityEngine.SceneManagement.SceneManager.LoadScene("Editor");

            }
            else{
                GlobalGameData.Reset();
                UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
            }

        });



        Like_Btn.onClick.AddListener(() => {

            if (Like_Btn.gameObject.GetComponent<Image>().color == new Color32(100, 100, 100, 255))
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Select_Sound, 1.0f);

            StartCoroutine(Update_Like());

            Like_Btn.gameObject.GetComponent<Image>().color = new Color32(100, 100, 100, 255);

        });



        Setting_Btn.onClick.AddListener(() =>{

            if (SettingPanel_Obj.activeSelf == true)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.OpenWindow_Sound, 1.0f);


            //일시정지
            Time.timeScale = 0.0f;

            //타워 범위 끄기
            if (m_TempTowerPick != null && m_TempTowerPick.transform.Find("Tower").Find("Range").GetComponent<SpriteRenderer>().enabled == true)
                m_TempTowerPick.transform.Find("Tower").Find("Range").GetComponent<SpriteRenderer>().enabled = false;

            //그리드 끄기
            GridOnOff_bool = false;

            GridGroup.SetActive(GridOnOff_bool);

            //그리드가 꺼졌다면,
            if (GridGroup.activeSelf == false) 
            {
                //선택타워 초기화
                m_TempTowerPick = null;

                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.CloseWindow_Sound, 1.0f);

                //타워 정보 판넬 끄기
                m_InfoPanel_flag = false;
            }

            SettingPanel_Obj.SetActive(true);

        });




    }




    void Awake ()
    {
        Time.timeScale = 1.0f;


        g_GMGR_Inst = this;
        State = GameState.Loading;

        //리소스 로드
        ResourceLoad();

        //오브젝트 로드
        ObjectLoad();

        //버튼클릭 모음
        ButtonClickCollection();


        path = "Data/GameData";

        

        if (GlobalUserData.m_Uid_str == "" || GlobalUserData.m_Uid_str == null){

            GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "유저 데이터가 존재하지 않습니다.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Login");

            });
           
        }
        else{
            GameData_Load();
        }
    }


    void Start()
    {
        //타워 레이어
        m_TowerMask = 1 << LayerMask.NameToLayer("Tower");

        SpawnMgr.Inst.GetRoundData();


        //게임 초기 설정
        m_Heart = GlobalGameData.m_GameSetting.Heart;
        m_Gold = GlobalGameData.m_GameSetting.Gold;
        m_ReloadCost = GlobalGameData.m_GameSetting.ReloadCost;
        Gold_Text.text = "X " + m_Gold.ToString();
        Heart_Text.text = "X " + m_Heart.ToString();
        Round_Text.text = "1/" + GlobalGameData.m_RoundSet_Array.Length + " 라운드";
        Reload_InfoText.text = string.Format("{0:#,###}",GlobalGameData.m_GameSetting.ReloadCost).ToString();
        
        

        Canvas_Width = GameObject.Find("Canvas").GetComponent<RectTransform>().rect.width;
        OnPos_X = (Canvas_Width/2) - (m_TowerInfo_Frame.GetComponent<RectTransform>().rect.width / 2);
        OffPos_X = 500.0f;



        m_TowerInfo_Frame.transform.localPosition = new Vector3(OnPos_X, m_TowerInfo_Frame.transform.localPosition.y, m_TowerInfo_Frame.transform.localPosition.z);
        m_TowerInfo_Panel.transform.localPosition = new Vector3(OffPos_X, 0.0f, 0.0f);
        


        m_TowerInfoOffPos = m_TowerInfo_Panel.transform.position;

        Info_Close_Btn.onClick.AddListener(() => {

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.CloseWindow_Sound, 1.0f);

            m_InfoPanel_flag = false;

        });

        

        GameFinishWindow_Obj.SetActive(false);
        SettingPanel_Obj.SetActive(false);
        Like_Btn.gameObject.SetActive(false);

        TimeText.gameObject.SetActive(false);

        //BGM Random Play
        SoundMgr.g_inst.RandomPlay();


        //초기 뽑기
        ReloadList();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(1)) {
            Debug.Log("Frame local = " + m_TowerInfo_Frame.transform.localPosition);
            Debug.Log("Panel local = " + m_TowerInfo_Panel.transform.localPosition);
        }
        




        //마우스 버튼 클릭  //IsPointerOverUIObject() 결과값(레이캐스트에 걸린 UI가) 0이여야 실행
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject()) {


            //클릭한 위치 레이캐스트
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            //타워 클릭시
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_TowerMask.value)) {

                SpriteRenderer RangeSprite = hit.transform.Find("Tower").Find("Range").GetComponent<SpriteRenderer>();


                //다른타워 클릭
                if (m_TempTowerPick != null && m_TempTowerPick != hit.transform.gameObject) {

                    
                    //이전 타워 끄기
                    m_TempTowerPick.transform.Find("Tower").Find("Range").GetComponent<SpriteRenderer>().enabled = false;

                    GridOnOff_bool = true;
                    RangeSprite.enabled = GridOnOff_bool;


                }
                //같은타워 클릭 or 처음 클릭
                else {

                    GridOnOff_bool = !GridOnOff_bool;
                    RangeSprite.enabled = GridOnOff_bool;
                }

                //선택타워 임시저장
                m_TempTowerPick = hit.transform.gameObject;

                //타워 정보 판넬 켜기-----------------------------------

                if(m_InfoPanel_flag == false)
                    SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.OpenWindow_Sound, 1.0f);

                m_InfoPanel_flag = true;

                InfoPanelUpdate(hit.transform.Find("Tower").gameObject);

            }
            //타워 이외 클릭시
            else {

                //이전 선택타워 범위표시 끄기
                if (m_TempTowerPick != null && m_TempTowerPick.transform.Find("Tower").Find("Range").GetComponent<SpriteRenderer>().enabled == true) 
                    m_TempTowerPick.transform.Find("Tower").Find("Range").GetComponent<SpriteRenderer>().enabled = false;

                GridOnOff_bool = false;
            }

            

            GridGroup.SetActive(GridOnOff_bool);

            //그리드가 꺼졌다면,
            if (GridGroup.activeSelf == false) {
                
                //선택타워 초기화
                m_TempTowerPick = null;

                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.CloseWindow_Sound, 1.0f);

                //타워 정보 판넬 끄기
                m_InfoPanel_flag = false;

            }
        }



        SlotCheck();

        InfoPanelOnOff_Update();

        Round_Update();

    } //void Update()


    private void Round_Update () {


        //게임준비중 일때,
        if (State == GameState.Ready) {
            //5초 카운트
            if (0 < StartCntTime){
                StartCntTime -= Time.deltaTime;

                if (TimeText.gameObject.activeSelf == false)
                {
                    TimeAnimPlay();
                }
                
                TimeText.text = string.Format("{0:N1}", StartCntTime) + " s";

                //시작 시간정보 저장
                m_ClearTime = Time.time;

                if( 0 >= StartCntTime)
                {
                    if (TimeText.gameObject.activeSelf == true)
                    {
                        TimeAnimStop();
                    }
                }
            }

            //라운드 타임이 있는경우
            if (0 < NextRound_delt)
                //몬스터 그룹에 몬스터가 남아있지 않을 경우
                if (GameObject.Find("ActiveGroup").gameObject.transform.childCount == 0) {
                    
                    
                    NextRound_delt -= Time.deltaTime;

                    if (TimeText.gameObject.activeSelf == false)
                    {
                        TimeAnimPlay();
                    }

                    TimeText.text = string.Format("{0:N1}", NextRound_delt) + " s";

                    //Debug.Log(NextRound_delt);
                }

            //5초 이후일때 && 다음 라운드 카운팅이 끝난경우,
            if (StartCntTime <= 0 && NextRound_delt <= 0 && State != GameState.End) {

                //게임중 상태로 변경
                State = GameState.Playing;

                if (TimeText.gameObject.activeSelf == true)
                {
                    TimeAnimStop();
                }

                //라운드 시작
                Debug.Log(Round_Cnt + 1 + "라운드 시작");
                SpawnMgr.Inst.RoundStart(Round_Cnt);
            }
        }
        //게임 끝 (클리어)
        else if (State == GameState.End && GameObject.Find("ActiveGroup").gameObject.transform.childCount == 0 && Time.timeScale > 0) {

            m_ClearTime = Time.time - m_ClearTime;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Clear_Sound, 1.0f);

            Debug.Log("클리어!!");

            TimeAnimStop();

            if (GlobalGameData.m_GameMode != GameMode.Test)
            {
                //유저아이디, 게임id, 최대 라운드, 남은 목숨, 클리어 타임, 클리어 날짜 저장
                //기록보다 현재 라운드가 높은경우
                if (GlobalUserData.m_TopRound_int < Round_Cnt + 1)
                {

                    StartCoroutine(UserInfo_Update(GlobalUserData.m_Uid_str, GlobalGameData.m_GameCode_Str, Round_Cnt + 1, m_Heart, TimeFormat(m_ClearTime), System.DateTime.Now.ToString("yyyy-MM-dd"), 0));


                    //게임 플레이 카운트 증가
                    StartCoroutine(Update_PlayCount());
                }
                //기록과 현재 라운드가 같은경우
                else if (GlobalUserData.m_TopRound_int == (Round_Cnt + 1))
                {

                    //목숨 더 많을 경우
                    if (GlobalUserData.m_ClearHeart_int < m_Heart)
                    {

                        //기록
                        StartCoroutine(UserInfo_Update(GlobalUserData.m_Uid_str, GlobalGameData.m_GameCode_Str, Round_Cnt + 1, m_Heart, TimeFormat(m_ClearTime), System.DateTime.Now.ToString("yyyy-MM-dd"), 0));

                        //게임 플레이 카운트 증가
                        StartCoroutine(Update_PlayCount());


                    }
                    //목숨 동일하고 && 클리어타임 더 빠를다면,
                    else if (GlobalUserData.m_ClearHeart_int == m_Heart && TimeRestore(GlobalUserData.m_ClearTime_str) > m_ClearTime)
                    {

                        //기록
                        StartCoroutine(UserInfo_Update(GlobalUserData.m_Uid_str, GlobalGameData.m_GameCode_Str, Round_Cnt + 1, m_Heart, TimeFormat(m_ClearTime), System.DateTime.Now.ToString("yyyy-MM-dd"), 0));


                        //게임 플레이 카운트 증가
                        StartCoroutine(Update_PlayCount());

                    }
                }
            }
            
          
            Time.timeScale = 0.0f;


            GameFinishWindow_Obj.SetActive(true);

            if(GlobalGameData.m_GameMode == GameMode.Custom)
                Like_Btn.gameObject.SetActive(true);
            else
                Like_Btn.gameObject.SetActive(false);


            //
            if (GlobalGameData.m_GameMode == GameMode.Test)
            {

                WindowTitle_Text.text = "테스트 종료";

                TimeTitle_Text.text = "테스트 게임이";
                Time_Text.text = "종료되었습니다.";

                InfoTitle_Text.text = "에디터로";
                Info_Text.text = "돌아갑니다.";

                FinishLobby_Btn.transform.Find("Text").GetComponent<Text>().text = "돌아가기";

            }
            else
            {
                WindowTitle_Text.text = "Clear";

                TimeTitle_Text.text = "클리어 타임 : ";
                Time_Text.text = TimeFormat(m_ClearTime);

                InfoTitle_Text.text = "남은 목숨 : ";
                Info_Text.text = m_Heart + "개";

                FinishLobby_Btn.transform.Find("Text").GetComponent<Text>().text = "로비로 이동";
            }
            

        }
        //게임끝 (클리어)

        //게임끝 (게임오버)
        if (m_Heart <= 0 && State != GameState.End && Time.timeScale > 0) {
            State = GameState.End;
            m_ClearTime = Time.time - m_ClearTime;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.GameOver_Sound, 1.0f);
            TimeAnimStop();


            Debug.Log("게임오버!!");

            if (GlobalUserData.m_TopRound_int <= Round_Cnt) {
                
                Round_Cnt = (Round_Cnt-1) < 0 ? 0 : (Round_Cnt - 1); 
                //유저아이디, 게임id, 최대 라운드 저장
                StartCoroutine(UserInfo_Update(GlobalUserData.m_Uid_str, GlobalGameData.m_GameCode_Str, Round_Cnt, 0, "", System.DateTime.Now.ToString("yyyy-MM-dd"),0));

                //게임 플레이 카운트 증가
                StartCoroutine(Update_PlayCount());

            }

            Time.timeScale = 0.0f;

            GameFinishWindow_Obj.SetActive(true);

            if (GlobalGameData.m_GameMode == GameMode.Custom)
                Like_Btn.gameObject.SetActive(true);
            else
                Like_Btn.gameObject.SetActive(false);
            
            //
            if (GlobalGameData.m_GameMode == GameMode.Test)
            {

                WindowTitle_Text.text = "테스트 종료";

                TimeTitle_Text.text = "";
                Time_Text.text = "";

                InfoTitle_Text.text = "";
                Info_Text.text = "";

                FinishLobby_Btn.transform.Find("Text").GetComponent<Text>().text = "돌아가기";

               

            }
            else
            {
                WindowTitle_Text.text = "Game Over";

                TimeTitle_Text.text = "플레이 타임 : ";
                Time_Text.text = TimeFormat(m_ClearTime);

                InfoTitle_Text.text = "최대 라운드 : ";
                Info_Text.text = Round_Cnt + "라운드";

                FinishLobby_Btn.transform.Find("Text").GetComponent<Text>().text = "로비로 이동";

            }

        }
    }






    // UI터치 시 GameObject 터치 무시
    private bool IsPointerOverUIObject () {

        //이벤트 시스템
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        //현재 마우스 포인터
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        //레이캐스트 결과 리스트 생성
        List<RaycastResult> results = new List<RaycastResult>();

        //현재 마우스 위치에 레이캐스트에 걸리는(UI)것들 results에 담기
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        //result 개수 (UI개수) 반환
        return results.Count > 0;
    }


    //안내용 그리드 그리기
    public void DrawGrid () {
        if (GridTile != null) {
            for (int x = 0; x < Grid_X; x++) {
                for (int z = 0; z < Grid_Z; z++) {
                    Instantiate(GridTile, new Vector3(x, GridGroup.transform.position.y, z), GridTile.transform.rotation).transform.parent = GridGroup.transform;
                }
            }
        }
        //그리드 끄기
        GridGroup.SetActive(false);
    }



    //----------------데미지 텍스트
    public void DamageText (GameObject _monsterobj, int _value, Transform _txtTr, Color ? _color = null) {


        if (_monsterobj.activeSelf == true) {


            GameObject a_DamClone = (GameObject)Instantiate(m_DamageObj);

            if (a_DamClone != null && m_HUD_Canvas != null) {

                a_StCacPos = new Vector3(_txtTr.position.x + 0.5f, 1.1f,
                                         _txtTr.position.z + 0.5f);

                a_DamClone.transform.SetParent(m_HUD_Canvas);
                DamageText a_DamageTx = a_DamClone.GetComponent<DamageText>();
                a_DamageTx.m_target = _monsterobj;

                a_DamageTx.m_SavePos = a_StCacPos;
                a_DamageTx.m_DamageVel = (int)_value;


                if (_color != null) {
                    a_DamageTx._color = _color.HasValue ? _color.Value : Color.white;
                    a_DamageTx.m_RefText.color = _color.HasValue ? _color.Value : Color.white;
                }

                a_DamClone.transform.position = a_StCacPos;

            }
        }
    }
    //----------------데미지 텍스트




    public void PlusGold (MonType _monType) {
        int plusGold = 0;

        if (_monType == MonType.Normal)
            plusGold = 100;

        else if (_monType == MonType.MIddleBoss)
            plusGold = 200;

        else if (_monType == MonType.Boss)
            plusGold = 300;


        
        m_Gold += plusGold;

            

        //Gold Text 업데이트
        Gold_Text.text = "X " + m_Gold.ToString();

        GameObject a_floatingTextObj = Instantiate(GoldText_obj, GoldText_obj.transform.parent);
        FloatingText a_FloatingText_Script = a_floatingTextObj.AddComponent<FloatingText>();
        a_FloatingText_Script.Setting(GoldText_obj);
        a_FloatingText_Script.m_floatingText.text = "+" + plusGold;
        a_FloatingText_Script.m_floatingText.color = Color.blue;
        a_FloatingText_Script.m_MaxHeight = 20.0f;
    }

    public void MinusGold (int _gold) {

        m_Gold -= _gold;


        //Gold Text 업데이트
        Gold_Text.text = "X " + m_Gold.ToString();

        GameObject a_floatingTextObj = Instantiate(GoldText_obj, GoldText_obj.transform.parent);
        FloatingText a_FloatingText_Script = a_floatingTextObj.AddComponent<FloatingText>();
        a_FloatingText_Script.Setting(GoldText_obj);
        a_FloatingText_Script.m_floatingText.text = "-"+_gold;
        a_FloatingText_Script.m_floatingText.color = Color.red;
        a_FloatingText_Script.m_MaxHeight = 20.0f;
    }


    public void MinusHeart(int _damage)
    {
        SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.LostHeart_Sound, 2.0f);

        m_Heart -= _damage;

        Heart_Text.text = "X " + m_Heart.ToString();

        GameObject a_floatingTextObj = Instantiate(Heart_Text.gameObject, Heart_Text.gameObject.transform.parent);
        FloatingText a_FloatingText_Script = a_floatingTextObj.AddComponent<FloatingText>();
        a_FloatingText_Script.Setting(Heart_Text.gameObject);
        a_FloatingText_Script.m_floatingText.text = "-" + _damage;
        a_FloatingText_Script.m_floatingText.color = Color.red;
        a_FloatingText_Script.m_MaxHeight = 20.0f;
    }



    public void InfoPanelOnOff_Update () {

        if (m_InfoPanel_flag == false) {
            if (m_TowerInfo_Panel.transform.position.x < m_TowerInfoOffPos.x) {
                m_TowerInfo_Panel.transform.position =
                    Vector3.MoveTowards(m_TowerInfo_Panel.transform.position,
                                        m_TowerInfoOffPos, m_InfoPanelSpeed * Time.deltaTime);
            }
        }
        else {
            if (m_TowerInfo_Frame.transform.position.x < m_TowerInfo_Panel.transform.position.x) {
                m_TowerInfo_Panel.transform.position =
                    Vector3.MoveTowards(m_TowerInfo_Panel.transform.position,
                                        m_TowerInfo_Frame.transform.position, m_InfoPanelSpeed * Time.deltaTime);
            }
        }

    }

    public void InfoPanelUpdate (GameObject _towerobj) {

        //타워 이미지 (타워 게임 오브젝트)
        Info_Tower_Image.sprite = _towerobj.GetComponent<SpriteRenderer>().sprite;

        TowerStatus.Inst.TowerInfoPanelUpdate(_towerobj, UI_InfoCellArray);
    }


    public void ReloadList () {

        //시드초기화
        UnityEngine.Random.InitState((int)(DateTime.Now.Ticks));

        int[] rndNumber = new int[3];

        //0부터 [TowerName 이넘클래스 내의 개수]까지 값중에서 랜덤값 3개 뽑기s
        for (int i = 0; i < rndNumber.Length; i++) {

            //슬롯 하위에 오브젝트가 있다면, 오브젝트 제거
            if(TowerList_Panel.transform.Find("Slot" + (i + 1).ToString()).childCount > 0)
                Destroy(TowerList_Panel.transform.Find("Slot" + (i + 1).ToString()).GetChild(0).gameObject);

            rndNumber[i] = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(TowerName)).Length);

            //Slot 배열, 랜덤값 배열 넘기기
            TowerStatus.Inst.TLSlotSet(rndNumber[i], Slot_Array[i]);

        }
    }

    public void SlotCheck () {

        if (Slot_Array[0].gameObject.transform.childCount == 0 && 
            Slot_Array[1].gameObject.transform.childCount == 0 && 
            Slot_Array[2].gameObject.transform.childCount == 0) {

            ReloadList();
        }
    }



    public void GameData_Load () {

        if(GlobalGameData.m_GameMode == GameMode.Test){
            g_Message = "테스트 모드로 진입하셨습니다.";
            Retrun_Btn.gameObject.SetActive(true);
            return;
        }
        else if(GlobalGameData.m_GameMode == GameMode.Custom)
        {
            g_Message = "커스텀 모드로 진입하셨습니다.";
            Retrun_Btn.gameObject.SetActive(false);
            StartCoroutine(UserInfo_Select());

        }
        else
        {
            if (Resources.Load<TextAsset>(path) != null)
            {
                
                var load_json = Resources.Load<TextAsset>(path);

                //Json -> GDF 복호화
                GameData_Format GD_Load = JsonUtility.FromJson<GameData_Format>(load_json.ToString());


                //타워리스트는 TowerStatus에서 Restore
                //(Class 타입)
                GlobalGameData.m_TowersData_TL = JsonUtility.FromJson<TowerList>(GD_Load._tower_Setting);

                //맵 (int 2차배열)
                GlobalGameData.m_Map_Array = LobbyMgr.Inst.StringToArray(GD_Load._mapRoad_Data);
                //몬스터 스폰 데이터 (int 2차배열)
                GlobalGameData.m_MonSpawn_Array = LobbyMgr.Inst.StringToArray(GD_Load._monSpawn_Data);
                //라운드 데이터 (배열)
                GlobalGameData.m_RoundSet_Array = JsonHelper.FromJson<RoundSetting>(GD_Load._round_Setting);
                //몬스터 셋팅 (배열)
                GlobalGameData.m_MonSet_Array = JsonHelper.FromJson<MonSet>(GD_Load._monster_Setting);
                //게임 셋팅 (Class 변환)
                GlobalGameData.m_GameSetting = JsonUtility.FromJson<GameSetting>(GD_Load._game_Setting);

                GlobalGameData.m_GameMode = GameMode.Standard;  

                GlobalGameData.m_GameCode_Str = "0";
                Retrun_Btn.gameObject.SetActive(false);

            }
            else
            {

                GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

                InfoWindow.GetComponent<InfoMessage>().info_Text.text = "게임 파일이 존재하지 않습니다.";
                InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
                });
            }

        }
            
    }

    public void TimeAnimPlay()
    {
        TimeAnim.enabled = true;

        TimeAnim.Play("Time");
        TimeText.gameObject.SetActive(true);
    }

    public void TimeAnimStop()
    {
        TimeAnim.enabled = false;
        TimeText.gameObject.SetActive(false);
        TimeAnim.gameObject.GetComponent<Image>().sprite = defImage;
    }

    IEnumerator UserInfo_Update (string _uid, string _gamenum, int _topround, int _heart, string _cleartime, string _cleardate,int _like) {
        //PHP - UserInfo_Update
        //입력받은 userid와 gameid로 기록이 있나 확인
        //기록이 없으면, Insert  
        //  게임오버시, user_id, game_number, top_round 값만 / 클리어시, user_id, game_number, top_round, heart, clear_time, clear_date

        //기록이 있으면, 입력받은 top_round 기록된 top_round보다 같거나높을때만(신기록) Update
        //  게임오버시, top_round 만 업데이트 / 클리어시, top_round, heart, clear_time, clear_date 업데이트


        //계정생성후 기본게임에 대한 기록 생성을 위해 user_id와 기본값들을 넣어줌
        //기록이 있는경우 아무런작업을 안하고, 기록이 없는경우 Insert를 통해 초기 기록생성
        WWWForm form = new WWWForm();
        form.AddField("user_id", _uid, System.Text.Encoding.UTF8);
        form.AddField("game_id", _gamenum, System.Text.Encoding.UTF8);
        form.AddField("top_round", _topround);
        form.AddField("heart", _heart);
        form.AddField("clear_time", _cleartime, System.Text.Encoding.UTF8);
        form.AddField("clear_date", _cleardate, System.Text.Encoding.UTF8);
        form.AddField("like_count", _like);

        WWW webRequest = new WWW(UserInfo_UpdateUrl, form);
        yield return webRequest;
        g_Message = webRequest.text;

        Debug.Log(g_Message);


    }

    IEnumerator UserInfo_Select()
    {
        //PHP로 MySql에서 inputField의 이름값으로 검색
        WWWForm form = new WWWForm();
        form.AddField("user_id", GlobalUserData.m_Uid_str, System.Text.Encoding.UTF8);
        form.AddField("game_id", GlobalGameData.m_GameCode_Str, System.Text.Encoding.UTF8);
        WWW webRequest = new WWW(UserInfo_SelectUrl, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(webRequest.bytes);
        g_Message = sz;


        if (sz.Contains("Select-Success!!") == true)
        {
            //text로 이름과 점수 출력해주고
            //JSON 파싱
            if (sz.Contains("top_round") == true)
            {

                var N = JSON.Parse(sz);
                if (N != null)
                {
                    if (N["top_round"] != null)
                    {
                        GlobalUserData.m_TopRound_int = N["top_round"];
                    }

                    if (N["heart"] != null)
                    {
                        GlobalUserData.m_ClearHeart_int = N["heart"];
                    }

                    if (N["clear_time"] != null)
                    {
                        GlobalUserData.m_ClearTime_str = N["clear_time"];
                    }

                    if (N["clear_date"] != null)
                    {
                        GlobalUserData.m_ClearDate_str = N["clear_date"];
                    }

                    if(N["like"] != null)
                    {
                        if (N["like"] == "1")
                            Like_Btn.gameObject.GetComponent<Image>().color = new Color32(100, 100, 100, 255);
                        else
                            Like_Btn.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

                    }

                }
            }//if (sz.Contains("nick_name") == true)
        }//if (sz.Contains("Login-Success!!") == true)
        else
        {

            GlobalUserData.m_TopRound_int = 0;
            GlobalUserData.m_ClearHeart_int = 0;
            GlobalUserData.m_ClearTime_str = "";
            GlobalUserData.m_ClearDate_str = "";

           //데이터가 없다면,
           g_Message = "등록된 정보가 없습니다.";
        }


    }



    IEnumerator Update_PlayCount()
    {
        //PHP로 MySql에서 inputField의 이름값으로 검색
        WWWForm form = new WWWForm();
        form.AddField("game_id", GlobalGameData.m_GameCode_Str, System.Text.Encoding.UTF8);
        WWW webRequest = new WWW(PlayCount_UpdateUrl, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(webRequest.bytes);
        g_Message = sz;


        if (sz.Contains("UPDATE-Success!!") == true)
        {

            Debug.Log("플레이 카운트 증가");
            g_Message = "플레이 카운트 증가";

        }//if (sz.Contains("Login-Success!!") == true)
        else
        {


            //데이터가 없다면,
            Debug.Log("업데이트 에러");

            g_Message = "업데이트 에러";
        }

    }

    IEnumerator Update_Like()
    {
        //PHP로 MySql에서 inputField의 이름값으로 검색
        WWWForm form = new WWWForm();
        form.AddField("game_id", GlobalGameData.m_GameCode_Str, System.Text.Encoding.UTF8);
        form.AddField("user_id", GlobalUserData.m_Uid_str, System.Text.Encoding.UTF8);
        WWW webRequest = new WWW(LikeCount_UpdateUrl, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(webRequest.bytes);
        g_Message = sz;


        if (sz.Contains("UPDATE-Success!!") == true)
        {

            Debug.Log("좋아요 카운트 증가");
            g_Message = "좋아요 카운트 증가";

        }
        else
        {


            //데이터가 없다면,
            Debug.Log("업데이트 에러");

            g_Message = "업데이트 에러";
        }

    }



    //시간값을 00:00:00.00 형식 문자열로 변환
    string TimeFormat (float _time) {
        string hour = "00";
        string min = "00";
        string sec = "00";
        string ms = "00";
        string time_format = "";

        if (_time > 3600)
            hour = string.Format("{0:D2}", ((int)_time / 3600));

        if (_time > 60)
            min = string.Format("{0:D2}", ((int)_time / 60 % 60));

        if (_time > 0)
            sec = string.Format("{0:D2}", (int)_time % 60);

        ms = string.Format("{0:D2}", Mathf.FloorToInt((_time - Mathf.FloorToInt(_time)) * 100));


        time_format = hour + ":" + min + ":" + sec + "." + ms;

        return time_format;
    }

    //시간 문자열을 float 형태로 복원
    float TimeRestore (string _time) {
        float _realNum = 0.0f;
        float _decimal = 0.0f;

        string[] _divTime;

        _divTime = _time.Split(new string[] { ":", "." }, StringSplitOptions.None);

        _realNum += int.Parse(_divTime[0]) * 3600;
        _realNum += int.Parse(_divTime[1]) * 60;
        _realNum += int.Parse(_divTime[2]);

        _decimal = int.Parse(_divTime[3]) * 0.01f;

        Debug.Log(_realNum);
        Debug.Log(_decimal);
        Debug.Log(_realNum + _decimal);


        return _realNum + _decimal;
    }


    /*
    void OnGUI () {
        if (g_Message != "") {
            GUILayout.Label("<color=White><size=25>" + g_Message + "</size></color>");
        }
    }
    */


}



/*

JsonUtility = 배열을 받을 수 없음.
JsonHelper = 한번 감싸준뒤 변환해서 문제 해결




//--------------저장(로컬)
     if (Input.GetMouseButtonDown(1)) {
            //맵데이터 2차배열
            //MapMgr.Inst.RoadArray

            //라운드데이터 클래스
            //GlobalGameData.m_RoundSet_Array

            //타워셋팅 클래스
            //TowerStatus

            //몬스터셋팅 클래스
            //GlobalGameData.m_MonSet_Array

            //게임셋팅 클래스
            //GlobalGameData.m_GameSetting

            //전체
            GameData_Format gDATA = new GameData_Format();
            gDATA._mapRoad_Data = LobbyMgr.Inst.ArrayToString(MapMgr.Inst.RoadArray);
            gDATA._monSpawn_Data = LobbyMgr.Inst.ArrayToString(GlobalGameData.m_MonSpawn_Array);
            gDATA._round_Setting = JsonHelper.ToJson(GlobalGameData.m_RoundSet_Array);
            gDATA._tower_Setting = JsonUtility.ToJson(TL_Load);
            gDATA._monster_Setting = JsonHelper.ToJson(GlobalGameData.m_MonSet_Array);
            gDATA._game_Setting = JsonUtility.ToJson(GlobalGameData.m_GameSetting);

            string test_JSON = JsonUtility.ToJson(gDATA);

            string path = Application.dataPath + "/" + "GameData_1" + ".Json";
            File.WriteAllText(path, test_JSON);

            Debug.Log("");
        }

*/
