using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DamageText : MonoBehaviour {
    bool m_PunchState = true;   //
    bool m_MoveState = false;   //Up 이동상태
    bool m_AlphaState = false;  //알파블랜드 상태
    float m_MvSpeed = 7.7f;     //데미지 글씨가 위로 올라가는 속도

    public Text m_RefText = null;
    public Color _color = new Color32(99, 255, 139, 255);   //마지막에 투명하게 사라지게 하기 위한 연출용 변수
    public Vector3 m_SavePos = Vector3.zero;        //시작 위치
    private Vector3 m_SaveScale = Vector3.zero;     //시작 사이즈
    private float m_TargetScale = 0.07f;            //사이즈 연출에서 펀칭효과의 최종 목표사이즈
    public float m_DamageVel = 0.0f;                //표시 해 줄 데미지 값


    private float m_EffAddTime = 0.0f;              //데미지 텍스트 연출에서 전체 시간 흐름을 의미하는 변수
    private float m_OldEffTime = 0.0f;              //한번 발생하게 하기 위한 계산용 변수
    float a_CacScale = 0.0f;                        //펀칭 효과를 위한 계산용 변수

    float m_MaxHeight = 0.5f;                       //최대로 올라갈 수 있는 높이값
    private Vector3 m_CurCacPos = Vector3.zero;     //이동 효과를 위한 계산용 변수
    float a_CacYPos = 0.0f;                         //이동 효과를 위한 계산용 변수
    float a_OldYPos = 0.0f;

    private float alpha = 0.0f;                     //알파 효과를 위한

    public GameObject m_target;

    private void Awake () {

        m_RefText = this.gameObject.GetComponent<Text>();
    }


    // Start is called before the first frame update
    void Start () {

        if (m_RefText != null) {
            _color = m_RefText.color;

            m_RefText.text = "-" + string.Format("{0:#,###}",m_DamageVel).ToString();
        }

        m_SaveScale = this.transform.localScale;
        a_CacScale = m_TargetScale;

        if (m_target == null || m_target.activeSelf == false) {
            Destroy(this.gameObject);
        }

    }

    // Update is called once per frame.
    void Update () {

        if (m_target == null || m_target.activeSelf == false) {
            Destroy(this.gameObject);
        }


        if (m_PunchState == true) { //펀칭 효과

            m_OldEffTime = m_EffAddTime;
            m_EffAddTime += Time.deltaTime;

            if (0.033f < m_EffAddTime) {

                if (m_OldEffTime <= 0.033f) {

                    this.transform.localScale = new Vector3(m_TargetScale, m_TargetScale, 1.0f);
                    a_CacScale = m_TargetScale;
                    return;
                }

                a_CacScale *= 0.85f;

                if (a_CacScale < m_SaveScale.x)
                    a_CacScale = m_SaveScale.x;

                if (m_EffAddTime < 0.14f) {

                    this.transform.localScale = new Vector3(a_CacScale, a_CacScale, 1.0f);
                }
                else if (0.19f <= m_EffAddTime) {

                    m_EffAddTime = 0.0f;
                    this.transform.localScale = new Vector3(m_SaveScale.x, m_SaveScale.y, 1.0f);
                    m_PunchState = false;   //펀칭 연출 종류
                    m_MoveState = true;     //이동 연출 시작
                }
            } //if(0.033f < m_EffAddTime)
        } //if(m_PunchState == true)


        if (m_MoveState == true) {    //이동연출
            //m_EffAddTime += Time.deltaTime;

            if (a_CacYPos < m_MaxHeight) //위로 올라가는 이동 처리
            {
                if (m_MaxHeight - 0.2f < a_CacYPos) //특정 구간에서 감속
                    m_MvSpeed = 1.0f;
                else
                    m_MvSpeed = 7.7f;

                a_OldYPos = a_CacYPos;
                a_CacYPos += Time.deltaTime * m_MvSpeed;
                if (m_MaxHeight < a_CacYPos)
                    a_CacYPos = m_MaxHeight;

                m_CurCacPos = m_SavePos;    //m_SavePos <-- 스폰 시작시 초기화 셋팅됨
                m_CurCacPos.z = m_SavePos.z + a_CacYPos;
                this.transform.position = m_CurCacPos;

                if (a_OldYPos <= m_MaxHeight - 0.1f && m_MaxHeight - 0.1f < a_CacYPos) {
                    m_AlphaState = true;
                    m_EffAddTime = 0.0f;    //변수 재활용 초기화
                }
            }
            else //도착
            {
                m_MoveState = false;
            }

            if (m_MaxHeight + 0.2f < a_CacYPos)  //예외처리
            {
                Destroy(this.gameObject);   //자폭
            }
        }// if(m_MoveState == true)


        if (m_AlphaState == true)//투명도 연출구간
        {
            m_EffAddTime += Time.deltaTime * 1.7f;
            if (0.8f < m_EffAddTime)
                m_EffAddTime = 0.8f;
            alpha = 1.0f * (m_EffAddTime / 0.8f);
            _color = m_RefText.color;
            _color.a = 1.0f - alpha;
            m_RefText.color = _color;

            if (0.8f <= m_EffAddTime)   //삭제
                Destroy(this.gameObject);
        }//if(m_AlphaState == true) //투명도 연출구간
    }

}
