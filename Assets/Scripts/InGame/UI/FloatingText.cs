using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
[최대높이]까지[속도] 만큼 증가
[일정높이] 부터[속도] 감속

최대높이까지 도달시
투명도 알파값 줄이고 알파값 일정수치 이하일때 삭제
*/


public class FloatingText : MonoBehaviour {

    public Text m_floatingText = null;

    public Vector3 m_SavePos = Vector3.zero;        //시작 위치
    public float m_MaxHeight = 30.0f;               //최대로 올라갈 수 있는 높이값
    public float m_SlowHeightMult = 0.7f;               //감속 구간(배율)
    public float m_AlphaHeightMult = 0.8f;              //알파 처리 구간(배율)

    public float m_MvSpeedMax = 100.0f;             //올라갈때 속도
    public float m_MvSpeedMin = 30.0f;              //감속시 속도

    bool m_MoveState = false;                       //Up 이동상태
    bool m_AlphaState = false;                      //알파블랜드 상태

    float m_MvSpeed;                                //데미지 글씨가 위로 올라가는 속도
    private float m_EffAddTime = 0.0f;              //데미지 텍스트 연출에서 전체 시간 흐름을 의미하는 변수
    private Vector3 m_CurCacPos = Vector3.zero;     //이동 효과를 위한 계산용 변수
    float a_CacYPos = 0.0f;                         //이동 효과를 위한 계산용 변수
    float a_OldYPos = 0.0f;


    //---알파효과
    private float alpha = 0.0f;                     //알파 효과를 위한
    private Color _color;                            //마지막에 투명하게 사라지게 하기 위한 연출용 변수
    //---알파효과




    // Start is called before the first frame update
    void Start () {
        this.name = "floatingText";
    }

    // Update is called once per frame
    void Update () {


        if (m_MoveState == true) {
            if (a_CacYPos < m_MaxHeight) //위로 올라가는 이동 처리
            {
                if (m_MaxHeight * m_SlowHeightMult < a_CacYPos) //특정 구간에서 감속
                    m_MvSpeed = m_MvSpeedMin;
                else
                    m_MvSpeed = m_MvSpeedMax;

                a_OldYPos = a_CacYPos;
                a_CacYPos += Time.deltaTime * m_MvSpeed;

                if (m_MaxHeight < a_CacYPos)
                    a_CacYPos = m_MaxHeight;

                m_CurCacPos = m_SavePos;    //m_SavePos <-- 스폰 시작시 초기화 셋팅됨
                m_CurCacPos.y = m_SavePos.y + a_CacYPos;
                this.transform.position = m_CurCacPos;

                if (a_OldYPos <= m_MaxHeight * m_AlphaHeightMult && m_MaxHeight * m_AlphaHeightMult < a_CacYPos) {
                    m_AlphaState = true;
                    m_EffAddTime = 0.0f;    //변수 재활용 초기화
                }
            }
            else //도착
            {
                m_MoveState = false;
            }

            if (m_MaxHeight * 1.2f < a_CacYPos)  //예외처리
            {
                Destroy(this.gameObject);   //자폭
            }
        }

        if (m_AlphaState == true)//투명도 연출구간
         {
            m_EffAddTime += Time.deltaTime * 1.7f;
            if (0.8f < m_EffAddTime)
                m_EffAddTime = 0.8f;
            alpha = 1.0f * (m_EffAddTime / 0.8f);
            _color = m_floatingText.color;
            _color.a = 1.0f - alpha;
            m_floatingText.color = _color;

            if (0.8f <= m_EffAddTime)   //삭제
                Destroy(this.gameObject);
        }//if(m_AlphaState == true) //투명도 연출구간

    }



    public void Setting (GameObject _obj) {

        //텍스트 컴포넌트 받아오기
        m_floatingText = this.GetComponent<Text>();

        //투명도 연출위한 컬러값 저장
        _color = m_floatingText.color;
        //폰트크기 만큼 Hight 변경
        m_floatingText.GetComponent<RectTransform>().sizeDelta = new Vector2(m_floatingText.GetComponent<RectTransform>().sizeDelta.x, m_floatingText.fontSize);

        //시작위치 = 현재위치.y + hight만큼 증가;
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y + m_floatingText.GetComponent<RectTransform>().sizeDelta.y,
                                         transform.position.z);

        m_SavePos = transform.position;

        m_MoveState = true;
    }

    public void Setting (Vector3 _pos, float _width, int _size, string _text, Color _color) {

        transform.position = _pos;

        //오브젝트에 Text컴포넌트가 없다면 추가
        if (transform.GetComponent<Text>() == null) {
            m_floatingText = this.gameObject.AddComponent<Text>();
        }
        else {
            //텍스트 컴포넌트 받아오기
            m_floatingText = this.GetComponent<Text>();
        }

        //폰트 크기 및 Text의 Hight 폰트 크기만큼 변경
        m_floatingText.fontSize = _size;
        m_floatingText.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _size + 10);
        m_floatingText.text = _text;
        m_floatingText.color = _color;

        //폰트 - 기본폰트
        m_floatingText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        //텍스트 오버플로우 설정
        m_floatingText.horizontalOverflow = HorizontalWrapMode.Overflow;
        m_floatingText.verticalOverflow = VerticalWrapMode.Overflow;
        //정렬
        m_floatingText.alignment = TextAnchor.MiddleCenter;

        //시작위치 = 현재위치.y + hight만큼 증가;
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y + m_floatingText.GetComponent<RectTransform>().sizeDelta.y,
                                         transform.position.z);
        m_SavePos = transform.position;
        m_MoveState = true;
    }
}
