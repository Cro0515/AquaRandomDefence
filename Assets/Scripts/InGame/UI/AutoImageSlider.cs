using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*

    1.이미지를 표기하고자 하는 위치에 빈 게임오브젝트 생성후 해당 오브젝트에 스크립트와 스프라이트렌더 컴포넌트를 추가한다.
    2.CreateSlider()함수를 통해 초기 생성을 해주어야 하며, 매개변수는 슬라이드를 하고자하는 [이미지가 들어있는 폴더의 경로] 와 FrameRate(float)이다.
    3.StartSlide() 함수를 통해 슬라이드를 시작 할 수 있으며, StopSlide()를 통해 멈출 수 있다.
 
*/


public class AutoImageSlider : MonoBehaviour
{
    [HideInInspector] public string path;
    [HideInInspector] public Sprite[] ImageArray;
    SpriteRenderer SR;
    float FrameRate;

    int i = 0;


    // Start is called before the first frame update
    void Start()
    {

        SR = this.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CreateSlider (string _path, float _framerate) {
        path = _path;
        FrameRate = _framerate;

        ImageArray = Resources.LoadAll<Sprite>(path);

    }


    public void NextImage () {

        SR.sprite = ImageArray[i];
        i++;

        if (i > ImageArray.Length - 1)
            i = 0;
    }

    public void StartSlide () {

        SR.enabled = true;

        InvokeRepeating("NextImage", 0, FrameRate);

    }

    public void StopSlide () {
        CancelInvoke("NextImage");

        if(SR.sprite != null)
            SR.sprite = null;
        i = 0;

        SR.enabled = false;
    }
   
  
}
