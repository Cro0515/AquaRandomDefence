using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public class ConvertArrayString : MonoBehaviour
{

    public static string ArrayToString (int[,] _arr) {
        string arrayString = "";


        for (int i = 0; i < _arr.GetLength(0); i++) {
            arrayString += "{";
            for (int j = 0; j < _arr.GetLength(1); j++) {
                arrayString += _arr[i, j].ToString();

                if (j < _arr.GetLength(1) - 1)
                    arrayString += ",";
            }
            arrayString += "}";

            if (i < _arr.GetLength(0) - 1)
                arrayString += "\n";
        }


        return arrayString;
    }


    public static int[,] StringToArray (string _str) {
        // { 개수 = i (21)
        // , 개수 나누기 i +1  = j
        MatchCollection matches = Regex.Matches(_str, "{");
        int i_cnt = matches.Count;
        Debug.Log(i_cnt);
        matches = Regex.Matches(_str, ",");
        int j_cnt = matches.Count;
        j_cnt = (j_cnt / i_cnt) + 1;
        Debug.Log(j_cnt);

        int[,] arr = new int[i_cnt, j_cnt];


        string[] row_splitStr = { "{", "}", "\n" };
        string[] row = _str.Split(row_splitStr, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < i_cnt; i++) {
            string[] value = row[i].Split(',');
            for (int j = 0; j < j_cnt; j++) {
                arr[i, j] = int.Parse(value[j]);
            }
        }

        return arr;
    }


    public static void EqualCompare (int[,] _arr1, int[,] _arr2) {
        int cnt = 0;


        for (int i = 0; i < _arr1.GetLength(0); i++) {
            for (int j = 0; j < _arr1.GetLength(1); j++) {

                if (_arr1[i, j] == _arr2[i, j]) {
                    cnt++;
                }

            }
        }

        Debug.Log("원본배열의 모든 요소 개수" + _arr1.GetLength(0) * _arr1.GetLength(1));
        Debug.Log("원본배열과 동일한 요소 개수 : " + cnt);

    }
}
