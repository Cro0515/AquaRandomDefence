using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TowerMaker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

    public GameObject realTower = null;     //실제 생성될 타워
    private GameObject alphaTower = null;   //생성전 표시해줄 투명
    public GameObject guideCell = null;    //생성 가능 여부 표시 셀

    private bool Isclicked = false;
    private bool Isdestroy = false;

    Color calcColor, redColor, GreenColor;
    Quaternion CalcRot= Quaternion.identity;

    int TowerCost;
    Text Cost_Text;


    /*
    알파타워=리얼타워 인스탄시에이트 로 복사
    알파타워 색상값(알파값) 변경
    알파타워 컴포넌트 제거    
    */


    // Start is called before the first frame update
    void Start()
    {
        Isclicked = false;
        Isdestroy = false;
        CalcRot.eulerAngles = new Vector3(90, 0, 0);
        calcColor = realTower.transform.Find("Tower").GetComponent<SpriteRenderer>().color;

        guideCell = GameMgr.g_GMGR_Inst.GuideCell;
        redColor = new Color(255, 0, 0, guideCell.GetComponent<SpriteRenderer>().color.a);
        GreenColor = new Color(0, 255, 0, guideCell.GetComponent<SpriteRenderer>().color.a);


        Cost_Text = this.transform.Find("Coin_Text").gameObject.GetComponent<Text>();


        TowerCost = TowerStatus.Inst.CostCheck(realTower.transform.Find("Tower").gameObject);
        Cost_Text.text = TowerCost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    public void OnPointerDown (PointerEventData eventData) {

        SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Install_Sound, 1.0f);


        //안내용 그리드 켜기
        GameMgr.g_GMGR_Inst.GridGroup.SetActive(true);

        //반투명 타워 생성
        Isclicked = true;
        alphaTower = Instantiate(realTower, this.transform.position, this.transform.rotation * CalcRot);
        alphaTower.transform.Find("Tower").GetComponent<SpriteRenderer>().color = new Color(calcColor.r, calcColor.g, calcColor.b, 0.4f);

        

        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousepos.y = 0.1f;
        alphaTower.transform.position = mousepos;
        
        //UI 끄기
        GameMgr.g_GMGR_Inst.m_InfoPanel_flag = false;

        //테스트 모드라면 리턴 버튼 끄기
        if (GlobalGameData.m_GameMode == GameMode.Test)
            GameMgr.g_GMGR_Inst.Retrun_Btn.gameObject.SetActive(false);

        CostCheck();
    }


    public void OnDrag (PointerEventData eventData) {

        //반투명 타워 이동
        if (Isclicked == true) {
            //설치 가능여부 안내용 셀 켜기
            guideCell.SetActive(true);
            //이전 타워 범위 끄기
            if (GameMgr.g_GMGR_Inst.m_TempTowerPick != null)
                GameMgr.g_GMGR_Inst.m_TempTowerPick.transform.Find("Tower").Find("Range").GetComponent<SpriteRenderer>().enabled = false;


            //마우스 위치의 반올림
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 calcPos = new Vector3(Mathf.RoundToInt(mousepos.x), 0.1f, Mathf.RoundToInt(mousepos.z));
            alphaTower.transform.position = calcPos;
            guideCell.transform.position = calcPos;


            //설치 가능여부 안내용 셀 색상 변경
            if (PathFindeAlg.Inst.NodeArray[(int)calcPos.x, (int)calcPos.z].Type == NodeType.None) {
                guideCell.GetComponent<SpriteRenderer>().color = GreenColor;
            }
            else {
                guideCell.GetComponent<SpriteRenderer>().color = redColor;
            }
        }


    }

    public void OnPointerUp (PointerEventData eventData) {

        Vector3 createPos = alphaTower.transform.position;

        if(CostCheck() == false){
            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Error_Sound, 1.0f);

            GameObject emptyGameObject = new GameObject("Message");
            emptyGameObject.transform.parent = GameObject.Find("Canvas").gameObject.transform;
            FloatingText FloatingMessage = emptyGameObject.AddComponent<FloatingText>();
            FloatingMessage.Setting(GameMgr.g_GMGR_Inst.Gold_Text.transform.position,
                                    180.0f, 30, "골드가 부족합니다.", Color.red);

        }



        //설치가능 조건
        //NodeArray 배열의 Type값이 None(빈공간, 0)일때만, 생성
        if (PathFindeAlg.Inst.NodeArray[(int)createPos.x, (int)createPos.z].Type == NodeType.None && CostCheck() == true) {

            SoundMgr.g_inst.UISound_Audio.PlayOneShot(SoundMgr.g_inst.Install_Sound, 1.0f);

            //실제 타워 설치
            Isclicked = false;
            GameObject createTower = Instantiate(realTower, alphaTower.transform.position, this.transform.rotation * CalcRot);
            createTower.transform.SetParent(GameObject.Find("TowerGroup").transform);
            PathFindeAlg.Inst.NodeArray[(int)createPos.x, (int)createPos.z].Type = NodeType.Tower;

            //범위표시 안내용 스프라이트 끄기
            if(createTower.transform.Find("Tower").Find("Range") == true)
                createTower.transform.Find("Tower").Find("Range").GetComponent<SpriteRenderer>().enabled = false;

            //설치 후 동작하기 위한 BulletGroup 액티브, m_isWarking 켜기
            if (createTower.transform.Find("BulletGroup") != null)
                createTower.transform.Find("BulletGroup").gameObject.SetActive(true);


            GameMgr.g_GMGR_Inst.MinusGold(TowerCost);
            Isdestroy = true;
        }

        //반투명 타워 삭제
        Destroy(alphaTower);
        //설치 가능여부 안내용 셀 끄기
        guideCell.SetActive(false);

        //안내용 그리드 끄기
        GameMgr.g_GMGR_Inst.GridGroup.SetActive(false);

        //이전 타워 범위 끄기
        if(GameMgr.g_GMGR_Inst.m_TempTowerPick != null)
            GameMgr.g_GMGR_Inst.m_TempTowerPick.transform.Find("Tower").Find("Range").GetComponent<SpriteRenderer>().enabled = false;

        //테스트 모드라면 다시 켜기
        if (GlobalGameData.m_GameMode == GameMode.Test)
            GameMgr.g_GMGR_Inst.Retrun_Btn.gameObject.SetActive(true);

        if (Isdestroy == true) {
            Destroy(this.gameObject);
        }
    }

    bool CostCheck () {

        if (TowerCost <= GameMgr.g_GMGR_Inst.m_Gold){
            return true;
        }

        if (TowerCost < 0) {
            return false;
        }


        return false;
    }




}
