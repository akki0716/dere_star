using UnityEngine;
using System.Collections.Generic;

public class data_warehouse : MonoBehaviour {




    public lane1[] lane1_notes;
    public lane2[] lane2_notes;
    public lane3[] lane3_notes;
    public lane4[] lane4_notes;
    public lane5[] lane5_notes;
    public BPM_struct[] bpm_property;
    public bool[] note_checker;

    public GameObject[] lane1_Makes;
    public GameObject[] lane2_Makes;
    public GameObject[] lane3_Makes;
    public GameObject[] lane4_Makes;
    public GameObject[] lane5_Makes;


    /*
    void Start () {
        log();

        
	}


  


    /// <summary>
    /// 使わないので後で消す
    /// </summary>
	void log()
	{
        
        foreach (var item in lane1_notes)
        {
            Debug.Log(item);
        }
        
	}
    */


    /// <summary>
    /// レーン1のノート情報の構造体。タイミング、種類、ホールド時間
    /// </summary>
    public struct lane1
    {
        public int timing, type, hold_time;
        public bool alive;

        public lane1(int ti , int ty , int h_t ,bool  ali)
        {
            timing = ti;
            type = ty;
            hold_time = h_t;
            alive = ali;
        }
    }


    /// <summary>
    /// レーン2のノート情報の構造体。タイミング、種類、ホールド時間
    /// </summary>
    public struct lane2
    {
        public int timing, type, hold_time;
        public bool alive;

        public lane2(int ti, int ty, int h_t, bool ali)
        {
            timing = ti;
            type = ty;
            hold_time = h_t;
            alive = ali;
        }
    }



    /// <summary>
    /// レーン3のノート情報の構造体。タイミング、種類、ホールド時間
    /// </summary>
    public struct lane3
    {
        public int timing, type, hold_time;
        public bool alive;

        public lane3(int ti, int ty, int h_t, bool ali)
        {
            timing = ti;
            type = ty;
            hold_time = h_t;
            alive = ali;
        }
    }



    /// <summary>
    /// レーン4のノート情報の構造体。タイミング、種類、ホールド時間
    /// </summary>
    public struct lane4
    {
        public int timing, type, hold_time;
        public bool alive;

        public lane4(int ti, int ty, int h_t, bool ali)
        {
            timing = ti;
            type = ty;
            hold_time = h_t;
            alive = ali;
        }
    }



    /// <summary>
    /// レーン5のノート情報の構造体。タイミング、種類、ホールド時間
    /// </summary>
    public struct lane5
    {
        public int timing, type, hold_time;
        public bool alive;

        public lane5(int ti, int ty, int h_t, bool ali)
        {
            timing = ti;
            type = ty;
            hold_time = h_t;
            alive = ali;
        }
    }



    /// <summary>
    /// bpm変更の構造体
    /// </summary>
    public struct BPM_struct
    {
        public double bpm;
        public int timing;

        public BPM_struct(double bp, int ti)
        {
            bpm = bp;
            timing = ti;
            
        }
    }



    /// <summary>
    /// ノート構造体、作成したノートのリストをの配列をscore_loadから作らせる
    /// </summary>
    /// <param name="size"></param>
    public void note_array_create(int lane1,int lane2 ,int lane3, int lane4 ,int lane5)
    {
        lane1_notes = new lane1[lane1];
        lane2_notes = new lane2[lane2];
        lane3_notes = new lane3[lane3];
        lane4_notes = new lane4[lane4];
        lane5_notes = new lane5[lane5];

        lane1_Makes = new GameObject[lane1];
        lane2_Makes = new GameObject[lane2];
        lane3_Makes = new GameObject[lane3];
        lane4_Makes = new GameObject[lane4];
        lane5_Makes = new GameObject[lane5];


        Debug.Log("");
    }


    /// <summary>
    /// bpm構造体のの配列をscore_loadから作らせる
    /// </summary>
    /// <param name="size"></param>
    public void bpm_array_create(int size)
    {
        bpm_property = new BPM_struct[size];

    }

    /*
    /// <summary>
    /// ノートの判定可否を保存する配列を作る
    /// </summary>
    /// <param name="size"></param>
    public void check_create()
    {
        note_checker = new bool[note_property.Length];

    }
    */
}
