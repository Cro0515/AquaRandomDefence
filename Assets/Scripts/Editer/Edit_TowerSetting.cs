using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Edit_TowerSetting : MonoBehaviour
{
    public static Edit_TowerSetting g_Inst;

    Button ClamTower_Btn;
    Button CrabTower_Btn;
    Button ElectricTower_Btn;
    Button PufferTower_Btn;
    Button PoisonPufferTower_Btn;
    Button ShirmpTower_Btn;

    Button Clicked_Btn;     //클릭된 버튼 저장용 변수
    Vector2 Clicked_Size = new Vector2(150, 70);
    Vector2 Generic_Size = new Vector2(160, 70);
    Color Clicked_Color = new Color32(200, 200, 200, 255);
    Color Generic_Color = new Color32(255, 255, 255, 255);

    float Generic_Pos_X = 0;



    Button Finish_Btn;


    //-----------------Custom Panel
    Image Tower_Image;
    Text TowerName_Text;
    Text TowerContents_Text;

    Text TowerType_Text;
    Text TowerTarget_Text;


    InputField[] TowerDamage_Input_Array =  new InputField[4];
    InputField[] TowerAttackSpeed_Input_Array = new InputField[4];
    InputField[] TowerRange_Input_Array = new InputField[4];

    GameObject OptionLine_1_Float; 
    Image OptionLine_1_Float_Image;
    Text OptionLine_1_Float_Title;
    InputField[] TowerOption_1_Float_Input_Array = new InputField[4];

    GameObject OptionLine_1_Int;
    Image OptionLine_1_Int_Image;
    Text OptionLine_1_Int_Title;
    InputField[] TowerOption_1_Int_Input_Array = new InputField[4];


    GameObject OptionLine_2;
    Image OptionLine_2_Image;
    Text OptionLine_2_Title;
    InputField[] TowerOption_2_Input_Array = new InputField[4];

    InputField[] TowerCost_Input_Array = new InputField[4];
    //-----------------Custom Panel


    //-----------------Tower Info Data

    ClamTower local_ClamTower;
    CrabTower local_CrabTower;
    ElectricEelTower local_ElectricEelTower;
    PufferTower local_PufferTower;
    PoisonPufferTower local_PoisonPufferTower;

    ClamTower temp_ClamTower = new ClamTower();
    CrabTower temp_CrabTower = new CrabTower();
    ElectricEelTower temp_ElectricEelTower = new ElectricEelTower();
    PufferTower temp_PufferTower = new PufferTower();
    PoisonPufferTower temp_PoisonPufferTower = new PoisonPufferTower();




    Sprite m_ClamTower_Sprite;
    Sprite m_CrabTower_Sprite;
    Sprite m_ElectricEelTower_Sprite;
    Sprite m_PufferTower_Sprite;
    Sprite m_PoisonPufferTower_Sprite;

    Sprite m_ChainCnt_Sprite;
    Sprite m_DeBuffTime_Sprite;
    Sprite m_PoisonDamage_Sprite;

    //-----------------Tower Info Data

    float Option1_Float_Min = 0.0f;
    float Option1_Float_Max = 0.0f;
    int Option1_Int_Min = 0;
    int Option1_Int_Max = 0;

    int Option2_Min = 0;
    int Option2_Max = 0;




    void ResourceLoad () {

        m_ClamTower_Sprite = Resources.Load<GameObject>("Prefab/Tower/ClamTower").transform.Find("Tower").GetComponent<SpriteRenderer>().sprite;
        m_CrabTower_Sprite = Resources.Load<GameObject>("Prefab/Tower/CrabTower").transform.Find("Tower").GetComponent<SpriteRenderer>().sprite;
        m_ElectricEelTower_Sprite = Resources.Load<GameObject>("Prefab/Tower/ElectricTower").transform.Find("Tower").GetComponent<SpriteRenderer>().sprite;
        m_PufferTower_Sprite = Resources.Load<GameObject>("Prefab/Tower/PufferTower").transform.Find("Tower").GetComponent<SpriteRenderer>().sprite;
        m_PoisonPufferTower_Sprite = Resources.Load<GameObject>("Prefab/Tower/PoisonPufferTower").transform.Find("Tower").GetComponent<SpriteRenderer>().sprite;

        m_ChainCnt_Sprite = Resources.Load<Sprite>("Image/UI/TowerInfo/Info_ChainCnt");
        m_DeBuffTime_Sprite = Resources.Load<Sprite>("Image/UI/TowerInfo/Info_Time");
        m_PoisonDamage_Sprite = Resources.Load<Sprite>("Image/UI/TowerInfo/Info_Poison");

    }

    void ObjectLoad (){
        ClamTower_Btn = GameObject.Find("TowerButtonGroup").transform.Find("ClamTowerBtn").gameObject.GetComponent<Button>();
        CrabTower_Btn = GameObject.Find("TowerButtonGroup").transform.Find("CrabTowerBtn").gameObject.GetComponent<Button>();
        ElectricTower_Btn = GameObject.Find("TowerButtonGroup").transform.Find("ElectricTowerBtn").gameObject.GetComponent<Button>();
        PufferTower_Btn = GameObject.Find("TowerButtonGroup").transform.Find("PufferTowerBtn").gameObject.GetComponent<Button>();
        PoisonPufferTower_Btn = GameObject.Find("TowerButtonGroup").transform.Find("PoisonPufferTowerBtn").gameObject.GetComponent<Button>();
        ShirmpTower_Btn = GameObject.Find("TowerButtonGroup").transform.Find("ShirmpTowerBtn").gameObject.GetComponent<Button>();

        Finish_Btn = GameObject.Find("TowerEditPanelObj").transform.Find("FinishBtn").gameObject.GetComponent<Button>();


        //-----------------Custom Panel
        //타워 정보
        Tower_Image = GameObject.Find("TowerCustomPanel/TowerInfo/TowerBackground").transform.Find("TowerImage").gameObject.GetComponent<Image>();
        TowerName_Text = GameObject.Find("TowerCustomPanel/TowerInfo/TowerTitleBackground").transform.Find("TowerNameText").gameObject.GetComponent<Text>();
        TowerContents_Text = GameObject.Find("TowerCustomPanel/TowerInfo").transform.Find("TowerContentsText").gameObject.GetComponent<Text>();

        //타입, 타겟
        TowerType_Text = GameObject.Find("TowerCustomPanel/AbsoluteCell/Type").transform.Find("TypeContents_Text").gameObject.GetComponent<Text>();
        TowerTarget_Text = GameObject.Find("TowerCustomPanel/AbsoluteCell/Target").transform.Find("TargetContents_Text").gameObject.GetComponent<Text>();

        //데미지
        TowerDamage_Input_Array[0] = GameObject.Find("Line_Damage").transform.Find("Rare_Input").gameObject.GetComponent<InputField>();
        TowerDamage_Input_Array[1] = GameObject.Find("Line_Damage").transform.Find("Epic_Input").gameObject.GetComponent<InputField>();
        TowerDamage_Input_Array[2] = GameObject.Find("Line_Damage").transform.Find("Unique_Input").gameObject.GetComponent<InputField>();
        TowerDamage_Input_Array[3] = GameObject.Find("Line_Damage").transform.Find("Legendary_Input").gameObject.GetComponent<InputField>();

        //공격속도
        TowerAttackSpeed_Input_Array[0] = GameObject.Find("Line_AttackSpeed").transform.Find("Rare_Input").gameObject.GetComponent<InputField>();
        TowerAttackSpeed_Input_Array[1] = GameObject.Find("Line_AttackSpeed").transform.Find("Epic_Input").gameObject.GetComponent<InputField>();
        TowerAttackSpeed_Input_Array[2] = GameObject.Find("Line_AttackSpeed").transform.Find("Unique_Input").gameObject.GetComponent<InputField>();
        TowerAttackSpeed_Input_Array[3] = GameObject.Find("Line_AttackSpeed").transform.Find("Legendary_Input").gameObject.GetComponent<InputField>();

        //범위
        TowerRange_Input_Array[0] = GameObject.Find("Line_Range").transform.Find("Rare_Input").gameObject.GetComponent<InputField>();
        TowerRange_Input_Array[1] = GameObject.Find("Line_Range").transform.Find("Epic_Input").gameObject.GetComponent<InputField>();
        TowerRange_Input_Array[2] = GameObject.Find("Line_Range").transform.Find("Unique_Input").gameObject.GetComponent<InputField>();
        TowerRange_Input_Array[3] = GameObject.Find("Line_Range").transform.Find("Legendary_Input").gameObject.GetComponent<InputField>();

        //옵션 1 라인 Float
        OptionLine_1_Float = GameObject.Find("TowerCustomPanel/CustomCell").transform.Find("Line_Cell_1_Float").gameObject;
        OptionLine_1_Float_Image = OptionLine_1_Float.transform.Find("Icon").transform.Find("Icon_Image").gameObject.GetComponent<Image>();
        OptionLine_1_Float_Title = OptionLine_1_Float.transform.Find("Title_Text").GetComponent<Text>();
        TowerOption_1_Float_Input_Array[0] = OptionLine_1_Float.transform.Find("Rare_Input").gameObject.GetComponent<InputField>();
        TowerOption_1_Float_Input_Array[1] = OptionLine_1_Float.transform.Find("Epic_Input").gameObject.GetComponent<InputField>();
        TowerOption_1_Float_Input_Array[2] = OptionLine_1_Float.transform.Find("Unique_Input").gameObject.GetComponent<InputField>();
        TowerOption_1_Float_Input_Array[3] = OptionLine_1_Float.transform.Find("Legendary_Input").gameObject.GetComponent<InputField>();

        //옵션 1 라인 Int
        OptionLine_1_Int = GameObject.Find("TowerCustomPanel/CustomCell").transform.Find("Line_Cell_1_Int").gameObject;
        OptionLine_1_Int_Image = OptionLine_1_Int.transform.Find("Icon").transform.Find("Icon_Image").gameObject.GetComponent<Image>();
        OptionLine_1_Int_Title = OptionLine_1_Int.transform.Find("Title_Text").GetComponent<Text>();
        TowerOption_1_Int_Input_Array[0] = OptionLine_1_Int.transform.Find("Rare_Input").gameObject.GetComponent<InputField>();
        TowerOption_1_Int_Input_Array[1] = OptionLine_1_Int.transform.Find("Epic_Input").gameObject.GetComponent<InputField>();
        TowerOption_1_Int_Input_Array[2] = OptionLine_1_Int.transform.Find("Unique_Input").gameObject.GetComponent<InputField>();
        TowerOption_1_Int_Input_Array[3] = OptionLine_1_Int.transform.Find("Legendary_Input").gameObject.GetComponent<InputField>();

        //옵션 2 라인
        OptionLine_2 = GameObject.Find("TowerCustomPanel/CustomCell").transform.Find("Line_Cell_2").gameObject;
        OptionLine_2_Image = OptionLine_2.transform.Find("Icon").transform.Find("Icon_Image").gameObject.GetComponent<Image>();
        OptionLine_2_Title = OptionLine_2.transform.Find("Title_Text").GetComponent<Text>();
        TowerOption_2_Input_Array[0] = OptionLine_2.transform.Find("Rare_Input").gameObject.GetComponent<InputField>();
        TowerOption_2_Input_Array[1] = OptionLine_2.transform.Find("Epic_Input").gameObject.GetComponent<InputField>();
        TowerOption_2_Input_Array[2] = OptionLine_2.transform.Find("Unique_Input").gameObject.GetComponent<InputField>();
        TowerOption_2_Input_Array[3] = OptionLine_2.transform.Find("Legendary_Input").gameObject.GetComponent<InputField>();

        //코스트
        TowerCost_Input_Array[0] = GameObject.Find("Line_Cost").transform.Find("Rare_Input").gameObject.GetComponent<InputField>();
        TowerCost_Input_Array[1] = GameObject.Find("Line_Cost").transform.Find("Epic_Input").gameObject.GetComponent<InputField>();
        TowerCost_Input_Array[2] = GameObject.Find("Line_Cost").transform.Find("Unique_Input").gameObject.GetComponent<InputField>();
        TowerCost_Input_Array[3] = GameObject.Find("Line_Cost").transform.Find("Legendary_Input").gameObject.GetComponent<InputField>();

        //-----------------Custom Panel


    }

    void InputOnEditEnd_Collection () {

        //OnEditEnd
        TowerDamage_Input_Array[0].onEndEdit.AddListener(delegate {
            ValueVerification(TowerDamage_Input_Array[0], 1, 1000000);
        });
        TowerDamage_Input_Array[1].onEndEdit.AddListener(delegate {
            ValueVerification(TowerDamage_Input_Array[1], 1, 1000000);
        });
        TowerDamage_Input_Array[2].onEndEdit.AddListener(delegate {
            ValueVerification(TowerDamage_Input_Array[2], 1, 1000000);
        });
        TowerDamage_Input_Array[3].onEndEdit.AddListener(delegate {
            ValueVerification(TowerDamage_Input_Array[3], 1, 1000000);
        });


        TowerAttackSpeed_Input_Array[0].onEndEdit.AddListener(delegate {
            ValueVerification(TowerAttackSpeed_Input_Array[0], 0.1f, 10.0f);
        });
        TowerAttackSpeed_Input_Array[1].onEndEdit.AddListener(delegate {
            ValueVerification(TowerAttackSpeed_Input_Array[1], 0.1f, 10.0f);
        });
        TowerAttackSpeed_Input_Array[2].onEndEdit.AddListener(delegate {
            ValueVerification(TowerAttackSpeed_Input_Array[2], 0.1f, 10.0f);
        });
        TowerAttackSpeed_Input_Array[3].onEndEdit.AddListener(delegate {
            ValueVerification(TowerAttackSpeed_Input_Array[3], 0.1f, 10.0f);
        });


        TowerRange_Input_Array[0].onEndEdit.AddListener(delegate {
            ValueVerification(TowerRange_Input_Array[0], 1.0f, 10.0f);
        });
        TowerRange_Input_Array[1].onEndEdit.AddListener(delegate {
            ValueVerification(TowerRange_Input_Array[1], 1.0f, 10.0f);
        });
        TowerRange_Input_Array[2].onEndEdit.AddListener(delegate {
            ValueVerification(TowerRange_Input_Array[2], 1.0f, 10.0f);
        });
        TowerRange_Input_Array[3].onEndEdit.AddListener(delegate {
            ValueVerification(TowerRange_Input_Array[3], 1.0f, 10.0f);
        });


        TowerOption_1_Float_Input_Array[0].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_1_Float_Input_Array[0], Option1_Float_Min, Option1_Float_Max);
        });
        TowerOption_1_Float_Input_Array[1].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_1_Float_Input_Array[1], Option1_Float_Min, Option1_Float_Max);
        });
        TowerOption_1_Float_Input_Array[2].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_1_Float_Input_Array[2], Option1_Float_Min, Option1_Float_Max);
        });
        TowerOption_1_Float_Input_Array[3].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_1_Float_Input_Array[3], Option1_Float_Min, Option1_Float_Max);
        });


        TowerOption_1_Int_Input_Array[0].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_1_Int_Input_Array[0], Option1_Int_Min, Option1_Int_Max);
        });
        TowerOption_1_Int_Input_Array[1].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_1_Int_Input_Array[1], Option1_Int_Min, Option1_Int_Max);
        });
        TowerOption_1_Int_Input_Array[2].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_1_Int_Input_Array[2], Option1_Int_Min, Option1_Int_Max);
        });
        TowerOption_1_Int_Input_Array[3].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_1_Int_Input_Array[3], Option1_Int_Min, Option1_Int_Max);
        });


        TowerOption_2_Input_Array[0].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_2_Input_Array[0], Option2_Min, Option2_Max);
        });
        TowerOption_2_Input_Array[1].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_2_Input_Array[1], Option2_Min, Option2_Max);
        });
        TowerOption_2_Input_Array[2].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_2_Input_Array[2], Option2_Min, Option2_Max);
        });
        TowerOption_2_Input_Array[3].onEndEdit.AddListener(delegate {
            ValueVerification(TowerOption_2_Input_Array[3], Option2_Min, Option2_Max);
        });

        TowerCost_Input_Array[0].onEndEdit.AddListener(delegate {
            ValueVerification(TowerCost_Input_Array[0], 1, 1000000);
        });
        TowerCost_Input_Array[1].onEndEdit.AddListener(delegate {
            ValueVerification(TowerCost_Input_Array[1], 1, 1000000);
        });
        TowerCost_Input_Array[2].onEndEdit.AddListener(delegate {
            ValueVerification(TowerCost_Input_Array[2], 1, 1000000);
        });
        TowerCost_Input_Array[3].onEndEdit.AddListener(delegate {
            ValueVerification(TowerCost_Input_Array[3], 1, 1000000);
        });
        //OnEditEnd
    }

    void BtnClick_Collection () {
        ClamTower_Btn.onClick.AddListener(() => {

            if (Clicked_Btn == ClamTower_Btn)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_1_Sound, 1.0f);

            //이전 데이터 세이브
            TowerDataSave();

            //버튼 액션
            ClamTower_Btn.transform.position = new Vector3(Generic_Pos_X + 7.0f, ClamTower_Btn.transform.position.y, ClamTower_Btn.transform.position.z);
            ClamTower_Btn.GetComponent<RectTransform>().sizeDelta = Clicked_Size;
            ClamTower_Btn.GetComponent<Image>().color = Clicked_Color;

            if (Clicked_Btn != null ) {
                Clicked_Btn.transform.position = new Vector3(Generic_Pos_X, Clicked_Btn.transform.position.y, Clicked_Btn.transform.position.z);
                Clicked_Btn.GetComponent<RectTransform>().sizeDelta = Generic_Size;
                Clicked_Btn.GetComponent<Image>().color = Generic_Color;
            }
            //

            //---로컬 데이터 가져오기
            //이미지 교체
            Tower_Image.sprite = m_ClamTower_Sprite;
            //타워 이름 변경
            TowerName_Text.text = local_ClamTower.UI_InfoNickname;
            //타워 설명 변경
            TowerContents_Text.text = local_ClamTower.UI_InfoContents;
            //타워 타입
            TowerType_Text.text = TowerStatus.TypeReturnString(local_ClamTower.m_Type);
            //타겟
            TowerTarget_Text.text = TowerStatus.TargetReturnString(local_ClamTower.m_Target);

            for (int i = 0; i < 4; i++) {
                TowerDamage_Input_Array[i].text = "";
                TowerAttackSpeed_Input_Array[i].text = "";
                TowerRange_Input_Array[i].text = "";
                TowerCost_Input_Array[i].text = "";

                //공격력
                TowerDamage_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_ClamTower.m_Damage[i].ToString();
                //공격속도
                TowerAttackSpeed_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_ClamTower.m_AttackSpeed[i].ToString();
                //범위
                TowerRange_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_ClamTower.m_Range[i].ToString();
                //범위 활성화
                if (Clicked_Btn == CrabTower_Btn) {
                    TowerRange_Input_Array[i].gameObject.GetComponent<Image>().color = new Color32(254, 254, 254, 255);
                    TowerRange_Input_Array[i].GetComponent<InputField>().enabled = true;
                }
                //코스트
                TowerCost_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_ClamTower.m_Cost[i].ToString();
            }

            //옵션1 비활성
            if (OptionLine_1_Float.activeSelf == true){
                OptionLine_1_Float.SetActive(false);
            }

            if(OptionLine_1_Int.activeSelf == true){
                OptionLine_1_Int.SetActive(false);
            }


            //None
            if (OptionLine_2.activeSelf == true)
                OptionLine_2.SetActive(false);

            //임시저장 변수에 값 존재 확인 후 대입
            TempDataFill(TowerName.Clam);





            Clicked_Btn = ClamTower_Btn;
        });

        CrabTower_Btn.onClick.AddListener(() => {

            if (Clicked_Btn == CrabTower_Btn)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_1_Sound, 1.0f);

            //이전 데이터 세이브
            TowerDataSave();

            CrabTower_Btn.transform.position = new Vector3(Generic_Pos_X + 7.0f, CrabTower_Btn.transform.position.y, CrabTower_Btn.transform.position.z);
            CrabTower_Btn.GetComponent<RectTransform>().sizeDelta = Clicked_Size;
            CrabTower_Btn.GetComponent<Image>().color = Clicked_Color;

            if (Clicked_Btn != null) {

                Clicked_Btn.transform.position = new Vector3(Generic_Pos_X, Clicked_Btn.transform.position.y, Clicked_Btn.transform.position.z);
                Clicked_Btn.GetComponent<RectTransform>().sizeDelta = Generic_Size;
                Clicked_Btn.GetComponent<Image>().color = Generic_Color;
            }

            Tower_Image.sprite = m_CrabTower_Sprite;
            TowerName_Text.text = local_CrabTower.UI_InfoNickname;
            TowerContents_Text.text = local_CrabTower.UI_InfoContents;
            TowerType_Text.text = TowerStatus.TypeReturnString(local_CrabTower.m_Type);
            TowerTarget_Text.text = TowerStatus.TargetReturnString(local_CrabTower.m_Target);


            for (int i = 0; i < 4; i++) {

                TowerDamage_Input_Array[i].text = "";
                TowerAttackSpeed_Input_Array[i].text = "";
                TowerRange_Input_Array[i].text = "";
                TowerCost_Input_Array[i].text = "";

                //공격력
                TowerDamage_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_CrabTower.m_Damage[i].ToString();

                //공격속도
                TowerAttackSpeed_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_CrabTower.m_AttackSpeed[i].ToString();

                //범위
                TowerRange_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_CrabTower.m_Range.ToString();

                //범위 Input 비활성 및 색상변경
                TowerRange_Input_Array[i].gameObject.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
                TowerRange_Input_Array[i].GetComponent<InputField>().enabled = false;

                //코스트
                TowerCost_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_CrabTower.m_Cost[i].ToString();
            }

            //옵션1 비활성
            if (OptionLine_1_Float.activeSelf == true){
                OptionLine_1_Float.SetActive(false);
            }

            if (OptionLine_1_Int.activeSelf == true){
                OptionLine_1_Int.SetActive(false);
            }

            //옵션2 비활성
            if (OptionLine_2.activeSelf == true)
                OptionLine_2.SetActive(false);

            TempDataFill(TowerName.Crab);



            Clicked_Btn = CrabTower_Btn;
        });

        ElectricTower_Btn.onClick.AddListener(() => {

            if (Clicked_Btn == ElectricTower_Btn)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_1_Sound, 1.0f);

            //이전 데이터 세이브
            TowerDataSave();

            ElectricTower_Btn.transform.position = new Vector3(Generic_Pos_X + 7.0f, ElectricTower_Btn.transform.position.y, ElectricTower_Btn.transform.position.z);
            ElectricTower_Btn.GetComponent<RectTransform>().sizeDelta = Clicked_Size;
            ElectricTower_Btn.GetComponent<Image>().color = Clicked_Color;

            if (Clicked_Btn != null) {

                Clicked_Btn.transform.position = new Vector3(Generic_Pos_X, Clicked_Btn.transform.position.y, Clicked_Btn.transform.position.z);
                Clicked_Btn.GetComponent<RectTransform>().sizeDelta = Generic_Size;
                Clicked_Btn.GetComponent<Image>().color = Generic_Color;
            }

            Tower_Image.sprite = m_ElectricEelTower_Sprite;
            TowerName_Text.text = local_ElectricEelTower.UI_InfoNickname;
            TowerContents_Text.text = local_ElectricEelTower.UI_InfoContents;
            TowerType_Text.text = TowerStatus.TypeReturnString(local_ElectricEelTower.m_Type);
            TowerTarget_Text.text = TowerStatus.TargetReturnString(local_ElectricEelTower.m_Target);

            for (int i = 0; i < 4; i++) {

                TowerDamage_Input_Array[i].text = "";
                TowerAttackSpeed_Input_Array[i].text = "";
                TowerRange_Input_Array[i].text = "";
                TowerOption_1_Int_Input_Array[i].text = "";
                TowerCost_Input_Array[i].text = "";

                //공격력
                TowerDamage_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_ElectricEelTower.m_Damage[i].ToString();

                //공격속도
                TowerAttackSpeed_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_ElectricEelTower.m_AttackSpeed[i].ToString();

                //범위
                TowerRange_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_ElectricEelTower.m_Range[i].ToString();
                
                //범위 활성화
                if (Clicked_Btn == CrabTower_Btn) {
                    TowerRange_Input_Array[i].gameObject.GetComponent<Image>().color = new Color32(254, 254, 254, 255);
                    TowerRange_Input_Array[i].GetComponent<InputField>().enabled = true;
                }

                //체인 공격 대상
                TowerOption_1_Int_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_ElectricEelTower.m_ChainCnt[i].ToString();

                //코스트
                TowerCost_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_ElectricEelTower.m_Cost[i].ToString();

            }

            //옵션1 Float 비활성
            OptionLine_1_Float.SetActive(false);

            //옵션1 Int 활성
            if (OptionLine_1_Int.activeSelf == false)
                OptionLine_1_Int.SetActive(true);
            OptionLine_1_Int_Title.text = "연쇄공격 수";
            OptionLine_1_Int_Image.sprite = m_ChainCnt_Sprite;

            if (OptionLine_1_Int.activeSelf == false)
                OptionLine_1_Int.SetActive(true);
            OptionLine_1_Int_Title.text = "연쇄공격 수";
            OptionLine_1_Int_Image.sprite = m_ChainCnt_Sprite;

            //옵션1 인풋 범위 지정
            Option1_Int_Min = 1;
            Option1_Int_Max = 10;

           
            //옵션2 비활성
            if (OptionLine_2.activeSelf == true)
                OptionLine_2.SetActive(false);

            TempDataFill(TowerName.ElectricEel);

            Clicked_Btn = ElectricTower_Btn;
        });

        PufferTower_Btn.onClick.AddListener(() => {

            if (Clicked_Btn == PufferTower_Btn)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_1_Sound, 1.0f);

            //이전 데이터 세이브
            TowerDataSave();

            PufferTower_Btn.transform.position = new Vector3(Generic_Pos_X + 7.0f, PufferTower_Btn.transform.position.y, PufferTower_Btn.transform.position.z);
            PufferTower_Btn.GetComponent<RectTransform>().sizeDelta = Clicked_Size;
            PufferTower_Btn.GetComponent<Image>().color = Clicked_Color;

            if (Clicked_Btn != null) {

                Clicked_Btn.transform.position = new Vector3(Generic_Pos_X, Clicked_Btn.transform.position.y, Clicked_Btn.transform.position.z);
                Clicked_Btn.GetComponent<RectTransform>().sizeDelta = Generic_Size;
                Clicked_Btn.GetComponent<Image>().color = Generic_Color;
            }

            Tower_Image.sprite = m_PufferTower_Sprite;
            TowerName_Text.text = local_PufferTower.UI_InfoNickname;
            TowerContents_Text.text = local_PufferTower.UI_InfoContents;
            TowerType_Text.text = TowerStatus.TypeReturnString(local_PufferTower.m_Type);
            TowerTarget_Text.text = TowerStatus.TargetReturnString(local_PufferTower.m_Target);


          

            for (int i = 0; i < 4; i++) {

                TowerDamage_Input_Array[i].text = "";
                TowerAttackSpeed_Input_Array[i].text = "";
                TowerRange_Input_Array[i].text = "";
                TowerOption_1_Float_Input_Array[i].text = "";
                TowerCost_Input_Array[i].text = "";


                //공격력
                TowerDamage_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_PufferTower.m_Damage[i].ToString();
                //공격속도
                TowerAttackSpeed_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_PufferTower.m_AttackSpeed[i].ToString();
                //범위
                TowerRange_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_PufferTower.m_Range[i].ToString();
                //범위 활성화
                if (Clicked_Btn == CrabTower_Btn) {
                    TowerRange_Input_Array[i].gameObject.GetComponent<Image>().color = new Color32(254, 254, 254, 255);
                    TowerRange_Input_Array[i].GetComponent<InputField>().enabled = true;
                }
                //빙결 시간
                TowerOption_1_Float_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_PufferTower.m_FreezeTime[i].ToString();
                //코스트
                TowerCost_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_PufferTower.m_Cost[i].ToString();
            }

            //옵션1 Int 비활성
            if (OptionLine_1_Int.activeSelf == true)
                OptionLine_1_Int.SetActive(false);
            
            //옵션1 Float 활성
            if (OptionLine_1_Float.activeSelf == false) 
                OptionLine_1_Float.SetActive(true);
            OptionLine_1_Float_Title.text = "디버프 시간";
            OptionLine_1_Float_Title.fontSize = 20;
            OptionLine_1_Float_Image.sprite = m_DeBuffTime_Sprite;

            //옵션1 인풋 범위 지정
            Option1_Float_Min = 0.1f;
            Option1_Float_Max = 100.0f;

            //옵션2 비활성
            if (OptionLine_2.activeSelf == true)
                OptionLine_2.SetActive(false);

            TempDataFill(TowerName.Puffer);

            Clicked_Btn = PufferTower_Btn;
        });

        PoisonPufferTower_Btn.onClick.AddListener(() => {
            
            if (Clicked_Btn == PoisonPufferTower_Btn)
                return;

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.PageFlip_1_Sound, 1.0f);

            //이전 데이터 세이브
            TowerDataSave();

            PoisonPufferTower_Btn.transform.position = new Vector3(Generic_Pos_X + 7.0f, PoisonPufferTower_Btn.transform.position.y, PoisonPufferTower_Btn.transform.position.z);
            PoisonPufferTower_Btn.GetComponent<RectTransform>().sizeDelta = Clicked_Size;
            PoisonPufferTower_Btn.GetComponent<Image>().color = Clicked_Color;

            if (Clicked_Btn != null) {

                Clicked_Btn.transform.position = new Vector3(Generic_Pos_X, Clicked_Btn.transform.position.y, Clicked_Btn.transform.position.z);
                Clicked_Btn.GetComponent<RectTransform>().sizeDelta = Generic_Size;
                Clicked_Btn.GetComponent<Image>().color = Generic_Color;
            }

            Tower_Image.sprite = m_PoisonPufferTower_Sprite;
            TowerName_Text.text = local_PoisonPufferTower.UI_InfoNickname;
            TowerContents_Text.text = local_PoisonPufferTower.UI_InfoContents;
            TowerType_Text.text = TowerStatus.TypeReturnString(local_PoisonPufferTower.m_Type);
            TowerTarget_Text.text = TowerStatus.TargetReturnString(local_PoisonPufferTower.m_Target);


            for (int i = 0; i < 4; i++) {

                TowerDamage_Input_Array[i].text = "";
                TowerAttackSpeed_Input_Array[i].text = "";
                TowerRange_Input_Array[i].text = "";
                TowerOption_1_Float_Input_Array[i].text = "";
                TowerOption_2_Input_Array[i].text = "";
                TowerCost_Input_Array[i].text = "";


                //공격력
                TowerDamage_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_PoisonPufferTower.m_Damage[i].ToString();

                //공격속도
                TowerAttackSpeed_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_PoisonPufferTower.m_AttackSpeed[i].ToString();

                //범위
                TowerRange_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_PoisonPufferTower.m_Range[i].ToString();

                //범위 활성화
                if (Clicked_Btn == CrabTower_Btn) {
                    TowerRange_Input_Array[i].gameObject.GetComponent<Image>().color = new Color32(254, 254, 254, 255);
                    TowerRange_Input_Array[i].GetComponent<InputField>().enabled = true;
                }

                //독 지속시간
                TowerOption_1_Float_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_PoisonPufferTower.m_PoisonTime[i].ToString();

                //독 데미지
                TowerOption_2_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_PoisonPufferTower.m_PoisonDamage[i].ToString();

                //코스트
                TowerCost_Input_Array[i].transform.Find("Placeholder").GetComponent<Text>().text = local_PoisonPufferTower.m_Cost[i].ToString();

            }

            //옵션1 Int 비활성
            if (OptionLine_1_Int.activeSelf == true)
                OptionLine_1_Int.SetActive(false);
            
            //옵션1 Float 활성
            if (OptionLine_1_Float.activeSelf == false)
                OptionLine_1_Float.SetActive(true);
            OptionLine_1_Float_Title.text = "디버프 시간";
            OptionLine_1_Float_Title.fontSize = 20;
            OptionLine_1_Float_Image.sprite = m_DeBuffTime_Sprite;

            //옵션1 인풋 범위 지정
            Option1_Float_Min = 0.1f;
            Option1_Float_Max = 100.0f;


            //옵션2 활성
            if (OptionLine_2.activeSelf == false)
                OptionLine_2.SetActive(true);
            OptionLine_2_Title.text = "디버프 데미지";
            OptionLine_2_Title.fontSize = 20;
            OptionLine_2_Image.sprite = m_PoisonDamage_Sprite;

            //옵션2 인풋 범위 지정
            Option2_Min = 1;
            Option2_Max = 1000000;


            TempDataFill(TowerName.PoisonPuffer);

            Clicked_Btn = PoisonPufferTower_Btn;
        });

        ShirmpTower_Btn.onClick.AddListener(() => {
            
        });

        Finish_Btn.onClick.AddListener(() => {

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Button2_Sound, 1.0f);

            TowerDataSave();


            if (TempDataCheck()){

                //다음 스탭으로 넘어가기
                EditorMgr.g_Inst.m_EditStep++;
                EditorMgr.g_Inst.m_Blink_Target = EditorMgr.g_Inst.StepBar_MonsterSetting_Btn.gameObject;

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


                //EditorMgr.g_Inst.temp_Towers_Data = 
                //임시 타워들 변수들을 JsonUtility.ToJson 이용하여 string 타입으로 이뤄진 EditMgr 스크립트의 TowerData변수에 변환하고 저장하기
                EditorMgr.g_Inst.temp_Towers_Data.ClamTower = JsonUtility.ToJson(temp_ClamTower);
                EditorMgr.g_Inst.temp_Towers_Data.CrabTower = JsonUtility.ToJson(temp_CrabTower);
                EditorMgr.g_Inst.temp_Towers_Data.ElectricEelTower = JsonUtility.ToJson(temp_ElectricEelTower);
                EditorMgr.g_Inst.temp_Towers_Data.PufferTower = JsonUtility.ToJson(temp_PufferTower);
                EditorMgr.g_Inst.temp_Towers_Data.PoisonPufferTower = JsonUtility.ToJson(temp_PoisonPufferTower);


               
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



    void Awake () {
        g_Inst = this;

        ObjectLoad();
        ResourceLoad();
        BtnClick_Collection();
        InputOnEditEnd_Collection();
    }


    // Start is called before the first frame update
    void Start()
    {
        GetLocalTowerData();

        //고정데이터 작업
        FixedDataFill();






        Generic_Pos_X = ClamTower_Btn.transform.position.x;

        //첫번쨰 버튼 클릭 Invoke();
        ClamTower_Btn.onClick.Invoke();



    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            temp_ClamTower.m_Damage = Enumerable.Repeat<int>(1, 4).ToArray<int>();
            temp_ClamTower.m_AttackSpeed = Enumerable.Repeat<float>(11, 4).ToArray<float>();
            temp_ClamTower.m_Range = Enumerable.Repeat<float>(1, 4).ToArray<float>();
            temp_ClamTower.m_Cost = Enumerable.Repeat<int>(1, 4).ToArray<int>();

            temp_CrabTower.m_Damage = Enumerable.Repeat<int>(1, 4).ToArray<int>();
            temp_CrabTower.m_AttackSpeed = Enumerable.Repeat<float>(1, 4).ToArray<float>();
            temp_CrabTower.m_Cost = Enumerable.Repeat<int>(1, 4).ToArray<int>();

            temp_ElectricEelTower.m_Damage = Enumerable.Repeat<int>(1, 4).ToArray<int>();
            temp_ElectricEelTower.m_AttackSpeed = Enumerable.Repeat<float>(1, 4).ToArray<float>();
            temp_ElectricEelTower.m_Range = Enumerable.Repeat<float>(1, 4).ToArray<float>();
            temp_ElectricEelTower.m_ChainCnt = Enumerable.Repeat<int>(1, 4).ToArray<int>();
            temp_ElectricEelTower.m_Cost = Enumerable.Repeat<int>(1, 4).ToArray<int>();

            temp_PufferTower.m_Damage = Enumerable.Repeat<int>(1, 4).ToArray<int>();
            temp_PufferTower.m_AttackSpeed = Enumerable.Repeat<float>(1, 4).ToArray<float>();
            temp_PufferTower.m_Range = Enumerable.Repeat<float>(1, 4).ToArray<float>();
            temp_PufferTower.m_FreezeTime = Enumerable.Repeat<float>(1, 4).ToArray<float>();
            temp_PufferTower.m_Cost = Enumerable.Repeat<int>(1, 4).ToArray<int>();

            temp_PoisonPufferTower.m_Damage = Enumerable.Repeat<int>(1, 4).ToArray<int>();
            temp_PoisonPufferTower.m_AttackSpeed = Enumerable.Repeat<float>(1, 4).ToArray<float>();
            temp_PoisonPufferTower.m_Range = Enumerable.Repeat<float>(1, 4).ToArray<float>();
            temp_PoisonPufferTower.m_PoisonTime = Enumerable.Repeat<float>(1, 4).ToArray<float>();
            temp_PoisonPufferTower.m_PoisonDamage = Enumerable.Repeat<int>(1, 4).ToArray<int>();
            temp_PoisonPufferTower.m_Cost = Enumerable.Repeat<int>(1, 4).ToArray<int>();


            for(int i =0; i < 4; i++)
            {
                if(TowerDamage_Input_Array[i].IsActive() == true)
                    TowerDamage_Input_Array[i].text = "1";

                if (TowerAttackSpeed_Input_Array[i].IsActive() == true)
                    TowerAttackSpeed_Input_Array[i].text = "1";

                if (TowerRange_Input_Array[i].IsActive() == true)
                    TowerRange_Input_Array[i].text = "1";

                if (TowerOption_1_Float_Input_Array[i].IsActive() == true)
                    TowerOption_1_Float_Input_Array[i].text = "1";

                if (TowerOption_2_Input_Array[i].IsActive() == true)
                    TowerOption_2_Input_Array[i].text = "1";

                if (TowerCost_Input_Array[i].IsActive() == true)
                    TowerCost_Input_Array[i].text = "1";
            }


        }

    }


    void GetLocalTowerData () {

        //타워 복원
        local_ClamTower = JsonUtility.FromJson<ClamTower>(EditorMgr.g_Inst.local_Towers_Data.ClamTower);
        local_CrabTower = JsonUtility.FromJson<CrabTower>(EditorMgr.g_Inst.local_Towers_Data.CrabTower);
        local_ElectricEelTower = JsonUtility.FromJson<ElectricEelTower>(EditorMgr.g_Inst.local_Towers_Data.ElectricEelTower);
        local_PufferTower = JsonUtility.FromJson<PufferTower>(EditorMgr.g_Inst.local_Towers_Data.PufferTower);
        local_PoisonPufferTower = JsonUtility.FromJson<PoisonPufferTower>(EditorMgr.g_Inst.local_Towers_Data.PoisonPufferTower);


       
        //임시변수 초기화
        temp_ClamTower.m_Damage = Enumerable.Repeat<int>(-1, 4).ToArray<int>();
        temp_ClamTower.m_AttackSpeed = Enumerable.Repeat<float>(-1, 4).ToArray<float>();
        temp_ClamTower.m_Range = Enumerable.Repeat<float>(-1, 4).ToArray<float>();
        temp_ClamTower.m_Cost = Enumerable.Repeat<int>(-1, 4).ToArray<int>();

        temp_CrabTower.m_Damage = Enumerable.Repeat<int>(-1, 4).ToArray<int>();
        temp_CrabTower.m_AttackSpeed = Enumerable.Repeat<float>(-1, 4).ToArray<float>();
        temp_CrabTower.m_Cost = Enumerable.Repeat<int>(-1, 4).ToArray<int>();

        temp_ElectricEelTower.m_Damage = Enumerable.Repeat<int>(-1, 4).ToArray<int>();
        temp_ElectricEelTower.m_AttackSpeed = Enumerable.Repeat<float>(-1, 4).ToArray<float>();
        temp_ElectricEelTower.m_Range = Enumerable.Repeat<float>(-1, 4).ToArray<float>();
        temp_ElectricEelTower.m_ChainCnt = Enumerable.Repeat<int>(-1, 4).ToArray<int>();
        temp_ElectricEelTower.m_Cost = Enumerable.Repeat<int>(-1, 4).ToArray<int>();

        temp_PufferTower.m_Damage = Enumerable.Repeat<int>(-1, 4).ToArray<int>();
        temp_PufferTower.m_AttackSpeed = Enumerable.Repeat<float>(-1, 4).ToArray<float>();
        temp_PufferTower.m_Range = Enumerable.Repeat<float>(-1, 4).ToArray<float>();
        temp_PufferTower.m_FreezeTime = Enumerable.Repeat<float>(-1, 4).ToArray<float>();
        temp_PufferTower.m_Cost = Enumerable.Repeat<int>(-1, 4).ToArray<int>();

        temp_PoisonPufferTower.m_Damage = Enumerable.Repeat<int>(-1, 4).ToArray<int>();
        temp_PoisonPufferTower.m_AttackSpeed = Enumerable.Repeat<float>(-1, 4).ToArray<float>();
        temp_PoisonPufferTower.m_Range = Enumerable.Repeat<float>(-1, 4).ToArray<float>();
        temp_PoisonPufferTower.m_PoisonTime = Enumerable.Repeat<float>(-1, 4).ToArray<float>();
        temp_PoisonPufferTower.m_PoisonDamage = Enumerable.Repeat<int>(-1, 4).ToArray<int>();
        temp_PoisonPufferTower.m_Cost = Enumerable.Repeat<int>(-1, 4).ToArray<int>();
       

        if(GlobalGameData.m_GameMode == GameMode.Test)
        {
            temp_ClamTower = JsonUtility.FromJson<ClamTower>(EditorMgr.g_Inst.temp_Towers_Data.ClamTower);
            temp_CrabTower = JsonUtility.FromJson<CrabTower>(EditorMgr.g_Inst.temp_Towers_Data.CrabTower);
            temp_ElectricEelTower = JsonUtility.FromJson<ElectricEelTower>(EditorMgr.g_Inst.temp_Towers_Data.ElectricEelTower);
            temp_PufferTower = JsonUtility.FromJson<PufferTower>(EditorMgr.g_Inst.temp_Towers_Data.PufferTower);
            temp_PoisonPufferTower = JsonUtility.FromJson<PoisonPufferTower>(EditorMgr.g_Inst.temp_Towers_Data.PoisonPufferTower);


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






    public void TowerDataSave () {

        if (Clicked_Btn == ClamTower_Btn) {
            for (int i = 0; i < 4; i++) {
                temp_ClamTower.m_Damage[i] = (TowerDamage_Input_Array[i].text == "" ? -1 : int.Parse(TowerDamage_Input_Array[i].text.Replace(",","")));
                temp_ClamTower.m_AttackSpeed[i] = (TowerAttackSpeed_Input_Array[i].text == "" ? -1 : float.Parse(TowerAttackSpeed_Input_Array[i].text.Replace(",", "")));
                temp_ClamTower.m_Range[i] = (TowerRange_Input_Array[i].text == "" ? -1 : float.Parse(TowerRange_Input_Array[i].text.Replace(",", "")));
                temp_ClamTower.m_Cost[i] = (TowerCost_Input_Array[i].text == "" ? -1 : int.Parse(TowerCost_Input_Array[i].text.Replace(",", "")));

            }
        }
        else if (Clicked_Btn == CrabTower_Btn) {
            for (int i = 0; i < 4; i++) {
                temp_CrabTower.m_Damage[i] = (TowerDamage_Input_Array[i].text == "" ? -1 : int.Parse(TowerDamage_Input_Array[i].text.Replace(",", "")));
                temp_CrabTower.m_AttackSpeed[i] = (TowerAttackSpeed_Input_Array[i].text == "" ? -1 : float.Parse(TowerAttackSpeed_Input_Array[i].text.Replace(",", "")));
                temp_CrabTower.m_Cost[i] = (TowerCost_Input_Array[i].text == "" ? -1 : int.Parse(TowerCost_Input_Array[i].text.Replace(",", "")));

            }
        }
        else if (Clicked_Btn == ElectricTower_Btn) {
            for (int i = 0; i < 4; i++) {
                temp_ElectricEelTower.m_Damage[i] = (TowerDamage_Input_Array[i].text == "" ? -1 : int.Parse(TowerDamage_Input_Array[i].text.Replace(",", "")));
                temp_ElectricEelTower.m_AttackSpeed[i] = (TowerAttackSpeed_Input_Array[i].text == "" ? -1 : float.Parse(TowerAttackSpeed_Input_Array[i].text.Replace(",", "")));
                temp_ElectricEelTower.m_Range[i] = (TowerRange_Input_Array[i].text == "" ? -1 : float.Parse(TowerRange_Input_Array[i].text.Replace(",", "")));
                temp_ElectricEelTower.m_ChainCnt[i] = (TowerOption_1_Int_Input_Array[i].text == "" ? -1 : int.Parse(TowerOption_1_Int_Input_Array[i].text.Replace(",", "")));
                temp_ElectricEelTower.m_Cost[i] = (TowerCost_Input_Array[i].text == "" ? -1 : int.Parse(TowerCost_Input_Array[i].text.Replace(",", "")));

            }
        }
        else if (Clicked_Btn == PufferTower_Btn) {
            for (int i = 0; i < 4; i++) {
                temp_PufferTower.m_Damage[i] = (TowerDamage_Input_Array[i].text == "" ? -1 : int.Parse(TowerDamage_Input_Array[i].text.Replace(",", "")));
                temp_PufferTower.m_AttackSpeed[i] = (TowerAttackSpeed_Input_Array[i].text == "" ? -1 : float.Parse(TowerAttackSpeed_Input_Array[i].text.Replace(",", "")));
                temp_PufferTower.m_Range[i] = (TowerRange_Input_Array[i].text == "" ? -1 : float.Parse(TowerRange_Input_Array[i].text.Replace(",", "")));
                temp_PufferTower.m_FreezeTime[i] = (TowerOption_1_Float_Input_Array[i].text == "" ? -1 : float.Parse(TowerOption_1_Float_Input_Array[i].text.Replace(",", "")));
                temp_PufferTower.m_Cost[i] = (TowerCost_Input_Array[i].text == "" ? -1 : int.Parse(TowerCost_Input_Array[i].text.Replace(",", "")));
            }
        }
        else if (Clicked_Btn == PoisonPufferTower_Btn) {
            for (int i = 0; i < 4; i++) {
                temp_PoisonPufferTower.m_Damage[i] = (TowerDamage_Input_Array[i].text == "" ? -1 : int.Parse(TowerDamage_Input_Array[i].text.Replace(",", "")));
                temp_PoisonPufferTower.m_AttackSpeed[i] = (TowerAttackSpeed_Input_Array[i].text == "" ? -1 : float.Parse(TowerAttackSpeed_Input_Array[i].text.Replace(",", "")));
                temp_PoisonPufferTower.m_Range[i] = (TowerRange_Input_Array[i].text == "" ? -1 : float.Parse(TowerRange_Input_Array[i].text.Replace(",", "")));
                temp_PoisonPufferTower.m_PoisonTime[i] = (TowerOption_1_Float_Input_Array[i].text == "" ? -1 : float.Parse(TowerOption_1_Float_Input_Array[i].text.Replace(",", "")));
                temp_PoisonPufferTower.m_PoisonDamage[i] = (TowerOption_2_Input_Array[i].text == "" ? -1 : int.Parse(TowerOption_2_Input_Array[i].text.Replace(",", "")));
                temp_PoisonPufferTower.m_Cost[i] = (TowerCost_Input_Array[i].text == "" ? -1 : int.Parse(TowerCost_Input_Array[i].text.Replace(",", "")));

            }
        }
    }

    void TempDataFill (TowerName _tower) {

        if (_tower == TowerName.Clam) {
            for (int i = 0; i < 4; i++) {

                //EditMgr에 데이터가 없다면,  temp_ClamTower 값 넣기
                //값이 있다면, EditMgr의 값으로 넣기
                TowerDamage_Input_Array[i].text = temp_ClamTower.m_Damage[i] < 0 ? "" : temp_ClamTower.m_Damage[i].ToString();
                TowerAttackSpeed_Input_Array[i].text = temp_ClamTower.m_AttackSpeed[i] < 0 ? "" : temp_ClamTower.m_AttackSpeed[i].ToString();
                TowerRange_Input_Array[i].text = temp_ClamTower.m_Range[i] < 0 ? "" : temp_ClamTower.m_Range[i].ToString();
                TowerCost_Input_Array[i].text = temp_ClamTower.m_Cost[i] < 0 ? "" : temp_ClamTower.m_Cost[i].ToString();

                ValueVerification(TowerAttackSpeed_Input_Array[i], 0.1f, 10.0f);
                ValueVerification(TowerRange_Input_Array[i], 1.0f, 10.0f);

                ValueVerification(TowerDamage_Input_Array[i], 1, 1000000);
                ValueVerification(TowerCost_Input_Array[i], 1, 1000000);

            }
        }
        else if (_tower == TowerName.Crab) {
            for (int i = 0; i < 4; i++) {
                TowerDamage_Input_Array[i].text = temp_CrabTower.m_Damage[i] < 0 ? "" : temp_CrabTower.m_Damage[i].ToString();
                TowerAttackSpeed_Input_Array[i].text = temp_CrabTower.m_AttackSpeed[i] < 0 ? "" : temp_CrabTower.m_AttackSpeed[i].ToString();
                TowerCost_Input_Array[i].text = temp_CrabTower.m_Cost[i] < 0 ? "" : temp_CrabTower.m_Cost[i].ToString();

                ValueVerification(TowerAttackSpeed_Input_Array[i], 0.1f, 10.0f);
                ValueVerification(TowerDamage_Input_Array[i], 1, 1000000);
                ValueVerification(TowerCost_Input_Array[i], 1, 1000000);
            }
        }
        else if (_tower == TowerName.ElectricEel) {
            for (int i = 0; i < 4; i++) {
                TowerDamage_Input_Array[i].text = temp_ElectricEelTower.m_Damage[i] < 0 ? "" : temp_ElectricEelTower.m_Damage[i].ToString();
                TowerAttackSpeed_Input_Array[i].text = temp_ElectricEelTower.m_AttackSpeed[i] < 0 ? "" : temp_ElectricEelTower.m_AttackSpeed[i].ToString();
                TowerRange_Input_Array[i].text = temp_ElectricEelTower.m_Range[i] < 0 ? "" : temp_ElectricEelTower.m_Range[i].ToString();
                TowerOption_1_Int_Input_Array[i].text = temp_ElectricEelTower.m_ChainCnt[i] < 0 ? "" : temp_ElectricEelTower.m_ChainCnt[i].ToString();
                TowerCost_Input_Array[i].text = temp_ElectricEelTower.m_Cost[i] < 0 ? "" : temp_ElectricEelTower.m_Cost[i].ToString();

                ValueVerification(TowerAttackSpeed_Input_Array[i], 0.1f, 10.0f);
                ValueVerification(TowerRange_Input_Array[i], 0.1f, 10.0f);

                ValueVerification(TowerDamage_Input_Array[i], 1, 1000000);
                ValueVerification(TowerOption_1_Int_Input_Array[i], Option1_Int_Min, Option1_Int_Max);
                ValueVerification(TowerCost_Input_Array[i], 1, 1000000);

            }
        }
        else if (_tower == TowerName.Puffer) {
            for (int i = 0; i < 4; i++) {
                TowerDamage_Input_Array[i].text = temp_PufferTower.m_Damage[i] < 0 ? "" : temp_PufferTower.m_Damage[i].ToString();
                TowerAttackSpeed_Input_Array[i].text = temp_PufferTower.m_AttackSpeed[i] < 0 ? "" : temp_PufferTower.m_AttackSpeed[i].ToString();
                TowerRange_Input_Array[i].text = temp_PufferTower.m_Range[i] < 0 ? "" : temp_PufferTower.m_Range[i].ToString();
                TowerOption_1_Float_Input_Array[i].text = temp_PufferTower.m_FreezeTime[i] < 0 ? "" : temp_PufferTower.m_FreezeTime[i].ToString();
                TowerCost_Input_Array[i].text = temp_PufferTower.m_Cost[i] < 0 ? "" : temp_PufferTower.m_Cost[i].ToString();

                ValueVerification(TowerAttackSpeed_Input_Array[i], 0.1f, 10.0f);
                ValueVerification(TowerRange_Input_Array[i], 0.1f, 10.0f);
                ValueVerification(TowerOption_1_Float_Input_Array[i], 0.1f, 100.0f);


                ValueVerification(TowerDamage_Input_Array[i], 1, 1000000);
                ValueVerification(TowerCost_Input_Array[i], 1, 1000000);
            }
        }
        else if (_tower == TowerName.PoisonPuffer) {
            for (int i = 0; i < 4; i++) {
                TowerDamage_Input_Array[i].text = temp_PoisonPufferTower.m_Damage[i] < 0 ? "" : temp_PoisonPufferTower.m_Damage[i].ToString();
                TowerAttackSpeed_Input_Array[i].text = temp_PoisonPufferTower.m_AttackSpeed[i] < 0 ? "" : temp_PoisonPufferTower.m_AttackSpeed[i].ToString();
                TowerRange_Input_Array[i].text = temp_PoisonPufferTower.m_Range[i] < 0 ? "" : temp_PoisonPufferTower.m_Range[i].ToString();
                TowerOption_1_Float_Input_Array[i].text = temp_PoisonPufferTower.m_PoisonTime[i] < 0 ? "" : temp_PoisonPufferTower.m_PoisonTime[i].ToString();
                TowerOption_2_Input_Array[i].text = temp_PoisonPufferTower.m_PoisonDamage[i] < 0 ? "" : temp_PoisonPufferTower.m_PoisonDamage[i].ToString();
                TowerCost_Input_Array[i].text = temp_PoisonPufferTower.m_Cost[i] < 0 ? "" : temp_PoisonPufferTower.m_Cost[i].ToString();

                ValueVerification(TowerAttackSpeed_Input_Array[i], 0.1f, 10.0f);
                ValueVerification(TowerRange_Input_Array[i], 0.1f, 10.0f);
                ValueVerification(TowerOption_1_Float_Input_Array[i], 0.1f, 100.0f);


                ValueVerification(TowerDamage_Input_Array[i], 1, 1000000);
                ValueVerification(TowerCost_Input_Array[i], 1, 1000000);
                ValueVerification(TowerOption_2_Input_Array[i], Option2_Min, Option2_Max);

            }
        }
    }


    void FixedDataFill(){
        
        //타워 이름 및 설명
        temp_ClamTower.UI_InfoNickname = local_ClamTower.UI_InfoNickname;
        temp_ClamTower.UI_InfoContents = local_ClamTower.UI_InfoContents;

        temp_CrabTower.UI_InfoNickname = local_CrabTower.UI_InfoNickname;
        temp_CrabTower.UI_InfoContents = local_CrabTower.UI_InfoContents;

        temp_ElectricEelTower.UI_InfoNickname = local_ElectricEelTower.UI_InfoNickname;
        temp_ElectricEelTower.UI_InfoContents = local_ElectricEelTower.UI_InfoContents;

        temp_PufferTower.UI_InfoNickname = local_PufferTower.UI_InfoNickname;
        temp_PufferTower.UI_InfoContents = local_PufferTower.UI_InfoContents;

        temp_PoisonPufferTower.UI_InfoNickname = local_PoisonPufferTower.UI_InfoNickname;
        temp_PoisonPufferTower.UI_InfoContents = local_PoisonPufferTower.UI_InfoContents;


        //변경 불가능한 고정 데이터 입력 (로랑-범위, 망둥이-체인범위)
        temp_CrabTower.m_Range = local_CrabTower.m_Range;
        temp_ElectricEelTower.m_ChainRange = local_ElectricEelTower.m_ChainRange;



    }




    public bool TempDataCheck () {
        int cnt = 0;

        cnt += (Array.Exists(temp_ClamTower.m_Damage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_ClamTower.m_AttackSpeed, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_ClamTower.m_Range, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_ClamTower.m_Cost, x => x < 0) ? 1 : 0);

        
        cnt += (Array.Exists(temp_CrabTower.m_Damage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_CrabTower.m_AttackSpeed, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_CrabTower.m_Cost, x => x < 0) ? 1 : 0);

        cnt += (Array.Exists(temp_ElectricEelTower.m_Damage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_ElectricEelTower.m_AttackSpeed, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_ElectricEelTower.m_Range, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_ElectricEelTower.m_ChainCnt, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_ElectricEelTower.m_Cost, x => x < 0) ? 1 : 0);

        cnt += (Array.Exists(temp_PufferTower.m_Damage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_PufferTower.m_AttackSpeed, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_PufferTower.m_Range, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_PufferTower.m_FreezeTime, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_PufferTower.m_Cost, x => x < 0) ? 1 : 0);

        cnt += (Array.Exists(temp_PoisonPufferTower.m_Damage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_PoisonPufferTower.m_AttackSpeed, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_PoisonPufferTower.m_Range, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_PoisonPufferTower.m_PoisonTime, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_PoisonPufferTower.m_PoisonDamage, x => x < 0) ? 1 : 0);
        cnt += (Array.Exists(temp_PoisonPufferTower.m_Cost, x => x < 0) ? 1 : 0);

        return cnt <= 0;
    }
}
