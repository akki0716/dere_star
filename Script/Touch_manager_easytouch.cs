using HedgehogTeam.EasyTouch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// タッチ入力の処理とその結果のノーツ判定までを行う(判定した後の処理は別)
/// </summary>
public class Touch_manager_easytouch : MonoBehaviour {

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
    void Start()
    {
        Debug.Log("easy touch manager");
    }
    // Update is called once per frame
    void Update()
    {

    }
    // Subscribe to events
    void OnEnable()
    {
        EasyTouch.On_TouchStart += On_TouchStart;
        EasyTouch.On_Swipe += On_Swipe;
        EasyTouch.On_Cancel += On_Cancel;
    }
    // Unsubscribe
    void OnDisable()
    {
        EasyTouch.On_TouchStart -= On_TouchStart;
        EasyTouch.On_Swipe -= On_Swipe;
        EasyTouch.On_Cancel -= On_Cancel;
    }
    // Unsubscribe
    void OnDestroy()
    {
        EasyTouch.On_TouchStart -= On_TouchStart;
        EasyTouch.On_Swipe -= On_Swipe;
        EasyTouch.On_Cancel -= On_Cancel;
    }

    

    // Touch start event
    public void On_TouchStart(Gesture gesture)
    {
        Debug.Log("Touch in " + gesture.position);
        
        TouchBegan(gesture, gesture.fingerIndex);
    }


    public void On_Swipe(Gesture gesture)
    {
        Debug.Log("On_Swipe " + Camera.main.ScreenToWorldPoint(gesture.position));
        //座標タイプで行く
        TouchSwipe(gesture, gesture.fingerIndex);
    }


    public void On_Cancel(Gesture gesture)
    {
        Debug.Log("cancel");
    }








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
    void TouchBegan(Gesture gesture, int ID)
    {
        GetTouchTime(gesture.fingerIndex);
        StartPos = Camera.main.ScreenToWorldPoint(gesture.position);
        Debug.Log("Pos " + StartPos);
        lane = JudgeLane(StartPos);//レーンを判断
        Judge_manager.Judge(lane, 1, TouchTime[ID]);
    }



    /// <summary>
    /// スワイプ認識時点の処理
    /// </summary>
    void TouchSwipe(Gesture gesture, int ID)
    {
        GetTouchTime(gesture.fingerIndex);
        if (gesture.swipeVector != Vector2.zero)
        {
            Debug.Log("swipeVector " + gesture.swipe);
            lane = JudgeLane(Camera.main.ScreenToWorldPoint(gesture.position));//レーンを判断
            Debug.Log("lane" + lane);
            Judge_manager.Judge(lane, 2, TouchTime[ID]);
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
