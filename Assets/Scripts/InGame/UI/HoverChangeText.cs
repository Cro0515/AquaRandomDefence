using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverChangeText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {


    Text[] textArray = new Text[4];


    public string[] curValue = new string[4];
    public string[] nextValue = new string[4];
    Color nextColor_Red = new Color32(255,80,80,255);
    Color nextColor_Blue = new Color32(0,200,255,255);



    public void OnPointerEnter (PointerEventData eventData) {
        
        for (int i = 0; i < textArray.Length; i++) {

            if (nextValue[i] != null && textArray[i].text != "-"){
                
                textArray[i].text = nextValue[i];

                float nextVal = float.TryParse(nextValue[i], out nextVal) ? nextVal : float.Parse(nextValue[i].Replace("마리", ""));
                float curtVal = float.TryParse(curValue[i], out curtVal) ? curtVal : float.Parse(curValue[i].Replace("마리", ""));

                if (i != 1){
                    if (nextVal > curtVal)
                        textArray[i].color = nextColor_Blue;
                    else if (nextVal < curtVal)
                        textArray[i].color = nextColor_Red;
                }
                else{
                    if (nextVal > curtVal)
                        textArray[i].color = nextColor_Red;
                    else if (nextVal < curtVal)
                        textArray[i].color = nextColor_Blue;
                }
            }
        }
    }

    public void OnPointerExit (PointerEventData eventData) {
        
        for (int i = 0; i < textArray.Length; i++) {
            if (curValue[i] != null && textArray[i].text != "-") {
                textArray[i].text = curValue[i];
                textArray[i].color = Color.white;
            }
        }
        
    }




    // Start is called before the first frame update
    void Start () {

        for (int i = 0; i < textArray.Length; i++) {
            textArray[i] = this.transform.parent.Find("InfoCell_" + (i+2)).Find("CellContents_Text").GetComponent<Text>();
        }

    }

    // Update is called once per frame
    void Update () {

    }
}
