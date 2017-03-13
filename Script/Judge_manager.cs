using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge_manager : MonoBehaviour {

    [SerializeField]
    Touch_manager Touch_manager;

    [SerializeField]
    data_warehouse data_warehouse;

    [SerializeField]
    Time_manager Time_manager;

    [SerializeField]
    Note_manager Note_manager;

    [SerializeField]
    Note_ObjectPool Note_ObjectPool;





    //-------------判定関係---------------------
    /// <summary>
    /// 判定するノート情報の構造体
    /// </summary>
    public Judge_note judge_note;



    /// <summary>
    /// レーン1の今判定するノーツのインデックス
    /// </summary>
    int lane1_judge_index = 0;






    //-------------見逃し関係---------------------
    /// <summary>
    /// レーン1の見逃しmissを探索するノートのインデックス
    /// </summary>
    int lane1_Cheak;

    /// <summary>
    /// レーン2の見逃しmissを探索するノートのインデックス
    /// </summary>
    int lane2_Cheak;


    /*
    /// <summary>
    /// ノーツ音コンポーネント
    /// </summary>
    public CriAtomSource atomSource_se;
    */


    int PERFECT = 3;
    int GREAT = 7;
    int GOOD = 10 ;
    int BAD = 20 ;

    /// <summary>
    /// タッチした時間とノートの時間の差
    /// </summary>
    int Time_lag;

    /*
    // Use this for initialization
    void Start () {
		
	}
	
    */
	
	void Update () {
		Through_Notes();
	}

    /// <summary>
    /// 判定を行う
    /// </summary>
    /// <param name="lane"></param>
    /// <param name="type"></param>
    /// <param name="time"></param>
    public void Judge(int lane, int type, int time)
    {
        if (lane == 1)
        {
            Judge_note_pull(1);
            if (judge_note.type == type)
            {
                Time_lag = Mathf.Abs(judge_note.timing - time);
                Time_Judge(Time_lag);
                Note_ObjectPool.releaseNote(data_warehouse.lane1_Makes[lane1_judge_index], 1);
                
            }

        }
    }



    void Judge_note_pull(int lane)
    {
        if (lane == 1)
        {
            judge_note = new Judge_note(data_warehouse.lane1_notes[lane1_judge_index].timing,
                data_warehouse.lane1_notes[lane1_judge_index].type, data_warehouse.lane1_notes[lane1_judge_index].hold_time,
                true);
        }
    }




    void Time_Judge(int lag)
    {
        if (lag <=PERFECT)
        {
            Debug.Log("PERFECT!!!");
        }
        else if(lag <= GREAT)
        {
            Debug.Log("GREAT!!");
        }
        else if (lag <= GOOD)
        {
            Debug.Log("GREAT!!");
        }

    }





    /// <summary>
    /// 見逃しmissの探索と破壊
    /// </summary>
    /// 
    void Through_Notes()
    {//見逃しmissでノーツを破壊すると下端まで行っていないのにノーツが消えてしまう
        if (Note_manager._isSearch_lane1== true && Time_manager.count_time >= (data_warehouse.lane1_notes[lane1_Cheak].timing + 50))
        //&& data_warehouse.lane1_notes[lane1_Cheak].alive == true)
        {
            //後で「タッチで判定されていたらやらない」ようにする
            if (lane1_Cheak < data_warehouse.lane1_notes.Length - 1)
            {
                lane1_Cheak++;
            }
        }

        if (Note_manager._isSearch_lane2 == true &&  Time_manager.count_time >= (data_warehouse.lane2_notes[lane2_Cheak].timing + 50))
        //&& data_warehouse.lane2_notes[lane1_Cheak].alive == true)
        {
            data_warehouse.lane2_Makes[lane2_Cheak].SetActive(false);
            //Debug.Log(lane2_Cheak);
            if (lane2_Cheak < data_warehouse.lane1_notes.Length -1 )
            {
                lane2_Cheak++;
            }
        }


    }
    


    /// <summary>
    /// 判定するノートの情報
    /// </summary>
    public struct Judge_note
    {
        public int timing, type, hold_time;
        public bool alive;

        public Judge_note(int ti, int ty, int h_t, bool ali)
        {
            timing = ti;
            type = ty;
            hold_time = h_t;
            alive = true;
        }




    }
}
