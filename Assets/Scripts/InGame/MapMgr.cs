using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    백그라운드, 디자인, 길 생성 / 길찾기(추후 별도 분리 예정)

*/

public enum DT  //DesignTile
{
    N,  //없음
    G1, //풀1
    G2, //풀2
    G3, //풀3
    S1, //돌1
    S2, //돌2
    S3, //돌3
    W1, //물결1
    W2, //물결2
}






public class MapMgr : MonoBehaviour
{
    public static MapMgr Inst;  //싱글턴 인스턴스 변수
    GameMgr GameMgr = null;     //게임 매니저 인스턴스 변수


    public GameObject Road_Obj;
    public GameObject Design_Obj;
    public GameObject Background_Obj;

    //리소스 타일 목록
    private GameObject BackTile_Obj;

    private GameObject DesignTile_Grass1;
    private GameObject DesignTile_Grass2;
    private GameObject DesignTile_Grass3;
    private GameObject DesignTile_Stone1;
    private GameObject DesignTile_Stone2;
    private GameObject DesignTile_Stone3;
    private GameObject DesignTile_WaterCurrent1;
    private GameObject DesignTile_WaterCurrent2;

    private GameObject RoadTile_LeftTop;
    private GameObject RoadTile_CenterTop;
    private GameObject RoadTile_RightTop;
    private GameObject RoadTile_LeftMiddle;
    private GameObject RoadTile_RightMiddle;
    private GameObject RoadTile_LeftBottom;
    private GameObject RoadTile_CenterBottom;
    private GameObject RoadTile_RightBottom;
    //리소스 타일 목록


    public int[,] RoadArray;


    void Awake()
    {
        Inst = this;
        GameMgr = GameMgr.g_GMGR_Inst;

        //리소스 로드
        ResourceLoad();
    }

    // Start is called before the first frame update
    void Start ()
    {

        //백그라운드 생성
        BackGroundDraw();

        //디자인 생성
        DesignDarw();

        //길 생성
        RoadDraw();

        //길찾기(A*) 알고리즘 
        PathFindeAlg.Inst.PathFind(RoadArray);



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    //백그라운드 타일 그리기
    void BackGroundDraw()
    {
        if (BackTile_Obj != null)
            for (int x = 0; x < 21; x++)
            {
                for (int z = 0; z < 12; z++)
                {
                    Instantiate(BackTile_Obj, new Vector3(x, Background_Obj.transform.position.y, z),
                            BackTile_Obj.transform.rotation).transform.parent = Background_Obj.transform;
                }
            }
    }

    //디자인 타일 그리기
    void DesignDarw()
    {
        //디자인 배열 입력 //추후 파일 가져오기로 변경
        DT[,] DesignArray = new DT[,] {
            {DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N},
            {DT.N,DT.N,DT.N,DT.N,DT.N,DT.G1,DT.W1,DT.N,DT.S1,DT.N,DT.N,DT.N},
            {DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N},
            {DT.N,DT.N,DT.N,DT.N,DT.S2,DT.N,DT.N,DT.N,DT.N,DT.N,DT.W1,DT.N},
            {DT.N,DT.N,DT.N,DT.N,DT.W1,DT.N,DT.N,DT.W2,DT.N,DT.N,DT.S3,DT.N},
            {DT.N,DT.N,DT.W2,DT.N,DT.G1,DT.N,DT.N,DT.G3,DT.N,DT.N,DT.G1,DT.N},
            {DT.N,DT.N,DT.W1,DT.N,DT.G2,DT.N,DT.N,DT.W1,DT.N,DT.N,DT.N,DT.N},
            {DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.W2,DT.N,DT.N,DT.N,DT.N},
            {DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.W2,DT.N},
            {DT.N,DT.N,DT.N,DT.N,DT.N,DT.S2,DT.N,DT.N,DT.N,DT.N,DT.G3,DT.N},
            {DT.N,DT.N,DT.G1,DT.N,DT.N,DT.W1,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N},
            {DT.N,DT.N,DT.G3,DT.N,DT.N,DT.N,DT.N,DT.N,DT.S1,DT.N,DT.N,DT.N},
            {DT.N,DT.N,DT.S3,DT.N,DT.N,DT.N,DT.N,DT.G3,DT.N,DT.N,DT.W2,DT.N},
            {DT.N,DT.N,DT.W1,DT.N,DT.W1,DT.N,DT.N,DT.G2,DT.N,DT.N,DT.W1,DT.N},
            {DT.N,DT.N,DT.N,DT.N,DT.W2,DT.N,DT.N,DT.W1,DT.N,DT.N,DT.W2,DT.N},
            {DT.N,DT.N,DT.N,DT.N,DT.W2,DT.N,DT.N,DT.N,DT.N,DT.N,DT.S2,DT.N},
            {DT.N,DT.N,DT.G3,DT.N,DT.S2,DT.N,DT.N,DT.N,DT.N,DT.N,DT.G2,DT.N},
            {DT.N,DT.N,DT.G1,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N},
            {DT.N,DT.N,DT.G2,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.N},
            {DT.N,DT.N,DT.N,DT.N,DT.N,DT.N,DT.G1,DT.G2,DT.N,DT.W1,DT.N,DT.N},
            {DT.N,DT.N,DT.N,DT.N,DT.N,DT.W1,DT.N,DT.N,DT.N,DT.W2,DT.N,DT.N}

        };


        //안내용 그리드 x,z값 전달 및 그리드 생성
        GameMgr.g_GMGR_Inst.Grid_X = DesignArray.GetLength(0);
        GameMgr.g_GMGR_Inst.Grid_Z = DesignArray.GetLength(1);
        GameMgr.g_GMGR_Inst.DrawGrid();
        //안내용 그리드 x,z값 전달 및 그리드 생성



        //타일 생성 배치
        for (int x = 0; x < DesignArray.GetLength(0); x++)
        {
            for (int z = 0; z < DesignArray.GetLength(1); z++)
            {
                if(DesignArray[x,z] == DT.N)
                    continue;
                
                else if(DesignArray[x, z] == DT.G1)
                    Instantiate(DesignTile_Grass1, new Vector3(x, Design_Obj.transform.position.y, z),
                        DesignTile_Grass1.transform.rotation).transform.parent = Design_Obj.transform;
                
                else if (DesignArray[x, z] == DT.G2)
                    Instantiate(DesignTile_Grass2, new Vector3(x, Design_Obj.transform.position.y, z),
                        DesignTile_Grass2.transform.rotation).transform.parent = Design_Obj.transform;
                
                else if (DesignArray[x, z] == DT.G3)
                    Instantiate(DesignTile_Grass3, new Vector3(x, Design_Obj.transform.position.y, z),
                        DesignTile_Grass3.transform.rotation).transform.parent = Design_Obj.transform;
                
                else if (DesignArray[x, z] == DT.S1)
                    Instantiate(DesignTile_Stone1, new Vector3(x, Design_Obj.transform.position.y, z),
                        DesignTile_Stone1.transform.rotation).transform.parent = Design_Obj.transform;
                
                else if (DesignArray[x, z] == DT.S2)
                    Instantiate(DesignTile_Stone2, new Vector3(x, Design_Obj.transform.position.y, z),
                        DesignTile_Stone2.transform.rotation).transform.parent = Design_Obj.transform;
                
                else if (DesignArray[x, z] == DT.S3)
                    Instantiate(DesignTile_Stone3, new Vector3(x, Design_Obj.transform.position.y, z),
                        DesignTile_Stone3.transform.rotation).transform.parent = Design_Obj.transform;
                
                else if (DesignArray[x, z] == DT.W1)
                    Instantiate(DesignTile_WaterCurrent1, new Vector3(x, Design_Obj.transform.position.y, z),
                        DesignTile_WaterCurrent1.transform.rotation).transform.parent = Design_Obj.transform;
                
                else
                    Instantiate(DesignTile_WaterCurrent2, new Vector3(x, Design_Obj.transform.position.y, z),
                       DesignTile_WaterCurrent2.transform.rotation).transform.parent = Design_Obj.transform;
            }
        }

    }

    //길 타일 그리기
    void RoadDraw()
    {
        //DB가져오기
        //글로벌 데이터에 게임넘버 저장
        //게임넘버 기준으로 


        RoadArray = GlobalGameData.m_Map_Array;


        /*
        //길 그리기 //추후 파일 가져오기로 변경
        RoadArray = new int[,] {
            {-1,-1,-1,-1,-1,-1,-1,-1,-1,7,-1,-1},
            {-1,0,0,0,0,0,0,0,0,2,0,-1},
            {-1,0,0,6,4,4,1,0,0,7,0,-1},
            {-1,0,0,7,0,0,2,0,0,2,0,-1},
            {-1,0,0,2,0,0,2,0,0,7,0,-1},
            {-1,0,0,7,0,0,7,0,0,2,0,-1},
            {-1,0,0,2,0,0,2,0,0,7,0,-1},
            {-1,0,0,7,0,0,7,0,0,2,0,-1},
            {-1,0,0,2,0,0,2,0,0,7,0,-1},
            {-1,0,0,7,0,0,7,0,0,2,0,-1},
            {-1,0,0,2,0,0,2,0,0,7,0,-1},
            {-1,0,0,7,0,0,7,0,0,2,0,-1},
            {-1,0,0,2,0,0,2,0,0,7,0,-1},
            {-1,0,0,7,0,0,7,0,0,2,0,-1},
            {-1,0,0,2,0,0,2,0,0,7,0,-1},
            {-1,0,0,7,0,0,7,0,0,2,0,-1},
            {-1,0,0,2,0,0,2,0,0,7,0,-1},
            {-1,0,0,7,0,0,7,0,0,2,0,-1},
            {-1,0,0,2,0,0,8,4,5,3,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,-1,-1,2,-1,-1,-1,-1,-1,-1,-1,-1}

            
            { -1,-1,-1,-1,-1,-1,-1,-1,-1,7,-1,-1},
            {-1,0,0,0,0,0,0,0,0,2,0,-1},
            {-1,0,0,6,4,4,1,0,0,7,0,-1},
            {-1,0,0,7,0,0,2,0,0,2,0,-1},
            {-1,0,0,2,0,0,2,0,0,7,0,-1},
            {-1,0,0,7,0,0,8,4,5,3,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1}, 
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1 },
            {-1,0,0,2,0,0,0,0,0,0,0,-1 },
            {-1,-1,-1,7,-1,-1,-1,-1,-1,-1,-1,-1}
            
            
            
            {-1,-1,-1,2,-1,-1,-1,-1,-1,-1,-1,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,0,0,2,0,0,0,0,0,0,0,-1},
            {-1,0,0,7,0,0,0,0,0,0,0,-1},
            {-1,-1,-1,2,-1,-1,-1,-1,-1,-1,-1,-1}
            
            
        };
         */



        for (int x = 0; x < RoadArray.GetLength(0); x++) {
            for (int z = 0; z < RoadArray.GetLength(1); z++) {

                if (RoadArray[x, z] == 0)
                    continue;

                else if(RoadArray[x,z] == 1)
                    Instantiate(RoadTile_LeftTop, new Vector3(x, Road_Obj.transform.position.y, z),
                        RoadTile_LeftTop.transform.rotation).transform.parent = Road_Obj.transform;

                else if (RoadArray[x, z] == 2)  
                    Instantiate(RoadTile_CenterTop, new Vector3(x, Road_Obj.transform.position.y, z),
                        RoadTile_CenterTop.transform.rotation).transform.parent = Road_Obj.transform;

                else if (RoadArray[x, z] == 3)
                    Instantiate(RoadTile_RightTop, new Vector3(x, Road_Obj.transform.position.y, z),
                        RoadTile_RightTop.transform.rotation).transform.parent = Road_Obj.transform;

                else if (RoadArray[x, z] == 4)
                    Instantiate(RoadTile_LeftMiddle, new Vector3(x, Road_Obj.transform.position.y, z),
                       RoadTile_LeftMiddle.transform.rotation).transform.parent = Road_Obj.transform;

                else if (RoadArray[x, z] == 5)
                    Instantiate(RoadTile_RightMiddle, new Vector3(x, Road_Obj.transform.position.y, z),
                       RoadTile_RightMiddle.transform.rotation).transform.parent = Road_Obj.transform;

                else if (RoadArray[x, z] == 6)
                    Instantiate(RoadTile_LeftBottom, new Vector3(x, Road_Obj.transform.position.y, z),
                       RoadTile_LeftBottom.transform.rotation).transform.parent = Road_Obj.transform;

                else if (RoadArray[x, z] == 7)
                    Instantiate(RoadTile_CenterBottom, new Vector3(x, Road_Obj.transform.position.y, z),
                       RoadTile_CenterBottom.transform.rotation).transform.parent = Road_Obj.transform;

                else if (RoadArray[x, z] == 8)
                    Instantiate(RoadTile_RightBottom, new Vector3(x, Road_Obj.transform.position.y, z),
                       RoadTile_RightBottom.transform.rotation).transform.parent = Road_Obj.transform;
            }
        }
    }




    void ResourceLoad () {
        BackTile_Obj = Resources.Load<GameObject>("Prefab/Tile/Back_Tile");

        DesignTile_Grass1 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Grass1");
        DesignTile_Grass2 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Grass2");
        DesignTile_Grass3 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Grass3");
        DesignTile_Stone1 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Stone1");
        DesignTile_Stone2 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Stone2");
        DesignTile_Stone3 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Stone3");
        DesignTile_WaterCurrent1 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_WaterCurrent1");
        DesignTile_WaterCurrent2 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_WaterCurrent2");

        RoadTile_LeftTop = Resources.Load<GameObject>("Prefab/Tile/RoadTile_LeftTop");
        RoadTile_CenterTop = Resources.Load<GameObject>("Prefab/Tile/RoadTile_CenterTop");
        RoadTile_RightTop = Resources.Load<GameObject>("Prefab/Tile/RoadTile_RightTop");
        RoadTile_LeftMiddle = Resources.Load<GameObject>("Prefab/Tile/RoadTile_LeftMiddle");
        RoadTile_RightMiddle = Resources.Load<GameObject>("Prefab/Tile/RoadTile_RightMiddle");
        RoadTile_LeftBottom = Resources.Load<GameObject>("Prefab/Tile/RoadTile_LeftBottom");
        RoadTile_CenterBottom = Resources.Load<GameObject>("Prefab/Tile/RoadTile_CenterBottom");
        RoadTile_RightBottom = Resources.Load<GameObject>("Prefab/Tile/RoadTile_RightBottom");

    }

}