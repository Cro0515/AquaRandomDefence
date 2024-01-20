using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;


/*

      130/250

      클리어시------------
      내기록 (2023/01/01)
      클리어! 
      목숨 수 : 5
      타임 : 00:05:12.25  

      클리어 못했을시 --------
      내기록 (2023/01/01)
      최대 라운드 : 3

      기록 없을시---------
      <기록없음>

  */




public class NodeScript : MonoBehaviour
{


    public Text m_Title_Text;
    public Text m_PlayCnt_Text;
    public Text m_LikeCnt_Text;
    public Text m_CreatorNickName_Text;
    public Text m_Date_Text;



    //---------------------------상세
    public GameObject DetailGroup;

    public Text m_GameCode_Text;
    public Text m_RoundCnt_Text;
    public Text m_RecordDate_Text;
    public Text m_Recoed_Text;

    public Button m_Play_Btn;
    public Button m_Rank_Btn;




    string SELECT_MyRecord_URL = "http://devwhale.dothome.co.kr/ARD_Test/SELECT_MyRecord.php";

    string SELECT_Game_URL = "";







    // Start is called before the first frame update
    void Start()
    {


        this.GetComponent<Button>().onClick.AddListener(()=> {

            if (DetailGroup.activeSelf == false) {
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

                this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 320);
                StartCoroutine(SELECT_MyRecord());
                DetailGroup.SetActive(true);
            }
            else {
                SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button1_Sound, 1.0f);

                this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200);
                DetailGroup.SetActive(false);
            }
        });

        m_Play_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            LobbyMgr.Inst.CustomGameStart(m_GameCode_Text.text);

        });



        m_Rank_Btn.onClick.AddListener(() => { 
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.OpenWindow_Sound, 1.0f);



        });





    }

    // Update is called once per frame
    void Update()
    {
        
    }

  

    IEnumerator SELECT_MyRecord () {
        WWWForm form = new WWWForm();
        form.AddField("user_id", GlobalUserData.m_Uid_str);
        form.AddField("game_id", m_GameCode_Text.text);
        WWW webRequest = new WWW(SELECT_MyRecord_URL, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(webRequest.bytes);
        Debug.Log(sz);

        if (sz.Contains("RecordSearch-Success!!") == true) {
            var N = JSON.Parse(sz);
            if (N != null) {

                //클리어 못한경우 (탑라운드만 존재)
                if (N["heart"] == "0") {
                    m_RecordDate_Text.text = N["clear_date"];
                    m_Recoed_Text.text = "최대 라운드 : " + N["top_round"];
                }
                else {
                //클리어 한 경우 (탑라운드, 목숨, 클리어타임, 클리어데이트 전부 존재)
                    m_RecordDate_Text.text = N["clear_date"];
                    m_Recoed_Text.text = "클리어!" + "\n" + "목숨 수 : " + N["heart"] + "\n" + "타임 : " + N["clear_time"];
                }
            }


        }
        else {
            //기록이 아예 없는경우 (아예 존재 x)
            m_RecordDate_Text.text = "";
            m_Recoed_Text.text = "<기록 없음>";
        }
    }




}
