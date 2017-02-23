using UnityEngine;
using System.Collections.Generic;

public class data_warehouse : MonoBehaviour {




    public Note_struct[] note_property;
    public BPM_struct[] bpm_property;
    //public float start_bpm;
    public bool[] note_checker;



    void Start () {
        //log();

        
	}


  


    /// <summary>
    /// 使わないので後で消す
    /// </summary>
	void log()
	{

        foreach (var item in note_property)
        {
            Debug.Log(item);
        }
	}



    /// <summary>
    /// ノート情報の構造体、レーン、タイミング、種類、ホールド時間
    /// </summary>
    public struct Note_struct
    {
        public int lane, timing, type , hold_time;

        public Note_struct(int la, int ti , int ty , int h_t)
        {
            lane = la;
            timing = ti;
            type = ty;
            hold_time = h_t;
        }
    }


    /// <summary>
    /// bpm変更の構造体
    /// </summary>
    public struct BPM_struct
    {
        public float bpm;
        public int timing;

        public BPM_struct(float bp, int ti)
        {
            bpm = bp;
            timing = ti;
            
        }
    }



    /// <summary>
    /// ノート構造体の配列をscore_loadから作らせる
    /// </summary>
    /// <param name="size"></param>
    public void note_array_create(int size)
    {
        note_property = new Note_struct[size];
        
    }


    /// <summary>
    /// bpm構造体のの配列をscore_loadから作らせる
    /// </summary>
    /// <param name="size"></param>
    public void bpm_array_create(int size)
    {
        bpm_property = new BPM_struct[size];

    }


    /// <summary>
    /// ノートの判定可否を保存する配列を作る
    /// </summary>
    /// <param name="size"></param>
    public void check_create()
    {
        note_checker = new bool[note_property.Length];

    }
}
