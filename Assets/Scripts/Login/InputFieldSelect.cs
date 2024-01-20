using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InputFieldSelect : MonoBehaviour
{
    [SerializeField]
    Selectable[] selectables;



    int selectedIdx = 0;
    Coroutine focusupdate = null;
    private void Awake () {
        selectables = GetComponentsInChildren<Selectable>(true);
    }


    private void OnEnable () {
        //for(int i =0; i < selectables.Length; i++)
        //{
        //    if(selectables[i].gameObject.activeSelf == true && selectables[i].IsInteractable() == true)
        //    {
        //        selectables[i
        //        break;
        //    }
        //}
        StartCoroutine(SetDefFocus());
        focusupdate = StartCoroutine(FocusUpdate());
    }
    IEnumerator SetDefFocus () {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < selectables.Length; i++) {
            if (selectables[i].gameObject.activeSelf == true && selectables[i].IsInteractable() == true) {
                selectables[i].Select();

                selectedIdx = i;
                break;
            }
        }
    }
    private void OnDisable () {
        StopCoroutine(focusupdate);
    }

    IEnumerator FocusUpdate () {
        while (true) {
            KeyDownBind();
            yield return new WaitForSeconds(0);
        }
    }

    private void KeyDownBind () {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab)) {
            int idx = 0;
            if (selectedIdx <= 0) {
                idx = selectables.Length - 1;
            }
            else {
                idx = selectedIdx - 1;
            }
            for (int i = idx; i < selectables.Length; i--) {
                if (selectables[i].gameObject.activeSelf == true && selectables[i].IsInteractable() == true) {
                    selectables[i].Select();

                    selectedIdx = i;
                    break;
                }
                if (i == 0)
                    i = selectables.Length - 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab)) {
            int idx = 0;
            if (selectedIdx >= selectables.Length - 1) {
                idx = 0;
            }
            else {
                idx = selectedIdx + 1;
            }
            for (int i = idx; i < selectables.Length; i++) {
                if (selectables[i].gameObject.activeSelf == true && selectables[i].IsInteractable() == true) {
                    selectables[i].Select();

                    selectedIdx = i;
                    break;
                }
            }

        }
        else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
            Button button = GetComponentInChildren<Button>(true);
            if (button != null)
                button.onClick.Invoke();
        }
    }
}
