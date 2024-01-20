using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

/*

 
*/

public enum TowerName {
    Clam,
    Crab,
    ElectricEel,
    Puffer,
    PoisonPuffer,
}

public enum TowerState{
    Inst,
    Idle,
    Attack,
}



public enum Grade {
    Rare,       //레어
    Epic,       //에픽
    Uniqe,      //유니크
    Legendary   //레전더리
}

public enum Target {
    First,      //가장 먼저 들어온 몬스터
    Random,     //랜덤
    Area,       //범위내 모든
    HighHP,
    Alliance,

}


public enum Type {
    Attack,     //공격타워
    Debuff,     //디버프 타워
    Buff,       //버프 타워
}


public class UI_InfoCell {
    public Image m_image;
    public Text m_title;
    public Text m_value;


    public UI_InfoCell (Image _image, Text _title, Text _value) {
        m_image = _image;
        m_title = _title;
        m_value = _value;
    }
}

//조개
public class ClamTower {

    public Type m_Type;                             //종류
    public Target m_Target;                         //공격순위
    public int[] m_Damage = new int[4];             //공격력
    public float[] m_Range = new float[4];          //범위
    public float[] m_AttackSpeed = new float[4];    //공격딜레이
    public int[] m_Cost = new int[4];               //가격


    public string UI_InfoNickname = "";
    public string UI_InfoContents = "";


}


//가재
public class CrabTower {

    public Type m_Type;                             //종류
    public Target m_Target;                         //공격순위
    public int[] m_Damage = new int[4];             //공격력
    public float m_Range;                           //범위 ※고정※
    public float[] m_AttackSpeed = new float[4];    //공격딜레이
    public int[] m_Cost = new int[4];               //가격

    public string UI_InfoNickname = "";
    public string UI_InfoContents = "";

}


//뱀장어
public class ElectricEelTower {

    public Type m_Type;                             //종류
    public Target m_Target;                         //공격순위
    public int[] m_Damage = new int[4];             //공격력
    public float[] m_Range = new float[4];          //범위
    public float[] m_AttackSpeed = new float[4];    //공격딜레이
    public int[] m_ChainCnt = new int[4];           //체인공격 대상 수
    public float m_ChainRange;                      //체인범위
    public int[] m_Cost = new int[4];               //가격


    public string UI_InfoNickname = "";
    public string UI_InfoContents = "";

}


//푸퍼
public class PufferTower {

    public Type m_Type;                             //종류
    public Target m_Target;                         //공격순위
    public int[] m_Damage = new int[4];             //공격력
    public float[] m_Range = new float[4];          //범위
    public float[] m_AttackSpeed = new float[4];    //공격딜레이
    public float[] m_FreezeTime = new float[4];     //빙결(디버프) 지속시간
    public int[] m_Cost = new int[4];               //가격


    public string UI_InfoNickname = "";
    public string UI_InfoContents = "";

}

//포이즌푸퍼
public class PoisonPufferTower {

    public Type m_Type;                             //종류
    public Target m_Target;                         //공격순위
    public int[] m_Damage = new int[4];             //공격력
    public float[] m_Range = new float[4];          //범위
    public float[] m_AttackSpeed = new float[4];    //공격딜레이
    public float[] m_PoisonTime = new float[4];     //독(디버프) 지속시간
    public int[] m_PoisonDamage = new int[4];       //독데미지
    public int[] m_Cost = new int[4];               //가격


    public string UI_InfoNickname = "";
    public string UI_InfoContents = "";

}


//새우
public class ShrimpTower {

    public Target m_Target = Target.First;
    public int m_Damage = 10;  //공격력
    public float m_Range = 5.0f;   //범위
    public float m_AttackSpeed = 1.0f; //공격딜레이

}


public class TowerList {

    public string ClamTower = JsonUtility.ToJson(new ClamTower());
    public string CrabTower = JsonUtility.ToJson(new CrabTower ());
    public string ElectricEelTower = JsonUtility.ToJson(new ElectricEelTower());
    public string PufferTower = JsonUtility.ToJson(new PufferTower());
    public string PoisonPufferTower = JsonUtility.ToJson(new PoisonPufferTower());
 


}


public class TowerStatus : MonoBehaviour
{
    public static TowerStatus Inst;
    HoverChangeText Hover;

    public ClamTower ClamTower = new ClamTower();
    public CrabTower CrabTower = new CrabTower();
    public ElectricEelTower ElectricEelTower = new ElectricEelTower();
    public PufferTower PufferTower = new PufferTower();
    public PoisonPufferTower PoisonPufferTower = new PoisonPufferTower();




    void Awake () {
        Inst = this;

        TowerListLoad();
        //타워로드
    }


    void Start () {
        Hover = GameObject.Find("Info_Upgrade_Btn").GetComponent<HoverChangeText>();


    }


    public int CostCheck (GameObject _obj) {

        if (_obj.GetComponent<ClamTowerMgr>() != null) {
            return ClamTower.m_Cost[(int)_obj.GetComponent<ClamTowerMgr>().m_Grade];
        }
        else if (_obj.GetComponent<CrabTowerMgr>() != null) {
            return CrabTower.m_Cost[(int)_obj.GetComponent<CrabTowerMgr>().m_Grade];
        }
        else if (_obj.GetComponent<ElectricTowerMgr>() != null) {
            return ElectricEelTower.m_Cost[(int)_obj.GetComponent<ElectricTowerMgr>().m_Grade];
        }
        else if (_obj.GetComponent<PufferTowerMgr>() != null) {
            return PufferTower.m_Cost[(int)_obj.GetComponent<PufferTowerMgr>().m_Grade];
        }
        else if (_obj.GetComponent<PoisonPufferTowerMgr>() != null) {
            return PoisonPufferTower.m_Cost[(int)_obj.GetComponent<PoisonPufferTowerMgr>().m_Grade];
        }

        return -1;
    }

    public string GradeReturnString (Grade _grade) {
        switch (_grade) {
            case Grade.Rare:
                return "레어";
            case Grade.Epic:
                return "에픽";
            case Grade.Uniqe:
                return "유니크";
            case Grade.Legendary:
                return "레전더리";
            default:
                return "";
        }
    }

    public static string TargetReturnString (Target _target) {
        switch (_target) {
            case Target.First:
                return "앞쪽";
            case Target.Area:
                return "범위";
            case Target.Random:
                return "랜덤";
            case Target.HighHP:
                return "높은체력";
            case Target.Alliance:
                return "아군";
            default:
                return "";
        }
    }

    public static string TypeReturnString (Type _type) {
        switch (_type) {
            case Type.Attack:
                return "공격";
            case Type.Debuff:
                return "디버프";
            case Type.Buff:
                return "버프";
            default:
                return "";
        }
    }


     
    public void TowerListLoad () {


        if (GlobalGameData.m_TowersData_TL != null) {

            ClamTower = JsonUtility.FromJson<ClamTower>(GlobalGameData.m_TowersData_TL.ClamTower);
            CrabTower = JsonUtility.FromJson<CrabTower>(GlobalGameData.m_TowersData_TL.CrabTower);
            ElectricEelTower = JsonUtility.FromJson<ElectricEelTower>(GlobalGameData.m_TowersData_TL.ElectricEelTower);
            PufferTower = JsonUtility.FromJson<PufferTower>(GlobalGameData.m_TowersData_TL.PufferTower);
            PoisonPufferTower = JsonUtility.FromJson<PoisonPufferTower>(GlobalGameData.m_TowersData_TL.PoisonPufferTower);

            Debug.Log("타워 로드 완료");
        }
        else {

            Debug.Log("데이터가 존재하지 않습니다.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }
    }










    public void TLSlotSet (int _rndNum, GameObject _slotArray) {

        //뽑은 숫자와 ToweName에 해당하는 TL 프리팹을 Slot에 넣기 (TowerName에 해당하는 프리팹 판별 메서드는 TowerStatus쪽에 구현)

        switch (_rndNum) {

            case 0:
                GameObject clam = Instantiate(GameMgr.g_GMGR_Inst.TL_Clam, _slotArray.transform);
                break;

            case 1:
                Instantiate(GameMgr.g_GMGR_Inst.TL_Crab, _slotArray.transform);
                break;

            case 2:
                Instantiate(GameMgr.g_GMGR_Inst.TL_ElectricEel, _slotArray.transform);
                break;

            case 3:
                Instantiate(GameMgr.g_GMGR_Inst.TL_Puffer, _slotArray.transform);
                break;

            case 4:
                Instantiate(GameMgr.g_GMGR_Inst.TL_PoisonPuffer, _slotArray.transform);
                break;

        }
    }




    public void TowerInfoPanelUpdate (GameObject _obj, UI_InfoCell[] _cellobjArray) {

        //--------공통 및 고정 UI
        _cellobjArray[0].m_title.text = "타입";
        _cellobjArray[0].m_image.sprite = GameMgr.g_GMGR_Inst.UI_Type_Sprite;

        _cellobjArray[1].m_title.text = "타겟";
        _cellobjArray[1].m_image.sprite = GameMgr.g_GMGR_Inst.UI_Target_Sprite;

        _cellobjArray[2].m_title.text = "공격력";
        _cellobjArray[2].m_image.sprite = GameMgr.g_GMGR_Inst.UI_Damage_Sprite;

        _cellobjArray[3].m_title.text = "공격속도";
        _cellobjArray[3].m_image.sprite = GameMgr.g_GMGR_Inst.UI_AttackSpeed_Sprite;
        //--------공통 및 고정 UI


        //조개타워(세르프)
        if (_obj.GetComponent<ClamTowerMgr>() != null) {
            ClamTowerMgr script = _obj.GetComponent<ClamTowerMgr>();
            Grade NextGrade = script.m_Grade + 1;

            //타워 등급_Text - 등급 및 색상
            GameMgr.g_GMGR_Inst.Info_TowerGrade_Text.text = GradeReturnString(script.m_Grade);
            GameMgr.g_GMGR_Inst.Info_TowerGrade_Text.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)script.m_Grade];

            //타워 이름_Text - 타워 이름
            GameMgr.g_GMGR_Inst.Info_TowerName_Text.text = ClamTower.UI_InfoNickname;
            //타워 설명_Text
            GameMgr.g_GMGR_Inst.Info_TowerContents_Text.text = ClamTower.UI_InfoContents;

            //타입
            _cellobjArray[0].m_value.text = TypeReturnString(ClamTower.m_Type);

            //타겟
            _cellobjArray[1].m_value.text = TargetReturnString(ClamTower.m_Target);

            //공격력
            _cellobjArray[2].m_value.text = ClamTower.m_Damage[(int)script.m_Grade].ToString();
            _cellobjArray[2].m_value.color = Color.white;

            //공격속도
            _cellobjArray[3].m_value.text = ClamTower.m_AttackSpeed[(int)script.m_Grade].ToString();
            _cellobjArray[3].m_value.color = Color.white;


            //None
            _cellobjArray[4].m_title.text = "-";
            _cellobjArray[4].m_image.sprite = GameMgr.g_GMGR_Inst.UI_None_Sprite;
            _cellobjArray[4].m_value.text = "-";
            _cellobjArray[4].m_value.color = Color.white;

            _cellobjArray[5].m_title.text = "-";
            _cellobjArray[5].m_image.sprite = GameMgr.g_GMGR_Inst.UI_None_Sprite;
            _cellobjArray[5].m_value.text = "-";
            _cellobjArray[5].m_value.color = Color.white;


            //현재 등급 레전더리 미만일경우,
            if (script.m_Grade < Grade.Legendary) {

                //타워 다음등급
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.text = GradeReturnString(NextGrade);
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                //업그레이드 비용
                GameMgr.g_GMGR_Inst.Info_UpgradeCost_Text.text = ClamTower.m_Cost[(int)NextGrade].ToString() + "메소";

                //업그레이드 버튼 색상
                if (GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.activeSelf == false) {
                    GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.SetActive(true);
                    GameMgr.g_GMGR_Inst.Info_NextGradeTitle_Text.gameObject.SetActive(true);
                    GameMgr.g_GMGR_Inst.Info_Coin_Image.gameObject.SetActive(true);
                }
                GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.GetComponent<Image>().color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];


                //호버시 나타내줄 정보 입력
                Hover.curValue[0] = _cellobjArray[2].m_value.text;
                Hover.nextValue[0] = ClamTower.m_Damage[(int)NextGrade].ToString();

                Hover.curValue[1] = _cellobjArray[3].m_value.text;
                Hover.nextValue[1] = ClamTower.m_AttackSpeed[(int)NextGrade].ToString();

                Hover.curValue[2] = "-";
                Hover.nextValue[2] = "-";

                Hover.curValue[3] = "-";
                Hover.nextValue[3] = "-";

                //호버시 나타내줄 정보 입력

            }
            //현재 등급 레전더리 일경우,
            else {
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.text = "";
                GameMgr.g_GMGR_Inst.Info_UpgradeCost_Text.text = "";
                GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.SetActive(false);
                GameMgr.g_GMGR_Inst.Info_NextGradeTitle_Text.gameObject.SetActive(false);
                GameMgr.g_GMGR_Inst.Info_Coin_Image.gameObject.SetActive(false);
            }

        }
        //가재타워(로랑)
        else if (_obj.GetComponent<CrabTowerMgr>() != null) {
            CrabTowerMgr script = _obj.GetComponent<CrabTowerMgr>();
            Grade NextGrade = script.m_Grade + 1;
            GameMgr.g_GMGR_Inst.Info_TowerGrade_Text.text = GradeReturnString(script.m_Grade);
            GameMgr.g_GMGR_Inst.Info_TowerGrade_Text.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)script.m_Grade];
            GameMgr.g_GMGR_Inst.Info_TowerName_Text.text = CrabTower.UI_InfoNickname;
            GameMgr.g_GMGR_Inst.Info_TowerContents_Text.text = CrabTower.UI_InfoContents;
            _cellobjArray[0].m_value.text = TypeReturnString(CrabTower.m_Type);
            _cellobjArray[1].m_value.text = TargetReturnString(CrabTower.m_Target);
            _cellobjArray[2].m_value.text = CrabTower.m_Damage[(int)script.m_Grade].ToString();
            _cellobjArray[2].m_value.color = Color.white;
            _cellobjArray[3].m_value.text = CrabTower.m_AttackSpeed[(int)script.m_Grade].ToString();
            _cellobjArray[3].m_value.color = Color.white;

            //None
            _cellobjArray[4].m_title.text = "-";
            _cellobjArray[4].m_image.sprite = GameMgr.g_GMGR_Inst.UI_None_Sprite;
            _cellobjArray[4].m_value.text = "-";
            _cellobjArray[4].m_value.color = Color.white;

            _cellobjArray[5].m_title.text = "-";
            _cellobjArray[5].m_image.sprite = GameMgr.g_GMGR_Inst.UI_None_Sprite;
            _cellobjArray[5].m_value.text = "-";
            _cellobjArray[5].m_value.color = Color.white;

            if (script.m_Grade < Grade.Legendary) {

                //타워 다음등급
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.text = GradeReturnString(NextGrade);
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                //업그레이드 비용
                GameMgr.g_GMGR_Inst.Info_UpgradeCost_Text.text = CrabTower.m_Cost[(int)NextGrade].ToString() + "메소";

                //업그레이드 버튼 색상
                if (GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.activeSelf == false) {
                    GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.SetActive(true);
                    GameMgr.g_GMGR_Inst.Info_NextGradeTitle_Text.gameObject.SetActive(true);
                    GameMgr.g_GMGR_Inst.Info_Coin_Image.gameObject.SetActive(true);
                }
                GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.GetComponent<Image>().color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                //호버시 나타내줄 정보 입력
                Hover.curValue[0] = _cellobjArray[2].m_value.text;
                Hover.nextValue[0] = CrabTower.m_Damage[(int)NextGrade].ToString();

                Hover.curValue[1] = _cellobjArray[3].m_value.text;
                Hover.nextValue[1] = CrabTower.m_AttackSpeed[(int)NextGrade].ToString();

                Hover.curValue[2] = "-";
                Hover.nextValue[2] = "-";

                Hover.curValue[3] = "-";
                Hover.nextValue[3] = "-";
                //호버시 나타내줄 정보 입력
            }
            else {
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.text = "";
                GameMgr.g_GMGR_Inst.Info_UpgradeCost_Text.text = "";
                GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.SetActive(false);
                GameMgr.g_GMGR_Inst.Info_NextGradeTitle_Text.gameObject.SetActive(false);
                GameMgr.g_GMGR_Inst.Info_Coin_Image.gameObject.SetActive(false);
            }

        }
        //전기타워(망둥이)
        else if (_obj.GetComponent<ElectricTowerMgr>() != null) {
            ElectricTowerMgr script = _obj.GetComponent<ElectricTowerMgr>();
            Grade NextGrade = script.m_Grade + 1;
            GameMgr.g_GMGR_Inst.Info_TowerGrade_Text.text = GradeReturnString(script.m_Grade);
            GameMgr.g_GMGR_Inst.Info_TowerGrade_Text.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)script.m_Grade];
            GameMgr.g_GMGR_Inst.Info_TowerName_Text.text = ElectricEelTower.UI_InfoNickname;
            GameMgr.g_GMGR_Inst.Info_TowerContents_Text.text = ElectricEelTower.UI_InfoContents;
            _cellobjArray[0].m_value.text = TypeReturnString(ElectricEelTower.m_Type);
            _cellobjArray[1].m_value.text = TargetReturnString(ElectricEelTower.m_Target);
            _cellobjArray[2].m_value.text = ElectricEelTower.m_Damage[(int)script.m_Grade].ToString();
            _cellobjArray[2].m_value.color = Color.white;
            _cellobjArray[3].m_value.text = ElectricEelTower.m_AttackSpeed[(int)script.m_Grade].ToString();
            _cellobjArray[3].m_value.color = Color.white;


            //연쇄 공격 대상 수
            _cellobjArray[4].m_title.text = "연쇄공격 수";
            _cellobjArray[4].m_image.sprite = GameMgr.g_GMGR_Inst.UI_ChainAttack_Sprite;
            _cellobjArray[4].m_value.text = ElectricEelTower.m_ChainCnt[(int)script.m_Grade].ToString() + "마리";
            _cellobjArray[4].m_value.color = Color.white;

            //None
            _cellobjArray[5].m_title.text = "-";
            _cellobjArray[5].m_image.sprite = GameMgr.g_GMGR_Inst.UI_None_Sprite;
            _cellobjArray[5].m_value.text = "-";
            _cellobjArray[5].m_value.color = Color.white;

            if (script.m_Grade < Grade.Legendary) {

                //타워 다음등급
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.text = GradeReturnString(NextGrade);
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                //업그레이드 비용
                GameMgr.g_GMGR_Inst.Info_UpgradeCost_Text.text = ElectricEelTower.m_Cost[(int)NextGrade].ToString() + "메소";

                //업그레이드 버튼 색상
                if (GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.activeSelf == false) {
                    GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.SetActive(true);
                    GameMgr.g_GMGR_Inst.Info_NextGradeTitle_Text.gameObject.SetActive(true);
                    GameMgr.g_GMGR_Inst.Info_Coin_Image.gameObject.SetActive(true);
                }
                GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.GetComponent<Image>().color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                //호버시 나타내줄 정보 입력
                Hover.curValue[0] = _cellobjArray[2].m_value.text;
                Hover.nextValue[0] = ElectricEelTower.m_Damage[(int)NextGrade].ToString();

                Hover.curValue[1] = _cellobjArray[3].m_value.text;
                Hover.nextValue[1] = ElectricEelTower.m_AttackSpeed[(int)NextGrade].ToString();

                Hover.curValue[2] = _cellobjArray[4].m_value.text;
                Hover.nextValue[2] = ElectricEelTower.m_ChainCnt[(int)NextGrade].ToString() + "마리";

                Hover.curValue[3] = "-";
                Hover.nextValue[3] = "-";
                //호버시 나타내줄 정보 입력
            }
            else {
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.text = "";
                GameMgr.g_GMGR_Inst.Info_UpgradeCost_Text.text = "";
                GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.SetActive(false);
                GameMgr.g_GMGR_Inst.Info_NextGradeTitle_Text.gameObject.SetActive(false);
                GameMgr.g_GMGR_Inst.Info_Coin_Image.gameObject.SetActive(false);
            }

        }
        //복어타워(푸퍼)
        else if (_obj.GetComponent<PufferTowerMgr>() != null) {
            PufferTowerMgr script = _obj.GetComponent<PufferTowerMgr>();
            Grade NextGrade = script.m_Grade + 1;
            GameMgr.g_GMGR_Inst.Info_TowerGrade_Text.text = GradeReturnString(script.m_Grade);
            GameMgr.g_GMGR_Inst.Info_TowerGrade_Text.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)script.m_Grade];
            GameMgr.g_GMGR_Inst.Info_TowerName_Text.text = PufferTower.UI_InfoNickname;
            GameMgr.g_GMGR_Inst.Info_TowerContents_Text.text = PufferTower.UI_InfoContents;
            _cellobjArray[0].m_value.text = TypeReturnString(PufferTower.m_Type);
            _cellobjArray[1].m_value.text = TargetReturnString(PufferTower.m_Target);
            _cellobjArray[2].m_value.text = PufferTower.m_Damage[(int)script.m_Grade].ToString();
            _cellobjArray[2].m_value.color = Color.white;
            _cellobjArray[3].m_value.text = PufferTower.m_AttackSpeed[(int)script.m_Grade].ToString();
            _cellobjArray[3].m_value.color = Color.white;


            //디버프 지속시간
            _cellobjArray[4].m_title.text = "디버프 지속시간";
            _cellobjArray[4].m_image.sprite = GameMgr.g_GMGR_Inst.UI_DebuffTime;
            _cellobjArray[4].m_value.text = PufferTower.m_FreezeTime[(int)script.m_Grade].ToString();
            _cellobjArray[4].m_value.color = Color.white;

            //None
            _cellobjArray[5].m_title.text = "-";
            _cellobjArray[5].m_image.sprite = GameMgr.g_GMGR_Inst.UI_None_Sprite;
            _cellobjArray[5].m_value.text = "-";
            _cellobjArray[5].m_value.color = Color.white;

            if (script.m_Grade < Grade.Legendary) {

                //타워 다음등급
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.text = GradeReturnString(NextGrade);
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                //업그레이드 비용
                GameMgr.g_GMGR_Inst.Info_UpgradeCost_Text.text = PufferTower.m_Cost[(int)NextGrade].ToString() + "메소";

                //업그레이드 버튼 색상
                if (GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.activeSelf == false) {
                    GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.SetActive(true);
                    GameMgr.g_GMGR_Inst.Info_NextGradeTitle_Text.gameObject.SetActive(true);
                    GameMgr.g_GMGR_Inst.Info_Coin_Image.gameObject.SetActive(true);
                }
                GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.GetComponent<Image>().color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                //호버시 나타내줄 정보 입력
                Hover.curValue[0] = _cellobjArray[2].m_value.text;
                Hover.nextValue[0] = PufferTower.m_Damage[(int)NextGrade].ToString();

                Hover.curValue[1] = _cellobjArray[3].m_value.text;
                Hover.nextValue[1] = PufferTower.m_AttackSpeed[(int)NextGrade].ToString();

                Hover.curValue[2] = _cellobjArray[4].m_value.text;
                Hover.nextValue[2] = PufferTower.m_FreezeTime[(int)NextGrade].ToString();

                Hover.curValue[3] = "-";
                Hover.nextValue[3] = "-";
                //호버시 나타내줄 정보 입력
            }
            else {
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.text = "";
                GameMgr.g_GMGR_Inst.Info_UpgradeCost_Text.text = "";
                GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.SetActive(false);
                GameMgr.g_GMGR_Inst.Info_NextGradeTitle_Text.gameObject.SetActive(false);
                GameMgr.g_GMGR_Inst.Info_Coin_Image.gameObject.SetActive(false);
            }

        }
        //독복어타워(포이즌푸퍼)
        else if (_obj.GetComponent<PoisonPufferTowerMgr>() != null) {
            PoisonPufferTowerMgr script = _obj.GetComponent<PoisonPufferTowerMgr>();
            Grade NextGrade = script.m_Grade + 1;
            GameMgr.g_GMGR_Inst.Info_TowerGrade_Text.text = GradeReturnString(script.m_Grade);
            GameMgr.g_GMGR_Inst.Info_TowerGrade_Text.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)script.m_Grade];
            GameMgr.g_GMGR_Inst.Info_TowerName_Text.text = PoisonPufferTower.UI_InfoNickname;
            GameMgr.g_GMGR_Inst.Info_TowerContents_Text.text = PoisonPufferTower.UI_InfoContents;
            _cellobjArray[0].m_value.text = TypeReturnString(PoisonPufferTower.m_Type);
            _cellobjArray[1].m_value.text = TargetReturnString(PoisonPufferTower.m_Target);
            _cellobjArray[2].m_value.text = PoisonPufferTower.m_Damage[(int)script.m_Grade].ToString();
            _cellobjArray[2].m_value.color = Color.white;
            _cellobjArray[3].m_value.text = PoisonPufferTower.m_AttackSpeed[(int)script.m_Grade].ToString();
            _cellobjArray[3].m_value.color = Color.white;


            //디버프 지속시간
            _cellobjArray[4].m_title.text = "디버프 지속시간";
            _cellobjArray[4].m_image.sprite = GameMgr.g_GMGR_Inst.UI_DebuffTime;
            _cellobjArray[4].m_value.text = PoisonPufferTower.m_PoisonTime[(int)script.m_Grade].ToString();
            _cellobjArray[4].m_value.color = Color.white;

            //디버프 데미지
            _cellobjArray[5].m_title.text = "디버프 데미지";
            _cellobjArray[5].m_image.sprite = GameMgr.g_GMGR_Inst.UI_Poison_Sprite;
            _cellobjArray[5].m_value.text = PoisonPufferTower.m_PoisonDamage[(int)script.m_Grade].ToString();
            _cellobjArray[5].m_value.color = Color.white;

            if (script.m_Grade < Grade.Legendary) {

                //타워 다음등급
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.text = GradeReturnString(NextGrade);
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                //업그레이드 비용
                GameMgr.g_GMGR_Inst.Info_UpgradeCost_Text.text = PoisonPufferTower.m_Cost[(int)NextGrade].ToString() + "메소";

                //업그레이드 버튼 색상
                if (GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.activeSelf == false) {
                    GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.SetActive(true);
                    GameMgr.g_GMGR_Inst.Info_NextGradeTitle_Text.gameObject.SetActive(true);
                    GameMgr.g_GMGR_Inst.Info_Coin_Image.gameObject.SetActive(true);
                }
                GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.GetComponent<Image>().color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                //호버시 나타내줄 정보 입력
                Hover.curValue[0] = _cellobjArray[2].m_value.text;
                Hover.nextValue[0] = PoisonPufferTower.m_Damage[(int)NextGrade].ToString();

                Hover.curValue[1] = _cellobjArray[3].m_value.text;
                Hover.nextValue[1] = PoisonPufferTower.m_AttackSpeed[(int)NextGrade].ToString();

                Hover.curValue[2] = _cellobjArray[4].m_value.text;
                Hover.nextValue[2] = PoisonPufferTower.m_PoisonTime[(int)NextGrade].ToString();

                Hover.curValue[3] = _cellobjArray[5].m_value.text;
                Hover.nextValue[3] = PoisonPufferTower.m_PoisonDamage[(int)NextGrade].ToString();
                //호버시 나타내줄 정보 입력
            }
            else {
                GameMgr.g_GMGR_Inst.Info_NextGrade_Text.text = "";
                GameMgr.g_GMGR_Inst.Info_UpgradeCost_Text.text = "";
                GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.SetActive(false);
                GameMgr.g_GMGR_Inst.Info_NextGradeTitle_Text.gameObject.SetActive(false);
                GameMgr.g_GMGR_Inst.Info_Coin_Image.gameObject.SetActive(false);
            }

        }


    }

    public void TowerUpgrade (GameObject _obj) {

        //임시 타워 Null인지 체크
        if (_obj != null) {

            //조개타워(세르프)
            if (_obj.GetComponent<ClamTowerMgr>() != null) {

                ClamTowerMgr script = _obj.GetComponent<ClamTowerMgr>();

                Grade NextGrade = script.m_Grade + 1;


                //골드 있는지 판정,
                if (GameMgr.g_GMGR_Inst.m_Gold >= ClamTower.m_Cost[(int)NextGrade]) {

                    //사운드
                    SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.LevelUp_Sound, 1.0f);
                    //이펙트
                    GameObject LevelUpEffect = Instantiate(GameMgr.g_GMGR_Inst.LevelUp_Effect, GameMgr.g_GMGR_Inst.EffectGroup.transform);
                    LevelUpEffect.transform.position = new Vector3(_obj.transform.position.x, _obj.transform.position.y + 1.0f, _obj.transform.position.z + 0.2f);


                    //골드감소
                    GameMgr.g_GMGR_Inst.MinusGold(ClamTower.m_Cost[(int)NextGrade]);

                    //스크립트내 타워 스탯 업글
                    script.m_Grade += 1;
                    script.m_Damage = ClamTower.m_Damage[(int)NextGrade];
                    script.m_AttackSpped = ClamTower.m_AttackSpeed[(int)NextGrade];
                    script.m_Range = ClamTower.m_Range[(int)NextGrade];

                    _obj.transform.Find("Range").transform.localScale = new Vector3(script.m_Range, script.m_Range, script.m_Range);

                    //타워 테두리 색상 변경
                    script.m_GradeSpritRenderer.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                    //정보 패널 업데이트
                    TowerInfoPanelUpdate(_obj, GameMgr.g_GMGR_Inst.UI_InfoCellArray);

                }
                //골드가 없다면,
                else {

                    SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Error_Sound, 1.0f);

                    //없다면, 골드가 부족합니다. Text 플로팅
                    GameObject emptyGameObject = new GameObject("Message");
                    emptyGameObject.transform.parent = GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.transform;
                    FloatingText FloatingMessage = emptyGameObject.AddComponent<FloatingText>();
                    FloatingMessage.Setting(GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.transform.position,
                                            180.0f, 30, "골드가 부족합니다.", Color.red);
                }
            }
            //로랑 타워
            else if (_obj.GetComponent<CrabTowerMgr>() != null) {
                CrabTowerMgr script = _obj.GetComponent<CrabTowerMgr>();
                Grade NextGrade = script.m_Grade + 1;



                //골드 있는지 판정,
                if (GameMgr.g_GMGR_Inst.m_Gold >= CrabTower.m_Cost[(int)NextGrade]) {

                    //사운드
                    SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.LevelUp_Sound, 1.0f);
                    //이펙트
                    GameObject LevelUpEffect = Instantiate(GameMgr.g_GMGR_Inst.LevelUp_Effect, GameMgr.g_GMGR_Inst.EffectGroup.transform);
                    LevelUpEffect.transform.position = new Vector3(_obj.transform.position.x, _obj.transform.position.y + 1.0f, _obj.transform.position.z + 0.2f);


                    //골드감소
                    GameMgr.g_GMGR_Inst.MinusGold(CrabTower.m_Cost[(int)NextGrade]);

                    //스크립트내 타워 스탯 업글
                    script.m_Grade += 1;
                    script.m_Damage = CrabTower.m_Damage[(int)NextGrade];
                    script.m_AttackSpped = CrabTower.m_AttackSpeed[(int)NextGrade];

                    //타워 테두리 색상 변경
                    script.m_GradeSpritRenderer.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                    //정보 패널 업데이트
                    TowerInfoPanelUpdate(_obj, GameMgr.g_GMGR_Inst.UI_InfoCellArray);
                }
                //골드가 없다면,
                else {

                    SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Error_Sound, 1.0f);

                    //없다면, 골드가 부족합니다. Text 플로팅
                    GameObject emptyGameObject = new GameObject("Message");
                    emptyGameObject.transform.parent = GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.transform;
                    FloatingText FloatingMessage = emptyGameObject.AddComponent<FloatingText>();
                    FloatingMessage.Setting(GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.transform.position,
                                            180.0f, 30, "골드가 부족합니다.", Color.red);
                }
            }
            //망둥이 타워
            else if (_obj.GetComponent<ElectricTowerMgr>() != null) {
                ElectricTowerMgr script = _obj.GetComponent<ElectricTowerMgr>();
                Grade NextGrade = script.m_Grade + 1;

                //골드 있는지 판정,
                if (GameMgr.g_GMGR_Inst.m_Gold >= ElectricEelTower.m_Cost[(int)NextGrade]) {

                    //사운드
                    SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.LevelUp_Sound, 1.0f);
                    //이펙트
                    GameObject LevelUpEffect = Instantiate(GameMgr.g_GMGR_Inst.LevelUp_Effect, GameMgr.g_GMGR_Inst.EffectGroup.transform);
                    LevelUpEffect.transform.position = new Vector3(_obj.transform.position.x, _obj.transform.position.y + 1.0f, _obj.transform.position.z + 0.2f);


                    //골드감소
                    GameMgr.g_GMGR_Inst.MinusGold(ElectricEelTower.m_Cost[(int)NextGrade]);

                    //스크립트내 타워 스탯 업글
                    script.m_Grade += 1;
                    script.m_Damage = ElectricEelTower.m_Damage[(int)NextGrade];
                    script.m_AttackSpped = ElectricEelTower.m_AttackSpeed[(int)NextGrade];
                    script.m_Range = ElectricEelTower.m_Range[(int)NextGrade];
                    _obj.transform.Find("Range").transform.localScale = new Vector3(script.m_Range, script.m_Range, script.m_Range);
                    script.m_ChainCnt = ElectricEelTower.m_ChainCnt[(int)NextGrade];

                    //타워 테두리 색상 변경
                    script.m_GradeSpritRenderer.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                    //정보 패널 업데이트
                    TowerInfoPanelUpdate(_obj, GameMgr.g_GMGR_Inst.UI_InfoCellArray);
                }
                //골드가 없다면,
                else {

                    SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Error_Sound, 1.0f);

                    //없다면, 골드가 부족합니다. Text 플로팅
                    GameObject emptyGameObject = new GameObject("Message");
                    emptyGameObject.transform.parent = GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.transform;
                    FloatingText FloatingMessage = emptyGameObject.AddComponent<FloatingText>();
                    FloatingMessage.Setting(GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.transform.position,
                                            180.0f, 30, "골드가 부족합니다.", Color.red);
                }
            }
            //푸퍼 타워
            else if (_obj.GetComponent<PufferTowerMgr>() != null) {
                PufferTowerMgr script = _obj.GetComponent<PufferTowerMgr>();
                Grade NextGrade = script.m_Grade + 1;

                //골드 있는지 판정,
                if (GameMgr.g_GMGR_Inst.m_Gold >= PufferTower.m_Cost[(int)NextGrade]) {

                    //사운드
                    SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.LevelUp_Sound, 1.0f);
                    //이펙트
                    GameObject LevelUpEffect = Instantiate(GameMgr.g_GMGR_Inst.LevelUp_Effect, GameMgr.g_GMGR_Inst.EffectGroup.transform);
                    LevelUpEffect.transform.position = new Vector3(_obj.transform.position.x, _obj.transform.position.y + 1.0f, _obj.transform.position.z + 0.2f);


                    //골드감소
                    GameMgr.g_GMGR_Inst.MinusGold(PufferTower.m_Cost[(int)NextGrade]);

                    //스크립트내 타워 스탯 업글
                    script.m_Grade += 1;
                    script.m_Damage = PufferTower.m_Damage[(int)NextGrade];
                    script.m_AttackSpped = PufferTower.m_AttackSpeed[(int)NextGrade];
                    script.m_Range = PufferTower.m_Range[(int)NextGrade];
                    _obj.transform.Find("Range").transform.localScale = new Vector3(script.m_Range, script.m_Range, script.m_Range);
                    script.m_FreezeTime = PufferTower.m_FreezeTime[(int)NextGrade];

                    //타워 테두리 색상 변경
                    script.m_GradeSpritRenderer.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                    //정보 패널 업데이트
                    TowerInfoPanelUpdate(_obj, GameMgr.g_GMGR_Inst.UI_InfoCellArray);
                }
                //골드가 없다면,
                else {

                    SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Error_Sound, 1.0f);

                    //없다면, 골드가 부족합니다. Text 플로팅
                    GameObject emptyGameObject = new GameObject("Message");
                    emptyGameObject.transform.parent = GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.transform;
                    FloatingText FloatingMessage = emptyGameObject.AddComponent<FloatingText>();
                    FloatingMessage.Setting(GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.transform.position,
                                            180.0f, 30, "골드가 부족합니다.", Color.red);
                }
            }
            //포이즌 푸퍼
            else if (_obj.GetComponent<PoisonPufferTowerMgr>() != null) {
                PoisonPufferTowerMgr script = _obj.GetComponent<PoisonPufferTowerMgr>();
                Grade NextGrade = script.m_Grade + 1;

                //골드 있는지 판정,
                if (GameMgr.g_GMGR_Inst.m_Gold >= PoisonPufferTower.m_Cost[(int)NextGrade]) {

                    //사운드
                    SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.LevelUp_Sound, 1.0f);
                    //이펙트
                    GameObject LevelUpEffect = Instantiate(GameMgr.g_GMGR_Inst.LevelUp_Effect, GameMgr.g_GMGR_Inst.EffectGroup.transform);
                    LevelUpEffect.transform.position = new Vector3(_obj.transform.position.x, _obj.transform.position.y + 1.0f, _obj.transform.position.z + 0.2f);


                    //골드감소
                    GameMgr.g_GMGR_Inst.MinusGold(PoisonPufferTower.m_Cost[(int)NextGrade]);

                    //스크립트내 타워 스탯 업글
                    script.m_Grade += 1;
                    script.m_Damage = PoisonPufferTower.m_Damage[(int)NextGrade];
                    script.m_AttackSpped = PoisonPufferTower.m_AttackSpeed[(int)NextGrade];
                    script.m_Range = PoisonPufferTower.m_Range[(int)NextGrade];
                    _obj.transform.Find("Range").transform.localScale = new Vector3(script.m_Range, script.m_Range, script.m_Range);
                    script.m_PoisonTime = PoisonPufferTower.m_PoisonTime[(int)NextGrade];
                    script.m_PoisonDamage = PoisonPufferTower.m_PoisonDamage[(int)NextGrade];

                    //타워 테두리 색상 변경
                    script.m_GradeSpritRenderer.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[(int)NextGrade];

                    //정보 패널 업데이트
                    TowerInfoPanelUpdate(_obj, GameMgr.g_GMGR_Inst.UI_InfoCellArray);
                }
                //골드가 없다면,
                else {

                    SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Error_Sound, 1.0f);

                    //없다면, 골드가 부족합니다. Text 플로팅
                    GameObject emptyGameObject = new GameObject("Message");
                    emptyGameObject.transform.parent = GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.transform;
                    FloatingText FloatingMessage = emptyGameObject.AddComponent<FloatingText>();
                    FloatingMessage.Setting(GameMgr.g_GMGR_Inst.Info_Upgrade_Btn.gameObject.transform.position,
                                            180.0f, 30, "골드가 부족합니다.", Color.red);
                }
            }

        }
    }







}

