using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using SimpleJSON;
using UnityEngine.EventSystems;
using System.IO;

public class LobbyMgr : MonoBehaviour
{
    public static LobbyMgr Inst = null;
    public Text m_NickName_Text;
    public Button m_Profile_Btn;
    public Button m_Setting_Btn;

    public Button m_StandardGame_Btn;
    public Button m_CustomGame_Btn;



    [Header("ProfilePanel")]
    public GameObject m_ProfilePanel;
    public Button m_ProfileClose_Btn;
    public Text m_Profile_UserId_Text;
    public Text m_Profile_NickName_Text;
    public Text m_Profile_TopRound_Text;
    public Text m_Profile_Heart_Text;
    public Text m_Profile_ClearTime_Text;
    public Text m_Profile_ClearDate_Text;


    [Header("CustomGamePanel")]
    public GameObject m_CustomPanel;
    public Button m_CustomClose_Btn;

    public Button m_LikeSort_Btn;
    public Button m_PlaySort_Btn;
    public Button m_RecentSort_Btn;

    public Dropdown m_SearchOption_DL;
    public InputField m_Search_Input;

    public Button m_Search_Btn;
    //public Button m_Refresh_Btn;

    public GameObject m_ScrollViewContent;
    GameObject Node;

    public Button m_CreateCustomGame_Btn;



    [Header("Setting")]
    GameObject SettingPanel_Obj;
    GameObject InfoMessage;




    //------사운드
    //SoundMgr m_SoundMgr;



    //------사운드




    bool Like_flag = true;
    bool Play_flag = false;
    bool Recent_flag = false;

    string ORDERBY_OPTION = "";
    string WHERE_OPTION = "";






    string SELECT_CustomGameList_URL = "http://devwhale.dothome.co.kr/ARD_Test/SELECT_GameList.php";
    string SELECT_GAME_URL = "http://devwhale.dothome.co.kr/ARD_Test/SELECT_GAME.php";
    string SELECT_UserInfo_Url = "http://devwhale.dothome.co.kr/ARD_Test/UserInfo_Select.php";
    string g_Message = "";





    void ObjectLoad()
    {

        if(GameObject.Find("SoundGroup") != null){
            //m_SoundMgr = GameObject.Find("SoundGroup").GetComponent<SoundMgr>();

        }
        else{
            UnityEngine.SceneManagement.SceneManager.LoadScene("Login");

        }


    }


    private void Awake()
    {
        ObjectLoad();
    }


    // Start is called before the first frame update
    void Start ()
    {
        Inst = this;

        Node = Resources.Load<GameObject>("Prefab/UI/Node");
        InfoMessage = Resources.Load<GameObject>("Prefab/UI/InfoMessage");


        UserDataLoad();


        BtnClick_Collection();


        SettingPanel_Obj = GameObject.Find("Canvas").transform.Find("SettingPanel").gameObject;



        SettingPanel_Obj.SetActive(false);
        SettingPanel_Obj.transform.Find("Lobby_Btn").gameObject.SetActive(false);




        if (SoundMgr.g_inst.Background_Audio.clip != SoundMgr.g_inst.BGMTitle_Sound) {
            SoundMgr.g_inst.RandomStop();

            SoundMgr.g_inst.Background_Audio.clip = SoundMgr.g_inst.BGMTitle_Sound;
            SoundMgr.g_inst.Background_Audio.playOnAwake = true;    //씬 시작시 시작
            SoundMgr.g_inst.Background_Audio.loop = true; //반복 재생
            SoundMgr.g_inst.Background_Audio.Play();
        }





    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Touch_Sound, 1.0f);

        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(DL_ValueChange(m_SearchOption_DL.value));

        }



    }

    void UserDataLoad () {

        if(GlobalUserData.m_Uid_str == "" || GlobalUserData.m_Uid_str == null){

            UnityEngine.SceneManagement.SceneManager.LoadScene("Login");

            return;
        }



        StartCoroutine(UserInfo_Select());


        

    }


   



    //2차 배열 -> JSON 타입 변환
    public string ArrayToString (int[,] _arr) {
        string arrayString = "";


        for (int i = 0; i < _arr.GetLength(0); i++) {
            arrayString += "{";
            for (int j = 0; j < _arr.GetLength(1); j++) {
                arrayString += _arr[i, j].ToString();

                if (j < _arr.GetLength(1) - 1)
                    arrayString += ",";
            }
            arrayString += "}";

            if (i < _arr.GetLength(0) - 1)
                arrayString += "\n";
        }


        return arrayString;
    }


    //JSON -> 2차 배열 복원
    public int[,] StringToArray (string _str) {
        // { 개수 = i (21)
        // , 개수 나누기 i +1  = j
        MatchCollection matches = Regex.Matches(_str, "{");
        int i_cnt = matches.Count;
        //Debug.Log(i_cnt);
        matches = Regex.Matches(_str, ",");
        int j_cnt = matches.Count;
        j_cnt = (j_cnt / i_cnt) + 1;

        int[,] arr = new int[i_cnt, j_cnt];


        string[] row_splitStr = { "{", "}", "\n" };
        string[] row = _str.Split(row_splitStr, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < i_cnt; i++) {
            string[] value = row[i].Split(',');
            for (int j = 0; j < j_cnt; j++) {
                arr[i, j] = int.Parse(value[j]);
            }
        }

        return arr;
    }


    bool EqualCompare (int[,] _arr1, int[,] _arr2) {
        int cnt = 0;

        for (int i = 0; i < _arr1.GetLength(0); i++) {
            for (int j = 0; j < _arr1.GetLength(1); j++) {

                if (_arr1[i, j] == _arr2[i, j]) {
                    cnt++;
                }

            }
        }


        if (_arr1.GetLength(0) * _arr1.GetLength(1) == cnt) {
            return true;
        }
        else {
            return false;
        }

    }


    void BtnClick_Collection () {


        //프로필 버튼
        m_Profile_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.OpenWindow_Sound, 1.0f);

            m_ProfilePanel.SetActive(true);
        });

        //프로필 닫기
        m_ProfileClose_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.CloseWindow_Sound, 1.0f);

            m_ProfilePanel.SetActive(false);
        });

        //설정
        m_Setting_Btn.onClick.AddListener(() => {

            if (SettingPanel_Obj.activeSelf == true)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.OpenWindow_Sound, 1.0f);

            SettingPanel_Obj.SetActive(true);


        });

        //일반 게임
        m_StandardGame_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.SceneChange_Sound, 1.0f);

            GlobalGameData.m_GameMode = GameMode.Standard;




            //GameMgr에서 로컬 파일로 가져옴
            UnityEngine.SceneManagement.SceneManager.LoadScene("InGame");


        });

        //커스텀 게임 판넬 켜기
        m_CustomGame_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.OpenWindow_Sound, 1.0f);

            m_LikeSort_Btn.gameObject.GetComponent<Image>().color = Color.gray;
            ORDERBY_OPTION = "like_count";

            m_CustomPanel.SetActive(true);
            StartCoroutine(SELECT_CustomGameList());

        });

        //커스텀 게임 판넬 끄기
        m_CustomClose_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.CloseWindow_Sound, 1.0f);

            //정렬 버튼 초기화
            Like_flag = true;
            Play_flag = false;
            Recent_flag = false;

            SortBtnClicked();


            //드롭다운 리스트 초기화


            //인풋 초기화
            m_Search_Input.text = "";

            m_CustomPanel.SetActive(false);
        });

        //
        

        //좋아요 정렬
        m_LikeSort_Btn.onClick.AddListener(() => {

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);


            Like_flag = true;
            Play_flag = !Like_flag;
            Recent_flag = !Like_flag;
            ORDERBY_OPTION = "like_count";
            StartCoroutine(SELECT_CustomGameList());

            SortBtnClicked();
        });

        //플레이순 정렬
        m_PlaySort_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            Play_flag = true;
            Like_flag = !Play_flag;
            Recent_flag = !Play_flag;
            ORDERBY_OPTION = "play_count";
            StartCoroutine(SELECT_CustomGameList());

            SortBtnClicked();
        });

        //최신순 정렬
        m_RecentSort_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            Recent_flag = true;
            Like_flag = !Recent_flag;
            Play_flag = !Recent_flag;
            ORDERBY_OPTION = "make_date";
            StartCoroutine(SELECT_CustomGameList());

            SortBtnClicked();
        });


        //검색 버튼
        m_Search_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            StartCoroutine(SELECT_CustomGameList());

        });

        m_CreateCustomGame_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            SoundMgr.g_inst.Background_Audio.clip = SoundMgr.g_inst.BGMGoldBeach_Sound;
            SoundMgr.g_inst.Background_Audio.playOnAwake = true;    //씬 시작시 시작
            SoundMgr.g_inst.Background_Audio.loop = true; //반복 재생
            SoundMgr.g_inst.Background_Audio.Play();

            UnityEngine.SceneManagement.SceneManager.LoadScene("Editor");
        });



    }






    void SortBtnClicked () {


        if (Like_flag == true) {

            m_LikeSort_Btn.gameObject.GetComponent<Image>().color = Color.gray;
            m_PlaySort_Btn.gameObject.GetComponent<Image>().color = Color.white;
            m_RecentSort_Btn.gameObject.GetComponent<Image>().color = Color.white;
        }
        else if (Play_flag == true) {

            m_LikeSort_Btn.gameObject.GetComponent<Image>().color = Color.white;
            m_PlaySort_Btn.gameObject.GetComponent<Image>().color = Color.gray;
            m_RecentSort_Btn.gameObject.GetComponent<Image>().color = Color.white;
        }
        else {

            m_LikeSort_Btn.gameObject.GetComponent<Image>().color = Color.white;
            m_PlaySort_Btn.gameObject.GetComponent<Image>().color = Color.white;
            m_RecentSort_Btn.gameObject.GetComponent<Image>().color = Color.gray;
        }

    }

    string DL_ValueChange (int _num) {
        string _str = "";

        if (_num == 0)
            _str = "title";

        else if (_num == 1)
            _str = "game_id";

        else if (_num == 2)
            _str = "creator_nickname";

        return _str;
    }


    public void CustomGameStart(string _gameCode)
    {
        SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.SceneChange_Sound, 1.0f);

        StartCoroutine(SELECT_CustomGameLoad(_gameCode));

    }




    IEnumerator SELECT_CustomGameList()
    {

        //기존 노드 제거
        Transform[] childList = m_ScrollViewContent.GetComponentsInChildren<Transform>();

        if (1 < childList.Length && childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                Destroy(childList[i].gameObject);
            }
        }
        //기존 노드 제거



        WWWForm form = new WWWForm();

        form.AddField("ORDERBY_Option", ORDERBY_OPTION);
        form.AddField("WHERE_Option", DL_ValueChange(m_SearchOption_DL.value));
        form.AddField("WHERE_Keyword", m_Search_Input.text);


        WWW webRequest = new WWW(SELECT_CustomGameList_URL, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(webRequest.bytes);
        g_Message = sz;
        Debug.Log(sz);


        if (sz.Contains("Game Search-Success!!") == true)
        {

            if (sz.Contains("game_id") == true)
            {

                var N = JSON.Parse(sz);

                if (N != null)
                {

                    for (int i = 0; i < N.Count; i++)
                    {

                        //스크롤 뷰에 노드 생성
                        GameObject SV_Node = Instantiate(Node);
                        SV_Node.transform.SetParent(m_ScrollViewContent.transform);
                        SV_Node.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        SV_Node.name = "Node_" + i;
                        NodeScript SV_NodeScript = SV_Node.GetComponent<NodeScript>();

                        //타이틀
                        if (N[i]["title"] != null && N[i]["title"] != "")
                        {
                            SV_NodeScript.m_Title_Text.text = N[i]["title"];
                        }

                        //게임ID
                        if (N[i]["game_id"] != null && N[i]["game_id"] != "")
                        {
                            SV_NodeScript.m_GameCode_Text.text = N[i]["game_id"];
                        }

                        //제작자
                        if (N[i]["creator_nickname"] != null && N[i]["creator_nickname"] != "")
                        {
                            SV_NodeScript.m_CreatorNickName_Text.text = N[i]["creator_nickname"];
                        }

                        //플레이횟수
                        if (N[i]["play_count"] != null && N[i]["play_count"] != "")
                        {
                            SV_NodeScript.m_PlayCnt_Text.text = N[i]["play_count"];
                        }

                        //좋아요수
                        if (N[i]["like_count"] != null && N[i]["like_count"] != "")
                        {
                            SV_NodeScript.m_LikeCnt_Text.text = N[i]["like_count"];
                        }

                        //제작날짜
                        if (N[i]["make_date"] != null && N[i]["make_date"] != "")
                        {
                            SV_NodeScript.m_Date_Text.text = N[i]["make_date"];
                        }

                        //총 라운드
                        if (N[i]["round_data"] != null && N[i]["round_data"] != "")
                        {
                            int[,] array_int = StringToArray(N[i]["round_data"]);
                            SV_NodeScript.m_RoundCnt_Text.text = array_int.GetLength(0).ToString();
                        }
                    }


                    LayoutRebuilder.ForceRebuildLayoutImmediate(m_ScrollViewContent.GetComponent<RectTransform>());
                }
            }
        }//if (sz.Contains("Game Search-Success!!") == true)
    }




    //기본게임은 로컬저장으로 가져와서 이젠 쓸모 없음
    //추후 커스텀 게임 로드시 참고하여 작성

    IEnumerator SELECT_CustomGameLoad(string _gameCode) {


        WWWForm form = new WWWForm();
        form.AddField("game_id", _gameCode);
        WWW webRequest = new WWW(SELECT_GAME_URL, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(webRequest.bytes);
        g_Message = sz;
        Debug.Log("=====");

        Debug.Log(sz);


        //JSON.Parse(sz) 실행시 JsonUtility 및 JsonHelper로 변환한 JSON 경우엔 형식에 맞지 않아 이상한 해시값 및 형식이 깨져서 FromJson으로 복원하면 에러가 발생한다.
        //때문에 Class 형식을 JSON으로 변환한경우 아래와같이 PHP전송시 들어간 불필요한 문자열들이 제거된 값을 string타입의 변수에 분류해준다.
        string[] Split_Json = sz.Split(new string[] { "\"round_setting\":\"" }, System.StringSplitOptions.None);
        Split_Json = Split_Json[1].Split(new string[] { "\",\"monster_setting\":\"","\",\"tower_setting\":\"" ,"\",\"game_setting\":\"", "\"}\nSelect-Success!!"}, System.StringSplitOptions.None);
        string RoundSet_Json = Split_Json[0];
        string TowerList_Json = Split_Json[1];
        string MonSet_Json = Split_Json[2];
        string GameSet_Json = Split_Json[3];
        //----------------------------------------------------------------------------------------------------------------------------------------------------------

        if (sz.Contains("Select-Success!!") == true) {
            string temp = "";
            //text로 이름과 점수 출력해주고
            //JSON 파싱
            if (sz.Contains("game_id") == true) {

                var N = JSON.Parse(sz);
                if (N != null) {
                   
                    if(N["game_id"] != null){
                        GlobalGameData.m_GameCode_Str = N["game_id"];
                    }

                    if (N["map_data"] != null) {
                        temp = "";
                        temp = N["map_data"];

                        //int 2차배열 = 변환 메서드 사용
                        GlobalGameData.m_Map_Array = StringToArray(temp);
                    }

                    if (N["round_data"] != null) {
                        temp = "";
                        temp = N["round_data"];

                        //int 2차배열 = 변환 메서드 사용
                        GlobalGameData.m_MonSpawn_Array = StringToArray(temp);
                    }
                    
                    if (N["round_setting"] != null) {
                        //배열 = JsonHelper 사용
                        GlobalGameData.m_RoundSet_Array = JsonHelper.FromJson<RoundSetting>(RoundSet_Json);
                    }

                    if (N["game_setting"] != null){
                        //class 타입 = JsonUtility 사용
                        GlobalGameData.m_GameSetting = JsonUtility.FromJson<GameSetting>(GameSet_Json);
                    }

                    if (N["tower_setting"] != null){
                        //class 타입 = JsonUtility 사용
                        GlobalGameData.m_TowersData_TL = JsonUtility.FromJson<TowerList>(TowerList_Json);
                    }

                    if (N["monster_setting"] != null){
                        //배열 = JsonHelper 사용
                        GlobalGameData.m_MonSet_Array = JsonHelper.FromJson<MonSet>(MonSet_Json);
                    }
                    
                    
                  

                    Debug.Log("게임 데이터 로드 완료");
                }
            }//if (sz.Contains("nick_name") == true)


         

            if (GlobalGameData.m_Map_Array != null && GlobalGameData.m_MonSpawn_Array != null &&
                GlobalGameData.m_RoundSet_Array != null && GlobalGameData.m_MonSet_Array != null &&
                GlobalGameData.m_GameSetting != null && GlobalGameData.m_TowersData_TL !=  null) {
                GlobalGameData.m_GameMode = GameMode.Custom;
                UnityEngine.SceneManagement.SceneManager.LoadScene("InGame");
            }
            else {
                GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

                InfoWindow.GetComponent<InfoMessage>().info_Text.text = "정보가 올바르게 로드되지 않았습니다.";
                InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                    Destroy(InfoWindow);

                });
            }

        }//if (sz.Contains("Login-Success!!") == true)
        else {
            GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "등록된 번호가 없습니다.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                Destroy(InfoWindow);

            });
        }

    }



    IEnumerator UserInfo_Select()
    {
        //PHP로 MySql에서 inputField의 이름값으로 검색
        WWWForm form = new WWWForm();
        form.AddField("user_id", GlobalUserData.m_Uid_str, System.Text.Encoding.UTF8);
        form.AddField("game_id", 0);
        WWW webRequest = new WWW(SELECT_UserInfo_Url, form);
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


                    m_NickName_Text.text = GlobalUserData.m_NickName_str;
                    m_Profile_UserId_Text.text = GlobalUserData.m_Uid_str;
                    m_Profile_NickName_Text.text = GlobalUserData.m_NickName_str;
                    m_Profile_TopRound_Text.text = GlobalUserData.m_TopRound_int.ToString();
                    m_Profile_Heart_Text.text = GlobalUserData.m_ClearHeart_int.ToString();
                    m_Profile_ClearTime_Text.text = GlobalUserData.m_ClearTime_str;
                    m_Profile_ClearDate_Text.text = GlobalUserData.m_ClearDate_str;


                }
            }//if (sz.Contains("nick_name") == true)
        }//if (sz.Contains("Login-Success!!") == true)
        else
        {

            //데이터가 없다면,
            g_Message = "등록된 정보가 없습니다.";
        }

    }

    /*
    void OnGUI()
    {
        if (g_Message != "")
        {
            //GUILayout.Label("<color=White><size=25>" + g_Message + "</size></color>");
        }
    }
    */

}
