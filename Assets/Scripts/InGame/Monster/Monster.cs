using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;



public enum MonName {
    PinBoom = 0,    //핀붐           PinBoomb     
    Dust,           //먼지뭉치       Dust
    Bottle,         //포도주스병     Bottle
    Petroleum,      //갈매기 슬라임  Petroleum
    Trash,          //더스트박스     DustBox
}


public enum MonType {
    Normal,
    MIddleBoss,
    Boss
}


public enum MonState {
    Normal,
    Freeze,
    Poison,

}


[Serializable]
public class MonSet{
    public MonSet(float _speed, int _hp, int _damage)
    {
        Speed = _speed + 2.0f;
        Hp = _hp;
        Damage = _damage;
    }

    public float Speed;
    public int Hp, Damage;

}



public class Monster : MonoBehaviour {


    PathFindeAlg PathFindeAlg_Inst = null;  //길찾기 인스턴스 변수
    GameMgr GameMgr_Inst = null;            //게임 매니저 인스턴스 변수

    public MonName m_MonName;   //몬스터 이름
    public MonType m_MonType;   //몬스터 타입
    public float m_Speed;       //이동속도
    public int m_Hp;            //체력
    public int m_Damage;        //데미지
    public MonState m_State;

    Vector3 m_TargetPos;        //목표(웨이포인트) 벡터
    Vector3 m_DirVec;           //현재위치 -> 목표 방향 벡터
    Vector3 m_FinishPos;        //마지막 지점
    float a_CacStep;
    int nodeCnt = 0;

    GameObject m_TowerObj;


    //-------------CrabEffect
    GameObject EffectObj_Crab_1;
    GameObject EffectObj_Crab_2;
    bool attackflag = false;
    float Crab_fillamount = 0.0f;
    //-------------CrabEffect


    //-------------PufferEffect
    GameObject EffectObj_Freeze;
    public float temp_Speed;
    float Puffer_fillamount = 0.0f;

    //-------------PufferEffect

    //-------------PoisonPufferEffect
    AutoImageSlider Effect_Slider;
    float Poison_time = 0.0f;
    float Color_red = 255;
    float Color_blue = 255;
    int Poison_Damage;
    //-------------PoisonPufferEffect

    SpriteRenderer m_SpriteRenderer;




    //
    GameObject EffectGroup;

    GameObject PinBoom_DieEffect;
    GameObject Dust_DieEffect;
    GameObject Bottle_DieEffect;
    GameObject Petroleum_DieEffect;
    GameObject Trash_DieEffect;

    GameObject Clam_HitEffect;
    GameObject Poison_HitEffect;

    Animator m_Anim;



    // Start is called before the first frame update
    void Awake ()
    {
        PathFindeAlg_Inst = PathFindeAlg.Inst;
        GameMgr_Inst = GameMgr.g_GMGR_Inst;
        m_TargetPos = new Vector3(PathFindeAlg_Inst.FinalNodeList[nodeCnt].x,0.1f, PathFindeAlg_Inst.FinalNodeList[nodeCnt].y);
        m_FinishPos = new Vector3(PathFindeAlg_Inst.EndPos.x, 0.1f, PathFindeAlg_Inst.EndPos.y);



        //-------이펙트
        m_SpriteRenderer = this.GetComponent<SpriteRenderer>();

        EffectObj_Crab_1 = this.transform.Find("Effect").transform.Find("Effect_Claw1").gameObject;
        EffectObj_Crab_2 = this.transform.Find("Effect").transform.Find("Effect_Claw2").gameObject;
        EffectObj_Crab_1.SetActive(false);
        EffectObj_Crab_2.SetActive(false);

        EffectObj_Freeze = this.transform.Find("Effect").transform.Find("Effect_Freeze").gameObject;
        EffectObj_Freeze.SetActive(false);

        Effect_Slider = this.transform.Find("StatePos").gameObject.GetComponent<AutoImageSlider>();

        m_Anim = this.GetComponent<Animator>();
        EffectGroup = GameObject.Find("EffectGroup").gameObject;

        PinBoom_DieEffect = Resources.Load<GameObject>("Prefab/Monster/PinBoom_DieEffect");
        Dust_DieEffect = Resources.Load<GameObject>("Prefab/Monster/Dust_DieEffect");
        Bottle_DieEffect = Resources.Load<GameObject>("Prefab/Monster/Bottle_DieEffect");
        Petroleum_DieEffect = Resources.Load<GameObject>("Prefab/Monster/Petroleum_DieEffect");
        Trash_DieEffect = Resources.Load<GameObject>("Prefab/Monster/Trash_DieEffect");


        Clam_HitEffect = Resources.Load<GameObject>("Prefab/Effect/Effect_Clam");
        Poison_HitEffect = Resources.Load<GameObject>("Prefab/Effect/Effect_Poison");


    }


    void Start()
    {
        Debug.Log(PinBoom_DieEffect);
        Debug.Log(Dust_DieEffect);
        Debug.Log(Bottle_DieEffect);
        Debug.Log(Petroleum_DieEffect);
        Debug.Log(Trash_DieEffect);
    }

    // Update is called once per frame
    void Update()
    {

        //사망시 리턴
        if (m_Hp <= 0) {

            SoundMgr.g_inst.EffectPlay(SoundMgr.g_inst.MonsterDie_Sound);

            GameObject tempEffect;

            switch (m_MonName)
            {
                case MonName.PinBoom:
                    tempEffect = Instantiate(PinBoom_DieEffect, EffectGroup.transform);
                    break;

                case MonName.Dust:
                    tempEffect = Instantiate(Dust_DieEffect, EffectGroup.transform);
                    break;

                case MonName.Bottle:
                    tempEffect = Instantiate(Bottle_DieEffect, EffectGroup.transform);
                    break;

                case MonName.Petroleum:
                    tempEffect = Instantiate(Petroleum_DieEffect, EffectGroup.transform);
                    break;

                case MonName.Trash:
                    tempEffect = Instantiate(Trash_DieEffect, EffectGroup.transform);
                    break;

                default:
                    tempEffect = null;
                    break;
            }
            
            if(tempEffect != null) { 
                tempEffect.transform.position = this.transform.position;
                tempEffect.transform.localScale = this.transform.localScale;
            }

            

                
            //독 디버프(Invoke) 함수 해제
            AntiDote();

            //골드 추가
            GameMgr_Inst.PlusGold(m_MonType);

            //반환
            ReturnMon();
        }


        //길찾기 알고리즘을 통한 루트대로 이동
        m_DirVec = m_TargetPos - this.transform.position;
        m_DirVec.Normalize();           //방향벡터



        //루트 방향 바라보기
        Vector3 tempDirVec = this.transform.localScale;




        //오른쪽
        //방향 벡터는 양수 //실제 로컬스케일은 음수
        if ((m_DirVec.x > 0 && tempDirVec.x > 0) || (m_DirVec.x < 0 && tempDirVec.x < 0))
        {
            //기본 이미지가 왼쪽방향 바라보게 되어있음.
            
            this.transform.localScale = new Vector3(tempDirVec.x * -1.0f, tempDirVec.y, tempDirVec.z);


        }


        a_CacStep = Time.deltaTime * m_Speed;

        float dis = Vector3.Distance(m_TargetPos, this.transform.position);

        //스피드가 빠를수록 비교값이 높아야하고 // 스피드 최고 20
        //스피드가 느릴수록 비교값이 낮아야함.  // 스피드 최저 1
        if (dis >= (m_Speed * 0.03f)) {
            this.transform.position += (m_DirVec * a_CacStep);
        }
        else {
            this.transform.position = m_TargetPos;
            nodeCnt++;

            if (PathFindeAlg_Inst.FinalNodeList.Count > nodeCnt)
                m_TargetPos = new Vector3(PathFindeAlg_Inst.FinalNodeList[nodeCnt].x, 0.1f, PathFindeAlg_Inst.FinalNodeList[nodeCnt].y);
        }
        //길찾기 알고리즘을 통한 루트대로 이동



        //도트 데미지 처리
        //(Update에서 지속시간 값이 0보다 클경우 deltaTime만큼 차감
        //Poison 상태이고, 지속시간이 0일경우 AntiDote
        if (Poison_time > 0.0f) {
            Poison_time -= Time.deltaTime;
        }

        if (m_State == MonState.Poison && Poison_time <= 0.0f) {
            AntiDote();
        }
        //도트 데미지 처리




        //이펙트
        effectCrab();

        effectPuffer();

        effectPoisonPuffer();
        //



        //마지막 지점에 도착시
        if ( this.transform.position == m_FinishPos) {
            //Destroy(this.gameObject);
            AntiDote();
            ReturnMon();

            //생명력 감소
            GameMgr_Inst.MinusHeart(m_Damage);

        }
        //마지막 지점에 도착시

    }

    public void Set_MonStatus (MonName _monName, MonSet _monSet) {
        m_Speed = _monSet.Speed;
        m_Hp = _monSet.Hp;
        m_Damage = _monSet.Damage;
        m_MonName = _monName;
        m_State = MonState.Normal;

        if (_monName == MonName.Trash)
            m_MonType = MonType.Boss;

        else if (_monName == MonName.Petroleum)
            m_MonType = MonType.MIddleBoss;

        else
            m_MonType = MonType.Normal;

    }



    public void ReturnMon () {

        SpawnMgr.Inst.MonObjPool_Array[(int)m_MonName].PushObject(this.gameObject);
    }


    public void TakeDamage (int _damage, TowerName _towername, GameObject _towerobj, Vector3 _pos = default(Vector3)) {
        m_TowerObj = _towerobj;



        m_Hp -= _damage;
        GameMgr_Inst.DamageText(this.gameObject, _damage, this.transform);


        //-----------타워별 이펙트 처리
        if(_towername == TowerName.Clam)
        {
            SoundMgr.g_inst.EffectPlay(SoundMgr.g_inst.ClamHit_Sound);

            GameObject tempHitEffect = Instantiate(Clam_HitEffect, EffectGroup.transform);
            tempHitEffect.transform.position = _pos;

        }
        else if (_towername == TowerName.Crab && EffectObj_Crab_1.activeSelf == false) {


            EffectObj_Crab_1.SetActive(true);
            attackflag = true;

        }
        else if (_towername == TowerName.Puffer) {
            SoundMgr.g_inst.EffectPlay(SoundMgr.g_inst.MonsterHit_Sound);

            if (m_Speed > 0.0f) {
                Freeze();
                Invoke("Invoke_UnFreeze", _towerobj.GetComponentInChildren<PufferTowerMgr>().m_FreezeTime);
            }
        }
        else if (_towername == TowerName.PoisonPuffer) {
            SoundMgr.g_inst.EffectPlay(SoundMgr.g_inst.MonsterHit_Sound);

            GameObject tempHitEffect = Instantiate(Poison_HitEffect, EffectGroup.transform);
            tempHitEffect.transform.position = _pos;

            //디버프 지속시간 받아오기
            Poison_time = _towerobj.GetComponentInChildren<PoisonPufferTowerMgr>().m_PoisonTime;

            //독상태가 아닐경우
            if (m_State != MonState.Poison) {

                m_State = MonState.Poison;

                //슬라이드 이펙트 생성 및 시작
                Effect_Slider.CreateSlider("Image/Poison", 0.1f);
                Effect_Slider.StartSlide();

                //1초 주기로 DoteDMG 실행
                Poison_Damage = m_TowerObj.GetComponentInChildren<PoisonPufferTowerMgr>().m_PoisonDamage;
                InvokeRepeating("Invoke_DoteDMG", 1.0f, 1.0f);

                //(Update에서 지속시간 값이 0보다 클경우 deltaTime만큼 차감
                //Poison 상태이고, 지속시간이 0일경우 AntiDote

            }


        }

        //-----------타워별 처리


    }


    public void effectCrab () {


        if (EffectObj_Crab_1.activeSelf == true && attackflag == true) {

            Crab_fillamount += Time.deltaTime * 5;
            EffectObj_Crab_1.GetComponent<SpriteRenderer>().material.SetFloat("_Cutoff", Crab_fillamount);

            if (Crab_fillamount >= 1.0f) {

                Crab_fillamount = 0.0f;
                EffectObj_Crab_1.GetComponent<SpriteRenderer>().material.SetFloat("_Cutoff", Crab_fillamount);
                EffectObj_Crab_1.SetActive(false);
                EffectObj_Crab_2.SetActive(true);
            }
        }
        else if (EffectObj_Crab_2.activeSelf == true && attackflag == true) {

            Crab_fillamount += Time.deltaTime * 5;
            EffectObj_Crab_2.GetComponent<SpriteRenderer>().material.SetFloat("_Cutoff", Crab_fillamount);

            if (Crab_fillamount >= 1.0f) {

                Crab_fillamount = 0.0f;
                EffectObj_Crab_2.GetComponent<SpriteRenderer>().material.SetFloat("_Cutoff", Crab_fillamount);
                EffectObj_Crab_2.SetActive(false);
                attackflag = false;
            }
        }
    }





    public void effectPuffer () {

        if (EffectObj_Freeze.activeSelf == true && Puffer_fillamount < 1.0f) {

            Puffer_fillamount += Time.deltaTime * 7;
            EffectObj_Freeze.GetComponentInChildren<SpriteRenderer>().material.SetFloat("_Cutoff", Puffer_fillamount);
            
        }

    }

    public void Freeze () {
        m_State = MonState.Freeze;
        //애니메이션 해제
        m_Anim.enabled = false;

        EffectObj_Freeze.SetActive(true);

        temp_Speed = m_Speed;
        m_Speed = 0.0f;


    }


    public void Invoke_UnFreeze () {
        //애니메이션 적용
        m_Anim.enabled = true;

        m_Speed = temp_Speed;

        Puffer_fillamount = 0.0f;
        EffectObj_Freeze.GetComponentInChildren<SpriteRenderer>().material.SetFloat("_Cutoff", Puffer_fillamount);
        EffectObj_Freeze.SetActive(false);
        m_State = MonState.Normal;
    }








    public void effectPoisonPuffer () {
        

        if (m_State == MonState.Poison) {

            //color값 초록색
            m_SpriteRenderer.color = new Color32(160, 80, 255, 255);

            //Debug.Log(m_SpriteRenderer.color);

            //0일때 증가

        }
    }

    public void Invoke_DoteDMG () {
        m_Hp -= Poison_Damage;

        GameMgr_Inst.DamageText(this.gameObject, Poison_Damage, this.transform, new Color32(100,0,200,255));
    }

    public void AntiDote () {

        CancelInvoke("DoteDMG");

        //이펙트 해제
        Effect_Slider.StopSlide();
        m_SpriteRenderer.color = new Color32(255, 255, 255, 255);
        //상태 변경
        m_State = MonState.Normal;
    }





    

}
