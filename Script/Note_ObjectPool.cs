using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Note_ObjectPool : MonoBehaviour
{

    //public Player Player;//Playerスクリプト↓
    [SerializeField]
    public Note_manager Note_manager;//Note_manager

    [SerializeField]
    public data_warehouse data_warehouse;



    //public Note_player Note_player;//Note_playerスクリプト
    //public Hold_Note_player Hold_Note_player;//Hold_Note_player



    /// <summary>
    /// スラッシュノートのスプライト
    /// </summary>
    public Sprite SlashSprite;

    /// <summary>
    ///ノーマルノートのスプライト
    /// </summary>
    public Sprite NormalSprite;



    /// <summary>
    ///現在のスプライト
    /// </summary>
    SpriteRenderer MainSpriteRenderer;




    //ObjectPoolのインスタンス
    private static Note_ObjectPool _instance;
    // シングルトン
    public static Note_ObjectPool instance
    {

        get
        {
            // シーン上からオブジェクトプールを取得して返す
            _instance = FindObjectOfType<Note_ObjectPool>();
            return _instance;


        }
    }


    /// <summary>
    /// noteのprefab
    /// </summary>
    public GameObject NotePrefab;



    /// <summary>
    /// ホールドノートのプレファブ
    /// </summary>
    public GameObject HoldNotePrefab;

    /// <summary>
    /// 新しく作ったときのオブジェクト
    /// </summary>
    GameObject newObj;


    /// <summary>
    /// ノートの初期位置
    /// </summary>
    Vector3 Note_Pos;


    /// <summary>
    /// 
    /// </summary>
    Vector3 Hold_note_size = new Vector3(1, 50);


    /// <summary>
    /// ノートの初期角度
    /// </summary>
    Quaternion originalRotation;

    [SerializeField]
    private List<GameObject> pooledNoteObjects;
    [SerializeField]
    private List<GameObject> pooledHoleNoteObjects;


    /// <summary>
    /// ノートに付随した Note_playerスクリプトを指定
    /// </summary>
    Note_player Note_player;

    /// <summary>
    /// 
    /// </summary>
    Hold_Note_player Hold_Note_player;



    /// <summary>
    /// ノートの番号
    /// </summary>
    private int N_NoteNum = 0;


    int lane1_Make_index = 0;
    int lane2_Make_index = 0;
    int lane3_Make_index = 0;
    int lane4_Make_index = 0;
    int lane5_Make_index = 0;





    //float float_time = 2.0f;//テスト用


    void Start()
    {
        //Debug.Log("N_Note_ObjectPool_start");
        // originalPos = Note_controller.transform.position;
        //originalRotation = Note_controller.transform.rotation;
        //originalPos = new Vector3(-7.45f, 7.85f, 0);
        originalRotation = new Quaternion(0, 0, 0, 0);
        //リストを生成
        pooledNoteObjects = new List<GameObject>();
        pooledHoleNoteObjects = new List<GameObject>();
    }







    /// <summary>
    /// ノートを生成
    /// </summary>
    /// <param name="lane">レーン</param>
    /// <param name="type">ノートタイプ</param>
    /// <returns></returns>
    public GameObject Note_Make(int lane, int type, float float_time)
    {

        Note_Pos = Note_Pos_decide(lane);//位置を設定
        //Debug.Log("gatnote");
        //poolされたゲームオブジェクトで使用出来るもの(非アクティブ)がある場合
        //位置、角度を初期化、オブジェクトを有効化して返す
        foreach (GameObject obj in pooledNoteObjects)
        {
            if (obj.activeInHierarchy == false)//余っている(falseな)ゲームオブジェクトがある
            {

                obj.transform.position = Note_Pos;//位置を反映

                obj.transform.rotation = originalRotation;//角度を設定


                SpriteChange(type, obj);//グラフィックを変える


                Note_player = obj.GetComponent<Note_player>();//Note_playerスクリプトを取得
                Note_player.float_time = float_time;//流す時間を設定

                //Debug.Log("再利用時ポジション " + obj.name + " " + obj.transform.position);
                //アクティブにする
                obj.name = "N_Note" + N_NoteNum.ToString() + "_lane" + lane;
                N_NoteNum++;
                obj.SetActive(true);
                Note_player.Tween.Play();

                //オブジェクトを返す
                //Debug.Log("再利用");

                switch (lane)
                {
                    case 1:
                        data_warehouse.lane1_Makes[lane1_Make_index] = obj;
                        lane1_Make_index++;
                        break;
                    case 2:
                        data_warehouse.lane2_Makes[lane2_Make_index] = obj;
                        lane2_Make_index++;
                        break;
                    case 3:
                        //Player.Create_Notes_lane3.Add(obj);
                        break;
                    case 4:
                        //Player.Create_Notes_lane4.Add(obj);
                        break;
                    case 5:
                        //Player.Create_Notes_lane5.Add(obj);
                        break;
                }

                return obj;
            }
        }

        //使用できるものが無かった場合
        //新たに生成して、リストに追加して返す

        newObj = Instantiate(NotePrefab, Note_Pos, originalRotation);
        Note_player = newObj.GetComponent<Note_player>();//Note_playerスクリプトを取得
        //Note_player.float_time = float_time;//流す時間を設定
        SpriteChange(type, newObj);
        Note_player.Tween.Play();
        //Debug.Log("生成");
        //オブジェクトに番号をつける
        newObj.name = "N_Note" + N_NoteNum.ToString() + "_lane" + lane;
        //Debug.Log("オブジェクト名 " + N_NoteNum);
        N_NoteNum++;
        //リストに追加
        pooledNoteObjects.Add(newObj);




        switch (lane)
        {
            case 1:
                data_warehouse.lane1_Makes[lane1_Make_index] = newObj;
                lane1_Make_index++;
                break;
            case 2:
                data_warehouse.lane2_Makes[lane2_Make_index] = newObj;
                lane2_Make_index++;
                break;
            case 3:
                //Player.Create_Notes_lane3.Add(newObj);
                break;
            case 4:
                //Player.Create_Notes_lane4.Add(newObj);
                break;
            case 5:
                //Player.Create_Notes_lane5.Add(newObj);
                break;
        }

        //オブジェクトを返す
        return newObj;

    }




    /// <summary>
    /// ロングノートを生成
    /// </summary>
    /// <param name="lane"></param>
    /// <param name="option"></param>
    /// <param name="float_time"></param>
    /// <param name="hold_time"></param>
    /// <returns></returns>
    public GameObject Note_Make(int lane, int option, float float_time, int hold_time)//ホールドノートの時
    {

        Note_Pos = Note_Pos_decide(lane);//位置を設定
                                         //Debug.Log("gatnote " + N_NoteNum);

        foreach (GameObject obj in pooledHoleNoteObjects)
        {
            if (obj.activeInHierarchy == false)//ゲームオブジェクトがアクティブでない=使えるのがある
            {
                //位置を反映
                obj.transform.position = Note_Pos;
                //角度を設定
                obj.transform.rotation = originalRotation;
                //長さを設定(見逃すと長さ0になるため))
                obj.transform.localScale = Hold_note_size;
                obj.transform.localScale = Expander((float)hold_time / 100);
                obj.name = "N_Hold_Note" + N_NoteNum.ToString() +"_lane" + lane;
                Hold_Note_player = obj.GetComponent<Hold_Note_player>();//Note_playerスクリプトを取得
                //Hold_Note_player.float_time = float_time;//流す時間を設定
                obj.SetActive(true);
                Hold_Note_player.Moven.Play();
                N_NoteNum++;

                switch (lane)
                {
                    case 1:
                        data_warehouse.lane1_Makes[lane1_Make_index] = obj;
                        lane1_Make_index++;
                        break;
                    case 2:
                        //Player.Create_Notes_lane2.Add(obj);
                        break;
                    case 3:
                        //Player.Create_Notes_lane3.Add(obj);
                        break;
                    case 4:
                        //Player.Create_Notes_lane4.Add(obj);
                        break;
                    case 5:
                        //Player.Create_Notes_lane5.Add(obj);
                        break;
                }

                return obj;
            }
        }
        //使用できるものが無かった場合
        //新たに生成して、リストに追加して返す
        GameObject newObj = Instantiate(HoldNotePrefab, Note_Pos, originalRotation);
        newObj.name = "N_Hold_Note" + N_NoteNum.ToString() + "_lane" + lane;
        //newObj.transform.localScale = expander((float)hold_time / 100);//大きさ変更
        newObj.transform.localScale = Hold_note_size;
        newObj.transform.localScale = Expander((float)hold_time / 100);
        Hold_Note_player = newObj.GetComponent<Hold_Note_player>();//Note_playerスクリプトを取得
        Hold_Note_player.Moven.Play();
        N_NoteNum++;
        //リストに追加
        pooledHoleNoteObjects.Add(newObj);

        switch (lane)
        {
            case 1:
                data_warehouse.lane1_Makes[lane1_Make_index] = newObj;
                lane1_Make_index++;
                break;
            case 2:
                //Player.Create_Notes_lane2.Add(newObj);
                break;
            case 3:
                //Player.Create_Notes_lane3.Add(newObj);
                break;
            case 4:
                //Player.Create_Notes_lane4.Add(newObj);
                break;
            case 5:
                //Player.Create_Notes_lane5.Add(newObj);
                break;
        }
        //オブジェクトを返す
        return newObj;



    }









    //これがDestroyの代わりを果たす
    public void releaseNote(GameObject obj, int option)
    {
        //Debug.Log("破壊オブジェクト " + obj.name);
        //GameObject adb = obj; 動作がおかしかったらこれを使うように

        if (option == 1 || option == 2)
        {
            Note_player = obj.GetComponent<Note_player>();
            Note_player.Tween.Kill();
        }
        else
        {
            // Hold_Note_player = obj.GetComponent<Hold_Note_player>();
            // Hold_Note_player.Short.Kill();
            /*これが無いと、”ホールド中に手を離したときなど、内部的には音符が短くなるアニメーションが起こっているときに
             * 再び同じノーツが再利用されるとアニメーションがそのまま再生されてしまう。ので(見た目上)破壊するときに
             * アニメーションを止めることでおかしくならなくする。
             */


        }

        obj.SetActive(false);

    }



    /// <summary>
    /// 生成した時にノートをどこに置くか決定
    /// </summary>
    /// <param name="lane"></param>
    /// <returns></returns>
    Vector3 Note_Pos_decide(int lane)
    {

        Vector3 Note_P;
        switch (lane)
        {//7.85
            case 1:
                Note_P = new Vector3(-7.45f, 7.85f, 0);//Yはどのレーンでも共通に
                break;
            case 2:
                Note_P = new Vector3(-3.70f, 7.85f, 0);
                break;
            case 3:
                Note_P = new Vector3(0, 7.85f, 0);
                break;
            case 4:
                Note_P = new Vector3(3.75f, 7.85f, 0);
                break;
            case 5:
                Note_P = new Vector3(7.47f, 7.85f, 0);
                break;
            default:
                Note_P = new Vector3(0, 5f, 0);//この場合デフォルトが無いとエラーになる
                break;
        }
        return Note_P;
    }




    /// <summary>
    /// ノートタイプに応じてスプライトを変化させる
    /// </summary>
    void SpriteChange(int type, GameObject obj)
    {
        if (type == 2)//スラッシュ
        {
            MainSpriteRenderer = obj.GetComponent<SpriteRenderer>();//現在のスプライトを取得
            MainSpriteRenderer.sprite = SlashSprite;
            //Debug.Log("スラッシュノート");

        }
        else if (type == 1)//ノーマル
        {
            MainSpriteRenderer = obj.GetComponent<SpriteRenderer>();//現在のスプライトを取得
            MainSpriteRenderer.sprite = NormalSprite;
            //Debug.Log("ノーマルノート");
        }
    }










    /// <summary>
    /// ホールドを秒数に応じた長さに伸縮する
    /// </summary>
    /// <param name="hold_time"></param>
    /// <returns></returns>
      
    Vector3 Expander(float hold_time)
    {
        
        //Debug.Log("expander hold_time " + hold_time) ;
        Vector3 length;
        float x;//ノートを伸ばす倍率
        x = 3 / Note_manager.float_steam_time;//基準秒数より何倍早く流れているのか
        length = new Vector3(1, (17f * x * hold_time), 1);//基準17f、基準からの倍率、ホールド時間
        return length;
        
    }










    //最初のノート生成
    public GameObject Start_Make()
    {
        GameObject newObj = Instantiate(NotePrefab, new Vector3(-7.45f, 7.85f, 0), new Quaternion(0, 0, 0, 0));
        newObj.SetActive(false);
        pooledNoteObjects.Add(newObj);

        return newObj;
    }






}