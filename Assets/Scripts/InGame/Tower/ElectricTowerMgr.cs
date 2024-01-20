using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ObjectPoolClass;
using System.Linq;
using System;

public class ElectricTowerMgr : MonoBehaviour
{
    [HideInInspector] public Grade  m_Grade;        //등급
    [HideInInspector] public int    m_Damage;       //공격력
    [HideInInspector] public float  m_Range;        //범위
    [HideInInspector] public float  m_AttackSpped;  //공격속도
    [HideInInspector] public int    m_ChainCnt;     //체인 공격 대상 수
    [HideInInspector] public float  m_ChainRange;   //체인 공격 범위
    


    [HideInInspector] public bool m_isWarking = false;

    [HideInInspector] float m_AttackDelta_Time = 0.0f;

    [HideInInspector] public GameObject m_BulletGroup;                                              //총알 오브젝트 그룹(타워 Acctive 역할)
    [HideInInspector] public LinkedList<GameObject> m_TargetList = new LinkedList<GameObject>();   //타겟 리스트 (링크드 리스트)
    [HideInInspector] LinkedListNode<GameObject> node;                                                               //타겟 리스트 갱신용 노드 변수

    [HideInInspector] TowerStatus TowerStatus = null; //타워정보 인스턴스 변수

    [HideInInspector] public SpriteRenderer m_GradeSpritRenderer;    //타워 등급 스프라이트 렌더


    //---------------연쇄공격 관련 변수
    [HideInInspector] public List<Transform> m_ChainAttack_List = new List<Transform>();              //연쇄공격을 위한 리스트
    [HideInInspector] public List<Transform> ALL_Monster_List;                                        //모든 몬스터 받아올 리스트
    [HideInInspector] GameObject ActiveGroup;
    //---------------연쇄공격 관련 변수
    


    //----------------이펙트 관련 변수
    LineRenderer m_LineRender;                          //이펙트에 사용될 LineRender
    Material material_effect_1, material_effect_2;      //이펙트 변경을 위한 material
    bool m_matrial_flag, m_coroutine_flag;              //마테리얼 변경을 위한 bool
    Vector3[] m_LRPos_Array;
    //----------------이펙트 관련 변수

    [HideInInspector] public Animator m_Anim;


    private void Awake () {
        TowerStatus = TowerStatus.Inst;

        //마테리얼 초기화
        material_effect_1 = Resources.Load<Material>("Material/LinkedLightning_1");
        material_effect_2 = Resources.Load<Material>("Material/LinkedLightning_2");
        //마테리얼 초기화

        m_LineRender = this.gameObject.GetComponent<LineRenderer>();

        m_BulletGroup = this.gameObject.transform.parent.Find("BulletGroup").gameObject;


        m_GradeSpritRenderer = this.gameObject.transform.Find("Grade").GetComponent<SpriteRenderer>();

        m_Anim = GetComponent<Animator>();

    }


    // Start is called before the first frame update
    void Start()
    {
        //타워 정보 가져오기
        m_Grade = Grade.Rare;
        m_Damage = TowerStatus.ElectricEelTower.m_Damage[(int)m_Grade];
        m_Range = TowerStatus.ElectricEelTower.m_Range[(int)m_Grade];
        m_AttackSpped = TowerStatus.ElectricEelTower.m_AttackSpeed[(int)m_Grade];
        m_ChainCnt = TowerStatus.ElectricEelTower.m_ChainCnt[(int)m_Grade];
        m_ChainRange = TowerStatus.ElectricEelTower.m_ChainRange;
        //

        //공격범위 만큼 Circle 반경 증가
        this.transform.Find("Range").transform.localScale = new Vector3(m_Range, m_Range, m_Range);
        //

        ActiveGroup = GameObject.Find("ActiveGroup").transform.gameObject;

        //----------------이펙트 관련 초기화 및 설정
        //라인 렌더러 초기화 및 액티브 비활성
        m_LineRender.enabled = false;
        //----------------이펙트 관련 초기화 및 설정


        //등급 색상 부여 (초기 레어)
        m_GradeSpritRenderer.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[0];

        //초기 애니메이션 동작 막기 위해 컴포넌트 비활성화 
        m_Anim.speed = 1.0f;
        m_Anim.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //설치후 동작하게 하기 위한 BulletGroup 액티브
        if (m_isWarking == false && m_BulletGroup.activeSelf == true) {
            m_isWarking = true;
            m_Anim.enabled = true;

        }



        if (m_LineRender.enabled == true) {

            LR_PosUpdate();
             
        }

        if (m_TargetList.First != null) {

            node = m_TargetList.First;
            //타겟 리스트 갱신
            for (int i = 0; i <= m_TargetList.Count - 1; i++) {

                //메모리풀로 반환된 몬스터가 리스트내 있을경우 타겟 리스트에서 제거
                //Debug.Log(node);
                //Debug.Log(node.Value.gameObject);

                if (node != null && node.Value.gameObject.activeSelf == false) {
                    m_TargetList.Remove(node);
                }

                if (node.Next == null)
                    break;

                node = node.Next;
            }
        }//if (m_TargetList.First != null)


        if (m_isWarking == true) {

            //공격 델타타임이 0 이상일때   
            if (m_AttackDelta_Time > 0) {
                //감소
                m_AttackDelta_Time -= Time.deltaTime;
            }


            //쿨타임에 따른 공격
            //Debug.Log(m_TargetList.Count);
            //타겟 리스트에 타겟이 있고 && 공격 델타타임 0 이하일때
            if (m_TargetList.Count != 0 && m_AttackDelta_Time <= 0) {

                //타겟 몬스터 바라보기
                if (m_TargetList.First.Value.transform.position.x > this.transform.parent.position.x) {
                    //this.transform.parent.localScale = new Vector3(-1, 1, 1);
                    this.transform.localScale = new Vector3(-1, 1, 1);

                }
                else {
                    //this.transform.parent.localScale = new Vector3(1, 1, 1);
                    this.transform.localScale = new Vector3(1, 1, 1);

                }

                //공격
                AttackTarget();

                //공격 델타 타임 초기화
                m_AttackDelta_Time = m_AttackSpped;
            }
        }//if (m_isWarking == true) 



    }




    void AttackTarget () {

        float min_dis = m_ChainRange;
        int mem_i = -1;

        //메인타겟을 연쇄공격 리스트에 추가
        GameObject mainTarget = m_TargetList.First.Value.gameObject;
        m_ChainAttack_List.Add(mainTarget.transform);


        //-------------활성화된 몬스터 for문으로 가져오기 (ALL_Monster_List)
        //-> for문 순회로 받아오기 // tag가 Monster && 메인타겟이 아닌것 &&
        for (int i = 0; i < ActiveGroup.transform.childCount; i++) {

            Transform tempObj = ActiveGroup.transform.GetChild(i);

            if (tempObj.tag == "Monster" && tempObj != mainTarget.transform)
                ALL_Monster_List.Add(tempObj);
        }
        //-------------활성화된 몬스터 for문으로 가져오기 (ALL_Monster_List)



        //-----------전체 몬스터 목록에서 연쇄공격리스트 선정 (ALL_Monster_List -> m_ChainAttack_List)
        //연쇄공격 대상수 만큼 반복
        for (int j = 0; j < m_ChainCnt-1; j++) {

            //메인타겟으로부터 가까운 몬스터 탐색
            for (int i = 0; i < ALL_Monster_List.Count; i++) {

                //메인 타겟과 몬스터 거리측정 
                float temp_dis = (mainTarget.transform.position - ALL_Monster_List[i].transform.position).sqrMagnitude;

                //최소 거리값 비교
                if (min_dis > temp_dis && m_ChainRange > temp_dis) {

                    //작다면 i번째 타겟 거리값과 i값 저장
                    min_dis = temp_dis;
                    mem_i = i;
                }
            }//for

            if (mem_i >= 0) {
                //체인 리스트에 추가
                m_ChainAttack_List.Add(ALL_Monster_List[mem_i]);

                //타겟변경, 제거
                mainTarget = ALL_Monster_List[mem_i].gameObject;
                ALL_Monster_List.RemoveAt(mem_i);
            }

            //초기값 재설정
            min_dis = m_ChainRange;
            mem_i = -1;

        }//for
        //-----------전체 몬스터 목록에서 연쇄공격리스트 선정 (ALL_Monster_List -> m_ChainAttack_List)


        //라인렌더 배열 초기화 && 라인렌더 위치값 업데이트
        m_LRPos_Array = new Vector3[m_ChainAttack_List.Count + 1];
        LR_PosUpdate();
        //EditorApplication.isPaused = true;

        
        //공격 효과음
        SoundMgr.g_inst.EffectPlay(SoundMgr.g_inst.ElectricAttack_Sound);

        //데미지 부여
        for (int i = 0; i < m_ChainAttack_List.Count; i++) {
            m_ChainAttack_List[i].GetComponent<Monster>().TakeDamage(m_Damage,TowerName.ElectricEel,this.gameObject);
        }
        


        //---------------------이펙트
        //마테리얼 변경
        if (m_matrial_flag == true) {
            m_LineRender.material = material_effect_1;
            m_matrial_flag = !m_matrial_flag;
        }
        else {
            m_LineRender.material = material_effect_2;
            m_matrial_flag = !m_matrial_flag;
        }
        //마테리얼 변경

        //코루틴함수를 통해 이펙트 변경 효과
        if (m_coroutine_flag == true) {
            StartCoroutine(EffectFlash());
        }
        else {
            StopCoroutine(EffectFlash());
            StartCoroutine(EffectFlash());
        }
        //코루틴함수를 통해 이펙트 변경 효과

        //---------------------이펙트

    }


    void LR_PosUpdate () {

        //LineRender에 타워(자기자신) 추가
         m_LRPos_Array[0] = this.transform.position;

        //LineRender에 사용하기위해 Transform Type의 List를 Vector3 Array로 형변환 
        for (int i = 0; i < m_ChainAttack_List.Count; i++) {

            if (m_ChainAttack_List[i].gameObject.activeSelf == true) {

                m_LRPos_Array[i + 1] = m_ChainAttack_List[i].transform.position;
                m_LRPos_Array[i + 1].y = 0.2f;
            }
        }

        //LinRender에 입력
        m_LineRender.positionCount = m_LRPos_Array.Length;
        m_LineRender.SetPositions(m_LRPos_Array);
    }




    IEnumerator EffectFlash () {
        m_coroutine_flag = false;
        m_LineRender.enabled = true;

        yield return new WaitForSeconds(0.05f);
        m_LineRender.startWidth = 0.2f;
        m_LineRender.endWidth = 0.2f;

        yield return new WaitForSeconds(0.05f);
        m_LineRender.startWidth = 0.8f;
        m_LineRender.endWidth = 0.8f;

        yield return new WaitForSeconds(0.05f);
        m_LineRender.startWidth = 0.5f;
        m_LineRender.endWidth = 0.5f;

        yield return new WaitForSeconds(0.05f);
        m_LineRender.startWidth = 0.3f;
        m_LineRender.endWidth = 0.3f;

        yield return new WaitForSeconds(0.05f);
        m_LineRender.startWidth = 0.2f;
        m_LineRender.endWidth = 0.2f;

        yield return new WaitForSeconds(0.05f);
        m_LineRender.startWidth = 0.07f;
        m_LineRender.endWidth = 0.07f;

        yield return new WaitForSeconds(0.05f);
        m_LineRender.startWidth = 0.0f;
        m_LineRender.endWidth = 0.0f;

        m_LineRender.enabled = false;
        m_coroutine_flag = true;

        m_ChainAttack_List.Clear();
        ALL_Monster_List.Clear();
        Array.Clear(m_LRPos_Array, 0, 0);
        m_LineRender.positionCount = 0;
        //Debug.Log("초기화");
        //EditorApplication.isPaused = true;

    }




    private void OnTriggerEnter (Collider other) {
        if (other.tag == "Monster") {
            m_TargetList.AddLast(other.gameObject);
        }
    }


    private void OnTriggerExit (Collider other) {   
        if (other.tag == "Monster") {

            if (m_TargetList.Find(other.gameObject) != null) {

                m_TargetList.Remove(other.gameObject);
            }


        }
    }

}
