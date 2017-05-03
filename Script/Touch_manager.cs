using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// タッチ入力の処理とその結果のノーツ判定までを行う(判定した後の処理は別)
/// </summary>
public class Touch_manager : MonoBehaviour {

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

    /// <summary>
    /// どのレーンがホールド状態になっているか
    /// </summary>
    public bool[] lane_holding = new bool [6];

    /// <summary>
    /// どのぐらいの指の動きをスワイプとするか
    /// </summary>
    [SerializeField]
     float SwipeSensibility;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)//とりあえずタッチした時間を取っておく
            {
                touch = Input.GetTouch(i);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        GetTouchTime(i);
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
                        TouchBegan(touch,i);
                        break;
                    case TouchPhase.Moved:
                        TouchMoved(touch, i);
                        break;
                    case TouchPhase.Stationary:
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
        TouchTime[fingerID] = Time_manager.count_time;
    }




    /// <summary>
    /// タッチ開始時点の処理
    /// </summary>
    void TouchBegan(Touch touch,int ID)
    {
        
        //Debug.Log("touch.position " + touch.position);
        StartPos = Camera.main.ScreenToWorldPoint(touch.position);
        Debug.Log("Pos " + StartPos);
        lane = JudgeLane(StartPos);//レーンを判断
        Judge_manager.Judge(lane, 1, TouchTime[ID]);
    }



    /// <summary>
    /// タッチした指が動いた
    /// </summary>
    /// <param name="touch"></param>
    /// <param name="ID"></param>
    void TouchMoved(Touch touch, int ID)
    {
        //Debug.Log("moved");
        Vector3 NowPos = Camera.main.ScreenToWorldPoint(touch.position);
        if (Mathf.Abs(StartPos.x - NowPos.x) >= SwipeSensibility)
        {
            GetTouchTime(ID);
            Judge_manager.Judge(lane, 2, TouchTime[ID]);
        }
        else if (Mathf.Abs(StartPos.y - NowPos.y) >= SwipeSensibility)
        {
            GetTouchTime(ID);
            Judge_manager.Judge(lane, 2, TouchTime[ID]);
        }
    }



    /// <summary>
    /// ホールドし続けている
    /// </summary>
    void TouchHolding(Touch touch, int ID)
    {
        Vector3 NowPos = Camera.main.ScreenToWorldPoint(touch.position);
        lane = JudgeLane(NowPos);//レーンを判断
        if (lane_holding[lane] == false)//ホールドしたレーンから外れたら
        {
            Judge_manager.Holdkill(lane);
            Debug.Log("hold false");
        }
        if (lane_holding[lane] == true)//ホールド判定中なら
        {
            Judge_manager.Holding(lane);//ホールド時間を超えてないか
        }
        
    }


    void TouchEnd(Touch touch, int ID)
    {
        Vector3 EndPos = Camera.main.ScreenToWorldPoint(touch.position);
        lane = JudgeLane(EndPos);//レーンを判断
        if (lane_holding[lane] == true)//ホールド中のレーンだったら
        {
            Judge_manager.Holdbreak(lane, Time_manager.count_time);
            Debug.Log("hold false");
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












}
