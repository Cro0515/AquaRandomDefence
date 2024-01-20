using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPoolClass;

public class BulletCtrl : MonoBehaviour
{

    [HideInInspector]public GameObject m_TargetObj; //타겟

    private Vector3 m_StartPos = Vector3.zero;      //시작 위치
    private Vector3 m_DirTgVec = Vector3.zero;      //방향 벡터
    private Vector3 m_TowerPos = Vector3.zero;      //타워 위치
    private Vector3 a_MoveNextStep;                 //계산용 벡터

    private float m_BulletSpeed = 10.0f;            //총알 속도

    private GameObject m_TowerObj;

    ObjectPool m_BulletObjPool;
    int m_Damage;
    TowerName m_TName;

    private float m_DebuffTime = 0.0f;
    //bool IsSpawn = false;


    





    // Start is called before the first frame update
    void Start()
    {
        //타워 구별
        DistinctionTower();
        m_TowerObj = transform.parent.gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        //타겟이 없다면 제거
        if (m_TargetObj == null || m_TargetObj.activeSelf == false) {
            this.gameObject.gameObject.SetActive(false);
            m_BulletObjPool.PushObject(this.gameObject);
        }
        else {
            //방향벡터 = 총알->타겟
            m_DirTgVec = m_TargetObj.transform.position - this.gameObject.transform.position;
            m_DirTgVec.y = 0;
            m_DirTgVec.Normalize();

            //이동
            a_MoveNextStep = m_DirTgVec * (Time.deltaTime * m_BulletSpeed);
            a_MoveNextStep.y = 0;


            this.transform.position = transform.position + a_MoveNextStep; 

            //회전
            float dz = m_TargetObj.transform.position.z - transform.position.z;
            float dx = m_TargetObj.transform.position.x - transform.position.x;

            float rotateDegree = Mathf.Atan2(dz, dx) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(90, 0f, rotateDegree);

            //회전
        }

    }

    public void BulletSpawn (GameObject _TargetObj, Vector3 _TowerPos, ObjectPool _BulletObjPool ) {

        if (_TargetObj == null) {
            this.gameObject.gameObject.SetActive(false);
            m_BulletObjPool.PushObject(this.gameObject);

            return;
        }

        m_TargetObj = _TargetObj;
        m_TowerPos = _TowerPos;
        
        Vector3 a_DirVec = m_TargetObj.transform.position - m_TowerPos;
        a_DirVec.y = 0;
        a_DirVec.Normalize();
        
        m_StartPos = m_TowerPos + (a_DirVec * 0.5f);
        
        this.transform.position = new Vector3(m_StartPos.x, transform.position.y, m_StartPos.z);


        //오브젝트 풀 정보 연결 시켜주기
        m_BulletObjPool = _BulletObjPool;

    }




    public void OnTriggerEnter (Collider other) {

        if (other.gameObject == m_TargetObj) {
            //Destroy(this.gameObject);

            //데미지 주기
            other.GetComponent<Monster>().TakeDamage(m_Damage, m_TName, m_TowerObj, this.transform.position);

            //이펙트
            if (transform.parent.parent.GetComponentInChildren<ClamTowerMgr>() != null)
            {


            }
            else if (transform.parent.parent.GetComponentInChildren<PoisonPufferTowerMgr>() != null)
            {



            }


            //오브젝트 풀로 반환
            m_BulletObjPool.PushObject(this.gameObject);

        }
    }


    void DistinctionTower () {

        //조개 타워 경우
        if (transform.parent.parent.GetComponentInChildren<ClamTowerMgr>() != null) {
            m_Damage = this.transform.parent.parent.GetComponentInChildren<ClamTowerMgr>().m_Damage;
            m_TName = TowerName.Clam;
        }

        //복어 타워 경우
        if (transform.parent.parent.GetComponentInChildren<PufferTowerMgr>() != null) {
            m_Damage = this.transform.parent.parent.GetComponentInChildren<PufferTowerMgr>().m_Damage;
            m_DebuffTime = this.transform.parent.parent.GetComponentInChildren<PufferTowerMgr>().m_FreezeTime;
            m_TName = TowerName.Puffer;
        }

        //포이즌복어 타워 경우
        if (transform.parent.parent.GetComponentInChildren<PoisonPufferTowerMgr>() != null) {
            m_Damage = this.transform.parent.parent.GetComponentInChildren<PoisonPufferTowerMgr>().m_Damage;
            m_TName = TowerName.PoisonPuffer;
        }


    }


}
