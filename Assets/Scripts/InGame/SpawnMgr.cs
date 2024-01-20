using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPoolClass;
using System;




//라운드 설정(이속배수, hp배수, 스폰딜레이)
//1차 배열
[Serializable]
public class RoundSetting {
    public RoundSetting (float _SpeedMult = 1.0f, float _HpMult = 1.0f, float _SpawnDelay = 1.0f) {
        Speed_Mult = _SpeedMult;
        Hp_Mult = _HpMult;
        Spawn_Delay = _SpawnDelay;
    }

    public float Speed_Mult, Hp_Mult, Spawn_Delay;

}


public class SpawnMgr : MonoBehaviour
{
    static public SpawnMgr Inst;                    //싱글턴 인스턴스 변수

    public int[,] MonSpawn_Array;                   //라운드 몬스터 정보 저장용 배열
    public RoundSetting[] RoundSetting_Array;       //라운드 옵션 저장용 배열
    public MonSet[] MonSet_Array;                   //몬스터 스탯 옵션 저장용 배열

    float NextRoundTime, SpawnDelay;                //다음 라운드까지 정비 시간

    MonName MonName;                                //몬스터 이름

    int MonCount = 0;                               //생성한 몬스터 개수
    int RoundCnt = 0;                               //라운드 번호

    bool Spawn_flag = false;


    //몬스터 오브젝트 풀
    [HideInInspector]
    public ObjectPool[] MonObjPool_Array;           //각 몬스터 오브젝트 풀 담을 배열(게임오브젝트)
    GameObject m_MonGroupObj;                       //몬스터 그룹 (게임오브젝트)




    void Awake()
    {
        Inst = this;
    }

    void Start(){

        //몬스터 스폰을 위한 오브젝트 풀 셋팅
        SetObjPool();



    }



    void Update()
    {
        if (Spawn_flag == true)
        {
            //몬스터 스폰 딜레이 0이상일때, 델타타임만큼 감소
            if (SpawnDelay > 0)
                SpawnDelay -= Time.deltaTime;


            //몬스터 스폰 딜레이 0이하일때,
            if (SpawnDelay <= 0 && GameMgr.g_GMGR_Inst.State != GameState.End)
            {
                GameMgr.g_GMGR_Inst.Round_Text.text = RoundCnt + 1 + "/" + GlobalGameData.m_RoundSet_Array.Length + " 라운드";

                //몬스터 생성
                CreateMon();

                //해당 라운드 끝난경우
                if ((int)MonName == System.Enum.GetNames(typeof(MonName)).Length) {
                    Debug.Log(RoundCnt + 1 + "라운드 종료");

                    //라운드가 더 있는지 없는지 판별
                    if (RoundCnt + 1 == MonSpawn_Array.GetLength(0)) {
                        Spawn_flag = false;
                        GameMgr.g_GMGR_Inst.State = GameState.End;
                        MonName = 0;
                    }
                    else {
                        Spawn_flag = false;
                        GameMgr.g_GMGR_Inst.NextRound_delt = 5.0f;
                        GameMgr.g_GMGR_Inst.State = GameState.Ready;
                        GameMgr.g_GMGR_Inst.Round_Cnt++;
                        MonName = 0;
                    }
                }
            }
            //스폰 딜레이 = 스폰 옵션배열[_RoundCnt].SpawnDelay
            //라운드배열[_RoundCnt].Mon1 개수
        }
    }



    public void RoundStart(int _RoundCnt) {

        RoundCnt = _RoundCnt;
        Spawn_flag = true;
        GameMgr.g_GMGR_Inst.Round_Text.text = RoundCnt + 1 + "/" + GlobalGameData.m_RoundSet_Array.Length + " 라운드";

    }

    MonType _monType;
    public void CreateMon()
    {
        
        //몬스터 배열에 몬스터가 0마리 인경우
        if (MonSpawn_Array[RoundCnt, (int)MonName] <= 0){
            MonName++;
            return;
        }

        //생성
        //GameObject Monster = MonObjPool_Array[(int)MonName].gameObject;
        GameObject Monster = MonObjPool_Array[(int)MonName].PopObject().gameObject;
        Monster.AddComponent<Monster>().Set_MonStatus(MonName, MonSet_Array[(int)MonName]);
        Monster.transform.parent = GameObject.Find("ActiveGroup").transform;
        
        
        
        Monster.transform.position = new Vector3(PathFindeAlg.Inst.StartPos.x, 0.1f, PathFindeAlg.Inst.StartPos.y);
        //생성


        int rHP = (int)Math.Round((float)Monster.GetComponent<Monster>().m_Hp * RoundSetting_Array[RoundCnt].Hp_Mult, 1);
        int rSpeed = (int)Math.Round((float)Monster.GetComponent<Monster>().m_Speed * RoundSetting_Array[RoundCnt].Speed_Mult, 1);
        
        Monster.GetComponent<Monster>().m_Hp = rHP;
        Monster.GetComponent<Monster>().m_Speed = rSpeed;



        MonCount++;

        //스폰 딜레이 초기화
        SpawnDelay = RoundSetting_Array[RoundCnt].Spawn_Delay;

        //해당 몬스터 전부 소환한 경우
        if (MonSpawn_Array[RoundCnt, (int)MonName] <= MonCount)
        {
            Debug.Log(RoundCnt + 1 + "라운드 / " + MonName + "몬스터 / " + MonCount + "마리 소환");
            Debug.Log("[라운드 옵션] " + " HP 배수: "+ RoundSetting_Array[RoundCnt].Hp_Mult + " / 스피드 배수: " + RoundSetting_Array[RoundCnt].Speed_Mult + " / 스폰 딜레이: " + RoundSetting_Array[RoundCnt].Spawn_Delay);

            //몬스터 카운트 초기화
            MonCount = 0;

            //몬스터 번호 증가 = 다음 몬스터
            MonName++;
            Debug.Log("다음 몬스터 : " + MonName);
        }
    }

    public void GetRoundData () {


        //라운드 몬스터 정보 가져오기
        /*
        RoundMon_Array = new int[,]{


            //{1,0,0,0,0},
            //{0,1,0,0,0},
            //{0,0,1,0,0},
            //{0,0,0,1,0},
            //{0,0,0,0,1},
            

            
            
            {5,0,0,0,0},
            {1,10,5,1,1},
            {1,1,10,5,1},
            {1,1,1,10,5},
            {1,1,1,1,10},

        };

        //*임시* 글로벌데이터에 저장
        GlobalGameData.m_MonSpawn_Array = RoundMon_Array;
        

        //라운드 옵션 가져오기
        RoundSetting_Array = new RoundSetting[RoundMon_Array.Length];
        for (int i = 0; i < RoundSetting_Array.Length; i++) {
            RoundSetting_Array[i] = new RoundSetting();
        };


        //몬스터 스탯 가져오기
        MonSet_Array = new MonSet[]
        {
            //JSON


            new MonSet(1,100,5),
            new MonSet(2,150,10),
            new MonSet(3,200,15),
            new MonSet(4,300,30),
            new MonSet(5,500,50),
        };
        */

        if (GlobalGameData.m_MonSpawn_Array != null &&
            GlobalGameData.m_RoundSet_Array != null &&
            GlobalGameData.m_MonSet_Array != null) {

            MonSpawn_Array = GlobalGameData.m_MonSpawn_Array;
            RoundSetting_Array = GlobalGameData.m_RoundSet_Array;
            MonSet_Array = GlobalGameData.m_MonSet_Array;

            Debug.Log("몬스터, 라운드 데이터 로드완료.");
        }
        else {
            Debug.Log("몬스터와 라운드 관련 데이터가 없습니다.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }
    }


    public void SetObjPool () {

        
        m_MonGroupObj = GameObject.Find("MonsterGroup").gameObject;

        //Enum에 등록된 몬스터 개수
        int MonLength = System.Enum.GetValues(typeof(MonName)).Length;

        //각 몬스터풀(게임오브젝트) 받아놓을 배열
        MonObjPool_Array = new ObjectPool[MonLength];

        //MonsterGroup 오브젝트 하위에 몬스터 종류 만큼 오브젝트 풀 생성
        for (int i = 0; i < MonLength; i++) {

            //오브젝트 풀용  빈 게임오브젝트 생성
            GameObject emptyObj = new GameObject(MonName + "Pool");

            //부모지정
            emptyObj.transform.SetParent(m_MonGroupObj.transform);

            //컴포넌트 추가 & 배열저장
            MonObjPool_Array[i] = emptyObj.AddComponent<ObjectPool>();

            //각 오브젝트 풀에 20마리씩 해당 몬스터들 생성
            MonObjPool_Array[i].Create_ObjPool((GameObject)Resources.Load("Prefab/Monster/" + MonName),
                                                50,
                                                MonObjPool_Array[i].gameObject.transform,
                                                m_MonGroupObj.transform);

            MonName++;
        }
        MonName = 0;

        

    }

}



