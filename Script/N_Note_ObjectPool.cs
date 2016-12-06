using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class N_Note_ObjectPool : MonoBehaviour
    {

    public Player Player;//Playerスクリプト

    public Note_player Note_player;//Note_playerスクリプト


    public Sprite SlashSprite;//スラッシュノートのスプライト
    public Sprite NormalSprite;//ノーマルノートのスプライト
    SpriteRenderer MainSpriteRenderer;//現在のスプライト


    //ObjectPoolのインスタンス
    private static N_Note_ObjectPool _instance;
    // シングルトン
    public static N_Note_ObjectPool instance
    {
        
        get
        {
            // シーン上からオブジェクトプールを取得して返す
            _instance = FindObjectOfType<N_Note_ObjectPool>();
            return _instance;
            

        }
    }
    //noteのprefab
    public GameObject NotePrefab;
    //ホールドノート
    public GameObject HoldNotePrefab;

    //玉の初期位置
    Vector3 Note_Pos;

    Vector3 Hold_note_size = new Vector3(1,50);

    //玉の初期角度
    Quaternion originalRotation;

   [SerializeField]  private List<GameObject> pooledNoteObjects;
   [SerializeField]  private List<GameObject> pooledHoleNoteObjects;
    //ボールの番号
    private int N_NoteNum = 0;

    void Start()
    {
        //Debug.Log("N_Note_ObjectPool_start");
        // originalPos = Note_controller.transform.position;
        //originalRotation = Note_controller.transform.rotation;
        //originalPos = new Vector3(-7.45f, 7.85f, 0);
        //originalRotation = new Quaternion(0, 0, 0, 0);
        //リストを生成
         pooledNoteObjects = new List<GameObject>();
         pooledHoleNoteObjects = new List<GameObject>();
    }


    //オブジェクト生成
    public GameObject getNote(int lane, int option)
    {

        Note_Pos = Note_Pos_decide(lane);//位置を設定
        //Debug.Log("gatnote");
        if (option == 3)//ホールドノートの時
        {
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
                    obj.SetActive(true);
                    switch (lane)
                    {
                        case 1:
                            Player.Create_Notes_lane1.Add(obj);
                            break;
                        case 2:
                            Player.Create_Notes_lane2.Add(obj);
                            break;
                        case 3:
                            Player.Create_Notes_lane3.Add(obj);
                            break;
                        case 4:
                            Player.Create_Notes_lane4.Add(obj);
                            break;
                        case 5:
                            Player.Create_Notes_lane5.Add(obj);
                            break;
                    }
                    return obj;
                }
            }
            GameObject newObj = (GameObject)Instantiate(HoldNotePrefab, Note_Pos, originalRotation);
            newObj.name = "N_HoldNote" + N_NoteNum.ToString();
            N_NoteNum++;
            //リストに追加
            pooledHoleNoteObjects.Add(newObj);
            switch (lane)
            {
                case 1:
                    Player.Create_Notes_lane1.Add(newObj);
                    break;
                case 2:
                    Player.Create_Notes_lane2.Add(newObj);
                    break;
                case 3:
                    Player.Create_Notes_lane3.Add(newObj);
                    break;
                case 4:
                    Player.Create_Notes_lane4.Add(newObj);
                    break;
                case 5:
                    Player.Create_Notes_lane5.Add(newObj);
                    break;
            }
            //オブジェクトを返す
            return newObj;
        }
        else//通常、スラッシュの場合
        {
            //poolされたゲームオブジェクトで使用出来るもの(非アクティブ)がある場合
            //位置、角度を初期化、オブジェクトを有効化して返す
            foreach (GameObject obj in pooledNoteObjects)
            {
                if (obj.activeInHierarchy == false)//ゲームオブジェクトがアクティブでない
                {
                    //位置を反映
                    obj.transform.position = Note_Pos;
                    //角度を設定
                    obj.transform.rotation = originalRotation;
                    //スラッシュならグラフィックを変える
                    if (option == 2)
                    {
                        MainSpriteRenderer = obj.GetComponent<SpriteRenderer>();//現在のスプライトを取得
                        MainSpriteRenderer.sprite = SlashSprite;
                        //Debug.Log("スラッシュノート");

                    }
                    else if (option == 1)
                    {
                        MainSpriteRenderer = obj.GetComponent<SpriteRenderer>();//現在のスプライトを取得
                        MainSpriteRenderer.sprite = NormalSprite;
                        //Debug.Log("ノーマルノート");
                    }
                    //Debug.Log("再利用時ポジション " + obj.name + " " + obj.transform.position);
                    //アクティブにする

                    obj.SetActive(true);

                    //オブジェクトを返す
                    //Debug.Log("再利用");
                    switch (lane)
                    {
                        case 1:
                            Player.Create_Notes_lane1.Add(obj);
                            break;
                        case 2:
                            Player.Create_Notes_lane2.Add(obj);
                            break;
                        case 3:
                            Player.Create_Notes_lane3.Add(obj);
                            break;
                        case 4:
                            Player.Create_Notes_lane4.Add(obj);
                            break;
                        case 5:
                            Player.Create_Notes_lane5.Add(obj);
                            break;
                    }
                    return obj;
                }
            }

            //使用できるものが無かった場合
            //新たに生成して、リストに追加して返す
            GameObject newObj = (GameObject)Instantiate(NotePrefab, Note_Pos, originalRotation);
            if (option == 2)
            {
                MainSpriteRenderer = newObj.GetComponent<SpriteRenderer>();//現在のスプライトを取得
                MainSpriteRenderer.sprite = SlashSprite;
                //Debug.Log("スラッシュノート");
            }
            else if (option == 1)
            {
                MainSpriteRenderer = newObj.GetComponent<SpriteRenderer>();//現在のスプライトを取得
                MainSpriteRenderer.sprite = NormalSprite;
                //Debug.Log("ノーマルノート");
            }
            //Debug.Log("生成");
            //オブジェクトに番号をつける
            newObj.name = "N_Note" + N_NoteNum.ToString();
            //Debug.Log("オブジェクト名 " + N_NoteNum);
            N_NoteNum++;
            //リストに追加
            pooledNoteObjects.Add(newObj);
            switch (lane)
            {
                case 1:
                    Player.Create_Notes_lane1.Add(newObj);
                    break;
                case 2:
                    Player.Create_Notes_lane2.Add(newObj);
                    break;
                case 3:
                    Player.Create_Notes_lane3.Add(newObj);
                    break;
                case 4:
                    Player.Create_Notes_lane4.Add(newObj);
                    break;
                case 5:
                    Player.Create_Notes_lane5.Add(newObj);
                    break;
            }
            //オブジェクトを返す
            return newObj;
        }





       


    }











    //これがDestroyの代わりを果たす
    public void releaseNote(GameObject obj,int option)
    {
        //GameObject adb = obj; 動作がおかしかったらこれを使うように
        if (option == 1 || option == 2)
        {
            Note_player = obj.GetComponent<Note_player>();
            Note_player.Tween.Kill();
        }
       
        obj.SetActive(false);
        
    }


    Vector3 Note_Pos_decide(int lane)//生成した時にノートをどこに置くか決定
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

}