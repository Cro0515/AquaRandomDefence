using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class InfoMessage : MonoBehaviour
{
    public Text info_Text;
    public Button OK_Btn;



    private void Awake()
    {
        info_Text = GameObject.Find("InfoPanel").transform.Find("Info_Text").GetComponent<Text>();
        OK_Btn = GameObject.Find("OK_Btn").GetComponent<Button>();



    }



    // Start is called before the first frame update
    void Start()
    {
        if(SoundMgr.g_inst != null)
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.InfoWindow_Sound, 1.0f);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("Login");


        OK_Btn.onClick.AddListener(() => {
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button1_Sound, 1.0f);


        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
