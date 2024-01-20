/*

            -몬스터 데이터 가져오기 (로컬)
-플레이스 홀더 작업
-
    
    

 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Edit_MonsterSetting : MonoBehaviour
{


    //-------Button
    Button PinBoom_Btn;
    Button Dust_Btn;
    Button Bottle_Btn;
    Button Petroleum_Btn;
    Button Trash_Btn;

    Button Clicked_Btn;     //클린된 버튼 저장용 변수

    Vector2 Clicked_Size = new Vector2(170, 70);
    Vector2 Generic_Size = new Vector2(180, 70);
    Color Clicked_Color = new Color32(200, 200, 200, 255);
    Color Generic_Color = new Color32(255, 255, 255, 255);
    float Clicked_X_Pos;   //703
    float Generic_X_Pos;   //693.3
    
    

    Button Finish_Btn;
    //-------Button



    //-------Info Panel
    Image Monster_Image;
    Text MonsterName_Text;
    Text MonsterText_Text;

    InputField MonsterHP_Input;
    InputField MonsterDamage_Input;
    InputField MonsterSpeed_Input;


    Sprite m_PinBoom_Sprite;
    Sprite m_Dust_Sprite;
    Sprite m_Bottle_Sprite;
    Sprite m_Petroleum_Sprite;
    Sprite m_Trash_Sprite;





    //-------Info Panel




    //-------Monster Info Data

    MonSet[] local_MonSet;

    //-------Monster Info Data



    void ResourceLoad ()
    {
        m_PinBoom_Sprite = Resources.Load<GameObject>("Prefab/Monster/PinBoom").GetComponent<SpriteRenderer>().sprite;
        m_Dust_Sprite = Resources.Load<GameObject>("Prefab/Monster/Dust").GetComponent<SpriteRenderer>().sprite;
        m_Bottle_Sprite = Resources.Load<GameObject>("Prefab/Monster/Bottle").GetComponent<SpriteRenderer>().sprite;
        m_Petroleum_Sprite = Resources.Load<GameObject>("Prefab/Monster/Petroleum").GetComponent<SpriteRenderer>().sprite;
        m_Trash_Sprite = Resources.Load<GameObject>("Prefab/Monster/Trash").GetComponent<SpriteRenderer>().sprite;

    }

    void ObjectLoad ()
    {
        PinBoom_Btn = GameObject.Find("MonsterButtonGroup").transform.Find("PinBoomBtn").gameObject.GetComponent<Button>();
        Dust_Btn = GameObject.Find("MonsterButtonGroup").transform.Find("DustBtn").gameObject.GetComponent<Button>();
        Bottle_Btn = GameObject.Find("MonsterButtonGroup").transform.Find("BottleBtn").gameObject.GetComponent<Button>();
        Petroleum_Btn = GameObject.Find("MonsterButtonGroup").transform.Find("PetroleumBtn").gameObject.GetComponent<Button>();
        Trash_Btn = GameObject.Find("MonsterButtonGroup").transform.Find("TrashBtn").gameObject.GetComponent<Button>();

        Monster_Image = GameObject.Find("MonsterCustomPanel/MonsterInfo/MonsterBackground").transform.Find("MonsterImage").gameObject.GetComponent<Image>();
        MonsterName_Text = GameObject.Find("MonsterCustomPanel/MonsterInfo/MonsterTitleBackground").transform.Find("MonsterNameText").gameObject.GetComponent<Text>();
        MonsterText_Text = GameObject.Find("MonsterCustomPanel/MonsterInfo").transform.Find("MonsterContentsText").gameObject.GetComponent<Text>();

        MonsterHP_Input = GameObject.Find("Line_HP").transform.Find("Hp_Input").gameObject.GetComponent<InputField>();
        MonsterDamage_Input = GameObject.Find("Line_Damage").transform.Find("Damage_Input").gameObject.GetComponent<InputField>();
        MonsterSpeed_Input = GameObject.Find("Line_Speed").transform.Find("Speed_Input").gameObject.GetComponent<InputField>();

        Finish_Btn = GameObject.Find("MonsterSettingPanelObj").transform.Find("FinishBtn").gameObject.GetComponent<Button>();

    }

    void InputOnEditEnd_Collection()
    {
        MonsterHP_Input.onEndEdit.AddListener(delegate {
            ValueVerification(MonsterHP_Input,1,100000);
        });

        MonsterDamage_Input.onEndEdit.AddListener(delegate {
            ValueVerification(MonsterDamage_Input, 1, 100);
        });

        MonsterSpeed_Input.onEndEdit.AddListener(delegate {
            ValueVerification(MonsterSpeed_Input, 1.0f, 10.0f);
        });

    }


    void BtnClick_Collection ()
    {

        PinBoom_Btn.onClick.AddListener(()=> {

            //같은 버튼 눌렀을때
            if (Clicked_Btn == PinBoom_Btn)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_2_Sound, 1.0f);


            //이전 데이터 세이브
            MonDataSave();

            //버튼 액션
            PinBoom_Btn.transform.position = new Vector3(Clicked_X_Pos, PinBoom_Btn.transform.position.y, PinBoom_Btn.transform.position.z);
            PinBoom_Btn.GetComponent<RectTransform>().sizeDelta = Clicked_Size;
            PinBoom_Btn.GetComponent<Image>().color = Clicked_Color;

            if(Clicked_Btn != null){
                Clicked_Btn.transform.position = new Vector3(Generic_X_Pos, Clicked_Btn.transform.position.y, Clicked_Btn.transform.position.z);
                Clicked_Btn.GetComponent<RectTransform>().sizeDelta = Generic_Size;
                Clicked_Btn.GetComponent<Image>().color = Generic_Color;
            }
            //--버튼 액션

            //이미지 교체
            Monster_Image.sprite = m_PinBoom_Sprite;
            //이미지 사이즈 복원
            if (Clicked_Btn == Bottle_Btn)
                Monster_Image.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            //몬스터 이름 변경
            MonsterName_Text.text = "핀붐";
            //폰트 사이즈 복원
            if (Clicked_Btn == Petroleum_Btn)
                MonsterName_Text.fontSize = 30;
            //몬스터 설명 변경
            MonsterText_Text.text = "핀핀붐~핀붐~";


            //플레이스 홀더 변경
            MonsterHP_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.PinBoom].Hp.ToString();
            MonsterDamage_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.PinBoom].Damage.ToString();
            MonsterSpeed_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.PinBoom].Speed.ToString();

            //임시변수에 값이 있다면, 임시변수 값 출력
            //값이 없다면, 빈칸
            TempDataFill(MonName.PinBoom);        


            //누른버튼 업데이트
            Clicked_Btn = PinBoom_Btn;
        });
        
        Dust_Btn.onClick.AddListener(() => {
            //같은 버튼 눌렀을때
            if (Clicked_Btn == Dust_Btn)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_2_Sound, 1.0f);

            //이전 데이터 세이브
            MonDataSave();

            //버튼 액션
            Dust_Btn.transform.position = new Vector3(Clicked_X_Pos, Dust_Btn.transform.position.y, Dust_Btn.transform.position.z);
            Dust_Btn.GetComponent<RectTransform>().sizeDelta = Clicked_Size;
            Dust_Btn.GetComponent<Image>().color = Clicked_Color;

            if (Clicked_Btn != null)
            {
                Clicked_Btn.transform.position = new Vector3(Generic_X_Pos, Clicked_Btn.transform.position.y, Clicked_Btn.transform.position.z);
                Clicked_Btn.GetComponent<RectTransform>().sizeDelta = Generic_Size;
                Clicked_Btn.GetComponent<Image>().color = Generic_Color;
            }
            //--버튼 액션

            //이미지 교체
            Monster_Image.sprite = m_Dust_Sprite;
            //이미지 사이즈 복원
            if (Clicked_Btn == Bottle_Btn)
                Monster_Image.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            //몬스터 이름 변경
            MonsterName_Text.text = "더스트";
            //폰트 사이즈 복원
            if (Clicked_Btn == Petroleum_Btn)
                MonsterName_Text.fontSize = 30;
            //몬스터 설명 변경
            MonsterText_Text.text = "먼지 먼먼지~";


            //플레이스 홀더 변경
            MonsterHP_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Dust].Hp.ToString();
            MonsterDamage_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Dust].Damage.ToString();
            MonsterSpeed_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Dust].Speed.ToString();

            //임시변수에 값이 있다면, 임시변수 값 출력
            //값이 없다면, 빈칸
            TempDataFill(MonName.Dust);


            //누른버튼 업데이트
            Clicked_Btn = Dust_Btn;

        });

        Bottle_Btn.onClick.AddListener(() => {
            //같은 버튼 눌렀을때
            if (Clicked_Btn == Bottle_Btn)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_2_Sound, 1.0f);

            //이전 데이터 세이브
            MonDataSave();

            //버튼 액션
            Bottle_Btn.transform.position = new Vector3(Clicked_X_Pos, Bottle_Btn.transform.position.y, Bottle_Btn.transform.position.z);
            Bottle_Btn.GetComponent<RectTransform>().sizeDelta = Clicked_Size;
            Bottle_Btn.GetComponent<Image>().color = Clicked_Color;

            if (Clicked_Btn != null)
            {
                Clicked_Btn.transform.position = new Vector3(Generic_X_Pos, Clicked_Btn.transform.position.y, Clicked_Btn.transform.position.z);
                Clicked_Btn.GetComponent<RectTransform>().sizeDelta = Generic_Size;
                Clicked_Btn.GetComponent<Image>().color = Generic_Color;
            }
            //--버튼 액션

            //이미지 교체
            Monster_Image.sprite = m_Bottle_Sprite;
            //이미지 크기 조절
            Monster_Image.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 100);
            //몬스터 이름 변경
            MonsterName_Text.text = "포도주스 병";
            //폰트 사이즈 복원
            if (Clicked_Btn == Petroleum_Btn)
                MonsterName_Text.fontSize = 30;
            //몬스터 설명 변경
            MonsterText_Text.text = "미닛메이드 포도~";


            //플레이스 홀더 변경
            MonsterHP_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Bottle].Hp.ToString();
            MonsterDamage_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Bottle].Damage.ToString();
            MonsterSpeed_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Bottle].Speed.ToString();

            //임시변수에 값이 있다면, 임시변수 값 출력
            //값이 없다면, 빈칸
            TempDataFill(MonName.Bottle);


            //누른버튼 업데이트
            Clicked_Btn = Bottle_Btn;

        });

        Petroleum_Btn.onClick.AddListener(() => {
            //같은 버튼 눌렀을때
            if (Clicked_Btn == Petroleum_Btn)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_2_Sound, 1.0f);

            //이전 데이터 세이브
            MonDataSave();

            //버튼 액션
            Petroleum_Btn.transform.position = new Vector3(Clicked_X_Pos, Petroleum_Btn.transform.position.y, Petroleum_Btn.transform.position.z);
            Petroleum_Btn.GetComponent<RectTransform>().sizeDelta = Clicked_Size;
            Petroleum_Btn.GetComponent<Image>().color = Clicked_Color;

            if (Clicked_Btn != null)
            {
                Clicked_Btn.transform.position = new Vector3(Generic_X_Pos, Clicked_Btn.transform.position.y, Clicked_Btn.transform.position.z);
                Clicked_Btn.GetComponent<RectTransform>().sizeDelta = Generic_Size;
                Clicked_Btn.GetComponent<Image>().color = Generic_Color;
            }
            //--버튼 액션

            //이미지 교체
            Monster_Image.sprite = m_Petroleum_Sprite;
            //이미지 사이즈 복원
            if (Clicked_Btn == Bottle_Btn)
                Monster_Image.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            //몬스터 이름 변경
            MonsterName_Text.text = "갈매기 슬라임";
            //폰트 사이즈 변경
            MonsterName_Text.fontSize = 25;
            //몬스터 설명 변경
            MonsterText_Text.text = "끼룩~ 끠룩~";


            //플레이스 홀더 변경
            MonsterHP_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Petroleum].Hp.ToString();
            MonsterDamage_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Petroleum].Damage.ToString();
            MonsterSpeed_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Petroleum].Speed.ToString();

            //임시변수에 값이 있다면, 임시변수 값 출력
            //값이 없다면, 빈칸
            TempDataFill(MonName.Petroleum);


            //누른버튼 업데이트
            Clicked_Btn = Petroleum_Btn;

        });

        Trash_Btn.onClick.AddListener(() => {
            //같은 버튼 눌렀을때
            if (Clicked_Btn == Trash_Btn)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_2_Sound, 1.0f);

            //이전 데이터 세이브
            MonDataSave();

            //버튼 액션
            Trash_Btn.transform.position = new Vector3(Clicked_X_Pos, Trash_Btn.transform.position.y, Trash_Btn.transform.position.z);
            Trash_Btn.GetComponent<RectTransform>().sizeDelta = Clicked_Size;
            Trash_Btn.GetComponent<Image>().color = Clicked_Color;

            if (Clicked_Btn != null)
            {
                Clicked_Btn.transform.position = new Vector3(Generic_X_Pos, Clicked_Btn.transform.position.y, Clicked_Btn.transform.position.z);
                Clicked_Btn.GetComponent<RectTransform>().sizeDelta = Generic_Size;
                Clicked_Btn.GetComponent<Image>().color = Generic_Color;
            }
            //--버튼 액션

            //이미지 교체
            Monster_Image.sprite = m_Trash_Sprite;
            //이미지 사이즈 복원
            if (Clicked_Btn == Bottle_Btn)
                Monster_Image.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            //몬스터 이름 변경
            MonsterName_Text.text = "더스트 박스";
            //폰트 사이즈 복원
            if (Clicked_Btn == Petroleum_Btn)
                MonsterName_Text.fontSize = 30;
            //몬스터 설명 변경
            MonsterText_Text.text = "The st BOX";


            //플레이스 홀더 변경
            MonsterHP_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Trash].Hp.ToString();
            MonsterDamage_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Trash].Damage.ToString();
            MonsterSpeed_Input.transform.Find("Placeholder").GetComponent<Text>().text = local_MonSet[(int)MonName.Trash].Speed.ToString();

            //임시변수에 값이 있다면, 임시변수 값 출력
            //값이 없다면, 빈칸
            TempDataFill(MonName.Trash);


            //누른버튼 업데이트
            Clicked_Btn = Trash_Btn;

        });



        Finish_Btn.onClick.AddListener(() =>{

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            MonDataSave();


            if (TempDataChheck())
            {

                //다음 스탭으로 넘어가기
                EditorMgr.g_Inst.m_EditStep++;
                EditorMgr.g_Inst.m_Blink_Target = EditorMgr.g_Inst.StepBar_RoundSetting_Btn.gameObject;

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

                //안내창 
                Debug.Log("빈칸을 채워주세요");

                GameObject InfoWindow = Instantiate(EditorMgr.g_Inst.InfoMessage, GameObject.Find("Canvas").transform);

                InfoWindow.GetComponent<InfoMessage>().info_Text.text = "빈칸을 채워주세요";
                InfoWindow.GetComponent<InfoMessage>().OK_Btn.onClick.AddListener(() => {
                    Destroy(InfoWindow);

                });
            }

        });
    }

  


    void Awake ()
    {
        ObjectLoad();
        ResourceLoad();
        BtnClick_Collection();
        InputOnEditEnd_Collection();

    }


    // Start is called before the first frame update
    void Start ()
    {
        GetLocalMonData();

        Generic_X_Pos = PinBoom_Btn.transform.position.x;
        Clicked_X_Pos = Generic_X_Pos + 10.0f;

        //첫번쨰 몬스터 버튼 클릭 Invoke();
        PinBoom_Btn.onClick.Invoke();

    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(PinBoom_Btn.transform.position);


            for (int i = 0; i < EditorMgr.g_Inst.temp_MonSet_Data.Length; i++)
            {
                EditorMgr.g_Inst.temp_MonSet_Data[i].Damage = 5;
                EditorMgr.g_Inst.temp_MonSet_Data[i].Hp = 10000;
                EditorMgr.g_Inst.temp_MonSet_Data[i].Speed = 5;

            }

            MonsterDamage_Input.text = "5";
            MonsterHP_Input.text = "10000";
            MonsterSpeed_Input.text = "5";
        }



    }






    void GetLocalMonData()
    {

        local_MonSet = EditorMgr.g_Inst.local_MonSet_Data;


        if (GlobalGameData.m_GameMode == GameMode.Standard)
        {
            EditorMgr.g_Inst.temp_MonSet_Data = new MonSet[local_MonSet.Length];


            //임시변수 초기화
        
            for (int i = 0; i < EditorMgr.g_Inst.temp_MonSet_Data.Length; i++)
            {
                EditorMgr.g_Inst.temp_MonSet_Data[i] = new MonSet(-3.0f, -1, -1);
            }
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




    void MonDataSave(){

        if(Clicked_Btn == PinBoom_Btn){
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.PinBoom].Hp = (MonsterHP_Input.text == "" ? -1 : int.Parse(MonsterHP_Input.text.Replace(",", "")));
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.PinBoom].Damage = (MonsterDamage_Input.text == "" ? -1 : int.Parse(MonsterDamage_Input.text.Replace(",", "")));
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.PinBoom].Speed = (MonsterSpeed_Input.text == "" ? -1 : float.Parse(MonsterSpeed_Input.text.Replace(",", "")));
        }
        else if(Clicked_Btn == Dust_Btn)
        {
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Dust].Hp = (MonsterHP_Input.text == "" ? -1 : int.Parse(MonsterHP_Input.text.Replace(",", "")));
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Dust].Damage = (MonsterDamage_Input.text == "" ? -1 : int.Parse(MonsterDamage_Input.text.Replace(",", "")));
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Dust].Speed = (MonsterSpeed_Input.text == "" ? -1 : float.Parse(MonsterSpeed_Input.text.Replace(",", "")));
        }
        else if(Clicked_Btn == Bottle_Btn)
        {
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Bottle].Hp = (MonsterHP_Input.text == "" ? -1 : int.Parse(MonsterHP_Input.text.Replace(",", "")));
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Bottle].Damage = (MonsterDamage_Input.text == "" ? -1 : int.Parse(MonsterDamage_Input.text.Replace(",", "")));
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Bottle].Speed = (MonsterSpeed_Input.text == "" ? -1 : float.Parse(MonsterSpeed_Input.text.Replace(",", "")));
        }
        else if(Clicked_Btn == Petroleum_Btn)
        {
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Petroleum].Hp = (MonsterHP_Input.text == "" ? -1 : int.Parse(MonsterHP_Input.text.Replace(",", "")));
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Petroleum].Damage = (MonsterDamage_Input.text == "" ? -1 : int.Parse(MonsterDamage_Input.text.Replace(",", "")));
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Petroleum].Speed = (MonsterSpeed_Input.text == "" ? -1 : float.Parse(MonsterSpeed_Input.text.Replace(",", "")));
        }
        else if(Clicked_Btn == Trash_Btn)
        {
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Trash].Hp = (MonsterHP_Input.text == "" ? -1 : int.Parse(MonsterHP_Input.text.Replace(",", "")));
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Trash].Damage = (MonsterDamage_Input.text == "" ? -1 : int.Parse(MonsterDamage_Input.text.Replace(",", "")));
            EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Trash].Speed = (MonsterSpeed_Input.text == "" ? -1 : float.Parse(MonsterSpeed_Input.text.Replace(",", "")));
        }



    }

    void TempDataFill(MonName _mon){

        if(_mon == MonName.PinBoom){
            MonsterHP_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.PinBoom].Hp < 0 ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.PinBoom].Hp.ToString();
            MonsterDamage_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.PinBoom].Damage < 0 ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.PinBoom].Damage.ToString();
            MonsterSpeed_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.PinBoom].Speed < 0.0f ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.PinBoom].Speed.ToString();


            ValueVerification(MonsterHP_Input, 1, 100000);
            ValueVerification(MonsterDamage_Input, 1, 100);

            ValueVerification(MonsterSpeed_Input, 1.0f, 10.0f);
        }
        else if(_mon == MonName.Dust){
            MonsterHP_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Dust].Hp < 0 ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Dust].Hp.ToString();
            MonsterDamage_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Dust].Damage < 0 ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Dust].Damage.ToString();
            MonsterSpeed_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Dust].Speed < 0.0f ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Dust].Speed.ToString();
           
            ValueVerification(MonsterHP_Input, 1, 100000);
            ValueVerification(MonsterDamage_Input, 1, 100);

            ValueVerification(MonsterSpeed_Input, 1.0f, 10.0f);
        }
        else if(_mon == MonName.Bottle){
            MonsterHP_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Bottle].Hp < 0 ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Bottle].Hp.ToString();
            MonsterDamage_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Bottle].Damage < 0 ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Bottle].Damage.ToString();
            MonsterSpeed_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Bottle].Speed < 0.0f ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Bottle].Speed.ToString();

            ValueVerification(MonsterHP_Input, 1, 100000);
            ValueVerification(MonsterDamage_Input, 1, 100);

            ValueVerification(MonsterSpeed_Input, 1.0f, 10.0f);
        }
        else if(_mon == MonName.Petroleum){
            MonsterHP_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Petroleum].Hp < 0 ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Petroleum].Hp.ToString();
            MonsterDamage_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Petroleum].Damage < 0 ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Petroleum].Damage.ToString();
            MonsterSpeed_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Petroleum].Speed < 0.0f ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Petroleum].Speed.ToString();

            ValueVerification(MonsterHP_Input, 1, 100000);
            ValueVerification(MonsterDamage_Input, 1, 100);

            ValueVerification(MonsterSpeed_Input, 1.0f, 10.0f);
        }
        else if(_mon == MonName.Trash){
            MonsterHP_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Trash].Hp < 0 ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Trash].Hp.ToString();
            MonsterDamage_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Trash].Damage < 0 ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Trash].Damage.ToString();
            MonsterSpeed_Input.text = EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Trash].Speed < 0.0f ? "" : EditorMgr.g_Inst.temp_MonSet_Data[(int)MonName.Trash].Speed.ToString();

            ValueVerification(MonsterHP_Input, 1, 100000);
            ValueVerification(MonsterDamage_Input, 1, 100);

            ValueVerification(MonsterSpeed_Input, 1.0f, 10.0f);
        }


    }

    bool TempDataChheck(){
        int cnt = 0;

        for (int i = 0; i < System.Enum.GetValues(typeof(MonName)).Length; i++){

            cnt += (EditorMgr.g_Inst.temp_MonSet_Data[i].Hp >= 0 ? 0 : 1);
            cnt += (EditorMgr.g_Inst.temp_MonSet_Data[i].Damage >= 0 ? 0 : 1);
            cnt += (EditorMgr.g_Inst.temp_MonSet_Data[i].Speed >= 0.0f ? 0 : 1);
        }

        return cnt <= 0;
    }

}
