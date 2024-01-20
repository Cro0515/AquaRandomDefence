using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum NodeType {
    Wall = -1,   //벽
    None,   //빈공간
    Road,   //길
    Tower,  //타워
}

public class PathFindeAlg : MonoBehaviour
{
   
    //메인 노드
    public class Node {
        public Node (int _x, int _y, NodeType _Type) {
            x = _x;
            y = _y;
            Type = _Type;
        }
        public Node ParentNode;

        public NodeType Type;
        public int x, y, G, H;
        public int F {
            get {
                return G + H;
            }
        }

    }

    static public PathFindeAlg Inst;    //싱글턴 인스턴스 변수


    //메인노드 및 A*알고리즘 관련 변수
    public Node[,] NodeArray;  //메인 노드(벽, 길, 타워, 빈곳 정보)
    public List<Node> FinalNodeList;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;

    [HideInInspector]
    public Vector2Int StartPos, EndPos, bottomLeft, topRight;


    public void Awake () {
        Inst = this;
    }

    //매핑 및 길찾기(A*) 알고리즘
    public void PathFind (int[,] _RoadArray) {
        
        bottomLeft = new Vector2Int(0, 0);
        topRight = new Vector2Int(_RoadArray.GetLength(0), _RoadArray.GetLength(1));

        //A* 용 Node 배열 생성
        NodeArray = new Node[_RoadArray.GetLength(0), _RoadArray.GetLength(1)];
        NodeType NType;

        //타일 배열과 매핑
        for (int x = 0; x < _RoadArray.GetLength(0); x++) {
            for (int z = 0; z < _RoadArray.GetLength(1); z++) {

                //0보다 작을경우 벽
                if (_RoadArray[x, z] < 0)
                    NType = NodeType.Wall;
                //0보다 클경우 길
                else if (_RoadArray[x, z] > 0)
                    NType = NodeType.Road;
                //0일경우 빈공간
                else
                    NType = NodeType.None;

                //x = 0 일때, start
                if (StartPos == new Vector2Int(0, 0) && _RoadArray[0, z] > 0) {
                    StartPos = new Vector2Int(0, z);
                    NType = NodeType.Wall;
                    Debug.Log("StartPos : " + StartPos);
                }

                //x = 20 일때, end
                if (EndPos == new Vector2Int(0, 0) && _RoadArray[20, z] > 0) {
                    EndPos = new Vector2Int(20, z);
                    NType = NodeType.Wall;
                    Debug.Log("EndPos : " + EndPos);

                }

                NodeArray[x, z] = new Node(x, z, NType);
            }
        }

        //시작노드 끝노드  
        StartNode = NodeArray[StartPos.x - bottomLeft.x, StartPos.y - bottomLeft.y];
        TargetNode = NodeArray[EndPos.x - bottomLeft.x, EndPos.y - bottomLeft.y];


        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        //길찾기(A*) 메인 로직
        while (OpenList.Count > 0) {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H)
                    CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // 마지막
            if (CurNode == TargetNode) {
                Node TargetCurNode = TargetNode;
                while (TargetCurNode != StartNode) {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                //인스턴스 변수 통해 게임 상태값 변경
                GameMgr.g_GMGR_Inst.State = GameState.Ready;
                return;
            }


            // 상하좌우
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }


    void OpenListAdd (int checkX, int checkY) {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 &&
            NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].Type == NodeType.Road &&
            !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y])) {

            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
            Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
            int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode)) {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }



}
