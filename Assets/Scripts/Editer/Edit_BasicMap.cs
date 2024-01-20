using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edit_BasicMap : MonoBehaviour
{
    //public static MapMgr Inst;  //싱글턴 인스턴스 변수
    EditorMgr Editer_MGR = null;     //게임 매니저 인스턴스 변수


    public GameObject Edit_Road_Obj;
    public GameObject Edit_Design_Obj;

    //리소스 타일 목록
                       
    private GameObject Edit_DesignTile_Grass1;
    private GameObject Edit_DesignTile_Grass2;
    private GameObject Edit_DesignTile_Grass3;
    private GameObject Edit_DesignTile_Stone1;
    private GameObject Edit_DesignTile_Stone2;
    private GameObject Edit_DesignTile_Stone3;
    private GameObject Edit_DesignTile_WaterCurrent1;
    private GameObject Edit_DesignTile_WaterCurrent2;

    //리소스 타일 목록


    public int[,] RoadArray;




    private void Awake () {


        //리소스 로드
        ResourceLoad();
    }

    // Start is called before the first frame update
    void Start()
    {
        Editer_MGR = EditorMgr.g_Inst;
        //백그라운드 생성
        //BackGroundDraw();

        //디자인 생성
        DesignDarw();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

   



    void DesignDarw () {
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
        //Debug.Log(EMGR.Grid_X + " / " + EMGR.Grid_Z);
        Editer_MGR.Grid_X = DesignArray.GetLength(0);
        Editer_MGR.Grid_Z = DesignArray.GetLength(1);
        Editer_MGR.DrawGrid();
        //안내용 그리드 x,z값 전달 및 그리드 생성



        //타일 생성 배치
        for (int x = 0; x < DesignArray.GetLength(0); x++) {
            for (int z = 0; z < DesignArray.GetLength(1); z++) {
                if (DesignArray[x, z] == DT.N)
                    continue;

                else if (DesignArray[x, z] == DT.G1)
                    Instantiate(Edit_DesignTile_Grass1, new Vector3(x, Edit_Design_Obj.transform.position.y, z),
                        Edit_DesignTile_Grass1.transform.rotation).transform.parent = Edit_Design_Obj.transform;

                else if (DesignArray[x, z] == DT.G2)
                    Instantiate(Edit_DesignTile_Grass2, new Vector3(x, Edit_Design_Obj.transform.position.y, z),
                        Edit_DesignTile_Grass2.transform.rotation).transform.parent = Edit_Design_Obj.transform;

                else if (DesignArray[x, z] == DT.G3)
                    Instantiate(Edit_DesignTile_Grass3, new Vector3(x, Edit_Design_Obj.transform.position.y, z),
                        Edit_DesignTile_Grass3.transform.rotation).transform.parent = Edit_Design_Obj.transform;

                else if (DesignArray[x, z] == DT.S1)
                    Instantiate(Edit_DesignTile_Stone1, new Vector3(x, Edit_Design_Obj.transform.position.y, z),
                        Edit_DesignTile_Stone1.transform.rotation).transform.parent = Edit_Design_Obj.transform;

                else if (DesignArray[x, z] == DT.S2)
                    Instantiate(Edit_DesignTile_Stone2, new Vector3(x, Edit_Design_Obj.transform.position.y, z),
                        Edit_DesignTile_Stone2.transform.rotation).transform.parent = Edit_Design_Obj.transform;

                else if (DesignArray[x, z] == DT.S3)
                    Instantiate(Edit_DesignTile_Stone3, new Vector3(x, Edit_Design_Obj.transform.position.y, z),
                        Edit_DesignTile_Stone3.transform.rotation).transform.parent = Edit_Design_Obj.transform;

                else if (DesignArray[x, z] == DT.W1)
                    Instantiate(Edit_DesignTile_WaterCurrent1, new Vector3(x, Edit_Design_Obj.transform.position.y, z),
                        Edit_DesignTile_WaterCurrent1.transform.rotation).transform.parent = Edit_Design_Obj.transform;

                else
                    Instantiate(Edit_DesignTile_WaterCurrent2, new Vector3(x, Edit_Design_Obj.transform.position.y, z),
                       Edit_DesignTile_WaterCurrent2.transform.rotation).transform.parent = Edit_Design_Obj.transform;
            }
        }

    }




















    void ResourceLoad () {

        Edit_DesignTile_Grass1 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Grass1");
        Edit_DesignTile_Grass2 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Grass2");
        Edit_DesignTile_Grass3 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Grass3");
        Edit_DesignTile_Stone1 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Stone1");
        Edit_DesignTile_Stone2 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Stone2");
        Edit_DesignTile_Stone3 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_Stone3");
        Edit_DesignTile_WaterCurrent1 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_WaterCurrent1");
        Edit_DesignTile_WaterCurrent2 = Resources.Load<GameObject>("Prefab/Tile/DesignTile_WaterCurrent2");


    }
}
