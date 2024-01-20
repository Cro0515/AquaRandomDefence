using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.IO;


public enum EditStap {
    MapEdit,
    TowerSetting,
    MonsterSetting,
    RoundSetting,
    Test,
    Upload
}

public enum MapMakeStep {
    None,
    SetStart,
    DrawRoad,
    SetEnd,
    Finish
}

public enum TileType {
    None,
    Start,
    End,
    Road,
    Forbidden
}

public enum RoadDirection {
    None,
    Left,   //y - 1
    Right,  //y + 1
    Up,     //x - 1
    Down,   //x + 1
}


public class Cell {
    public TileType TileType;                                       //타일 종류 (없음,스타트,엔드,길)
    public RoadDirection[] TileDir = new RoadDirection[2];          //설치가능 방향
    public bool SetPossible;                                        //설치가능 여부
    public GameObject GuideCell;


    public Cell () {
        TileType = TileType.None;
        TileDir[0] = RoadDirection.None;
        TileDir[1] = RoadDirection.None;
        SetPossible = false;
        GuideCell = null;
    }

    public Cell DeepCopy () {
        Cell newCell = new Cell();
        newCell.TileType = this.TileType;
        newCell.TileDir[0] = this.TileDir[0];
        newCell.TileDir[1] = this.TileDir[1];
        newCell.SetPossible = this.SetPossible;

        return newCell;
    }
}


public class History {
    public Cell cell;               //m_Map의 값 (타일타입, 타일방향)
    public GameObject roadObj;
    public Vector2 pos;
    public List<Button> btnList;
    public Vector2 prevXY;
    public Vector2 nextXY;
    public int roadType;

}




public class EditorMgr : MonoBehaviour {
    [HideInInspector] public static EditorMgr g_Inst = null;


    //-------------그리드
    [HideInInspector] private GameObject GridGroup = null;
    [HideInInspector] private GameObject GridTile = null;

    [HideInInspector] public int Grid_X, Grid_Z = 0;
    //-------------그리드




    //-------------맵 에디터 판넬
    GameObject SelectUI_Obj;

    Button Road_Horizon_1_Btn;
    Button Road_Horizon_2_Btn;
    Button Road_Vertical_1_Btn;
    Button Road_Vertical_2_Btn;

    Button Road_Corner_1_Btn;
    Button Road_Corner_2_Btn;
    Button Road_Corner_3_Btn;
    Button Road_Corner_4_Btn;

    Button Undo_Btn;
    Button Redo_Btn;

    int RoadType;
    //-------------맵 에디터 판넬




    //-------------맵 에디트
    bool EditMode = true;
    MapMakeStep m_MapMakeStep;
    GameObject m_GuideCell;

    GameObject m_CellGroup;
    GameObject m_FrameGroup;
    GameObject m_RoadGroup;

    Cell[,] m_Map = new Cell[21, 12];                               //맵 배열

    Color Color_Green = new Color32(0, 255, 0, 100);                //설치 가능 지역 표시 위한 컬러
    Color Color_Alpha = new Color32(0, 0, 0, 0);                    //설치 가능 지역 이외 기본값 컬러

    Color Color_RoadActive = new Color32(255, 255, 255, 255);       //활성화 시 알파값
    Color Color_RoadDisabled = new Color32(255, 255, 255, 125);     //비활성화시 알파값 (반투명)

    Vector2 m_prevXY;                                               //바로 이전에 설치된 타일의 좌표
    Vector2 m_nextXY;                                               //현재 타일 기준 다음에 설치 가능한 좌표

    GameObject m_SelectRoad_Obj;                                    //타일 설치를 위한 선택된 타일 오브젝트 저장용 변수
    RoadDirection[] m_SelectRoad_Dir = new RoadDirection[2];        //선택된 타일의 길 방향 저장용 변수


    //Undo Redo
    public Stack<History> m_Undo_Stack = new Stack<History>();
    public Stack<History> m_Redo_Stack = new Stack<History>();

    GameObject Stack_Obj;

    //Undo Redo



    //타일 타입에 따른 방향 부여
    RoadDirection[] m_Horizon_Dir = new RoadDirection[2];           
    RoadDirection[] m_Vertical_Dir = new RoadDirection[2];
    RoadDirection[] m_Corner_1_Dir = new RoadDirection[2];
    RoadDirection[] m_Corner_2_Dir = new RoadDirection[2];
    RoadDirection[] m_Corner_3_Dir = new RoadDirection[2];
    RoadDirection[] m_Corner_4_Dir = new RoadDirection[2];
    //타일 타입에 따른 방향 부여

    List<Button> m_RoadBtn_List = new List<Button>();              //버튼 활성, 비활성 위한 버튼배열


    //실제 타일 오브젝트
    private GameObject RoadTile_LeftTop;
    private GameObject RoadTile_CenterTop;
    private GameObject RoadTile_RightTop;
    private GameObject RoadTile_LeftMiddle;
    private GameObject RoadTile_RightMiddle;
    private GameObject RoadTile_LeftBottom;
    private GameObject RoadTile_CenterBottom;
    private GameObject RoadTile_RightBottom;
    //실제 타일 오브젝트

    //-------------맵 에디트




    //-------------슬라이드 판넬 변수
    Vector3 m_ActivePos;
    Vector3 m_HidePos;
    private float m_ScSpeed = 2000.0f;
    bool PanelHide_flag = false;

    public Button CloseBtn;

    GameObject m_ToolPanel;
    Camera m_MainCamera;

    GameObject ClosePos;
    GameObject PanelStartPos;
    //--------------슬라이드 판넬 변수



    //--------------상단 step bar UI 변수
    public EditStap m_EditStep;

    public Button StepBar_MapEdit_Btn;
    public Button StepBar_TowerSetting_Btn;
    public Button StepBar_MonsterSetting_Btn;
    public Button StepBar_RoundSetting_Btn;
    public Button StepBar_Test_Btn;
    public Button StepBar_Upload_Btn;
    public Button StepBar_Prev;


    public Color Color_StepBar_Yellow = new Color32(255,255,0,255);
    public Color Color_StepBar_Gray = new Color32(200, 200, 200, 255);
    public Color Color_StepBar_Disable = new Color32(200, 200, 200, 100);
    public Color Color_StepBar_White = new Color32(255, 255, 255, 255);
    
    public Color m_Blink_Start_Color;
    public Color m_Blink_End_Color;

    public GameObject m_Blink_Target;
    public bool m_BlinkSwitch_flag = false;
    float m_BlinkTime = 0.0f;
    float m_BlinkSpeed = 2.0f;

    GameObject TowerSetting_Panel;
    GameObject MonsterSetting_Panel;
    GameObject RoundSetting_Panel;
    GameObject Test_Panel;
    GameObject Upload_Panel;

    Button Setting_Btn;

    GameObject Setting_Panel;
    //--------------상단 스탭UI 변수


    //--------------타워 데이터 로드 및 변수

    string path;

    public TowerList local_Towers_Data;

    public  int[,] local_MonSpawn_Data;
    public  RoundSetting[] local_Round_Data;
    public  MonSet[] local_MonSet_Data;
    public  GameSetting local_GameSetting_Data;






    //--------------타워 데이터 로드 및 변수


    //게임 데이터 임시 변수
    public int[,] temp_Map_Data = new int[21, 12];
    public int[,] temp_MonSpawn_Data;
    public TowerList temp_Towers_Data = new TowerList();
    public MonSet[] temp_MonSet_Data;
    public RoundSetting[] temp_RoundSet_Data;
    public GameSetting temp_GameSet_Data;


    //게임 데이터 임시 변수

    public GameObject InfoMessage;

    bool m_soundPlay = true;


    //리소스 로드 모음
    void ResourceLoad () {

        GridTile = Resources.Load<GameObject>("Prefab/Tile/Grid_Tile");
        m_GuideCell = Resources.Load<GameObject>("Prefab/Tile/GuideCell");

        RoadTile_LeftTop = Resources.Load<GameObject>("Prefab/Tile/RoadTile_LeftTop");
        RoadTile_CenterTop = Resources.Load<GameObject>("Prefab/Tile/RoadTile_CenterTop");
        RoadTile_RightTop = Resources.Load<GameObject>("Prefab/Tile/RoadTile_RightTop");
        RoadTile_LeftMiddle = Resources.Load<GameObject>("Prefab/Tile/RoadTile_LeftMiddle");
        RoadTile_RightMiddle = Resources.Load<GameObject>("Prefab/Tile/RoadTile_RightMiddle");
        RoadTile_LeftBottom = Resources.Load<GameObject>("Prefab/Tile/RoadTile_LeftBottom");
        RoadTile_CenterBottom = Resources.Load<GameObject>("Prefab/Tile/RoadTile_CenterBottom");
        RoadTile_RightBottom = Resources.Load<GameObject>("Prefab/Tile/RoadTile_RightBottom");

        InfoMessage = Resources.Load<GameObject>("Prefab/UI/InfoMessage");

    }

    //오브젝트 로드 모음
    void ObjectLoad () {

        GridGroup = GameObject.Find("GridGroup").gameObject;

        m_ToolPanel = GameObject.Find("ToolPanel").gameObject;
        m_MainCamera = GameObject.Find("Main Camera").gameObject.GetComponent<Camera>();

        PanelStartPos = m_ToolPanel.transform.Find("PanelStartPos").gameObject;
        ClosePos = m_ToolPanel.transform.Find("ClosePos").gameObject;
        m_ActivePos = GameObject.Find("MapEditToolFrame").gameObject.transform.position;


        float clacPos = m_ActivePos.x - (ClosePos.transform.position.x - PanelStartPos.transform.position.x);
        m_HidePos = new Vector3(clacPos, m_ActivePos.y, m_ActivePos.z);


        //맵 에디트 패널--------------
        SelectUI_Obj = GameObject.Find("Canvas").transform.Find("Select").gameObject;

        Undo_Btn = GameObject.Find("Undo_Btn").GetComponent<Button>();
        Redo_Btn = GameObject.Find("Redo_Btn").GetComponent<Button>();

        Road_Horizon_1_Btn = GameObject.Find("Road_Horizon_Btn_1").gameObject.GetComponent<Button>();
        Road_Horizon_2_Btn = GameObject.Find("Road_Horizon_Btn_2").gameObject.GetComponent<Button>();
        Road_Vertical_1_Btn = GameObject.Find("Road_Vertical_Btn_1").gameObject.GetComponent<Button>();
        Road_Vertical_2_Btn = GameObject.Find("Road_Vertical_Btn_2").gameObject.GetComponent<Button>();

        Road_Corner_1_Btn = GameObject.Find("Road_Corner_Btn_1").gameObject.GetComponent<Button>();
        Road_Corner_2_Btn = GameObject.Find("Road_Corner_Btn_2").gameObject.GetComponent<Button>();
        Road_Corner_3_Btn = GameObject.Find("Road_Corner_Btn_3").gameObject.GetComponent<Button>();
        Road_Corner_4_Btn = GameObject.Find("Road_Corner_Btn_4").gameObject.GetComponent<Button>();

        m_CellGroup = GridGroup.transform.Find("CellGroup").gameObject;
        m_FrameGroup = GridGroup.transform.Find("FrameGroup").gameObject;
        m_RoadGroup = GameObject.Find("Map").transform.Find("RoadGroup").gameObject;
        Stack_Obj = GameObject.Find("History").transform.Find("StackObj").gameObject;
        //맵 에디트 패널--------------



        //상단 스탭바--------

        StepBar_MapEdit_Btn = GameObject.Find("StepBar").transform.Find("MapEdit_Button").GetComponent<Button>();
        StepBar_TowerSetting_Btn = GameObject.Find("StepBar").transform.Find("TowerSetting_Button").GetComponent<Button>();
        StepBar_MonsterSetting_Btn = GameObject.Find("StepBar").transform.Find("MonsterSetting_Button").GetComponent<Button>();
        StepBar_RoundSetting_Btn = GameObject.Find("StepBar").transform.Find("RoundSetting_Button").GetComponent<Button>();
        StepBar_Test_Btn = GameObject.Find("StepBar").transform.Find("TestPlay_Button").GetComponent<Button>();
        StepBar_Upload_Btn = GameObject.Find("StepBar").transform.Find("Upload_Button").GetComponent<Button>();


        TowerSetting_Panel = GameObject.Find("Canvas").transform.Find("TowerEditPanelObj").gameObject;
        MonsterSetting_Panel= GameObject.Find("Canvas").transform.Find("MonsterSettingPanelObj").gameObject;
        RoundSetting_Panel = GameObject.Find("Canvas").transform.Find("RoundSettingPanelObj").gameObject;
        Test_Panel = GameObject.Find("Canvas").transform.Find("TestPanelObj").gameObject;
        Upload_Panel = GameObject.Find("Canvas").transform.Find("UploadPanelObj").gameObject;

        Setting_Btn = GameObject.Find("StepBar").transform.Find("Setting_Button").GetComponent<Button>();
        Setting_Panel = GameObject.Find("Canvas").transform.Find("SettingPanel").gameObject;
    }

    //버튼 메소드 모음
    void BtnClickCollect () {


        CloseBtn.onClick.AddListener(() => {


            PanelHide_flag = !PanelHide_flag;
            CloseBtn.transform.localScale = new Vector3(-CloseBtn.transform.localScale.x, CloseBtn.transform.localScale.y, CloseBtn.transform.localScale.z);
            GridGroup.SetActive(!GridGroup.activeSelf);

            //숨길시
            if (PanelHide_flag == true) {
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.CloseWindow_Sound, 1.0f);

                m_MainCamera.gameObject.transform.position = new Vector3(10.0f, m_MainCamera.gameObject.transform.position.y, m_MainCamera.gameObject.transform.position.z);
                m_MainCamera.orthographicSize = 6.5f;
            }
            else {
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.OpenWindow_Sound, 1.0f);

                m_MainCamera.gameObject.transform.position = new Vector3(7.5f, m_MainCamera.gameObject.transform.position.y, m_MainCamera.gameObject.transform.position.z);
                m_MainCamera.orthographicSize = 7.5f;
            }
        });

        Road_Horizon_1_Btn.onClick.AddListener(() => {
            RoadBtnClick(Road_Horizon_1_Btn, RoadTile_CenterTop, m_Horizon_Dir);

        });

        Road_Horizon_2_Btn.onClick.AddListener(() => {
            RoadBtnClick(Road_Horizon_2_Btn, RoadTile_CenterBottom, m_Horizon_Dir);

        });

        Road_Vertical_1_Btn.onClick.AddListener(() => {
            RoadBtnClick(Road_Vertical_1_Btn, RoadTile_LeftMiddle, m_Vertical_Dir);

        });

        Road_Vertical_2_Btn.onClick.AddListener(() => {
            RoadBtnClick(Road_Vertical_2_Btn, RoadTile_RightMiddle, m_Vertical_Dir);

        });

        Road_Corner_1_Btn.onClick.AddListener(() => {
            RoadBtnClick(Road_Corner_1_Btn, RoadTile_LeftTop, m_Corner_1_Dir);

        });

        Road_Corner_2_Btn.onClick.AddListener(() => {
            RoadBtnClick(Road_Corner_2_Btn, RoadTile_RightTop, m_Corner_2_Dir);

        });

        Road_Corner_3_Btn.onClick.AddListener(() => {
            RoadBtnClick(Road_Corner_3_Btn, RoadTile_LeftBottom, m_Corner_3_Dir);

        });

        Road_Corner_4_Btn.onClick.AddListener(() => {
            RoadBtnClick(Road_Corner_4_Btn, RoadTile_RightBottom, m_Corner_4_Dir);

        });

        Undo_Btn.onClick.AddListener(() => {
            History_Undo();
        });

        Redo_Btn.onClick.AddListener(() => {
            History_Redo();
        });


        StepBar_MapEdit_Btn.onClick.AddListener(() => {

            if (StepBar_MapEdit_Btn.GetComponent<Image>().color == Color_StepBar_Disable ||
                StepBar_MapEdit_Btn.GetComponent<Image>().color == Color_StepBar_Yellow)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_3_Sound, 1.0f);


            m_EditStep = EditStap.MapEdit;
            StepBar_Click(StepBar_MapEdit_Btn);

            if(m_MapMakeStep == MapMakeStep.Finish){
                StepBar_Start(StepBar_TowerSetting_Btn);
            }
            StepBar_MapEdit_Btn.GetComponent<Image>().color = Color_StepBar_Yellow;
        });

        StepBar_TowerSetting_Btn.onClick.AddListener(() => {

            if (StepBar_TowerSetting_Btn.GetComponent<Image>().color == Color_StepBar_Disable ||
                StepBar_TowerSetting_Btn.GetComponent<Image>().color == Color_StepBar_Yellow)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_3_Sound, 1.0f);

            m_EditStep = EditStap.TowerSetting;
            StepBar_Click(StepBar_TowerSetting_Btn);
        });

        StepBar_MonsterSetting_Btn.onClick.AddListener(() => {

            if (StepBar_MonsterSetting_Btn.GetComponent<Image>().color == Color_StepBar_Disable ||
                StepBar_MonsterSetting_Btn.GetComponent<Image>().color == Color_StepBar_Yellow)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_3_Sound, 1.0f);

            m_EditStep = EditStap.MonsterSetting;
            StepBar_Click(StepBar_MonsterSetting_Btn);
        });

        StepBar_RoundSetting_Btn.onClick.AddListener(() => {

            if (StepBar_RoundSetting_Btn.GetComponent<Image>().color == Color_StepBar_Disable ||
                StepBar_RoundSetting_Btn.GetComponent<Image>().color == Color_StepBar_Yellow)
                return;
            
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_3_Sound, 1.0f);

            m_EditStep = EditStap.RoundSetting;
            StepBar_Click(StepBar_RoundSetting_Btn);
        });

        StepBar_Test_Btn.onClick.AddListener(() => {

            if (StepBar_Test_Btn.GetComponent<Image>().color == Color_StepBar_Disable ||
                StepBar_Test_Btn.GetComponent<Image>().color == Color_StepBar_Yellow)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_3_Sound, 1.0f);

            m_EditStep = EditStap.Test;
            StepBar_Click(StepBar_Test_Btn);
        });

        StepBar_Upload_Btn.onClick.AddListener(() => {

            if (StepBar_Upload_Btn.GetComponent<Image>().color == Color_StepBar_Disable ||
                StepBar_Upload_Btn.GetComponent<Image>().color == Color_StepBar_Yellow)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_3_Sound, 1.0f);

            m_EditStep = EditStap.Upload;
            StepBar_Click(StepBar_Upload_Btn);
        });


        Setting_Btn.onClick.AddListener(() => {

            if (Setting_Panel.activeSelf == true)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.OpenWindow_Sound, 1.0f);

            Setting_Panel.SetActive(true);
            

        });

    }




    private void Awake () {
        g_Inst = this;

        path = "Data/GameData";



        //리소스 로드
        ResourceLoad();

        //오브젝트 로드
        ObjectLoad();


        BtnClickCollect();


        //
        m_Horizon_Dir[0] = RoadDirection.Left;
        m_Horizon_Dir[1] = RoadDirection.Right;

        m_Vertical_Dir[0] = RoadDirection.Up;
        m_Vertical_Dir[1] = RoadDirection.Down;

        m_Corner_1_Dir[0] = RoadDirection.Down;
        m_Corner_1_Dir[1] = RoadDirection.Right;

        m_Corner_2_Dir[0] = RoadDirection.Left;
        m_Corner_2_Dir[1] = RoadDirection.Down;

        m_Corner_3_Dir[0] = RoadDirection.Up;
        m_Corner_3_Dir[1] = RoadDirection.Right;

        m_Corner_4_Dir[0] = RoadDirection.Left;
        m_Corner_4_Dir[1] = RoadDirection.Up;
        //

        if (GlobalUserData.m_Uid_str == "" || GlobalUserData.m_Uid_str == null){
            GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "유저 데이터가 존재하지 않습니다.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
            });
        }
        else{
            //로컬 파일 로드
            GameData_Load();
        }
    }




    // Start is called before the first frame update
    void Start() {

        m_EditStep = EditStap.MapEdit; ;

        //게임시작 준비
        Set_StartPoint();

        m_soundPlay = false;
        Road_Horizon_1_Btn.onClick.Invoke();
        Redo_Btn.image.color = Color_RoadDisabled;
        Undo_Btn.image.color = Color_RoadDisabled;



        //stepbar 초기설정
        StepBar_MapEdit_Btn.GetComponent<Image>().color = Color_StepBar_Yellow;
        StepBar_TowerSetting_Btn.GetComponent<Image>().color = Color_StepBar_Disable;
        StepBar_MonsterSetting_Btn.GetComponent<Image>().color = Color_StepBar_Disable;
        StepBar_RoundSetting_Btn.GetComponent<Image>().color = Color_StepBar_Disable;
        StepBar_Test_Btn.GetComponent<Image>().color = Color_StepBar_Disable;
        StepBar_Upload_Btn.GetComponent<Image>().color = Color_StepBar_Disable;
        StepBar_Prev = StepBar_MapEdit_Btn;


        //Panel 초기 설정
        TowerSetting_Panel.SetActive(false);
        MonsterSetting_Panel.SetActive(false);
        RoundSetting_Panel.SetActive(false);
        Test_Panel.SetActive(false);
        Upload_Panel.SetActive(false);
        Setting_Panel.SetActive(false);
        

        //게임모드 true일시
        if (GlobalGameData.m_GameMode == GameMode.Test)
        {
            //Debug.Log("원상복구");

            //글로벌 데이터에 있는 데이터를 임시변수에 입력

            temp_MonSpawn_Data = GlobalGameData.m_MonSpawn_Array;
            temp_Towers_Data = GlobalGameData.m_TowersData_TL;
            temp_MonSet_Data = GlobalGameData.m_MonSet_Array;
            temp_RoundSet_Data = GlobalGameData.m_RoundSet_Array;
            temp_GameSet_Data = GlobalGameData.m_GameSetting;


            //스탭 = 테스트
            //m_EditStep = EditStap.Test;
            //StepBar_Test_Btn.onClick.Invoke();

            //맵 데이터 기반으로 맵 그리기

            //스택에서 스택으로 (reverse 작업)
            Stack<History> _revHistory = new Stack<History>();

            int _size = GlobalGameData.m_undoHistory.Count;
            for (int i = 0; i < _size; i++){
                _revHistory.Push(GlobalGameData.m_undoHistory.Pop());
            }

            //스택에서 뽑아서 해당 버튼 클릭
            _size = _revHistory.Count;
            for (int i = 0; i<_size; i++){

                History _tempHis = _revHistory.Pop();

                NumbertoRoad(_tempHis.roadType).onClick.Invoke();

                //해당 좌표 클릭
                MapEdit_Click((int)_tempHis.pos.x, (int)_tempHis.pos.y);
            }

            m_EditStep = EditStap.Test;
            StepBar_Click(StepBar_Test_Btn);



        }
    }

    // Update is called once per frame
    void Update () {


        //판넬 온오프
        PanelOnOffUpdate();

        //상단 메뉴바 깜빡임
        if(m_Blink_Target != null)
            StepBar_Blink();

        if (m_EditStep == EditStap.MapEdit) {

            //버튼 클릭시
            if (Input.GetMouseButtonDown(0) && PanelHide_flag == false) {
                Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
                int x = Mathf.RoundToInt(point.x);
                int y = Mathf.RoundToInt(point.z);
                //Debug.Log("클릭 좌표 : " + x + " , " + y);

                MapEdit_Click(x, y);
            }


            if (Input.GetMouseButtonDown(1)) {

                Debug.Log(GlobalGameData.m_GameMode);

                /*
                Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
                int x = Mathf.RoundToInt(point.x);
                int y = Mathf.RoundToInt(point.z);

                Debug.Log("클릭 좌표 : " + x + " , " + y);
                Debug.Log("[테스트] 설치가능 여부 : " + m_Map[x, y].SetPossible);
                Debug.Log("[테스트] 타일 타입 : " + m_Map[x, y].TileType);
                Debug.Log("[테스트] 이전 블럭 위치 : " + m_prevXY);
                Debug.Log("[테스트] 현재 상태 : " + m_EditStep);
               
                */
            }
        }

        if (Input.GetMouseButtonDown(1)) {

            Debug.Log(ClosePos.transform.position);
            //Debug.Log(Screen.width + " / " + Screen.height);


        }
    }






    //----------------------------------------------------------------------------------


    //판넬 온오프
    void PanelOnOffUpdate () {


        //숨기기 x
        if (PanelHide_flag == false) {
            if (m_ToolPanel.transform.position.x < m_ActivePos.x) {
                m_ToolPanel.transform.position = Vector3.MoveTowards(m_ToolPanel.transform.position, m_ActivePos, m_ScSpeed * Time.deltaTime);
            }

        }
        //숨기기
        else {
            if (m_ToolPanel.transform.position.x > m_HidePos.x) {
                m_ToolPanel.transform.position = Vector3.MoveTowards(m_ToolPanel.transform.position, m_HidePos, m_ScSpeed * Time.deltaTime);
            }

        }

    }

    //안내용 그리드 그리기
    public void DrawGrid () {
        if (GridTile != null && m_GuideCell != null) {
            for (int x = 0; x < Grid_X; x++) {
                for (int z = 0; z < Grid_Z; z++) {
                    Instantiate(GridTile, new Vector3(x, m_FrameGroup.transform.position.y, z), GridTile.transform.rotation).transform.parent = m_FrameGroup.transform;

                    m_Map[x, z] = new Cell();

                    m_Map[x, z].GuideCell = Instantiate(m_GuideCell, new Vector3(x, m_CellGroup.transform.position.y, z), m_GuideCell.transform.rotation);
                    m_Map[x, z].GuideCell.transform.parent = m_CellGroup.transform;
                    m_Map[x, z].GuideCell.GetComponent<SpriteRenderer>().color = Color_Alpha;
                    m_Map[x, z].GuideCell.SetActive(false);
                }
            }
        }

        GridGroup.SetActive(true);
    }

    //스타트 지점 설치 준비
    void Set_StartPoint () {

        SelectUI_Obj.SetActive(true);


        //가로 블럭 이외 블럭들은 선택못하게

        m_RoadBtn_List.Add(Road_Horizon_1_Btn);
        m_RoadBtn_List.Add(Road_Horizon_2_Btn);
        RoadBtn_ListActive(m_RoadBtn_List);



        if (m_MapMakeStep != MapMakeStep.SetStart) {

            //왼쪽 사이드 생성가능지역 표시
            for (int x = 0; x < 21; x++) { //x
                for (int y = 0; y < 12; y++) {  //y
                    if (x == 0 && !(y == 0 || y == 11)) {

                        Set_PossibleCell(new Vector2(x, y));
                    }

                    //설치 금지 구역
                    if (x == 0 || x == 20 || y == 11 || y == 0) {
                        m_Map[x, y].TileType = TileType.Forbidden;
                    }
                }
            }
        }
        m_MapMakeStep = MapMakeStep.SetStart;
    }





    //방향에따른 좌표 계산 메소드
    //방향값과 좌표
    Vector2 Calc_Vector (RoadDirection _dir, Vector2 _curXY) {
        Vector2 tempXY = _curXY;

        switch (_dir) {

            case RoadDirection.Right:   //x+1
                tempXY.x += 1;
                break;

            case RoadDirection.Left:    //x-1
                tempXY.x -= 1;
                break;

            case RoadDirection.Up:      //y+1
                tempXY.y += 1;
                break;

            case RoadDirection.Down:    //y-1
                tempXY.y -= 1;
                break;
        }

        //_curXY의 xy값 최대치 최소치 정해주기
        if (tempXY.x < 0 || tempXY.x > 21 || tempXY.y < 0 || tempXY.y > 12) {
            tempXY.x = -1;
            tempXY.y = -1;
        }

        return tempXY;
    }

    //현재블럭의 설치가능 위치에 이전블럭이 존재하는지 체크
    bool Check_PrevCell (RoadDirection[] _dir, Vector2 _curXY, Vector2 _prevXY) {

        //----현재블럭의 설치가능 좌표 구하기
        Vector2[] possibleXY = new Vector2[2];

        for (int i = 0; i < _dir.Length; i++) {

            //방향값 없으면 패스
            if (_dir[i] == RoadDirection.None)
                continue;

            //방향에따른 좌표 계산
            possibleXY[i] = Calc_Vector(_dir[i], _curXY);
        }
        //----현재블럭의 설치가능 좌표 구하기


        //현재블럭의 설치가능 구역안에 prevXY값이 있는지 체크 있다면, true 반환
        if (Array.Exists(possibleXY, x => x == _prevXY) == true)
            return true;
        else {
            return false;
        }
    }

    void MapEdit_Click (int _x, int _y) {
        //마우스 좌표 -> 배열 좌표
       

        //설치 가능 지역 아닐경우 리턴
        if ((_x >= 0 && _y >= 0 && _x < 21 && _y < 12) == false || m_Map[_x, _y].SetPossible == false)
            return;


        //스타트 지점 설치 단계
        if (m_MapMakeStep == MapMakeStep.SetStart) {
            History_Reset();

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Install_Sound, 1.0f);

            //해당 위치에 START오브젝트 생성, 배열값 변경
            GameObject StartObj = Instantiate(RoadTile_CenterTop, new Vector3(_x, m_RoadGroup.transform.position.y, _y), RoadTile_CenterTop.transform.rotation);
            StartObj.transform.parent = m_RoadGroup.transform;
            StartObj.name = "Start";



            m_Map[_x, _y].TileType = TileType.Start;
            m_Map[_x, _y].TileDir[0] = m_SelectRoad_Dir[0];
            m_Map[_x, _y].TileDir[1] = m_SelectRoad_Dir[1];



            //모든 설치가능 초기화
            Reset_PossibleCell();
            for (int i = 0; i < 2; i++) {

                //m_Map과 현재좌표 넘겨서 설치가능 좌표받기
                Vector2 possibleVector = Calc_Vector(m_Map[_x, _y].TileDir[i], new Vector2(_x, _y));

                //생성가능지역으로 설정
                Set_PossibleCell(possibleVector);
            }



            //다음 스탭으로
            m_MapMakeStep = MapMakeStep.DrawRoad;

            //막아두었던 버튼 다시 활성화
            //Left와 연결가능한 버튼만 활성화

            m_RoadBtn_List.Clear();

            m_RoadBtn_List.Add(Road_Horizon_1_Btn);
            m_RoadBtn_List.Add(Road_Horizon_2_Btn);
            m_RoadBtn_List.Add(Road_Corner_2_Btn);
            m_RoadBtn_List.Add(Road_Corner_4_Btn);

            ////맨위 또는 맨아래 일 경우 코너 리스트에서 제외  /0,10 0,1
            if(_y == 10)
                m_RoadBtn_List.Remove(Road_Corner_4_Btn);
            if(_y == 1)
                m_RoadBtn_List.Remove(Road_Corner_2_Btn);


            RoadBtn_ListActive(m_RoadBtn_List);

            //설치한 좌표 저장
            m_prevXY.x = _x;
            m_prevXY.y = _y;


            RoadType = RoadtoNumber(StartObj);

            //작업내용 저장 //Undo Redo 기능
            History_save(m_Map[_x, _y], StartObj, new Vector2(_x, _y), m_RoadBtn_List, m_prevXY, RoadType);

            //Undo 버튼 활성화
            Undo_Btn.image.color = Color_RoadActive;

        }
        //길 설치 단계
        else if (m_MapMakeStep == MapMakeStep.DrawRoad) {
            //현재블럭의 설치가능지역에 이전블럭이 있는지 체크
            if (Check_PrevCell(m_SelectRoad_Dir, new Vector2(_x, _y), m_prevXY) == true) {

                History_Reset();

                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Install_Sound, 1.0f);

                //있다면, 생성
                GameObject RoadObj = Instantiate(m_SelectRoad_Obj, new Vector3(_x, m_RoadGroup.transform.position.y, _y), m_SelectRoad_Obj.transform.rotation);
                RoadObj.transform.parent = m_RoadGroup.transform;
                RoadObj.name = "Road";




                m_Map[_x, _y].TileType = TileType.Road;
                m_Map[_x, _y].TileDir[0] = m_SelectRoad_Dir[0];
                m_Map[_x, _y].TileDir[1] = m_SelectRoad_Dir[1];



                //모든 설치가능 초기화
                Reset_PossibleCell();


                for (int i = 0; i < 2; i++) {
                    //m_Map과 현재좌표 넘겨서 설치가능 좌표받기
                    Vector2 possibleVector = Calc_Vector(m_Map[_x, _y].TileDir[i], new Vector2(_x, _y));

                    if (m_Map[(int)possibleVector.x, (int)possibleVector.y].TileType == TileType.None) {
                        //생성가능지역으로 설정
                        Set_PossibleCell(possibleVector);

                        m_nextXY = possibleVector;
                    }
                    //마지막 설치 단계일때,
                    else if (m_Map[(int)possibleVector.x, (int)possibleVector.y].TileType == TileType.Forbidden && possibleVector.x >= 20) {

                        Set_PossibleCell(possibleVector);

                        m_nextXY = possibleVector;
                        m_MapMakeStep = MapMakeStep.SetEnd;

                        // 길 버튼 수평만 활성화
                        m_RoadBtn_List.Clear();



                        m_RoadBtn_List.Add(Road_Horizon_1_Btn);
                        m_RoadBtn_List.Add(Road_Horizon_2_Btn);
                    }
                }


                if (m_MapMakeStep == MapMakeStep.DrawRoad) {

                    //타일 전체 비활성화
                    m_RoadBtn_List.Clear();

                    //이전 방향 기준으로 활성화
                    //현재 블럭과 이어질 수 있는 블럭들 전부 활성화
                    for (int i = 1; i < Enum.GetValues(typeof(RoadDirection)).Length; i++) {

                        //설치 예정인 좌표 기준 네방향
                        Vector2 c_Vec = Calc_Vector((RoadDirection)i, m_nextXY);

                        //현재블럭이 있다면,
                        if (c_Vec == new Vector2(_x, _y)) {

                            //해당 Dir기준으로 리스트에 넣기
                            switch ((RoadDirection)i) {

                                case RoadDirection.Left:
                                    m_RoadBtn_List.Add(Road_Horizon_1_Btn);
                                    m_RoadBtn_List.Add(Road_Horizon_2_Btn);
                                    m_RoadBtn_List.Add(Road_Corner_2_Btn);
                                    m_RoadBtn_List.Add(Road_Corner_4_Btn);
                                    break;

                                case RoadDirection.Right:
                                    m_RoadBtn_List.Add(Road_Horizon_1_Btn);
                                    m_RoadBtn_List.Add(Road_Horizon_2_Btn);
                                    m_RoadBtn_List.Add(Road_Corner_1_Btn);
                                    m_RoadBtn_List.Add(Road_Corner_3_Btn);
                                    break;

                                case RoadDirection.Up:
                                    m_RoadBtn_List.Add(Road_Vertical_1_Btn);
                                    m_RoadBtn_List.Add(Road_Vertical_2_Btn);
                                    m_RoadBtn_List.Add(Road_Corner_3_Btn);
                                    m_RoadBtn_List.Add(Road_Corner_4_Btn);
                                    break;

                                case RoadDirection.Down:
                                    m_RoadBtn_List.Add(Road_Vertical_1_Btn);
                                    m_RoadBtn_List.Add(Road_Vertical_2_Btn);
                                    m_RoadBtn_List.Add(Road_Corner_1_Btn);
                                    m_RoadBtn_List.Add(Road_Corner_2_Btn);
                                    break;

                                default:
                                    break;
                            }
                            break;
                        }
                    }

                    //활성화된 목록에서 설치 불가능 구역과 이어지는 블럭들 제거
                    for (int i = 1; i < Enum.GetValues(typeof(RoadDirection)).Length; i++) {

                        //설치 예정인 좌표 기준 네방향
                        Vector2 c_Vec = Calc_Vector((RoadDirection)i, m_nextXY);

                        //설치불가능한 장소이고, 이전좌표 아닌경우
                        if (m_Map[(int)c_Vec.x, (int)c_Vec.y].TileType != TileType.None && c_Vec != new Vector2(_x, _y)) {

                            if (m_Map[(int)c_Vec.x, (int)c_Vec.y].TileType == TileType.Forbidden && c_Vec.x >= 20) {
                                break;
                            }

                            //해당 방향 목록에서 제거
                                switch ((RoadDirection)i) {

                                case RoadDirection.Left:
                                    if (m_RoadBtn_List.Contains(Road_Corner_2_Btn) == true)
                                        m_RoadBtn_List.Remove(Road_Corner_2_Btn);

                                    if (m_RoadBtn_List.Contains(Road_Corner_4_Btn) == true)
                                        m_RoadBtn_List.Remove(Road_Corner_4_Btn);

                                    m_RoadBtn_List.Remove(Road_Horizon_1_Btn);
                                    m_RoadBtn_List.Remove(Road_Horizon_2_Btn);

                                    break;

                                case RoadDirection.Right:
                                    if (m_RoadBtn_List.Contains(Road_Corner_1_Btn) == true)
                                        m_RoadBtn_List.Remove(Road_Corner_1_Btn);

                                    if (m_RoadBtn_List.Contains(Road_Corner_3_Btn) == true)
                                        m_RoadBtn_List.Remove(Road_Corner_3_Btn);

                                    m_RoadBtn_List.Remove(Road_Horizon_1_Btn);
                                    m_RoadBtn_List.Remove(Road_Horizon_2_Btn);

                                    break;

                                case RoadDirection.Up:
                                    if (m_RoadBtn_List.Contains(Road_Corner_3_Btn) == true)
                                        m_RoadBtn_List.Remove(Road_Corner_3_Btn);

                                    if (m_RoadBtn_List.Contains(Road_Corner_4_Btn) == true)
                                        m_RoadBtn_List.Remove(Road_Corner_4_Btn);

                                    m_RoadBtn_List.Remove(Road_Vertical_1_Btn);
                                    m_RoadBtn_List.Remove(Road_Vertical_2_Btn);

                                    break;

                                case RoadDirection.Down:
                                    if (m_RoadBtn_List.Contains(Road_Corner_1_Btn) == true)
                                        m_RoadBtn_List.Remove(Road_Corner_1_Btn);

                                    if (m_RoadBtn_List.Contains(Road_Corner_2_Btn) == true)
                                        m_RoadBtn_List.Remove(Road_Corner_2_Btn);

                                    m_RoadBtn_List.Remove(Road_Vertical_1_Btn);
                                    m_RoadBtn_List.Remove(Road_Vertical_2_Btn);

                                    break;

                                default:
                                    break;
                            }//switch
                        }//if (m_Map[(int)c_Vec.x, (int)c_Vec.y].TileType != TileType.None && c_Vec != new Vector2(x, y)) {

                    }

                }//if (m_EditStep == EditStep.DrawRoad)

              
                if(_x == 18 && _y == 10)
                    if (m_RoadBtn_List.Contains(Road_Corner_4_Btn) == true)
                        m_RoadBtn_List.Remove(Road_Corner_4_Btn);

                if(_x == 18 && _y == 1)
                    if (m_RoadBtn_List.Contains(Road_Corner_2_Btn) == true)
                        m_RoadBtn_List.Remove(Road_Corner_2_Btn);

                RoadBtn_ListActive(m_RoadBtn_List);

                //설치한 좌표 저장
                m_prevXY.x = _x;
                m_prevXY.y = _y;


                RoadType = RoadtoNumber(RoadObj);


                //작업내용 저장 //Undo Redo 기능
                History_save(m_Map[_x, _y], RoadObj, new Vector2(_x, _y), m_RoadBtn_List, m_prevXY, RoadType);
            }
        }//Step = DrawRoad
        //엔드 지점 설치 단계
        else if (m_MapMakeStep == MapMakeStep.SetEnd) {
            History_Reset();

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Install_Sound, 1.0f);

            GameObject RoadObj = Instantiate(m_SelectRoad_Obj, new Vector3(_x, m_RoadGroup.transform.position.y, _y), m_SelectRoad_Obj.transform.rotation);
            RoadObj.transform.parent = m_RoadGroup.transform;
            RoadObj.name = "End";


            m_Map[_x, _y].TileType = TileType.End;
            m_Map[_x, _y].TileDir[0] = m_SelectRoad_Dir[0];
            m_Map[_x, _y].TileDir[1] = m_SelectRoad_Dir[1];


            Reset_PossibleCell();

          
            m_RoadBtn_List.Clear();
            RoadBtn_ListActive(m_RoadBtn_List);


            //설치한 좌표 저장
            m_prevXY.x = _x;
            m_prevXY.y = _y;


            RoadType = RoadtoNumber(RoadObj);

            //작업내용 저장 //Undo Redo 기능
            History_save(m_Map[_x, _y], RoadObj, new Vector2(_x, _y), m_RoadBtn_List, m_prevXY, RoadType);

            m_MapMakeStep = MapMakeStep.Finish;

            //맵 데이터 저장
            MapDataSave();

            m_EditStep++;

            StepBar_Start(StepBar_TowerSetting_Btn);
            StepBar_MapEdit_Btn.GetComponent<Image>().color = Color_StepBar_Yellow;


        }
    }






    //undo redo 기능
    void History_save (Cell _cell, GameObject _obj, Vector2 _pos, List<Button> _list, Vector2  _prev, int _roadType) {
        //작업내용 저장 //Undo Redo 기능
        History History_unit = new History();
        History_unit.cell = _cell.DeepCopy();               //셀 깊은복사
        History_unit.cell.GuideCell = _cell.GuideCell;      //셀오브젝트 주소복사
        History_unit.roadObj = _obj;                        //오브젝트
        History_unit.pos = _pos;                            //좌표

        History_unit.btnList = _list.ToList();              //버튼목록 깊은복사
        History_unit.prevXY = _prev;                        //이전 설치좌표
        History_unit.roadType = _roadType;                  //테스트 단계에서 씬이동시 오브젝트 못불러오는 문제 해결을 위한 버튼 저장용 int


        m_Undo_Stack.Push(History_unit);

    }

    //undo
    void History_Undo () {

        if (m_Undo_Stack.Count <= 0)
            return;

        SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_1_Sound, 1.0f);

        //스택에 있는 게임오브젝트를 History 오브젝트 하위 UndoStack으로 이동
        History undo_history = m_Undo_Stack.Pop();

        History temp_history = new History();
        temp_history.cell = undo_history.cell.DeepCopy();
        temp_history.cell.GuideCell = undo_history.cell.GuideCell;
        temp_history.roadObj = undo_history.roadObj;
        temp_history.pos = undo_history.pos;
        temp_history.btnList = undo_history.btnList.ToList();
        temp_history.prevXY = undo_history.prevXY;
        temp_history.roadType = undo_history.roadType;

        m_Redo_Stack.Push(temp_history);

        undo_history.roadObj.transform.parent = Stack_Obj.transform;

        //설치가능지역 제거
        Reset_PossibleCell();

        //설치가능지역 = 스택에서 뽑아낸 pos값으로 설정
        Set_PossibleCell(undo_history.pos);

        //변수값을 스택내 값으로 변경 & 기존 값 초기화
        m_Map[(int)undo_history.pos.x, (int)undo_history.pos.y].TileDir[0] = RoadDirection.None;
        m_Map[(int)undo_history.pos.x, (int)undo_history.pos.y].TileDir[1] = RoadDirection.None;
        m_Map[(int)undo_history.pos.x, (int)undo_history.pos.y].TileType = TileType.None;

        m_RoadBtn_List.Clear();

        if (undo_history.roadObj.name == "Road") {
            m_MapMakeStep = MapMakeStep.DrawRoad;
            m_RoadBtn_List = m_Undo_Stack.Peek().btnList.ToList();
        }
        else if (undo_history.roadObj.name == "Start") {
            Set_StartPoint();
        }
        else if (undo_history.roadObj.name == "End") {
            //맵 에디트 --;
            m_EditStep = EditStap.MapEdit;
            //스탭바 
            Blink_Stop();
            StepBar_TowerSetting_Btn.GetComponent<Image>().color = Color_StepBar_Disable;
            StepBar_Prev.GetComponent<Image>().color = Color_StepBar_Yellow;

            m_MapMakeStep = MapMakeStep.SetEnd;
            undo_history.cell.TileType = TileType.Forbidden;

            m_RoadBtn_List = m_Undo_Stack.Peek().btnList.ToList();
        }

        if (m_Undo_Stack.Count > 0) {
            m_prevXY.x = m_Undo_Stack.Peek().prevXY.x;
            m_prevXY.y = m_Undo_Stack.Peek().prevXY.y;
        }
        else {
            m_prevXY.x = 0;
            m_prevXY.y = 0;
        }

        //타일 버튼 활성화
        RoadBtn_ListActive(m_RoadBtn_List);

        //undo 스택에 값이 없다면, Undo 버튼 비활성화
        if (m_Undo_Stack.Count <= 0)
            Undo_Btn.image.color = Color_RoadDisabled;

        //Redo 버튼 활성화 상태가 아니라면, Redo 버튼 활성화
        if (Redo_Btn.image.color != Color_RoadActive)
            Redo_Btn.image.color = Color_RoadActive;       
    }

    //redo
    void History_Redo () {

        if (m_Redo_Stack.Count <= 0)
            return;

        SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_2_Sound, 1.0f);

        //스택에서 뽑기
        History redo_history = m_Redo_Stack.Pop();

        //게임오브젝트 원래 위치로 돌려놓기
        redo_history.roadObj.transform.parent = m_RoadGroup.transform;

        Reset_PossibleCell();

        //각종 값들 되돌리기
        m_Map[(int)redo_history.pos.x, (int)redo_history.pos.y].TileDir[0] = redo_history.cell.TileDir[0];
        m_Map[(int)redo_history.pos.x, (int)redo_history.pos.y].TileDir[1] = redo_history.cell.TileDir[1];

        m_Map[(int)redo_history.pos.x, (int)redo_history.pos.y].TileType = redo_history.cell.TileType;

        //설치가능지역 설정
        for (int i = 0; i < 2; i++) {
            Vector2 possibleVector = Calc_Vector(m_Map[(int)redo_history.pos.x, (int)redo_history.pos.y].TileDir[i], redo_history.pos);

            if ((possibleVector.x > 0 && possibleVector.y > 0 && possibleVector.x < 21 && possibleVector.y < 12) 
                && (m_Map[(int)possibleVector.x, (int)possibleVector.y].TileType == TileType.None || 
                m_Map[(int)possibleVector.x, (int)possibleVector.y].TileType == TileType.Forbidden && possibleVector.x >= 20)) {

                //설치가능 지역 설정
                Set_PossibleCell(possibleVector);

                m_nextXY = possibleVector;
            }
        }

        //EditStep
        if (redo_history.roadObj.name == "Start") {
            m_MapMakeStep = MapMakeStep.DrawRoad;

            //Undo 버튼 활성화
            Undo_Btn.image.color = Color_RoadActive;
        }
        else if (redo_history.roadObj.name == "End") {
            m_MapMakeStep = MapMakeStep.Finish;

            //에디트 스탭 ++
            m_EditStep++;

            //스탭바 블링크
            m_Blink_Target = StepBar_TowerSetting_Btn.gameObject;

            //시작색 설정 및 타겟 색상 초기화
            m_Blink_Target.GetComponent<Image>().color = Color_StepBar_Gray;
            m_Blink_Start_Color = Color_StepBar_Gray;
            //끝색 설정
            m_Blink_End_Color = Color_StepBar_White;
            //블링크 스위치 온
            m_BlinkSwitch_flag = true;
        }

        //m_prevXY
        m_prevXY.x = redo_history.prevXY.x;
        m_prevXY.y = redo_history.prevXY.y;

        //m_nextXY
        m_nextXY = redo_history.nextXY;

        //버튼리스트
        m_RoadBtn_List.Clear();
        m_RoadBtn_List = redo_history.btnList.ToList();
        RoadBtn_ListActive(m_RoadBtn_List);

        //Undo스택에 다시 추가
        History_save(m_Map[(int)redo_history.pos.x, (int)redo_history.pos.y],
                    redo_history.roadObj,
                    redo_history.pos,
                    redo_history.btnList,
                    redo_history.prevXY,
                    redo_history.roadType);

        //Redo 스택에 값이 없다면, Redo 비활성화
        if (m_Redo_Stack.Count <= 0)
            Redo_Btn.image.color = Color_RoadDisabled;

    }

    //redo undo 스택 초기화
    void History_Reset(){
        /*
        Redo 버튼이 활성화 된 상태에서 오브젝트 설치시
        Redo 리스트 & StackObj 하위 오브젝트 날리기
        */

        if(Redo_Btn.image.color == Color_RoadActive)
        {
            m_Redo_Stack.Clear();

            //Stack_Obj 하위 오브젝트 삭제
            Transform[] child = Stack_Obj.GetComponentsInChildren<Transform>();
            if(child != null)
                for(int i = 1; i< child.Length; i++){
                    if (child[i] != Stack_Obj.transform)
                        Destroy(child[i].gameObject);
                }
            Redo_Btn.image.color = Color_RoadDisabled;
        }
    }






    //타일 버튼 활성화 메소드
    void RoadBtn_ListActive (List<Button> _list) {

        Road_Vertical_1_Btn.gameObject.GetComponent<Image>().raycastTarget = false;
        Road_Vertical_1_Btn.gameObject.GetComponentInChildren<Image>().color = Color_RoadDisabled;
        Road_Vertical_2_Btn.gameObject.GetComponent<Image>().raycastTarget = false;
        Road_Vertical_2_Btn.gameObject.GetComponentInChildren<Image>().color = Color_RoadDisabled;
        Road_Horizon_1_Btn.gameObject.GetComponent<Image>().raycastTarget = false;
        Road_Horizon_1_Btn.gameObject.GetComponentInChildren<Image>().color = Color_RoadDisabled;
        Road_Horizon_2_Btn.gameObject.GetComponent<Image>().raycastTarget = false;
        Road_Horizon_2_Btn.gameObject.GetComponentInChildren<Image>().color = Color_RoadDisabled;
        Road_Corner_1_Btn.gameObject.GetComponent<Image>().raycastTarget = false;
        Road_Corner_1_Btn.gameObject.GetComponentInChildren<Image>().color = Color_RoadDisabled;
        Road_Corner_2_Btn.gameObject.GetComponent<Image>().raycastTarget = false;
        Road_Corner_2_Btn.gameObject.GetComponentInChildren<Image>().color = Color_RoadDisabled;
        Road_Corner_3_Btn.gameObject.GetComponent<Image>().raycastTarget = false;
        Road_Corner_3_Btn.gameObject.GetComponentInChildren<Image>().color = Color_RoadDisabled;
        Road_Corner_4_Btn.gameObject.GetComponent<Image>().raycastTarget = false;
        Road_Corner_4_Btn.gameObject.GetComponentInChildren<Image>().color = Color_RoadDisabled;

        //Select 비활성화
        SelectUI_Obj.SetActive(false);

        if (_list.Count > 0) {
            //select 비활성화라면, 활성화
            if (SelectUI_Obj.activeSelf == false)
                SelectUI_Obj.SetActive(true);

            //리스트에 존재하는 버튼만 활성화
            for (int i = 0; i < _list.Count; i++) {
                _list[i].GetComponent<Image>().raycastTarget = true;
                _list[i].GetComponentInChildren<Image>().color = Color_RoadActive;

                //선택상태 가장 첫번째 버튼으로 설정
                if (i == 0) {

                    m_soundPlay = false;
                    _list[i].onClick.Invoke();
                }
            }
        }

    }

    //설치 가능지역 설정 메소드
    void Set_PossibleCell (Vector2 _pos) {

        if (_pos.x >= 0 && _pos.y >= 0) {

            m_Map[(int)_pos.x, (int)_pos.y].SetPossible = true;
            m_Map[(int)_pos.x, (int)_pos.y].GuideCell.GetComponent<SpriteRenderer>().color = Color_Green;
            m_Map[(int)_pos.x, (int)_pos.y].GuideCell.SetActive(true);
        }
    }

    //설치 가능지역 초기화 메소드
    void Reset_PossibleCell () {
        for (int x = 0; x < 21; x++) { //x
            for (int y = 0; y < 12; y++) {  //y

                m_Map[x, y].SetPossible = false;
                m_Map[x, y].GuideCell.GetComponent<SpriteRenderer>().color = Color_Alpha;
                m_Map[x, y].GuideCell.SetActive(false);

                //설치 금지 구역
                if (x == 0 || x == 20 || y == 11 || y == 0) {
                    m_Map[x, y].TileType = TileType.Forbidden;
                }
            }
        }
    }


    void RoadBtnClick(Button _roadBtn, GameObject _tileObj, RoadDirection[] _roadDir)
    {
        if (SelectUI_Obj.activeSelf == true)
        {
            if(m_soundPlay == true)
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Select_Sound, 1.0f);

            SelectUI_Obj.transform.SetParent(_roadBtn.gameObject.transform, false);
            SelectUI_Obj.transform.position = _roadBtn.transform.position;
            m_SelectRoad_Obj = _tileObj;
            m_SelectRoad_Dir = _roadDir;

            if (m_soundPlay == false)
                m_soundPlay = true;
        }

    }



    void StepBar_Blink(){

        if (m_Blink_Target == null)
            return;

        if (m_BlinkSwitch_flag == true)
            m_Blink_Target.GetComponent<Image>().color = Color.Lerp(Color_StepBar_Gray, Color_StepBar_White, m_BlinkTime);
        else
            m_Blink_Target.GetComponent<Image>().color = Color.Lerp(Color_StepBar_White, Color_StepBar_Gray, m_BlinkTime);

        m_BlinkTime += Time.deltaTime * m_BlinkSpeed;

        if ((m_BlinkSwitch_flag == true && m_Blink_Target.GetComponent<Image>().color == Color_StepBar_White) || 
            (m_BlinkSwitch_flag == false && m_Blink_Target.GetComponent<Image>().color == Color_StepBar_Gray)) {
            m_BlinkTime = 0.0f;
            m_BlinkSwitch_flag = !m_BlinkSwitch_flag;
        }
    }
    
    void StepBar_Start(Button _target)
    {
        SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Alarm_Sound, 1.0f);


        if (StepBar_Prev != null && StepBar_Prev.GetComponent<Image>().color == Color_StepBar_Yellow)
        {
            StepBar_Prev.GetComponent<Image>().color = Color_StepBar_Gray;
        }


        m_Blink_Target = _target.gameObject;
        //시작색 설정 및 타겟 색상 초기화
        m_Blink_Target.GetComponent<Image>().color = Color_StepBar_Gray;
        //블링크 스위치 온
        m_BlinkSwitch_flag = true;
    }

    void Blink_Stop()
    {
        if (m_Blink_Target == null)
            return;

        m_Blink_Target.GetComponent<Image>().color = Color_StepBar_Gray;
        m_Blink_Target = null;
    }



    void StepBar_Click (Button _btn) {

       
        //모든 창 다 꺼지게
        TowerSetting_Panel.SetActive(false);
        MonsterSetting_Panel.SetActive(false);
        RoundSetting_Panel.SetActive(false);
        Test_Panel.SetActive(false);
        Upload_Panel.SetActive(false);


        //해당 패널만 켜지게
        if(_btn == StepBar_MapEdit_Btn){

            //비활성화

            if (StepBar_TowerSetting_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_TowerSetting_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

            if (StepBar_MonsterSetting_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_MonsterSetting_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

            if (StepBar_RoundSetting_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_RoundSetting_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

            if (StepBar_Test_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_Test_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

            if (StepBar_Upload_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_Upload_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

        }
        else if (_btn == StepBar_TowerSetting_Btn){
            TowerSetting_Panel.SetActive(true);

            //활성화
            if (StepBar_MapEdit_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_MapEdit_Btn.GetComponent<Image>().color = Color_StepBar_Gray;


            //비활성화
            if (StepBar_MonsterSetting_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_MonsterSetting_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

            if(StepBar_RoundSetting_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_RoundSetting_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

            if (StepBar_Test_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_Test_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

            if (StepBar_Upload_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_Upload_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

        }
        else if(_btn == StepBar_MonsterSetting_Btn){
            MonsterSetting_Panel.SetActive(true);

            //활성화
            if (StepBar_MapEdit_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_MapEdit_Btn.GetComponent<Image>().color = Color_StepBar_Gray;

            if (StepBar_TowerSetting_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_TowerSetting_Btn.GetComponent<Image>().color = Color_StepBar_Gray;
            
            //비활성화
            if (StepBar_RoundSetting_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_RoundSetting_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

            if (StepBar_Test_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_Test_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

            if (StepBar_Upload_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_Upload_Btn.GetComponent<Image>().color = Color_StepBar_Disable;
        }
        else if (_btn == StepBar_RoundSetting_Btn){
            RoundSetting_Panel.SetActive(true);

            //활성화
            if (StepBar_MapEdit_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_MapEdit_Btn.GetComponent<Image>().color = Color_StepBar_Gray;

            if (StepBar_TowerSetting_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_TowerSetting_Btn.GetComponent<Image>().color = Color_StepBar_Gray;

            if (StepBar_MonsterSetting_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_MonsterSetting_Btn.GetComponent<Image>().color = Color_StepBar_Gray;

            //비활성화
            if (StepBar_Test_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_Test_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

            if (StepBar_Upload_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_Upload_Btn.GetComponent<Image>().color = Color_StepBar_Disable;

        }
        else if(_btn == StepBar_Test_Btn){
            Test_Panel.SetActive(true);

            //활성화
            if (StepBar_MapEdit_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_MapEdit_Btn.GetComponent<Image>().color = Color_StepBar_Gray;

            if (StepBar_TowerSetting_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_TowerSetting_Btn.GetComponent<Image>().color = Color_StepBar_Gray;

            if (StepBar_MonsterSetting_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_MonsterSetting_Btn.GetComponent<Image>().color = Color_StepBar_Gray;

            if (StepBar_RoundSetting_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_RoundSetting_Btn.GetComponent<Image>().color = Color_StepBar_Gray;
            
            //비활성화
            if (StepBar_Upload_Btn.GetComponent<Image>().color != Color_StepBar_Disable)
                StepBar_Upload_Btn.GetComponent<Image>().color = Color_StepBar_Disable;


        }
        else if(_btn == StepBar_Upload_Btn){
            Upload_Panel.SetActive(true);

            //활성화
            if (StepBar_MapEdit_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_MapEdit_Btn.GetComponent<Image>().color = Color_StepBar_Gray;

            if (StepBar_TowerSetting_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_TowerSetting_Btn.GetComponent<Image>().color = Color_StepBar_Gray;

            if (StepBar_MonsterSetting_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_MonsterSetting_Btn.GetComponent<Image>().color = Color_StepBar_Gray;

            if (StepBar_RoundSetting_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_RoundSetting_Btn.GetComponent<Image>().color = Color_StepBar_Gray;

            if (StepBar_Test_Btn.GetComponent<Image>().color != Color_StepBar_Gray)
                StepBar_Test_Btn.GetComponent<Image>().color = Color_StepBar_Gray;
        }


        //블링크 스탑
        Blink_Stop();

        _btn.GetComponent<Image>().color = Color_StepBar_Yellow;
        StepBar_Prev = _btn;
    }




    void MapDataSave()
    {

        //사이드 -1 처리
        for (int i = 0; i < 21; i++)
            for (int j = 0; j < 12; j++)
                if (i == 0 || j == 0 || i == 20 || j == 11)
                    temp_Map_Data[i, j] = -1;

        //타일 좌표값으로 배열 좌표로 변환

        //map/RoadGroup 하위 오브젝트들 가져오기
        Transform[] RoadArry = m_RoadGroup.GetComponentsInChildren<Transform>();


        for (int i = 1; i < RoadArry.Length; i++)
        {
            //위치 기준으로 배열 좌표 변환
            int tempRoadPos_X = (int)RoadArry[i].transform.position.x;
            int tempRoadPos_Y = (int)RoadArry[i].transform.position.z;

            //오브젝트의 스프라이트 기준으로 값 할당 
            temp_Map_Data[tempRoadPos_X, tempRoadPos_Y] = RoadtoNumber(RoadArry[i].gameObject);
        }

        Debug.Log("맵 데이터 저장완료 ");
    }


    //오브젝트의 스프라이트 기준으로 값 할당 
    int RoadtoNumber(GameObject _obj){

        //값에 대한 정보는 MapMgr.cs Line:285 참고
        //if (testObj.GetComponent<SpriteRenderer>().sprite == RoadTile_CenterTop.GetComponent<SpriteRenderer>().sprite)

        if (_obj.GetComponent<SpriteRenderer>().sprite == RoadTile_LeftTop.GetComponent<SpriteRenderer>().sprite)
            return 1;

        else if (_obj.GetComponent<SpriteRenderer>().sprite == RoadTile_CenterTop.GetComponent<SpriteRenderer>().sprite)
            return 2;

        else if (_obj.GetComponent<SpriteRenderer>().sprite == RoadTile_RightTop.GetComponent<SpriteRenderer>().sprite)
            return 3;

        else if (_obj.GetComponent<SpriteRenderer>().sprite == RoadTile_LeftMiddle.GetComponent<SpriteRenderer>().sprite)
            return 4;

        else if (_obj.GetComponent<SpriteRenderer>().sprite == RoadTile_RightMiddle.GetComponent<SpriteRenderer>().sprite)
            return 5;

        else if (_obj.GetComponent<SpriteRenderer>().sprite == RoadTile_LeftBottom.GetComponent<SpriteRenderer>().sprite)
            return 6;

        else if (_obj.GetComponent<SpriteRenderer>().sprite == RoadTile_CenterBottom.GetComponent<SpriteRenderer>().sprite)
            return 7;

        else if (_obj.GetComponent<SpriteRenderer>().sprite == RoadTile_RightBottom.GetComponent<SpriteRenderer>().sprite)
            return 8;

        else
            return 0;
    }



    Button NumbertoRoad(int _num)
    {
        Button _btn;

        switch (_num)
        {
            case 1:
                _btn = Road_Corner_1_Btn;
                break;

            case 2:
                _btn = Road_Horizon_1_Btn;
                break;

            case 3:
                _btn = Road_Corner_2_Btn;
                break;

            case 4:
                _btn = Road_Vertical_1_Btn;
                break;

            case 5:
                _btn = Road_Vertical_2_Btn;
                break;

            case 6:
                _btn = Road_Corner_3_Btn;
                break;

            case 7:
                _btn = Road_Horizon_2_Btn;
                break;

            case 8:
                _btn = Road_Corner_4_Btn;
                break;

            default:
                _btn = Road_Corner_2_Btn;
                break;
        }

        return _btn;
    }



    public void GameData_Load ()
    {
        if (Resources.Load<TextAsset>(path) != null)
        {
            var load_json = Resources.Load<TextAsset>(path);
            
            GameData_Format GD_Load = JsonUtility.FromJson<GameData_Format>(load_json.ToString());

            //타워리스트
            local_Towers_Data = JsonUtility.FromJson<TowerList>(GD_Load._tower_Setting);
            //몬스터 스폰 데이터
            local_MonSpawn_Data = LobbyMgr.Inst.StringToArray(GD_Load._monSpawn_Data);
            //라운드 데이터
            local_Round_Data = JsonHelper.FromJson<RoundSetting>(GD_Load._round_Setting);
            //몬스터 셋팅
            local_MonSet_Data = JsonHelper.FromJson<MonSet>(GD_Load._monster_Setting);
            //게임 셋팅
            local_GameSetting_Data= JsonUtility.FromJson<GameSetting>(GD_Load._game_Setting);


            Debug.Log("게임 데이터 로드 완료");
        }
        else{
            
            GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "파일이 존재하지 않습니다.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");

            });
        }
    }
}














