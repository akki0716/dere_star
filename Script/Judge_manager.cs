using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


    /// <summary>
    /// デバッグ表示用テキスト
    /// </summary>
    [SerializeField]
    GameObject text;

    /// <summary>
    /// デバッグ表示用テキスト
    /// </summary>
    [SerializeField]
    GameObject text2;

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
        /*
        if (Input.touchCount > 0)
        {
            Debug.Log("タッチ");
        }
        */
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
            if (judge_note_lane1.type == type && data_warehouse.lane1_notes[lane1_judge_index].alive ==true)//タッチかフリック
            {
                bool _isJudgeRange;//判定範囲内か
                Time_lag = Mathf.Abs(judge_note_lane1.timing - time);
                _isJudgeRange = Time_Judge(Time_lag, judge_note_lane1.type);
                if (_isJudgeRange == true)//判定が起こったら
                {
                    text.GetComponent<Text>().text = "Judge";
                    Note_ObjectPool.releaseNote(data_warehouse.lane1_Makes[lane1_judge_index], 1);
                    data_warehouse.lane1_notes[lane1_judge_index].alive = false;
                    if (lane1_judge_index < (data_warehouse.lane1_notes.Length - 1))
                    {
                        lane1_judge_index++;
                    }
                }
                
            }
            else if(type == 1 && judge_note_lane1.type == 3 && data_warehouse.lane1_notes[lane1_judge_index].alive == true)//ホールド開始
            {
                bool _isJudgeRange;//判定範囲内か
                Time_lag = Mathf.Abs(judge_note_lane1.timing - time);
                Hold_start_time[lane] = time;
                _isBAD = false;//これで一度ホールドをbadにしても後のホールドが正しく判定できる
                _isJudgeRange = Time_Judge(Time_lag, judge_note_lane1.type);
                if (_isJudgeRange == true && _isBAD == false)//判定が起こったら
                {
                    Hold_Note_player = data_warehouse.lane1_Makes[lane1_judge_index].GetComponent<Hold_Note_player>();
                    Hold_Note_player.Moven.Kill();//止める
                    Touch_manager.lane_hold_allow[lane] = true;//ホールド判定を許可
                    shortn_time = data_warehouse.lane1_Makes[lane1_judge_index].transform.localScale.y / (hold_base_scale * (3 / Note_manager.float_steam_time));
                    Hold_Note_player.Shorten(shortn_time);
                    /*ホールドの開始時にインデックスを動かすとホールドの終了時にもインデックスが動いておかしくなる(ホールドでインデックスを動かすのは終了時のみ
                    if (lane1_judge_idex < (data_wareouse.lane1_otes.Length - 1))
                    {
                        lane1_jude_index++;
                    }
                    */
                }
                else if(_isJudgeRange == true && _isBAD == true )//BADだったら
                {
                    Hold_Note_player = data_warehouse.lane1_Makes[lane1_judge_index].GetComponent<Hold_Note_player>();
                    Hold_Note_player.Moven.Kill();//止める
                    Note_ObjectPool.releaseNote(data_warehouse.lane1_Makes[lane1_judge_index], 3);
                    data_warehouse.lane1_notes[lane1_judge_index].alive = false;
                   //badだとインデックスを動かさないといけない
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
    public void Holdbreak(int lane, int time)
    {

        int lag;
        bool alive;//ホールドが1度判定されていないか

        //Debug.Log("");

        switch (lane)//ホールドが1度判定されていればavlieがfalseになる
        {
            case 1:
                //alive = data_warehouse.lane1_notes[lane1_judge_index].alive;
                if (data_warehouse.lane1_notes[lane1_judge_index].timing - BAD >= Time_manager.count_time || data_warehouse.lane1_notes[lane1_judge_index].alive == false
                    || data_warehouse.lane1_notes[lane1_judge_index].type !=3)//今見ているノートが先の時間もしくはホールドでない
                {
                    alive = false;
                }
                else
                {
                    alive = true;
                }
                break;
            default:
                alive = true; //これになってはいけない
                break;
        }

        //ホールドを一度badになった後、もう一度そのレーンでホールドして離すとPERFECTになってしまう




        if (alive == true)
        {


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
                Debug.Log("holdbreak PERFECT!!!");
                atomSource_se.Play("clap");
            }
            else if (lag <= GREAT)
            {
                Debug.Log("holdbreak GREAT!!");
                atomSource_se.Play("clap");
            }
            else if (lag <= GOOD)
            {
                Debug.Log("holdbreak GOOD!");
                atomSource_se.Play("clap");
            }
            else
            {
                Debug.Log("holdbreak BAD…");
                atomSource_se.Play("miss");
            }

            switch (lane)
            {
                case 1:
                    Hold_Note_player = data_warehouse.lane1_Makes[lane1_judge_index].GetComponent<Hold_Note_player>();
                    Hold_Note_player.Moven.Kill();//止める
                    text.GetComponent<Text>().text = "Holdbreak";
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
                if (judge_note_lane1.hold_time <= (Time_manager.count_time - Hold_start_time[lane]) &&
                    judge_note_lane1.hold_time !=0)//ホールド時間が0でない(=タップとかホールドのままタッチし続けていない)
                    //judge_note_lane1.timing+ judge_note_lane1.hold_time <= Time_manager.count_time)//ホールド時間を超えているかつ(ホールドの開始時間+ホールド時間＝ホールド終了時刻)よりも今の時間が後ろである
                {
                    Debug.Log("Holding PERFECT!!!");
                    text.GetComponent<Text>().text = "Holding PERFECT";
                    text2.GetComponent<Text>().text = (Time_manager.count_time - Hold_start_time[lane]).ToString();
                    atomSource_se.Play("clap");
                    Hold_start_time[lane] = Time_manager.count_time;
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
            Touch_manager.lane_hold_allow[lane] = false;//ホールド判定状態を終わる
            Touch_manager.hold_unlock(lane);//ホールド
            Hold_Note_player = data_warehouse.lane1_Makes[lane1_judge_index].GetComponent<Hold_Note_player>();
            Hold_Note_player.Moven.Kill();//止める
            Debug.Log(data_warehouse.lane1_Makes[lane1_judge_index].name);
            text.GetComponent<Text>().text = "Holdkill";
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
            Debug.Log("Time_Judge PERFECT!!!");
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
            Debug.Log("Time_Judge GREAT!!");
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
            Debug.Log("Time_Judge GREAT!!");
        }
        else if (lag <= BAD)
        {
            atomSource_se.Play("miss");
            JudgeRange = true;
            _isBAD = true;
            Debug.Log("Time_Judge BAD…");
            text.GetComponent<Text>().text = "Time_Judge BAD…";
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
        //タッチ、フリックの見逃しsearch
        if (Note_manager._isSearch_lane1== true && Time_manager.count_time >= (data_warehouse.lane1_notes[lane1_judge_index].timing + 50)
            && data_warehouse.lane1_notes[lane1_judge_index].type != 3
            && data_warehouse.lane1_notes[lane1_judge_index].alive == true)
        {
            Debug.Log("見逃し " + data_warehouse.lane1_notes[lane1_judge_index].timing);
            text.GetComponent<Text>().text = "Through_Notes";
            Note_ObjectPool.releaseNote(data_warehouse.lane1_Makes[lane1_judge_index], data_warehouse.lane1_notes[lane1_judge_index].type);
            //「タッチで判定されていたらやらない」
            data_warehouse.lane1_notes[lane1_judge_index].alive = false;

            if (lane1_judge_index < data_warehouse.lane1_notes.Length - 1)
            {
                lane1_judge_index++;
            }
        }
        if (Note_manager._isSearch_lane1 == true && Time_manager.count_time >= (data_warehouse.lane1_notes[lane1_judge_index].timing + 50 )
            && data_warehouse.lane1_notes[lane1_judge_index].type == 3
            && Touch_manager.lane_hold_allow[1] == false
            && data_warehouse.lane1_notes[lane1_judge_index].alive == true)
        {
            text.GetComponent<Text>().text = "見逃しホールド";
            Debug.Log("見逃しホールド " + data_warehouse.lane1_notes[lane1_judge_index].timing);
            ///Note_ObjectPool.releaseNote(data_warehouse.lane1_Makes[lane1_judge_index], 3);
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
    

   /* 
    void judge_note_search()
    {
        if (Time_manager.count_time >= data_warehouse.lane1_notes[lane1_index].timing)
        {

        }
    }
    */

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



    /// <summary>
    /// 今判定しているノートがホールドか否か返す
    /// </summary>
    /// <param name="lane"></param>
    /// <returns></returns>
    public bool isHoldNote(int lane)
    {
        if (lane == 1)
        {
            return judge_note_lane1.type == 3 ? true : false;//三項演算子。typeが3ならtrueが返る。
        }
        /*
        if (lane == 2)
        {
            return judge_note_lane2.type == 3 ? true : false;
        }
        if (lane == 3)
        {
            return judge_note_lane3.type == 3 ? true : false;
        }
        if (lane == 4)
        {
            return judge_note_lane4.type == 3 ? true : false;
        }
        if (lane == 5)
        {
            return judge_note_lane5.type == 3 ? true : false;
        }
        */
        else
        {
            return false;
        }
    }
}
