using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ノートの出現を司る
/// </summary>
public class Note_manager : MonoBehaviour {
    //--------------他のスクリプト------------------------
    [SerializeField]
   BGM_manager BGM_manager;

    [SerializeField]
    data_warehouse data_warehouse;

    [SerializeField]
    Time_manager Time_manager;

    [SerializeField]
    Note_ObjectPool Note_ObjectPool;

    //--------------BGM再生時間関係---------------------
    /// <summary>
    /// BGMの再生開始時間
    /// </summary>
    int BGM_start_time;

    //--------------ノート再生時間関係---------------------

    /// <summary>
    /// 現在のBPM
    /// </summary>
    double Now_BPM;

    /// <summary>
    /// 音符が流れるのにかかる時間。
    /// </summary>
    int steam_time = 0;

    /// <summary>
    /// 音符が流れる基本時間。HS1,BPM100のときn秒かけて流れる
    /// </summary>
    double base_steam_time = 3;

    /// <summary>
    /// 音符が流れる時間をfloatにしてノートに使えるように
    /// </summary>
    public static float   float_steam_time;

    /// <summary>
    /// ハイスピ
    /// </summary>
    [SerializeField]
    double HS;

    /// <summary>
    /// 現在の時間
    /// </summary>
    int count_start_time;



    /// <summary>
    /// BGMを再生しているかのフラグ
    /// </summary>
    bool _isBGMplay = false;


    /// <summary>
    /// 再生前に作っておくノートの数
    /// </summary>
    int Start_Make_vol = 10;


    //--------------ノート再生時間関係---------------------
    /// <summary>
    /// レーン1にノーツがあるか(=探索するか)
    /// </summary>
    public bool _isSearch_lane1;

    /// <summary>
    /// レーン2にノーツがあるか(=探索するか)
    /// </summary>
    public bool _isSearch_lane2;

    /// <summary>
    /// レーン3にノーツがあるか(=探索するか)
    /// </summary>
    public bool _isSearch_lane3;

    /// <summary>
    /// レーン4にノーツがあるか(=探索するか)
    /// </summary>
    public bool _isSearch_lane4;

    /// <summary>
    /// レーン5にノーツがあるか(=探索するか)
    /// </summary>
    public bool _isSearch_lane5;


    /// <summary>
    /// レーン1の直近の生成すべきタイミング
    /// </summary>
    int lane1_timing;

    /// <summary>
    /// レーン1の生成すべきタイミングのインデックス
    /// </summary>
    int lane1_index  = 0;

    /// <summary>
    /// レーン2の生成すべきタイミングのインデックス
    /// </summary>
    int lane2_index = 0;




    


    //--------------以下メソッド---------------------



    void Start () {
        if (Application.platform == RuntimePlatform.Android)// Android機種なら
        {
            BGM_start_time = BGM_manager.BGM_delay();//遅延対策
        }
        Now_BPM = data_warehouse.bpm_property[0].bpm;
        Debug.Log(data_warehouse.lane1_notes[0].timing);
        steam_time = steam_time_decide(Now_BPM, 1 / HS);//音符を流す基本時間を決める
        count_start_time = BGM_manager.start_decide(steam_time);//カウントを開始する時間を出す
        Time_manager.count_time = count_start_time;//それをセットする
        BGM_manager.BGM_set();//BGMをセットする
        _isSearch_lane();
        DG.Tweening.DOTween.Init(false, true);

        for (int i = 0; i < Start_Make_vol; i++)//最初にある程度ノーツを作っておく
        {
            Note_ObjectPool.Start_Make();
        }
        Time_manager._is_timecount = true;//時間測定開始

    }
	
	
	void FixedUpdate ()
    {
        Note_maker();





    }

    /*
    void Update()
    {

        Through_Notes();



    }
    */


    /// <summary>
    /// 時間を読んでノート(とBGM)生成を行う
    /// </summary>
    void Note_maker()
    {
        if (Time_manager._is_timecount == true)//タイム計測中なら
        {
            if (_isBGMplay == false && Time_manager.count_time == BGM_manager.BGM_start_time)//bgmが鳴っていないかつBGM再生時間に来たら
            {
                BGM_manager.BGM_play();
                Debug.Log("bgm start " + Time_manager.count_time);
                _isBGMplay = true;
            }
            if (_isSearch_lane1 == true && (Time_manager.count_time + steam_time) == data_warehouse.lane1_notes[lane1_index].timing)
            {
                Note_ObjectPool.Note_Make(1, data_warehouse.lane1_notes[lane1_index].type, float_steam_time);
                if (lane1_index < (data_warehouse.lane1_notes.Length - 1))
                {
                    lane1_index++;
                }
                //Debug.Log("ノート作成");
            }
            if (_isSearch_lane2 == true && (Time_manager.count_time + steam_time) == data_warehouse.lane2_notes[lane2_index].timing)
            {
                Note_ObjectPool.Note_Make(2, data_warehouse.lane2_notes[lane2_index].type, float_steam_time);
                if (lane2_index < (data_warehouse.lane2_notes.Length - 1))
                {
                    lane2_index++;
                }
                //Debug.Log("ノート作成");
            }


        }
    }






    /// <summary>
    /// 音符が流れる速度を決める
    /// </summary>
    /// <param name="N_BPM">現在のBPM</param>
    /// <param name="HiSpeed">HS</param>
    /// <returns></returns>
    int steam_time_decide(double N_BPM, double HiSpeed)
    {
        double x; //基準bpmからの倍率
        double base_time;//現在のbpmでhs1の時の流れる時間
        int steam_time_accuracy = 100; //音符が流れる時間の精度、score_load.csのnote_accuracyと連動
        x = 100 / N_BPM;
        base_time = base_steam_time * x * HiSpeed; //bpm100,HS1のとき3秒かけて流れる
        //Debug.Log("base_time  " + base_time);
        float_steam_time = (float)base_time;
        //Debug.Log("float_steam_time  " + float_steam_time);

        return ((int)(base_time * steam_time_accuracy));
    }


    /// <summary>
    /// 各レーンにノートが入っているか判断してフラグ立てをしておく
    /// </summary>
    void _isSearch_lane()
    {
        if (data_warehouse.lane1_notes.Length != 0)
        {
            _isSearch_lane1 = true;
        }
        else
        {
            _isSearch_lane1 = false;
        }

        if (data_warehouse.lane2_notes.Length != 0)
        {
            _isSearch_lane2 = true;
        }
        else
        {
            _isSearch_lane2 = false;
        }

        if (data_warehouse.lane3_notes.Length != 0)
        {
            _isSearch_lane3 = true;
        }
        else
        {
            _isSearch_lane3 = false;
        }

        if (data_warehouse.lane4_notes.Length != 0)
        {
            _isSearch_lane4 = true;
        }
        else
        {
            _isSearch_lane4 = false;
        }

        if (data_warehouse.lane5_notes.Length != 0)
        {
            _isSearch_lane5 = true;
        }
        else
        {
            _isSearch_lane5 = false;
        }
    }

    public static float Get_float_steam_time()
    {
        return float_steam_time;
    }


}
