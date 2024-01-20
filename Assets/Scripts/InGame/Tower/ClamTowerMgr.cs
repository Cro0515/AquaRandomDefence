using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPoolClass;







public class ClamTowerMgr : MonoBehaviour
{

    [HideInInspector] public Grade  m_Grade;        //등급
    [HideInInspector] public int    m_Damage;       //공격력
    [HideInInspector] public float  m_Range;        //범위
    [HideInInspector] public float  m_AttackSpped;  //공격속도  0.1~10
    [HideInInspector] public TowerState m_State = TowerState.Inst;


    float m_AttackDelta_Time = 0.0f;


    [HideInInspector] public GameObject m_BulletObj;      //총알 오브젝트
    [HideInInspector] public GameObject m_BulletGroup;    //총알 오브젝트 그룹
    [HideInInspector] public GameObject m_BulletPool;     //총알 오브젝트 풀(게임 오브젝트)

    [HideInInspector] public ObjectPool ObjPool_Bullet;                                            //총알 오브젝트 풀 라이브러리
    [HideInInspector] public LinkedList<GameObject> m_TargetList = new LinkedList<GameObject>();   //타겟 리스트 (링크드 리스트)
    [HideInInspector] LinkedListNode<GameObject> node;                                                               //타겟 리스트 갱신용 노드 변수

    [HideInInspector] TowerStatus TowerStatus = null; //타워정보 인스턴스 변수

    [HideInInspector] public SpriteRenderer m_GradeSpritRenderer;    //타워 등급 스프라이트 렌더

    [HideInInspector] public Animator m_Anim;


    void Awake () {

        TowerStatus = TowerStatus.Inst;
        m_BulletObj = Resources.Load<GameObject>("Prefab/Bullet/Clam_Pearl");
        m_BulletGroup = this.gameObject.transform.parent.Find("BulletGroup").gameObject;
        m_BulletPool = this.gameObject.transform.parent.Find("BulletPool").gameObject;
        ObjPool_Bullet = this.gameObject.AddComponent<ObjectPool>();

        m_GradeSpritRenderer = this.gameObject.transform.Find("Grade").GetComponent<SpriteRenderer>();

        m_Anim = GetComponent<Animator>();
    }



    // Start is called before the first frame update
    void Start()
    {

        //타워 정보 가져오기
        m_Grade = Grade.Rare;
        m_Damage = TowerStatus.ClamTower.m_Damage[(int)m_Grade];
        m_Range = TowerStatus.ClamTower.m_Range[(int)m_Grade];
        m_AttackSpped = TowerStatus.ClamTower.m_AttackSpeed[(int)m_Grade];
        //


        //공격범위 만큼 Circle 반경 증가
        this.transform.Find("Range").transform.localScale = new Vector3(m_Range, m_Range, m_Range);
        //

        //총알 오브젝트 풀 생성
        ObjPool_Bullet.Create_ObjPool(m_BulletObj,10, m_BulletPool.transform, m_BulletGroup.transform);
        //
        
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
        if (m_State == TowerState.Inst && m_BulletGroup.activeSelf == true) {
            m_State = TowerState.Idle;
            m_Anim.enabled = true;

        }

        if(m_State != TowerState.Inst) {

            //공격 델타타임이 0 이상일때
            if (m_AttackDelta_Time > 0) {
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

            //쿨타임에 따른 공격
            //Debug.Log(m_TargetList.Count);
            //타겟 리스트에 타겟이 있고 && 공격 델타타임 0 이하일때
            if (m_TargetList.Count != 0 && m_AttackDelta_Time <= 0 ) {


                //애니메이션 state 파라미터 변경
                if (m_State != TowerState.Attack){
                    m_State = TowerState.Attack;
                    m_Anim.SetInteger("state",(int)m_State);

                    m_Anim.speed = (1/m_AttackSpped < 1 ? 1 : 1/m_AttackSpped);
                }

                //타겟 몬스터 바라보기
                if ( m_TargetList.First.Value.transform.position.x > this.transform.parent.position.x) {
                    //this.transform.parent.localScale = new Vector3(-1, 1, 1);
                    this.transform.localScale = new Vector3(-1, 1, 1);
                }
                else {
                    //this.transform.parent.localScale = new Vector3(1, 1, 1);
                    this.transform.localScale = new Vector3(1, 1, 1);
                }

                

                //공격
                AttackTarget();


                //공격 델타 초기화
                m_AttackDelta_Time = m_AttackSpped;
            }
            else if(m_TargetList.Count == 0){
                m_State = TowerState.Idle;
                m_Anim.SetInteger("state", (int)m_State);
                m_Anim.speed = 1.0f;

            }
        }

        if (m_TargetList.First != null) {

            node = m_TargetList.First;
            //타겟 리스트 갱신
            for (int i = 0; i <= m_TargetList.Count-1; i++) {

                //메모리풀로 반환된 몬스터가 리스트내 있을경우 타겟 리스트에서 제거
                //Debug.Log(node);
                //Debug.Log(node.Value.gameObject);
                if (node != null) {
                    if (node.Value.gameObject.activeSelf == false) {
                        m_TargetList.Remove(node);
                    }

                    node = node.Next;
                }

                
            }
        }
    }

 

    void AttackTarget () {


        /*
        //오브젝트 풀로 대체
        ////총알 오브젝트 생성
        //GameObject BulletObj = (GameObject)Instantiate(m_BulletObj);
        ////총알 오브젝트 부모를 BulletGroup으로 설정
        //BulletObj.transform.parent = m_BulletGroup.transform;
        */

        //총알 발사
        //오브젝트 풀에서 총알 꺼낸뒤, 총알 스크립트의 스폰함수로 총알 생성
        BulletCtrl a_BulletSC = ObjPool_Bullet.PopObject().GetComponent<BulletCtrl>();
        a_BulletSC.BulletSpawn(m_TargetList.First.Value, this.gameObject.transform.position, ObjPool_Bullet);
        //

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
