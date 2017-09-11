using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class Hold_Note_player : MonoBehaviour {

    public GameObject Note_ObjectPool_OBJ;
    public Note_ObjectPool Note_ObjectPool;

    public GameObject Note_manager_OBJ;
    public Note_manager Note_manager;//ノート速度を決めるために必要

    public Tween Moven;

    public Tween Short;

    public Tween miss_short;

    float hold_base_scale = 17f;

    //デバッグ用。後で消す。
    GameObject text;


    /*
    void Start () {
        Note_ObjectPool_OBJ = GameObject.Find("NoteObjectPool");
        Note_ObjectPool = N_note_OBJ.GetComponent<Note_ObjectPool>();
        
    }
    */

    void OnEnable()
    {
        //PlayerOBJ = GameObject.Find("Player");
        //Note_manager = PlayerOBJ.GetComponent<Player>();
        Debug.Log("hold enale");

        //Moven = transform.DOLocalMoveY(-7.81f, (Note_manager.float_steam_time * 1.1863f)).SetEase(Ease.Linear);
        Moven = transform.DOLocalMoveY(-7.81f, (Note_manager.float_steam_time * 1.18636363636f)).SetEase(Ease.Linear).OnComplete(EndShort);
        //↑画面上端から下端までの距離＝上端からラインまでの距離*1.18...倍
        text = GameObject.Find("Text");
    }


  



    public void Shorten(float HoldTime)
    {
        //Debug.Log(this.gameObject.name + " Shorten  HoldTime " + HoldTime);
        
        Short = transform.DOScaleY(0, HoldTime).SetEase(Ease.Linear).OnComplete(des);
        Short.Play();
        //Debug.Log("shorten");
        text.GetComponent<Text>().text = "Shorten";
    }


    void EndShort()//見逃しmissの時に実行される
    {
        
        Debug.Log("noteplay EndShort ");
        Debug.Log(Note_manager.float_steam_time);
        Debug.Log(transform.localScale.y/(hold_base_scale * (3 / Note_manager.float_steam_time)) );
        miss_short = transform.DOScaleY(0, (transform.localScale.y / (hold_base_scale * (3 / Note_manager.float_steam_time)))).SetEase(Ease.Linear).OnComplete(des);
        miss_short.Play();
        text.GetComponent<Text>().text = "EndShort";
    }


    void des()
    {
        
        text.GetComponent<Text>().text = "des";
        Note_ObjectPool.releaseNote(gameObject,3);
        //Debug.Log("hold des " + gameObject);
    }
}
