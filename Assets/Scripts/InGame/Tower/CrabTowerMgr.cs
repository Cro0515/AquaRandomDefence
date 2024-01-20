using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabTowerMgr : MonoBehaviour
{
    [HideInInspector] public Grade  m_Grade;        //등급
    [HideInInspector] public int    m_Damage;       //공격력
    [HideInInspector] public float  m_Range;        //범위 ※ 0.75 고정 ※
    [HideInInspector] public float  m_AttackSpped;  //공격속도
    [HideInInspector] public TowerState m_State = TowerState.Inst;


    [HideInInspector] float m_AttackDelta_Time = 0.0f;


    [HideInInspector] public GameObject m_BulletGroup;                                              //총알 오브젝트 그룹(타워 Acctive 역할)
    [HideInInspector] public LinkedList<GameObject> m_TargetList = new LinkedList<GameObject>();   //타겟 리스트 (링크드 리스트)
    [HideInInspector] LinkedListNode<GameObject> node;                                                               //타겟 리스트 탐색용 노드

    [HideInInspector] TowerStatus TowerStatus = null; //타워정보 인스턴스 변수

    [HideInInspector] public SpriteRenderer m_GradeSpritRenderer;    //타워 등급 스프라이트 렌더

    [HideInInspector] public Animator m_Anim;
    [HideInInspector] Sprite m_Idle_Sprite;


    private void Awake () {
        TowerStatus = TowerStatus.Inst;
        m_BulletGroup = this.gameObject.transform.parent.Find("BulletGroup").gameObject;
        m_GradeSpritRenderer = this.gameObject.transform.Find("Grade").GetComponent<SpriteRenderer>();
        m_Idle_Sprite = this.GetComponent<SpriteRenderer>().sprite;

        m_Anim = GetComponent<Animator>();

    }


    // Start is called before the first frame update
    void Start()
    {
        //타워 정보 가져오기
        m_Grade = Grade.Rare;
        m_Damage = TowerStatus.CrabTower.m_Damage[(int)m_Grade];
        m_Range = TowerStatus.CrabTower.m_Range;        //※ 범위 고정 ※
        m_AttackSpped = TowerStatus.CrabTower.m_AttackSpeed[(int)m_Grade];
        //

        m_Anim.speed = 1.0f;

        //공격범위 만큼 Circle 반경 증가
        this.transform.Find("Range").transform.localScale = new Vector3(m_Range, m_Range, 3);
        //

        //등급 색상 부여 (초기 레어)
        m_GradeSpritRenderer.color = GameMgr.g_GMGR_Inst.Grade_ColorArray[0];

        //초기 애니메이션 동작 막기 위해 컴포넌트 비활성화 
        m_Anim.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        //설치후 동작하게 하기 위한 BulletGroup 액티브
        if (m_State == TowerState.Inst && m_BulletGroup.activeSelf == true) {
            m_State = TowerState.Idle;

        }


        if (m_State != TowerState.Inst) {


            //공격 델타타임이 0 이상일때
            if (m_AttackDelta_Time > 0)
            {
                //감소
                m_AttackDelta_Time -= Time.deltaTime;


                //공격속도가 느릴시 애니메이션 후 다시 Idle로
                //애니메이션 state 파라미터 변경
                if (m_AttackSpped >= 1.0f)
                {
                    m_State = TowerState.Idle;
                    m_Anim.SetInteger("state", (int)m_State);
                    m_Anim.speed = 1.0f;
                }
            }


            //범위 안에 들어오면 범위안에 적 모두 공격
            //몬스터 리스트안에 몬스터가 있다면
            if (m_TargetList.Count != 0 && m_AttackDelta_Time <= 0) {

                if (m_State != TowerState.Attack){
                    m_State = TowerState.Attack;
                    m_Anim.speed = (1 / m_AttackSpped < 1 ? 1 : 1 / m_AttackSpped);

                }

                if(m_Anim.enabled == false)
                {
                    m_Anim.enabled = true;
                }


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
                Attack();

                m_AttackDelta_Time = m_AttackSpped;

            }
            else if (m_TargetList.Count == 0)
            {
                this.GetComponent<SpriteRenderer>().sprite = m_Idle_Sprite;
                m_Anim.enabled = false;


            }
        }
    }





    //공격 메소드
    void Attack () {
        //타겟리스트 갱신
        TargetList_Update();

        if (m_TargetList.First != null) {
            node = m_TargetList.First;

            SoundMgr.g_inst.EffectPlay(SoundMgr.g_inst.CrabAttack_Sound);

            //현재 몬스터 리스트에 있는 모든 몬스터들 공격
            for (int i = 0; i < m_TargetList.Count; i++) {

                node.Value.gameObject.GetComponent<Monster>().TakeDamage(m_Damage, TowerName.Crab, this.gameObject);

                if (node.Next == null) {
                    break;
                }

                node = node.Next;
            }

            node = null;
        }
    }




    //타겟 리스트 갱신 메소드
    void TargetList_Update () {

        if (m_TargetList.First != null) {
            node = m_TargetList.First;

            for (int i = 0; i < m_TargetList.Count; i++) {

                //메모리풀로 반환된 몬스터가 리스트내 있을 경우 타겟 리스트에서 제거
                if (node != null && node.Value.gameObject.activeSelf == false) {
                    m_TargetList.Remove(node);

                }

                if (node.Next == null) {
                    break;
                }

                node = node.Next;
            }

            node = null;
        }
    }



    //콜라이더 안으로 들어올때
    private void OnTriggerEnter (Collider other) {

        //몬스터 리스트에 추가
        if (other.tag == "Monster") {
            m_TargetList.AddLast(other.gameObject);
        }
    }


    //콜라이더 벗어나면
    private void OnTriggerExit (Collider other) {

        //몬스터 리스트에서 해당 몬스터 제거
        if (other.tag == "Monster") {
            if (m_TargetList.Find(other.gameObject) != null){

                m_TargetList.Remove(other.gameObject);
            }
        }
    }


   
}
