using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListTest : MonoBehaviour {



    //public List<  List<int>    > = new List< List<int> >();//作ったノートを保持する配列
    List<List<int>> IntList = new List<List<int>>();

    // Use this for initialization
    void Start ()
    {

        IntList.Add(new List<int>() { 1, 3, 5, 7, 9 });//1行目(横)
        IntList.Add(new List<int>() { 2, 4, 6 });//2行目(横)
        IntList.Add(new List<int>() { 3, 6, 9, 12, 15, 18 });

        //List.Add(IntList.Add(8));


        //以下は内容表示の文
        for (int i = 0; i < IntList.Count; i++)
        {
            // 反復子（表示状の行番号）を表示
            Debug.Log("<" + i.ToString() + "> "); 

            // 各要素の値を表示するループ
            // ２次元目のループ
            for (int j = 0; j < IntList[i].Count; j++)
            {
                // 要素の値を表示
                Debug.Log(IntList[i][j].ToString()) ;

                // 末尾の要素でない場合
                if (j != (IntList[i].Count - 1))
                {
                    // カンマで区切る
                    //Debug.Log(textBox1.Text += ", "); ;
                }
                // 末尾要素の場合
                else
                {
                    // 改行
                    Debug.Log("");
                }
            }
        }

        

    }


	
}

