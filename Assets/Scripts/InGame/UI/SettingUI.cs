using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingUI : MonoBehaviour
{

    //------------환경설정
    [HideInInspector] GameObject SettingPanel_Obj;

    [HideInInspector] public Slider MasterVolume_Slider;
    [HideInInspector] public Toggle MasterVolume_Toggle;
                      
    [HideInInspector] public Slider BackGroundVolume_Slider;
    [HideInInspector] public Slider EffectVolume_Slider;
    [HideInInspector] public Slider UIVolume_Slider;
                      
                      
    [HideInInspector] public Button Close_Btn;
    [HideInInspector] public Button SettingLobby_Btn;
    [HideInInspector] public Button QuitGame_Btn;
                      
    [HideInInspector] public GameObject YN_Message_Obj;
    [HideInInspector] public Text YN_Message_Text;
    [HideInInspector] public Button Yes_Btn;
    [HideInInspector] public Button No_Btn;

    [HideInInspector] public bool GameLobby_flag = false; // 0 = 로비 1 = 게임종료

    //------------환경설정


    void ObjectLoad()
    {
        SettingPanel_Obj = this.gameObject;

        MasterVolume_Slider = SettingPanel_Obj.transform.Find("FullVolume_Slider").GetComponent<Slider>();
        MasterVolume_Toggle = SettingPanel_Obj.transform.Find("FullVolume_Toggle").GetComponent<Toggle>();

        BackGroundVolume_Slider = SettingPanel_Obj.transform.Find("BackGroundVolume_Slider").GetComponent<Slider>();
        EffectVolume_Slider = SettingPanel_Obj.transform.Find("EffectVolume_Slider").GetComponent<Slider>();
        UIVolume_Slider = SettingPanel_Obj.transform.Find("UIVolume_Slider").GetComponent<Slider>();


        Close_Btn = SettingPanel_Obj.transform.Find("Close_Btn").GetComponent<Button>();
        SettingLobby_Btn = SettingPanel_Obj.transform.Find("Lobby_Btn").GetComponent<Button>();
        QuitGame_Btn = SettingPanel_Obj.transform.Find("QuitGame_Btn").GetComponent<Button>();


        YN_Message_Obj = SettingPanel_Obj.transform.Find("YN_Message_Panel").transform.gameObject;
        YN_Message_Text = YN_Message_Obj.transform.Find("YNMessage_Text").GetComponent<Text>();
        Yes_Btn = YN_Message_Obj.transform.Find("Yes_Btn").GetComponent<Button>();
        No_Btn = YN_Message_Obj.transform.Find("No_Btn").GetComponent<Button>();

    }


    void BtnClick_Collection()
    {
        Close_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.CloseWindow_Sound, 1.0f);


            Time.timeScale = 1.0f;
            SettingPanel_Obj.SetActive(false);
        });


        SettingLobby_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.InfoWindow_Sound, 1.0f);


            GameLobby_flag = false;

            YN_Message_Obj.SetActive(true);

            YN_Message_Text.text = "게임을 포기하고 로비로 나가시겠습니까?";
        });

        QuitGame_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.InfoWindow_Sound, 1.0f);

            GameLobby_flag = true;

            YN_Message_Obj.SetActive(true);

            YN_Message_Text.text = "게임을 종료 하시겠습니까?";
        });

        Yes_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            //로비
            if (GameLobby_flag == false)
            {
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.SceneChange_Sound, 1.0f);

                //
                GlobalGameData.Reset();


                UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
            }
            //게임종료
            else
            {
                Application.Quit();
            }

        });

        No_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button1_Sound, 1.0f);

            YN_Message_Obj.SetActive(false);

        });
    }


    void SliderEvent_Collection()
    {
        
        MasterVolume_Toggle.onValueChanged.AddListener((bool _flag) =>{

            /*
            SoundMgr.g_inst.UISound_Audio.volume = _flag == true ? 1 : 0;
            SoundMgr.g_inst.Background_Audio.volume = _flag == true ? 1 : 0;
            for (int i = 0; i < SoundMgr.g_inst.EffectArray_Audio.Length; i++){
                SoundMgr.g_inst.EffectArray_Audio[i].volume = _flag == true ? 1 : 0;
            }
            SoundMgr.g_inst.SoundOnOff = _flag;
            */

            SoundMgr.g_inst.UISound_Audio.mute = !_flag;
            SoundMgr.g_inst.Background_Audio.mute = !_flag;
            for (int i = 0; i < SoundMgr.g_inst.EffectArray_Audio.Length; i++)
            {
                SoundMgr.g_inst.EffectArray_Audio[i].mute = !_flag;
            }
            SoundMgr.g_inst.SoundOnOff = _flag;

        });

        MasterVolume_Slider.onValueChanged.AddListener((float _volume) => {

            float sound = MasterVolume_Slider.value;

            if (sound == -40f)
            {
                SoundMgr.g_inst.MasterMixer.SetFloat("BGM", -80);
                SoundMgr.g_inst.MasterMixer.SetFloat("UI", -80);
                SoundMgr.g_inst.MasterMixer.SetFloat("Effect", -80);
                

            }
            else
            {
                SoundMgr.g_inst.MasterMixer.SetFloat("BGM", sound);
                SoundMgr.g_inst.MasterMixer.SetFloat("UI", sound);
                SoundMgr.g_inst.MasterMixer.SetFloat("Effect", sound);

            }
        });


        BackGroundVolume_Slider.onValueChanged.AddListener((float _volume) => {

            SoundMgr.g_inst.Background_Audio.volume = _volume;
        });

        UIVolume_Slider.onValueChanged.AddListener((float _volume) => {

            SoundMgr.g_inst.UISound_Audio.volume = _volume;
        });

        EffectVolume_Slider.onValueChanged.AddListener((float _volume) => {

            for(int i = 0; i < SoundMgr.g_inst.EffectArray_Audio.Length; i++){
                SoundMgr.g_inst.EffectArray_Audio[i].volume = _volume;

            }

        });

    }


    void Awake()
    {
        ObjectLoad();




        BtnClick_Collection();
        SliderEvent_Collection();
    }


    // Start is called before the first frame update
    void Start()
    {
        float _vol = 0.0f;

        SoundMgr.g_inst.MasterMixer.GetFloat("BGM", out _vol);
        MasterVolume_Slider.value = _vol;

        MasterVolume_Toggle.isOn = SoundMgr.g_inst.SoundOnOff;

        BackGroundVolume_Slider.value = SoundMgr.g_inst.Background_Audio.volume;
        EffectVolume_Slider.value = SoundMgr.g_inst.EffectArray_Audio[0].volume;
        UIVolume_Slider.value = SoundMgr.g_inst.UISound_Audio.volume;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
