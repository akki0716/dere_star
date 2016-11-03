using UnityEngine;
using System.Collections.Generic;

public class data_warehouse : MonoBehaviour {


   


    public List<int> note_timing_lane1 = new List<int>();//11チャンネルのノートタイミングのリスト
    public List<int> note_timing_lane2 = new List<int>();//12チャンネルのノートタイミングのリスト
    public List<int> note_timing_lane3 = new List<int>();//13チャンネルのノートタイミングのリスト
    public List<int> note_timing_lane4 = new List<int>();//14チャンネルのノートタイミングのリスト
    public List<int> note_timing_lane5 = new List<int>();//15チャンネルのノートタイミングのリスト

    public List<int> note_option_lane1 = new List<int>();//レーン1のノートオプションのリスト
    public List<int> note_option_lane2 = new List<int>();//レーン2のノートオプションのリスト
    public List<int> note_option_lane3 = new List<int>();//レーン3のノートオプションのリスト
    public List<int> note_option_lane4 = new List<int>();//レーン4のノートオプションのリスト
    public List<int> note_option_lane5 = new List<int>();//レーン5のノートオプションのリスト



    public List<int> Holdnote_seconds_lane1 = new List<int>();//レーン1のホールドノートの秒数のリスト
    public List<int> Holdnote_seconds_lane2 = new List<int>();//レーン2のホールドノートの秒数のリスト
    public List<int> Holdnote_seconds_lane3 = new List<int>();//レーン3のホールドノートの秒数のリスト
    public List<int> Holdnote_seconds_lane4 = new List<int>();//レーン4のホールドノートの秒数のリスト
    public List<int> Holdnote_seconds_lane5 = new List<int>();//レーン5のホールドノートの秒数のリスト


    public List<int> BPM_change_time = new List<int>();//bpmが変動する実時間
    public List<double> BPM_list = new List<double>();//bpm変動のリスト
    /*
    public List<float> Note_Start_point1_x = new List<float>();
    public List<float> Note_Start_point1_y = new List<float>();

    public List<float> Note_Start_point2_x = new List<float>();
    public List<float> Note_Start_point2_y = new List<float>();

    public List<float> Note_Start_point3_x = new List<float>();
    public List<float> Note_Start_point3_y = new List<float>();

    public List<float> Note_Start_point4_x = new List<float>();
    public List<float> Note_Start_point4_y = new List<float>();

    public List<float> Note_end_point_x = new List<float>();
    public List<float> Note_end_point_y = new List<float>();

    public List<int> A_frick_note_way = new List<int>();

    public int Now_note_number = 0;
    */

    void Start () {
        log();


    }


    void Update () {
	
	}

    void log()
    {
        foreach (int b in note_timing_lane1) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("---------------timing (レーン1)----------------------- " + b);
        }
        foreach (int b in note_timing_lane2) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("----------------timing (配列12ch)----------------------- " + b);
        }
        foreach (int b in note_timing_lane3) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("-----------------timing (配列13ch)---------------- " + b);
        }
        foreach (int b in note_timing_lane4) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("------------------timing (配列14ch)----------------- " + b);
        }
        foreach (int b in note_timing_lane5) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("-----------timing (配列15ch)---------------- " + b);
        }

        foreach (int b in note_option_lane1) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("---------------option (レーン1)----------------------- " + b);
        }
        foreach (int b in note_option_lane2) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("---------------option (レーン2)----------------------- " + b);
        }
        foreach (int b in note_option_lane3) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("---------------option (レーン3)----------------------- " + b);
        }
        foreach (int b in note_option_lane4) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("---------------option (レーン4)----------------------- " + b);
        }
        foreach (int b in note_option_lane5) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("---------------option (レーン5)----------------------- " + b);
        }



        foreach (int b in Holdnote_seconds_lane1) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("---------------ホールド秒数 (レーン1)----------------------- " + b);
        }
        foreach (int b in Holdnote_seconds_lane2) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("---------------ホールド秒数 (レーン2)----------------------- " + b);
        }
        foreach (int b in Holdnote_seconds_lane3) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("---------------ホールド秒数 (レーン3)----------------------- " + b);
        }
        foreach (int b in Holdnote_seconds_lane4) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("---------------ホールド秒数 (レーン4)----------------------- " + b);
        }
        foreach (int b in Holdnote_seconds_lane5) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("---------------ホールド秒数 (レーン5)----------------------- " + b);
        }



        /*
        foreach (int b in Note_Start_point1_x) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("--------------------------格納されたNote_Start_point1_x--------------------------------- " + b);
        }
        foreach (int b in Note_Start_point1_y) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("--------------------------格納されたNote_Start_point1_y--------------------------------- " + b);
        }

        foreach (int b in Note_Start_point2_x) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("--------------------------格納されたNote_Start_point2_x--------------------------------- " + b);
        }
        foreach (int b in Note_Start_point2_y) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("--------------------------格納されたNote_Start_point2_y--------------------------------- " + b);
        }


        foreach (int b in Note_Start_point3_x) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("--------------------------格納されたNote_Start_point3_x--------------------------------- " + b);
        }
        foreach (int b in Note_Start_point3_y) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("--------------------------格納されたNote_Start_point3_y--------------------------------- " + b);
        }

        foreach (int b in Note_Start_point4_x) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("--------------------------格納されたNote_Start_point4_x--------------------------------- " + b);
        }
        foreach (int b in Note_Start_point4_y) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("--------------------------格納されたNote_Start_point4_y--------------------------------- " + b);
        }


        foreach (int b in Note_end_point_x) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("--------------------------格納されたNote_end_point_x--------------------------------- " + b);
        }
        foreach (int b in Note_end_point_y) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("--------------------------格納されたNote_end_point_y--------------------------------- " + b);
        }

        foreach (int b in A_frick_note_way) //現在の行のすべてのノーツのタイミングを出す
        {
            Debug.Log("--------------------------格納されたA_frick_note_way--------------------------------- " + b);
        }
        */

    }



}
