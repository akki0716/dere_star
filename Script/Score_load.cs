using UnityEngine;
using System.Collections;
using System.IO;
using System; //Exception
using System.Text; //Encoding
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine.UI;//テキスト表示用。後で消す

/*

    問題点


*/

public class Score_load : MonoBehaviour {



    //この範囲はReadFile関連
    string filepath = "";

    //ヘッダー解釈用
    string bms = "";
    string[] lines;
    string temp_line;
    //bms情報
    string TITLE;
    string SUBTITLE;
    string ARTIST;
    double BPM;
    float PLAYLEVEL;
    int DIFFICULTY;
    string sound_source;
    List<double> header_bpm_list = new List<double>();//ヘッダーで指定されたbpmのリスト
    int max_measure = 0;



    //---------譜面解釈用-----------

    double Now_BPM;
    string obj_vol;
    double offset_time = 0; //bgm再生開始から1小節の開始までの時間
    double now_measure_time; //現在の1小節の時間
    double per_measure = 0; //1小節を1とした時の小節位置(≒パーセント)
    List<string> now_measure = new List<string>();//現在見ている行のタイミング部分を配列化したもの。現在見ている行が変わるごとにクリアが必要。
    List<double> bpm_change_point = new List<double>();//bpmが変動しているポイントを1小節を1とした割合で保存するリスト。　探索する小節が変わるごとにクリアが必要。
    List<double> measure_time_parts = new List<double>();//小節内でbpmが変動していた時の各区間の長さのリスト　探索する小節が変わるごとにクリアが必要。
    double header_bpm = 0;//ヘッダーで指定されたbpm
    List<double> measure_time_list = new List<double>();//小節の長さのリスト。 クリアは必要ない。
    int per_measure_count = -1;//小節内のbpm変動が何回目か(?)
    double note_part_pos; //ノーツの位置しているbpm区間の長さを1とした時ノーツのの位置
    double measure_x;//変拍子の時 小節は何倍か
    double this_note_timing = -1;//今のノートのタイミング

    bool Hold_searching_lane1 = false;//次のノートがホールド終点かの判断
    bool Hold_searching_lane2 = false;//次のノートがホールド終点かの判断
    bool Hold_searching_lane3 = false;//次のノートがホールド終点かの判断
    bool Hold_searching_lane4 = false;//次のノートがホールド終点かの判断
    bool Hold_searching_lane5 = false;//次のノートがホールド終点かの判断
    double Hold_time_parts_lane1 = 0;//ホールド時間のパーツ
    double Hold_time_parts_lane2 = 0;//ホールド時間のパーツ
    double Hold_time_parts_lane3 = 0;//ホールド時間のパーツ
    double Hold_time_parts_lane4 = 0;//ホールド時間のパーツ
    double Hold_time_parts_lane5 = 0;//ホールド時間のパーツ


    //-----データ格納用--------

    public data_warehouse data_warehouse;//data_warehouseスクリプト






    void Start () {
        filepath_decide();
        ReadFile();
    }
	

	void Update () {
	
	}

    

    void ReadFile()
    {

        // FileReadTest.txtファイルを読み込む
        FileInfo file = new FileInfo(filepath);
        try
        {
            // 一行毎読み込み
            using (StreamReader sr = new StreamReader(file.OpenRead(), Encoding.Default))
            {
                bms = sr.ReadToEnd();

                //Debug.Log("bms" + bms);
                lines = bms.Split('\n'); //linesに1行毎にツッコむ
                //Debug.Log("lines" + lines);
            }

        }
        catch (Exception e)
        {
            // 改行コード
            bms += SetDefaultText();
            Debug.Log("エラー ");
            
        }


        foreach (var i in lines)//ヘッダー解釈
        {
            temp_line = i;

            if (temp_line == "\r" || temp_line == "\n" || temp_line == "")//空の行
            {

            }
            else
            {
                temp_line = temp_line.Substring(0, 4);
            }
            //Debug.Log("temp_line" + temp_line);
            switch (temp_line) //ヘッダー解釈
            {
                //case "*--"://ヘッダーコメント
                //  break;
                case "#TIT"://タイトル
                    TITLE = i.Substring(6);
                    //Debug.Log("TITLE " + TITLE);
                    break;
                case "#SUB"://タイトル
                    SUBTITLE = i.Substring(9);
                    //Debug.Log("SUBTITLE " + SUBTITLE);
                    break;
                case "#ART":
                    ARTIST = i.Substring(7);
                    //Debug.Log("ARTIST " + ARTIST);
                    break;
                case "#BPM":
                    if (Regex.IsMatch(i.Substring(4, 1), @"\s"))//5-6文字目が空白(=ヘッダーの公称bpm)の場合
                    {
                        //Debug.Log("ヘッダーBPM ");
                        //Debug.Log(i.Substring(4, 1));
                        BPM = double.Parse(i.Substring(5));
                        Now_BPM = BPM;
                        data_warehouse.BPM_list.Add(Now_BPM);
                        //Debug.Log("BPM " + BPM);
                    }
                    if (Regex.IsMatch(i.Substring(4, 1), @"[0-9]$"))//5-6文字目が数字(=bpmリスト)の場合
                    {
                        if (Regex.IsMatch(i.Substring(6, 1), @"\s"))//7文字目が空白(=bpm変動99回目以下)の場合
                        {
                            //Debug.Log(i.Substring(7));
                            //Debug.Log("bpm変動99回目以下 ");
                            header_bpm_list.Add(double.Parse(i.Substring(7)));
                        }
                        else
                        {
                            //Debug.Log(i.Substring(8));
                            //Debug.Log("bpm変動100回目以上 ");
                            header_bpm_list.Add(double.Parse(i.Substring(8)));
                        }

                    }
                    break;
                case "#PLA":
                    if (i.Substring(0, 10) == "#PLAYLEVEL")
                    {
                        PLAYLEVEL = float.Parse(i.Substring(11));
                        //Debug.Log("PLAYLEVEL " + PLAYLEVEL);
                    }
                    break;
                case "#TOT":
                    DIFFICULTY = int.Parse(i.Substring(6));
                    //Debug.Log("DIFFICULTY " + DIFFICULTY);
                    break;
                case "#WAV":
                    if (i.Substring(0, 6) == "#WAV01")
                    {
                        sound_source = i.Substring(6);
                        //Debug.Log("sound_source " + sound_source);
                    }
                    break;
            }

            //最大の小節を数える
            if (temp_line.StartsWith("#"))//で始まる行の
            {
                if (Regex.IsMatch(i.Substring(1, 3), @"[0-9]$"))//2-4文字目が数字の場合
                {
                    if (int.Parse(i.Substring(1, 3)) > max_measure)//小節数の最大を超えたら
                    {
                        max_measure = int.Parse(i.Substring(1, 3));//小節数の最大を更新
                        //Debug.Log("max_measure " + max_measure);
                    }

                }
            }
            
        }//ヘッダー解釈foreach終了




        /*
        //000小節の長さ判定
        foreach (var bbb in lines)
        {
            if (bbb.StartsWith("#") && bbb.Substring(0, 6) == "#00002")
            {
                Debug.Log("000小節");
                Debug.Log(bbb.Substring(0, 6));
            }
        }//やっぱ使わない。めんどくささと割に合わない
        */

        for (int search_measure = 0; search_measure <= max_measure; search_measure++)
        //譜面解釈。ターゲットとなる小節を決めて1小説ごとに解釈
        {
            measure_x = 1;
            //Debug.Log(search_measure + "小節目");
            foreach (var adb in lines)//ファイル内部のすべての行をループ
            {
                if (adb.StartsWith("#") && Regex.IsMatch(adb.Substring(1, 3), @"[0-9]$") && int.Parse(adb.Substring(1, 3)) == search_measure)
                //#で始まる行の2-4文字目が数字の場合目的の小節に合致するなら
                {
                    if (measure_time_list.Count != search_measure)//ノーツもbpm変動も無い小節があった場合の補正
                    {
                        for (int i = measure_time_list.Count; i <= search_measure -1; i++)
                        {
                            //Debug.Log("for");
                            now_measure_time += (measure_time(Now_BPM, 1) * measure_x);
                            //measure_time_parts.Add(measure_time(Now_BPM, 1) * measure_x);
                            measure_time_list.Add(now_measure_time);
                            //Debug.Log("now_measure_time " + now_measure_time);
                        }
                    }


                    //小節の時間を出す
                    switch (int.Parse(adb.Substring(4, 2))) //チャンネル番号で条件分岐
                    {
                        case 1:
                            obj_vol = adb.Substring(7);  //チップ指定部分を切り出す
                            note_insert(obj_vol);//小節を入れた配列を作る

                            for (int i = 0; i < now_measure.Count; i++)
                            {
                                if (System.Text.RegularExpressions.Regex.IsMatch(now_measure[i], @"^[0-9a-zA-Z]+[1-9a-zA-Z]$") )
                                {
                                    now_measure_time = measure_time(Now_BPM, 1);//000小節の時間を出す
                                    per_measure = (double)(i) / now_measure.Count; //1小節を1とした時の小節位置(≒パーセント)
                                    offset_time = now_measure_time * (1 - per_measure);//1-per_measureでチップから001小節までの秒数が出る
                                    Debug.Log("offset_time " + offset_time);
                                    now_measure_time = offset_time;
                                    measure_time_list.Add(offset_time);

                                }
                            }
                            now_measure.Clear();
                            header_bpm = 0.1;
                            break;
                            
                        case 2: //変拍子
                            measure_x = double.Parse(adb.Substring(7));
                            //Debug.Log("measure_x  " + measure_x);
                            break;
                            
                        case 8://bpm変動
                            //bpmが変動していない場合の小節時間は後の部分が担当
                            obj_vol = adb.Substring(7);//チップ指定部分を取り出す

                            note_insert(obj_vol);//小節を入れた配列を作る


                            for (int i = 0; i < now_measure.Count; i++) //チップがある位置を特定するために配列の中身を見る
                            {
                                if (System.Text.RegularExpressions.Regex.IsMatch(now_measure[i], @"^[0-9a-zA-Z]+[1-9a-zA-Z]$") )
                                //1文字目が数字orA-Z、2文字目が0以外の数字orA-Zなら
                                {
                                    //Debug.Log("------------------------------------------------ ");
                                    //Debug.Log("now_measure.Count " + now_measure.Count);
                                    //Debug.Log("i " + i + " now_measure " + now_measure[i]);
                                    per_measure = (double)(i)/ now_measure.Count; //1小節を1とした時の小節位置(≒パーセント)
                                    Debug.Log("per_measure " + per_measure);
                                    bpm_change_point.Add(per_measure);//小節位置を保存しておく

                                    per_measure_count++;
                                    //Debug.Log("per_measure_count " + per_measure_count);


                                    if (per_measure_count != 0)//小節内で2度以上のBPM変動の場合
                                    {
                                        now_measure_time += ( measure_time(Now_BPM, (per_measure - (bpm_change_point[per_measure_count - 1]))) )* measure_x;
                                        //(measure_time(Now_BPM, (per_measure - (bpm_change_point[per_measure_count - 1]))) * measure_x)1
                                        //Debug.Log(Now_BPM);
                                        //Debug.Log("2回目以降 now_measure_time " + now_measure_time);
                                        measure_time_parts.Add( (measure_time(Now_BPM, (per_measure - (bpm_change_point[per_measure_count - 1]))) )* measure_x);

                                        data_warehouse.BPM_change_time.Add((int)(now_measure_time * 1000));//bpm変動の実秒数を保存
                                    }
                                    else//小節内最初のbpm変動の場合
                                    {
                                        now_measure_time +=( ( measure_time(Now_BPM, per_measure) )* measure_x); //measure_time(Now_BPM, per_measure)* measure_x  measure_time(Now_BPM, per_measure)
                                        measure_time_parts.Add( (measure_time(Now_BPM, per_measure)) * measure_x );
                                        Debug.Log("初回now_measure_time " + now_measure_time);
                                        
                                        data_warehouse.BPM_change_time.Add((int)(now_measure_time * 1000));//bpm変動の実秒数を保存
                                    }

                                    //Debug.Log("now_measure_time " + now_measure_time);
                                    //チップがある位置の配列の中身の値を16から10進数に変換、それ-1をbpmlistのインデックスとした。
                                    header_bpm = header_bpm_list[(Convert.ToInt32(now_measure[i], 16)) - 1];
                                    //Debug.Log("specified_bpm " + specified_bpm);
                                    header_bpm = Math.Floor(header_bpm * 1000) / 1000;
                                    //↑これで少数第4位の切り捨て
                                    //Debug.Log("specified_bpm " + specified_bpm);
                                    Now_BPM = header_bpm; //次回に利用するために保存
                                    data_warehouse.BPM_list.Add(Now_BPM);
                                }
                            }

                            if (header_bpm != 0) //小節内でbpm変動があったなら
                            {//最後のbpm変動を加算するための処理
                                now_measure_time += (measure_time(Now_BPM, (1 - per_measure)) * measure_x);
                                measure_time_parts.Add((measure_time(Now_BPM, (1 - per_measure)) * measure_x) );
                            }
                            
                            //Debug.Log("最終now_measure_time " + now_measure_time);

                            measure_time_list.Add(now_measure_time);
                            now_measure.Clear();//次は見る行が変わるのでクリアが必要
                            break;
                        default://bpm変動も変拍子も無い場合
                            now_measure_time += (measure_time(Now_BPM, 1) * measure_x);
                            //measure_time_parts.Add(measure_time(Now_BPM, 1) * measure_x);
                            measure_time_list.Add(now_measure_time);
                            //Debug.Log("now_measure_time " + now_measure_time);
                            break;
                    }
                  
                    //ノーツの時間を出す
                    switch (int.Parse(adb.Substring(4, 2)))
                    {
                        case 11://通常ノート
                        case 12://フリックノート
                        case 13://通常ノート
                        case 14://フリックノート
                        case 15://通常ノート
                        case 18://フリックノート
                        case 21://通常ノート
                        case 22://フリックノート
                        case 23://通常ノート
                        case 24://フリックノート
                        case 51://ロングノート
                        case 53:
                        case 55:
                        case 61:
                        case 63:
                            obj_vol = adb.Substring(7);//チップ指定部分を取り出す
                            note_insert(obj_vol);//小節を入れた配列を作る
                            for (int i = 0; i < now_measure.Count; i++)//配列の中を見る
                            {
                                if (System.Text.RegularExpressions.Regex.IsMatch(now_measure[i], @"^[0-9a-zA-Z]+[1-9a-zA-Z]$"))//チップなら
                                {
                                    per_measure = (double)(i) / now_measure.Count; //1小節を1とした時の小節位置(≒パーセント)
                                    //Debug.Log("ノーツper_measure " + per_measure);
                                    double stack = 0;//チップが位置する区間までの各bpm区間の長さを格納

                                    if (bpm_change_point.Count != 0)//bpm_change_pointの中身がある(=bpm変動が起こっていた)
                                    {
                                        for (int c = 0; c < bpm_change_point.Count; c++)//ノーツがどこの区間にあるか調べるためにbpm_change_pointの中身を見る
                                        {
                                            if(bpm_change_point[c] == per_measure)//見てる値とノーツの値が同じ(=bpm変動とノーツのタイミングが同じ)
                                            {
                                                note_part_pos = 0.0; //
                                                //Debug.Log("note_part_pos  " + note_part_pos);
                                                if (c == 0)//bpm変動が初回
                                                {
                                                    stack = measure_time_parts[c];
                                                }
                                                else//bpm変動が初回以外
                                                {
                                                    for (int d = 0; d <= c; d++)//チップが位置する区間までの各bpm区間の長さを出す
                                                    {
                                                        stack += measure_time_parts[d];
                                                        //Debug.Log("stack " + stack);
                                                    }
                                                }
                                                //Debug.Log("このチップが位置するbpm区間の長さa " + measure_time_parts[c+1]);
                                                this_note_timing = measure_time_list[search_measure - 1] + stack;
                                                if (int.Parse(adb.Substring(4, 2)) < 51)
                                                {
                                                    add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));
                                                }
                                                
                                                //直前の小節の秒+直前の区間までの秒数
                                                break;
                                            }
                                            else if(c == 0)//初回のbpm変動の場合
                                            {
                                                if (per_measure < bpm_change_point[c])//チップの位置＜初回のbpm変動の位置
                                                {
                                                    note_part_pos = per_measure / bpm_change_point[0];
                                                    //Debug.Log(" note_part_pos " + note_part_pos);

                                                    //Debug.Log("このチップが位置するbpm区間の長さb " + measure_time_parts[c]);
                                                    this_note_timing = measure_time_list[search_measure - 1] + (measure_time_parts[c] * note_part_pos);
                                                    //直前の小節の秒+(チップが位置する区間の長さ * チップの位置)
                                                    if (int.Parse(adb.Substring(4, 2)) < 51)
                                                    {
                                                        add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));
                                                    }
                                                    break;
                                                }

                                                else if(bpm_change_point.Count == 1)//小節内に1回のみのbpm変動の場合
                                                {
                                                    if (bpm_change_point[0] == 0)//bpm変動が小節の頭1回のみだったら
                                                    {
                                                        note_part_pos = per_measure;
                                                        //Debug.Log("note_part_pos " + note_part_pos);
                                                        
                                                        //Debug.Log("このチップが位置するbpm区間の長さc " + measure_time_parts[c+1]);
                                                        this_note_timing = measure_time_list[search_measure - 1] + (measure_time_parts[c + 1] * note_part_pos);
                                                        //直前の小節の秒+(チップが位置する区間の長さ * チップの位置)
                                                        if (int.Parse(adb.Substring(4, 2)) < 51)
                                                        {
                                                            add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));
                                                        }
                                                        break;
                                                    }

                                                    else//bpm変動が小節の頭以外での1回のみだったら
                                                    {
                                                        note_part_pos = (per_measure - bpm_change_point[c]) / (1 - bpm_change_point[c]);
                                                        // Debug.Log("note_part_pos " + note_part_pos);
                                                        //Debug.Log("このチップが位置するbpm区間の長さd " + measure_time_parts[c+1]);
                                                        this_note_timing = measure_time_list[search_measure - 1] + measure_time_parts[c] + (measure_time_parts[c + 1] * note_part_pos);
                                                        //直前の小節の秒+直前の区間の秒+(チップが位置する区間の長さ * チップの位置)
                                                        if (int.Parse(adb.Substring(4, 2)) < 51)
                                                        {
                                                            add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (bpm_change_point[c-1] < per_measure && per_measure < bpm_change_point[c] )//どこかのbpm区間の間にある
                                            {
                                                note_part_pos = (per_measure - bpm_change_point[c-1])/ (bpm_change_point[c] - bpm_change_point[c-1]);
                                                //Debug.Log("note_part_pos  " + note_part_pos);
                                                //Debug.Log("このチップが位置するbpm区間の長さe " + measure_time_parts[c]);
                                                for (int d = 0; d < c; d++)//チップが位置する区間までの各bpm区間の長さを出す
                                                {
                                                    stack += measure_time_parts[d];
                                                    //Debug.Log("stack " + stack);
                                                }
                                                this_note_timing = measure_time_list[search_measure - 1] + stack + (measure_time_parts[c] * note_part_pos);
                                                //直前の小節の秒+直前の区間までの秒+(チップが位置する区間の長さ * チップの位置)
                                                if (int.Parse(adb.Substring(4, 2)) < 51)
                                                {
                                                    add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));
                                                }
                                                break;
                                            }
                                            else if(c == (bpm_change_point.Count -1))//小節内最後のbpm変動の場合
                                            {
                                                //Debug.Log("最後" + c +" " +bpm_change_point[c]);
                                                note_part_pos = (per_measure - bpm_change_point[c]) / (1 - bpm_change_point[c]);
                                                //Debug.Log("<color=blue> note_part_pos </color> " + note_part_pos);
                                                //Debug.Log("このチップが位置するbpm区間の長さf " + measure_time_parts[c+1]);
                                                for (int d = 0; d <= c; d++)//チップが位置する区間までの各bpm区間の長さを出す
                                                {
                                                    stack += measure_time_parts[d];
                                                    //Debug.Log("stack " + stack);
                                                }
                                                this_note_timing = measure_time_list[search_measure - 1] + stack + (measure_time_parts[c+1] * note_part_pos);
                                                //直前の小節の秒+直前の区間までの秒+(チップが位置する区間の長さ * チップの位置)
                                                if (int.Parse(adb.Substring(4, 2)) < 51)
                                                {
                                                    add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));
                                                }
                                                break;
                                            }
                                            //Debug.Log("このチップが存在する小節の1つ前までの長さ " + measure_time_list[search_measure - 1]);
                                        }
                                    }
                                    
                                    else //bpm_change_pointの中身が無い(=bpm変動が起こっていないがチップは小節上にある)
                                    {
                                        this_note_timing = 
                                            measure_time_list[search_measure - 1]+((measure_time_list[search_measure] - measure_time_list[search_measure-1])* per_measure)  ;
                                        //直前の小節の秒+(チップが位置する区間の長さ * チップの位置)

                                        if (int.Parse(adb.Substring(4, 2)) < 51)
                                        {
                                            add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));
                                        }
                                    }
                                    Debug.Log("このチップが存在する秒数 " + this_note_timing);


                                    switch (int.Parse(adb.Substring(4, 2)))
                                    {
                                        case 51:
                                            if (Hold_searching_lane1 == false)//ホールドの最初の部分の場合
                                            {
                                                Hold_searching_lane1 = true;//最初であるフラグを立てる
                                                Hold_time_parts_lane1 = this_note_timing;
                                                add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));//開始時間をツッコむ
                                            }
                                            else if (Hold_searching_lane1 == true)
                                            {
                                                //ホールド時間をツッコむ
                                                data_warehouse.Holdnote_seconds_lane1.Add((int)( (this_note_timing - Hold_time_parts_lane1) * 1000));
                                                Debug.Log("レーン1 ホールド時間 " + (this_note_timing - Hold_time_parts_lane1));
                                                Hold_searching_lane1 = false;
                                                Hold_time_parts_lane1 = 0;
                                            }
                                            break;

                                        case 53:
                                            if (Hold_searching_lane2 == false)//ホールドの最初の部分の場合
                                            {
                                                Hold_searching_lane2 = true;//最初であるフラグを立てる
                                                Hold_time_parts_lane2 = this_note_timing;
                                                add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));//開始時間をツッコむ
                                            }
                                            else if (Hold_searching_lane2 == true)
                                            {
                                                Debug.Log("レーン2 ホールド時間 " + (this_note_timing - Hold_time_parts_lane2));
                                                data_warehouse.Holdnote_seconds_lane2.Add((int)((this_note_timing - Hold_time_parts_lane2) * 1000));
                                                Hold_searching_lane2 = false;
                                                Hold_time_parts_lane2 = 0;
                                            }
                                            break;
                                        case 55:
                                            if (Hold_searching_lane3 == false)//ホールドの最初の部分の場合
                                            {
                                                Hold_searching_lane3 = true;//最初であるフラグを立てる
                                                Hold_time_parts_lane3 = this_note_timing;
                                                add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));//開始時間をツッコむ
                                            }
                                            else if (Hold_searching_lane3 == true)
                                            {
                                                data_warehouse.Holdnote_seconds_lane3.Add((int)((this_note_timing - Hold_time_parts_lane3) * 1000));
                                                Debug.Log("レーン3 ホールド時間 " + (this_note_timing - Hold_time_parts_lane3));
                                                Hold_searching_lane3 = false;
                                                Hold_time_parts_lane3 = 0;
                                            }
                                            break;
                                        case 61:
                                            if (Hold_searching_lane4 == false)//ホールドの最初の部分の場合
                                            {
                                                Hold_searching_lane4 = true;//最初であるフラグを立てる
                                                Hold_time_parts_lane4 = this_note_timing;
                                                add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));//開始時間をツッコむ
                                            }
                                            else if (Hold_searching_lane4 == true)
                                            {
                                                data_warehouse.Holdnote_seconds_lane4.Add((int)((this_note_timing - Hold_time_parts_lane4) * 1000));
                                                Debug.Log("レーン4 ホールド時間 " + (this_note_timing - Hold_time_parts_lane4));
                                                Hold_searching_lane4 = false;
                                                Hold_time_parts_lane4 = 0;
                                            }
                                            break;
                                        case 63:
                                            if (Hold_searching_lane5 == false)//ホールドの最初の部分の場合
                                            {
                                                Hold_searching_lane5 = true;//最初であるフラグを立てる
                                                Hold_time_parts_lane5 = this_note_timing;
                                                add_note_timing(this_note_timing, int.Parse(adb.Substring(4, 2)));//開始時間をツッコむ
                                            }
                                            else if (Hold_searching_lane5 == true)
                                            {
                                                data_warehouse.Holdnote_seconds_lane5.Add((int)((this_note_timing - Hold_time_parts_lane5) * 1000));
                                                Debug.Log("レーン5 ホールド時間 " + (this_note_timing - Hold_time_parts_lane5));
                                                Hold_searching_lane5 = false;
                                                Hold_time_parts_lane5 = 0;
                                            }
                                            break;

                                    }
                                }
                            }
                            now_measure.Clear();//次は見る行が変わるのでクリアが必要
                            break;
                            
                    }

                   








                }



























            }
            
            header_bpm = 0;
            
            bpm_change_point.Clear();//小節ごとにリセットする必要がある
            this_note_timing = -1;//小節ごとにリセットする必要がある
            /*
            for (int i = 0; i < measure_time_parts.Count; i++)
            {
                //中身を覗くためなので処理には必要なし
                Debug.Log("1小説の各区間の長さ measure_time_parts[" + i + "] " + measure_time_parts[i]);
            }
            */




            measure_time_parts.Clear();//小節ごとにリセットする必要がある
        }


        /*

        for (int i = 0; i < measure_time_list.Count ; i ++)
        {
            
            Debug.Log("各小節までの長さ measure_time_list[" + i +"] " + measure_time_list[i]);
        }
        */














        last_process();


        

    }//readfile終了


    /*
    ここからbmsファイル読み取り用のメソッド
    */

    







    // 改行コード処理 エラー処理
    string SetDefaultText()
    {
        return "C#あ\n";
    }



    double measure_time(double N_BPM, double Measure_length)//1小節の時間を出す
    {
        double time;
        time = 60 * 4 * Measure_length / N_BPM; //4/4拍子でのMeasure_length小節時間
        //60*拍子*小節の長さ/テンポ
        return time;
    }


    void note_insert(string o_vol)//小節を入れた配列を作るメソッド
    {
        for (int i = 0; i < o_vol.Length - 1; i += 2) 
        {
            now_measure.Add(o_vol.Substring(i, 2));
            //Debug.Log("obj_vol.Substring(i, 2) " + obj_vol.Substring(i, 2));
        }
    }



    void add_note_timing(double T_N_timng,int lane)//ノートタイミングの収納
    {
        switch (lane)
        {
            /*
                        case 11://通常ノート     1
                        case 12://フリックノート

                        case 13://通常ノート     2
                        case 14://フリックノート

                        case 15://通常ノート     3
                        case 18://フリックノート

                        case 21://通常ノート     4
                        case 22://フリックノート

                        case 23://通常ノート     5
                        case 24://フリックノート

                        case 51://ロングノート
                        case 52:
                        case 53:
                        case 54:
                        case 55:

白鍵盤レーンの時は1を
青鍵盤レーンの時は2を
タイミング追加の時に入れる
ロングノートは始点のタイミングだけを追加してオプションは3
終点で時間を入れる


            オプションまでまとめてソートが出来ないので、時間+オプション数字で入れる
            タイミング*10+オプションで入れる
            で取り出すときは配列の中身%10であまり1桁を出す
    */
            //レーン1
            case 11:
                data_warehouse.note_timing_lane1.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane1.Add((int)(T_N_timng * 1000) * 10 + 1);
                break;
            case 12:
                data_warehouse.note_timing_lane1.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane1.Add((int)(T_N_timng * 1000) * 10 + 2);
                break;
            //レーン2
            case 13:
                data_warehouse.note_timing_lane2.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane2.Add((int)(T_N_timng * 1000) * 10 + 1);
                break;
            case 14:
                data_warehouse.note_timing_lane2.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane2.Add((int)(T_N_timng * 1000) * 10 + 2);
                break;
            //レーン3
            case 15:
                data_warehouse.note_timing_lane3.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane3.Add((int)(T_N_timng * 1000) * 10 + 1);
                break;
            case 18:
                data_warehouse.note_timing_lane3.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane3.Add((int)(T_N_timng * 1000) * 10 + 2);
                break;
            //レーン4
            case 21:
                data_warehouse.note_timing_lane4.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane4.Add((int)(T_N_timng * 1000) * 10 + 1);
                break;
            case 22:
                data_warehouse.note_timing_lane4.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane4.Add((int)(T_N_timng * 1000) * 10 + 2);
                break;
            //レーン5
            case 23:
                data_warehouse.note_timing_lane5.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane5.Add((int)(T_N_timng * 1000) * 10 + 1);
                break;
            case 24:
                data_warehouse.note_timing_lane5.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane5.Add((int)(T_N_timng * 1000) * 10 + 2);
                break;

            //レーン1 ホールド
            case 51:
                data_warehouse.note_timing_lane1.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane1.Add((int)(T_N_timng * 1000) * 10 + 3);
                break;

            case 53:
                data_warehouse.note_timing_lane2.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane2.Add((int)(T_N_timng * 1000) * 10 + 3);
                break;


            case 55:
                data_warehouse.note_timing_lane3.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane3.Add((int)(T_N_timng * 1000) * 10 + 3);
                break;

            case 61:
                data_warehouse.note_timing_lane4.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane4.Add((int)(T_N_timng * 1000) * 10 + 3);
                break;

            case 63:
                data_warehouse.note_timing_lane5.Add((int)(T_N_timng * 1000));
                data_warehouse.note_option_lane5.Add((int)(T_N_timng * 1000) * 10 + 3);
                break;

        }
    }


    void last_process()
    {
        //ソート
        data_warehouse.note_timing_lane1.Sort();
        data_warehouse.note_option_lane1.Sort();
        data_warehouse.note_timing_lane2.Sort();
        data_warehouse.note_option_lane2.Sort();
        data_warehouse.note_timing_lane3.Sort();
        data_warehouse.note_option_lane3.Sort();
        data_warehouse.note_timing_lane4.Sort();
        data_warehouse.note_option_lane4.Sort();
        data_warehouse.note_timing_lane5.Sort();
        data_warehouse.note_option_lane5.Sort();

        //オプションの切り出し、再格納
        for (int i = 0; i < data_warehouse.note_option_lane1.Count; i++)
        {
            data_warehouse.note_option_lane1[i] = data_warehouse.note_option_lane1[i] % 10;
        }
        for (int i = 0; i < data_warehouse.note_option_lane2.Count; i++)
        {
            data_warehouse.note_option_lane2[i] = data_warehouse.note_option_lane2[i] % 10;
        }
        for (int i = 0; i < data_warehouse.note_option_lane3.Count; i++)
        {
            data_warehouse.note_option_lane3[i] = data_warehouse.note_option_lane3[i] % 10;
        }
        for (int i = 0; i < data_warehouse.note_option_lane4.Count; i++)
        {
            data_warehouse.note_option_lane4[i] = data_warehouse.note_option_lane4[i] % 10;
        }
        for (int i = 0; i < data_warehouse.note_option_lane5.Count; i++)
        {
            data_warehouse.note_option_lane5[i] = data_warehouse.note_option_lane5[i] % 10;
        }

    }


    void filepath_decide()//ファイルパスを決定する
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (AndroidJavaObject externalFilesDir = currentActivity.Call<AndroidJavaObject>("getExternalFilesDir", null))
                    {
                        filepath = externalFilesDir.Call<string>("getCanonicalPath") + "/Songs/Winter,again/Winter,again10key.bms";
                    }//後で/songs/以下をロード結果無いし指定で変化させる
                }
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            filepath = Application.dataPath + "/Raw/Songs/Winter,again/" + "Winter,again10key.bms";
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            filepath = Application.dataPath + "/" + "Songs/" + "Winter,again/" + "Winter,again10key.bms";
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            filepath = Application.dataPath + "/" + "Songs/" + "Winter,again/" + "Winter,again10key.bms";
        }

    }


    
    void OnGUI()//後で消す
    {
        //GUI.TextArea(new Rect(5, 5, Screen.width, 50), Application.persistentDataPath + "\n"+ Application.dataPath + "\n" + Application.temporaryCachePath + "\n"+Application.streamingAssetsPath);
        GUI.TextArea(new Rect(5, 160, 80, 80), filepath);
        GUI.TextArea(new Rect(5, 130, 80, 80), bms);
    }
    


    void Awake()
    {
        Application.targetFrameRate = 60; //60FPSに設定
        Time.timeScale =0.5f;//ゲーム速度
        Input.simulateMouseWithTouches = false;//タッチでマウスイベントが起こらないように、製作中は必要
    }

}
