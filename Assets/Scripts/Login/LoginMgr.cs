using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using SimpleJSON;  //SimpleJSON을 사용하기 위해 네임스페이스를 추가

//------------이메일 형식이 맞는지 확인하는 방법 스크립트
using System.Globalization;
using System.Text.RegularExpressions;
using System;
using System.Text;
using UnityEngine.EventSystems;
//------------이메일 형식이 맞는지 확인하는 방법 스크립트



public class LoginMgr : MonoBehaviour
{
    public string g_Message = "";


    //--------Fade Out
    public Image m_FadePanel;
    private float AniDuring = 0.8f;  //페이드아웃 연출을 시간 설정
    private bool m_StartFade = false;
    private float m_CacTime = 0.0f;
    private float m_AddTimer = 0.0f;
    private Color m_Color;
    //--------Fade Out



    [Header("LoginPanel")]
    public GameObject m_LoginPanelObj;
    public InputField IDInputField;         //Email로 받을 것
    public InputField PassInputField;
    public Button m_LoginBtn = null;
    public Button m_CreateAccOpenBtn = null;

    public Button m_GoogleLoginBtn;
    public Button m_FacebookLoginBtn;
    public Button m_GuestLoginBtn;


    [Header("CreateAccountPanel")]
    public GameObject m_CreateAccPanelObj;
    public InputField New_IDInputField;     //Email로 받을 것
    public InputField New_PassInputField;
    public InputField New_NickInputField;
    public Button m_CreateAccountBtn = null;
    public Button m_CancelButton = null;

    //------이메일 형식이 맞는지 확인하는 변수
    private bool invalidEmailType = false;       // 이메일 포맷이 올바른지 체크
    private bool isValidFormat = false;          // 올   바른 형식인지 아닌지 체크
    //------이메일 형식이 맞는지 확인하는 변수


    string UpdateUrl = "http://devwhale.dothome.co.kr/ARD_Test/UserInfo_Update.php";
    string SelectUrl = "http://devwhale.dothome.co.kr/ARD_Test/UserInfo_Select.php";


    GameObject GlobalDataObj;

    GameObject InfoMessage;

    Text m_Touch_Text;
    bool Blink_flag = false;
    Color Color_Disabled = new Color32(255, 255, 255, 0);
    Color Color_Active = new Color32(255, 255, 255, 255);
    float Blink_time = 0.0f;
    float BlinkSpeed = 0.8f;

    int ClickCount = 0;

    //------사운드
    GameObject SoundGroup;
    //SoundMgr m_SoundMgr;



    //------사운드

    string randomNickName = "";

    void Awake () {

        Application.targetFrameRate = 60;

        InfoMessage = Resources.Load<GameObject>("Prefab/UI/InfoMessage");
        GlobalDataObj = GameObject.Find("GlobalDataObj").gameObject;
        SoundGroup = GameObject.Find("SoundGroup").gameObject;
        //m_SoundMgr = SoundGroup.GetComponent<SoundMgr>();


        m_Touch_Text = GameObject.Find("Touch_Text").GetComponent<Text>();
        DontDestroyOnLoad(GlobalDataObj);
        DontDestroyOnLoad(SoundGroup);

    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_LoginBtn != null)
            m_LoginBtn.onClick.AddListener(LoginBtn);


        if (m_CreateAccOpenBtn != null)
            m_CreateAccOpenBtn.onClick.AddListener(OpenCreateAccBtn);


        if (m_CreateAccountBtn != null)
            m_CreateAccountBtn.onClick.AddListener(CreateAccountBtn);


        if (m_CancelButton != null)
            m_CancelButton.onClick.AddListener(CreateCancelBtn);

        if (m_GuestLoginBtn != null)
            m_GuestLoginBtn.onClick.AddListener(GuestLoginBtn);


        m_FacebookLoginBtn.onClick.AddListener(() => {
            return;
        });

        m_GoogleLoginBtn.onClick.AddListener(() => {
            return;
        });



        m_LoginPanelObj.SetActive(false);



        //오디오 사운드 초기 설정

        SoundMgr.g_inst.MasterMixer.SetFloat("BGM",-20f);
        SoundMgr.g_inst.MasterMixer.SetFloat("UI",-20f);
        SoundMgr.g_inst.MasterMixer.SetFloat("Effect",-20f);

        SoundMgr.g_inst.UISound_Audio.volume = 0.5f;
        SoundMgr.g_inst.Background_Audio.volume = 0.5f;

        for(int i = 0; i < SoundMgr.g_inst.EffectArray_Audio.Length; i++)
        {
            SoundMgr.g_inst.EffectArray_Audio[i].volume = 0.5f;
        }



        //BGM Play
        SoundMgr.g_inst.Background_Audio.clip = SoundMgr.g_inst.BGMTitle_Sound;
        SoundMgr.g_inst.Background_Audio.playOnAwake = true;    //씬 시작시 시작
        SoundMgr.g_inst.Background_Audio.loop = true; //반복 재생
        SoundMgr.g_inst.Background_Audio.Play();
    }



    // Update is called once per frame
    void Update()
    {
        if (m_StartFade == true) {
            m_LoginPanelObj.SetActive(false);

            if (m_CacTime < 1.0f) {
                m_AddTimer = m_AddTimer + Time.deltaTime;
                m_CacTime = m_AddTimer / AniDuring;
                m_Color = m_FadePanel.color;
                m_Color.a = m_CacTime;   //Mathf.Lerp(0.0f, 100.0f, m_CacTime);
                m_FadePanel.color = m_Color;
                if (1.0f <= m_CacTime) {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
                }
            }
        }//if (a_OneClick == false)    



        TextBlink();



        if (Input.GetMouseButtonDown(0))
        {

            if(m_Touch_Text.gameObject.activeSelf == true)
            {
                m_LoginPanelObj.SetActive(true);
                
                m_Touch_Text.gameObject.SetActive(false);
            }


           
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Touch_Sound, 1.0f);

        }



        if (Input.GetMouseButtonDown(1))
        {

            IDInputField.text = "test01@test.com";
            PassInputField.text = "q1w2e3r4";



        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClickCount++;
            if (!IsInvoking("DoubleClick"))
                Invoke("DoubleClick", 1.0f);

        }
        else if (ClickCount == 2)
        {
            CancelInvoke("DoubleClick");
            Application.Quit();
        }

    }

   

    public void LoginBtn () {
        SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);


        string a_IdStr = IDInputField.text;
        string a_PwStr = PassInputField.text;
        if (a_IdStr.Trim() == "" || a_PwStr.Trim() == "") {
            GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "ID, PW 빈칸 없이 입력해 주셔야 합니다.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                Destroy(InfoWindow);

            });
            return;
        }

        if (!(3 <= a_IdStr.Length && a_IdStr.Length < 20)) {
            GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "ID는 3글자 이상 20글자 이하 입니다.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                Destroy(InfoWindow);

            });
            return;
        }

        if (!(6 <= a_PwStr.Length && a_PwStr.Length < 20)) {
            GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "비밀번호는 6글자 이상 20글자 이하 입니다.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                Destroy(InfoWindow);

            });
            return;
        }

        if (!CheckEmailAddress(IDInputField.text)) {
            GameObject InfoWindow = Instantiate(InfoMessage,GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "Email 형식이 맞지 않습니다.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                Destroy(InfoWindow);
            
            });
            return;
        }

        var request = new LoginWithEmailAddressRequest {
            Email = IDInputField.text,
            Password = PassInputField.text,
            //-------------이옵션으로 추가해 줘야 로그인하면서 유저의 각종 정보를 가져올 수 있다.
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() {
                //-이옵션으로  DisplayName, AvatarUrl을 가져올 수 있다.
                GetPlayerProfile = true,
                ProfileConstraints = new PlayerProfileViewConstraints() {
                    ShowDisplayName = true, //이 옵션으로 Display Name,
                    //ShowAvatarUrl = true //이 옵션으로 AvatarUrl을 가져올 수 있다.
                },
                //-이옵션으로  DisplayName, AvatarUrl을 가져올 수 있다.

                //GetPlayerStatistics = true //이 옵션으로 통계값(순위표에 관여하는)을 불러올 수 있다.
                GetUserData = true, //-이 옵션으로 <플레이어 데이터(타이틀) 값을 불러올 수 있다.

            }
            //-------------이옵션으로 추가해 줘야 로그인하면서 유저의 각종 정보를 가져올 수 있다.
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);


    }



    //Playfab에도 facebook에도 LoginResult가 있어서 충돌로 인해 오류 발생한다.
    //명확하게 PlayFab.ClientModels를 붙여준다.
    private void OnLoginSuccess (PlayFab.ClientModels.LoginResult result) {
        g_Message = "로그인 성공";

        Debug.Log(g_Message);


        //플레이팹에서 해당 유저 정보 가져와서 글로벌벨류에 저장
        //유저 아이디, 유저 닉네임 = 플레이팹에서 가져오기
        GlobalUserData.m_Uid_str = result.PlayFabId;

        if (result.InfoResultPayload != null) {
            GlobalUserData.m_NickName_str = result.InfoResultPayload.PlayerProfile.DisplayName;
        }



        //웹서버 UserInfo DB에서 UserId와 GameNumber(0)으로 기록 검색
        //존재하지 않다면, 새로 Insert 하고 글로벌 벨류 값은 0 0 "" "" 
        StartCoroutine(UserInfo_Update());

        //존재한다면, 기본게임 클리어 기록 가져오기
        StartCoroutine(UserInfo_Select());


        SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.SceneChange_Sound, 1.0f);

        //페이드
        m_FadePanel.gameObject.SetActive(true);
        if (m_StartFade == false) {
            m_StartFade = true;

        }
    }

    private void OnLoginFailure (PlayFabError error) {
        
        GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

        string temp = error.GenerateErrorReport();

        if (temp.Contains("User not found") || temp.Contains("Invalid email address or password"))
            temp = "아이디 또는 비밀번호가 틀렸습니다.";


        InfoWindow.GetComponent<InfoMessage>().info_Text.text = "[로그인 실패]\r\n" + temp;
        InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
            Destroy(InfoWindow);

        });

    }



    private void GuestLoginBtn()
    {

        //게스트 계정 생성
        PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest()
        {
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            },
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier
        }, GuestLoginSuccess, GuestLoginFailure);


    }


    private void GuestLoginSuccess(PlayFab.ClientModels.LoginResult result)
    {

        //유저 정보 불러오기
        GlobalUserData.m_Uid_str = result.PlayFabId;
        if (result.InfoResultPayload.PlayerProfile == null)
        {
            GuestLoginBtn();
            return;
        }


        //닉네임 존재여부 확인
        if (result.InfoResultPayload.PlayerProfile.DisplayName != null && result.InfoResultPayload.PlayerProfile.DisplayName != "")
        {
            //존재시
            if (result.InfoResultPayload != null){

                //불러오기
                GlobalUserData.m_NickName_str = result.InfoResultPayload.PlayerProfile.DisplayName;
            }

            //유저 기록 불러오기
            //웹서버 UserInfo DB에서 UserId와 GameNumber(0)으로 기록 검색
            //존재하지 않다면, 새로 Insert 하고 글로벌 벨류 값은 0 0 "" "" 
            StartCoroutine(UserInfo_Update());

            //존재한다면, 기본게임 클리어 기록 가져오기
            StartCoroutine(UserInfo_Select());


            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.SceneChange_Sound, 1.0f);

            //페이드
            m_FadePanel.gameObject.SetActive(true);
            if (m_StartFade == false)
            {
                m_StartFade = true;

            }
        }
        else
        {
            //없을시

            //기기ID로 닉네임 생성 or Gest_XXXXXX
            MakeRandomCode();

            //닉네임 업데이트
            // playfab 서버 접속되었는지 확인하여 되면 실행. 아니면 에러메세지 출력
            var request = new UpdateUserTitleDisplayNameRequest { DisplayName = randomNickName };
            PlayFabClientAPI.UpdateUserTitleDisplayName(request, NickNameCreateSuccess, NickNameCreateFailure);


        }

        
    }


    private void GuestLoginFailure(PlayFabError error)
    {
        // 기기 로그인 실패 안내창
        GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

        InfoWindow.GetComponent<InfoMessage>().info_Text.text = "기기 로그인 실패 \r\n문의 요망.\r\n" + error.GenerateErrorReport();
        InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
            Destroy(InfoWindow);

        });
    }   
     
    private void NickNameCreateSuccess(UpdateUserTitleDisplayNameResult result)
    {
        GlobalUserData.m_NickName_str = randomNickName;

        //유저 기록 불러오기
        //웹서버 UserInfo DB에서 UserId와 GameNumber(0)으로 기록 검색
        //존재하지 않다면, 새로 Insert 하고 글로벌 벨류 값은 0 0 "" "" 
        StartCoroutine(UserInfo_Update());

        //존재한다면, 기본게임 클리어 기록 가져오기
        StartCoroutine(UserInfo_Select());


        SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.SceneChange_Sound, 1.0f);

        //페이드
        m_FadePanel.gameObject.SetActive(true);
        if (m_StartFade == false)
        {
            m_StartFade = true;

        }
    }


    private void NickNameCreateFailure(PlayFabError error)
    {
        randomNickName = "";
        //다시 게스트 로그인 시도 (닉네임 재 생성)
        GuestLoginBtn();
    }




    public void OpenCreateAccBtn () {

        SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.OpenWindow_Sound, 1.0f);


        if (m_LoginPanelObj != null)
            m_LoginPanelObj.SetActive(false);

        if (m_CreateAccPanelObj != null)
            m_CreateAccPanelObj.SetActive(true);
    }





    public void CreateAccountBtn () {

        //UISound_Audio.PlayOneShot(Button1_Sound, 1.0f);


        string a_IdStr = New_IDInputField.text;
        string a_PwStr = New_PassInputField.text;
        string a_NickStr = New_NickInputField.text;

        if (a_IdStr.Trim() == "" || a_PwStr.Trim() == "" || a_NickStr.Trim() == "") {


            GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "ID, PW, 별명 빈칸 없이 입력해 주셔야 합니다.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                Destroy(InfoWindow);

            });
            return;
        }

        if (!(3 <= a_IdStr.Length && a_IdStr.Length < 20)) {
            GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "ID는 3글자 이상 20글자 이하로 작성해 주세여.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                Destroy(InfoWindow);

            });

            return;
        }

        if (!(6 <= a_PwStr.Length && a_PwStr.Length < 20)) {
            GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "비밀번호는 6글자 이상 20글자 이하로 작성해 주세여.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                Destroy(InfoWindow);

            });

            return;
        }

        if (!CheckEmailAddress(New_IDInputField.text)) {
            GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

            InfoWindow.GetComponent<InfoMessage>().info_Text.text = "Email 형식이 맞지 않습니다.";
            InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                Destroy(InfoWindow);

            });

            return;
        }



        var request = new RegisterPlayFabUserRequest {
            Email = New_IDInputField.text,
            Password = New_PassInputField.text,
            DisplayName = New_NickInputField.text,
            RequireBothUsernameAndEmail = false
            //UserName = "구글이나 페이스북 유니크 ID"
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFailure);


        //<ID 또는 Emaill을 등록하지 않아도 계정이 생성되게 하고 싶으면
        //RequireBothUsernameAndEmail = false 로 지정하지 않으면 사용자 이름과 이메일이 모두 필요
        //RegisterPlayFabUserRequest의 매개변수 이며 기본적으로 true로 설정되어 있습니다.
        //요청에 추가하고 사용자 이름을 지정하지 않고 등록 할 수 있도록 false로 설정 할 수 있습니다.

        New_IDInputField.text = "";
        New_PassInputField.text = "";
        New_NickInputField.text = "";

    }

    private void RegisterSuccess (RegisterPlayFabUserResult result) {
        GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

        InfoWindow.GetComponent<InfoMessage>().info_Text.text = "가입 성공";
        InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
            Destroy(InfoWindow);

        });
        

        CreateCancelBtn();

    }

    private void RegisterFailure (PlayFabError error) {

        GameObject InfoWindow = Instantiate(InfoMessage, GameObject.Find("Canvas").transform);

        string temp = error.GenerateErrorReport();

        if (temp.Contains("The display name entered is not available"))
            temp = "이미 사용중인 닉네임 입니다.";
        else if (temp.Contains("Email address not available"))
            temp = "이미 사용중인 이메일 입니다.";
        else if (temp.Contains("Invalid input parameters"))
            temp = "닉네임은 3글자 이상으로 작성해주세요.";



        InfoWindow.GetComponent<InfoMessage>().info_Text.text = "[가입 실패]\r\n" + temp;
        InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
            Destroy(InfoWindow);

        });

    }



    public void CreateCancelBtn () {

        SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.CloseWindow_Sound, 1.0f);

        if (m_LoginPanelObj != null)
            m_LoginPanelObj.SetActive(true);

        if (m_CreateAccPanelObj != null)
            m_CreateAccPanelObj.SetActive(false);


        New_IDInputField.text = "";
        New_PassInputField.text = "";
        New_NickInputField.text = "";

    }




    //------이메일 형식이 맞는지 확인 함수
    private bool CheckEmailAddress (string EmailStr) {
        if (string.IsNullOrEmpty(EmailStr))
            isValidFormat = false;

        EmailStr = Regex.Replace(EmailStr, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
        if (invalidEmailType)
            isValidFormat = false;

        // true 로 반환할 시, 올바른 이메일 포맷임.
        isValidFormat = Regex.IsMatch(EmailStr,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase);

        return isValidFormat;
    }

    private string DomainMapper (Match match) {
        // IdnMapping class with default property values.
        IdnMapping idn = new IdnMapping();

        string domainName = match.Groups[2].Value;
        try {
            domainName = idn.GetAscii(domainName);
        }
        catch (ArgumentException) {
            invalidEmailType = true;
        }
        return match.Groups[1].Value + domainName;
    }
    //------이메일 형식이 맞는지 확인 함수


    void MakeRandomCode()
    {


        string strRandomChar = "QWERTYUIOPASDFGHJKLZXCVBNM0123456789"; //랜덤으로 들어갈 문자 및 숫자 


        UnityEngine.Random.InitState((int)(DateTime.Now.Ticks));


        StringBuilder rs = new StringBuilder();

        for (int i = 0; i < 6; i++)
        {
            rs.Append(strRandomChar[(int)(UnityEngine.Random.Range(0.1f, 0.9f) * strRandomChar.Length)]);
        }

        randomNickName = "Gest_" + rs.ToString();
    }

    void TextBlink()
    {

        if(Blink_flag == true)
            m_Touch_Text.color = Color.Lerp(Color_Active, Color_Disabled, Blink_time);
        else
            m_Touch_Text.color = Color.Lerp(Color_Disabled, Color_Active, Blink_time);

        Blink_time += Time.deltaTime * BlinkSpeed;

        if((Blink_flag==true && m_Touch_Text.color == Color_Disabled) ||
            (Blink_flag == false && m_Touch_Text.color == Color_Active))
        {
            Blink_time = 0.0f;
            Blink_flag = !Blink_flag;
        }

    }





    IEnumerator UserInfo_Update () {
        //PHP - UserInfo_Update
        //입력받은 userid와 gameid로 기록이 있나 확인
        //기록이 없으면, Insert  
        //  게임오버시, user_id, game_number, top_round 값만 / 클리어시, user_id, game_number, top_round, heart, clear_time, clear_date

        //기록이 있으면, 입력받은 top_round 기록된 top_round보다 같거나높을때만(신기록) Update
        //  게임오버시, top_round 만 업데이트 / 클리어시, top_round, heart, clear_time, clear_date 업데이트


        //계정생성후 기본게임에 대한 기록 생성을 위해 user_id와 기본값들을 넣어줌
        //기록이 있는경우 아무런작업을 안하고, 기록이 없는경우 Insert를 통해 초기 기록생성
        WWWForm form = new WWWForm();
        form.AddField("user_id", GlobalUserData.m_Uid_str, System.Text.Encoding.UTF8);
        form.AddField("game_id", 0);
        form.AddField("top_round", 0);
        form.AddField("heart", 0);
        form.AddField("clear_time", "", System.Text.Encoding.UTF8);
        form.AddField("clear_date", "", System.Text.Encoding.UTF8);
        form.AddField("like_count", 0);

        WWW webRequest = new WWW(UpdateUrl, form);
        yield return webRequest;
        g_Message = webRequest.text;

    }


    IEnumerator UserInfo_Select () {
        //PHP로 MySql에서 inputField의 이름값으로 검색
        WWWForm form = new WWWForm();
        form.AddField("user_id", GlobalUserData.m_Uid_str, System.Text.Encoding.UTF8);
        form.AddField("game_id", 0);
        WWW webRequest = new WWW(SelectUrl, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(webRequest.bytes);
        g_Message = sz;


        if (sz.Contains("Select-Success!!") == true) {
            //text로 이름과 점수 출력해주고
            //JSON 파싱
            if (sz.Contains("top_round") == true) {

                var N = JSON.Parse(sz);
                if (N != null) {
                    if (N["top_round"] != null) {
                        GlobalUserData.m_TopRound_int = N["top_round"];
                    }

                    if (N["heart"] != null) {
                        GlobalUserData.m_ClearHeart_int = N["heart"];
                    }

                    if (N["clear_time"] != null) {
                        GlobalUserData.m_ClearTime_str = N["clear_time"];
                    }

                    if (N["clear_date"] != null) {
                        GlobalUserData.m_ClearDate_str = N["clear_date"];
                    }
                }
            }//if (sz.Contains("nick_name") == true)
        }//if (sz.Contains("Login-Success!!") == true)
        else {

            //데이터가 없다면,
            g_Message = "등록된 정보가 없습니다.";
        }


    }



    


    /*
    void OnGUI () {
        if (g_Message != "") {
            GUILayout.Label("<color=White><size=25>" + g_Message + "</size></color>");
        }
    }
    */
}
