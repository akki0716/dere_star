using UnityEngine;
using UnityEngine.UI;//テキスト表示用
using System; //Exception
using System.Text; //Encoding
using System.IO;
using System.Collections.Generic;
using System.Collections;//コルーチン用
using DG.Tweening;

//ノートを破壊したあとどうやら常にdisactiveになってるっぽい

public class Player : MonoBehaviour {

    public data_warehouse data_warehouse;//data_warehouseスクリプト

    public N_Note_ObjectPool N_note_objP;//N_Note_ObjectPoolスクリプト

    //演奏開始時間算出関連----------------------------------------------------------------------------------------------------
    int note_time_minimum;//最小のノートスタート時間
    int extra_time = 100;//カウントスタートより更に余分に遅らせる時間
    int steam_time = 0;//音符が流れるのにかかる時間。
    public float float_steam_time;//音符が流れる時間をfloatにしてノート側(Note_player)から読めるように
    double HS = 2;//ハイスピ
    double Now_BPM;//現在のbpm
    int BGM_start_time = 0;//BGMを流し始める時間。遅延対策に

    string[] settinglines;//BGM再生時間を変動させるために読み込んだ設定テキストの「行」


    //ノーツ再生関連----------------------------------------------------------------------------------------------------
    int time_count = 0;//現在の時間

    int search_note_lane1 = 0;//レーン1の今探索するノート
    int search_note_lane2 = 0;//レーン2の今探索するノート
    int search_note_lane3 = 0;//レーン3の今探索するノート
    int search_note_lane4 = 0;//レーン4の今探索するノート
    int search_note_lane5 = 0;//レーン5の今探索するノート
    int search_bpm = 0;//探索するbpm変動ポイント

    bool count_switch = false;//時間計測をするか

    public AudioSource BGM;//bgm再生用

    public GameObject searchtime_txt;//少数時間を表示する文字
    public GameObject time_count_txt;//少数時間を表示する文字
    public GameObject music_time_txt;//少数時間を表示する文字

    //判定関連----------------------------------------------------------------------------------------------------
    public int judge_note_lane1 = 0;//レーン1の判定を今探索するノート
    public int judge_note_lane2 = 0;//レーン2の判定を今探索するノート
    public int judge_note_lane3 = 0;//レーン3の判定を今探索するノート
    public int judge_note_lane4 = 0;//レーン4の判定を今探索するノート
    public int judge_note_lane5 = 0;//レーン5の判定を今探索するノート
    
    Vector2 m_TouchPos1;//タッチ座標 たぶん後で消す


    int touch_time;//タッチした時のtime_count

    float judge_point1_x_max = -5.6f;//レーン1の座標判定x最大 たぶん後で消す
    float judge_point1_x_min = -9;//レーン1の座標判定x最小 たぶん後で消す

    List<float> touch_area_x_max = new List<float>();//座標判定x最大
    List<float> touch_area_x_min = new List<float>();//座標判定x最小

    float  flick_area = -3.6f;//フリック判定するY座標

    int judge_time_PERFECT = 30;//判定範囲。ミリ秒。
    int judge_time_GREAT = 65;//判定範囲。ミリ秒。
    int judge_time_GOOD = 100;//判定範囲。ミリ秒。
    int judge_time_BAD = 150;//判定範囲。ミリ秒。

    public CriAtomSource atomSource_se;

    public GameObject PERFECT;
    public GameObject GREAT;
    public GameObject GOOD;


    public List<GameObject> Create_Notes_lane1 = new List<GameObject>();//作ったノートを保持する配列
    public List<GameObject> Create_Notes_lane2 = new List<GameObject>();//作ったノートを保持する配列
    public List<GameObject> Create_Notes_lane3 = new List<GameObject>();//作ったノートを保持する配列
    public List<GameObject> Create_Notes_lane4 = new List<GameObject>();//作ったノートを保持する配列
    public List<GameObject> Create_Notes_lane5 = new List<GameObject>();//作ったノートを保持する配列

    //各レーン別現在ホールド判断中かのフラグ
    bool Hold_switch_lane1 = false;
    bool Hold_switch_lane2 = false;
    bool Hold_switch_lane3 = false;
    bool Hold_switch_lane4 = false;
    bool Hold_switch_lane5 = false;

    //各レーン別判定するホールド秒数
    public int judge_Hold_lane1 = 0;//レーン1のホールド秒数を今探索するノート
    public int judge_Hold_lane2 = 0;//レーン2のホールド秒数を今探索するノート
    public int judge_Hold_lane3 = 0;//レーン3のホールド秒数を今探索するノート
    public int judge_Hold_lane4 = 0;//レーン4のホールド秒数を今探索するノート
    public int judge_Hold_lane5 = 0;//レーン5のホールド秒数を今探索するノート


    public float HoldTime_lane1 = 0;//ホールドアニメーションすべき秒数
    public float HoldTime_lane2 = 0;
    public float HoldTime_lane3 = 0;
    public float HoldTime_lane4 = 0;
    public float HoldTime_lane5 = 0;

    int Hold_start_lane1 = 0;//ホールドを開始した時間(＝ホールド最初のチップを判定した時間)
    int Hold_start_lane2 = 0;
    int Hold_start_lane3 = 0;
    int Hold_start_lane4 = 0;
    int Hold_start_lane5 = 0;


    int Hold_ID_lane1 = 0;//それぞれのレーンを、どの指でタッチしたかのfingerID
    int Hold_ID_lane2 = 0;
    int Hold_ID_lane3 = 0;
    int Hold_ID_lane4 = 0;
    int Hold_ID_lane5 = 0;


    //タッチした場所がどのレーンか。デバック用
    public string touch1;
    public string touch2;
    public string touch3;
    public string touch4;
    public string touch5;


    public int note_option_lane1 = 0;//レーン1の今判定するノートのオプション
    public int note_option_lane2 = 0;//レーン2の今判定するノートのオプション
    public int note_option_lane3 = 0;//レーン3の今判定するノートのオプション
    public int note_option_lane4 = 0;//レーン4の今判定するノートのオプション
    public int note_option_lane5 = 0;//レーン5の今判定するノートのオプション

    //public String des_target_note;//破壊対象のオブジェクトの名前


    public GameObject debug;//デバッグテキスト表示する文字。後で消す。


    void Start()
    {
        //GetComponent<Text>().text = "Player開始 ";
        //Debug.Log("judge_note_lane1 start" + judge_note_lane1);
       

        touch_area_decide();

        if (Application.platform == RuntimePlatform.Android)// Android機種なら
        {
            BGM_delay();
        }
        Now_BPM = data_warehouse.BPM_list[0];

        steam_time = steam_time_decide(Now_BPM, 1/ HS);
        //Debug.Log("steam_time " + steam_time);
        start_decide();
        count_switch = true;
        
        atomSource_se = gameObject.GetComponent<CriAtomSource>();
    }

    void FixedUpdate()
    {

        if (time_count == BGM_start_time)
        {
            BGM.Play();
        }

        if (count_switch == true)
        {
            note_search();
            miss_note_search();
            time_count++;

            searchtime_txt.GetComponent<Text>().text = "searchtime " + (time_count + steam_time).ToString();
            //Debug.Log("search " + (time_count + steam_time));
            time_count_txt.GetComponent<Text>().text = "time_count " + time_count.ToString();
            //BGMより先に音符を流しはじめきゃいけないので曲時間をそのまま取る方法は使えない
            //music_time_txt.GetComponent<Text>().text = "music_time " + ((BGM.time)*1000).ToString();
        }
        

    }

    void Update()
    {
        touch_judge();
        Hold_touch_judge();
    }

    void touch_area_decide()//各レーンのタッチ範囲を決定
    {
        touch_area_x_max.Add(-6f);
        touch_area_x_min.Add(-9f);

        touch_area_x_max.Add(-2f);
        touch_area_x_min.Add(-5f);

        touch_area_x_max.Add(2f);
        touch_area_x_min.Add(-1.5f);

        touch_area_x_max.Add(5.5f);
        touch_area_x_min.Add(2.5f);

        touch_area_x_max.Add(10f);
        touch_area_x_min.Add(6f);

        /*

        touch_area_x_max.Add(-5.6f);
        touch_area_x_min.Add(-9f);

        touch_area_x_max.Add(-2f);
        touch_area_x_min.Add(-5.59f);

        touch_area_x_max.Add(1.99f);
        touch_area_x_min.Add(-1.99f);

        touch_area_x_max.Add(5.6f);
        touch_area_x_min.Add(2f);

        touch_area_x_max.Add(10f);
        touch_area_x_min.Add(5.61f);
        */

    }

    int steam_time_decide(double N_BPM, double HiSpeed)//音符が流れる速度を決める
    {
        double x; //基準bpmからの倍率
        double base_time;//現在のbpmでhs1の時の流れる時間
        int steam_time_accuracy = 100; //音符が流れる時間の精度、score_load.csのnote_accuracyと連動
        x = 100/ N_BPM;
        base_time = 3 * x * HiSpeed;
        //Debug.Log("base_time  " + base_time);
        float_steam_time = (float)base_time;
        //Debug.Log("float_steam_time  " + float_steam_time);
        return ((int)(base_time * steam_time_accuracy));
    }


    void start_decide()//カウント開始時間を決定するメソッド
    {
        note_time_minimum = 100000;
        if (data_warehouse.note_timing_lane1.Count != 0)
        {
            if (note_time_minimum >= data_warehouse.note_timing_lane1[0])
            {
                note_time_minimum = data_warehouse.note_timing_lane1[0];
            }
        }
        if (data_warehouse.note_timing_lane2.Count != 0)
        {
            if (note_time_minimum >= data_warehouse.note_timing_lane2[0])
            {
                note_time_minimum = data_warehouse.note_timing_lane2[0];
            }
        }
        if (data_warehouse.note_timing_lane3.Count != 0)
        {
            if (note_time_minimum >= data_warehouse.note_timing_lane3[0])
            {
                note_time_minimum = data_warehouse.note_timing_lane3[0];
            }
        }
        if (data_warehouse.note_timing_lane4.Count != 0)
        {
            if (note_time_minimum >= data_warehouse.note_timing_lane4[0])
            {
                note_time_minimum = data_warehouse.note_timing_lane4[0];
            }
        }
        if (data_warehouse.note_timing_lane5.Count != 0)
        {
            if (note_time_minimum >= data_warehouse.note_timing_lane5[0])
            {
                note_time_minimum = data_warehouse.note_timing_lane5[0];
            }
        }

        // Debug.Log("最小時間 " + note_time_minimum);
        time_count = note_time_minimum - (steam_time + extra_time);
        if (time_count >= 0)
        {
            time_count = -200;
        }
        //time_count = -1000;
        //Debug.Log("カウント開始時間 " + time_count);
    }

    void note_search()//流すノートの探索・生成
    {

        if (data_warehouse.note_timing_lane1.Count != 0)//レーン1にノートが入っているか
        {
            if ((time_count + steam_time) == data_warehouse.note_timing_lane1[search_note_lane1])//今の時間がノートを流すべき時間なら
            {
                
                N_Note_ObjectPool.instance.getNote(1, data_warehouse.note_option_lane1[search_note_lane1]);
                //Debug.Log("レーン1時間です " + (time_count + steam_time));
                if (search_note_lane1 < (data_warehouse.note_timing_lane1.Count - 1))
                {
                    search_note_lane1++;
                }
            }
        }
        
        if (data_warehouse.note_timing_lane2.Count != 0)//レーン2にノートが入っているか
        {
            if ((time_count + steam_time) == data_warehouse.note_timing_lane2[search_note_lane2])//今の時間がノートを流すべき時間なら
            {
                N_Note_ObjectPool.instance.getNote(2, data_warehouse.note_option_lane2[search_note_lane2]);
                //Debug.Log("レーン2時間です " + (time_count + steam_time));
                if (search_note_lane2 < (data_warehouse.note_timing_lane2.Count - 1))
                {
                    search_note_lane2++;
                }
            }
        }

        if (data_warehouse.note_timing_lane3.Count != 0)//レーン3にノートが入っているか
        {
            if ((time_count + steam_time) == data_warehouse.note_timing_lane3[search_note_lane3])//今の時間がノートを流すべき時間なら
            {
                N_Note_ObjectPool.instance.getNote(3, data_warehouse.note_option_lane3[search_note_lane3]);
                //Debug.Log("レーン3時間です " + (time_count + steam_time));
                if (search_note_lane3 < (data_warehouse.note_timing_lane3.Count - 1))
                {
                    search_note_lane3++;
                }
            }
        }
        if (data_warehouse.note_timing_lane4.Count != 0)//レーン4にノートが入っているか
        {
            if ((time_count + steam_time) == data_warehouse.note_timing_lane4[search_note_lane4])//今の時間がノートを流すべき時間なら
            {
                N_Note_ObjectPool.instance.getNote(4, data_warehouse.note_option_lane4[search_note_lane4]);
                //Debug.Log("レーン4時間です " + (time_count + steam_time));
                if (search_note_lane4 < (data_warehouse.note_timing_lane4.Count - 1))
                {
                    search_note_lane4++;
                }
            }
        }
        if (data_warehouse.note_timing_lane5.Count != 0)//レーン5にノートが入っているか
        {
            if ((time_count + steam_time) == data_warehouse.note_timing_lane5[search_note_lane5])//今の時間がノートを流すべき時間なら
            {
                N_Note_ObjectPool.instance.getNote(5, data_warehouse.note_option_lane5[search_note_lane5]);
                //Debug.Log("レーン5時間です " + (time_count + steam_time));
                if (search_note_lane5 < (data_warehouse.note_timing_lane5.Count - 1))
                {
                    search_note_lane5++;
                }
            }
        }



        if (data_warehouse.BPM_change_time.Count != 0)//bpm変動があるか
        {
            if ((time_count + steam_time) == data_warehouse.BPM_change_time[search_bpm])//今の時間はbpm変動の時間か
            {
                Debug.Log("bpm変動 " + (time_count + steam_time));
                if (search_bpm < (data_warehouse.BPM_change_time.Count - 1))
                {
                    search_bpm++;
                }
            }
        }
        
    }
    

    void miss_note_search()//見逃しmissのノートの探索
    {
        if (data_warehouse.note_timing_lane1.Count != 0)//レーン1にノートが入っているか
        {
            
            if ((data_warehouse.note_timing_lane1[judge_note_lane1] + judge_time_BAD +1) == time_count)//今の時間がノートがBADになる時間なら
            {
                //Debug.Log("miss判定時間1 " + (data_warehouse.note_timing_lane1[judge_note_lane1] + judge_time_BAD + 1));
                //Debug.Log("judge_note_lane1 a " + judge_note_lane1);
                if (Create_Notes_lane1[judge_note_lane1].activeInHierarchy == true)//オブジェクトがまだある
                {
 //                   Debug.Log("レーン1 BAD");
                    if ((data_warehouse.note_timing_lane1.Count-1) > judge_note_lane1 )
                    {
                        judge_note_lane1++;
                        //Debug.Log("judge_note_lane1  b" + judge_note_lane1);
                        //Debug.Log("miss判定時間2 " + (data_warehouse.note_timing_lane1[judge_note_lane1] + judge_time_BAD + 1));
                    }
                    
                }
                
            }
            
        }
        if (data_warehouse.note_timing_lane2.Count != 0)//レーン2にノートが入っているか
        {
            if ((data_warehouse.note_timing_lane2[judge_note_lane2] + judge_time_BAD + 1) == time_count)//今の時間がノートがBADになる時間なら
            {
                if (Create_Notes_lane2[judge_note_lane2].activeInHierarchy == true)//オブジェクトがまだある
                {
   //                 Debug.Log("レーン2 BAD");
                    if ((data_warehouse.note_timing_lane2.Count - 1) > judge_note_lane2)
                    {
                        judge_note_lane2++;
                    }

                }
            }
        }
        if (data_warehouse.note_timing_lane3.Count != 0)//レーン3にノートが入っているか
        {
            if ((data_warehouse.note_timing_lane3[judge_note_lane3] + judge_time_BAD + 1) == time_count)//今の時間がノートがBADになる時間なら
            {
                if (Create_Notes_lane3[judge_note_lane3].activeInHierarchy == true)//オブジェクトがまだある
                {
     //               Debug.Log("レーン3 BAD");
                    if ((data_warehouse.note_timing_lane3.Count - 1) > judge_note_lane3)
                    {
                        judge_note_lane3++;
                    }

                }
            }
        }
        if (data_warehouse.note_timing_lane4.Count != 0)//レーン4にノートが入っているか
        {
            if ((data_warehouse.note_timing_lane4[judge_note_lane4] + judge_time_BAD + 1) == time_count)//今の時間がノートがBADになる時間なら
            {
                if (Create_Notes_lane4[judge_note_lane4].activeInHierarchy == true)//オブジェクトがまだある
                {
    //                Debug.Log("レーン4 BAD");
                    if ((data_warehouse.note_timing_lane4.Count - 1) > judge_note_lane4)
                    {
                        judge_note_lane4++;
                    }

                }
            }
        }
        if (data_warehouse.note_timing_lane5.Count != 0)//レーン2にノートが入っているか
        {
            if ((data_warehouse.note_timing_lane5[judge_note_lane5] + judge_time_BAD + 1) == time_count)//今の時間がノートがBADになる時間なら
            {
                if (Create_Notes_lane5[judge_note_lane5].activeInHierarchy == true)//オブジェクトがまだある
                {
    //                Debug.Log("レーン5 BAD");
                    if ((data_warehouse.note_timing_lane5.Count - 1) > judge_note_lane5)
                    {
                        judge_note_lane5++;
                    }

                }
            }
        }
    }


    void BGM_delay()//Android遅延対策用
    {
        string settingfile;
        string sttingcontent = "";
        
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (AndroidJavaObject externalFilesDir = currentActivity.Call<AndroidJavaObject>("getExternalFilesDir", null))
                {
                    settingfile = externalFilesDir.Call<string>("getCanonicalPath") + "/setting.txt";
                }
            }
        }

        FileInfo file = new FileInfo(settingfile);
        try
        {
            // 一行毎読み込み
            using (StreamReader sr = new StreamReader(file.OpenRead(), Encoding.Default))
            {
                sttingcontent = sr.ReadToEnd();

                //Debug.Log("bms" + bms);
                settinglines = sttingcontent.Split('\n'); //linesに1行毎にツッコむ
                //Debug.Log("lines" + lines);
            }

        }
        catch (Exception e)
        {
            // 改行コード
            sttingcontent += "ああああ";
            Debug.Log("エラー ");

        }


        foreach (var adb in settinglines)//ファイル内部のすべての行をループ
        {

            if(adb.Substring(0, 4) == "#Del")
            {
                BGM_start_time =  int.Parse(adb.Substring(10));
            }

        }
            
    }

    void option_search()//判定するノートのオプション(種類)を検索)
    {
        if (data_warehouse.note_option_lane1.Count != 0)
        {
            note_option_lane1 = data_warehouse.note_option_lane1[judge_note_lane1];
        }
        if (data_warehouse.note_option_lane2.Count != 0)
        {
            note_option_lane2 = data_warehouse.note_option_lane2[judge_note_lane2];
        }
        if (data_warehouse.note_option_lane3.Count != 0)
        {
            note_option_lane3 = data_warehouse.note_option_lane3[judge_note_lane3];
        }
        if (data_warehouse.note_option_lane4.Count != 0)
        {
            note_option_lane4 = data_warehouse.note_option_lane4[judge_note_lane4];
        }
        if (data_warehouse.note_option_lane5.Count != 0)
        {
            note_option_lane5 = data_warehouse.note_option_lane5[judge_note_lane5];
        }
        
        
        
        
    }

    void touch_judge()//タッチ判定
    {

        /*
        if (Input.touchCount > 0)//タッチが1箇所以上
        {
            for (int i = 0; i < Input.touchCount; i++)//今タッチされてる指の数だけ iは指の数
            {
                for (int y = 0; y < 5; y++)//yはレーン番号
                {
                    if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                        Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                        Input.GetTouch(i).phase == TouchPhase.Began)//タッチが範囲内
                                                                    //Input.GetTouch(i).phase == TouchPhase.Moved)//タッチが範囲内
                    {
                        judge(y,1);
                    }
                }
            }
        }*/
        
        if (Input.touchCount > 0)//タッチが1箇所以上
        {
            option_search();
            for (int i = 0; i < Input.touchCount; i++)//今タッチされてる指の数だけ iは指の数
            {
                for (int y = 0; y < 5; y++)//yはレーン番号
                {
                    if (note_option_lane1 == 1)//通常タッチ
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Input.GetTouch(i).phase == TouchPhase.Began)//タッチが範囲内
                                                                        //Input.GetTouch(i).phase == TouchPhase.Moved)//タッチが範囲内
                        {
                            judge(y, note_option_lane1);
                        }
                    }
                    else if (note_option_lane1 == 2)//フリック
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).y >= flick_area &&
                            Input.GetTouch(i).phase == TouchPhase.Moved)//タッチが範囲内

                        {
                            judge(y, note_option_lane1);
                        }
                    }
                    else if (note_option_lane1 == 3)//ホールド
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Input.GetTouch(i).phase == TouchPhase.Began)//タッチが範囲内
                        {
                            judge(y, note_option_lane1);
                            Hold_ID_lane1 = i;

                            debug.GetComponent<Text>().text = "Touching! ";
                        }



                    }


                    if (note_option_lane2 == 1)//通常タッチ
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Input.GetTouch(i).phase == TouchPhase.Began)//タッチが範囲内
                                                                        //Input.GetTouch(i).phase == TouchPhase.Moved)//タッチが範囲内
                        {
                            judge(y, note_option_lane2);
                        }
                    }
                    else if (note_option_lane2 == 2)//フリック
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).y >= flick_area &&
                            Input.GetTouch(i).phase == TouchPhase.Moved)//タッチが範囲内

                        {
                            judge(y, note_option_lane2);
                        }
                    }
                    else if (note_option_lane2 == 3)//ホールド
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Input.GetTouch(i).phase == TouchPhase.Began)//タッチが範囲内
                        {
                            judge(y, note_option_lane2);
                            Hold_ID_lane2 = i;

                            debug.GetComponent<Text>().text = "Touching! ";
                        }

                    }


                    if (note_option_lane3 == 1)//通常タッチ
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Input.GetTouch(i).phase == TouchPhase.Began)//タッチが範囲内
                                                                        //Input.GetTouch(i).phase == TouchPhase.Moved)//タッチが範囲内
                        {
                            judge(y, note_option_lane3);
                        }
                    }
                    else if (note_option_lane3 == 2)//フリック
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).y >= flick_area &&
                            Input.GetTouch(i).phase == TouchPhase.Moved)//タッチが範囲内

                        {
                            judge(y, note_option_lane3);
                        }
                    }
                    else if (note_option_lane3 == 3)//ホールド
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Input.GetTouch(i).phase == TouchPhase.Began)//タッチが範囲内
                        {
                            judge(y, note_option_lane3);
                            Hold_ID_lane3 = i;

                            debug.GetComponent<Text>().text = "Touching! ";
                        }
                    }


                    if (note_option_lane4 == 1)//通常タッチ
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Input.GetTouch(i).phase == TouchPhase.Began)//タッチが範囲内
                                                                        //Input.GetTouch(i).phase == TouchPhase.Moved)//タッチが範囲内
                        {
                            judge(y, note_option_lane4);
                        }
                    }
                    else if (note_option_lane4 == 2)//フリック
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).y >= flick_area &&
                            Input.GetTouch(i).phase == TouchPhase.Moved)//タッチが範囲内

                        {
                            judge(y, note_option_lane4);
                        }
                    }
                    else if (note_option_lane4 == 3)//ホールド
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Input.GetTouch(i).phase == TouchPhase.Began)//タッチが範囲内
                        {
                            judge(y, note_option_lane4);
                            Hold_ID_lane4 = i;

                            debug.GetComponent<Text>().text = "Touching! ";
                        }
                    }

                    if (note_option_lane5 == 1)//通常タッチ
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Input.GetTouch(i).phase == TouchPhase.Began)//タッチが範囲内
                                                                        //Input.GetTouch(i).phase == TouchPhase.Moved)//タッチが範囲内
                        {
                            judge(y, note_option_lane5);
                        }
                    }
                    else if (note_option_lane5 == 2)//フリック
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).y >= flick_area &&
                            Input.GetTouch(i).phase == TouchPhase.Moved)//タッチが範囲内

                        {
                            judge(y, note_option_lane5);
                        }
                    }
                    else if (note_option_lane5 == 3)//ホールド
                    {
                        if (Camera.main.ScreenToWorldPoint(Input.touches[i].position).x <= touch_area_x_max[y] &&
                            Camera.main.ScreenToWorldPoint(Input.touches[i].position).x >= touch_area_x_min[y] &&
                            Input.GetTouch(i).phase == TouchPhase.Began)//タッチが範囲内
                        {
                            judge(y, note_option_lane5);
                            Hold_ID_lane5 = i;

                            debug.GetComponent<Text>().text = "Touching! ";
                        }
                    }
                }
            }



        }



        /*以前の処理
        フリック無し
        */



        
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        { //そのままだとタッチでもマウスクリック部分が反応してしまうのでエディター上のみタッチ反応するように。完成版ではコメントアウトとかするべきだと思う。
            if (Input.GetMouseButtonDown(0))//マウスクリック。
            { 
                m_TouchPos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                for (int y = 0; y < 5; y++)//yはレーン番号
                {
                    if (m_TouchPos1.x <= touch_area_x_max[y] && m_TouchPos1.x >= touch_area_x_min[y])//タッチが範囲内
                    {
                        Debug.Log("クリック判定レーン:" + y);
                        judge(y,3);
                    }
                }
            }
        }
    
   

    }

    void judge(int lane,int option)//タッチタイミング判定
    {
        touch_time = time_count;//現在のタッチ時間
        
        Hold_Note_player Hold_Note_player;
        switch (lane)
        {
            case 0://レーン1
                //Debug.Log("レーン1");
                touch1 = "レーン1";
                if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane1[judge_note_lane1]) <= judge_time_PERFECT)
                {
                    //Explosion(1, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane1[judge_note_lane1], option);
                    }
                    
                    //Debug.Log("状態" + Create_Notes_lane1[judge_note_lane1].activeInHierarchy);

                    //Debug.Log("PERFECT！！");

                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if(option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane1 = touch_time;
                        HoldTime_lane1 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane1[judge_Hold_lane1] - (touch_time - data_warehouse.note_timing_lane1[judge_note_lane1]) );
                        HoldTime_lane1 = HoldTime_lane1 / 1000;
                        Debug.Log("HoldTime "  + HoldTime_lane1);
                        Hold_Note_player = Create_Notes_lane1[judge_note_lane1].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane1);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane1 = true;
                    }
                    if ((data_warehouse.note_timing_lane1.Count - 1) > judge_note_lane1)//ホールドの判定法いかんでは一番最後にこれを処理するべきかもしれない
                    {
                        judge_note_lane1++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane1[judge_note_lane1]) <= judge_time_GREAT)
                {
                    //Explosion(2, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane1[judge_note_lane1], option);
                    }
                    //Debug.Log("状態" + Create_Notes_lane1[judge_note_lane1].activeInHierarchy);
                    
                    //Debug.Log("GREAT！");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane1 = touch_time;
                        HoldTime_lane1 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane1[judge_Hold_lane1] - (touch_time - data_warehouse.note_timing_lane1[judge_note_lane1]));
                        HoldTime_lane1 = HoldTime_lane1 / 1000;
                        //Debug.Log("HoldTime " + HoldTime_lane1);
                        Hold_Note_player = Create_Notes_lane1[judge_note_lane1].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane1);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane1 = true;
                    }
                    if ((data_warehouse.note_timing_lane1.Count - 1) > judge_note_lane1)
                    {
                        judge_note_lane1++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane1[judge_note_lane1]) <= judge_time_GOOD)
                {
                    //Explosion(3, lane);//判定文字.
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane1[judge_note_lane1], option);
                    }
                    //Debug.Log("状態" + Create_Notes_lane1[judge_note_lane1].activeInHierarchy);
                    
                    //Debug.Log("GOOD");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane1 = touch_time;
                        HoldTime_lane1 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane1[judge_Hold_lane1] - (touch_time - data_warehouse.note_timing_lane1[judge_note_lane1]));
                        HoldTime_lane1 = HoldTime_lane1 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane1);
                        Hold_Note_player = Create_Notes_lane1[judge_note_lane1].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane1);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane1 = true;
                    }
                    if ((data_warehouse.note_timing_lane1.Count - 1) > judge_note_lane1)
                    {
                        judge_note_lane1++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane1[judge_note_lane1]) <= judge_time_BAD)
                {
                    atomSource_se.Play("miss");
                    //Explosion(3);//判定文字
                    if (option == 3)
                    {
                        Hold_Note_player = Create_Notes_lane1[judge_note_lane1].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                    }
                    N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane1[judge_note_lane1], option);
                    //Debug.Log(Create_Notes_lane1[judge_note_lane1].name + "状態" + Create_Notes_lane1[judge_note_lane1].activeInHierarchy);
                    if ((data_warehouse.note_timing_lane1.Count - 1) > judge_note_lane1)
                    {
                        judge_note_lane1++;
                    }
                    Debug.Log("BAD");
                    

                }
                break;
            case 1:
                //Debug.Log("レーン2");
                touch2 = "レーン2";
                if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane2[judge_note_lane2]) <= judge_time_PERFECT)
                {
                    //Explosion(1, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane2[judge_note_lane2], option);
                    }

                    //Debug.Log("PERFECT！！");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane2 = touch_time;
                        HoldTime_lane2 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane2[judge_Hold_lane2] - (touch_time - data_warehouse.note_timing_lane2[judge_note_lane2]));
                        HoldTime_lane2 = HoldTime_lane2 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane2);
                        Hold_Note_player = Create_Notes_lane2[judge_note_lane2].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane2);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane2 = true;
                    }
                    if ((data_warehouse.note_timing_lane2.Count - 1) > judge_note_lane2)
                    {
                        judge_note_lane2++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane2[judge_note_lane2]) <= judge_time_GREAT)
                {
                    //Explosion(2, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane2[judge_note_lane2], option);
                    }
                        
                    
                    //Debug.Log("GREAT！");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane2 = touch_time;
                        HoldTime_lane2 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane2[judge_Hold_lane2] - (touch_time - data_warehouse.note_timing_lane2[judge_note_lane2]));
                        HoldTime_lane2 = HoldTime_lane2 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane2);
                        Hold_Note_player = Create_Notes_lane2[judge_note_lane2].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane2);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane2 = true;
                    }
                    if ((data_warehouse.note_timing_lane2.Count - 1) > judge_note_lane2)
                    {
                        judge_note_lane2++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane2[judge_note_lane2]) <= judge_time_GOOD)
                {
                    //Explosion(3, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane2[judge_note_lane2], option);
                    }
                        
                    
                    //Debug.Log("GOOD");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane2 = touch_time;
                        HoldTime_lane2 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane2[judge_Hold_lane2] - (touch_time - data_warehouse.note_timing_lane2[judge_note_lane2]));
                        HoldTime_lane2 = HoldTime_lane2 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane2);
                        Hold_Note_player = Create_Notes_lane2[judge_note_lane2].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane2);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane2 = true;
                    }
                    if ((data_warehouse.note_timing_lane2.Count - 1) > judge_note_lane2)
                    {
                        judge_note_lane2++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane2[judge_note_lane2]) <= judge_time_BAD)
                {
                    //Explosion(3);//判定文字
                    if (option == 3)
                    {
                        Hold_Note_player = Create_Notes_lane2[judge_note_lane2].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                    }
                    N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane2[judge_note_lane2], option);
                    
                    //Debug.Log("GOOD");
                    atomSource_se.Play("miss");
                    if ((data_warehouse.note_timing_lane2.Count - 1) > judge_note_lane2)
                    {
                        judge_note_lane2++;
                    }
                }
                break;
            case 2:
                //Debug.Log("レーン3");
                touch3 = "レーン3";
                if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane3[judge_note_lane3]) <= judge_time_PERFECT)
                {
                    //Explosion(1, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane3[judge_note_lane3], option);
                    }
                        
                    
                    //Debug.Log("PERFECT！！");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane3 = touch_time;
                        HoldTime_lane3 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane3[judge_Hold_lane3] - (touch_time - data_warehouse.note_timing_lane3[judge_note_lane3]));
                        HoldTime_lane3 = HoldTime_lane3 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane3);
                        Hold_Note_player = Create_Notes_lane3[judge_note_lane3].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane3);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane3 = true;
                    }
                    if ((data_warehouse.note_timing_lane3.Count - 1) > judge_note_lane3)
                    {
                        judge_note_lane3++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane3[judge_note_lane3]) <= judge_time_GREAT)
                {
                    //Explosion(2, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane3[judge_note_lane3], option);
                    }
                        
                    
                    //Debug.Log("GREAT！");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane3 = touch_time;
                        HoldTime_lane3 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane3[judge_Hold_lane3] - (touch_time - data_warehouse.note_timing_lane3[judge_note_lane3]));
                        HoldTime_lane3 = HoldTime_lane3 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane3);
                        Hold_Note_player = Create_Notes_lane3[judge_note_lane3].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane3);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane3 = true;
                    }
                    if ((data_warehouse.note_timing_lane3.Count - 1) > judge_note_lane3)
                    {
                        judge_note_lane3++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane1[judge_note_lane3]) <= judge_time_GOOD)
                {
                    //Explosion(3, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane3[judge_note_lane3], option);
                    }
                        
                    
                    //Debug.Log("GOOD");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane3 = touch_time;
                        HoldTime_lane3 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane3[judge_Hold_lane3] - (touch_time - data_warehouse.note_timing_lane3[judge_note_lane3]));
                        HoldTime_lane3 = HoldTime_lane3 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane3);
                        Hold_Note_player = Create_Notes_lane3[judge_note_lane3].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane3);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane3 = true;
                    }
                    if ((data_warehouse.note_timing_lane3.Count - 1) > judge_note_lane3)
                    {
                        judge_note_lane3++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane1[judge_note_lane3]) <= judge_time_BAD)
                {
                    atomSource_se.Play("miss");
                    //Explosion(3);//判定文字
                    if (option == 3)
                    {
                        Hold_Note_player = Create_Notes_lane3[judge_note_lane3].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                    }
                    N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane3[judge_note_lane3], option);

                    //Debug.Log("GOOD");
                    
                    if ((data_warehouse.note_timing_lane3.Count - 1) > judge_note_lane3)
                    {
                        judge_note_lane3++;
                    }

                }
                break;
            case 3:
                //Debug.Log("レーン4");
                touch4 = "レーン4";
                if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane4[judge_note_lane4]) <= judge_time_PERFECT)
                {
                    //Explosion(1, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane4[judge_note_lane4], option);
                    }
                        
                    
                    //Debug.Log("PERFECT！！");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane4 = touch_time;
                        HoldTime_lane4 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane4[judge_Hold_lane4] - (touch_time - data_warehouse.note_timing_lane4[judge_note_lane4]));
                        HoldTime_lane4 = HoldTime_lane4 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane4);
                        Hold_Note_player = Create_Notes_lane4[judge_note_lane4].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane4);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane4 = true;
                    }
                    if ((data_warehouse.note_timing_lane4.Count - 1) > judge_note_lane4)
                    {
                        judge_note_lane4++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane4[judge_note_lane4]) <= judge_time_GREAT)
                {
                    //Explosion(2, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane4[judge_note_lane4], option);
                    }
                        
                    
                    //Debug.Log("GREAT！");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane4 = touch_time;
                        HoldTime_lane4 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane4[judge_Hold_lane4] - (touch_time - data_warehouse.note_timing_lane4[judge_note_lane4]));
                        HoldTime_lane4 = HoldTime_lane4 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane4);
                        Hold_Note_player = Create_Notes_lane4[judge_note_lane4].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane4);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane4 = true;
                    }
                    if ((data_warehouse.note_timing_lane4.Count - 1) > judge_note_lane4)
                    {
                        judge_note_lane4++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane1[judge_note_lane4]) <= judge_time_GOOD)
                {
                    //Explosion(3, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane4[judge_note_lane4], option);
                    }
                        
                    
                    //Debug.Log("GOOD");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane4 = touch_time;
                        HoldTime_lane4 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane4[judge_Hold_lane4] - (touch_time - data_warehouse.note_timing_lane4[judge_note_lane4]));
                        HoldTime_lane4 = HoldTime_lane4 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane4);
                        Hold_Note_player = Create_Notes_lane4[judge_note_lane4].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane4);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane4 = true;
                    }
                    if ((data_warehouse.note_timing_lane4.Count - 1) > judge_note_lane4)
                    {
                        judge_note_lane4++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane1[judge_note_lane4]) <= judge_time_BAD)
                {
                    //Explosion(3);//判定文字
                    if (option == 3)
                    {
                        Hold_Note_player = Create_Notes_lane4[judge_note_lane4].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                    }
                    atomSource_se.Play("miss");
                    N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane4[judge_note_lane4], option);
                    
                    //Debug.Log("GOOD");
                    
                    if ((data_warehouse.note_timing_lane4.Count - 1) > judge_note_lane4)
                    {
                        judge_note_lane4++;
                    }
                }
                break;
            case 4:
                //Debug.Log("レーン5");
                touch5 = "レーン5";
                if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane5[judge_note_lane5]) <= judge_time_PERFECT)
                {
                    //Explosion(1, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane5[judge_note_lane5], option);
                    }
                        
                    
                    //Debug.Log("PERFECT！！");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane5 = touch_time;
                        HoldTime_lane5 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane5[judge_Hold_lane5] - (touch_time - data_warehouse.note_timing_lane5[judge_note_lane5]));
                        HoldTime_lane5 = HoldTime_lane5 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane5);
                        Hold_Note_player = Create_Notes_lane5[judge_note_lane5].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane5);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane5 = true;
                    }
                    if ((data_warehouse.note_timing_lane5.Count - 1) > judge_note_lane5)
                    {
                        judge_note_lane5++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane5[judge_note_lane5]) <= judge_time_GREAT)
                {
                    //Explosion(2, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane5[judge_note_lane5], option);
                    }
                        
                    
                    //Debug.Log("GREAT！");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane5 = touch_time;
                        HoldTime_lane5 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane5[judge_Hold_lane5] - (touch_time - data_warehouse.note_timing_lane5[judge_note_lane5]));
                        HoldTime_lane5 = HoldTime_lane5 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane5);
                        Hold_Note_player = Create_Notes_lane5[judge_note_lane5].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane5);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane5 = true;
                    }
                    if ((data_warehouse.note_timing_lane5.Count - 1) > judge_note_lane5)
                    {
                        judge_note_lane5++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane1[judge_note_lane5]) <= judge_time_GOOD)
                {
                    //Explosion(3, lane);//判定文字
                    if (option != 3)
                    {
                        N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane5[judge_note_lane5], option);
                    }
                        
                    
                    //Debug.Log("GOOD");
                    if (option == 1)
                    {
                        atomSource_se.Play("clap");
                    }
                    else if (option == 2)
                    {
                        atomSource_se.Play("slash");
                    }
                    else if (option == 3)
                    {
                        atomSource_se.Play("clap");
                        Hold_start_lane5 = touch_time;
                        HoldTime_lane5 = Mathf.Abs(data_warehouse.Holdnote_seconds_lane5[judge_Hold_lane5] - (touch_time - data_warehouse.note_timing_lane5[judge_note_lane5]));
                        HoldTime_lane5 = HoldTime_lane5 / 1000;
                        Debug.Log("HoldTime " + HoldTime_lane5);
                        Hold_Note_player = Create_Notes_lane5[judge_note_lane5].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                        Hold_Note_player.Shorten(HoldTime_lane5);
                        //ホールドするべき秒数ーPERFECT時間からのズレ
                        Hold_switch_lane5 = true;
                    }
                    if ((data_warehouse.note_timing_lane5.Count - 1) > judge_note_lane5)
                    {
                        judge_note_lane5++;
                    }

                }
                else if (Mathf.Abs(touch_time - data_warehouse.note_timing_lane1[judge_note_lane5]) <= judge_time_BAD)
                {
                    //Explosion(3);//判定文字
                    atomSource_se.Play("miss");
                    if (option == 3)
                    {
                        Hold_Note_player = Create_Notes_lane5[judge_note_lane5].GetComponent<Hold_Note_player>();
                        Hold_Note_player.Moven.Kill();
                    }
                    N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane5[judge_note_lane5], option);
                    
                    //Debug.Log("GOOD");
                    
                    if ((data_warehouse.note_timing_lane5.Count - 1) > judge_note_lane5)
                    {
                        judge_note_lane5++;
                    }
                }
                break;

        }
        StartCoroutine(test());


    }




    void Hold_touch_judge()
    {
        if (Hold_switch_lane1 == true)//ホールド判定中なら
        {
            int Holding_time = 0;//現在までにホールドしている時間

            Holding_time = time_count - Hold_start_lane1;
            //Holding_time = time_count - (int)(HoldTime_lane1*1000);
            //Debug.Log("Holding_time " + Holding_time);

            if (Input.touches[Hold_ID_lane1].phase == TouchPhase.Ended)//指が離れた
            {
                //ホールドすべき秒数-指を離した時点の秒数
                if ( (data_warehouse.Holdnote_seconds_lane1[judge_Hold_lane1] - Holding_time ) <= judge_time_PERFECT)//PERFECT以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane1 = false;
                    
                }
                else if ((data_warehouse.Holdnote_seconds_lane1[judge_Hold_lane1] - Holding_time) <= judge_time_GREAT)//GREAT以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane1 = false;
                    
                }
                else if ((data_warehouse.Holdnote_seconds_lane1[judge_Hold_lane1] - Holding_time) <= judge_time_GOOD)//GOOD以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane1 = false;
                    
                }
                else if((data_warehouse.Holdnote_seconds_lane1[judge_Hold_lane1] - Holding_time) <= judge_time_BAD)//GOOD以内だったら
                {
                    atomSource_se.Play("miss");
                    Hold_switch_lane1 = false;
                    
                }
                else
                {
                    atomSource_se.Play("miss");
                    Hold_switch_lane1 = false;
                    
                }
                N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane1[judge_Hold_lane1], 3);
                if ((data_warehouse.note_timing_lane1.Count - 1) > judge_Hold_lane1)
                {
                    judge_Hold_lane1++;
                }
                debug.GetComponent<Text>().text = "Leaved! ";
            }
            if (Holding_time >= data_warehouse.Holdnote_seconds_lane1[judge_Hold_lane1])//ホールドすべき秒数を超えたら
            {
                atomSource_se.Play("clap");
                N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane1[judge_Hold_lane1], 3);
                Hold_switch_lane1 = false;
                if ((data_warehouse.note_timing_lane1.Count - 1) > judge_Hold_lane1)
                {
                    judge_Hold_lane1++;
                }
            }
        }

        if (Hold_switch_lane2 == true)//ホールド判定中なら
        {
            int Holding_time = 0;//現在までにホールドしている時間

            Holding_time = time_count - Hold_start_lane2;
            //Holding_time = time_count - (int)(HoldTime_lane2*1000);
            //Debug.Log("Holding_time " + Holding_time);

            if (Input.touches[Hold_ID_lane2].phase == TouchPhase.Ended)//指が離れた
            {
                //ホールドすべき秒数-指を離した時点の秒数
                if ((data_warehouse.Holdnote_seconds_lane2[judge_Hold_lane2] - Holding_time) <= judge_time_PERFECT)//PERFECT以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane2 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane2[judge_Hold_lane2] - Holding_time) <= judge_time_GREAT)//GREAT以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane2 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane2[judge_Hold_lane2] - Holding_time) <= judge_time_GOOD)//GOOD以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane2 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane2[judge_Hold_lane2] - Holding_time) <= judge_time_BAD)//GOOD以内だったら
                {
                    atomSource_se.Play("miss");
                    Hold_switch_lane2 = false;

                }
                else
                {
                    atomSource_se.Play("miss");
                    Hold_switch_lane2 = false;

                }
                N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane2[judge_Hold_lane2], 3);
                if ((data_warehouse.note_timing_lane2.Count - 1) > judge_Hold_lane2)
                {
                    judge_Hold_lane2++;
                }
                debug.GetComponent<Text>().text = "Leaved! ";
            }
            if (Holding_time >= data_warehouse.Holdnote_seconds_lane2[judge_Hold_lane2])//ホールドすべき秒数を超えたら
            {
                atomSource_se.Play("clap");
                N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane2[judge_Hold_lane2], 3);
                Hold_switch_lane2 = false;
                if ((data_warehouse.note_timing_lane2.Count - 1) > judge_Hold_lane2)
                {
                    judge_Hold_lane2++;
                }
            }
        }

        if (Hold_switch_lane3 == true)//ホールド判定中なら
        {
            int Holding_time = 0;//現在までにホールドしている時間

            Holding_time = time_count - Hold_start_lane3;
            //Holding_time = time_count - (int)(HoldTime_lane3*1000);
            //Debug.Log("Holding_time " + Holding_time);

            if (Input.touches[Hold_ID_lane3].phase == TouchPhase.Ended)//指が離れた
            {
                //ホールドすべき秒数-指を離した時点の秒数
                if ((data_warehouse.Holdnote_seconds_lane3[judge_Hold_lane3] - Holding_time) <= judge_time_PERFECT)//PERFECT以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane3 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane3[judge_Hold_lane3] - Holding_time) <= judge_time_GREAT)//GREAT以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane3 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane3[judge_Hold_lane3] - Holding_time) <= judge_time_GOOD)//GOOD以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane3 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane3[judge_Hold_lane3] - Holding_time) <= judge_time_BAD)//GOOD以内だったら
                {
                    atomSource_se.Play("miss");
                    Hold_switch_lane3 = false;

                }
                else
                {
                    atomSource_se.Play("miss");
                    Hold_switch_lane3 = false;

                }
                N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane3[judge_Hold_lane3], 3);
                if ((data_warehouse.note_timing_lane3.Count - 1) > judge_Hold_lane3)
                {
                    judge_Hold_lane3++;
                }
                debug.GetComponent<Text>().text = "Leaved! ";
            }
            if (Holding_time >= data_warehouse.Holdnote_seconds_lane3[judge_Hold_lane3])//ホールドすべき秒数を超えたら
            {
                atomSource_se.Play("clap");
                N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane3[judge_Hold_lane3], 3);
                Hold_switch_lane3 = false;
                if ((data_warehouse.note_timing_lane3.Count - 1) > judge_Hold_lane3)
                {
                    judge_Hold_lane3++;
                }
            }
        }



        if (Hold_switch_lane4 == true)//ホールド判定中なら
        {
            int Holding_time = 0;//現在までにホールドしている時間

            Holding_time = time_count - Hold_start_lane4;
            //Holding_time = time_count - (int)(HoldTime_lane4*1000);
            //Debug.Log("Holding_time " + Holding_time);

            if (Input.touches[Hold_ID_lane4].phase == TouchPhase.Ended)//指が離れた
            {
                //ホールドすべき秒数-指を離した時点の秒数
                if ((data_warehouse.Holdnote_seconds_lane4[judge_Hold_lane4] - Holding_time) <= judge_time_PERFECT)//PERFECT以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane4 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane4[judge_Hold_lane4] - Holding_time) <= judge_time_GREAT)//GREAT以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane4 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane4[judge_Hold_lane4] - Holding_time) <= judge_time_GOOD)//GOOD以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane4 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane4[judge_Hold_lane4] - Holding_time) <= judge_time_BAD)//GOOD以内だったら
                {
                    atomSource_se.Play("miss");
                    Hold_switch_lane4 = false;

                }
                else
                {
                    atomSource_se.Play("miss");
                    Hold_switch_lane4 = false;

                }
                N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane4[judge_Hold_lane4], 3);
                if ((data_warehouse.note_timing_lane4.Count - 1) > judge_Hold_lane4)
                {
                    judge_Hold_lane4++;
                }
                debug.GetComponent<Text>().text = "Leaved! ";
            }
            if (Holding_time >= data_warehouse.Holdnote_seconds_lane4[judge_Hold_lane4])//ホールドすべき秒数を超えたら
            {
                atomSource_se.Play("clap");
                N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane4[judge_Hold_lane4], 3);
                Hold_switch_lane4 = false;
                if ((data_warehouse.note_timing_lane4.Count - 1) > judge_Hold_lane4)
                {
                    judge_Hold_lane4++;
                }
            }
        }

        if (Hold_switch_lane5 == true)//ホールド判定中なら
        {
            int Holding_time = 0;//現在までにホールドしている時間

            Holding_time = time_count - Hold_start_lane5;
            //Holding_time = time_count - (int)(HoldTime_lane5*1000);
            //Debug.Log("Holding_time " + Holding_time);

            if (Input.touches[Hold_ID_lane5].phase == TouchPhase.Ended)//指が離れた
            {
                //ホールドすべき秒数-指を離した時点の秒数
                if ((data_warehouse.Holdnote_seconds_lane5[judge_Hold_lane5] - Holding_time) <= judge_time_PERFECT)//PERFECT以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane5 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane5[judge_Hold_lane5] - Holding_time) <= judge_time_GREAT)//GREAT以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane5 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane5[judge_Hold_lane5] - Holding_time) <= judge_time_GOOD)//GOOD以内だったら
                {
                    atomSource_se.Play("clap");
                    Hold_switch_lane5 = false;

                }
                else if ((data_warehouse.Holdnote_seconds_lane5[judge_Hold_lane5] - Holding_time) <= judge_time_BAD)//GOOD以内だったら
                {
                    atomSource_se.Play("miss");
                    Hold_switch_lane5 = false;

                }
                else
                {
                    atomSource_se.Play("miss");
                    Hold_switch_lane5 = false;

                }
                N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane5[judge_Hold_lane5], 3);
                if ((data_warehouse.note_timing_lane5.Count - 1) > judge_Hold_lane5)
                {
                    judge_Hold_lane5++;
                }
                debug.GetComponent<Text>().text = "Leaved! ";
            }
            if (Holding_time >= data_warehouse.Holdnote_seconds_lane5[judge_Hold_lane5])//ホールドすべき秒数を超えたら
            {
                atomSource_se.Play("clap");
                N_Note_ObjectPool.instance.releaseNote(Create_Notes_lane5[judge_Hold_lane5], 3);
                Hold_switch_lane5 = false;
                if ((data_warehouse.note_timing_lane5.Count - 1) > judge_Hold_lane5)
                {
                    judge_Hold_lane5++;
                }
            }
        }



























        else
        {
            debug.GetComponent<Text>().text = "not judge ";//後で消す
        }
    }
    


    public void Explosion(int rank ,int lane)//判定文字生成。後でオブジェクトプールに
    {
        GameObject judge_string;//どの判定文字を再生するか
        Vector3 app_point;//判定文字出現位置
        switch (rank)
        {
            case 1:
                judge_string = PERFECT;
                break;
            case 2:
                judge_string = GREAT;
                break;
            case 3:
                judge_string = GOOD;
                break;
            default://なければいけないが呼ばないように
                judge_string = PERFECT;
                break;
        }
        switch (lane)
        {

            case 0:
                app_point = new Vector3(-7.45f,-3f);
                break;
            case 1:
                app_point = new Vector3(-3.70f, -3f);
                break;
            case 2:
                app_point = new Vector3(0f, -3f);
                break;
            case 3:
                app_point = new Vector3(3.75f, -3f);
                break;
            case 4:
                app_point = new Vector3(7.47f, -3f);
                break;
            default://なければいけないが呼ばないように
                app_point = new Vector3(-7.45f, -3f);
                break;
        }


        //Instantiate(judge_string, new Vector3(-7.44f, -4.39f), transform.rotation);
        Instantiate(judge_string, app_point, transform.rotation);
    }


  


    void OnGUI()//タッチ座標の表示
    {
        
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {

                 GUI.Label(new Rect(350, 50 , 500, 200), "タッチ数 " + Input.touchCount);
                  for (int i = 0; i < Input.touchCount; i++)//今タッチされてる指の数だけ iは指の数
                 {
                        GUI.Label(new Rect(350, 10+(i*10), 500, 200), "タッチ"+ i + " " + Camera.main.ScreenToWorldPoint(Input.touches[i].position));
                    
                 }
                GUI.Label(new Rect(500, 20, 500, 200), "タッチレーン " + touch1);
                GUI.Label(new Rect(500, 30, 500, 200), "タッチレーン " + touch2);
                GUI.Label(new Rect(500, 40, 500, 200), "タッチレーン " + touch3);
                GUI.Label(new Rect(500, 50, 500, 200), "タッチレーン " + touch4);
                GUI.Label(new Rect(500, 60, 500, 200), "タッチレーン " + touch5);




            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                GUI.Label(new Rect(500, 10, 500, 200), "m_TouchPos1" + m_TouchPos1);
            }
            GUI.Label(new Rect(500, 20, 500, 200), "タッチレーン " + touch1);
            GUI.Label(new Rect(500, 30, 500, 200), "タッチレーン " + touch2);
            GUI.Label(new Rect(500, 40, 500, 200), "タッチレーン " + touch3);
            GUI.Label(new Rect(500, 50, 500, 200), "タッチレーン " + touch4);
            GUI.Label(new Rect(500, 60, 500, 200), "タッチレーン " + touch5);






    }


    IEnumerator test()//タッチレーン表示空白化用。後で消す。
    {
        //Debug.Log("コルーチン");
        yield return new WaitForSeconds(0.35f);
        touch1 = "";
        touch2 = "";
        touch3 = "";
        touch4 = "";
        touch5 = "";

    }
}

