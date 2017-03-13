using System;
using System.IO;
using System.Text;
using UnityEngine;



/// <summary>
/// BGM再生に関連する処理
/// </summary>
public class BGM_manager : MonoBehaviour
{

    /// <summary>
    /// BGM再生時間を変動させるために読み込んだ設定テキストの「行」
    /// </summary>
    string[] settinglines;

    /// <summary>
    /// BGMを再生開始する時間
    /// </summary>
    public int BGM_start_time = 0;

    [SerializeField]
    data_warehouse data_warehouse;

    /// <summary>
    /// 一番最初に流れてくるノートのタイミング
    /// </summary>
    int note_time_minimum;

    /// <summary>
    /// カウントを始める時間
    /// </summary>
    int count_start_time;


    /// <summary>
    /// count_start_timeに更に付ける時間
    /// </summary>
    int extra_time = 150;//50=50×10ミリ秒＝500ミリ秒＝0.5秒
    

    /// <summary>
    /// BGM
    /// </summary>
    AudioSource BGM;
        
        
        


    //-------------メソッド----------------------


    /// <summary>
    /// Android遅延対策のBGM遅延秒数を設定ファイルから取ってくる
    /// </summary>
    /// <returns></returns>
    public int BGM_delay()
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
                    //sdcard/Android/data/com.sato.derester/files/ 以下に置く(songsと同列)
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
            sttingcontent += "設定ファイルが読めませんでした。っh";
            Debug.Log("エラー ");
        }


        foreach (var adb in settinglines)//ファイル内部のすべての行をループ
        {

            if (adb.Substring(0, 4) == "#Del")
            {
                BGM_start_time = int.Parse(adb.Substring(10));
                //adbd.GetComponent<Text>().text = "BGM " + BGM_start_time;
                /*ここたぶんいらない
                if (BGM_start_time == 0)
                {
                    BGM_start_time = 100;
                }
                */
            }

        }

        return BGM_start_time;

    }

   


    /// <summary>
    /// カウントを始める時間を算出する
    /// </summary>
    public int start_decide(int steam_time)//カウント開始時間を決定するメソッド
    {
        // note_time_minimum = 100000;
        /*
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
         */
       // note_time_minimum = data_warehouse.note_property[0].timing;
        //Debug.Log("最小時間 " + note_time_minimum);
        //Debug.Log("steam_time" + steam_time);
        //Debug.Log("extra_time" + extra_time);
        count_start_time = note_time_minimum - (steam_time + extra_time );
        //Debug.Log("note_time_minimum - (steam_time" +( note_time_minimum - steam_time));
        //Debug.Log("count_start_time" + count_start_time);

        //time_count = -1000;
        //Debug.Log("カウント開始時間 " + time_count);
        return count_start_time;
    }




    /// <summary>
    /// オーディオソースコンポーネントを変数BGMにセットする
    /// </summary>
    public void BGM_set()
    {//startを書きたくなかったのでこうなっている
        BGM = this.gameObject.GetComponent<AudioSource>();
    }




    /// <summary>
    /// BGM再生
    /// </summary>
    public void BGM_play()
    {
        BGM.Play();
    }


}
