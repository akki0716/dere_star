using DG.Tweening;
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


    Hold_Note_player Hold_Note_player;

    [SerializeField]
    public CriAtomSource atomSource_se;



    //-------------判定関係---------------------
    /// <summary>
    /// 判定するノート情報の構造体
    /// </summary>
    public Judge_note judge_note_lane1;



    /// <summary>
    /// レーン1の今判定するノーツのインデックス
    /// </summary>
    int lane1_judge_index = 0;



    /// <summary>
    /// ホールドの基準となる長さ(スケール)
    /// </summary>
    float hold_base_scale = 17f;

    /// <summary>
    /// ホールドを短くする時間
    /// </summary>
    float shortn_time;


    bool _isBAD = false;

    /// <summary>
    /// Holdを初めた時間
    /// </summary>
    int[] Hold_start_time = new int[6];

    //-------------見逃し関係---------------------
    //いらなそう
    /// <summary>
    /// レーン1の見逃しmissを探索するノートのインデックス
    /// </summary>
    //int lane1_Cheak;

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
            if (judge_note_lane1.type == type)//タッチかフリック
            {
                bool _isJudgeRange;//判定範囲内か
                Time_lag = Mathf.Abs(judge_note_lane1.timing - time);
                _isJudgeRange = Time_Judge(Time_lag, judge_note_lane1.type);
                if (_isJudgeRange == true)//判定が起こったら
                {
                    Note_ObjectPool.releaseNote(data_warehouse.lane1_Makes[lane1_judge_index], 1);
                    data_warehouse.lane1_notes[lane1_judge_index].alive = false;
                    if (lane1_judge_index < (data_warehouse.lane1_notes.Length - 1))
                    {
                        lane1_judge_index++;
                    }
                }
                
            }
            else if(type == 1 && judge_note_lane1.type == 3)//ホールド開始
            {
                bool _isJudgeRange;//判定範囲内か
                Time_lag = Mathf.Abs(judge_note_lane1.timing - time);
                Hold_start_time[lane] = time;
                _isJudgeRange = Time_Judge(Time_lag, judge_note_lane1.type);
                if (_isJudgeRange == true && _isBAD == false)//判定が起こったら
                {
                    Hold_Note_player = data_warehouse.lane1_Makes[lane1_judge_index].GetComponent<Hold_Note_player>();
                    Hold_Note_player.Moven.Kill();//止める
                    Touch_manager.lane_holding[lane] = true;//ホールド判定状態へ
                    shortn_time = data_warehouse.lane1_Makes[lane1_judge_index].transform.localScale.y / (hold_base_scale * (3 / Note_manager.float_steam_time));
                    Hold_Note_player.Shorten(shortn_time);
                    if (lane1_judge_index < (data_warehouse.lane1_notes.Length - 1))
                    {
                        lane1_judge_index++;
                    }
                }
                else if(_isJudgeRange == true && _isBAD == true )//BADだったら
                {
                    Hold_Note_player = data_warehouse.lane1_Makes[lane1_judge_index].GetComponent<Hold_Note_player>();
                    Hold_Note_player.Moven.Kill();//止める
                    Note_ObjectPool.releaseNote(data_warehouse.lane1_Makes[lane1_judge_index], 3);
                    data_warehouse.lane1_notes[lane1_judge_index].alive = false;
                    if (lane1_judge_index < (data_warehouse.lane1_notes.Length - 1))
                    {
                        lane1_judge_index++;
                    }
                }

               
            }

        }
    }



    /// <summary>
    /// ホールド中に指を離した
    /// </summary>
    /// <param name="lane"></param>
    public void Holdbreak(int lane,int time)
    {
        int lag;
        switch (lane)
        {

            case 1:
                lag = judge_note_lane1.hold_time - (time - Hold_start_time[lane]);
                break;
            default:
                lag = 0;
                break;
        }


         
        if (lag <= PERFECT)
        {
            Debug.Log("PERFECT!!!");
        }
        else if (lag <= GREAT)
        {
            Debug.Log("GREAT!!");
        }
        else if (lag <= GOOD)
        {
            Debug.Log("GREAT!!");
        }
        else
        {
            Debug.Log("BAD…");
        }

        switch (lane)
        {
            case 1:
                Hold_Note_player = data_warehouse.lane1_Makes[lane1_judge_index].GetComponent<Hold_Note_player>();
                Hold_Note_player.Moven.Kill();//止める
                Note_ObjectPool.releaseNote(data_warehouse.lane1_Makes[lane1_judge_index], 3);
                data_warehouse.lane1_notes[lane1_judge_index].alive = false;
                if (lane1_judge_index < (data_warehouse.lane1_notes.Length - 1))
                {
                    lane1_judge_index++;
                }
                break;
            default:
                break;
        }
    }





    /// <summary>
    /// ホールド中のホールド完了を判断する
    /// </summary>
    /// <param name="lane"></param>
    public void Holding(int lane)
    {
        switch (lane)
        {
            case 1:
                if (judge_note_lane1.hold_time <= (Time_manager.count_time - Hold_start_time[lane]))//ホールド時間を超えたら
                {
                    Debug.Log("PERFECT!!!");

                    Holdkill(lane);
                }
                break;
            default:
                break;
        }
        

    }






    /// <summary>
    /// ホールドを破壊する
    /// </summary>
    /// <param name="lane"></param>
    public void Holdkill(int lane)
    {
        switch (lane)
        {
            case 1:
            Touch_manager.lane_holding[lane] = false;//ホールド判定状態へ
            Hold_Note_player = data_warehouse.lane1_Makes[lane1_judge_index].GetComponent<Hold_Note_player>();
            Hold_Note_player.Moven.Kill();//止める
            Note_ObjectPool.releaseNote(data_warehouse.lane1_Makes[lane1_judge_index], 3);
            data_warehouse.lane1_notes[lane1_judge_index].alive = false;
            if (lane1_judge_index < (data_warehouse.lane1_notes.Length - 1))
            {
                lane1_judge_index++;
            }
                break;
            default:
                break;
        }
    }






    /// <summary>
    /// 判定するノートの情報を引っ張ってくる
    /// </summary>
    /// <param name="lane"></param>
    void Judge_note_pull(int lane)
    {
        if (lane == 1)
        {
            judge_note_lane1 = new Judge_note(data_warehouse.lane1_notes[lane1_judge_index].timing,
                data_warehouse.lane1_notes[lane1_judge_index].type, data_warehouse.lane1_notes[lane1_judge_index].hold_time,
                data_warehouse.lane1_notes[lane1_judge_index].alive);
        }
    }







    /// <summary>
    /// 判定ランクの判断
    /// </summary>
    /// <param name="lag"></param>
    /// <returns></returns>
    bool Time_Judge(int lag,int type)
    {
        bool JudgeRange;
        if (lag <=PERFECT)
        {
            if (type == 1 || type == 3)
            {
                atomSource_se.Play("clap");
            }
            else if(type == 2)
            {
                atomSource_se.Play("slash");
            }
            JudgeRange = true;
            Debug.Log("PERFECT!!!");
        }
        else if(lag <= GREAT)
        {
            if (type == 1 || type == 3)
            {
                atomSource_se.Play("clap");
            }
            else if (type == 2)
            {
                atomSource_se.Play("slash");
            }
            JudgeRange = true;
            Debug.Log("GREAT!!");
        }
        else if (lag <= GOOD)
        {
            if (type == 1 || type == 3)
            {
                atomSource_se.Play("clap");
            }
            else if (type == 2)
            {
                atomSource_se.Play("slash");
            }
            JudgeRange = true;
            Debug.Log("GREAT!!");
        }
        else if (lag <= BAD)
        {
            atomSource_se.Play("miss");
            JudgeRange = true;
            _isBAD = true;
            Debug.Log("BAD…");
        }
        else
        {
            //Debug.Log("BAD…");
            JudgeRange = false;
        }
        return JudgeRange;
    }





    /// <summary>
    /// 見逃しmissの探索
    /// </summary>
    /// 
    void Through_Notes()
    {
        if (Note_manager._isSearch_lane1== true && Time_manager.count_time >= (data_warehouse.lane1_notes[lane1_judge_index].timing + 50)
            && data_warehouse.lane1_notes[lane1_judge_index].type !=3
            && data_warehouse.lane1_notes[lane1_judge_index].alive == true)
        {
            Debug.Log("見逃し " + data_warehouse.lane1_notes[lane1_judge_index].timing);
            Note_ObjectPool.releaseNote(data_warehouse.lane1_Makes[lane1_judge_index], data_warehouse.lane1_notes[lane1_judge_index].type);
            //「タッチで判定されていたらやらない」
            data_warehouse.lane1_notes[lane1_judge_index].alive = false;
            if (lane1_judge_index < data_warehouse.lane1_notes.Length - 1)
            {
                lane1_judge_index++;
            }
        }

        if (Note_manager._isSearch_lane2 == true &&  Time_manager.count_time >= (data_warehouse.lane2_notes[lane2_Cheak].timing + 50))
        //&& data_warehouse.lane2_notes[lane1_Cheak].alive == true)
        {
            //Note_ObjectPool.releaseNote(data_warehouse.lane2_Makes[lane2_judge_index], data_warehouse.lane2_notes[lane2_judge_index].type);
            data_warehouse.lane2_Makes[lane2_Cheak].SetActive(false);
            //Debug.Log(lane2_Cheak);
            if (lane2_Cheak < data_warehouse.lane1_notes.Length -1 )
            {
                lane2_Cheak++;
            }
        }


    }
    


    /// <summary>
    /// 判定するノートの情報の構造体
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
