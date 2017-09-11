using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タッチ入力の処理とその結果のノーツ判定までを行う(判定した後の処理は別)
/// </summary>
public class Touch_manager  : MonoBehaviour {

    [SerializeField]
    Time_manager Time_manager;

    [SerializeField]
    data_warehouse data_warehouse;

    [SerializeField]
    Judge_manager Judge_manager;


    /// <summary>
    /// タッチ状態を入れておく構造体
    /// </summary>
    Touch touch ;

    public Touch_rect [] Touches = new Touch_rect[3];//タッチは最大3点


    /// <summary>
    /// 最初のタッチポジション
    /// </summary>
    Vector3 StartPos;

    /// <summary>
    /// タッチしたレーン。1番最初にタッチした位置が基準
    /// </summary>
    int lane;

    /// <summary>
    ///各指のタッチ時間を格納する配列。対応はfingerIDに準ずる(はず) 
    /// </summary>
    int [] TouchTime  = new int[6];
    //Touchesに吸収統合予定

    /// <summary>
    /// どのレーンがホールドを受け付ける状態になっているか
    /// </summary>
    public bool[] lane_hold_allow = new bool [6];

    

    /// <summary>
    /// どのぐらいの指の動きをスワイプとするか
    /// </summary>
    [SerializeField]
     float SwipeSensibility;

    /// <summary>
    /// ホールドと認識される時間
    /// </summary>
    public float hold_judge_Time;


    /// <summary>
    /// ホールド(になるタップ)を初めた時間
    /// </summary>
    float hold_start_time;

    /// <summary>
    /// デバッグ表示用テキスト
    /// </summary>
    [SerializeField]
    GameObject text;


    /*
    // Use this for initialization
    void Start () {
        Debug.Log("touch_manager");
        //Input.simulateMouseWithTouches = true;//マウスでタッチシミュレート
    }
	*/

    // Update is called once per frame
    void Update ()
    {
        if (Input.touchCount > 0)
        {
            //Debug.Log("タッチ");
            for (int i = 0; i < Input.touchCount; i++)//とりあえずタッチした時間を取っておく
            {
                touch = Input.GetTouch(i);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        GetTouchTime(touch.fingerId);
                        break;
                    default:
                        break;
                }
            }
            for (int i = 0; i < Input.touchCount; i++)//タッチ処理の振り分け
            {
                touch = Input.GetTouch(i);
                
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        TouchBegan(touch, touch.fingerId);
                        break;
                    case TouchPhase.Moved:
                        TouchMoved(touch, touch.fingerId);
                        break;
                    case TouchPhase.Stationary:
                        //Debug.Log("Stationary");
                        TouchHolding(touch, i);
                        break;
                    case TouchPhase.Ended:
                        TouchEnd(touch, i);
                        break;
                    default:
                        break;
                }

            }




        }

    }


    //TouchBegan(touch);//ワールド座標を取得



    /// <summary>
    /// この処理を行った時点の時間を入れる
    /// </summary>
    void GetTouchTime(int fingerID)
    {
        //TouchTime[fingerID] = Time_manager.count_time;
        Touches[fingerID].TouchTime = Time_manager.count_time;
    }




    /// <summary>
    /// タッチ開始時点の処理
    /// </summary>
    void TouchBegan(Touch touch,int fingerID)
    {
        text.GetComponent<Text>().text = "タッチした";
        Touches[fingerID].hold_start_time = Time.realtimeSinceStartup;
        //Debug.Log("touch.position " + touch.position);
        Touches[fingerID].Startpos = Camera.main.ScreenToWorldPoint(touch.position);
        //StartPos = Camera.main.ScreenToWorldPoint(touch.position);
        //Debug.Log("Pos " + StartPos);
        Touches[fingerID].lane = JudgeLane(Touches[fingerID].Startpos);//レーンを判断
        Touches[fingerID].hold_locked = false;
        Judge_manager.Judge(Touches[fingerID].lane, 1, Touches[fingerID].TouchTime);

    }



    /// <summary>
    /// タッチした指が動いた
    /// </summary>
    /// <param name="touch"></param>
    /// <param name="fingerID"></param>
    void TouchMoved(Touch touch, int fingerID)
    {
        //Debug.Log("moved");
        Vector3 NowPos = Camera.main.ScreenToWorldPoint(touch.position);
        if (Mathf.Abs(StartPos.x - NowPos.x) >= SwipeSensibility && Touches[fingerID].lane_holding == false && Touches[fingerID].hold_locked == false)//スワイプ
        {
            //text.GetComponent<Text>().text = "xスワイプ";
            GetTouchTime(fingerID);
            Debug.Log("swipe");
            StartPos.x = NowPos.x;
            StartPos.y = NowPos.y;
            Judge_manager.Judge(Touches[fingerID].lane, 2, Touches[fingerID].TouchTime);
        }
        else if (Mathf.Abs(StartPos.y - NowPos.y) >= SwipeSensibility && Touches[fingerID].lane_holding == false && Touches[fingerID].hold_locked == false)//スワイプ
        {
            //text.GetComponent<Text>().text = "yスワイプ";
            GetTouchTime(fingerID);
            Debug.Log("swipe");
            StartPos.x = NowPos.x;
            StartPos.y = NowPos.y;
            Judge_manager.Judge(Touches[fingerID].lane, 2, Touches[fingerID].TouchTime);
        }
        if (Touches[fingerID].hold_locked == false)//ホールドをしている
           // Touches[fingerID].lane_holding == true &&  //ホールドをしているかの判定もするとホールド判定していないうちにレーンが変わると反応しなくなる
        {
            int lane = JudgeLane(NowPos);
            if (Touches[fingerID].lane != lane)//違うレーンにスワイプしたら
            {
                Judge_manager.Holdbreak(Touches[fingerID].lane, Time_manager.count_time);
                Touches[fingerID].hold_locked = true;
                text.GetComponent<Text>().text = "違うレーン";
            }
        }
        

    }

    //ホールドが終了してTouches[fingerID].lane_holdingとTouches[fingerID].hold_lockedをfalseにする→TouchHoldingのホールド開始の処理がされてしまう
    //↑直したがスワイプで複数回処理されてしまう
    //↑最後のノーツなのでインデックスが動かない→フレームごとに処理されてしまう
    //直したはず


    /// <summary>
    /// ホールドし続けている
    /// </summary>
    void TouchHolding(Touch touch, int fingerID)
    {
        //指が止まっている＝レーンが変わることはないので←打ち消し、レーンにホールドノートがあるかを判定するのにレーン情報が必要
        Vector3 NowPos = Camera.main.ScreenToWorldPoint(touch.position);
        lane = JudgeLane(NowPos);//レーンを判断

        if (Time.realtimeSinceStartup - Touches[fingerID].hold_start_time >= hold_judge_Time && Touches[fingerID].lane_holding == false 
            && Touches[fingerID].hold_locked == false && lane_hold_allow[lane] == true)//Stationaryが一定時間を超え、かつホールド状態でない、かつホールド判定を拒否しておらず、さらにレーン上にホールドノートがある
        {
            Touches[fingerID].lane_holding = true;
            
            //Debug.Log("hold counter");
        }
        else if (Touches[fingerID].lane_holding == true && Touches[fingerID].hold_locked == false)//ホールド判定中でホールド判定を拒否していない 
        {
            Judge_manager.Holding(Touches[fingerID].lane);//ホールド時間を超えてないか
        }


    }

    /// <summary>
    /// タッチが終わった＝指が離れた
    /// </summary>
    /// <param name="touch"></param>
    /// <param name="ID"></param>
    void TouchEnd(Touch touch, int fingerID)
    {
        //
        Vector3 EndPos = Camera.main.ScreenToWorldPoint(touch.position);
        lane = JudgeLane(EndPos);//レーンを判断

        if (Judge_manager.isHoldNote(lane))
            //Touches[fingerID].lane_holding == true)//ホールド中のレーンだったら//ホールドをしているかの判定もするとホールド判定していないうちにレーンが変わると反応しなくなる
        {
            Touches[fingerID].lane_holding = false;
            Judge_manager.Holdbreak(Touches[fingerID].lane, Time_manager.count_time);

            Debug.Log("hold false");
            text.GetComponent<Text>().text = "指が離れた";
        }
    }






    /// <summary>
    /// 指がどのレーンなのか
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    int JudgeLane(Vector3 pos)
    {
        int lane = 0;
        if (pos.x >= -10 && -5.6 >= pos.x && data_warehouse.lane1_Makes.Length != 0)//-10より大きく、-5.6よりも小さいかつノートがある
        {
            lane = 1;
        }
        if (pos.x > -5.6 && -1.85 >= pos.x && data_warehouse.lane2_Makes.Length != 0)//-5.6より大きく、-1.85よりも小さい
        {
            lane = 2;
        }
        if (pos.x > -1.85 && 1.9 >= pos.x && data_warehouse.lane3_Makes.Length != 0)//-1.85より大きく、1.9よりも小さい
        {
            lane = 3;
        }
        if (pos.x > 1.9 && 5.6 >= pos.x && data_warehouse.lane4_Makes.Length != 0)//1.9より大きく、5.6よりも小さい
        {
            lane = 4;
        }
        if (pos.x > 5.6 && 10 >= pos.x && data_warehouse.lane5_Makes.Length != 0)//5.6より大きく、10よりも小さい
        {
            lane = 5;
        }

        return lane;
    }





    /// <summary>
    /// Touches[fingerID].hold_lockedの解除
    /// </summary>
    public void hold_unlock(int lane)
    {
        for (int i = 0; i < Touches.Length; i++)
        {
            if (Touches[i].lane == lane)
            {
                Touches[i].hold_locked = false;
                Touches[i].lane_holding = false;
                Touches[i].hold_start_time = 10000000000;
                break;
            }
        }
    }


    
    public struct Touch_rect
    {
    //必要なもの インデックス(はいらない)、最初にタッチした時間、 最初の位置、レーン(最初の位置に基づく)、ホールド状態
        public int TouchTime,lane;//最初にタッチした時間 レーン
        public Vector3 Startpos;//最初の位置
        public bool lane_holding,hold_locked; //レーン(最初の位置に基づく)、指の状態に関わらずホールドに関わる判定をさせないか(ホールド中の指を動かしたときに再度ホールドするまではホールド判定を起こさないように
        public float hold_start_time;//ホールドになるタッチをした時間

        public Touch_rect( int Te, Vector3 Ss, int le,bool lg,float he,bool hd)
        {
            TouchTime = Te;
            Startpos = Ss;
            lane = le;
            lane_holding = lg;
            hold_start_time = he;
            hold_locked = hd;
        }

    }
    



}
