using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Time_manager : MonoBehaviour {




	/// <summary>
	/// 現在の時間
	/// </summary>
	public int count_time;

	/// <summary>
	/// 時間計測をするかのフラグ
	/// </summary>
	public bool _is_timecount = false;


	/// <summary>
	/// ノートの時間精度。 
	/// 1000だと1/1000秒＝1ミリ秒、100だと1/100秒＝10ミリ秒。
	/// Score_load等と同様のもの
	/// </summary>
	int note_accuracy = 100;


    public GameObject time_count_txt;//少数時間を表示する文字

    /*
	// Use this for initialization
	void Start () {
		
	}
	*/
    // Update is called once per frame
    void FixedUpdate () {
		if (_is_timecount == true)
		{
            //count_time += (int)(Time.deltaTime * note_accuracy);//フレーム制のとき
            count_time++;
            //time_count_txt.GetComponent<UnityEngine.UI.Text>().text = "time_count " + count_time.ToString();
        }

		//Debug.Log(Time.deltaTime);

	}




    
}
